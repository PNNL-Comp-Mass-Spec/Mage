using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Collections.ObjectModel;
using PRISM.Logging;


namespace Mage
{

    /// <summary>
    /// Container that builds and operates chain of modules
    /// Allows client to:
    /// - Create processing modules and chain them together
    /// - Set module parameters
    /// - Connect client handlers to pipeline modules
    /// - Run process chain in separate thread
    /// Modules
    /// - Must implement IBaseModule in order to be managed by pipeline
    /// - Can extend BaseModule class (but don’t have to)
    /// - Can be in external DLL and be dynamically loaded
    /// </summary>
    public class ProcessingPipeline
    {
        private static readonly FileLogger traceLogPipeline = new FileLogger(FileLogger.BaseLogFileName, BaseLogger.LogLevels.INFO, FileLogger.AppendDateToBaseFileName);

        /// <summary>
        /// Event that is fired during execution of pipeline
        /// (pass-through for status messages from modules contained in the pipeline)
        /// </summary>
        public event EventHandler<MageStatusEventArgs> OnStatusMessageUpdated;

        /// <summary>
        /// Event that is fired during execution of pipeline
        /// (pass-through for warning messages from modules contained in the pipeline)
        /// </summary>
        public event EventHandler<MageStatusEventArgs> OnWarningMessageUpdated;

        /// <summary>
        /// Event that is fired when pipeline run terminates (normally or abnormally)
        /// </summary>
        public event EventHandler<MageStatusEventArgs> OnRunCompleted;

        #region Member Variables

        /// <summary>
        /// Ordered list of modules managed by this pipeline object
        /// </summary>
        private readonly List<IBaseModule> mModuleList = new List<IBaseModule>();

        /// <summary>
        /// Lookup reference for modules managed by this pipeline object (indexed by module name)
        /// </summary>
        private readonly Dictionary<string, IBaseModule> mModuleIndex = new Dictionary<string, IBaseModule>();

        #endregion

        #region Properties

        /// <summary>
        /// Message available to client after pipeline execution completes.
        /// </summary>
        public string CompletionCode { get; set; }

        /// <summary>
        /// Module whose Run method will be called to begin pipeline execution
        /// </summary>
        public IBaseModule RootModule { get; set; }

        /// <summary>
        /// Is pipeline currently executing or not
        /// </summary>
        public bool Running { get; set; }

        /// <summary>
        /// Arbitrary name for pipeline that client can set (shows up in log entries)
        /// </summary>
        public string PipelineName { get; set; }

        /// <summary>
        /// Return list of module in pipeline
        /// </summary>
        public Collection<IBaseModule> ModuleList => new Collection<IBaseModule>(mModuleList);

        #endregion

        #region Constructors

        /// <summary>
        /// Construct a new Mage pipeline object
        /// </summary>
        /// <param name="name">Name of the pipeline (appears in log messages)</param>
        public ProcessingPipeline(string name)
        {
            PipelineName = name;
            traceLogPipeline.Debug(string.Format("Building pipeline '{0}'", PipelineName));
        }

        #endregion

        #region Private Functions


        #endregion

        #region Functions Available to Clients

        /// <summary>
        /// Invoke the run method on the root module of pipeline in separate thread from thread pool
        /// </summary>
        public void Run()
        {
            // Fire off the pipeline
            ThreadPool.QueueUserWorkItem(RunRoot);
        }

        /// <summary>
        /// Call the Run method on the root module of pipeline (execution will be in caller's thread)
        /// </summary>
        /// <param name="state">Provided so that this function has necessary signature to be target of ThreadPool.QueueUserWorkItem</param>
        public void RunRoot(object state)
        {
            var bError = false;

            Running = true;
            CompletionCode = "";
            HandleStatusMessageUpdated(this, new MageStatusEventArgs("Running..."));
            traceLogPipeline.Info(string.Format("Pipeline {0} started...", PipelineName));

            // Give all modules in pipeline a chance to prepare themselves
            foreach (var modDef in mModuleIndex)
            {
                try
                {
                    modDef.Value.Prepare();
                }
                catch (MageException e)
                {
                    CompletionCode = e.Message;
                    HandleStatusMessageUpdated(this, new MageStatusEventArgs(e.Message, 2));
                    var msg = string.Format("Pipeline {0} failed: {1}", PipelineName, e.Message);
                    traceLogPipeline.Error(msg);
                    HandleWarningMessageUpdated(this, new MageStatusEventArgs(msg));
                    bError = true;
                }
            }

            if (!bError)
            {

                RootModule.Run(this);

                RunPostProcess();
            }

            // Give all modules in pipeline a chance to clean up after themselves
            foreach (var modDef in mModuleIndex)
            {
                modDef.Value.Cleanup();
            }

            Running = false;
            OnRunCompleted?.Invoke(this, new MageStatusEventArgs(CompletionCode));
        }

        private void RunPostProcess()
        {
            try
            {
                var processingError = false;

                // Give all modules in pipeline a chance to run post-processing
                // In particular, FileProcessingBase will call m_MyEMSLDatasetInfoCache.ProcessDownloadQueue
                foreach (var modDef in mModuleIndex)
                {
                    var postProcessSuccess = modDef.Value.PostProcess();

                    if (!postProcessSuccess)
                    {
                        processingError = true;
                    }
                }

                if (processingError)
                {
                    throw new MageException("Post processing error; see warnings for details");
                }

                if (string.IsNullOrEmpty(CompletionCode))
                {
                    if (Globals.AbortRequested)
                    {
                        HandleStatusMessageUpdated(this, new MageStatusEventArgs("Processing Aborted"));
                        traceLogPipeline.Info(string.Format("Pipeline {0} aborted...", PipelineName));
                    }
                    else
                    {
                        HandleStatusMessageUpdated(this, new MageStatusEventArgs("Process Complete"));
                        traceLogPipeline.Info(string.Format("Pipeline {0} completed...", PipelineName));
                    }
                }
                else
                {
                    HandleStatusMessageUpdated(this, new MageStatusEventArgs(CompletionCode, 1));
                    traceLogPipeline.Info(string.Format("Pipeline {0} completed... " + CompletionCode, PipelineName));
                }

            }
            catch (MageException e)
            {
                CompletionCode = e.Message;
                HandleStatusMessageUpdated(this, new MageStatusEventArgs(e.Message, 2));
                traceLogPipeline.Error(string.Format("Pipeline {0} failed: {1}", PipelineName, e.Message));
            }
            catch (NotImplementedException e)
            {
                CompletionCode = e.Message;
                HandleStatusMessageUpdated(this, new MageStatusEventArgs(e.Message, 2));
                traceLogPipeline.Error(string.Format("Pipeline {0} failed: {1}", PipelineName, e.Message));
            }
        }

        /// <summary>
        /// Terminate execution of pipeline
        /// </summary>
        public void Cancel()
        {
            CompletionCode = "Processing aborted";
            foreach (var modDef in mModuleIndex)
            {
                modDef.Value.Cancel();
            }
        }
        /// <summary>
        /// Set parameters for given module for a list of parameters
        /// </summary>
        /// <param name="moduleName">The name of the module to set parameters for</param>
        /// <param name="moduleParams">Key/value list of parameters</param>
        public void SetModuleParameters(string moduleName, Dictionary<string, string> moduleParams)
        {
            // Get reference to module by name and send it the list of parameters
            var mod = mModuleIndex[moduleName];
            if (mod != null)
            {
                mod.SetParameters(moduleParams);
            }
            else
            {
                traceLogPipeline.Error(string.Format("Could not find module '{0}' to set parameters ({1})", moduleName, PipelineName));
            }
        }

        /// <summary>
        /// Set a single parameter for a given module
        /// </summary>
        /// <param name="moduleName">The name of the module to set parameters for</param>
        /// <param name="paramName">Key</param>
        /// <param name="paramValue">Value</param>
        public void SetModuleParameter(string moduleName, string paramName, string paramValue)
        {
            // Get reference to module by name, package the parameter, and send it to the module
            var mod = mModuleIndex[moduleName];
            if (mod != null)
            {
                mod.SetPropertyByName(paramName, paramValue);
            }
            else
            {
                traceLogPipeline.Error(string.Format("Could not find module '{0}' to set parameter '{1}'.", moduleName, paramName));
            }
        }

        /// <summary>
        /// Connect the standard tabular output events (column definition and row data)
        /// of the upstream module to the standard tabular input handlers of the downstream module
        /// </summary>
        /// <param name="upstreamModule">Module name of upstream module</param>
        /// <param name="downstreamModule">Module name of downstream module</param>
        public void ConnectModules(string upstreamModule, string downstreamModule)
        {
            // Get reference to both the upstream and downstream modules by name
            // and wire the downstream module's pipeline event handlers to the upstream module's pipeline events
            try
            {
                var modUp = mModuleIndex[upstreamModule];
                var modDn = mModuleIndex[downstreamModule];
                ConnectModules(modUp, modDn);
            }
            catch (Exception e)
            {
                var msg = string.Format("Failed to connect module '{0}' to module '{1} ({2}): {3}", downstreamModule, upstreamModule, PipelineName, e.Message);
                traceLogPipeline.Error(msg);
                throw new MageException(msg);
            }
        }

        /// <summary>
        /// Connect the standard tabular output events (column definition and row data)
        /// of the upstream module to the standard tabular input handlers of the downstream module
        /// </summary>
        /// <param name="modUp">Upstream module</param>
        /// <param name="modDn">Downstream module</param>
        private void ConnectModules(IBaseModule modUp, IBaseModule modDn)
        {
            modUp.ColumnDefAvailable += modDn.HandleColumnDef;
            modUp.DataRowAvailable += modDn.HandleDataRow;
            traceLogPipeline.Debug(string.Format("Connected input of module '{0}' to output of module '{1} ({2})", modDn.ModuleName, modUp.ModuleName, PipelineName));
        }

        /// <summary>
        /// Connect the standard tabular input event handler delegates
        /// to the pipeline module identified by name
        /// </summary>
        /// <param name="moduleName">Module name of the upstream module</param>
        /// <param name="colHandler">Delegate function to receive column definition events</param>
        /// <param name="rowHandler">Delegate function to receive row data events</param>
        public void ConnectExternalModule(string moduleName, EventHandler<MageColumnEventArgs> colHandler, EventHandler<MageDataEventArgs> rowHandler)
        {
            var mod = mModuleIndex[moduleName];
            ConnectExternalModule(mod, colHandler, rowHandler);
        }

        /// <summary>
        /// Connect the standard tabular input event handler delegates
        /// to the given module
        /// </summary>
        /// <param name="mod">Module</param>
        /// <param name="colHandler">Delegate function to receive column definition events</param>
        /// <param name="rowHandler">Delegate function to receive row data events</param>
        private void ConnectExternalModule(IBaseModule mod, EventHandler<MageColumnEventArgs> colHandler, EventHandler<MageDataEventArgs> rowHandler)
        {
            if (mod != null)
            {
                mod.ColumnDefAvailable += colHandler;
                mod.DataRowAvailable += rowHandler;
                traceLogPipeline.Debug(string.Format("Connected external handler to module '{0}' ({1})", mod.ModuleName, PipelineName));
            }
            else
            {
                traceLogPipeline.Error(string.Format("Could not connect handler module to module '{0}' ({1})", "Null module", PipelineName));
            }
        }

        /// <summary>
        /// Connect the given ISinkModule object
        /// to the pipeline object identified by name
        /// </summary>
        /// <param name="moduleName">Module name of the upstream module</param>
        /// <param name="sink">Object to be connected</param>
        public void ConnectExternalModule(string moduleName, ISinkModule sink)
        {
            ConnectExternalModule(moduleName, sink.HandleColumnDef, sink.HandleDataRow);
        }

        /// <summary>
        /// Connect the given ISinkModule object
        /// to the last module in the pipeline
        /// </summary>
        /// <param name="sink">O</param>
        public void ConnectExternalModule(ISinkModule sink)
        {
            var mod = mModuleList.Last();
            ConnectExternalModule(mod, sink.HandleColumnDef, sink.HandleDataRow);
        }

        /// <summary>
        /// Connect given module to last module currently in pipeline
        /// and add the module to the pipeline
        /// </summary>
        /// <param name="mod"></param>
        public void AppendModule(IBaseModule mod)
        {
            ConnectExternalModule(mod);
            var name = (!string.IsNullOrEmpty(mod.ModuleName)) ? mod.ModuleName : string.Format("Module{0}", mModuleList.Count + 1);
            AddModule(name, mod);
        }

        /// <summary>
        /// Create a new module of class named by moduleType
        /// and set its name as given by moduleName, and add it to pipeline
        /// </summary>
        /// <param name="moduleName">Arbitrary name to set for new module</param>
        /// <param name="moduleType">name of class to instantiate the module from</param>
        /// <returns>Module</returns>
        public IBaseModule MakeModule(string moduleName, string moduleType)
        {
            var className = moduleType;
            try
            {
                var module = MakeModule(className);
                if (module == null)
                {
                    throw new MageException("Class not found in searched assemblies");
                }
                return AddModule(moduleName, module);
            }
            catch (Exception e)
            {
                var msg = "Module '" + moduleName + ":" + moduleType + "' could not be created - " + e.Message;
                traceLogPipeline.Error(msg);
                throw new MageException(msg);
            }
        }

        /// <summary>
        /// Create module from class name
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static IBaseModule MakeModule(string className)
        {
            IBaseModule module = null;
            var modType = ModuleDiscovery.GetModuleTypeFromClassName(className);
            if (modType != null)
            {
                module = (IBaseModule)Activator.CreateInstance(modType);
            }
            return module;
        }

        /// <summary>
        /// Add the module object to the pipeline and give it the specified module name
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <param name="module">Module object</param>
        /// <returns>Reference to the module</returns>
        public IBaseModule AddModule(string moduleName, IBaseModule module)
        {
            var modDef = new ModuleDef(moduleName, module);
            return AddModule(modDef);
        }

        /// <summary>
        /// Add the module object to the pipeline
        /// </summary>
        /// <param name="modDef"></param>
        /// <returns></returns>
        public IBaseModule AddModule(ModuleDef modDef)
        {
            if (modDef == null)
            {
                throw new MageException("Null module definition sent to AddModule (modDef is null)");
            }

            if (modDef.ModuleObject == null)
            {
                throw new MageException("Null module sent to AddModule (modDef.ModuleObject is null)");
            }

            if (!(modDef.ModuleObject is IBaseModule module))
            {
                throw new MageException("Module is not of type IBaseModule; cannot add " + modDef.ModuleObject);
            }

            mModuleList.Add(module);
            mModuleIndex.Add(modDef.ModuleName, module);
            module.StatusMessageUpdated += HandleStatusMessageUpdated;
            module.WarningMessageUpdated += HandleWarningMessageUpdated;
            traceLogPipeline.Debug(string.Format("Added module '{0}' ({1})", modDef.ModuleName, PipelineName));
            return module;
        }

        /// <summary>
        /// Return a reference to the given module
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <returns></returns>
        public IBaseModule GetModule(string moduleName)
        {
            return mModuleIndex[moduleName];
        }

        #endregion

        #region Error Messages

        /// <summary>
        /// Buffer to accumulate error messages from status update stream
        /// </summary>
        private readonly List<string> mErrorMessages = new List<string>();

        /// <summary>
        /// Get error messages
        /// </summary>
        public Collection<string> ErrorMessages => new Collection<string>(mErrorMessages);

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handler to receive OnStatusMessageUpdated events from modules in the pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandleStatusMessageUpdated(object sender, MageStatusEventArgs args)
        {
            if (args.ErrorCode > 0)
            {
                mErrorMessages.Add(args.Message);
            }
            OnStatusMessageUpdated?.Invoke(this, args);
        }

        /// <summary>
        /// Handler to receive OnWarningMessageUpdated events from modules in the pipeline
        /// </summary>
        /// <param name="sender">(ignored)</param>
        /// <param name="args">Contains status information to be displayed</param>
        private void HandleWarningMessageUpdated(object sender, MageStatusEventArgs args)
        {
            if (args.ErrorCode > 0)
            {
                mErrorMessages.Add(args.Message);
            }
            OnWarningMessageUpdated?.Invoke(this, args);
        }

        #endregion

        #region Build Pipeline From XML definitions

        /// <summary>
        /// Construct a pipeline from an XML description
        /// </summary>
        /// <param name="pipelineSpec">Path to the XML definition file</param>
        public void Build(string pipelineSpec)
        {
            // Step through XML module specification document
            // and build and wire modules as specified
            //

            // pipelineSpec = "<root>" + pipelineSpec + "</root>";
            var doc = new XmlDocument();
            doc.LoadXml(pipelineSpec);
            var xnl = doc.SelectNodes(".//module");

            // Get next module description from specification
            if (xnl == null)
            {
                return;
            }

            foreach (XmlNode n in xnl)
            {
                if (n.Attributes == null)
                {
                    continue;
                }

                var moduleName = n.Attributes["name"].InnerText;
                var moduleType = n.Attributes["type"].InnerText;

                // Create the module
                var mod = MakeModule(moduleName, moduleType);

                // Wire it to an upstream module, if required
                XmlNode cn = n.Attributes["connectedTo"];
                if (cn != null)
                {
                    var connectedTo = cn.InnerText;
                    ConnectModules(connectedTo, moduleName);
                }
                else
                {
                    // Module with no upstream module is assumed to be the root of the pipeline
                    // (we play by Highlander rules - there can be only one)
                    RootModule = mod;
                }
            }
        }

        /// <summary>
        /// Set the parameters for all the modules in the pipeline from an XML definition file
        /// </summary>
        /// <param name="pipelineModuleParams">Path to the XML definition file</param>
        public void SetAllModuleParameters(string pipelineModuleParams)
        {
            // Step though XML document that defines parameters for modules.
            // For each module in the document, extract a key/value list of parameters
            // and send them to the module

            // Parse the XML definition of the module parameters
            pipelineModuleParams = "<root>" + pipelineModuleParams + "</root>";
            var doc = new XmlDocument();
            doc.LoadXml(pipelineModuleParams);

            // Step through list of module sections in specification
            var xnl = doc.SelectNodes(".//module");

            // Do section for current module in specification
            if (xnl == null)
            {
                return;
            }

            foreach (XmlNode modNode in xnl)
            {
                // Get the name of the module that the parameters belong to
                if (modNode.Attributes == null)
                {
                    continue;
                }

                var moduleName = modNode.Attributes["name"].InnerText;

                // Build list of parameters for the module
                var moduleParams = new Dictionary<string, string>();
                foreach (XmlNode paramNode in modNode.ChildNodes)
                {
                    if (paramNode.Attributes != null)
                    {
                        var paramName = paramNode.Attributes["name"].InnerText;
                        var paramVal = paramNode.InnerText;
                        moduleParams.Add(paramName, paramVal);
                    }
                }

                // Send list of parameters to module
                SetModuleParameters(moduleName, moduleParams);
            }
        }
        #endregion

        #region Assemble Common Pipeline Configurations

        /// <summary>
        /// Assemble a linear processing pipeline from a list of modules.
        /// Modules will added and interconnected in the order in which they
        /// appear in the list.  The first module is assumed to be the root module.
        /// Modules that implement ISinkModule, but not IBaseModule, are connected
        /// as external modules.  Module names are generated automatically
        /// </summary>
        /// <param name="name">Name of the pipeline</param>
        /// <param name="moduleList">List of modules to build pipeline from</param>
        /// <returns>Pipeline</returns>
        public static ProcessingPipeline Assemble(string name, IEnumerable<object> moduleList)
        {
            var namedModuleList = new Collection<ModuleDef>();
            var seq = 0;
            foreach (var moduleObject in moduleList)
            {
                string moduleName;
                if (moduleObject is string)
                {
                    moduleName = moduleObject as string + (++seq);
                }
                else
                {
                    moduleName = moduleObject.GetType().Name + (++seq);
                }
                namedModuleList.Add(new ModuleDef(moduleName, moduleObject));
            }
            return Assemble(name, namedModuleList);
        }

        /// <summary>
        /// Assemble a linear processing pipeline from a list of modules.
        /// Modules will added and interconnected in the order in which they
        /// appear in the list.  The first module is assumed to be the root module.
        /// Modules that implement ISinkModule, but not IBaseModule, are connected
        /// as external modules.
        /// </summary>
        /// <param name="name">Name of the pipeline</param>
        /// <param name="namedModuleList">List of modules to add, with name of module</param>
        /// <returns>Pipeline</returns>
        public static ProcessingPipeline Assemble(string name, IEnumerable<ModuleDef> namedModuleList)
        {
            var pipeline = new ProcessingPipeline(name);

            var currentModName = "";
            foreach (var mod in namedModuleList)
            {
                var precedingModName = currentModName;
                currentModName = mod.ModuleName;

                var moduleObject = mod.ModuleObject;
                if (moduleObject is string)
                {
                    moduleObject = MakeModule(moduleObject as string);
                }
                if (moduleObject is IBaseModule)
                {
                    pipeline.AddModule(currentModName, moduleObject as IBaseModule);
                    if (pipeline.RootModule == null)
                    {
                        pipeline.RootModule = moduleObject as IBaseModule;
                    }
                    if (!string.IsNullOrEmpty(precedingModName))
                    {
                        pipeline.ConnectModules(precedingModName, currentModName);
                    }
                }
                else
                {
                    if (moduleObject is ISinkModule && !string.IsNullOrEmpty(precedingModName))
                    {
                        pipeline.ConnectExternalModule(precedingModName, moduleObject as ISinkModule);
                    }
                }
            }
            return pipeline;
        }

        /// <summary>
        /// Assemble a linear processing pipeline from a list of modules.
        /// Modules will be added and interconnected in the order in which they
        /// appear in the list.  The first module is assumed to be the root module.
        /// Modules that implement ISinkModule, but not IBaseModule, are connected
        /// as external modules.  Module names are generated automatically
        /// </summary>
        /// <param name="name">Name of the pipeline</param>
        /// <param name="moduleList">comma-delimited list of modules as arguments</param>
        /// <returns></returns>
        public static ProcessingPipeline Assemble(string name, params object[] moduleList)
        {
            var moduleCollection = new Collection<object>(moduleList);
            return Assemble(name, moduleCollection);
        }

        /// <summary>
        /// Assemble a linear processing pipeline from an XML spec
        /// </summary>
        /// <param name="pipelineSpecXML"></param>
        /// <returns></returns>
        public static ProcessingPipeline Assemble(string pipelineSpecXML)
        {
            var doc = new XmlDocument();
            doc.LoadXml(pipelineSpecXML);
            var xnl = doc.SelectNodes(".//module");

            var p = doc.SelectSingleNode("/pipeline");
            if (p?.Attributes == null)
            {
                throw new NullReferenceException("pipeline XML does not contain node 'pipeline'; cannot assemble");
            }

            var pipelineName = p.Attributes["name"].InnerText;

            var namedModuleList = new Collection<ModuleDef>();
            if (xnl == null)
            {
                return Assemble(pipelineName, namedModuleList);
            }

            foreach (XmlNode n in xnl)
            {
                if (n?.Attributes == null)
                {
                    throw new NullReferenceException("pipeline XML node does not contain attribute; cannot assemble");
                }

                // Create the module
                var nameAttr = n.Attributes["name"];

                var moduleName = nameAttr?.InnerText ?? string.Format("Module{0}", namedModuleList.Count + 1);
                var moduleType = n.Attributes["type"].InnerText;
                var mod = MakeModule(moduleType);

                var pnl = n.SelectNodes(".//param");
                if (pnl != null)
                {
                    foreach (XmlNode paramNode in pnl)
                    {
                        if (paramNode.Attributes == null)
                            continue;

                        var paramName = paramNode.Attributes["name"].InnerText;
                        var paramVal = paramNode.InnerText;
                        mod.SetPropertyByName(paramName, paramVal);
                    }
                }
                namedModuleList.Add(new ModuleDef(moduleName, mod));
            }
            return Assemble(pipelineName, namedModuleList);
        }

        #endregion
    }

    /// <summary>
    /// Class that encapsulates a minimum definition of a pipeline
    /// module for pipeline creation.
    ///
    /// It contains a reference to a pipeline module and the
    /// name that the module will have in the pipeline
    /// </summary>
    public class ModuleDef
    {

        /// <summary>
        /// Pipeline name of Mage module object
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Mage module object wrapped by this object
        /// </summary>
        public object ModuleObject { get; set; }

        /// <summary>
        /// Construct a new ModuleDef object
        /// from given module with given module name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="module"></param>
        public ModuleDef(string name, object module)
        {
            ModuleName = name;
            ModuleObject = module;
        }
    }

}

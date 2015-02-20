using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mage;
using MageExtExtractionFilters;
using System.Collections.ObjectModel;
using System.IO;

namespace MageExtExtractionFilters
{

	/// <summary>
	/// Builds Mage piplines and pipeline queues for extracting DMS analsis job
	/// results file content
	/// </summary>
	public class ExtractionPipelines
	{


		public static PipelineQueue MakePipelineQueueToExtractFromFileList(SimpleSink fileList, ExtractionType extractionParms, DestinationType destination)
		{
			PipelineQueue pipelineQueue = new PipelineQueue();

			ProcessingPipeline pxfl = MakePipelineToExportFileList(new MyEMSLSinkWrapper(fileList), destination);
			pipelineQueue.Add(pxfl);

			// extract contents of list of files 
			ProcessingPipeline pefc = MakePipelineToExtractFileContents(new SinkWrapper(fileList), extractionParms, destination);
			pipelineQueue.Add(pefc);

			return pipelineQueue;
		}

		/// <summary>
		/// Build and return Mage pipeline queue to extract contents of results files
		/// for jobs given in jobList according to parameters defined in mExtractionParms 
		/// and deliver output according to mDestination.  Also create metadata file for jobList.
		/// </summary>
		/// <param name="mode"></param>
		public static PipelineQueue MakePipelineQueueToExtractFromJobList(BaseModule jobList, ExtractionType extractionParms, DestinationType destination)
		{
			PipelineQueue pipelineQueue = new PipelineQueue();

			ProcessingPipeline pxjm = MakePipelineToExportJobMetadata(jobList, destination);
			pipelineQueue.Add(pxjm);

			// buffer module to accumulate aggregated file list
			SimpleSink fileList = new SimpleSink();
			
			// search job results folders for list of results files to process
			// and accumulate into buffer module
			ProcessingPipeline plof = MakePipelineToGetListOfFiles(jobList, fileList, extractionParms);
			pipelineQueue.Add(plof);

			ProcessingPipeline pxfl = MakePipelineToExportFileList(new MyEMSLSinkWrapper(fileList), destination);
			pipelineQueue.Add(pxfl);

			// extract contents of list of files 
			ProcessingPipeline pefc = MakePipelineToExtractFileContents(new SinkWrapper(fileList), extractionParms, destination);
			pipelineQueue.Add(pefc);

			return pipelineQueue;
		}

		/// <summary>
		/// Make a Mage pipeline to get list of files for list of jobs 
		/// containted in given source module and accumulate
		/// into given sink module
		/// </summary>
		/// <param name="source">Mage module that contains list of jobs to search</param>
		/// <param name="sink">Mage module to accumulate list of results (and MSGF) files into</param>
		/// <returns></returns>
		public static ProcessingPipeline MakePipelineToGetListOfFiles(BaseModule jobListSource, BaseModule fileListSink, ExtractionType extractionParms)
		{
			Collection<object> modules = new Collection<object>();
			modules.Add(jobListSource);

			FileListFilter fileFinder = new FileListFilter();
			fileFinder.IncludeFilesOrFolders = "File";
			fileFinder.FileSelectorMode = "RegEx";
			fileFinder.FileColumnName = "Name";
			fileFinder.FileNameSelector = extractionParms.RType.ResultsFileNamePattern;
			fileFinder.OutputColumnList = string.Format("Job, {0}|+|text, Folder, {1}|+|text, {2}|+|text, {3}|+|text", FileListFilter.COLUMN_NAME_FILE_TYPE, fileFinder.FileColumnName, FileListFilter.COLUMN_NAME_FILE_SIZE, FileListFilter.COLUMN_NAME_FILE_DATE);

			modules.Add(fileFinder);

			foreach (ResultType.MergeFile mf in extractionParms.RType.MergeFileTypes)
			{
				AddAssociatedFile assocFinder = new AddAssociatedFile();
				assocFinder.AssocFileNameReplacementPattern = string.Format("{0}|{1}", extractionParms.RType.ResultsFileNamePattern, mf.FileNameTag);
				assocFinder.ColumnToReceiveAssocFileName = mf.NameColumn;
				fileFinder.OutputColumnList += string.Format(", {0}|+|text", mf.NameColumn);
				modules.Add(assocFinder);
			}

			modules.Add(fileListSink);

			ProcessingPipeline filePipeline = ProcessingPipeline.Assemble("Search For Files", modules);
			return filePipeline;
		}

		/// <summary>
		/// make pipeline to extract contents of files given in list
		/// and add it to the queue
		/// </summary>
		public static ProcessingPipeline MakePipelineToExtractFileContents(BaseModule fileListSource, ExtractionType extractionParms, DestinationType destination)
		{
			FileContentExtractor extractor = new FileContentExtractor();
			extractor.ExtractionParms = extractionParms;
			extractor.Destination = destination;
			extractor.SourceFolderColumnName = "Folder";
			extractor.SourceFileColumnName = "Name";

			ProcessingPipeline filePipeline = ProcessingPipeline.Assemble("Process Files", fileListSource, extractor);
			return filePipeline;
		}

		/// <summary>
		///  Check that the tool type for jobs selected for extraction
		///  are correct and consistent with extraction type
		/// </summary>
		/// <param name="jobList"></param>
		/// <returns></returns>
		public static string CheckJobResultType(BaseModule jobList, string toolCol, ExtractionType extractionParams)
		{
			string msg = "";
			SimpleSink toolList = new SimpleSink();
			ProcessingPipeline.Assemble("GetToolList", jobList, toolList).RunRoot(null);
			Dictionary<string, int> tools = new Dictionary<string, int>();
			foreach (string[] row in toolList.Rows)
			{
				int idx = toolList.ColumnIndex[toolCol];
				string tool = GetBaseTool(row[idx]);
				if (tools.ContainsKey(tool))
				{
					tools[tool] = tools[tool] + 1;
				}
				else
				{
					tools[tool] = 1;
				}
			}
			if (tools.Count > 1)
			{
				string toolRollup = "(" + string.Join(", ", tools.Keys) + ")";
				msg = "Cannot work on a mix of job types " + toolRollup + Environment.NewLine + "Select only Sequest, X!Tandem, Inspect, or MSGFPlus jobs";
			}
			else
			{
				string f = extractionParams.RType.Filter;
				string t = tools.Keys.ElementAt(0);
				if (f.ToLower() != t.ToLower())
				{
					bool bInvalid = true;

					if (t.ToLower() == "msgfplus" && f.ToLower().StartsWith("msgfplus"))
					{
						// MSGF+ results can be processed by two different extractors: msgfdb and msgfdbFHT
						bInvalid = false;
					}

					if (bInvalid)
						msg = string.Format("Result type chosen for extraction ({0}) is not correct for selected jobs ({1})", f, t);
				}
			}
			return msg;
		}

		/// <summary>
		/// Get base tool name from raw tool name.
		/// Uses hueristic that all common tools begin 
		/// with the same prefix set off by underscore character.
		/// </summary>
		/// <param name="rawTool"></param>
		/// <returns></returns>
		public static string GetBaseTool(string rawTool)
		{
			string tool = rawTool.ToLower();
			int idx = rawTool.IndexOf('_');
			if (idx >= 0)
			{
				tool = tool.Substring(0, idx);
			}
			return tool;
		}

		protected static string GetContainerDirectory(DestinationType destination)
		{
			string containerPath = destination.ContainerPath;
			if (containerPath.ToLower().EndsWith(".db3"))
			{
				var containerFile = new FileInfo(containerPath);
				containerPath = containerFile.Directory.FullName;
			}
			return containerPath;
		}	

		/// <summary>
		/// Make a Mage pipeline to dump contents of job list 
		/// as metadata file or db table
		/// </summary>
		/// <param name="jobList"></param>
		/// <returns></returns>
		public static ProcessingPipeline MakePipelineToExportJobMetadata(BaseModule jobList, DestinationType destination)
		{
			BaseModule writer = null;
			switch (destination.Type)
			{
				case DestinationType.Types.SQLite_Output:
					SQLiteWriter sw = new SQLiteWriter();
					sw.DbPath = destination.ContainerPath;
					sw.TableName = destination.MetadataName;
					writer = sw;
					break;
				case DestinationType.Types.File_Output:
					DelimitedFileWriter dw = new DelimitedFileWriter();
					dw.FilePath = Path.Combine(destination.ContainerPath, destination.MetadataName);
					writer = dw;
					break;
			}
			ProcessingPipeline filePipeline = ProcessingPipeline.Assemble("Job Metadata", jobList, writer);
			return filePipeline;
		}

		/// <summary>
		/// Make a Mage pipeline to dump contents of file list 
		/// as file or db table
		/// </summary>
		/// <param name="fileList"></param>
		/// <returns></returns>
		public static ProcessingPipeline MakePipelineToExportFileList(MyEMSLSinkWrapper fileList, DestinationType destination)
		{
			BaseModule writer = null;
			switch (destination.Type)
			{
				case DestinationType.Types.SQLite_Output:
					SQLiteWriter sw = new SQLiteWriter();
					sw.DbPath = destination.ContainerPath;
					sw.TableName = destination.FileListName;
					writer = sw;
					break;
				case DestinationType.Types.File_Output:
					DelimitedFileWriter dw = new DelimitedFileWriter();
					dw.FilePath = Path.Combine(destination.ContainerPath, destination.FileListName);
					writer = dw;
					break;
			}

			fileList.TempFilesContainerPath = GetContainerDirectory(destination);
			fileList.PredownloadMyEMSLFiles = true;

			ProcessingPipeline filePipeline = ProcessingPipeline.Assemble("Job Metadata", fileList, writer);
			return filePipeline;
		}

	}
}

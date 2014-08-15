using System;
using System.Collections.Generic;
using System.Threading;

namespace Mage {

    /// <summary>
    /// Runs one or more Mage pipelines from a queue
    /// in a worker thread.
    /// </summary>
    public sealed class PipelineQueue {

        /// <summary>
        /// event that is fired when next pipeline in queue begins procession
        /// </summary>
        public event EventHandler<MageStatusEventArgs> OnPipelineStarted;

        /// <summary>
        /// event that is fired when pipeline queue terminates (normally or abnormally)
        /// </summary>
        public event EventHandler<MageStatusEventArgs> OnRunCompleted;


        #region Member Variables

        /// <summary>
        /// is queue currently running?
        /// </summary>
        private bool mQueueRunning;

        /// <summary>
        /// The current pipeline that is running (null if none)
        /// </summary>
        private ProcessingPipeline mCurrentPipeline;

        #endregion

        #region Properties

        /// <summary>
        /// Get currently running pipeline
        /// </summary>
        public ProcessingPipeline CurrentPipeline {
            get { return mCurrentPipeline; }
        }

        /// <summary>
        /// Is queue currently running
        /// </summary>
        public bool IsRunning {
            get { return mQueueRunning; }
        }

        /// <summary>
        /// Have a look at the internal queue of pipelines
        /// </summary>
        public Queue<ProcessingPipeline> Pipelines { get { return mPipelineQueue; } }

        #endregion

        #region Client Functions 

        /// <summary>
        /// Internal queue of pipelines to be run
        /// </summary>
        private readonly Queue<ProcessingPipeline> mPipelineQueue = new Queue<ProcessingPipeline>();

        /// <summary>
        /// Adds a pipeline to the queue
        /// </summary>
        /// <param name="pipeline"></param>
        public void Add(ProcessingPipeline pipeline) {
            mPipelineQueue.Enqueue(pipeline);
        }

        /// <summary>
        /// Add pipelines to queue and run the queue
        /// </summary>
        /// <param name="pipelines"></param>
        public void Run(params ProcessingPipeline[] pipelines) {
            foreach (ProcessingPipeline pipeline in pipelines) {
                Add(pipeline);
            }
            Run();
        }

        /// <summary>
        /// Run the queue in a worker thread
        /// </summary>
        public void Run() {
            Globals.AbortRequested = false;
            if (!mQueueRunning) {
                mQueueRunning = true;
                ThreadPool.QueueUserWorkItem(RunPipelinesInQueue);
            }
        }

        /// <summary>
        /// Run the queue in the caller's thread
        /// </summary>
        /// <param name="state"></param>
        public void RunRoot(object state) {
			Globals.AbortRequested = false;
            if (!mQueueRunning) {
                mQueueRunning = true;
                RunPipelinesInQueue(state);
            }
        }
 
        /// <summary>
        /// Cancel the currently running pipeline
        /// and set the abort flag to stop the queue
        /// </summary>
        public void Cancel() {
			Globals.AbortRequested = true;
            if (mCurrentPipeline != null) {
                mCurrentPipeline.Cancel();
            }
            while (mPipelineQueue.Count > 0) {
                ProcessingPipeline nextPipeline = mPipelineQueue.Dequeue();
                nextPipeline.Cancel();
            }
        }

        #endregion

        #region Internal Actions

        /// <summary>
        /// Run all the pipeline in the queue in order
        /// unless abort flag is set
        /// </summary>
        /// <param name="state"></param>
        private void RunPipelinesInQueue(object state) {
            while (mPipelineQueue.Count > 0) {
				if (Globals.AbortRequested) {
                    if (OnRunCompleted != null) {
                        OnRunCompleted(this, new MageStatusEventArgs("Pipeline: Processing aborted"));
                    }
                    break;
                }

                mCurrentPipeline = mPipelineQueue.Dequeue();
                UpdatePipelineStarted(mCurrentPipeline.PipelineName);
                mCurrentPipeline.RunRoot(null);
                mCurrentPipeline = null;
            }
            mPipelineQueue.Clear();
            mQueueRunning = false;
            UpdateQueueCompleted();
        }

        #endregion

        #region Notifications

        /// <summary>
        /// To inform subscribers
        /// </summary>
        private void UpdateQueueCompleted() {
            if (OnRunCompleted != null) {
				if (Globals.AbortRequested)
					OnRunCompleted(this, new MageStatusEventArgs("Aborted"));
				else
					OnRunCompleted(this, new MageStatusEventArgs("Done"));
            }
        }

        /// <summary>
        /// To inform subscribers
        /// </summary>
        /// <param name="msg"></param>
        private void UpdatePipelineStarted(string msg) {
            if (OnPipelineStarted != null) {
                OnPipelineStarted(this, new MageStatusEventArgs(msg));
            }
        }

        #endregion

    }
}

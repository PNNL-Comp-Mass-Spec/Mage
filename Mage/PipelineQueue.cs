using System;
using System.Collections.Generic;
using System.Threading;

namespace Mage
{
    /// <summary>
    /// Runs one or more Mage pipelines from a queue
    /// in a worker thread.
    /// </summary>
    public sealed class PipelineQueue
    {
        /// <summary>
        /// Event that is fired when next pipeline in queue begins procession
        /// </summary>
        public event EventHandler<MageStatusEventArgs> OnPipelineStarted;

        /// <summary>
        /// Event that is fired when pipeline queue terminates (normally or abnormally)
        /// </summary>
        public event EventHandler<MageStatusEventArgs> OnRunCompleted;

        /// <summary>
        /// Get currently running pipeline
        /// </summary>
        public ProcessingPipeline CurrentPipeline { get; private set; }

        /// <summary>
        /// Is queue currently running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Have a look at the internal queue of pipelines
        /// </summary>
        public Queue<ProcessingPipeline> Pipelines { get; } = new();

        /// <summary>
        /// Adds a pipeline to the queue
        /// </summary>
        /// <param name="pipeline"></param>
        public void Add(ProcessingPipeline pipeline)
        {
            Pipelines.Enqueue(pipeline);
        }

        /// <summary>
        /// Add pipelines to queue and run the queue
        /// </summary>
        /// <param name="pipelines"></param>
        public void Run(params ProcessingPipeline[] pipelines)
        {
            foreach (var pipeline in pipelines)
            {
                Add(pipeline);
            }
            Run();
        }

        /// <summary>
        /// Run the queue in a worker thread
        /// </summary>
        public void Run()
        {
            Globals.AbortRequested = false;

            if (!IsRunning)
            {
                IsRunning = true;
                ThreadPool.QueueUserWorkItem(RunPipelinesInQueue);
            }
        }

        /// <summary>
        /// Run the queue in the caller's thread
        /// </summary>
        /// <param name="state"></param>
        public void RunRoot(object state)
        {
            Globals.AbortRequested = false;
            if (!IsRunning)
            {
                IsRunning = true;
                RunPipelinesInQueue(state);
            }
        }

        /// <summary>
        /// Cancel the currently running pipeline
        /// and set the abort flag to stop the queue
        /// </summary>
        public void Cancel()
        {
            Globals.AbortRequested = true;
            CurrentPipeline?.Cancel();
            while (Pipelines.Count > 0)
            {
                var nextPipeline = Pipelines.Dequeue();
                nextPipeline.Cancel();
            }
        }

        /// <summary>
        /// Run all the pipeline in the queue in order
        /// unless abort flag is set
        /// </summary>
        /// <param name="state"></param>
        private void RunPipelinesInQueue(object state)
        {
            while (Pipelines.Count > 0)
            {
                if (Globals.AbortRequested)
                {
                    OnRunCompleted?.Invoke(this, new MageStatusEventArgs("Pipeline: Processing aborted"));
                    break;
                }

                CurrentPipeline = Pipelines.Dequeue();
                UpdatePipelineStarted(CurrentPipeline.PipelineName);
                CurrentPipeline.RunRoot(null);
                CurrentPipeline = null;
            }

            Pipelines.Clear();
            IsRunning = false;

            UpdateQueueCompleted();
        }

        /// <summary>
        /// To inform subscribers
        /// </summary>
        private void UpdateQueueCompleted()
        {
            if (OnRunCompleted != null)
            {
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
        private void UpdatePipelineStarted(string msg)
        {
            OnPipelineStarted?.Invoke(this, new MageStatusEventArgs(msg));
        }
    }
}

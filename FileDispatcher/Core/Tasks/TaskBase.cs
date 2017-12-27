using System;
using System.Threading.Tasks;

namespace FileDispatcher.Core.Tasks
{
    /// <summary>
    /// When derived provides methods to perform simple task
    /// </summary>
    [Serializable]
    public abstract class TaskBase : IDisposable
    {
        public const string SuccessResult = "Success";

        /// <summary>
        /// Raised when Dispatch occurs
        /// </summary>
        public event EventHandler<DispatchedEventArgs> Dispatched;

        /// <summary>
        /// Initializes a new instance of <see cref="TaskBase"/>.
        /// </summary>
        /// <param name="trigger">Task trigger</param>
        public TaskBase(ITrigger trigger)
        {
            Trigger = trigger;
            Trigger.Ready += Trigger_Ready;
        }

        /// <summary>
        /// Gets the trigger for task or sets the trigger and subscribes to <see cref="ITrigger.Ready"/>.
        /// <see cref="event"/>
        /// </summary>
        public ITrigger Trigger { get; private set; }

        /// <summary>
        /// Gets or sets the name of the task.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Free up or release resources.
        /// </summary>
        public virtual void Dispose()
        {
            Trigger.Dispose();
        }

        /// <summary>
        /// When implemented in derived class should perform purpose of this task
        /// and call <see cref="OnDispatched(DispatchedEventArgs)"/> method to raise <see cref="Dispatched"/> event
        /// </summary>
        /// <param name="sourcePath">Source path of dispatch operation</param>
        protected abstract DispatchedEventArgs PerformDispatch(string sourcePath);

        /// <summary>
        /// Raises the <see cref="Dispatched"/> event
        /// </summary>
        protected virtual void OnDispatched(DispatchedEventArgs dispatchedEventArgs) => Dispatched?.Invoke(this, dispatchedEventArgs);

        private Task PerformDispatchAsync(string sourcePath)
        {
            return Task.Run(() =>
            {
                DispatchedEventArgs dispatchedEventArgs = PerformDispatch(sourcePath);
                if (dispatchedEventArgs != null)
                    OnDispatched(dispatchedEventArgs);
            });
        }

        private void Trigger_Ready(object sender, ReadyEventArgs e) => PerformDispatchAsync(e.FullPath);
    }
}

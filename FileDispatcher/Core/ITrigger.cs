using System;

namespace FileDispatcher.Core
{
    //Can be implementedin Task or some of the tasks as well in order to chain tasks.
    /// <summary>
    /// Defines members required to trigger dispatch execution of <see cref="Tasks.TargetableTaskBase"/> on file system object (file, directory)
    /// </summary>
    public interface ITrigger : IDisposable
    {
        /// <summary>
        /// When implemented informs about file system object ready to be handled
        /// </summary>
        event EventHandler<ReadyEventArgs> Ready;

        /// <summary>
        /// When implemented gets or sets the value whether the trigger is enabled or disabled.
        /// </summary>
        bool Enabled { get; set; }
    }
}

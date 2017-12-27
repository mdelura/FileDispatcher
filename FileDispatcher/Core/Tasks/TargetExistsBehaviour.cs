using System;
using System.ComponentModel;

namespace FileDispatcher.Core.Tasks
{
    [Serializable]
    /// <summary>
    /// Specifies behaviour in case target object already exists
    /// </summary>
    public enum TargetExistsBehaviour
    {
        [Description("Do nothing")]
        DoNothing,
        [Description("Rename")]
        Rename,
        [Description("Overwrite")]
        Overwrite
    }
}

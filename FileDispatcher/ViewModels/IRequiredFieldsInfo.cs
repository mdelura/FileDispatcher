using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDispatcher.ViewModels
{
    /// <summary>
    /// When implemented should provide information about fields that are required to be filled.
    /// </summary>
    public interface IRequiredFieldsInfo
    {
        /// <summary>
        /// When implemented gets the value whether all required fields are filled.
        /// </summary>
        bool HasRequiredFieldsFilled { get; }
    }
}

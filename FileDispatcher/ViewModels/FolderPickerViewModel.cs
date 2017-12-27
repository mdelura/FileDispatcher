using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;

namespace FileDispatcher.ViewModels
{
    /// <summary>
    /// ViewModel for FolderPicker control
    /// </summary>
    public class FolderPickerViewModel : BindableDataErrorNotifierBase, IRequiredFieldsInfo
    {
        const string defaultPrompt = "Select Folder";

        public FolderPickerViewModel() : this(null)
        {
        }

        public FolderPickerViewModel(string path) : this(path, defaultPrompt)
        {
        }

        public FolderPickerViewModel(string path, string prompt)
        {
            _item = path;
            _prompt = prompt;
            PickFolderDialogCommand = new DelegateCommand(OnPickFolderDialog);
        }

        private string _item;
        public string Item
        {
            get => _item;
            set => SetPropertyAndValidate(ref _item, value);
        }

        private string _prompt;
        public string Prompt
        {
            get => _prompt;
            set => SetProperty(ref _prompt, value);
        }

        public bool HasRequiredFieldsFilled => !string.IsNullOrEmpty(_item);


        public ICommand PickFolderDialogCommand { get; private set; }

        protected override IEnumerable<ValidationResult> GetValidationResults(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Item):
                    yield return new ValidationResult(Directory.Exists(Item), "Path is invalid or does not exist.");
                    break;
            }
        }

        private void OnPickFolderDialog()
        {
            bool isPathValidAndNotEmpty = string.IsNullOrEmpty(Item) && !GetErrors(nameof(Item)).Cast<object>().Any();
            if (CommonFileDialog.IsPlatformSupported)
            {
                using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
                {
                    dialog.IsFolderPicker = true;
                    dialog.Title = _prompt;
                    if (isPathValidAndNotEmpty)
                    {
                        dialog.DefaultDirectory = Item;
                    }
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        Item = dialog.FileName;
                    }
                }
            }
            else
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    if (isPathValidAndNotEmpty)
                    {
                        dialog.SelectedPath = Item;
                    }
                    dialog.Description = _prompt;
                    if (dialog.ShowDialog() == DialogResult.OK)
                        Item = dialog.SelectedPath;
                }
            }
        }
    }
}

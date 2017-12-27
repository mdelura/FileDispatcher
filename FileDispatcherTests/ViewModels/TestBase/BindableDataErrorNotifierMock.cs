using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FileDispatcher.ViewModels;

namespace FileDispatcherTests.ViewModels.TestBase
{
    public class BindableDataErrorNotifierMock : BindableDataErrorNotifierBase
    {
        public const string ValidText = "valid text";
        public const string InvalidText = "invalid text";
        public const string TextErrorsContent = "Text is not valid";
        public const string ValueErrorsContent = "Text is not valid";

        public const int ValidValue = 1;
        public const int InvalidValue = 2;

        public BindableDataErrorNotifierMock()
        {
        }

        private string _text;

        public string Text
        {
            get => _text;
            set => SetPropertyAndValidate(ref _text, value);
        }

        private int _value;

        public int Value
        {
            get => _value;
            set => SetPropertyAndValidate(ref _value, value);
        }


        protected override IEnumerable<ValidationResult> GetValidationResults(string propertyName)
        {
            switch (propertyName)
            {
                case (nameof(Text)):
                    yield return new ValidationResult(_text == ValidText, TextErrorsContent);
                    break;
                case (nameof(Value)):
                    yield return new ValidationResult(_value == ValidValue, ValueErrorsContent);
                    break;
            }
        }
    }
}

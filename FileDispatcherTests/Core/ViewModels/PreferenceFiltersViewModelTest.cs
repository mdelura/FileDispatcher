using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FileDispatcher.Core.ViewModels;
using FileDispatcherTests.ViewModels.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.ViewModels
{
    [TestClass]
    public class PreferenceFiltersViewModelTest : BindableDataErrorNotifierTestBase<PreferenceFiltersViewModel>
    {
        ObservableCollection<string> _filters;
        ObservableCollection<string> _exclusionFilters;

        public PreferenceFiltersViewModelTest()
        {
            _filters = new ObservableCollection<string>()
            {
                "*.txt",
                "*.dat",
                "*.bin",
            };

            _exclusionFilters = new ObservableCollection<string>()
            {
                "file.txt",
                "data.*t",
                "t*",
            };
        }

        [TestMethod]
        public void AddItems_RaisesPropertyChanged()
        {
            //Arrange
            var expectedPropertyChangedPropertyNames = _filters
                .Select(f => nameof(PreferenceFiltersViewModel.Filters))
                .Concat(_exclusionFilters
                    .Select(f => nameof(PreferenceFiltersViewModel.ExclusionFilters)));
            //Act
            bindable.Filters.AddRange(_filters);
            bindable.ExclusionFilters.AddRange(_exclusionFilters);
            //Assert
            AssertCollectionsAreEqual(expectedPropertyChangedPropertyNames, propertyChangedPropertyNames);
        }

        [TestMethod]
        public void AddInvalidItems_RaisesErrorsChanged()
        {
            //Arrange
            var expectedErrorsChangedPropertyNames = _filters
                .Select(f => nameof(PreferenceFiltersViewModel.ExclusionFilters));
            bindable.Filters.AddRange(_filters);
            //Act
            bindable.ExclusionFilters.AddRange(_filters);
            //Assert
            AssertCollectionsAreEqual(expectedErrorsChangedPropertyNames, errorsChangedPropertyNames);
        }

        [TestMethod]
        public void AddInvalidItems_HasErrors()
        {
            //Arrange
            bindable.Filters.AddRange(_filters);
            //Act
            bindable.ExclusionFilters.AddRange(_filters);
            //Assert
            Assert.IsTrue(bindable.HasErrors);
        }

        [TestMethod]
        public void GetErrors_ReturnsExpectedErrorsContent()
        {
            //Arrange
            bindable.Filters.AddRange(_filters);
            bindable.ExclusionFilters.AddRange(_filters);
            //Act
            var errorsContent = GetErrorsContent(nameof(PreferenceFiltersViewModel.ExclusionFilters));
            var eachContentContainAtLeastOneFilter = errorsContent.Any() &&
                errorsContent
                .All(c => _filters
                    .Any(f => f.Intersect(c).Any()));
            //Assert
            Assert.IsTrue(eachContentContainAtLeastOneFilter);

        }

        protected override IEnumerable<string> ExpectedPropertyChangedPropertyNames()
        {
            return new string[]
            {
                nameof(PreferenceFiltersViewModel.Filters),
                nameof(PreferenceFiltersViewModel.ExclusionFilters)
            };
        }

        protected override IEnumerable<string> ExpectedErrorsChangedPropertyNames()
        {
            return new string[] { nameof(PreferenceFiltersViewModel.ExclusionFilters) };
        }

        protected override void SetProperties(PreferenceFiltersViewModel bindableDataErrorNotifier, bool validValues)
        {
            bindableDataErrorNotifier.Filters = _filters;
            if (validValues)
            {
                bindableDataErrorNotifier.ExclusionFilters = _exclusionFilters;
            }
            else
            {
                bindableDataErrorNotifier.ExclusionFilters = new ObservableCollection<string>(_exclusionFilters.Concat(_filters));
            }
        }
    }
}


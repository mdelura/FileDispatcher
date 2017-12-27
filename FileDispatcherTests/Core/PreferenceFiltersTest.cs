using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FileDispatcher.Core;
using FileDispatcher.Core.ExtensionMethods.SerializationExtensions;
using FileDispatcher.Core.ExtensionMethods.StringExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core
{
    [TestClass]
    public class PreferenceFiltersTest
    {
        static string[] wildCardFilters = new string[]
        {
            "r*.*",
            "*.txt",
            "file*.*",
        };

        static string[] specificFilters = new string[]
        {
            "readme.txt",
            "data.bin",
            "filesystem.cfg"
        };

        PreferenceFilters _preferenceFilters;

        [TestInitialize]
        public void TestInit()
        {
            _preferenceFilters = new PreferenceFilters();
        }

        [TestMethod]
        public void AddFilters_AddsFilters()
        {
            //Act
            _preferenceFilters.Filters.AddRange(specificFilters);
            //Assert
            CollectionAssert.AreEqual(specificFilters, _preferenceFilters.Filters);
        }

        [TestMethod]
        public void AddExclusionFilters_AddsExclusionFilters()
        {
            //Act
            _preferenceFilters.ExclusionFilters.AddRange(specificFilters);
            //Assert
            CollectionAssert.AreEqual(specificFilters, _preferenceFilters.ExclusionFilters);
        }

        [TestMethod]
        public void ReplaceFilters_ReplacesFilters()
        {
            //Arrange
            _preferenceFilters.Filters.AddRange(wildCardFilters);
            //Act
            for (int i = 0; i < _preferenceFilters.Filters.Count; i++)
            {
                _preferenceFilters.Filters[i] = specificFilters[i];
            }
            //Assert
            CollectionAssert.AreEqual(specificFilters, _preferenceFilters.Filters);
        }

        [TestMethod]
        public void ReplaceExclusionFilters_ReplacesExclusionFilters()
        {
            //Arrange
            _preferenceFilters.ExclusionFilters.AddRange(wildCardFilters);
            //Act
            for (int i = 0; i < _preferenceFilters.ExclusionFilters.Count; i++)
            {
                _preferenceFilters.ExclusionFilters[i] = specificFilters[i];
            }
            //Assert
            CollectionAssert.AreEqual(specificFilters, _preferenceFilters.ExclusionFilters);
        }

        [TestMethod]
        public void AddFilter_WhenEqualToExclusionFilter_ThrowsArgumentException()
        {
            //Arrange
            string testFilter = wildCardFilters[0];
            _preferenceFilters.ExclusionFilters.Add(testFilter);
            bool exceptionThrown = false;
            //Act
            try
            {
                _preferenceFilters.Filters.Add(testFilter);
            }
            catch (ArgumentException e)
            {
                exceptionThrown = true;
                StringAssert.Contains(e.Message, testFilter);
            }

            Assert.IsTrue(exceptionThrown, "Expected exception was not thrown");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddExclusionFilter_WhenEqualToFilter_ThrowsArgumentException()
        {
            //Arrange
            _preferenceFilters.Filters.AddRange(wildCardFilters);
            //Act
            _preferenceFilters.ExclusionFilters.Add(_preferenceFilters.Filters[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReplaceFilter_WhenEqualToExclusionFilter_ThrowsArgumentException()
        {
            //Arrange
            _preferenceFilters.ExclusionFilters.AddRange(wildCardFilters);
            _preferenceFilters.Filters.AddRange(specificFilters);
            //Act
            _preferenceFilters.Filters[0] = _preferenceFilters.ExclusionFilters[0];
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReplaceExclusionFilter_WhenEqualToFilter_ThrowsArgumentException()
        {
            //Arrange
            _preferenceFilters.Filters.AddRange(wildCardFilters);
            _preferenceFilters.ExclusionFilters.AddRange(specificFilters);

            //Act
            _preferenceFilters.ExclusionFilters[0] = _preferenceFilters.Filters[0];
        }

        [TestMethod]
        public void Serialize_Deserialize_Success()
        {
            //Arrange
            string serializationPath = Path.Combine(CreateTestDirectory(), "preferencefilters.bin");
            _preferenceFilters.Filters.AddRange(wildCardFilters);
            _preferenceFilters.ExclusionFilters.AddRange(specificFilters);

            //Act
            _preferenceFilters.SerializeToFile(serializationPath);
            PreferenceFilters deserializedPreferenceFilters = serializationPath.DeserializeFromFile<PreferenceFilters>();
            
            //Assert
            CollectionAssert.AreEqual(_preferenceFilters.Filters, deserializedPreferenceFilters.Filters);
            CollectionAssert.AreEqual(_preferenceFilters.ExclusionFilters, deserializedPreferenceFilters.ExclusionFilters);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Deserialized_AddFilter_WhenEqualToExclusionFilter_ThrowsArgumentException()
        {
            //Arrange
            string serializationPath = Path.Combine(CreateTestDirectory(), "preferencefilters.bin");
            _preferenceFilters.ExclusionFilters.AddRange(specificFilters);

            _preferenceFilters.SerializeToFile(serializationPath);
            PreferenceFilters deserializedPreferenceFilters = serializationPath.DeserializeFromFile<PreferenceFilters>();

            //Act
            deserializedPreferenceFilters.Filters.Add(specificFilters[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Deserialized_AddExclusionFilter_WhenEqualToFilter_ThrowsArgumentException()
        {
            //Arrange
            string serializationPath = Path.Combine(CreateTestDirectory(), "preferencefilters.bin");
            _preferenceFilters.Filters.AddRange(specificFilters);

            _preferenceFilters.SerializeToFile(serializationPath);
            PreferenceFilters deserializedPreferenceFilters = serializationPath.DeserializeFromFile<PreferenceFilters>();

            //Act
            deserializedPreferenceFilters.ExclusionFilters.Add(specificFilters[0]);
        }

        [TestMethod]
        public void AreMet_NameMatchesFilters_ReturnsTrue()
        {
            //Arrange
            _preferenceFilters.Filters.AddRange(wildCardFilters);
            string name = _preferenceFilters.Filters[0].Replace("*", "xyz");
            //Act and Assert
            Assert.IsTrue(_preferenceFilters.AreMet(name));
        }

        [TestMethod]
        public void AreMet_NamesIsNotMatchingFilters_ReturnsFalse()
        {
            //Arrange
            string[] wildCardFilters = new string[]
            {
                "r*.*",
                "*.txt",
                "file*.*",
            };

            string[] specificFilters = new string[]
            {
                "data.bin",
                "readme.txt",
                "filesystem.cfg"
            };

            _preferenceFilters.Filters.AddRange(specificFilters);
            string name = "file" + _preferenceFilters.Filters[0];
            //Act and Assert
            Assert.IsFalse(_preferenceFilters.AreMet(name));
        }

        [TestMethod]
        public void AreMet_NameMatchesExclusionFilters_ReturnsFalse()
        {
            //Arrange
            string[] wildCardFilters = new string[]
            {
                "r*.*",
                "*.txt",
                "file*.*",
            };

            string[] specificFilters = new string[]
            {
                "readme.txt",
                "data.bin",
                "filesystem.cfg"
            };

            _preferenceFilters.Filters.AddRange(wildCardFilters);
            _preferenceFilters.ExclusionFilters.AddRange(specificFilters);
            string name = _preferenceFilters.ExclusionFilters[0];

            //Act Assert
            Assert.IsTrue(_preferenceFilters.Filters.Any(f => name.FitsFilter(f)));
            Assert.IsFalse(_preferenceFilters.AreMet(name));
        }
    }
}

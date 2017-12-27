using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FileDispatcher.Views.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.Views.Converters
{
    [TestClass]
    public class ObservableStringsToStringConverterTest
    {
        static ObservableStringsToStringConverter converter;
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            converter = new ObservableStringsToStringConverter();
        }

        [TestMethod]
        public void Convert_ReturnsExpectedValue()
        {
            //Arrange
            string expectedValue = "test 1, test 2, test 3";

            ObservableCollection<string> testItems = new ObservableCollection<string>()
            {
                "test 1",
                "test 2",
                "test 3"
            };
            //Act
            string actualValue = converter.Convert(testItems, null, null, null).ToString();

            //Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void ConvertBack_ReturnsExpectedValue()
        {
            //Arrange
            string testValue = "test 1, test 2;test 3";

            string[] expectedItems = new string[]
            {
                "test 1",
                "test 2",
                "test 3"
            };
            //Act
            string[] actualItems = ((IEnumerable<string>)converter.ConvertBack(testValue, null, null, null))
                .ToArray();

            //Assert
            CollectionAssert.AreEqual(expectedItems, actualItems);
        }

        [TestMethod]
        public void Convert_ValueIsNull_ReturnsNull()
        {
            //Arrange
            string[] values = null;
            //Act
            object result = converter.Convert(values, null, null, null);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ConvertBack_ValueIsNull_ReturnsNull()
        {
            //Arrange
            string value = null;
            //Act
            object result = converter.ConvertBack(value, null, null, null);

            //Assert
            Assert.IsNull(result);
        }
    }
}

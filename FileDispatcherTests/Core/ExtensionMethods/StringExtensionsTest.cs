using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileDispatcher.Core.ExtensionMethods.StringExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core.ExtensionMethods
{
    [TestClass]
    public class StringExtensionsTest
    {
        const string fileName = "testfile.txt";

        [TestMethod]
        public void FitsFilter_MatchingFilter_ReturnsTrue()
        {
            //Arrange
            string fileName = "testfile.txt";

            string[] filters = new string[]
            {
                "*.*",
                "*.txt",
                "testfile.*",
                "test*.txt",
                "*file.txt",
                "?estf?l?.?x?",
                "?estf*l?.?x?",
                "?estf*l?*.?x?",
                "TEST*.TXT"
            };

            //Act and Assert
            foreach (string filter in filters)
            {
                Assert.IsTrue(fileName.FitsFilter(filter), $"Filter: {filter}");
            }
        }

        [TestMethod]
        public void FitsFilter_NotMatchingFilter_ReturnsFalse()
        {
            //Arrange
            string fileName = "testfile.txt";

            string[] filters = new string[]
            {
                "*..*",
                "*txt.",
                "testfile?.*",
                "test?????.txt",
                "*file?.txt",
                "?estf??l?.?x?",
                "file.txt"
            };

            //Act and Assert
            foreach (string filter in filters)
            {
                Assert.IsFalse(fileName.FitsFilter(filter), $"Filter: {filter}");
            }
        }

        [TestMethod]
        public void IsDirectory_DirectoryPath_ReturnsTrue()
        {
            //Arrange
            string path = CreateTestDirectory();
            //Act
            bool isDirectory = path.IsDirectory();
            //Assert
            Assert.IsTrue(isDirectory);
        }

        [TestMethod]
        public void IsDirectory_FilePath_ReturnsFalse()
        {
            //Arrange
            string path = CreateTestFile(CreateTestDirectory(), fileName);
            //Act
            bool isDirectory = path.IsDirectory();
            //Assert
            Assert.IsFalse(isDirectory);
        }

        [TestMethod]
        [ExpectedException(typeof(IOException), AllowDerivedTypes = true)]
        public void IsDirectory_PathDoesNotExist_ShouldThrowIOException()
        {
            //Arrange
            string path = GetNonExistingPath();
            //Act
            bool isDirectory = path.IsDirectory();
        }

        [TestMethod]
        public void ComputeLevenshteinDistanceTo_ReturnsExpectedValue()
        {
            //Arrange
            var items = new Tuple<string, string, int>[]
            {
                new Tuple<string, string, int>("book", "boko", 2),
                new Tuple<string, string, int>("text", string.Empty, "text".Length),
                new Tuple<string, string, int>(string.Empty, "test text", "test text".Length),
                new Tuple<string, string, int>("text", "text", 0),
                new Tuple<string, string, int>("Text", "text", 1),
                new Tuple<string, string, int>("ABC123", "DEF456", 6)

            }.Select(t => new
            {
                Text1 = t.Item1,
                Text2 = t.Item2,
                Distance = t.Item3
            })
            .ToArray();

            var expectedValues = items
                .Select(i => i.Distance)
                .ToArray();
            //Act
            var actualValues = items
                .Select(i => i.Text1.ComputeLevenshteinDistanceTo(i.Text2))
                .ToArray();
            //Assert
            AssertCollectionsAreEqual(expectedValues, actualValues);
        }

        [TestMethod]
        public void Standardize_ReturnsExpectedValue()
        {
            //Arrange
            var items = new Dictionary<string, string>
            {
                { "Files [ico]", "FILES ICO" },
                { " the.file", "THE FILE" },
                { ".3.Times.Faster.", "3 TIMES FASTER" }
            }
            .Select(kv => new
            {
                Item = kv.Key,
                ExpectedValue = kv.Value
            })
            .ToArray();

            var expectedValues = items
                .Select(i => i.ExpectedValue)
                .ToArray();
            
            //Act
            var actualValues = items
                .Select(i => i.Item.Standardize())
                .ToArray();

            //Assert
            AssertCollectionsAreEqual(expectedValues, actualValues);
        }

        [TestMethod]
        public void Truncate_ReturnsExpectedValues()
        {
            //Arrange
            var testItem = "123456789";
            var items = new Tuple<int, string>[]
            {
                new Tuple<int, string>(0, string.Empty ),
                new Tuple<int, string>(1, testItem.Substring(0,1)),
                new Tuple<int, string>(5, testItem.Substring(0,5)),
                new Tuple<int, string>(9, testItem ),
                new Tuple<int, string>(10, testItem ),
            }
            .Select(t => new
            {
                MaxLength = t.Item1,
                ExpectedValue = t.Item2
            })
            .ToArray();

            var expectedValues = items
                .Select(i => i.ExpectedValue)
                .ToArray();

            //Act
            var actualValues = items
                .Select(i => testItem.Truncate(i.MaxLength))
                .ToArray();

            //Assert
            AssertCollectionsAreEqual(expectedValues, actualValues);
        }
    }
}

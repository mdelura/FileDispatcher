using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using FileDispatcher;
using FileDispatcher.Core;
using FileDispatcher.Core.ExtensionMethods.SerializationExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core
{
    [TestClass]
    [Serializable]
    public class WatcherTriggerTest
    {
        const string matchAllFilter = "c*";

        const string createFileName = "create.txt";
        const string copyFileName = "copy.csv";
        const string moveFolderName = "CMoveFolder";
        const string createFolderName = "CreateFolder";

        static string assistantFolder;
        static string copySourceFilePath;
        static string moveSourceFolderPath;

        string _createFilePath;
        string _createFolderPath;
        string _copyTargetFilePath;
        string _moveTargetFolderPath;

        string[] _allTestItems; 

        WatcherTrigger _watcher;
        string _testFolder;
        List<ReadyEventArgs> _readyItems = new List<ReadyEventArgs>();

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            assistantFolder = CreateTestDirectory();
            copySourceFilePath = CreateTestFile(assistantFolder, copyFileName);
            moveSourceFolderPath = Path.Combine(assistantFolder, moveFolderName);
        }

        [TestInitialize]
        public void TestInit()
        {
            //Arrange folders and paths
            _testFolder = CreateTestDirectory();
            Directory.CreateDirectory(moveSourceFolderPath);//Needs to be done for each test as this folder is moved

            _allTestItems = new string[]
            {
                _createFilePath = Path.Combine(_testFolder, createFileName),
                _createFolderPath = Path.Combine(_testFolder, createFolderName),
                _copyTargetFilePath = Path.Combine(_testFolder, copyFileName),
                _moveTargetFolderPath = Path.Combine(_testFolder, moveFolderName),
            };

            //Arrange WatcherTrigger to most common settings
            _watcher = new WatcherTrigger()
            {
                Path = _testFolder,
                Enabled = true,
                WatchedElements = WatchedElements.File | WatchedElements.Directory,
                IncludeSubdirectories = false
            };
            _watcher.PreferenceFilters.Filters.Add(matchAllFilter);
            _watcher.Ready += (s, e) => _readyItems.Add(e);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _watcher.Dispose();
            _readyItems.Clear();
        }

        [TestMethod]
        public void PathSet_EnabledIsTrue_ItemReady_ReadyEventRaised()
        {
            //Arrange
            string[] expectedItems = _allTestItems;
            //Act
            ArrangeAllTestItems();
            WaitForResult(() => _readyItems.Count == expectedItems.Length);
            //Assert
            CollectionAssert.AreEquivalent(expectedItems, _readyItems.Select(e => e.FullPath).ToArray());
        }

        [TestMethod]
        public void PathSet_EnabledIsFalse_ItemReady_ReadyEventNotRaised()
        {
            //Arrange
            _watcher.Enabled = false;
            //Act
            ArrangeAllTestItems();
            //Assert
            Assert.IsFalse(_readyItems.Any());
        }

        [TestMethod]
        public void IncludeSubdirectoriesIsTrue_ItemInSubdirectory_ReadyEventRaised()
        {
            //Arrange
            _watcher.IncludeSubdirectories = true;
            string subdirectoryPath = Directory.CreateDirectory(Path.Combine(_testFolder, "SubFolder")).FullName;
            //Act
            CreateTestFile(subdirectoryPath, createFileName);
            //Waiting is required, there is a bit of delay here
            WaitForResult(() => _readyItems.Any());
            //Assert
            Assert.AreEqual(Path.Combine("SubFolder", createFileName), _readyItems.Single().Name);
        }

        [TestMethod]
        public void IncludeSubdirectoriesIsFalse_ItemInSubdirectory_ReadyEventNotRaised()
        {
            //Arrange
            _watcher.IncludeSubdirectories = true;
            _watcher.IncludeSubdirectories = false;
            string subdirectoryPath = Directory.CreateDirectory(Path.Combine(_testFolder, "SubFolder")).FullName;
            //Act
            CreateTestFile(subdirectoryPath, createFileName);
            //Waiting is required, there is a bit of delay here
            WaitForResult(() => _readyItems.Any());
            //Assert
            Assert.IsFalse(_readyItems.Any());
        }

        [TestMethod]
        public void WatchedElements_OnlyMatchedItems()
        {
            //Arrange
            _watcher.WatchedElements = WatchedElements.File;
            string[] expectedItems = new string[] { createFileName, copyFileName };
            //Act
            ArrangeAllTestItems();
            WaitForResult(() => _readyItems.Count == expectedItems.Count());
            //Assert
            CollectionAssert.AreEquivalent(expectedItems, _readyItems.Select(e => e.Name).ToArray());
        }


        [TestMethod]
        public void PreferenceFilters_OnlyMatchedItems()
        {
            //Arrange
            _watcher.PreferenceFilters.Filters.Clear();
            _watcher.PreferenceFilters.Filters.AddRange(new string[] { "cr*", "co*" });
            string[] expectedItems = new string[] { createFileName, copyFileName, createFolderName };
            //Act
            ArrangeAllTestItems();
            //Assert
            CollectionAssert.AreEquivalent(expectedItems, _readyItems.Select(e => e.Name).ToArray());
        }

        [TestMethod]
        public void PreferenceFilters_MatchExclusionFilters_ReadyEventNotRaised()
        {
            //Arrange
            _watcher.PreferenceFilters.Filters.Clear();
            _watcher.PreferenceFilters.Filters.AddRange(new string[] { "c*.*", "*Folder" });
            _watcher.PreferenceFilters.ExclusionFilters.AddRange(new string[] { "*.csv", "*mov*" });

            string[] expectedItems = new string[] { createFileName, createFolderName };
            //Act
            ArrangeAllTestItems();
            WaitForResult(() => _readyItems.Count == expectedItems.Length);
            //Assert
            CollectionAssert.AreEquivalent(expectedItems, _readyItems.Select(e => e.Name).ToArray());
        }

        [TestMethod]
        public void Serialize_Deserialize_Success()
        {
            //Arrange
            string serializationPath = Path.Combine(_testFolder, "watcher.bin");
            string[] expectedItems = _allTestItems;
            List<string> readyFullPathsFromDeserializedWatcher = new List<string>();
            //Act
            _watcher.SerializeToFile(serializationPath);
            WatcherTrigger deserializedWatcher = serializationPath.DeserializeFromFile<WatcherTrigger>();

            deserializedWatcher.Ready += (s, e) => readyFullPathsFromDeserializedWatcher.Add(e.FullPath);
            ArrangeAllTestItems();
            WaitForResult(() => readyFullPathsFromDeserializedWatcher.Count == expectedItems.Length);
            //Assert all property values are as expected
            AssertWatcherPropertiesHaveEqualValues(_watcher, deserializedWatcher);
            //Assert deserialized watcher 
            CollectionAssert.AreEquivalent(expectedItems, readyFullPathsFromDeserializedWatcher);
        }

        private void AssertWatcherPropertiesHaveEqualValues(WatcherTrigger expectedWatcher, WatcherTrigger actualWatcher)
        {
            List<string> differences = new List<string>();
            if (actualWatcher.Path != expectedWatcher.Path)
                differences.Add(
                    $"{nameof(WatcherTrigger.Path)}\r\n    Expected: : {expectedWatcher.Path}\r\n    Actual: : {actualWatcher.Path}");
            if (actualWatcher.IncludeSubdirectories != expectedWatcher.IncludeSubdirectories)
                differences.Add(
                    $"{nameof(WatcherTrigger.IncludeSubdirectories)}\r\n    Expected: : {expectedWatcher.IncludeSubdirectories}\r\n    Actual: : {actualWatcher.IncludeSubdirectories}");
            if (actualWatcher.Enabled != expectedWatcher.Enabled)
                differences.Add(
                    $"{nameof(WatcherTrigger.Enabled)}\r\n    Expected: : {expectedWatcher.Enabled}\r\n    Actual: : {actualWatcher.Enabled}");
            if (actualWatcher.WatchedElements != expectedWatcher.WatchedElements)
                differences.Add(
                    $"{nameof(WatcherTrigger.WatchedElements)}\r\n    Expected: : {expectedWatcher.WatchedElements}\r\n    Actual: : {actualWatcher.WatchedElements}");

            string expectedFilters = string.Join(", ", expectedWatcher.PreferenceFilters.Filters);
            string actualFilters = string.Join(", ", actualWatcher.PreferenceFilters.Filters);

            string expectedExclusionFilters = string.Join(", ", expectedWatcher.PreferenceFilters.ExclusionFilters);
            string actualExclusionFilters = string.Join(", ", actualWatcher.PreferenceFilters.ExclusionFilters);

            if (actualFilters != expectedFilters)
                differences.Add(
                    $"{nameof(PreferenceFilters.Filters)}\r\n    Expected: : {expectedFilters}\r\n    Actual: : {actualFilters}");
            if (actualExclusionFilters != expectedExclusionFilters)
                differences.Add(
                    $"{nameof(PreferenceFilters.ExclusionFilters)}\r\n    Expected: : {expectedExclusionFilters}\r\n    Actual: : {actualExclusionFilters}");

            Assert.IsFalse(differences.Any(), $"Differences in:\r\n{string.Join("\r\n", differences)}");
        }

        private void ArrangeAllTestItems()
        {
            CreateTestFile(_testFolder, createFileName);
            File.Copy(copySourceFilePath, _copyTargetFilePath);
            Directory.CreateDirectory(_createFolderPath);
            Directory.Move(moveSourceFolderPath, _moveTargetFolderPath);
        }
    }
}

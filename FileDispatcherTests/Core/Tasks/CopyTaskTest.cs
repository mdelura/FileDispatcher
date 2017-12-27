using System;
using System.Collections.Generic;
using System.IO;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcherTests.Core.Tasks.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core.Tasks
{
    [TestClass]
    [Serializable]
    public class CopyTaskTest : TargetableTaskBaseTestBase<CopyTask>
    {
        protected override IEnumerable<ReadyEventArgs> DispatchReadyItems()
        {
            yield return GetReadyEventArgs(MainTestDirectory, FileName);
            yield return GetReadyEventArgs(MainTestDirectory, FolderName);
        }

        protected override IEnumerable<DispatchedEventArgs> ExpectedDispatchPerformedEventArgs()
        {
            yield return new DispatchedEventArgs(DateTime.Now, testName, TestFilePath, Path.Combine(TargetFolderPath, FileName), TaskBase.SuccessResult);
            yield return new DispatchedEventArgs(DateTime.Now, testName, TestFolderPath, Path.Combine(TargetFolderPath, FolderName), TaskBase.SuccessResult);
        }

        protected override IEnumerable<Func<bool>> ExpectedResultPredicates()
        {
            yield return () => File.Exists(TestFilePath) && 
                File.Exists(Path.Combine(TargetFolderPath, FileName));
            yield return () => Directory.Exists(TestFolderPath) && 
                Directory.Exists(Path.Combine(TargetFolderPath, FolderName));
        }

        [TestMethod]
        public void CopySubdirectoriesIsTrue_WhenDirectoryHasContent_CopiesWithContent()
        {
            //Arrange
            DispatchedEventArgs[] expectedDispatchedItems = new DispatchedEventArgs[]
            {
                new DispatchedEventArgs(DateTime.Now, testName, TestFolderPath, Path.Combine(TargetFolderPath, FolderName), TaskBase.SuccessResult)
            };
            task.CopySubdirectories = true;
            CreateTestFile(TestFolderPath, FileName);
            string subdirectory = Directory.CreateDirectory(Path.Combine(TestFolderPath, FolderName)).FullName;
            CreateTestFile(subdirectory, FileName);
            //Act
            Trigger.RaiseReady(
                new ReadyEventArgs(new FileSystemEventArgs(WatcherChangeTypes.Created, MainTestDirectory, FolderName)));

            string targetSubdirectoryPath = Path.Combine(TargetFolderPath, FolderName, FolderName);
            Func<bool> expectedResultPredicate = () =>
                Directory.Exists(Path.Combine(TargetFolderPath, FolderName)) &&
                File.Exists(Path.Combine(TargetFolderPath, FolderName, FileName)) &&
                Directory.Exists(targetSubdirectoryPath) &&
                File.Exists(Path.Combine(targetSubdirectoryPath, FileName));

            WaitForResult(DefaultDelayTimeoutMillis, expectedResultPredicate);
            //Assert
            Assert.IsTrue(expectedResultPredicate());
            AssertDispatchedEventArgsItemsAreEquivalent(expectedDispatchedItems, dispatchedItems);
        }

        [TestMethod]
        public void CopySubdirectoriesIsFalse_WhenDirectoryHasContent_CopiesFilesOnly()
        {
            //Arrange
            DispatchedEventArgs[] expectedDispatchedItems = new DispatchedEventArgs[]
            {
                new DispatchedEventArgs(DateTime.Now, testName, TestFolderPath, Path.Combine(TargetFolderPath, FolderName), TaskBase.SuccessResult)
            };
            task.CopySubdirectories = false;
            CreateTestFile(TestFolderPath, FileName);
            string subdirectory = Directory.CreateDirectory(Path.Combine(TestFolderPath, FolderName)).FullName;
            CreateTestFile(subdirectory, FileName);
            //Act
            Trigger.RaiseReady(
                new ReadyEventArgs(new FileSystemEventArgs(WatcherChangeTypes.Created, MainTestDirectory, FolderName)));

            string targetSubdirectoryPath = Path.Combine(TargetFolderPath, FolderName, FolderName);
            Func<bool> expectedResultPredicate = () =>
                Directory.Exists(Path.Combine(TargetFolderPath, FolderName)) &&
                File.Exists(Path.Combine(TargetFolderPath, FolderName, FileName)) &&
                !Directory.Exists(targetSubdirectoryPath) &&
                !File.Exists(Path.Combine(targetSubdirectoryPath, FileName));

            WaitForResult(DefaultDelayTimeoutMillis, expectedResultPredicate);
            //Assert
            Assert.IsTrue(expectedResultPredicate());
            AssertDispatchedEventArgsItemsAreEquivalent(expectedDispatchedItems, dispatchedItems);
        }
    }
}

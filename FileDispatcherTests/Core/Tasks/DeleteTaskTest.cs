using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcherTests.Core.Tasks.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core.Tasks
{
    [TestClass]
    [Serializable]
    public class DeleteTaskTest : TaskBaseTestBase<DeleteTask>
    {

        protected override IEnumerable<DispatchedEventArgs> ExpectedDispatchPerformedEventArgs()
        {
            yield return new DispatchedEventArgs(DateTime.Now, testName, TestFilePath, null, TaskBase.SuccessResult);
            yield return new DispatchedEventArgs(DateTime.Now, testName, TestFolderPath, null, TaskBase.SuccessResult);
        }

        protected override IEnumerable<ReadyEventArgs> DispatchReadyItems()
        {
            yield return GetReadyEventArgs(MainTestDirectory, FileName);
            yield return GetReadyEventArgs(MainTestDirectory, FolderName);
        }

        protected override IEnumerable<Func<bool>> ExpectedResultPredicates()
        {
            yield return () => !File.Exists(TestFilePath);
            yield return () => !Directory.Exists(TestFolderPath);
        }
        
        [TestMethod]
        public void DeleteRecursiveIsTrue_WhenDirectoryHasContent_DeletesPath()
        {
            //Arrange
            DispatchedEventArgs[] expectedDispatchedItems = new DispatchedEventArgs[]
            {
                new DispatchedEventArgs(DateTime.Now, testName, TestFolderPath, null, TaskBase.SuccessResult)
            };
            task.DeleteRecursive = true;
            CreateTestFile(TestFolderPath, FileName);
            string subdirectory = Directory.CreateDirectory(Path.Combine(TestFolderPath, FolderName)).FullName;
            CreateTestFile(subdirectory, FileName);
            //Act
            Trigger.RaiseReady(
                new ReadyEventArgs(new FileSystemEventArgs(WatcherChangeTypes.Created, MainTestDirectory, FolderName)));
            WaitForResult(DefaultDelayTimeoutMillis, () => !Directory.Exists(TestFolderPath));
            //Assert
            Assert.IsFalse(Directory.Exists(TestFolderPath));
            AssertDispatchedEventArgsItemsAreEquivalent(expectedDispatchedItems, dispatchedItems);
        }

        [TestMethod]
        public void DeleteRecursiveIsFalse_WhenDirectoryHasContent_DeleteNotPerformed()
        {
            //Arrange
            task.DeleteRecursive = false;
            CreateTestFile(TestFolderPath, FileName);
            string subdirectory = Directory.CreateDirectory(Path.Combine(TestFolderPath, FolderName)).FullName;
            CreateTestFile(subdirectory, FileName);
            //Act
            Trigger.RaiseReady(
                new ReadyEventArgs(new FileSystemEventArgs(WatcherChangeTypes.Created, MainTestDirectory, FolderName)));
            //Wait for the result
            Thread.Sleep(1000);
            //Assert
            Assert.IsTrue(Directory.Exists(TestFolderPath));
            var dispatchedEventArgs = dispatchedItems.Single();
            Assert.AreEqual(dispatchedEventArgs.SourcePath, TestFolderPath);
            Assert.AreNotEqual(dispatchedEventArgs.ResultDescription, TaskBase.SuccessResult);
        }
    }
}

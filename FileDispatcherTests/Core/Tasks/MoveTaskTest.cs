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
    public class MoveTaskTest : TargetableTaskBaseTestBase<MoveTask>
    {
        const string contentFolderName = "ContentFolder";
        string _contentFolderPath;
        string _contentFolderSubdirectoryPath;

        protected override void PrepareTestItems()
        {
            base.PrepareTestItems();
            _contentFolderPath = Directory.CreateDirectory(Path.Combine(MainTestDirectory, contentFolderName)).FullName;
            CreateTestFile(_contentFolderPath, FileName);
            _contentFolderSubdirectoryPath = Directory.CreateDirectory(Path.Combine(_contentFolderPath, FolderName)).FullName;
            CreateTestFile(_contentFolderSubdirectoryPath, FileName);
        }

        protected override IEnumerable<ReadyEventArgs> DispatchReadyItems()
        {
            yield return GetReadyEventArgs(MainTestDirectory, FolderName);
            yield return GetReadyEventArgs(MainTestDirectory, FileName);
            yield return GetReadyEventArgs(MainTestDirectory, contentFolderName);
        }

        protected override IEnumerable<DispatchedEventArgs> ExpectedDispatchPerformedEventArgs()
        {
            yield return new DispatchedEventArgs(DateTime.Now, testName, TestFolderPath, Path.Combine(TargetFolderPath, FolderName), TaskBase.SuccessResult);
            yield return new DispatchedEventArgs(DateTime.Now, testName, TestFilePath, Path.Combine(TargetFolderPath, FileName), TaskBase.SuccessResult);
            yield return new DispatchedEventArgs(DateTime.Now, testName, _contentFolderPath, Path.Combine(TargetFolderPath, contentFolderName), TaskBase.SuccessResult);
        }

        protected override IEnumerable<Func<bool>> ExpectedResultPredicates()
        {
            yield return () => !Directory.Exists(TestFolderPath) &&
                Directory.Exists(Path.Combine(TargetFolderPath, FolderName));
            yield return () => !File.Exists(TestFilePath) &&
                File.Exists(Path.Combine(TargetFolderPath, FileName));
            yield return () =>
                !Directory.Exists(_contentFolderPath) &&
                Directory.Exists(Path.Combine(TargetFolderPath, contentFolderName)) &&
                !Directory.Exists(_contentFolderSubdirectoryPath) &&
                Directory.Exists(Path.Combine(TargetFolderPath, contentFolderName, FolderName)) &&
                !File.Exists(Path.Combine(_contentFolderPath, FileName)) &&
                File.Exists(Path.Combine(TargetFolderPath, contentFolderName, FileName)) &&
                !File.Exists(Path.Combine(_contentFolderSubdirectoryPath, FileName)) &&
                File.Exists(Path.Combine(TargetFolderPath, contentFolderName, FolderName, FileName));

        }
    }
}

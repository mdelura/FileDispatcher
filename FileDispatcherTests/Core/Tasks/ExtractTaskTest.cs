using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FileDispatcher.Core;
using FileDispatcher.Core.Tasks;
using FileDispatcherTests.Core.Tasks.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core.Tasks
{
    [TestClass]
    [Serializable]
    public class ExtractTaskTest : TargetableTaskBaseTestBase<ExtractTask>
    {
        const string archiveName = "archive.zip";

        string[] _filesToBeExtracted = new string[] { "extract1.txt", "extract2.txt" };
        string[] _filesToBeIgnored = new string[] { "donotextract1.txt", "donotextract2.txt" };

        string _archivePath;

        [TestInitialize]
        public override void TestInit()
        {
            base.TestInit();
            task.ExtractionPreferenceFilters.Filters.Add("*extract*.txt");
            task.ExtractionPreferenceFilters.ExclusionFilters.Add("donotextract*");
        }

        protected override void PrepareTestItems()
        {
            base.PrepareTestItems();
            //Create files
            foreach (var fileName in _filesToBeExtracted.Concat(_filesToBeIgnored))
            {
                CreateTestFile(TestFolderPath, fileName);
            }
            _archivePath = Path.Combine(MainTestDirectory, archiveName);
            //Create zip archive
            ZipFile.CreateFromDirectory(TestFolderPath, _archivePath);
        }

        protected override IEnumerable<ReadyEventArgs> DispatchReadyItems()
        {
            yield return GetReadyEventArgs(MainTestDirectory, archiveName);
        }

        protected override IEnumerable<DispatchedEventArgs> ExpectedDispatchPerformedEventArgs()
        {
            foreach (var fileName in _filesToBeExtracted)
            {
                yield return new DispatchedEventArgs(DateTime.Now, testName, _archivePath, Path.Combine(TargetFolderPath, fileName), TaskBase.SuccessResult);
            }
        }

        protected override IEnumerable<Func<bool>> ExpectedResultPredicates()
        {
            foreach (var fileName in _filesToBeExtracted)
            {
                yield return () => File.Exists(Path.Combine(TargetFolderPath, fileName));
            }

            foreach (var fileName in _filesToBeIgnored)
            {
                yield return () => !File.Exists(Path.Combine(TargetFolderPath, fileName));
            }
        }
    }
}

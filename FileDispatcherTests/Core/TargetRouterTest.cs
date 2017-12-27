using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileDispatcher.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core
{
    [TestClass]
    public class TargetRouterTest
    {
        const string directoryItemName = "Lib_0001";
        const string fileItemName = "Test Library.xml";
        const string noHighMatchFileName = "LibraryDoc.xml";
        const string highMatchFileName = "Test.Library.x86.xml";


        static string[] sourceItems = new string[]
        {
            directoryItemName,
            fileItemName
        };

        static string[] subdirectories = new string[]
        {
            "Test",
            "Libraries",
            "Test Library"
        };

        static string[] files = new string[]
        {
            highMatchFileName,
            "TestLib.dat",
        };

        static string sourceDirectoryItemPath;
        static string sourceFileItemPath;
        static string sourceNoHighMatchFilePath;

        static string baseTargetPath;

        static TargetRouter _targetRouter;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            sourceDirectoryItemPath = Directory.CreateDirectory(Path.Combine(CreateTestDirectory(), directoryItemName)).FullName;
            sourceFileItemPath = CreateTestFile(CreateTestDirectory(), fileItemName);
            sourceNoHighMatchFilePath = CreateTestFile(sourceDirectoryItemPath, noHighMatchFileName);

            baseTargetPath = CreateTestDirectory();

            //Create subdirectories in target path
            foreach (var subdir in subdirectories)
            {
                Directory.CreateDirectory(Path.Combine(baseTargetPath, subdir));
            }
            //Create files in target path
            foreach (var file in files)
            {
                CreateTestFile(baseTargetPath, file);
            }
        }

        [TestInitialize]
        public void TestInit()
        {
            _targetRouter = new TargetRouter();
            _targetRouter.BaseTargetPath = baseTargetPath;
        }

        [TestMethod]
        public void GetTargetPath_MatchNever_ReturnsExpectedPath()
        {
            //Arrange
            _targetRouter.MatchSubdirectory = Match.Never;
            _targetRouter.MatchToSimilarFile = Match.Never;

            var expectedPaths = new string[]
            {
                Path.Combine(baseTargetPath, directoryItemName),
                Path.Combine(baseTargetPath, fileItemName)
            };

            //Act
            var actualPaths = sourceItems
                .Select(i => _targetRouter.GetTargetPath(i)).
                ToArray();

            //Assert
            AssertCollectionsAreEqual(expectedPaths, actualPaths);
        }

        [TestMethod]
        public void GetTargetPath_MatchAlways_NoItems_ReturnsExpectedPath()
        {
            //Arrange
            _targetRouter.BaseTargetPath = CreateTestDirectory();
            _targetRouter.MatchSubdirectory = Match.Always;
            _targetRouter.MatchToSimilarFile = Match.Always;

            var items = new Tuple<string, string>[]
            {
                new Tuple<string, string>(sourceFileItemPath, Path.Combine(_targetRouter.BaseTargetPath, fileItemName)),
                new Tuple<string, string>(sourceDirectoryItemPath, Path.Combine(_targetRouter.BaseTargetPath, directoryItemName))
            }
            .Select(t => new
            {
                SourcePath = t.Item1,
                ExpectedValue = t.Item2
            })
            .ToArray();

            var expectedItems = items
                .Select(i => i.ExpectedValue);

            //Act
            var actualItems = items
                .Select(i => _targetRouter.GetTargetPath(i.SourcePath));

            //Assert
            AssertCollectionsAreEqual(expectedItems, actualItems);
        }

        [TestMethod]
        public void GetTargetPath_MatchToSimilarFile_ReturnsExpectedPath()
        {
            //Arrange
            _targetRouter.MatchSubdirectory = Match.Never;

            string expectedPath = Path.Combine(baseTargetPath, "Test.Library.x86");
            
            var testItems = new Tuple<Match, string, string>[]
            {
                new Tuple<Match, string, string>(Match.Always, sourceFileItemPath, Path.Combine(baseTargetPath, highMatchFileName)),
                new Tuple<Match, string, string>(Match.HighMatchOnly, sourceFileItemPath, Path.Combine(baseTargetPath, highMatchFileName)),
                new Tuple<Match, string, string>(Match.HighMatchOnly, sourceNoHighMatchFilePath, Path.Combine(baseTargetPath, noHighMatchFileName))
            }
            .Select(t => new
            {
                MatchFileValue = t.Item1,
                SourcePath = t.Item2,
                ExpectedPath = t.Item3
            })
            .ToArray();

            var expectedPaths = testItems
                .Select(i => i.ExpectedPath);
            //Act
            var actualPaths = new List<string>();
            foreach (var item in testItems)
            {
                _targetRouter.MatchToSimilarFile = item.MatchFileValue;
                actualPaths.Add(_targetRouter.GetTargetPath(item.SourcePath));
            }

            //Assert
            AssertCollectionsAreEqual(expectedPaths, actualPaths);
        }

        [TestMethod]
        public void GetTargetPath_MatchSubdirectory_ReturnsExpectedPath()
        {
            //Arrange
            var testItems = new Tuple<Match, Match, string, string>[]
            {
                new Tuple<Match, Match, string, string>(
                    Match.Always, Match.Never, sourceDirectoryItemPath, Path.Combine(baseTargetPath, "Libraries", directoryItemName)),
                new Tuple<Match, Match, string, string>(
                    Match.HighMatchOnly, Match.Never, sourceDirectoryItemPath, Path.Combine(baseTargetPath, directoryItemName)),
            }
            .Select(t => new
            {
                MatchSubdirectory = t.Item1,
                MatchFile = t.Item2,
                SourcePath = t.Item3,
                ExpectedPath = t.Item4
            })
            .ToArray();

            var expectedPaths = testItems
                .Select(i => i.ExpectedPath);
            //Act
            var actualPaths = new List<string>();
            foreach (var item in testItems)
            {
                _targetRouter.MatchSubdirectory = item.MatchSubdirectory;
                _targetRouter.MatchToSimilarFile = item.MatchFile;
                actualPaths.Add(_targetRouter.GetTargetPath(item.SourcePath));
            }

            //Assert
            AssertCollectionsAreEqual(expectedPaths, actualPaths);
        }

    }
}

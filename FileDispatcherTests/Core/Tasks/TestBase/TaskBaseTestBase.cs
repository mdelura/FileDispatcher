using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileDispatcher.Core;
using FileDispatcher.Core.ExtensionMethods.SerializationExtensions;
using FileDispatcher.Core.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core.Tasks.TestBase
{
    [TestClass]
    [Serializable]
    public abstract class TaskBaseTestBase<TTask> where TTask : TaskBase
    {
        protected const string testName = "Test Name";

        /// <summary>
        /// An instnace of tested <see cref="TaskBase"/>
        /// </summary>
        protected TTask task;

        /// <summary>
        /// List of <see cref="DispatchedEventArgs"/> from <see cref="TaskBase.Dispatched"/> event.
        /// </summary>
        protected List<DispatchedEventArgs> dispatchedItems = new List<DispatchedEventArgs>();

        /// <summary>
        /// Name of the file to be used in tests.
        /// </summary>
        public const string FileName = "test.txt";
        /// <summary>
        /// Name of the folder to be used in tests.
        /// </summary>
        public const string FolderName = "TestDir";

        /// <summary>
        /// Main test directory. By default a new directory is initialized for each test.
        /// </summary>
        public string MainTestDirectory { get; private set; }
        /// <summary>
        /// Path of the test folder located in <see cref="MainTestDirectory"/>
        /// </summary>
        public string TestFolderPath { get; private set; }
        /// <summary>
        /// Path of the test file located in <see cref="MainTestDirectory"/>
        /// </summary>
        public string TestFilePath { get; private set; }
        /// <summary>
        /// Test <see cref="ITrigger"/>
        /// </summary>
        internal TestTrigger Trigger { get; private set; } = new TestTrigger();

        /// <summary>
        /// When implemented should provide expected <see cref="DispatchedEventArgs"/> 
        /// for the <see cref="Trigger_Ready_PerformsExpectedDispatch"/> test.
        /// </summary>
        /// <returns>Expected DispatchedEventArgs</returns>
        protected abstract IEnumerable<DispatchedEventArgs> ExpectedDispatchPerformedEventArgs();
        /// <summary>
        /// When implemented should provide <see cref="ReadyEventArgs"/> of items to be dispatched 
        /// in <see cref="Trigger_Ready_PerformsExpectedDispatch"/> test.
        /// for the <see cref="Trigger_Ready_PerformsExpectedDispatch"/> test.
        /// </summary>
        /// <returns>Source paths of items to be dispatched.</returns>
        protected abstract IEnumerable<ReadyEventArgs> DispatchReadyItems();
        /// <summary>
        /// When implemented should provide expected result predicate functions
        /// for the <see cref="Trigger_Ready_PerformsExpectedDispatch"/> test.
        /// </summary>
        /// <returns>Expected result predicate functions</returns>
        protected abstract IEnumerable<Func<bool>> ExpectedResultPredicates();

        [TestInitialize]
        public virtual void TestInit()
        {
            PrepareTestItems();

            task = (TTask)Activator.CreateInstance(typeof(TTask), Trigger);
            task.Dispatched += (s, e) => dispatchedItems.Add(e);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            task.Dispose();
            dispatchedItems.Clear();
        }

        [TestMethod]
        public void Trigger_Ready_PerformsExpectedDispatch()
        {
            //Arrange 
            var expectedDispatchedItems = ExpectedDispatchPerformedEventArgs();
            //Act
            foreach (var readyEventArgs in DispatchReadyItems())
            {
                Trigger.RaiseReady(readyEventArgs);
            }
            //Introduce delay if necessary to see the effect in filesystem.
            WaitForResult(DefaultDelayTimeoutMillis, () => ExpectedResultPredicates().All(p => p() == true));
            //Assert
            foreach (var predicate in ExpectedResultPredicates())
            {
                Assert.IsTrue(predicate());
            }
            AssertDispatchedEventArgsItemsAreEquivalent(expectedDispatchedItems, dispatchedItems);
        }

        [TestMethod]
        public void Deserialized_Trigger_Ready_PerformsExpectedDispatch()
        {
            //Arrange
            string serializationPath = (Path.Combine(CreateTestDirectory(), "serialized.bin"));
            task.SerializeToFile(serializationPath);
            task.Dispose();
            task = serializationPath.DeserializeFromFile<TTask>();
            task.Dispatched += (s, e) => dispatchedItems.Add(e);
            //Act and Assert - simply call Trigger_Ready_PerformsExpectedDispatch test again
            Trigger_Ready_PerformsExpectedDispatch();
        }

        /// <summary>
        /// Assert that two <see cref="IEnumerable{DispatchedEventArgs}"/> collection items are equivalent, i.e. their properties, excluding are equal 
        /// </summary>
        /// <param name="expectedDispatchedEventArgs">Expected items</param>
        /// <param name="actualDispatchedEventArgs">Actual items</param>
        internal void AssertDispatchedEventArgsItemsAreEquivalent(IEnumerable<DispatchedEventArgs> expectedDispatchedEventArgs, 
            IEnumerable<DispatchedEventArgs> actualDispatchedEventArgs)
        {
            Func<DispatchedEventArgs, string> dispatchedInfo =
                (e) =>
                $"{nameof(DispatchedEventArgs.SourcePath)}: {e.SourcePath}, {nameof(DispatchedEventArgs.TargetPath)}: {e.TargetPath}, " +
                $"{nameof(DispatchedEventArgs.ResultDescription)}: {e.ResultDescription}";
            var expected = expectedDispatchedEventArgs
                .Select(e => dispatchedInfo(e))
                .ToArray();
            var actual = actualDispatchedEventArgs
                .Select(e => dispatchedInfo(e))
                .ToArray();
            CollectionAssert.AreEquivalent(expected, actual, $"\r\nExpected:\r\n  {string.Join("\r\n  ", expected)}\r\nActual:\r\n  {string.Join("\r\n  ", actual)}\r\n");
        }


        /// <summary>
        /// Prepare test files and directories according to defined constants and provide the paths in <see cref="MainTestDirectory"/>,
        /// <see cref="TestFolderPath"/> and <see cref="TestFilePath"/>
        /// </summary>
        protected virtual void PrepareTestItems()
        {
            MainTestDirectory = CreateTestDirectory();
            TestFolderPath = Directory.CreateDirectory(Path.Combine(MainTestDirectory, FolderName)).FullName;
            TestFilePath = CreateTestFile(MainTestDirectory, FileName);
        }

        /// <summary>
        /// Get an instance of <see cref="ReadyEventArgs"/> for provided <paramref name="directory"/> and <paramref name="name"/>
        /// </summary>
        /// <param name="directory">The root directory</param>
        /// <param name="name">The name of the ready file or directory.</param>
        /// <returns><see cref="ReadyEventArgs"/> for provided <paramref name="directory"/> and <paramref name="name"/></returns>
        protected ReadyEventArgs GetReadyEventArgs(string directory, string name)
        {
            return new ReadyEventArgs(new FileSystemEventArgs(WatcherChangeTypes.Created, directory, name));
        }

        [Serializable]
        public class TestTrigger : ITrigger
        {
            public bool Enabled { get; set; }

            public event EventHandler<ReadyEventArgs> Ready;

            public void Dispose()
            {
            }

            public void RaiseReady(ReadyEventArgs readyEventArgs) => Ready?.Invoke(this, readyEventArgs);
        }
    }
}

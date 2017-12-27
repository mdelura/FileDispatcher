using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests
{
    [TestClass]
    public class TestUtil
    {
        const string testFolderName = "TempTestDir";

        /// <summary>
        /// Default delay timeout in milliseconds.
        /// </summary>
        public const int DefaultDelayTimeoutMillis = 5000;

        static int folderCount;

        static string testFolderPath;

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            testFolderPath = Path.Combine(context.TestDir, testFolderName);
            Directory.CreateDirectory(testFolderPath);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Directory.Delete(testFolderPath, true);
        }

        /// <summary>
        /// Create a new test directory and return its full path
        /// </summary>
        /// <returns>Full path of the created directory</returns>
        public static string CreateTestDirectory()
        {
            string path = Path.Combine(testFolderPath, folderCount++.ToString("0000"));

            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Gets a path for non-existing directory.
        /// </summary>
        /// <returns>Non-existing path.</returns>
        public static string GetNonExistingPath() => Path.Combine(CreateTestDirectory(), Guid.NewGuid().ToString("N"));

        /// <summary>
        /// Create an empty test file in provided <paramref name="directoryPath"/>
        /// </summary>
        /// <param name="directoryPath">Path where the file will be created</param>
        /// <param name="fileName">Name of the file</param>
        /// <returns>Full path of the created file</returns>
        public static string CreateTestFile(string directoryPath, string fileName)
        {
            string filePath = Path.Combine(directoryPath, fileName);

            using (var file = File.Create(filePath))
            {
                file.Flush();
                file.Close();
            }
            return filePath;
        }

        /// <summary>
        /// Get message with comparison of <paramref name="expectedItems"/> and <paramref name="actualItems"/>
        /// </summary>
        /// <param name="expectedItems">Collection of expected items</param>
        /// <param name="actualItems">Collection of actual items</param>
        /// <returns>Text containing listed expected and actual items.</returns>
        public static string GetItemsComparisonMessage<T>(IEnumerable<T> expectedItems, IEnumerable<T> actualItems)
        {
            return 
                $"\r\nExpected items:\r\n  {string.Join("\r\n  ", expectedItems)}\r\n" +
                $"Actual items:\r\n  {string.Join("\r\n  ", actualItems)}\r\n";
        }

        /// <summary>
        /// Assert two collections are equal and provide items comparison message if assert failed.
        /// </summary>
        /// <typeparam name="T">Type of collection items</typeparam>
        /// <param name="expectedItems">Collection of expected items</param>
        /// <param name="actualItems">Collection of actual items</param>
        public static void AssertCollectionsAreEqual<T>(IEnumerable<T> expectedItems, IEnumerable<T> actualItems)
        {
            CollectionAssert.AreEqual(expectedItems.ToArray(), actualItems.ToArray(), 
                GetItemsComparisonMessage(expectedItems, actualItems));
        }

        /// <summary>
        /// Wait for result of the task to be ready. A delay is required to see the results in the filesystem.
        /// </summary>
        /// <param name="delayTimeoutMillis">Maximum waiting time in milliseconds</param>
        /// <param name="resultAchieved">A function to determine if the expected result was achieved.</param>
        public static void WaitForResult(int delayTimeoutMillis, Func<bool> resultAchieved)
        {
            int currentDelay = 0;
            int step = delayTimeoutMillis / 10;
            while (!resultAchieved() && currentDelay < delayTimeoutMillis)
            {
                currentDelay += step;
                Thread.Sleep(step);
            }
        }

        /// <summary>
        /// Wait for result of the task to be ready. A delay is required to see the results in the filesystem.
        /// </summary>
        /// <param name="resultAchieved">A function to determine if the expected result was achieved.</param>
        public static void WaitForResult(Func<bool> resultAchieved) => WaitForResult(DefaultDelayTimeoutMillis, resultAchieved);
    }
}

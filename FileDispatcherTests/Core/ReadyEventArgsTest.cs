using System;
using System.IO;
using FileDispatcher.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core
{
    [TestClass]
    public class ReadyEventArgsTest
    {
        [TestMethod]
        public void Ctor_CreatesExpectedReadyEventArgs()
        {
            //Arrange
            string path = CreateTestDirectory();
            string name = "TestName";
            FileSystemEventArgs fileSystemEventArgs =
                new FileSystemEventArgs(WatcherChangeTypes.Created, path, name);

            string expectedFullPath = Path.Combine(path, name);
            //Act
            ReadyEventArgs readyEventArgs = new ReadyEventArgs(fileSystemEventArgs);
            //Assert
            Assert.AreEqual(expectedFullPath, readyEventArgs.FullPath);
            Assert.AreEqual(name, readyEventArgs.Name);
        }
    }
}

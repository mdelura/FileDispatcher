using System;
using FileDispatcher.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileDispatcherTests.Core
{
    [TestClass]
    public class DispatchedEventArgsTest
    {
        [TestMethod]
        public void ToString_ParseFromLogEntry_ReturnsExpectedDispatchLogEntry()
        {
            //Arrange
            DispatchedEventArgs dispatchedEventArgs = 
                new DispatchedEventArgs(DateTime.Parse(DateTime.Today.ToString()), "test name",
                Environment.CurrentDirectory, Environment.CurrentDirectory, "Test result description");
            //Act
            string logEntry = dispatchedEventArgs.ToString();
            DispatchedEventArgs parsedDispatchLogEntry = DispatchedEventArgs.ParseFromLogEntry(logEntry);
            //Assert
            Assert.AreEqual(logEntry, parsedDispatchLogEntry.ToString());
        }
    }
}

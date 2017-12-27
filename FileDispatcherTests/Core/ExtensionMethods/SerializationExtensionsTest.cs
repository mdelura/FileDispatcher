using System;
using System.IO;
using FileDispatcher.Core.ExtensionMethods.SerializationExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static FileDispatcherTests.TestUtil;

namespace FileDispatcherTests.Core.ExtensionMethods
{
    [TestClass]
    public class SerializationExtensionsTest
    {
        [TestMethod]
        public void SerializeToFile_DeserializeFromFile_ObjectsAreEqual()
        {
            //Arrange
            string serializationPath = Path.Combine(CreateTestDirectory(), "serialized.bin");

            SerializationMock item = new SerializationMock()
            {
                Number = 100,
                Text = "Test text"
            };

            //Act
            item.SerializeToFile(serializationPath);
            SerializationMock deserializedItem = serializationPath.DeserializeFromFile<SerializationMock>();
            
            //Assert
            Assert.AreEqual(item.Number, deserializedItem.Number);
            Assert.AreEqual(item.Text, deserializedItem.Text);
        }

        [Serializable]
        class SerializationMock
        {
            public int Number { get; set; }
            public string Text { get; set; }
        }
    }
}

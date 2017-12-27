using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileDispatcher.Core.ExtensionMethods.SerializationExtensions
{
    /// <summary>
    /// Provides extension methods for serialization operations
    /// </summary>
    static class SerializationExtensions
    {
        /// <summary>
        /// Serialize <typeparamref name="T"/> to file using <see cref="BinaryFormatter"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object to be serialized</typeparam>
        /// <param name="that">Object to be serialized</param>
        /// <param name="path">Target file path</param>
        public static void SerializeToFile<T>(this T that, string path)
        {
            IFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = File.Create(path))
                formatter.Serialize(fileStream, that);
        }

        /// <summary>
        /// Serialize <typeparamref name="T"/> from file path using <see cref="BinaryFormatter"/>.
        /// </summary>
        /// <typeparam name="T">Type of the object to be deserialized</typeparam>
        /// <param name="that">Path of a file to desrialize</param>
        /// <returns></returns>
        public static T DeserializeFromFile<T>(this string that)
        {
            IFormatter formatter = new BinaryFormatter();
            using (FileStream fileStream = File.OpenRead(that))
                return  (T)formatter.Deserialize(fileStream);
        }


    }
}

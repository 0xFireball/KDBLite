using MsgPack.Serialization;
using OmniBean.PowerCrypt4;
using OmniBean.PowerCrypt4.Utilities;
using System;
using System.IO;

namespace KDBLite
{
    public class KDBDatabase : IDisposable
    {
        private Stream _targetStream;
        private BinaryWriter _targetWriter;
        private BinaryReader _targetReader;
        public KDBDataStructure Tables { get; private set; }
        public string EncryptionKey { get; }

        public KDBDatabase(Stream targetStream, string encryptionKey = "")
        {
            EncryptionKey = encryptionKey;
            _targetStream = targetStream;
            _targetWriter = new BinaryWriter(_targetStream);
            _targetReader = new BinaryReader(_targetStream);
        }

        /// <summary>
        /// Initializes the database and removes all existing data
        /// </summary>
        public void FormatDatabase()
        {
            EmptyTargetStream();
            Tables = new KDBDataStructure();
        }

        private void EmptyTargetStream()
        {
            _targetStream.SetLength(0);
            _targetStream.Position = 0;
            _targetStream.Flush();
        }

        /// <summary>
        /// Loads any existing data into the database.
        /// </summary>
        public void LoadDatabase()
        {
            _targetStream.Position = 0;
            var encryptedData = _targetReader.ReadBytes((int)_targetReader.BaseStream.Length);
            var decryptedData = PowerAES.Decrypt(encryptedData.GetString(), EncryptionKey).GetBytes();
            using (var decryptedStream = new MemoryStream(decryptedData))
            {
                Tables = Serializer.Unpack(decryptedStream) as KDBDataStructure;
            }
        }

        /// <summary>
        /// Saves a database's data into the stream. If you create a new database instance, LoadDatabase() must be called to load the data.
        /// </summary>
        public void SaveDatabase()
        {
            EmptyTargetStream();
            var intermediateStream = new MemoryStream();
            Serializer.Pack(intermediateStream, Tables);

            //Encrypt stream
            var databaseData = intermediateStream.ToArray();
            var encryptedData = PowerAES.Encrypt(databaseData.GetString(), EncryptionKey).GetBytes();
            _targetWriter.Write(encryptedData);
            _targetWriter.Flush();
        }

        public void Dispose()
        {
            _targetWriter.Close();
            _targetReader.Close();
        }

        private MessagePackSerializer Serializer => SerializationContext.Default.GetSerializer<KDBDataStructure>();
    }
}
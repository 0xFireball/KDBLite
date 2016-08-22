using MsgPack.Serialization;
using Newtonsoft.Json;
using OmniBean.PowerCrypt4;
using OmniBean.PowerCrypt4.Utilities;
using System.IO;

namespace KDBLite
{
    public class KDBDatabase
    {
        private Stream _targetStream;
        private BinaryWriter _targetWriter;
        private BinaryReader _targetReader;
        public KDBDataStructure Tables { get; private set; }

        public DatabaseSerializationSystem SerializationSystem { get; }
        public string EncryptionKey { get; }

        public KDBDatabase(Stream targetStream, string encryptionKey = "", DatabaseSerializationSystem serializationSystem = DatabaseSerializationSystem.MsgPack)
        {
            EncryptionKey = encryptionKey;
            SerializationSystem = serializationSystem;

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
                switch (SerializationSystem)
                {
                    case DatabaseSerializationSystem.MsgPack:
                        Tables = Serializer.Unpack(decryptedStream) as KDBDataStructure;
                        break;

                    case DatabaseSerializationSystem.Json:
                        using (var decryptedStreamReader = new StreamReader(decryptedStream))
                        {
                            var jsonString = decryptedStreamReader.ReadToEnd();
                            Tables = JsonConvert.DeserializeObject<KDBDataStructure>(jsonString);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Saves a database's data into the stream. If you create a new database instance, LoadDatabase() must be called to load the data.
        /// </summary>
        public void SaveDatabase()
        {
            EmptyTargetStream();
            using (var intermediateStream = new MemoryStream())
            {
                switch (SerializationSystem)
                {
                    case DatabaseSerializationSystem.MsgPack:
                        Serializer.Pack(intermediateStream, Tables);
                        break;

                    case DatabaseSerializationSystem.Json:
                        var serializedObj = JsonConvert.SerializeObject(Tables);
                        using (var intermediateStreamWriter = new StreamWriter(intermediateStream))
                        {
                            intermediateStreamWriter.Write(serializedObj);
                        }
                        break;
                }

                //Encrypt stream
                var databaseData = intermediateStream.ToArray();
                var encryptedData = PowerAES.Encrypt(databaseData.GetString(), EncryptionKey).GetBytes();
                _targetWriter.Write(encryptedData);
                _targetWriter.Flush();
            }
        }

        private MessagePackSerializer Serializer => SerializationContext.Default.GetSerializer<KDBDataStructure>();
    }
}
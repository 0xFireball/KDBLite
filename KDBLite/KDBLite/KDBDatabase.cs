using MsgPack.Serialization;
using System.IO;
using System;

namespace KDBLite
{
    public class KDBDatabase
    {
        private Stream _targetStream;
        private StreamWriter _targetWriter;
        public KDBDataStructure Tables { get; private set; }

        public KDBDatabase(Stream targetStream)
        {
            _targetStream = targetStream;
            _targetWriter = new StreamWriter(_targetStream);
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
            Tables = Serializer.Unpack(_targetStream) as KDBDataStructure;
        }

        /// <summary>
        /// Saves a database's data into the stream. If you create a new database instance, LoadDatabase() must be called to load the data.
        /// </summary>
        public void SaveDatabase()
        {
            EmptyTargetStream();
            Serializer.Pack(_targetStream, Tables);
        }

        private MessagePackSerializer Serializer => SerializationContext.Default.GetSerializer<KDBDataStructure>();
    }
}
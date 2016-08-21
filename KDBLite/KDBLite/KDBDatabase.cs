using System.Collections.Generic;
using System.IO;

namespace KDBLite
{
    public class KDBDatabase
    {
        private Stream _targetStream;
        private StreamWriter _targetWriter;
        

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
        }

        /// <summary>
        /// Loads any existing data into the database.
        /// </summary>
        public void LoadDatabase()
        {
        }
    }
}
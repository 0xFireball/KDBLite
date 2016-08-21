using System.IO;

namespace KDBLite
{
    public class KDBRow
    {
        public string Identifier { get; set; }
        public byte[] Data { get; set; }
    }
}
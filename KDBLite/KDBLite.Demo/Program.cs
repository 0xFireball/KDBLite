using KDBLite.Utilities;
using System.IO;
using System.Linq;

namespace KDBLite.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var targetStream = File.Open("database.kdb", FileMode.Create, FileAccess.ReadWrite))
            {
                var cryptoKey = "somesillypassword";
                var sillyDatabaseToWrite = new KDBDatabase(targetStream, cryptoKey);
                sillyDatabaseToWrite.FormatDatabase();

                var sillyTableName = "sillytable";
                sillyDatabaseToWrite.Tables.Add(new KDBTable { Identifier = sillyTableName });
                var sillyTable = sillyDatabaseToWrite.Tables.Where(table => table.Identifier == sillyTableName).ToArray()[0]; //Query with linq
                var sillyRowIdentifier = "sillyrow";
                var someSillyObject = new SillyObject { Name = "Computer" };
                sillyTable.Rows.Add(new KDBRow().CreateFromPackedObject(someSillyObject).SetIdentifier(sillyRowIdentifier));

                sillyDatabaseToWrite.SaveDatabase();

                var sillyDatabaseToRead = new KDBDatabase(targetStream, cryptoKey);
                sillyDatabaseToRead.LoadDatabase();

                var sillyTableToRead = sillyDatabaseToRead.Tables.Where(table => table.Identifier == sillyTableName).ToArray()[0]; //Query with linq again
                var sillyRowToRead = sillyTableToRead.Rows.Where(row => row.Identifier == sillyRowIdentifier).ToArray()[0];
                var sillyObjectRead = sillyRowToRead.GetObjectFromPackedData<SillyObject>();
            }
        }
    }
}
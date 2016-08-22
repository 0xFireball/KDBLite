using Newtonsoft.Json;
using OmniBean.PowerCrypt4.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace KDBLite.Utilities
{
    public static class KDBUtilities
    {
        //Populates a row's stream from an object
        public static KDBRow CreateFromPackedObject(this KDBRow kdbRow, object objectToPack)
        {
            kdbRow.Data = JsonConvert.SerializeObject(objectToPack).GetBytes();
            return kdbRow;
        }

        public static T GetObjectFromPackedData<T>(this KDBRow kdbRow)
        {
            var obj = JsonConvert.DeserializeObject<T>(kdbRow.Data.GetString());
            return obj;
        }

        public static KDBRow SetIdentifier(this KDBRow kdbRow, string identifier)
        {
            kdbRow.Identifier = identifier;
            return kdbRow;
        }

        public static KDBTable CreateTableFromList<T>(this KDBTable table, IEnumerable<T> list)
        {
            table.Rows.Clear();
            table.Rows.AddRange(list.Select(obj => CreateFromPackedObject(new KDBRow(), obj)));
            return table;
        }

        public static List<T> CreateListFromTable<T>(this KDBTable table)
        {
            return table.Rows.Select(row => GetObjectFromPackedData<T>(row)).ToList();
        }
    }
}
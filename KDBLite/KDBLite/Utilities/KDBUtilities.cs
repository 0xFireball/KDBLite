using Newtonsoft.Json;
using OmniBean.PowerCrypt4.Utilities;

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
    }
}
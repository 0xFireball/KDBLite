using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDBLite
{
    public class KDBTable
    {
        public List<KDBRow> Rows { get; private set; } = new List<KDBRow>();
        public string Identifier;
    }
}

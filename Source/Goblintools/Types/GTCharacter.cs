using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblintools.Types
{
    public class GTCharacter
    {
        public string Name { get; set; }
        public string Account { get; set; }
        public string Realm { get; set; }
        public string Race { get; set; }
        public string Class { get; set; }
        public long Played { get; set; }
    }
}

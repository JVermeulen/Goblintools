using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblintools.Types
{
    public class GTLog
    {
        public string Format { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Count { get; set; }
        public List<string> Data { get; set; }

        public GTLog()
        {
            Data = new List<string>();
        }
    }
}

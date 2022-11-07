using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardHook_test
{
    public class Config
    {
        /*
        DOWN : 91   win
        DOWN : 160  L-shift
        DOWN : 52   4
        */
        public static Collection<int> keymap = new Collection<int>();
        public Config()
        {
            keymap.Add(91);
            keymap.Add(160);
            keymap.Add(52);
        }
    }
}

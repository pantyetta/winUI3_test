using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardHook_test
{
    public class KeyStatus
    {
        Config config = new Config();

        private Collection<int> Keys;
        public KeyStatus()
        {
            Keys = new Collection<int>();
        }

        public bool getKey(int key)
        {
            return Keys.Contains(key);
        }

        public void setKey(int key)
        {
            var isExist = getKey(key);
            if (!isExist) Keys.Add(key);
        }

        public void delKey(int key)
        {
            Keys.Remove(key);
        }

        public Collection<int> getKeys()
        {
            return Keys;
        }

        public bool compare()
        {
            if (Keys.Count < Config.keymap.Count) return false;
            int checkKeys = 0;
            foreach (var key in Keys)
            {
                if (Config.keymap.Contains(key)) checkKeys++;
                if(checkKeys == Config.keymap.Count) break;
            }
            return checkKeys == Config.keymap.Count ? true : false;
        }
    }
}

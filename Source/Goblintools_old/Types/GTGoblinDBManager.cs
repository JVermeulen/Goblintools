using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Goblintools.Types
{
    public class GTGoblinDBManager
    {
        public List<GTGoblinDB> Items { get; private set; }

        public GTGoblinDBManager()
        {
            Items = new List<GTGoblinDB>();
        }

        public void Add(GTGoblinDB goblinDB)
        {
            var exists = Items.Where(g => g?.ToString() == goblinDB?.ToString()).Any();

            if (!exists)
                Items.Add(goblinDB);
        }

        public GTGoblinDB[] GetCharacter(string account, string name)
        {
            return Items.Where(g => g.Character.Account == account && $"{g.Character.Realm} - {g.Character.Name}" == name).ToArray();
        }

        public string[] GetAccounts()
        {
            return Items.Select(g => g.Character.Account).Distinct().ToArray();
        }

        public string[] GetCharacterNames(string account)
        {
            return Items.Where(g => g.Character.Account == account).Select(g => $"{g.Character.Realm} - {g.Character.Name}").ToArray();
        }

        public void LoadFromFolder(string path, string account, string name)
        {
            
        }
    }
}

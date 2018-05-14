using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modding;

namespace GMG
{
    public class GMG : Mod
    {
        public override string GetVersion()
        {
            return "1";
        }

        public override void Initialize()
        {
            Log("Initializing");
            ModHooks.Instance.AfterSavegameLoadHook += SaveGame;
            ModHooks.Instance.NewGameHook += AddComponent;
        }

        private void SaveGame(SaveGameData data)
        {
            AddComponent();
        }

        private void AddComponent()
        {
            GameManager.instance.gameObject.AddComponent<Gun>();
        }
    }
}

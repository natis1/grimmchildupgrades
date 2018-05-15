using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;

namespace GrimmChildUpgrades
{
    public class GrimmChildUpgrades : Mod
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
            GameManager.instance.gameObject.AddComponent<GrimmChild>();
        }
    }
}

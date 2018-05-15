using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modding;
using infinitegrimm;

namespace GrimmchildUpgrades
{
    public class GrimmchildUpgrades : Mod
    {

        public static string version = "0.1";
        public readonly int loadOrder = 50;
        public bool IGAvailable;
        public static bool usingIG;

        // Version detection code originally by Seanpr, used with permission.
        public override string GetVersion()
        {
            string ver = version;
            int minAPI = 40;

            bool apiTooLow = Convert.ToInt32(ModHooks.Instance.ModVersion.Split('-')[1]) < minAPI;
            bool noModCommon = !(from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Namespace == "ModCommon" select type).Any();

            // report if the user has infinitegrimm.
            bool infgrimmint = (from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Namespace == "infinitegrimm" select type).Any();
            if (infgrimmint) ver += " + IG";
            if (apiTooLow) ver += " (Error: ModAPI too old)";
            if (noModCommon) ver += " (Error: Grimmchild Upgrades requires ModCommon)";

            return ver;
        }

        public override void Initialize()
        {
            IGAvailable = (from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Namespace == "infinitegrimm" select type).Any();
            if (IGAvailable)
            {
                Log("Thank you, infinite Grimm. Always great seeing you!");
                usingIG = true;
            }
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

        public override int LoadPriority()
        {
            return loadOrder;
        }

    }
}

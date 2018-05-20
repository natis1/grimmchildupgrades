using System;
using System.Linq;
using Modding;
using UnityEngine;
using System.IO;

namespace GrimmchildUpgrades
{
    public class GrimmchildUpgrades : Mod <GrimmchildSettings, GrimmchildGlobalSettings>, ITogglableMod
    {

        public static string version = "0.1.1";
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
            // if the hooks exist for this delete them.
            ModHooks.Instance.AfterSavegameLoadHook -= ResetCharmCost;
            ModHooks.Instance.NewGameHook -= ResetCharmCostNew;

            SetupSettings();


            IGAvailable = (from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Namespace == "infinitegrimm" select type).Any();
            if (IGAvailable && GlobalSettings.infiniteGrimmIntegration)
            {
                Log("Thank you, infinite Grimm. Always great seeing you!");
                usingIG = true;
            } else
            {
                usingIG = false;
            }
            GrimmChild.ballSizeCFG = GlobalSettings.maxBallSize;
            GrimmChild.FBSpeedModifierCFG = GlobalSettings.maxBallMoveSpeed;
            GrimmChild.maxDamageCFG = GlobalSettings.maxDamage;
            GrimmChild.notchesCostCFG = GlobalSettings.notchesCost;
            GrimmChild.usingIG = usingIG;
            GrimmChild.volumeMod = GlobalSettings.volumeMultiplier;
            GrimmChild.filterAlpha = GlobalSettings.colorAlpha;
            GrimmChild.filterBlue = GlobalSettings.colorBlue;
            GrimmChild.filterGreen = GlobalSettings.colorGreen;
            GrimmChild.filterRed = GlobalSettings.colorRed;
            GrimmChild.ghostBallCFG = GlobalSettings.ghostBalls;
            GrimmChild.rangeModifierCFG = GlobalSettings.maxRangeMult;
            GrimmChild.speedModifierCFG = GlobalSettings.maxAttackSpeedMult;


            ModHooks.Instance.AfterSavegameLoadHook += SaveGame;
            ModHooks.Instance.NewGameHook += AddComponent;

            ModHooks.Instance.ApplicationQuitHook += SaveGlobalSettings;
        }

        void SetupSettings()
        {
            string settingsFilePath = Application.persistentDataPath + ModHooks.PathSeperator + GetType().Name + ".GlobalSettings.json";

            bool forceReloadGlobalSettings = (GlobalSettings != null && GlobalSettings.SettingsVersion != VersionInfo.SettingsVer);

            if (forceReloadGlobalSettings || !File.Exists(settingsFilePath))
            {
                if (forceReloadGlobalSettings)
                {
                    Log("Settings outdated! Rebuilding.");
                }
                else
                {
                    Log("Settings not found, rebuilding... File will be saved to: " + settingsFilePath);
                }

                GlobalSettings.Reset();
            }
            SaveGlobalSettings();
        }

        private void SaveGame(SaveGameData data)
        {
            AddComponent();
        }

        private void AddComponent()
        {
            

            GameManager.instance.gameObject.AddComponent<GrimmChild>();
            GameManager.instance.gameObject.AddComponent<GrimmballFireReal>();

            
        }

        public override int LoadPriority()
        {
            return loadOrder;
        }

        public void Unload()
        {
            Log("Disabling! If you see any more non-settings messages by this mod please report as an issue.");
            ModHooks.Instance.AfterSavegameLoadHook -= SaveGame;
            ModHooks.Instance.NewGameHook -= AddComponent;

            // this will let you uninstall
            ModHooks.Instance.AfterSavegameLoadHook += ResetCharmCost;
            ModHooks.Instance.NewGameHook += ResetCharmCostNew;

            
        }

        // a separate function shouldn't be needed for a new game but is because modapi sucks
        private void ResetCharmCostNew()
        {
            GameManager.instance.gameObject.AddComponent<GrimmReset>();
        }

        private void ResetCharmCost(SaveGameData data)
        {
            ResetCharmCostNew();
        }
    }
}

using Modding;

namespace GrimmchildUpgrades
{
    public class VersionInfo
    {
        readonly public static int SettingsVer = 1;
    }

    public class GrimmchildGlobalSettings : IModSettings
    {


        public void Reset()
        {
            BoolValues.Clear();
            IntValues.Clear();
            FloatValues.Clear();
            StringValues.Clear();

            infiniteGrimmIntegration = true;

            maxAttackSpeedMult = 3.0f;
            maxRangeMult = 2.0f;
            maxFBMoveSpeed = 3.0f;

            maxDamage = 20;
            notchesCost = 4;

            volumeMultiplier = 0.6f;

            colorAlpha = 1.0f;
            colorBlue = 1.0f;
            colorRed = 1.0f;
            colorGreen = 1.0f;

            SettingsVersion = VersionInfo.SettingsVer;
        }
        public int SettingsVersion { get => GetInt(); set => SetInt(value); }

        public bool infiniteGrimmIntegration { get => GetBool(); set => SetBool(value); }
        public float maxAttackSpeedMult { get => GetFloat(); set => SetFloat(value); }
        public float maxFBMoveSpeed { get => GetFloat(); set => SetFloat(value); }
        public float maxRangeMult { get => GetFloat(); set => SetFloat(value); }

        public int maxDamage { get => GetInt(); set => SetInt(value); }
        public int notchesCost { get => GetInt(); set => SetInt(value); }

        public float volumeMultiplier { get => GetFloat(); set => SetFloat(value); }

        public float colorRed { get => GetFloat(); set => SetFloat(value); }
        public float colorGreen { get => GetFloat(); set => SetFloat(value); }
        public float colorBlue { get => GetFloat(); set => SetFloat(value); }
        public float colorAlpha { get => GetFloat(); set => SetFloat(value); }

        public bool HardMode { get => GetBool(); set => SetBool(value); }
    }


    public class GrimmchildSettings : IModSettings
    {
        // none needed

    }



}

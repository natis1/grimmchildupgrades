using Modding;

namespace GrimmchildUpgrades
{
    public class VersionInfo
    {
        readonly public static int SettingsVer = 4;
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
            ghostBalls = true;

            maxAttackSpeedMult = 2.5f;
            maxRangeMult = 2.0f;
            maxBallMoveSpeed = 1.5f;
            maxBallSize = 2.0f;

            maxDamage = 35;
            notchesCost = 6;
            maxSoulAdd = 33;

            volumeMultiplier = 0.6f;

            colorAlpha = 1.0f;
            colorBlue = 1.0f;
            colorRed = 1.0f;
            colorGreen = 1.0f;

            SettingsVersion = VersionInfo.SettingsVer;
        }
        public int SettingsVersion { get => GetInt(); set => SetInt(value); }

        public bool infiniteGrimmIntegration { get => GetBool(); set => SetBool(value); }
        public bool ghostBalls { get => GetBool(); set => SetBool(value); }
        public float maxAttackSpeedMult { get => GetFloat(); set => SetFloat(value); }
        public float maxBallMoveSpeed { get => GetFloat(); set => SetFloat(value); }
        public float maxRangeMult { get => GetFloat(); set => SetFloat(value); }
        public float maxBallSize { get => GetFloat(); set => SetFloat(value); }

        public int maxDamage { get => GetInt(); set => SetInt(value); }
        public int notchesCost { get => GetInt(); set => SetInt(value); }
        public int maxSoulAdd { get => GetInt(); set => SetInt(value); }

        public float volumeMultiplier { get => GetFloat(); set => SetFloat(value); }

        public float colorRed { get => GetFloat(); set => SetFloat(value); }
        public float colorGreen { get => GetFloat(); set => SetFloat(value); }
        public float colorBlue { get => GetFloat(); set => SetFloat(value); }
        public float colorAlpha { get => GetFloat(); set => SetFloat(value); }
    }


    public class GrimmchildSettings : IModSettings
    {
        // none needed

    }



}

using UnityEngine;
using Modding;

namespace GrimmchildUpgrades
{
    class GrimmReset : MonoBehaviour
    {

        public void Start()
        {
            ModHooks.Instance.CharmUpdateHook += resetCharms;
        }

        private void resetCharms(PlayerData data, HeroController controller)
        {
            PlayerData.instance.charmCost_40 = 2;
            PlayerData.instance.CalculateNotchesUsed();
        }

        public void OnDestroy()
        {
            ModHooks.Instance.CharmUpdateHook -= resetCharms;
        }
    }
}

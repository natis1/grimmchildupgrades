using System;
using UnityEngine;

namespace GrimmchildUpgrades
{
    class CharmFix : MonoBehaviour
    {

        
        public void FixCharms()
        {
            // fuck TC. I love your game but goddamn screw this code
            int notchesUsed = 0;
            for (int i = 1; i < 41; i++)
            {
                if (PlayerData.instance.GetBool("equippedCharm_" + i))
                {
                    notchesUsed += PlayerData.instance.GetInt("charmCost_" + i);
                }
            }
            PlayerData.instance.SetInt("charmSlotsFilled", notchesUsed);
            if (PlayerData.instance.GetInt("charmSlotsFilled") > PlayerData.instance.GetInt("charmSlots"))
            {
                PlayerData.instance.SetBool("overcharmed", true);
            }
            else
            {
                PlayerData.instance.SetBool("overcharmed", false);
            }

            Log("Fixed charms from fsm");

        }

        public void Log(String str)
        {
            Modding.Logger.Log("[GrimmchildUpgrades] " + str);
        }
    }
}

using Modding;
using HutongGames.PlayMaker.Actions;
using static FsmUtil.FsmUtil;
using UnityEngine;
using System;
using ModCommon;
using HutongGames.PlayMaker;
using infinitegrimm;

namespace GrimmchildUpgrades
{
    class GrimmChild : MonoBehaviour
    {
        public GameObject grimmchild;
        public PlayMakerFSM gcFSM;
        public Wait IdleAction;
        public SetFloatValue fValue;
        public bool done;

        private float baseSpeed;
        private float baseFBSpeed;
        private float baseRange;


        public int IGMaxDamage;
        public int powerLevel;

        // these are variables someone can set in the
        // config file
        public static double speedModifier;
        public static double rangeModifier;
        public static double FBSpeedModifier;

        public static int maxDamage;
        public static int notchesCost;

        public static float volumeMod;
        public static float filterRed;
        public static float filterGreen;
        public static float filterBlue;
        public static float filterAlpha;

        public readonly string[] speedanimations = { "Idle 4", "Antic 4", "Shoot 4" };

        public readonly string[] waitstates = { "Follow" };
        public readonly float[] waitstateTimes = { 0.25f };


        // animations: Idle 4, Antic 4, Shoot 4
        // wait states: Follow (0.25)

        // fsm variables: Attack Timer 1.482669

        public void Start()
        {
            baseSpeed = -5.0f;
            ModHooks.Instance.BeforeSceneLoadHook += reset;
        }

        private string reset(string sceneName)
        {
            done = false;
            getIGDamage();
            return sceneName;
        }

        public void Update()
        {
            if (done && grimmchild != null)
            {
                return;
            }

            grimmchild = GameObject.FindGameObjectWithTag("Grimmchild");
            if (grimmchild == null)
            {
                return;
            }

            Log("Loaded Grimmchild");



            gcFSM = FSMUtility.LocateFSM(grimmchild, "Control");

            //gcFSM.FsmVariables.GetFsmInt("Attack Timer") = 5;

            GameObject gcRangeObj = grimmchild.FindGameObjectInChildren("Enemy Range");
            CircleCollider2D gcRange = gcRangeObj.GetComponent<CircleCollider2D>();

            // 7.81???
            //Log("GrimmRange is " + gcRange.radius );

            // 0.3763812
            //Log("Attack Timer is " + gcFSM.FsmVariables.GetFsmFloat("Attack Timer"));



            ///////////////////////////////////////////////////////////////////
            // This doesn't work, attempted to extend enemy range
            ///////////////////////////////////////////////////////////////////
            //FsmFloat floaty = gcFSM.FsmVariables.GetFsmFloat("GrimmEnemyRange");
            //if (floaty != null)
            //{
            //    floaty.Value *= 1000f;
            //}
            //else
            //{
            //    FsmInt re = gcFSM.FsmVariables.GetFsmInt("GrimmEnemyRange");
            //    if (re != null) { re.Value *= 1000; }

            //}

            //ChangeTransition(gcFSM, "Shoot", "FINISHED", "Check For Target");
            //ChangeTransition(gcFSM, "Shoot", "CANCEL", "Check For Target");
            done = true;
            Log("Done.");
        }

        public void Log(String str)
        {
            Modding.Logger.Log("[GrimmchildUpgrades] " + str);
        }

        public void OnDestroy()
        {
            ModHooks.Instance.BeforeSceneLoadHook -= reset;
        }

        public void getIGDamage()
        {
            if (GrimmchildUpgrades.usingIG)
            {
                try
                {
                    IGMaxDamage = InfiniteGlobalVars.maximumDamage;
                }
                catch
                {
                    IGMaxDamage = 100000;
                    Log("Unable to load infinite Grimm damage. Is Infinite Grimm up to date?");
                    powerLevel = 6;
                    return;
                }
            }
            else
            {
                IGMaxDamage = 100000;
                powerLevel = 6;
                return;
            }

            if (IGMaxDamage < 2000)
            {
                powerLevel = 1;
            }
            else if (IGMaxDamage < 4000)
            {
                powerLevel = 2;
            }
            else if (IGMaxDamage < 6000)
            {
                powerLevel = 3;
            }
            else if (IGMaxDamage < 10000)
            {
                powerLevel = 4;
            }
            else if (IGMaxDamage < 15000)
            {
                powerLevel = 5;
            }
            else
            {
                powerLevel = 6;
            }
        }

    }
}
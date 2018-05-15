using Modding;
using HutongGames.PlayMaker.Actions;
using static FsmUtil.FsmUtil;
using UnityEngine;
using System;
using ModCommon;
using HutongGames.PlayMaker;
using infinitegrimm;
using RandomizerMod.Extensions;

namespace GrimmchildUpgrades
{
    class GrimmChild : MonoBehaviour
    {
        public GameObject grimmchild;
        public PlayMakerFSM gcFSM;
        public Wait IdleAction;
        public SetFloatValue fValue;
        public bool done;

        private float baseFireInterval;
        private float baseFBSpeed;
        private float baseRange;


        public int IGMaxDamage;
        public int powerLevel;

        // these are variables someone can set in the
        // config file
        public static double speedModifier = 3.0f;
        public static double rangeModifier = 2.0f;
        public static double FBSpeedModifier = 3.0f;

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
            baseFireInterval = -5.0f;
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
                Log("Your attack timer value is: " + FSMUtility.LocateFSM(grimmchild, "Control").FsmVariables.GetFsmFloat("Attack Timer").Value);

                return;
            }

            grimmchild = GameObject.FindGameObjectWithTag("Grimmchild");
            if (grimmchild == null)
            {
                return;
            }

            Log("Loaded Grimmchild");

            gcFSM = FSMUtility.LocateFSM(grimmchild, "Control");
            if (baseFireInterval < 0)
            {
                setDefaultGCValues();
            }            
            
            
            FsmState followState = gcFSM.GetState("Follow");
            FloatCompare[] followCompare = followState.GetActionsOfType<FloatCompare>();
            //followCompare[0].float2 = 1.0f;



            followCompare[1].float2 = .5f;

            FsmState anticAttack = gcFSM.GetState("Antic");
            RandomFloat[] anticRand = anticAttack.GetActionsOfType<RandomFloat>();

            anticRand[0].max = (float)(baseFireInterval / speedModifier);
            anticRand[0].min = (float)(baseFireInterval / speedModifier);

            FsmState noTarget = gcFSM.GetState("No Target");
            SetFloatValue[] noTargetWait = noTarget.GetActionsOfType<SetFloatValue>();
            noTargetWait[0].floatValue = (float)(baseFireInterval / (speedModifier) );

            gcFSM.FsmVariables.GetFsmFloat("Flameball Speed").Value = (float) (baseFBSpeed * FBSpeedModifier);
            grimmchild.FindGameObjectInChildren("Enemy Range").GetComponent<CircleCollider2D>().radius = (float) (baseRange * rangeModifier);
            


            /*
            FsmState waitState2 = gcFSM.GetState("Pause");
            SetFloatValue[] waitFloat2 = waitState1.GetActionsOfType<SetFloatValue>();
            waitFloat2[0].floatVariable = (float)(baseFireInterval / speedModifier);
            
            FsmState waitState3 = gcFSM.GetState("Spawn");
            SetFloatValue[] waitFloat3 = waitState1.GetActionsOfType<SetFloatValue>();
            waitFloat3[0].floatVariable = (float)(baseFireInterval / speedModifier);

            FsmState waitState4 = gcFSM.GetState("Lv 1");
            SetFloatValue[] waitFloat4 = waitState1.GetActionsOfType<SetFloatValue>();
            waitFloat4[0].floatVariable = (float)(baseFireInterval / speedModifier);
            */
            //GameObject gcRangeObj = grimmchild.FindGameObjectInChildren("Enemy Range");
            //CircleCollider2D gcRange = gcRangeObj.GetComponent<CircleCollider2D>();

            // 7.81???
            //Log("GrimmRange is " + gcRange.radius );

            // 0.3763812
            //Log("Attack Timer is " + gcFSM.FsmVariables.GetFsmFloat("Attack Timer"));

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

        public void setDefaultGCValues()
        {
            GameObject gcRangeObj = grimmchild.FindGameObjectInChildren("Enemy Range");
            CircleCollider2D gcRange = gcRangeObj.GetComponent<CircleCollider2D>();
            baseRange = gcRange.radius;
            //baseFireInterval = gcFSM.FsmVariables.GetFsmFloat("Attack Timer").Value;
            baseFireInterval = 1.5f;
            baseFBSpeed = gcFSM.FsmVariables.GetFsmFloat("Flameball Speed").Value;

            Log("base range: " + baseRange + " base fire interval: " + baseFireInterval + " base FB speed : " + baseFBSpeed);
        }
    }
}
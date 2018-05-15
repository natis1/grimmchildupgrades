using Modding;
using HutongGames.PlayMaker.Actions;
using static FsmUtil.FsmUtil;
using UnityEngine;
using System;
using HutongGames.PlayMaker;

namespace GrimmChildUpgrades
{
    class GrimmChild : MonoBehaviour
    {
        public GameObject grimmchild;
        public PlayMakerFSM gcFSM;
        public Wait IdleAction;
        public SetFloatValue fValue;
        public bool done;

        public static double speedModifier;

        public readonly string[] speedanimations = { "Idle 4", "Antic 4", "Shoot 4" };

        public readonly string[] waitstates = { "Follow" };
        public readonly float[] waitstateTimes = { 0.25f };


        // animations: Idle 4, Antic 4, Shoot 4
        // wait states: Follow (0.25)

        // fsm variables: Attack Timer 1.482669

        public void Start()
        {
            ModHooks.Instance.BeforeSceneLoadHook += reset;
        }

        private string reset(string sceneName)
        {
            done = false;
            Modding.Logger.Log("Reset Scene");
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

            Log("Found GrimmChild");

            gcFSM = FSMUtility.LocateFSM(grimmchild, "Control");

            Log("GrimmRange is " + gcFSM.FsmVariables.GetFsmInt("GrimmEnemyRange"));
            Log("Attack Timer is " + gcFSM.FsmVariables.GetFsmInt("Attack Timer"));

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

            ChangeTransition(gcFSM, "Shoot", "FINISHED", "Check For Target");
            ChangeTransition(gcFSM, "Shoot", "CANCEL", "Check For Target");

            done = true;
            Log("Done.");
        }

        public void Log(String str)
        {
            Modding.Logger.Log("[GMG] " + str);
        }

        public void OnDestroy()
        {
            ModHooks.Instance.BeforeSceneLoadHook -= reset;
        }
    }
}
using Modding;
using HutongGames.PlayMaker.Actions;
using static FsmUtil.FsmUtil;
using UnityEngine;
using System;
using HutongGames.PlayMaker;

namespace GMG
{
    class Gun : MonoBehaviour
    {
        public GameObject grimmchild;
        public PlayMakerFSM gcFSM;
        public Wait IdleAction;
        public SetFloatValue fValue;
        public bool done;
        
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
            if (!done)
            {
                if (grimmchild == null)
                {
                    grimmchild = GameObject.FindGameObjectWithTag("Grimmchild");
                    if (grimmchild != null) {
                        Modding.Logger.Log("[GMG] Found GrimmChild");

                        gcFSM = FSMUtility.LocateFSM(grimmchild, "Control");
                        if (gcFSM != null)
                        {
                            Modding.Logger.Log("Got control.");
                        }

                        // Remove follow idle
                        // IdleAction = (Wait) GetAction(gcFSM, "Follow", 18);
                        // IdleAction.time.Value = 0;

                        // Remove slow attack timer
                        //RandomFloat randFloat = (RandomFloat) GetAction(gcFSM, "Antic", 3);
                        //randFloat.min = 0f;
                        //randFloat.max = 0.0101f;

                        //fValue = (SetFloatValue) GetAction(gcFSM, "No Target", 0);
                        //fValue.floatValue = 0f;
                        //fValue = (SetFloatValue) GetAction(gcFSM, "Lv 1", 0);
                        //fValue.floatValue = 0f;
                        //gcFSM.FsmVariables.GetFsmFloat("Attack Timer").Value = 0f;

                        FsmFloat floaty = gcFSM.FsmVariables.GetFsmFloat("GrimmEnemyRange");
                        if (floaty != null)
                        {
                            floaty.Value *= 1000f;
                        } else {
                            FsmInt re = gcFSM.FsmVariables.GetFsmInt("GrimmEnemyRange");
                            if (re != null) { re.Value *= 1000; }

                        }
                        FsmUtil.FsmUtil.ChangeTransition(gcFSM, "Shoot", "FINISHED", "Check For Target");
                        FsmUtil.FsmUtil.ChangeTransition(gcFSM, "Shoot", "CANCEL", "Check For Target");

                        // Change animations
                        Modding.Logger.Log("[GMG] Not Done.");
                        //tk2dSpriteAnimator gAnim = grimmchild.GetComponent<tk2dSpriteAnimator>();
                        //gAnim.GetClipByName("Fly Anim").fps = 200;
                        //gAnim.GetClipByName("Shoot Anim").fps = 200;
                        //Modding.Logger.Log("[GMG] Still Not Done.");
                        //gAnim.GetClipByName("TurnFly Anim").fps = 200;
                        //gAnim.GetClipByName("FastAnim Anim").fps = 200;
                        //gAnim.GetClipByName("Antic Anim").fps = 200;

                        done = true;
                        Modding.Logger.Log("[GMG] Done.");
                    }
                }
            }
        }

        public void OnDestroy()
        {
            ModHooks.Instance.BeforeSceneLoadHook -= reset;
        }
    }
}
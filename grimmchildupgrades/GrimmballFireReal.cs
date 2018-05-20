using Modding;
using HutongGames.PlayMaker.Actions;
using static FsmUtil.FsmUtil;
using UnityEngine;
using System;
using ModCommon;
using HutongGames.PlayMaker;
using infinitegrimm;
using RandomizerMod.Extensions;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Collections.Generic;

namespace GrimmchildUpgrades
{
    class GrimmballFireReal : MonoBehaviour
    {

        public static GameObject customGrimmball;
        public static SpawnObjectFromGlobalPool deadShootSpawner;
        public static GameObject grimmchild;
        public static FsmState shootState;
        public static FireAtTarget oldAttack;
        public static float ballSize;
        public static bool ghostBalls;
        public static int damage;


        public void Start()
        {
            ghostBalls = true;
            Log("Added custom fireball component");
        }

        

        public void GrimmballUpdater()
        {
            try
            {
                customGrimmball = UnityEngine.Object.Instantiate(deadShootSpawner.gameObject.Value, grimmchild.transform.position, Quaternion.Euler(Vector3.up));



                customGrimmball.transform.position = grimmchild.transform.position;
                Log("Stage 1 set");
                PlayMakerFSM customGrimmballControl = FSMUtility.LocateFSM(customGrimmball, "Control");
                CircleCollider2D grimmballHitbox = customGrimmball.GetComponent<CircleCollider2D>();
                Vector3 grimmballRealSize = new Vector3(9f, 9f, 9f);

                
                //customGrimmball.d
                Rigidbody2D grimmPhysics = customGrimmball.GetComponent<Rigidbody2D>();
                //grimmPhysics.
                //grimmPhysics.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
                grimmPhysics.isKinematic = true;
                //grimmPhysics.
                //Destroy(grimmPhysics);

                customGrimmball.transform.localScale = grimmballRealSize;
                grimmballHitbox.radius = 1.1f;

                SetScale realSize = customGrimmballControl.GetState("Init").GetActionsOfType<SetScale>()[0];
                realSize.x = ballSize;
                realSize.y = ballSize;

                //GameObject grimmBallAttack = customGrimmball.FindGameObjectInChildren("Enemy Damager");
                //grimmBallAttack.transform.localScale = grimmballRealSize;

                customGrimmballControl.SetState("Init");

                

                GameObject hitbox = customGrimmball.FindGameObjectInChildren("Enemy Damager");
                PlayMakerFSM hitboxControl = FSMUtility.LocateFSM(hitbox, "Attack");
                FsmState inv = hitboxControl.GetState("Invincible?");

                hitboxControl.FsmVariables.FindFsmInt("Damage").Value = damage;
                
                Fsm innerFSM = (Fsm)customGrimmballControl.GetType().GetField("fsm", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(customGrimmballControl);

                FsmState[] grimmballCtrlStates = new FsmState[innerFSM.States.Length + 1];
                for (int i = 0; i < innerFSM.States.Length; i++)
                {
                    grimmballCtrlStates[i] = innerFSM.States[i];
                }
                FsmState newState = new FsmState(grimmballCtrlStates[2]);
                newState.Name = "HitDetect";
                newState.RemoveActionsOfType<SetIsKinematic2d>();
                newState.RemoveActionsOfType<RecycleSelf>();

                grimmballCtrlStates[grimmballCtrlStates.Length - 1] = newState;
                


                innerFSM.GetType().GetField("states", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(innerFSM, grimmballCtrlStates);
                

                if (ghostBalls)
                {
                    FsmState initState = customGrimmballControl.GetState("Init");
                    Collision2dEvent initCollide = initState.GetActionsOfType<Collision2dEvent>()[0];
                    initCollide.collideTag = "Enemy";
                }

                oldAttack.gameObject.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
                oldAttack.gameObject.GameObject = customGrimmball;

                
            }
            catch (Exception e)
            {
                Log("Unable to make the ball because of error: " + e);
            }
        }


        // Token: 0x0600006E RID: 110 RVA: 0x00005168 File Offset: 0x00003368
        public GameObject GetTarget()
        {
            List<GameObject> enemyList = grimmchild.FindGameObjectInChildren("Enemy Range").GetComponent<GrimmEnemyRange>().enemyList;
            
            GameObject result = null;
            float num = 99999f;
            if (enemyList.Count > 0)
            {
                for (int i = enemyList.Count - 1; i > -1; i--)
                {
                    if (enemyList[i] == null || !enemyList[i].activeSelf)
                    {
                        enemyList.RemoveAt(i);
                    }
                }
                foreach (GameObject gameObject in enemyList)
                {
                    // just pick enemy in range
                    if (ghostBalls && gameObject != null)
                    {
                        float sqrMagnitude = (base.transform.position - gameObject.transform.position).sqrMagnitude;
                        if (sqrMagnitude < num)
                        {
                            result = gameObject;
                            num = sqrMagnitude;
                        }

                        // Otherwise also check if you can raycast to them.
                    } else if (!Physics2D.Linecast(base.transform.position, gameObject.transform.position, 256))
                    {
                        float sqrMagnitude = (base.transform.position - gameObject.transform.position).sqrMagnitude;
                        if (sqrMagnitude < num)
                        {
                            result = gameObject;
                            num = sqrMagnitude;
                        }
                    }
                }
            }
            return result;
        }

        public void Log(String str)
        {
            
            Modding.Logger.Log("[GrimmchildUpgrades] " + str);
        }
    }
}

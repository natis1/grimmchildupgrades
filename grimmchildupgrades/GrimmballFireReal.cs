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

namespace GrimmchildUpgrades
{
    class GrimmballFireReal : MonoBehaviour
    {

        public GameObject customGrimmball;
        public static SpawnObjectFromGlobalPool shootSpawner;

        public void Start()
        {
            Log("Added custom fireball component");
        }

        public void GrimmballUpdater()
        {
            Log("Holy shit I'm actually being run");
            
            
            customGrimmball = GameObject.Instantiate(shootSpawner.gameObject.Value);

            customGrimmball.PrintSceneHierarchyTree("grimmball2.txt");
            PlayMakerFSM customGrimmballControl = FSMUtility.LocateFSM(customGrimmball, "Control");
            CircleCollider2D grimmballHitbox = customGrimmball.GetComponent<CircleCollider2D>();
            Vector3 grimmballRealSize = new Vector3(9f, 9f, 9f);

            customGrimmball.transform.localScale = grimmballRealSize;
            grimmballHitbox.radius = 0.1f;
            SetScale[] customGrimmballScales = customGrimmballControl.GetState("Init").GetActionsOfType<SetScale>();

            Log("And it worked without segfaulting??!");
            //GameObject grimmBallAttack = customGrimmball.FindGameObjectInChildren("Enemy Damager");
        }

        public void Log(String str)
        {
            Modding.Logger.Log("[GrimmchildUpgrades] " + str);
        }
    }
}

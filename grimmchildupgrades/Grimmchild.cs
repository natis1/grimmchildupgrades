using Modding;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using System;
using ModCommon;
using HutongGames.PlayMaker;
using infinitegrimm;
using RandomizerMod.Extensions;
using UnityEngine.SceneManagement;

namespace GrimmchildUpgrades
{
    class GrimmChild : MonoBehaviour
    {
        public GameObject grimmchild;

        public PlayMakerFSM gcFSM;
        public bool done;

        private float baseFireInterval;
        private float baseFBSpeed;
        private float baseRange;


        public int IGMaxDamage;
        public int powerLevel;

        // these are variables someone can set in the
        // config file
        public static double speedModifierCFG;
        public static double rangeModifierCFG;
        public static double FBSpeedModifierCFG;


        public static bool ghostBallCFG;

        public static int maxDamageCFG;
        public static int notchesCostCFG;

        public static float ballSizeCFG;
        public static float volumeMod;
        public static float filterRed;
        public static float filterGreen;
        public static float filterBlue;
        public static float filterAlpha;

        public static bool usingIG;


        // These are the variables used in the game, modified from the cfg ones by the vectors below.
        public double speedModifier;
        public double rangeModifier;
        public double FBSpeedModifier;
        public int notchesCost;

        public bool ghostBall;

        public int maxDamage;

        public float ballSize;


        //take the weighted average of the original and modified speed where weighting based on vector
        public readonly double[] speedModAvgVec = { 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
        public readonly double[] rangeModAvgVec = { 0, 0.2, 0.4, 0.6, 0.8, 1.0 };
        public readonly double[] fbSpeedModAvgVec = { 0, 0.2, 0.4, 0.6, 0.8, 1.0 };
        public readonly double[] maxDmgModAvgVec = { 0, 0.2, 0.4, 0.6, 0.8, 1.0 };
        public readonly double[] ballSizeModAvgVec = { 0.1, 0.3, 0.5, 0.7, 0.85, 1.0 };
        public readonly int[] notchesCostVec = { 3, 4, 5, 6, 6, 6 };
        public readonly bool[] useGhostBall = { false, false, false, true, true, true };
        

        public readonly string[] speedanimations = { "Idle 4", "Antic 4", "Shoot 4" };

        public void Start()
        {
            done = false;
            baseFireInterval = -5.0f;
            getIGDamage();
            calulateRealMods();


            if (PlayerData.instance.GetBoolInternal("killedNightmareGrimm") && PlayerData.instance.GetInt("charmCost_40") != notchesCost)
            {
                fixCharmBug();
            }            

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += reset;
            ModHooks.Instance.CharmUpdateHook += addCharms;
            ModHooks.Instance.GetPlayerIntHook += addedCharm40;
        }

        private void addCharms(PlayerData data, HeroController controller)
        {
            done = false;
            fixCharmBug();

        }

        private void fixCharmBug()
        {
            // Hey look who fixed a bug in TEAM CHERRY CODE
            // Fuck you team cherry I just fixed your goddamn shitty
            // notch calculating system by using GetInt instead
            // of reading the int manually. And also by using the function
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

        }

        private int addedCharm40(string intName)
        {
            if (intName == "charmCost_40")
            {
                return notchesCost;
            }
            return PlayerData.instance.GetIntInternal(intName);
        }

        private void reset(Scene from, Scene to)
        {
            if (PlayerData.instance.GetBoolInternal("killedNightmareGrimm"))
            {
                int oldPower = powerLevel;
                getIGDamage();
                fixCharmBug();
                if (powerLevel != oldPower)
                {
                    calulateRealMods();
                }
                done = false;
            }
        }

        public void Update()
        {
            if (done)
            {
                return;
            }

            // skip it if they aren't even at tier 4 grimmchild yet. or if they're especially dumb and
            // banished grimm
            if (!PlayerData.instance.GetBoolInternal("killedNightmareGrimm"))
            {
                done = true;
                return;
            }


            grimmchild = GameObject.FindGameObjectWithTag("Grimmchild");
            if (grimmchild == null)
            {
                return;
            }

            gcFSM = FSMUtility.LocateFSM(grimmchild, "Control");
            if (baseFireInterval < 0.0)
            {
                Log("Set default values for speeds");
                setDefaultGCValues();
            }
            
            FsmState followState = gcFSM.GetState("Follow");
            FloatCompare[] followCompare = followState.GetActionsOfType<FloatCompare>();
            //followCompare[0].float2 = 1.0f;
            
            followCompare[1].float2 = .5f;

            followState.RemoveActionsOfType<Wait>();

            FsmState anticAttack = gcFSM.GetState("Antic");
            RandomFloat[] anticRand = anticAttack.GetActionsOfType<RandomFloat>();

            anticRand[0].max = (float)(baseFireInterval / speedModifier);
            anticRand[0].min = (float)(baseFireInterval / speedModifier);

            FsmState noTarget = gcFSM.GetState("No Target");
            SetFloatValue[] noTargetWait = noTarget.GetActionsOfType<SetFloatValue>();
            noTargetWait[0].floatValue = (float)(baseFireInterval / (speedModifier * 2) );

            gcFSM.FsmVariables.GetFsmFloat("Flameball Speed").Value = (float) (baseFBSpeed * FBSpeedModifier);
            grimmchild.FindGameObjectInChildren("Enemy Range").GetComponent<CircleCollider2D>().radius = (float) (baseRange * rangeModifier);

            


            tk2dSprite grimmSprite = grimmchild.GetComponent<tk2dSprite>();
            Color grimmColor = grimmSprite.color;
            
            grimmColor.a = filterAlpha;
            grimmColor.b = filterBlue;
            grimmColor.g = filterGreen;
            grimmColor.r = filterRed;
            grimmSprite.color = grimmColor;



            FsmState shootYouFool = gcFSM.GetState("Shoot");
            GrimmballFireReal.grimmchild = grimmchild;
            GrimmballFireReal.shootState = shootYouFool;
            GrimmballFireReal.ballSize = ballSize;
            GrimmballFireReal.damage = maxDamage;
            GrimmballFireReal.ghostBalls = ghostBall;


            CallMethod[] currentMethods = shootYouFool.GetActionsOfType<CallMethod>();

            if (currentMethods.Length == 0)
            {

                SpawnObjectFromGlobalPool[] spawnObjs = shootYouFool.GetActionsOfType<SpawnObjectFromGlobalPool>();
                try
                {
                    GrimmballFireReal.deadShootSpawner = spawnObjs[0];
                    shootYouFool.RemoveActionsOfType<SpawnObjectFromGlobalPool>();
                }
                catch
                {
                    Log("Not removing shoot spawner, probs because it's length is: " + spawnObjs.Length);
                }


                CallMethod newDankShootMethod = new CallMethod { };
                try
                {
                    newDankShootMethod.behaviour = GameManager.instance.gameObject.GetComponent<GrimmballFireReal>();
                    newDankShootMethod.methodName = "GrimmballUpdater";
                    newDankShootMethod.parameters = new FsmVar[0];
                    newDankShootMethod.everyFrame = false;
                } catch (Exception e)
                {
                    Log("Unable to add method: error " + e);
                }
                Log("Made custom call method");
                shootYouFool.AddAction(newDankShootMethod);
                
                
                FireAtTarget currentFAT = shootYouFool.GetActionsOfType<FireAtTarget>()[0];
                shootYouFool.RemoveActionsOfType<FireAtTarget>();
                GrimmballFireReal.oldAttack = currentFAT;

                currentFAT.spread = 0.0f;
                

                GetChild currentGetChild = shootYouFool.GetActionsOfType<GetChild>()[0];
                shootYouFool.RemoveActionsOfType<GetChild>();
                currentGetChild.childName = "Grimmball(Clone)";

                SetFsmInt currentSetFSMInt = shootYouFool.GetActionsOfType<SetFsmInt>()[0];
                shootYouFool.RemoveActionsOfType<SetFsmInt>();

                // Reorder these things to make sense
                shootYouFool.AddAction(currentGetChild);
                shootYouFool.AddAction(currentSetFSMInt);
                shootYouFool.AddAction(currentFAT);

                GameObject gcRangeObj = grimmchild.FindGameObjectInChildren("Enemy Range");
                //GrimmEnemyRange rangeDelete = gcRangeObj.GetComponent<GrimmEnemyRange>();
                //Destroy(rangeDelete);
                gcRangeObj.AddComponent<GrimmballFireReal>();

                FsmState targetScan = gcFSM.GetState("Check For Target");
                CallMethodProper rangeDetect = targetScan.GetActionsOfType<CallMethodProper>()[0];
                rangeDetect.methodName.Value = "GetTarget";
                rangeDetect.behaviour.Value = "GrimmballFireReal";

                setVolumeLevels();


            }
            
            gcFSM.SetState("Init");

            //grimmchild.PrintSceneHierarchyTree("modgrimmchild.txt");

            done = true;
        }

        public void Log(String str)
        {
            Modding.Logger.Log("[GrimmchildUpgrades] " + str);
        }

        public void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= reset;
            ModHooks.Instance.CharmUpdateHook -= addCharms;
            ModHooks.Instance.GetPlayerIntHook -= addedCharm40;
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

        public void calulateRealMods()
        {
            Log("Current power level is " + powerLevel);
            int truePower = powerLevel - 1;
            speedModifier = (1.0 - speedModAvgVec[truePower]) + (speedModifierCFG * speedModAvgVec[truePower]);
            rangeModifier = (1.0 - rangeModAvgVec[truePower]) + (rangeModifierCFG * rangeModAvgVec[truePower]);
            FBSpeedModifier = (1.0 - fbSpeedModAvgVec[truePower]) + (FBSpeedModifierCFG * fbSpeedModAvgVec[truePower]);
            ballSize = (float) ( (1.0 - ballSizeModAvgVec[truePower]) + (ballSizeCFG * ballSizeModAvgVec[truePower]));
            maxDamage = (int)((11.0 - 11.0 * maxDmgModAvgVec[truePower]) + ((float)maxDamageCFG * maxDmgModAvgVec[truePower]));
            if (powerLevel < 6 && ghostBallCFG)
            {
                ghostBall = useGhostBall[truePower];
            } else
            {
                ghostBall = ghostBallCFG;
            }

            if (powerLevel < 6 && notchesCostCFG >= notchesCostVec[truePower])
            {
                notchesCost = notchesCostVec[truePower];
            } else
            {
                notchesCost = notchesCostCFG;
            }


        }

        public void setDefaultGCValues()
        {
            GameObject gcRangeObj = grimmchild.FindGameObjectInChildren("Enemy Range");
            CircleCollider2D gcRange = gcRangeObj.GetComponent<CircleCollider2D>();
            baseRange = gcRange.radius;
            baseFireInterval = 1.5f;
            baseFBSpeed = gcFSM.FsmVariables.GetFsmFloat("Flameball Speed").Value;
            
        }

        public void setVolumeLevels()
        {
            FsmState[] statesWithAudio = new FsmState[9];
            statesWithAudio[0] = gcFSM.GetState("Spawn");
            statesWithAudio[1] = gcFSM.GetState("Follow");
            statesWithAudio[2] = gcFSM.GetState("Tele Start");
            statesWithAudio[3] = gcFSM.GetState("Warp Out");
            statesWithAudio[4] = gcFSM.GetState("Tele");
            statesWithAudio[5] = gcFSM.GetState("Despawn");
            statesWithAudio[6] = gcFSM.GetState("Antic");
            statesWithAudio[7] = gcFSM.GetState("Shoot");
            statesWithAudio[8] = gcFSM.GetState("Audio");

            for (int j = 0; j < statesWithAudio.Length; j++)
            {
                AudioPlayerOneShotSingle[] audioType1 = statesWithAudio[j].GetActionsOfType<AudioPlayerOneShotSingle>();
                AudioPlayerOneShot[] audioType2 = statesWithAudio[j].GetActionsOfType<AudioPlayerOneShot>();
                AudioPlaySimple[] audioType3 = statesWithAudio[j].GetActionsOfType<AudioPlaySimple>();
                AudioPlay[] audioType4 = statesWithAudio[j].GetActionsOfType<AudioPlay>();
                
                for (int i = 0; i < audioType1.Length; i++)
                {
                    audioType1[i].volume.Value = volumeMod;
                }
                for (int i = 0; i < audioType2.Length; i++)
                {
                    audioType2[i].volume.Value = volumeMod;
                }
                for (int i = 0; i < audioType3.Length; i++)
                {
                    audioType3[i].volume.Value = volumeMod;
                }
                for (int i = 0; i < audioType4.Length; i++)
                {
                    audioType4[i].volume.Value = volumeMod;
                }


            }
        }
    }
}
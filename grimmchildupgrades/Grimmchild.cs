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
        public GameObject charms;
        public GameObject charmsPanel;

        public PlayMakerFSM gcFSM;
        public bool done;
        public bool done2;

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

        public int totalNotchesUsed;
        public bool overCharmed;


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
            done2 = false;
            baseFireInterval = -5.0f;
            getIGDamage();
            calulateRealMods();
            
            fixCharmBug();

            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += reset;
            ModHooks.Instance.GetPlayerIntHook += addedCharm40;
            ModHooks.Instance.GetPlayerBoolHook += isOCed;
            ModHooks.Instance.CharmUpdateHook += addCharms;
        }

        private bool isOCed(string originalSet)
        {
            if (originalSet == "overcharmed")
            {
                fixCharmBug2();
                return overCharmed;
            }
            return PlayerData.instance.GetBoolInternal(originalSet);
        }

        // Please ignore, useless crap code to be removed in next git commit
        private void FixCharmFSM()
        {
            Log("Trying to find FSM");
            charms = GameObject.Find("Over Indicator");
            charmsPanel = GameObject.Find("Charms");
            if (charms != null && charmsPanel != null)
            {
                PlayMakerFSM ctrl = FSMUtility.LocateFSM(charms, "Over Control");
                FsmState clickedButton = ctrl.GetState("Idle");


                /*
                BoolTest a = clickedButton.GetActionsOfType<BoolTest>()[0];
                SetBoolValue b = clickedButton.GetActionsOfType<SetBoolValue>()[0];
                ActivateAllChildren c = clickedButton.GetActionsOfType<ActivateAllChildren>()[0];

                clickedButton.RemoveActionsOfType<BoolTest>();
                clickedButton.RemoveActionsOfType<SetBoolValue>();
                clickedButton.RemoveActionsOfType<ActivateAllChildren>();

                CallMethod charmfix = new CallMethod();

                charmfix.behaviour = GameManager.instance.gameObject.GetComponent<CharmFix>();
                charmfix.methodName = "FixCharms";
                charmfix.parameters = new FsmVar[0];
                charmfix.everyFrame = false;
                clickedButton.AddAction(charmfix);
                clickedButton.AddAction(a);
                clickedButton.AddAction(b);
                clickedButton.AddAction(c);

                PlayMakerFSM panel = FSMUtility.LocateFSM(charmsPanel, "UI Charms");
                panel.GetState("Deactivate UI").AddAction(charmfix);
                panel.GetState("To Equipment").AddAction(charmfix);
                panel.GetState("Build Equipped").AddAction(charmfix);

                */
                // wtf
                PlayMakerFSM panel = FSMUtility.LocateFSM(charmsPanel, "UI Charms");
                FsmState activateUI = panel.GetState("Activate UI");
                activateUI.ClearTransitions();
                activateUI.AddTransition("FINISHED", "Init");
                FsmState unequip = panel.GetState("Unequippable");
                unequip.ClearTransitions();
                unequip.AddTransition("FINISHED", "Init");

                /*
                FsmState checkpt = panel.GetState("Check Points");
                IntCompare l = checkpt.GetActionsOfType<IntCompare>()[0];
                checkpt.RemoveActionsOfType<IntCompare>();
                checkpt.AddAction(charmfix);
                checkpt.AddAction(l);
                */

                /*
                FsmState overNotch = panel.GetState("Over Notches");

                GetPlayerDataInt[] overNotch1 = overNotch.GetActionsOfType<GetPlayerDataInt>();
                IntOperator overNotch200 = overNotch.GetActionsOfType<IntOperator>()[0];
                SetFsmInt overNotch3 = overNotch.GetActionsOfType<SetFsmInt>()[0];
                SetFsmBool overNotch4 = overNotch.GetActionsOfType<SetFsmBool>()[0];
                Translate overNotch5 = overNotch.GetActionsOfType<Translate>()[0];
                SendEventByName overNotch6 = overNotch.GetActionsOfType<SendEventByName>()[0];
                overNotch.RemoveActionsOfType<GetPlayerDataInt>();
                overNotch.RemoveActionsOfType<IntOperator>();
                overNotch.RemoveActionsOfType<SetFsmInt>();
                overNotch.RemoveActionsOfType<SetFsmBool>();
                overNotch.RemoveActionsOfType<Translate>();
                overNotch.RemoveActionsOfType<SendEventByName>();
                overNotch.AddAction(charmfix);
                overNotch.AddAction(overNotch1[0]);
                overNotch.AddAction(overNotch1[1]);
                overNotch.AddAction(overNotch200);
                overNotch.AddAction(overNotch3);
                overNotch.AddAction(overNotch4);
                overNotch.AddAction(overNotch5);
                overNotch.AddAction(overNotch6);
                panel.GetState("Return Points").AddAction(charmfix);

                FsmState openst = panel.GetState("Open Slot?");
                IntCompare m = openst.GetActionsOfType<IntCompare>()[0];
                GetPlayerDataInt[] m1 = openst.GetActionsOfType<GetPlayerDataInt>();
                openst.RemoveActionsOfType<IntCompare>();
                openst.AddAction(charmfix);
                openst.AddAction(m1[0]);
                openst.AddAction(m1[1]);
                openst.AddAction(m);

                FsmState stopen = panel.GetState("Slot Open?");
                GetPlayerDataInt[] n1 = stopen.GetActionsOfType<GetPlayerDataInt>();
                IntCompare n = stopen.GetActionsOfType<IntCompare>()[0];
                stopen.RemoveActionsOfType<IntCompare>();
                stopen.RemoveActionsOfType<GetPlayerDataInt>();
                stopen.AddAction(charmfix);
                stopen.AddAction(n1[0]);
                stopen.AddAction(n1[1]);
                stopen.AddAction(n);

                FsmState openst2 = panel.GetState("Open Slot? 2");
                IntCompare o = openst.GetActionsOfType<IntCompare>()[0];
                openst2.RemoveActionsOfType<IntCompare>();
                openst2.AddAction(charmfix);
                openst2.AddAction(o);

                FsmState endOC = panel.GetState("End Overcharm?");
                PlayerDataBoolTest p = endOC.GetActionsOfType<PlayerDataBoolTest>()[0];
                GetPlayerDataInt[] q = endOC.GetActionsOfType<GetPlayerDataInt>();
                IntCompare r = endOC.GetActionsOfType<IntCompare>()[0];
                endOC.RemoveActionsOfType<PlayerDataBoolTest>();
                endOC.RemoveActionsOfType<GetPlayerDataInt>();
                endOC.RemoveActionsOfType<IntCompare>();
                endOC.AddAction(charmfix);
                endOC.AddAction(p);
                endOC.AddAction(q[0]);
                endOC.AddAction(q[1]);
                endOC.AddAction(r);

                //FsmState remainOC = panel.GetState("Remain Overcharmed");


                FsmState idle = panel.GetState("Idle Collection");

                SetBoolValue g = idle.GetActionsOfType<SetBoolValue>()[0];
                SendEventByName h = idle.GetActionsOfType<SendEventByName>()[0];
                idle.RemoveActionsOfType<SetBoolValue>();
                idle.RemoveActionsOfType<SendEventByName>();
                idle.AddAction(charmfix);
                idle.AddAction(g);
                idle.AddAction(h);
                */

                done2 = true;
            }
        }
        

        private void addCharms(PlayerData data, HeroController controller)
        {
            done = false;
        }
        

        private void fixCharmBug()
        {
            fixCharmBug1();
            fixCharmBug2();
            Log("Fixed charms successfully");
        }

        private void fixCharmBug1()
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
            totalNotchesUsed = notchesUsed;
        }

        private void fixCharmBug2()
        {
            if (totalNotchesUsed > PlayerData.instance.GetInt("charmSlots"))
            {
                overCharmed = true;
            }
            else
            {
                overCharmed = false;
            }
        }

        private int addedCharm40(string intName)
        {
            if (PlayerData.instance.GetBoolInternal("killedNightmareGrimm"))
            {
                if (intName == "charmCost_40")
                {
                    return notchesCost;
                }
                if (intName == "charmSlotsFilled")
                {
                    //durr
                    fixCharmBug1();
                    return totalNotchesUsed;
                }
                
                
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
            ModHooks.Instance.GetPlayerIntHook -= addedCharm40;
            ModHooks.Instance.CharmUpdateHook -= addCharms;
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
            Log(" notches cost CFG is " + notchesCostCFG);
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
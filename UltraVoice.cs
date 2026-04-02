using BepInEx;
using HarmonyLib;
using PluginConfig.API;
using PluginConfig.API.Fields;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraVoice
{
    [BepInPlugin("com.mof33.ultravoice", "UltraVoice", "0.9.4")]
    [BepInDependency("com.eternalUnion.pluginConfigurator")]

    public class UltraVoice : BaseUnityPlugin
    {
        public static UltraVoice Instance;

        private PluginConfigurator config;

        public const string MEATGRINDER_SCENE = "7927c42db92e4164cae682a55e6b7725";
        public const string DOUBLEDOWN_SCENE = "5bcb2e0461e7fce408badfcb6778c271";
        public const string CLAIRDELUNE_SCENE = "36abcaae9708abc4d9e89e6ec73a2846";
        public const string CLAIRDESOLEIL_SCENE = "ac1675e648695a343bd064c6d0c56e57";
        public const string LIKEANTENNAS_SCENE = "6e981b1865c649749a610aafc471e198";
        public const string THROUGHTHEMIRROR_SCENE = "45addc6c3730dae418321e00af1116c5";

        public static float LastVoiceTime = -999f;
        public static float V2IntroTime = -999f;

        public static bool SwordsmachineIntroPlayed;
        public static bool V2DiedDuringFight;
        public static bool GreenArmVoicePlayed = false;
        public static bool GreenArmVoiceRestartPlayed = false;
        public static bool GreenArmDeathPlayed = false;
        public static bool FerrymanCoinTossed = false;
        public static bool FerrymanPhaseChangePlayed = false;
        public static bool GuttertankSpawnSkippedInMirror = false;

        public static BoolField CerberusVoiceEnabled;
        public static BoolField SwordsmachineVoiceEnabled;
        public static BoolField StreetcleanerVoiceEnabled;
        public static BoolField V2VoiceEnabled;
        public static BoolField MindflayerVoiceEnabled;
        public static BoolField VirtueVoiceEnabled;
        public static BoolField FerrymanVoiceEnabled;
        public static BoolField GuttermanVoiceEnabled;
        public static BoolField MannequinVoiceEnabled;
        public static BoolField GuttertankVoiceEnabled;
        public static BoolField EarthmoverVoiceEnabled;
        public static FloatField VoiceVolume;
        public static FloatField VoiceCooldown;
        public static ConfigPanel TogglesPanel;
        public static ConfigPanel SlidersPanel;

        public enum SwordsmachineVoiceActor
        {
            Mof,
            Noto
        }

        public static EnumField<SwordsmachineVoiceActor> SwordsmachineVoiceActorField;

        public static bool IsMascMindflayer(Mindflayer mf)
        {
            var smr = mf.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr == null) return false;

            return smr.sharedMesh == mf.maleMesh;
        }

        public static bool IsVirtue(Drone d)
        {
            if (d == null || d.eid == null)
                return false;

            if (d.GetComponent<DroneFlesh>() != null)
                return false;

            return d.eid.enemyType == EnemyType.Virtue;
        }

        public static bool IsAgonyOrTundra(SwordsMachine sm)
        {
            string n = sm.gameObject.name;
            return n.Contains("Agony") || n.Contains("Tundra");
        }

        public static bool IsAgony(SwordsMachine sm)
        {
            if (sm == null) return false;

            string n = sm.gameObject.name;
            return n.Contains("Agony");
        }

        public static bool IsTundra(SwordsMachine sm)
        {
            if (sm == null) return false;

            string n = sm.gameObject.name;
            return n.Contains("Tundra");
        }

        public static readonly Color AgonyColor = new Color(0.79f, 0.17f, 0.17f);
        public static readonly Color TundraColor = new Color(0.2f, 0.73f, 0.87f);
        public static Color? GetSwordsmachineColorOverride(SwordsMachine sm)
        {
            if (IsAgony(sm))
                return AgonyColor;
            if (IsTundra(sm))
                return TundraColor;
            return null;
        }

        public static bool IsAgonisOrRudraksha(Ferryman ferryman)
        {
            if (ferryman == null)
                return false;

            Transform t = ferryman.transform;

            while (t != null)
            {
                if (t.name.Contains("10S - Secret Arena"))
                    return true;

                t = t.parent;
            }

            return false;
        }

        public static bool IsAgonis(Ferryman ferryman)
        {
            if (!IsAgonisOrRudraksha(ferryman))
                return false;

            EnemyIdentifier eid = ferryman.GetComponent<EnemyIdentifier>();
            if (eid == null)
                return false;

            return eid.mirrorOnly;
        }

        public static bool IsRudraksha(Ferryman ferryman)
        {
            if (!IsAgonisOrRudraksha(ferryman))
                return false;

            EnemyIdentifier eid = ferryman.GetComponent<EnemyIdentifier>();
            if (eid == null)
                return false;

            return !eid.mirrorOnly;
        }

        public static Color GetEnemyTypeColor(EnemyType enemyType)
        {
            return enemyType switch
            {
                EnemyType.Swordsmachine => new Color(0.91f, 0.6f, 0.05f),
                EnemyType.Streetcleaner => new Color(0.82f, 0.30f, 0.09f),
                EnemyType.Cerberus => new Color(0.65f, 0.65f, 0.65f),
                EnemyType.Mindflayer => new Color(0.26f, 0.89f, 0.74f),
                EnemyType.Virtue => new Color(0.4f, 0.75f, 0.94f),
                EnemyType.Gutterman => new Color(0.91f, 0.73f, 0.51f),
                EnemyType.Ferryman => new Color(0f, 0.66f, 0.77f),
                EnemyType.Guttertank => new Color(0.8f, 0.07f, 0.07f),
                EnemyType.Mannequin => new Color(0.91f, 0.91f, 0.91f),

                _ => Color.white
            };
        }

        public static IEnumerator DelayedVox(System.Action playAction, System.Func<bool> ready, Component attached)
        {
            float waited = 0f;
            while (!ready() && waited < 2f)
            {
                if (attached == null) yield break;
                yield return new WaitForSeconds(0.1f);
                waited += 0.1f;
            }
            if (ready() && attached != null)
            {
                playAction();
            }
        }

        public static Dictionary<int, float> EnemyVoiceCooldown = new Dictionary<int, float>();
        public static Dictionary<Component, float> SpawnVoiceEndTimes = new Dictionary<Component, float>();
        public static Dictionary<Component, float> EnemySpawnTimes = new Dictionary<Component, float>();

        public static HashSet<V2> V2InstaEnrageFlag = new HashSet<V2>();
        public static HashSet<int> FerrymanFakeProximityTriggered = new HashSet<int>();

        public static AudioClip UseSwordsmachineClip(AudioClip mofClip, AudioClip notoClip)
        {
            return SwordsmachineVoiceActorField != null && SwordsmachineVoiceActorField.value == SwordsmachineVoiceActor.Noto
                ? notoClip
                : mofClip;
        }

        public static AudioClip[] UseSwordsmachineClips(AudioClip[] mofClips, AudioClip[] notoClips)
        {
            return SwordsmachineVoiceActorField != null && SwordsmachineVoiceActorField.value == SwordsmachineVoiceActor.Noto
                ? notoClips
                : mofClips;
        }

        public static AudioClip CerberusPreludeClip;
        public static AudioClip[] CerberusAwakenClips;
        public static AudioClip[] CerberusEnrageClips;
        public static AudioClip[] CerberusOrbClips;
        public static AudioClip[] CerberusStompClips;
        public static AudioClip[] CerberusTackleClips;
        public static AudioClip[] CerberusDeathClips;

        public static AudioClip SwordsmachineIntroClip;
        public static AudioClip SwordsmachineIntroClipSecond;
        public static AudioClip SwordsmachineAgonySpawnClip;
        public static AudioClip SwordsmachineTundraSpawnClip;
        public static AudioClip SwordsmachineBigPainClip;
        public static AudioClip SwordsmachineAgonyKnockdownClip;
        public static AudioClip SwordsmachineTundraKnockdownClip;
        public static AudioClip SwordsmachineLungeClip;
        public static AudioClip SwordsmachineComboClip;
        public static AudioClip SwordsmachineDeathClip;
        public static AudioClip[] SwordsmachineSpawnClips;
        public static AudioClip[] SwordsmachineEnrageClips;
        public static AudioClip[] SwordsmachineRangedClips;

        public static AudioClip SwordsmachineIntroClipNoto;
        public static AudioClip SwordsmachineIntroClipSecondNoto;
        public static AudioClip SwordsmachineAgonySpawnClipNoto;
        public static AudioClip SwordsmachineTundraSpawnClipNoto;
        public static AudioClip SwordsmachineBigPainClipNoto;
        public static AudioClip SwordsmachineAgonyKnockdownClipNoto;
        public static AudioClip SwordsmachineTundraKnockdownClipNoto;
        public static AudioClip SwordsmachineLungeClipNoto;
        public static AudioClip SwordsmachineComboClipNoto;
        public static AudioClip SwordsmachineDeathClipNoto;
        public static AudioClip[] SwordsmachineSpawnClipsNoto;
        public static AudioClip[] SwordsmachineEnrageClipsNoto;
        public static AudioClip[] SwordsmachineRangedClipsNoto;

        public static AudioClip V2IntroFirstClip;
        public static AudioClip V2IntroFirstRestartClip;
        public static AudioClip V2IntroSecondClip;
        public static AudioClip V2IntroSecondRestartClip;
        public static AudioClip V2FastDefeatClip;
        public static AudioClip V2DefeatClip;
        public static AudioClip V2DeathClip;
        public static AudioClip V2FlailingClip;
        public static AudioClip V2EnragePatienceClip;
        public static AudioClip V2EnragePunchedClip;
        public static AudioClip V2EscapingClip;
        public static AudioClip[] V2ChatterClips;
        public static AudioClip[] V2ChatterPissedClips;
        public static AudioClip[] V2PainClips;

        public static AudioClip[] MindflayerSpawnClips;
        public static AudioClip[] MindflayerChatterClips;
        public static AudioClip[] MindflayerMeleeClips;
        public static AudioClip[] MindflayerEnrageClips;
        public static AudioClip[] MindflayerSpawnClipsMasc;
        public static AudioClip[] MindflayerChatterClipsMasc;
        public static AudioClip[] MindflayerMeleeClipsMasc;
        public static AudioClip[] MindflayerEnrageClipsMasc;

        public static AudioClip FerrymanBossIntroClip;
        public static AudioClip FerrymanCoinSkipClip;
        public static AudioClip FerrymanCoinFightClip;
        public static AudioClip FerrymanPhaseChangeClip;
        public static AudioClip FerrymanApproachClip;
        public static AudioClip[] FerrymanChatterClips;
        public static AudioClip[] FerrymanParryClips;
        public static AudioClip[] FerrymanDeathClips;

        public static AudioClip[] VirtueSpawnClips;
        public static AudioClip[] VirtueAttackClips;
        public static AudioClip[] VirtueEnrageClips;
        public static AudioClip[] VirtueDeathClips;

        public static AudioClip[] StreetcleanerChatterClips;
        public static AudioClip[] StreetcleanerAttackClips;
        public static AudioClip[] StreetcleanerParryClips;

        public static AudioClip GuttermanBossIntroClip;
        public static AudioClip[] GuttermanSpawnClips;
        public static AudioClip[] GuttermanSpinUpClips;
        public static AudioClip[] GuttermanLostSightClips;
        public static AudioClip[] GuttermanShieldBreakClips;
        public static AudioClip[] GuttermanEnrageClips;
        public static AudioClip[] GuttermanDeathClips;
        public static AudioClip[] GuttermanParryClips;

        public static AudioClip[] MannequinChatterClips;
        public static AudioClip[] MannequinDeathClips;

        public static AudioClip[] GuttertankSpawnClips;
        public static AudioClip GuttertankAttackClip;
        public static AudioClip[] GuttertankPunchHitClips;
        public static AudioClip[] GuttertankFrustratedClips;
        public static AudioClip[] GuttertankDeathClips;
        public static AudioClip[] GuttertankTripPainClips;

        public static AudioClip EarthmoverIntroClip;

        public static readonly string[] CerberusAwakenSubs =
        {
            "Who dares awaken me",
            "You will go no further",
            "Your story ends here",
            "Your defiance shall be punished"
        };

        public static readonly string[] CerberusEnrageSubs =
        {
            "You will regret crossing me",
            "Your arrogance will cost you",
            "I will avenge my brother",
            "This shall not go unpunished"
        };

        public static readonly string[] CerberusOrbSubs = {
            "Perish",
            "Die"
        };

        public static readonly string[] CerberusStompSubs = {
            "Tremble",
            "Fall"
        };

        public static readonly string[] CerberusTackleSubs = {
            "Face me",
            "You cannot run"
        };

        public static readonly string[] SwordsmachineEnrageSubs =
        {
            "GRRR",
            "GRRR",
            "MOTHERFUCKER",
            "OHOHO, YOU ARE SO DEAD"
        };

        public static readonly string[] SwordsmachineEnrageSubs2 =
        {
            "I'LL KICK YOUR ASS!",
            "I'LL KILL YOU!",
            null,
            null
        };

        public static readonly string[] SwordsmachineSpawnSubs =
        {
            "COME ON, COME ON",
            "LET'S SEE SOME BLOOD",
            "WHO'S READY TO FIGHT",
            "I SMELL BLOOD",
            "I'LL CUT YOU ALL DOWN",
            "FRESH BLOOD",
            "MORE MEAT FOR THE SLAUGHTER",
        };

        public static readonly string[] SwordsmachineRangedSubs =
        {
            "CATCH THIS",
            "TAKE THIS",
        };

        public static readonly string[] V2ChatterSubs =
        {
            "Do keep up with me",
            "You can do better than that",
            "Is this your best effort",
            "Keep your eye open now",
            "Mind your footing",
        };

        public static readonly string[] V2ChatterPissedSubs =
        {
            "What's the matter? Does your arm hurt!?",
            "You really think you can kill me now!?",
            "I won't lose to you again!",
            "I'll make quick work of you this time!",
            "You don't stand a chance against me!"
        };

        public static readonly string[] MindflayerSpawnSubs =
        {
            "I require your blood.",
            "You appear to contain blood.",
            "You are suitable for blood extraction.",
            "Please donate your blood to me.",
            "Thank you for your imminent blood donation."
        };

        public static readonly string[] MindflayerChatterSubs =
        {
            "This process will be brief.",
            "Your cooperation is appreciated.",
            "Please do not resist.",
            "Continued resistance is unnecessary."
        };

        public static readonly string[] MindflayerMeleeSubs =
        {
            "Please maintain distance.",
            "Kindly step back.",
            "You are too close."
        };

        public static readonly string[] MindflayerEnrageSubs =
        {
            "You have made a very unwise choice.",
            "Your behavior is unacceptable.",
            "This is your final warning.",
            "I will correct you by force."
        };

        public static string[] FerrymanChatterSubs =
        {
            "Have at you!",
            "Come on!",
            "Come at me!",
            "Try me!"
        };

        public static readonly string[] VirtueSpawnSubs =
        {
            "It comes to this.",
            "I will do what must be done.",
            "I pray this ends swiftly.",
            "My orders have been given.",
            "Forgive me, machine."
        };

        public static readonly string[] VirtueAttackSubs =
        {
            "Why must it be this way?",
            "I take no joy in this.",
            "Do not make this harder for yourself.",
            "Must purpose demand such cruelty?",
            "I would spare you, if I could."
        };

        public static readonly string[] VirtueEnrageSubs =
        {
            "You force my hand!",
            "You leave me no choice!",
            "So be it…"
        };

        public static readonly string[] StreetcleanerChatterSubs =
        {
            "IMPURITY",
            "SANITIZE",
            "EXTERMINATE",
            "PURGE",
            "UNCLEAN"
        };

        public static readonly string[] StreetcleanerAttackSubs =
        {
            "CLEANSING",
            "PURIFYING",
            "TO ASH",
            "QUIT MOVING",
            "STOP RESISTING"
        };

        public static readonly string[] StreetcleanerParrySubs =
        {
            "DENIED",
            "DEFLECTED",
            "HA HA"
        };

        public static readonly string[] GuttermanSpawnSubs =
       {
            "THIS WILL NOT TAKE LONG.",
            "WHO MUST I KILL TODAY?",
            "IT IS TIME TO KILL.",
            "FOR THE MOTHERLAND!",
            "SAY YOUR PRAYERS."
        };

        public static readonly string[] GuttermanSpinUpSubs =
        {
            "THERE YOU ARE!",
            "TARGET SIGHTED!"
        };

        public static readonly string[] GuttermanLostSightSubs =
        {
            "COME OUT AND FACE ME!",
            "SHOW YOURSELF, COWARD!",
            "DO NOT WASTE MY TIME."
        };

        public static readonly string[] GuttermanShieldBreakSubs =
        {
            "SHIELD DOWN!",
            "SHIELD DESTROYED!",
            "SHIELD BROKEN!",
            "DEFENSES BREACHED!"
        };

        public static readonly string[] GuttermanEnrageSubs =
        {
            "THAT COST MONEY!",
            "YOU WILL PAY FOR THIS!",
            "I WILL TURN YOU TO SCRAP!",
            "OH, YOU MAKE ME SO MAD…"
        };

        public static readonly string[] GuttertankSpawnSubs = {
            "WEAPONS ONLINE.",
            "READY FOR COMBAT.",
            "ENGAGING PROTOCOLS.",
            "ALL SYSTEMS OPERATIONAL."
        };

        public static readonly string[] GuttertankPunchHitSubs = {
            "DIRECT HIT.",
            "HIT CONFIRMED.",
            "TRY THAT AGAIN."
        };

        public static readonly string[] GuttertankFrustratedSubs = {
            "DAMN IT!",
            "VERDAMMT!",
            "AH, MY HEAD…",
            "SCHEIẞE!"
        };

        void LoadAssets()
        {
            string bundlePath = Path.Combine(
                Path.GetDirectoryName(Info.Location),
                "ultravoiceassets"
            );

            var bundle = AssetBundle.LoadFromFile(bundlePath);

            if (bundle == null)
            {
                Logger.LogError("UltraVoice: Failed to load asset bundle.");
                return;
            }

            CerberusPreludeClip = LoadClip(bundle, "cerb_WakeUpSpecial");

            CerberusAwakenClips = new[]
            {
                LoadClip(bundle,"cerb_WakeUp1"),
                LoadClip(bundle,"cerb_WakeUp2"),
                LoadClip(bundle,"cerb_WakeUp3"),
                LoadClip(bundle,"cerb_WakeUp4")
            };

            CerberusEnrageClips = new[]
            {
                LoadClip(bundle,"cerb_Enrage1"),
                LoadClip(bundle,"cerb_Enrage2"),
                LoadClip(bundle,"cerb_Enrage3"),
                LoadClip(bundle,"cerb_Enrage4")
            };

            CerberusOrbClips = new[]
            {
                LoadClip(bundle,"cerb_Orb1"),
                LoadClip(bundle,"cerb_Orb2")
            };

            CerberusStompClips = new[]
            {
                LoadClip(bundle,"cerb_Stomp1"),
                LoadClip(bundle,"cerb_Stomp2")
            };

            CerberusTackleClips = new[]
            {
                LoadClip(bundle,"cerb_Tackle1"),
                LoadClip(bundle,"cerb_Tackle2")
            };

            CerberusDeathClips = new[]
            {
                LoadClip(bundle,"cerb_Death1"),
                LoadClip(bundle,"cerb_Death2"),
                LoadClip(bundle,"cerb_Death3")
            };

            SwordsmachineIntroClip = LoadClip(bundle, "sm_SpawnSpecial");
            SwordsmachineIntroClipSecond = LoadClip(bundle, "sm_SpawnSpecial2");
            SwordsmachineBigPainClip = LoadClip(bundle, "sm_BigPain");
            SwordsmachineLungeClip = LoadClip(bundle, "sm_Lunge");
            SwordsmachineComboClip = LoadClip(bundle, "sm_Combo");
            SwordsmachineDeathClip = LoadClip(bundle, "sm_Death");

            SwordsmachineAgonySpawnClip = LoadClip(bundle, "sm_SpawnSpecialAgony");
            SwordsmachineTundraSpawnClip = LoadClip(bundle, "sm_SpawnSpecialTundra");
            SwordsmachineAgonyKnockdownClip = LoadClip(bundle, "sm_DownedAgony");
            SwordsmachineTundraKnockdownClip = LoadClip(bundle, "sm_DownedTundra");

            SwordsmachineIntroClipNoto = LoadClip(bundle, "sm_SpawnSpecialNoto");
            SwordsmachineIntroClipSecondNoto = LoadClip(bundle, "sm_SpawnSpecial2Noto");
            SwordsmachineBigPainClipNoto = LoadClip(bundle, "sm_BigPainNoto");
            SwordsmachineLungeClipNoto = LoadClip(bundle, "sm_LungeNoto");
            SwordsmachineComboClipNoto = LoadClip(bundle, "sm_ComboNoto");
            SwordsmachineDeathClipNoto = LoadClip(bundle, "sm_DeathNoto");

            SwordsmachineAgonySpawnClipNoto = LoadClip(bundle, "sm_SpawnSpecialAgonyNoto");
            SwordsmachineTundraSpawnClipNoto = LoadClip(bundle, "sm_SpawnSpecialTundraNoto");
            SwordsmachineAgonyKnockdownClipNoto = LoadClip(bundle, "sm_DownedAgonyNoto");
            SwordsmachineTundraKnockdownClipNoto = LoadClip(bundle, "sm_DownedTundraNoto");

            SwordsmachineSpawnClips = new AudioClip[]
            {
                LoadClip(bundle,"sm_Spawn1"),
                LoadClip(bundle,"sm_Spawn2"),
                LoadClip(bundle,"sm_Spawn3"),
                LoadClip(bundle,"sm_Spawn4"),
                LoadClip(bundle,"sm_Spawn5"),
                LoadClip(bundle,"sm_Spawn6"),
                LoadClip(bundle,"sm_Spawn7"),
            };

            SwordsmachineEnrageClips = new AudioClip[]
            {
                LoadClip(bundle,"sm_Enrage1"),
                LoadClip(bundle,"sm_Enrage2"),
                LoadClip(bundle,"sm_Enrage3"),
                LoadClip(bundle,"sm_Enrage4")
            };

            SwordsmachineRangedClips = new AudioClip[]
            {
                LoadClip(bundle,"sm_Ranged1"),
                LoadClip(bundle,"sm_Ranged2"),
            };

            SwordsmachineSpawnClipsNoto = new AudioClip[]
            {
                LoadClip(bundle,"sm_Spawn1Noto"),
                LoadClip(bundle,"sm_Spawn2Noto"),
                LoadClip(bundle,"sm_Spawn3Noto"),
                LoadClip(bundle,"sm_Spawn4Noto"),
                LoadClip(bundle,"sm_Spawn5Noto"),
                LoadClip(bundle,"sm_Spawn6Noto"),
                LoadClip(bundle,"sm_Spawn7Noto")
            };

            SwordsmachineEnrageClipsNoto = new AudioClip[]
            {
                LoadClip(bundle,"sm_Enrage1Noto"),
                LoadClip(bundle,"sm_Enrage2Noto"),
                LoadClip(bundle,"sm_Enrage3Noto"),
                LoadClip(bundle,"sm_Enrage4Noto")
            };

            SwordsmachineRangedClipsNoto = new AudioClip[]
            {
                LoadClip(bundle,"sm_Ranged1Noto"),
                LoadClip(bundle,"sm_Ranged2Noto"),
            };

            V2IntroFirstClip = LoadClip(bundle, "v2_IntroFirst");
            V2IntroFirstRestartClip = LoadClip(bundle, "v2_RestartIntroFirst");
            V2IntroSecondClip = LoadClip(bundle, "v2_IntroSecond");
            V2IntroSecondRestartClip = LoadClip(bundle, "v2_RestartIntroSecond");
            V2DefeatClip = LoadClip(bundle, "v2_Defeat");
            V2FastDefeatClip = LoadClip(bundle, "v2_FastDefeat");
            V2EnragePatienceClip = LoadClip(bundle, "v2_EnragePatience");
            V2EnragePunchedClip = LoadClip(bundle, "v2_EnragePunched");
            V2EscapingClip = LoadClip(bundle, "v2_Escaping");
            V2FlailingClip = LoadClip(bundle, "v2_Flailing");
            V2DeathClip = LoadClip(bundle, "v2_Death");

            V2ChatterClips = new AudioClip[]
            {
                LoadClip(bundle,"v2_Chatter1"),
                LoadClip(bundle,"v2_Chatter2"),
                LoadClip(bundle,"v2_Chatter3"),
                LoadClip(bundle,"v2_Chatter4"),
                LoadClip(bundle,"v2_Chatter5")
            };

            V2ChatterPissedClips = new AudioClip[]
            {
                LoadClip(bundle,"v2_ChatterPissed1"),
                LoadClip(bundle,"v2_ChatterPissed2"),
                LoadClip(bundle,"v2_ChatterPissed3"),
                LoadClip(bundle,"v2_ChatterPissed4"),
                LoadClip(bundle,"v2_ChatterPissed5")
            };

            V2PainClips = new AudioClip[]
            {
                LoadClip(bundle,"v2_Pain1"),
                LoadClip(bundle,"v2_Pain2"),
                LoadClip(bundle,"v2_Pain3"),
                LoadClip(bundle,"v2_Pain4"),
                LoadClip(bundle,"v2_Pain5")
            };

            MindflayerSpawnClips = new AudioClip[]
            {
                LoadClip(bundle,"mf_Spawn1"),
                LoadClip(bundle,"mf_Spawn2"),
                LoadClip(bundle,"mf_Spawn3"),
                LoadClip(bundle,"mf_Spawn4"),
                LoadClip(bundle,"mf_Spawn5"),
            };

            MindflayerChatterClips = new AudioClip[]
            {
                LoadClip(bundle,"mf_Chatter1"),
                LoadClip(bundle,"mf_Chatter2"),
                LoadClip(bundle,"mf_Chatter3"),
                LoadClip(bundle,"mf_Chatter4"),
            };

            MindflayerMeleeClips = new AudioClip[]
            {
                LoadClip(bundle,"mf_Melee1"),
                LoadClip(bundle,"mf_Melee2"),
                LoadClip(bundle,"mf_Melee3"),
            };

            MindflayerEnrageClips = new AudioClip[]
            {
                LoadClip(bundle,"mf_Enrage1"),
                LoadClip(bundle,"mf_Enrage2"),
                LoadClip(bundle,"mf_Enrage3"),
                LoadClip(bundle,"mf_Enrage4"),
            };

            MindflayerSpawnClipsMasc = new AudioClip[]
            {
                LoadClip(bundle,"mf_Spawn1Masc"),
                LoadClip(bundle,"mf_Spawn2Masc"),
                LoadClip(bundle,"mf_Spawn3Masc"),
                LoadClip(bundle,"mf_Spawn4Masc"),
                LoadClip(bundle,"mf_Spawn5Masc"),
            };

            MindflayerChatterClipsMasc = new AudioClip[]
            {
                LoadClip(bundle,"mf_Chatter1Masc"),
                LoadClip(bundle,"mf_Chatter2Masc"),
                LoadClip(bundle,"mf_Chatter3Masc"),
                LoadClip(bundle,"mf_Chatter4Masc"),
            };

            MindflayerMeleeClipsMasc = new AudioClip[]
            {
                LoadClip(bundle,"mf_Melee1Masc"),
                LoadClip(bundle,"mf_Melee2Masc"),
                LoadClip(bundle,"mf_Melee3Masc"),
            };

            MindflayerEnrageClipsMasc = new AudioClip[]
            {
                LoadClip(bundle,"mf_Enrage1Masc"),
                LoadClip(bundle,"mf_Enrage2Masc"),
                LoadClip(bundle,"mf_Enrage3Masc"),
                LoadClip(bundle,"mf_Enrage4Masc"),
            };

            FerrymanBossIntroClip = LoadClip(bundle, "ferry_FightStarted");
            FerrymanCoinSkipClip = LoadClip(bundle, "ferry_CoinSkip");
            FerrymanCoinFightClip = LoadClip(bundle, "ferry_CoinFight");
            FerrymanPhaseChangeClip = LoadClip(bundle, "ferry_PhaseChange");
            FerrymanApproachClip = LoadClip(bundle, "ferry_Approach");

            FerrymanChatterClips = new AudioClip[]
            {
                LoadClip(bundle,"ferry_Chatter1"),
                LoadClip(bundle,"ferry_Chatter2"),
                LoadClip(bundle,"ferry_Chatter3"),
                LoadClip(bundle,"ferry_Chatter4"),
            };

            FerrymanParryClips = new AudioClip[]
            {
                LoadClip(bundle,"ferry_Parry1"),
                LoadClip(bundle,"ferry_Parry2"),
                LoadClip(bundle,"ferry_Parry3"),
                LoadClip(bundle,"ferry_Parry4"),
            };

            FerrymanDeathClips = new AudioClip[]
            {
                LoadClip(bundle,"ferry_Death1"),
                LoadClip(bundle,"ferry_Death2"),
                LoadClip(bundle,"ferry_Death3"),
            };

            VirtueSpawnClips = new AudioClip[]
            {
                LoadClip(bundle,"virtue_Spawn1"),
                LoadClip(bundle,"virtue_Spawn2"),
                LoadClip(bundle,"virtue_Spawn3"),
                LoadClip(bundle,"virtue_Spawn4"),
                LoadClip(bundle,"virtue_Spawn5")
            };

            VirtueAttackClips = new AudioClip[]
            {
                LoadClip(bundle,"virtue_Attack1"),
                LoadClip(bundle,"virtue_Attack2"),
                LoadClip(bundle,"virtue_Attack3"),
                LoadClip(bundle,"virtue_Attack4"),
                LoadClip(bundle,"virtue_Attack5")
            };

            VirtueEnrageClips = new AudioClip[]
            {
                LoadClip(bundle,"virtue_Enrage1"),
                LoadClip(bundle,"virtue_Enrage2"),
                LoadClip(bundle,"virtue_Enrage3"),
            };

            VirtueDeathClips = new AudioClip[]
            {
                LoadClip(bundle,"virtue_Death1"),
                LoadClip(bundle,"virtue_Death2"),
                LoadClip(bundle,"virtue_Death3"),
            };

            StreetcleanerChatterClips = new[]
            {
                LoadClip(bundle,"sc_Chatter1"),
                LoadClip(bundle,"sc_Chatter2"),
                LoadClip(bundle,"sc_Chatter3"),
                LoadClip(bundle,"sc_Chatter4"),
                LoadClip(bundle,"sc_Chatter5")
            };

            StreetcleanerAttackClips = new[]
            {
                LoadClip(bundle,"sc_Attack1"),
                LoadClip(bundle,"sc_Attack2"),
                LoadClip(bundle,"sc_Attack3"),
                LoadClip(bundle,"sc_Attack4"),
                LoadClip(bundle,"sc_Attack5")
            };

            StreetcleanerParryClips = new[]
            {
                LoadClip(bundle,"sc_Parry1"),
                LoadClip(bundle,"sc_Parry2"),
                LoadClip(bundle,"sc_Parry3"),
            };

            GuttermanSpawnClips = new AudioClip[]
            {
                LoadClip(bundle, "gm_Spawn1"),
                LoadClip(bundle, "gm_Spawn2"),
                LoadClip(bundle, "gm_Spawn3"),
                LoadClip(bundle, "gm_Spawn4"),
                LoadClip(bundle, "gm_Spawn5")
            };

            GuttermanSpinUpClips = new AudioClip[]
            {
                LoadClip(bundle, "gm_RevUp1"),
                LoadClip(bundle, "gm_RevUp2"),
                LoadClip(bundle, "gm_RevUp3"),
                LoadClip(bundle, "gm_RevUp4")
            };

            GuttermanShieldBreakClips = new AudioClip[]
            {
                LoadClip(bundle, "gm_GuardBreak1"),
                LoadClip(bundle, "gm_GuardBreak2"),
                LoadClip(bundle, "gm_GuardBreak3"),
            };

            GuttermanEnrageClips = new AudioClip[]
            {
                LoadClip(bundle, "gm_Enrage1"),
                LoadClip(bundle, "gm_Enrage2"),
                LoadClip(bundle, "gm_Enrage3"),
                LoadClip(bundle, "gm_Enrage4")
            };

            GuttermanDeathClips = new AudioClip[]
            {
                LoadClip(bundle, "gm_Death1"),
                LoadClip(bundle, "gm_Death2"),
                LoadClip(bundle, "gm_Death3"),
            };

            GuttermanParryClips = new AudioClip[]
            {
                LoadClip(bundle, "gm_Parry1"),
                LoadClip(bundle, "gm_Parry2"),
                LoadClip(bundle, "gm_Parry3"),
            };

            MannequinChatterClips = new AudioClip[]
            {
                LoadClip(bundle, "mq_Laugh1"),
                LoadClip(bundle, "mq_Laugh2"),
                LoadClip(bundle, "mq_Laugh3"),
                LoadClip(bundle, "mq_Laugh4"),
                LoadClip(bundle, "mq_Laugh5")
            };

            MannequinDeathClips = new AudioClip[]
            {
                LoadClip(bundle, "mq_Death1"),
                LoadClip(bundle, "mq_Death2"),
            };

            GuttertankSpawnClips = new AudioClip[]
            {
                LoadClip(bundle, "gt_Spawn1"),
                LoadClip(bundle, "gt_Spawn2"),
                LoadClip(bundle, "gt_Spawn3"),
                LoadClip(bundle, "gt_Spawn4")
            };

            GuttertankAttackClip = LoadClip(bundle, "gt_Attack");

            GuttertankPunchHitClips = new AudioClip[]
            {
                LoadClip(bundle, "gt_PunchHit1"),
                LoadClip(bundle, "gt_PunchHit2"),
                LoadClip(bundle, "gt_PunchHit3")
            };

            GuttertankFrustratedClips = new AudioClip[]
            {
                LoadClip(bundle, "gt_PunchTrip1"),
                LoadClip(bundle, "gt_PunchTrip2"),
                LoadClip(bundle, "gt_PunchTrip3"),
                LoadClip(bundle, "gt_PunchTrip4")
            };

            GuttertankDeathClips = new AudioClip[]
            {
                LoadClip(bundle, "gt_Death1"),
                LoadClip(bundle, "gt_Death2"),
                LoadClip(bundle, "gt_Death3")
            };

            GuttertankTripPainClips = new AudioClip[]
            {
                LoadClip(bundle, "gt_TripPain1"),
                LoadClip(bundle, "gt_TripPain2"),
                LoadClip(bundle, "gt_TripPain3"),
            };

            EarthmoverIntroClip = LoadClip(bundle, "em_Intro");
        }

        AudioClip LoadClip(AssetBundle bundle, string name)
        {
            var clip = bundle.LoadAsset<AudioClip>(name);

            if (clip == null)
                Logger.LogWarning($"UltraVoice missing clip: {name}");

            return clip;
        }

        void Awake()
        {
            Instance = this;

            config = PluginConfigurator.Create("UltraVoice", "com.mof33.ultravoice");

            config.SetIconWithURL($"https://storage.filebin.net/filebin/b20425983c28fd7feab09818ce6af10c2e766bd0e547ab3bd40a9709c9474171?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=GK352fd2505074fc9dde7fd2cb%2F20260331%2Fhel1-dc4%2Fs3%2Faws4_request&X-Amz-Date=20260331T220455Z&X-Amz-Expires=900&X-Amz-SignedHeaders=host&response-cache-control=max-age%3D900&response-content-disposition=inline%3B%20filename%3D%22icon.png%22&response-content-type=image%2Fpng&x-id=GetObject&X-Amz-Signature=965a1e646897a43091c19c99eaae70175fefe00de01754a84238fa971ba6780b");

            UltraVoice.TogglesPanel = new ConfigPanel(config.rootPanel, "Enemy Line Toggles", "toggles");

            UltraVoice.CerberusVoiceEnabled = new BoolField(
                UltraVoice.TogglesPanel,
                "Enable Cerberus Voice Lines",
                "cerbvoice",
                true
            );

            UltraVoice.SwordsmachineVoiceEnabled = new BoolField(
                UltraVoice.TogglesPanel,
                "Enable Swordsmachine Voice Lines",
                "smvoice",
                true
            );

            UltraVoice.V2VoiceEnabled = new BoolField(
                UltraVoice.TogglesPanel,
                "Enable V2 Voice Lines",
                "v2voice",
                true
            );

            UltraVoice.StreetcleanerVoiceEnabled = new BoolField(
                UltraVoice.TogglesPanel,
                "Enable Streetcleaner Voice Lines",
                "scvoice",
                true
            );

            UltraVoice.FerrymanVoiceEnabled = new BoolField(
                UltraVoice.TogglesPanel,
                "Enable Ferryman Voice Lines",
                "ferryvoice",
                true
            );

            UltraVoice.MindflayerVoiceEnabled = new BoolField(
                UltraVoice.TogglesPanel,
                "Enable Mindflayer Voice Lines",
                "mfvoice",
                true
            );

            UltraVoice.VirtueVoiceEnabled = new BoolField(
                UltraVoice.TogglesPanel,
                "Enable Virtue Voice Lines",
                "virtuevoice",
                true
            );

            UltraVoice.GuttermanVoiceEnabled = new BoolField(
                UltraVoice.TogglesPanel,
                "Enable Gutterman Voice Lines",
                "gmvoice",
                true
            );

            UltraVoice.MannequinVoiceEnabled = new BoolField(
                UltraVoice.TogglesPanel,
                "Enable Mannequin Voice Lines",
                "mqvoice",
                true
            );

            UltraVoice.GuttertankVoiceEnabled = new BoolField(
                UltraVoice.TogglesPanel,
                "Enable Guttertank Voice Lines",
                "gtvoice",
                true
            );

            UltraVoice.VoiceVolume = new FloatField(
                config.rootPanel,
                "Voice Line Volume",
                "volume",
                1f,
                0f,
                1f
            );

            UltraVoice.VoiceCooldown = new FloatField(
                config.rootPanel,
                "Voice Cooldown",
                "cooldown",
                0.3f,
                0f,
                1f
            );

            SwordsmachineVoiceActorField = new EnumField<SwordsmachineVoiceActor>(
                config.rootPanel,
                "Swordsmachine Voice Actor",
                "smvoiceactor",
                SwordsmachineVoiceActor.Mof
            );

            SwordsmachineVoiceActorField.SetEnumDisplayName(SwordsmachineVoiceActor.Mof, "Mof");
            SwordsmachineVoiceActorField.SetEnumDisplayName(SwordsmachineVoiceActor.Noto, "Noto");

            LoadAssets();

            new Harmony("com.mof33.ultravoice").PatchAll();
        }

        public static bool CheckCooldown(Component enemy, float cooldown)
        {
            int id = enemy.GetInstanceID();

            if (EnemyVoiceCooldown.TryGetValue(id, out float last))
            {
                if (Time.time - last < cooldown)
                    return false;
            }

            EnemyVoiceCooldown[id] = Time.time;
            return true;
        }

        public static bool CheckGlobalCooldown()
        {
            if (Time.time - LastVoiceTime < VoiceCooldown.value)
                return false;

            LastVoiceTime = Time.time;
            return true;
        }

        public static bool IsSpawnVoicePlaying(Component enemy)
        {
            if (!SpawnVoiceEndTimes.TryGetValue(enemy, out float endTime))
                return false;

            return Time.time < endTime;
        }

        public static bool TooSoonAfterSpawn(Component enemy, float delay)
        {
            if (!EnemySpawnTimes.TryGetValue(enemy, out float spawnTime))
                return false;

            return Time.time - spawnTime < delay;
        }

        public static AudioSource CreateVoiceSource(Component enemy, string name, AudioClip clip, string subtitle = null, bool shouldInterrupt = false, Color? subtitleColor = null, float pitch = 1f)
        {
            if (enemy == null || clip == null)
                return null;

            if (enemy.gameObject.name.Contains("Big Johninator"))
                return null;

            if (shouldInterrupt)
            {
                InterruptVoices(enemy);
            }
            else
            {
                if (!CheckGlobalCooldown())
                    return null;

                if (IsSpawnVoicePlaying(enemy))
                    return null;
            }

            if (enemy is SwordsMachine sm)
            {
                var overrideColor = GetSwordsmachineColorOverride(sm);
                if (overrideColor.HasValue)
                    subtitleColor = overrideColor.Value;
            }

            if (!subtitleColor.HasValue)
            {
                if (enemy.TryGetComponent<EnemyIdentifier>(out var eid))
                {
                    subtitleColor = GetEnemyTypeColor(eid.enemyType);
                }
            }

            LastVoiceTime = Time.time;

            GameObject obj = new GameObject($"UltraVoice_{name}");
            obj.transform.SetParent(enemy.transform);
            obj.transform.localPosition = Vector3.zero;

            var src = obj.AddComponent<AudioSource>();

            src.clip = clip;
            src.spatialBlend = 1f;
            src.volume = UltraVoice.VoiceVolume.value;
            src.volume *= 2;
            src.pitch = pitch;
            src.minDistance = 50f;
            src.maxDistance = 500f;
            src.dopplerLevel = 0;

            var mixer = MonoSingleton<AudioMixerController>.Instance;
            src.outputAudioMixerGroup = mixer.allGroup;

            src.Play();

            if (!string.IsNullOrEmpty(subtitle))
                ShowSubtitle(subtitle, src, subtitleColor);

            SpawnVoiceEndTimes[enemy] = Time.time + clip.length;

            UnityEngine.Object.Destroy(obj, clip.length + 1f);

            return src;
        }

        public static void ShowSubtitle(string text, AudioSource src, Color? color = null)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (color.HasValue)
                {
                    string coloredText = $"<color=#{ColorUtility.ToHtmlStringRGB(color.Value)}>{text}</color>";
                    MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(coloredText, src);
                }
                else
                {
                    MonoSingleton<SubtitleController>.Instance.DisplaySubtitle(text, src);
                }
            }
        }

        public static void InterruptVoices(Component enemy)
        {
            foreach (Transform child in enemy.transform)
            {
                if (child.name.StartsWith("UltraVoice"))
                    UnityEngine.Object.Destroy(child.gameObject);
            }
        }

        public static void PlayRandomVoice(Component enemy, AudioClip[] clips, string[] subs, bool shouldInterrupt = false, Color? subtitleColor = null, float pitch = 1f)
        {
            if (clips == null || clips.Length == 0)
                return;

            int i = UnityEngine.Random.Range(0, clips.Length);

            string sub = null;
            if (subs != null && i < subs.Length)
                sub = subs[i];

            CreateVoiceSource(
                enemy,
                enemy.GetType().Name,
                clips[i],
                sub,
                shouldInterrupt,
                subtitleColor,
                pitch
            );
        }
    }

    // RESTART PATCHES

    [HarmonyPatch(typeof(OptionsManager), "RestartMission")]
    class Restart
    {
        static void Postfix(OptionsManager __instance)
        {
            UltraVoice.SwordsmachineIntroPlayed = false;
            UltraVoice.V2IntroTime = -999f;
            UltraVoice.GreenArmVoicePlayed = false;
            UltraVoice.GreenArmVoiceRestartPlayed = false;
            UltraVoice.FerrymanPhaseChangePlayed = false;
        }
    }

    [HarmonyPatch(typeof(OptionsManager), "RestartCheckpoint")]
    class RestartCheckpoint
    {
        static void Postfix(OptionsManager __instance)
        {
            UltraVoice.SwordsmachineIntroPlayed = false;
            UltraVoice.V2IntroTime = -999f;
            UltraVoice.GreenArmVoiceRestartPlayed = false;
            UltraVoice.FerrymanPhaseChangePlayed = false;
        }
    }

    [HarmonyPatch(typeof(DeathSequence), "EndSequence")]
    class RestartDeath
    {
        static void Postfix(DeathSequence __instance)
        {
            UltraVoice.SwordsmachineIntroPlayed = false;
            UltraVoice.V2IntroTime = -999f;
            UltraVoice.GreenArmVoiceRestartPlayed = false;
            UltraVoice.FerrymanPhaseChangePlayed = false;
        }
    }

    // CERBERUS PATCHES

    [HarmonyPatch(typeof(StatueFake), "SlowStart")]
    class CerberusIntroVoice
    {
        static void Postfix(StatueFake __instance)
        {
            if (__instance.quickSpawn)
                return;

            if (!UltraVoice.CerberusVoiceEnabled.value)
                return;

            UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(StatueFake cerb)
        {
            yield return new WaitForSeconds(3.5f);

            AudioClip clip = UltraVoice.CerberusPreludeClip;

            if (clip == null)
                yield break;

            GameObject obj = new GameObject("UltraVoice_CerberusIntro");
            obj.transform.position = cerb.transform.position;

            var src = obj.AddComponent<AudioSource>();

            src.clip = clip;
            src.spatialBlend = 1f;
            src.volume = 1f;
            src.volume *= UltraVoice.VoiceVolume.value;
            src.minDistance = 50f;
            src.maxDistance = 500f;
            src.dopplerLevel = 0;

            var mixer = MonoSingleton<AudioMixerController>.Instance;
            src.outputAudioMixerGroup = mixer.allGroup;

            src.Play();

            if (src == null)
                yield break;

            UltraVoice.ShowSubtitle(
                "You tread forbidden ground, machine",
                src,
                new Color(0.65f, 0.65f, 0.65f)
            );

            yield return new WaitForSeconds(3.75f);

            UltraVoice.ShowSubtitle(
                "BEGONE",
                src,
                new Color(0.65f, 0.65f, 0.65f)
            );
        }
    }

    [HarmonyPatch(typeof(StatueFake), "Activate")]
    class CerberusWake
    {
        static void Postfix(StatueFake __instance)
        {
            if (!__instance.quickSpawn) return;

            if (!UltraVoice.CerberusVoiceEnabled.value)
                return;

            UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(StatueFake cerb)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1f));

            UltraVoice.PlayRandomVoice(cerb, UltraVoice.CerberusAwakenClips, UltraVoice.CerberusAwakenSubs, false, new Color(0.65f, 0.65f, 0.65f));
        }
    }

    [HarmonyPatch(typeof(StatueBoss), "Enrage")]
    class CerberusEnrage
    {
        static void Postfix(StatueBoss __instance)
        {
            if (!UltraVoice.CerberusVoiceEnabled.value)
                return;

            UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(StatueBoss cerb)
        {
            yield return new WaitForSeconds(0.1f);
            UltraVoice.PlayRandomVoice(cerb, UltraVoice.CerberusEnrageClips, UltraVoice.CerberusEnrageSubs);
            UltraVoice.SpawnVoiceEndTimes[cerb] = Time.time + 3;
        }
    }

    [HarmonyPatch(typeof(StatueBoss), "Throw")]
    class CerberusOrb
    {
        static void Postfix(StatueBoss __instance)
        {
            if (UltraVoice.IsSpawnVoicePlaying(__instance))
                return;

            if (!UltraVoice.CerberusVoiceEnabled.value)
                return;

            if (UnityEngine.Random.Range(0, 1) < 0.75)
                UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(StatueBoss cerb)
        {
            yield return new WaitForSeconds(0.2f);

            UltraVoice.PlayRandomVoice(
                cerb,
                UltraVoice.CerberusOrbClips,
                UltraVoice.CerberusOrbSubs
            );
        }
    }

    [HarmonyPatch(typeof(StatueBoss), "Stomp")]
    class CerberusStomp
    {
        static void Postfix(StatueBoss __instance)
        {
            if (UltraVoice.IsSpawnVoicePlaying(__instance))
                return;

            if (!UltraVoice.CerberusVoiceEnabled.value)
                return;

            if (UnityEngine.Random.Range(0, 1) < 0.75)
                UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(StatueBoss cerb)
        {
            yield return new WaitForSeconds(0.2f);

            UltraVoice.PlayRandomVoice(
                cerb,
                UltraVoice.CerberusStompClips,
                UltraVoice.CerberusStompSubs
            );
        }
    }

    [HarmonyPatch(typeof(StatueBoss), "Tackle")]
    class CerberusTackle
    {
        static void Postfix(StatueBoss __instance)
        {
            if (UltraVoice.IsSpawnVoicePlaying(__instance))
                return;

            if (!UltraVoice.CerberusVoiceEnabled.value)
                return;

            if (UnityEngine.Random.Range(0, 1) < 0.75)
                UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(StatueBoss cerb)
        {
            yield return new WaitForSeconds(0.2f);

            UltraVoice.PlayRandomVoice(
                cerb,
                UltraVoice.CerberusTackleClips,
                UltraVoice.CerberusTackleSubs
            );
        }
    }

    [HarmonyPatch(typeof(StatueBoss), "OnGoLimp")]
    class CerberusDeathVoice
    {
        static void Postfix(StatueBoss __instance)
        {
            if (UltraVoice.CerberusDeathClips == null || UltraVoice.CerberusDeathClips.Length == 0)
                return;

            if (!UltraVoice.CerberusVoiceEnabled.value)
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.CerberusDeathClips,
                null,
                true
            );
        }
    }

    // SWORDSMACHINE PATCHES

    [HarmonyPatch(typeof(SwordsMachine), "Start")]
    class SwordsmachineIntro
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (UltraVoice.IsAgonyOrTundra(__instance))
                return;

            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            UltraVoice.EnemySpawnTimes[__instance] = Time.time;

            if (__instance.bossVersion)
            {
                if (SceneManager.GetActiveScene().name != UltraVoice.DOUBLEDOWN_SCENE)
                    return;

                UltraVoice.Instance.StartCoroutine(Play(__instance));
                return;
            }

            if (!UltraVoice.CheckCooldown(__instance, 3f))
                return;

            if (!__instance.bossVersion)
            {
                if (SceneManager.GetActiveScene().name == UltraVoice.MEATGRINDER_SCENE)
                    return;

                UltraVoice.PlayRandomVoice(
                    __instance,
                    UltraVoice.UseSwordsmachineClips(UltraVoice.SwordsmachineSpawnClips, UltraVoice.SwordsmachineSpawnClipsNoto),
                    UltraVoice.SwordsmachineSpawnSubs
                );
            }
        }

        static IEnumerator Play(SwordsMachine sm)
        {
            yield return null;

            AudioClip clip;
            string subtitle;

            bool useNoto = UltraVoice.SwordsmachineVoiceActorField != null &&
                           UltraVoice.SwordsmachineVoiceActorField.value == UltraVoice.SwordsmachineVoiceActor.Noto;

            if (!UltraVoice.SwordsmachineIntroPlayed)
            {
                clip = UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineIntroClip, UltraVoice.SwordsmachineIntroClipNoto);
                subtitle = "YOU WANT A FIGHT? LET'S FIGHT";
                UltraVoice.SwordsmachineIntroPlayed = true;
            }
            else
            {
                clip = UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineIntroClipSecond, UltraVoice.SwordsmachineIntroClipSecondNoto);
                subtitle = "DID YOU THINK I FORGOT ABOUT YOU?";
            }

            var src = UltraVoice.CreateVoiceSource(sm, "SwordsmachineIntro", clip, subtitle);
            if (src != null)
            {
                UltraVoice.SpawnVoiceEndTimes[sm] = Time.time + clip.length;
            }
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "Start")]
    class SwordsmachineSpecialSpawn
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            if (!UltraVoice.IsAgonyOrTundra(__instance))
                return;

            UltraVoice.EnemySpawnTimes[__instance] = Time.time;

            if (UltraVoice.IsAgony(__instance))
            {
                UltraVoice.CreateVoiceSource(
                    __instance,
                    "AgonySpawn",
                    UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineAgonySpawnClip, UltraVoice.SwordsmachineAgonySpawnClipNoto),
                    "JUMP 'EM!",
                    subtitleColor: new Color(0.79f, 0.17f, 0.17f)
                );
            }
            else if (UltraVoice.IsTundra(__instance))
            {
                UltraVoice.CreateVoiceSource(
                    __instance,
                    "TundraSpawn",
                    UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineTundraSpawnClip, UltraVoice.SwordsmachineTundraSpawnClipNoto),
                    "THERE THEY ARE!",
                    subtitleColor: new Color(0.2f, 0.73f, 0.87f)
                );
            }
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "Enrage")]
    class SwordsmachineEnrage
    {
        static void Prefix(SwordsMachine __instance)
        {
            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            if (UltraVoice.IsAgonyOrTundra(__instance))
                return;

            if (__instance.enraged)
                return;

            UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(SwordsMachine sm)
        {
            yield return new WaitForSeconds(0.75f);

            bool useNoto = UltraVoice.SwordsmachineVoiceActorField != null &&
                           UltraVoice.SwordsmachineVoiceActorField.value == UltraVoice.SwordsmachineVoiceActor.Noto;

            AudioClip[] enrageClips = useNoto ? UltraVoice.SwordsmachineEnrageClipsNoto : UltraVoice.SwordsmachineEnrageClips;

            int i = UnityEngine.Random.Range(0, enrageClips.Length);

            var src = UltraVoice.CreateVoiceSource(
                sm,
                "SwordsmachineEnrage",
                enrageClips[i],
                UltraVoice.SwordsmachineEnrageSubs[i]
            );

            if (src == null)
                yield break;

            if (!string.IsNullOrEmpty(UltraVoice.SwordsmachineEnrageSubs2[i]))
            {
                yield return new WaitForSeconds(0.75f);

                UltraVoice.ShowSubtitle(
                    UltraVoice.SwordsmachineEnrageSubs2[i],
                    src,
                    new Color(0.91f, 0.6f, 0.05f)
                );
            }
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "Knockdown")]
    class SwordsmachineSpecialPain
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            UltraVoice.CreateVoiceSource(
                __instance,
                "SwordsmachineBigPain",
                UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineBigPainClip, UltraVoice.SwordsmachineBigPainClipNoto),
                null,
                true
            );

            UltraVoice.Instance.StartCoroutine(PlaySpecialPain(__instance));
        }

        static IEnumerator PlaySpecialPain(SwordsMachine sm)
        {
            yield return new WaitForSeconds(0.75f);

            if (UltraVoice.IsAgony(sm))
            {
                UltraVoice.CreateVoiceSource(
                    sm,
                    "AgonyKnockdown",
                    UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineAgonyKnockdownClip, UltraVoice.SwordsmachineAgonyKnockdownClipNoto),
                    "DAMMIT!"
                );
            }
            else if (UltraVoice.IsTundra(sm))
            {
                UltraVoice.CreateVoiceSource(
                    sm,
                    "TundraKnockdown",
                    UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineTundraKnockdownClip, UltraVoice.SwordsmachineTundraKnockdownClipNoto),
                    "COVER ME!"
                );
            }
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "EndFirstPhase")]
    class SwordsmachinePain2
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            if (UltraVoice.IsAgonyOrTundra(__instance))
                return;

            UltraVoice.InterruptVoices(__instance);

            UltraVoice.CreateVoiceSource(
                __instance,
                "SwordsmachineBigPain",
                UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineBigPainClip, UltraVoice.SwordsmachineBigPainClipNoto),
                null,
                true
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "TeleportAway")]
    class SwordsmachineTeleportAway
    {
        static void Postfix(SwordsMachine __instance)
        {
            UltraVoice.SwordsmachineIntroPlayed = true;

            UltraVoice.InterruptVoices(__instance);
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "ShootGun")]
    class SwordsmachineShotgun
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            if (UltraVoice.TooSoonAfterSpawn(__instance, 0.25f))
                return;

            if (!UltraVoice.CheckCooldown(__instance, 3f))
                return;

            if (UltraVoice.IsSpawnVoicePlaying(__instance))
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.UseSwordsmachineClips(UltraVoice.SwordsmachineRangedClips, UltraVoice.SwordsmachineRangedClipsNoto),
                UltraVoice.SwordsmachineRangedSubs
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "SwordThrow")]
    class SwordsmachineSwordThrow
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            if (UltraVoice.TooSoonAfterSpawn(__instance, 0.25f))
                return;

            if (!UltraVoice.CheckCooldown(__instance, 3f))
                return;

            if (UltraVoice.IsSpawnVoicePlaying(__instance))
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.UseSwordsmachineClips(UltraVoice.SwordsmachineRangedClips, UltraVoice.SwordsmachineRangedClipsNoto),
                UltraVoice.SwordsmachineRangedSubs
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "SwordSpiral")]
    class SwordsmachineSwordSpiral
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            if (UltraVoice.TooSoonAfterSpawn(__instance, 0.25f))
                return;

            if (!UltraVoice.CheckCooldown(__instance, 3f))
                return;

            if (UltraVoice.IsSpawnVoicePlaying(__instance))
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.UseSwordsmachineClips(UltraVoice.SwordsmachineRangedClips, UltraVoice.SwordsmachineRangedClipsNoto),
                UltraVoice.SwordsmachineRangedSubs
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "Combo")]
    class SwordsmachineCombo
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            if (UltraVoice.TooSoonAfterSpawn(__instance, 0.25f))
                return;

            if (!UltraVoice.CheckCooldown(__instance, 3f))
                return;

            if (UltraVoice.IsSpawnVoicePlaying(__instance))
                return;

            var src = UltraVoice.CreateVoiceSource(
                __instance,
                "SwordsmachineCombo",
                UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineComboClip, UltraVoice.SwordsmachineComboClipNoto),
                "DIE, DIE, DIE"
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "RunningSwing")]
    class SwordsmachineLunge
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            if (UltraVoice.TooSoonAfterSpawn(__instance, 0.25f))
                return;

            if (!UltraVoice.CheckCooldown(__instance, 3f))
                return;

            if (UltraVoice.IsSpawnVoicePlaying(__instance))
                return;

            var src = UltraVoice.CreateVoiceSource(
                __instance,
                "SwordsmachineLunge",
                UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineLungeClip, UltraVoice.SwordsmachineLungeClipNoto),
                "DIE"
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "OnGoLimp")]
    class SwordsmachineDeath
    {
        static void Postfix(SwordsMachine __instance)
        {
            if (!UltraVoice.SwordsmachineVoiceEnabled.value)
                return;

            UltraVoice.InterruptVoices(__instance);

            var src = UltraVoice.CreateVoiceSource(
                __instance,
                "SwordsmachineDeath",
                UltraVoice.UseSwordsmachineClip(UltraVoice.SwordsmachineDeathClip, UltraVoice.SwordsmachineDeathClipNoto),
                null,
                true
            );
        }
    }

    [HarmonyPatch(typeof(SwordsMachine), "CancelShotgunShot")]
    class SwordsmachineInterruptShotgun
    {
        static void Postfix(SwordsMachine __instance)
        {
            UltraVoice.InterruptVoices(__instance);
        }
    }

    // V2 PATCHES

    [HarmonyPatch(typeof(V2), "Start")]
    class V2IntroFirst
    {
        static void Postfix(V2 __instance)
        {
            if (!UltraVoice.V2VoiceEnabled.value)
                return;

            if (StatsManager.Instance.restarts > 0)
                return;

            if (__instance.secondEncounter)
                return;

            if (SceneManager.GetActiveScene().name != UltraVoice.CLAIRDELUNE_SCENE)
                return;

            if (!__instance.inIntro)
                return;

            UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(V2 v2)
        {
            yield return new WaitForSeconds(0.9f);

            var src = UltraVoice.CreateVoiceSource(
                v2,
                "V2Intro",
                UltraVoice.V2IntroFirstClip
            );

            if (src == null)
                yield break;

            UltraVoice.V2IntroTime = Time.time;

            UltraVoice.SpawnVoiceEndTimes[v2] = Time.time + UltraVoice.V2IntroFirstClip.length;

            UltraVoice.ShowSubtitle("A war-machine of my own design?", src);

            yield return new WaitForSeconds(2.75f);

            if (v2 == null || !v2.inIntro)
                yield break;

            UltraVoice.ShowSubtitle("How flattering...", src);

            yield return new WaitForSeconds(1.75f);

            if (v2 == null || !v2.inIntro)
                yield break;

            UltraVoice.ShowSubtitle("Well, may the best machine win", src);
        }
    }

    [HarmonyPatch(typeof(V2), "Start")]
    class V2IntroRetry
    {
        static void Postfix(V2 __instance)
        {
            if (!UltraVoice.V2VoiceEnabled.value)
                return;

            if (StatsManager.Instance.restarts <= 0)
                return;

            if (__instance.bossVersion)
                return;

            UltraVoice.EnemySpawnTimes[__instance] = Time.time;

            if (!__instance.secondEncounter)
                UltraVoice.Instance.StartCoroutine(PlayRestart(__instance));

            if (__instance.secondEncounter)
                if (!UltraVoice.GreenArmVoiceRestartPlayed)
                    UltraVoice.Instance.StartCoroutine(PlayRestartSecond(__instance));
        }

        static IEnumerator PlayRestart(V2 v2)
        {
            if (SceneManager.GetActiveScene().name != UltraVoice.CLAIRDELUNE_SCENE)
                yield break;

            yield return new WaitForSeconds(1f);

            var src = UltraVoice.CreateVoiceSource(
                v2,
                "V2IntroFirstRestart",
                UltraVoice.V2IntroFirstRestartClip,
                "Back so soon?"
            );

            if (src == null)
                yield break;

            UltraVoice.V2IntroTime = Time.time;
            UltraVoice.SpawnVoiceEndTimes[v2] = Time.time + UltraVoice.V2IntroFirstRestartClip.length;
        }

        static IEnumerator PlayRestartSecond(V2 v2)
        {
            if (SceneManager.GetActiveScene().name != UltraVoice.CLAIRDESOLEIL_SCENE)
                yield break;

            var src = UltraVoice.CreateVoiceSource(
                v2,
                "V2IntroSecondRestart",
                UltraVoice.V2IntroSecondRestartClip,
                "Just stay down!"
            );

            if (src == null)
                yield break;

            UltraVoice.V2IntroTime = Time.time;
            UltraVoice.SpawnVoiceEndTimes[v2] = Time.time + UltraVoice.V2IntroSecondRestartClip.length;
            UltraVoice.GreenArmVoiceRestartPlayed = true;
        }
    }

    [HarmonyPatch(typeof(V2), "Update")]
    class V2CombatChatter
    {
        static void Postfix(V2 __instance)
        {
            if (!UltraVoice.V2VoiceEnabled.value)
                return;

            if (__instance == null)
                return;

            if (!__instance.active || __instance.target == null)
                return;

            if (UltraVoice.IsSpawnVoicePlaying(__instance))
                return;

            if (UltraVoice.TooSoonAfterSpawn(__instance, 3f))
                return;

            if (!UltraVoice.CheckCooldown(__instance, 6f))
                return;

            if (__instance.enraged)
                return;

            if (!__instance.secondEncounter)
                UltraVoice.PlayRandomVoice(
                    __instance,
                    UltraVoice.V2ChatterClips,
                    UltraVoice.V2ChatterSubs
                );
            else
                UltraVoice.PlayRandomVoice(
                    __instance,
                    UltraVoice.V2ChatterPissedClips,
                    UltraVoice.V2ChatterPissedSubs
                );
        }
    }

    [HarmonyPatch(typeof(V2), "KnockedOut")]
    class V2DefeatFirst
    {
        static void Prefix(V2 __instance)
        {
            if (!UltraVoice.V2VoiceEnabled.value)
                return;

            float timeSinceIntro = Time.time - UltraVoice.V2IntroTime;

            if (!__instance.secondEncounter)
                if (__instance.inIntro || timeSinceIntro < 15f)
                {
                    UltraVoice.Instance.StartCoroutine(FastDefeat(__instance));
                }
                else
                {
                    UltraVoice.Instance.StartCoroutine(Defeat(__instance));
                }
            else
                if (!__instance.dead)
                    UltraVoice.Instance.StartCoroutine(Escape(__instance));
                else
                    UltraVoice.Instance.StartCoroutine(Realization(__instance));
        }

        static IEnumerator FastDefeat(V2 v2)
        {
            var src = UltraVoice.CreateVoiceSource(
                v2,
                "V2FastDefeat",
                UltraVoice.V2FastDefeatClip,
                null,
                true
            );

            if (src == null)
                yield break;

            UltraVoice.ShowSubtitle("Enough!", src);

            yield return new WaitForSeconds(1.5f);

            UltraVoice.ShowSubtitle("You've proven your point...", src);
        }

        static IEnumerator Defeat(V2 v2)
        {
            yield return new WaitForSeconds(1.25f);

            var src = UltraVoice.CreateVoiceSource(
                v2,
                "V2Defeat",
                UltraVoice.V2DefeatClip
            );

            if (src == null)
                yield break;

            UltraVoice.ShowSubtitle("This isn't over! Mark my words...", src);
        }

        static IEnumerator Escape(V2 v2)
        {
            yield return new WaitForSeconds(1.25f);

            UltraVoice.CreateVoiceSource(
                v2,
                "V2Escape",
                UltraVoice.V2EscapingClip,
                "I won't give you the PLEASURE of killing me!"
            );
        }

        static IEnumerator Realization(V2 v2)
        {
            yield return new WaitForSeconds(0.5f);

            UltraVoice.CreateVoiceSource(
                v2.transform,
                "V2Realization",
                UltraVoice.V2FlailingClip,
                "No... NO!"
            );
        }
    }

    [HarmonyPatch(typeof(V2), "OnDamage")]
    class V2Pain
    {
        static void Postfix(V2 __instance, ref DamageData data)
        {
            if (!UltraVoice.V2VoiceEnabled.value)
                return;

            if (data.damage < 3f)
                return;

            if (__instance.dead)
                return;

            if (!UltraVoice.CheckCooldown(__instance, 0.1f))
                return;

            if (UltraVoice.V2PainClips == null || UltraVoice.V2PainClips.Length == 0)
                return;

            int i = UnityEngine.Random.Range(0, UltraVoice.V2PainClips.Length);

            UltraVoice.CreateVoiceSource(
                __instance,
                "V2Pain",
                UltraVoice.V2PainClips[i]
            );
        }
    }

    [HarmonyPatch(typeof(V2), "InstaEnrage")]
    class V2InstaEnrage
    {
        static void Prefix(V2 __instance)
        {
            if (!UltraVoice.V2VoiceEnabled.value)
                return;

            UltraVoice.V2InstaEnrageFlag.Add(__instance);

            if (__instance.enraged)
                return;

            UltraVoice.CreateVoiceSource(
                __instance,
                "V2Enrage",
                UltraVoice.V2EnragePunchedClip,
                "EXCUSE ME?!"
            );
        }
    }

    [HarmonyPatch(typeof(V2), "Enrage", new System.Type[] { })]
    class V2PatienceEnrage
    {
        static void Postfix(V2 __instance)
        {
            if (!UltraVoice.V2VoiceEnabled.value)
                return;

            if (UltraVoice.V2InstaEnrageFlag.Contains(__instance))
            {
                UltraVoice.V2InstaEnrageFlag.Remove(__instance);
                return;
            }

            UltraVoice.CreateVoiceSource(
                __instance,
                "V2Enrage",
                UltraVoice.V2EnragePatienceClip,
                "COME HERE!"
            );
        }
    }

    [HarmonyPatch(typeof(Animator), "Play",
                new System.Type[] { typeof(string), typeof(int), typeof(float) })]
    class V2CutsceneVoice
    {
        static void Postfix(Animator __instance, string stateName, int layer, float normalizedTime)
        {
            if (!UltraVoice.V2VoiceEnabled.value)
                return;

            if (__instance == null)
                return;

            var go = __instance.gameObject;

            if (go.name != "v2_GreenArm")
                return;

            if (UltraVoice.GreenArmVoicePlayed)
                return;

            UltraVoice.Instance.StartCoroutine(Play(__instance));
        }
        static IEnumerator Play(Animator v2)
        {
            UltraVoice.GreenArmVoicePlayed = true;

            var src = UltraVoice.CreateVoiceSource(
                v2,
                "V2Cutscene",
                UltraVoice.V2IntroSecondClip
            );

            if (src == null)
                yield break;

            if (!src) yield break;
            UltraVoice.ShowSubtitle("Ah, so glad you could make it", src);

            yield return new WaitForSeconds(2.75f);

            if (!src) yield break;
            UltraVoice.ShowSubtitle("You took something from me, you know", src);

            yield return new WaitForSeconds(2.5f);

            if (!src) yield break;
            UltraVoice.ShowSubtitle("I think I'll be taking it back.", src);
        }
    }

    [HarmonyPatch(typeof(GameObject), "SetActive")]
    class V2Death
    {
        static void Postfix(GameObject __instance, bool value)
        {
            if (!UltraVoice.V2VoiceEnabled.value)
                return;

            if (__instance.name != "v2_GreenArm")
                return;

            Transform t = __instance.transform;
            bool found = false;

            while (t != null)
            {
                if (t.name.Contains("8 Stuff(Clone)(Clone)"))
                {
                    found = true;
                    break;
                }
                t = t.parent;
            }

            if (!found)
                return;

            UltraVoice.CreateVoiceSource(
                __instance.transform,
                "GreenArmDeath",
                UltraVoice.V2DeathClip,
                "NOOOOOOO"
            );
        }
    }

    // MINDFLAYER PATCHES

    [HarmonyPatch(typeof(Mindflayer), "Start")]
    class MindflayerSpawn
    {
        static void Postfix(Mindflayer __instance)
        {
            if (!UltraVoice.MindflayerVoiceEnabled.value)
                return;

            if (__instance.dying == true) return;

            UltraVoice.EnemySpawnTimes[__instance] = Time.time;

            var clips = UltraVoice.IsMascMindflayer(__instance)
                ? UltraVoice.MindflayerSpawnClipsMasc
                : UltraVoice.MindflayerSpawnClips;

            UltraVoice.PlayRandomVoice(
                __instance,
                clips,
                UltraVoice.MindflayerSpawnSubs
            );
        }
    }

    [HarmonyPatch(typeof(Mindflayer), "Update")]
    class MindflayerChatter
    {
        static void Postfix(Mindflayer __instance)
        {
            if (!UltraVoice.MindflayerVoiceEnabled.value)
                return;

            if (__instance.dying == true) return;

            if (!UltraVoice.CheckCooldown(__instance, 5f))
                return;

            if (UltraVoice.TooSoonAfterSpawn(__instance, 3f))
                return;

            if (__instance.dying)
                return;

            if (UnityEngine.Random.Range(0, 1) > 0.75f)
            {
                var clips = UltraVoice.IsMascMindflayer(__instance)
                    ? UltraVoice.MindflayerChatterClipsMasc
                    : UltraVoice.MindflayerChatterClips;

                UltraVoice.PlayRandomVoice(
                    __instance,
                    clips,
                    UltraVoice.MindflayerChatterSubs
                );
            }
        }
    }

    [HarmonyPatch(typeof(Mindflayer), "MeleeAttack")]
    class MindflayerMelee
    {
        static void Prefix(Mindflayer __instance)
        {
            if (!UltraVoice.MindflayerVoiceEnabled.value)
                return;

            if (__instance.dying == true) return;

            if (!UltraVoice.CheckCooldown(__instance, 2f))
                return;

            UltraVoice.Instance.StartCoroutine(DelayedMeleeVoice(__instance));
        }

        static IEnumerator DelayedMeleeVoice(Mindflayer mf)
        {
            yield return new WaitForSeconds(0.5f);

            if (mf == null) yield break;

            var clips = UltraVoice.IsMascMindflayer(mf)
                ? UltraVoice.MindflayerMeleeClipsMasc
                : UltraVoice.MindflayerMeleeClips;

            UltraVoice.PlayRandomVoice(
                mf,
                clips,
                UltraVoice.MindflayerMeleeSubs
            );
        }
    }

    [HarmonyPatch(typeof(Mindflayer), "Enrage")]
    class MindflayerEnrage
    {
        static void Postfix(Mindflayer __instance)
        {
            if (!UltraVoice.MindflayerVoiceEnabled.value)
                return;

            if (__instance.dying == true) return;

            var clips = UltraVoice.IsMascMindflayer(__instance)
                ? UltraVoice.MindflayerEnrageClipsMasc
                : UltraVoice.MindflayerEnrageClips;

            UltraVoice.PlayRandomVoice(
                __instance,
                clips,
                UltraVoice.MindflayerEnrageSubs
            );
        }
    }

    [HarmonyPatch(typeof(Mindflayer), "Death")]
    class MindflayerDeath
    {
        static void Postfix(Mindflayer __instance)
        {
            UltraVoice.InterruptVoices(__instance);
        }
    }

    // FERRYMAN PATCHES

    [HarmonyPatch(typeof(Ferryman), "Start")]
    class FerrymanSpawnVoice
    {
        static void Postfix(Ferryman __instance)
        {
            if (!UltraVoice.FerrymanVoiceEnabled.value)
                return;

            if (__instance == null)
                return;

            if (StatsManager.Instance.restarts > 0)
                return;

            UltraVoice.EnemySpawnTimes[__instance] = Time.time;

            if (UltraVoice.IsAgonisOrRudraksha(__instance))
                return;

            if (!__instance.bossVersion)
                return;

            if (UltraVoice.FerrymanCoinTossed)
            {
                UltraVoice.Instance.StartCoroutine(PlayCoin(__instance));
                UltraVoice.EnemySpawnTimes[__instance] = Time.time;
                UltraVoice.SpawnVoiceEndTimes[__instance] = Time.time + UltraVoice.FerrymanCoinFightClip.length;
            }
            else
            {
                UltraVoice.Instance.StartCoroutine(PlayNoCoin(__instance));
                UltraVoice.EnemySpawnTimes[__instance] = Time.time;
                UltraVoice.SpawnVoiceEndTimes[__instance] = Time.time + UltraVoice.FerrymanBossIntroClip.length;
            }

            static IEnumerator PlayNoCoin(Ferryman ferry)
            {
                var src = UltraVoice.CreateVoiceSource(
                    ferry,
                    "FerrymanIntro",
                    UltraVoice.FerrymanBossIntroClip,
                    null,
                    true
                );

                if (src == null)
                    yield break;

                if (!src) yield break;
                UltraVoice.ShowSubtitle("Gabriel warned me of you and your likes", src, new Color(0f, 0.66f, 0.77f));

                yield return new WaitForSeconds(3f);

                if (!src) yield break;
                UltraVoice.ShowSubtitle("I will not make the same mistakes as him!", src, new Color(0f, 0.66f, 0.77f));
            }

            static IEnumerator PlayCoin(Ferryman ferry)
            {
                var src = UltraVoice.CreateVoiceSource(
                    ferry,
                    "FerrymanIntro",
                    UltraVoice.FerrymanCoinFightClip
                );

                UltraVoice.SpawnVoiceEndTimes[ferry] = Time.time + UltraVoice.FerrymanCoinFightClip.length;

                if (src == null)
                    yield break;

                if (!src) yield break;
                UltraVoice.ShowSubtitle("You SCOUNDREL", src, new Color(0f, 0.66f, 0.77f));

                yield return new WaitForSeconds(1.5f);

                if (!src) yield break;
                UltraVoice.ShowSubtitle("I should have never trusted a machine like you!", src, new Color(0f, 0.66f, 0.77f));
            }
        }
    }

    [HarmonyPatch(typeof(FerrymanFake), "CoinCatch")]
    class FerrymanCoinDetected
    {
        static void Postfix(FerrymanFake __instance)
        {
            if (!UltraVoice.FerrymanVoiceEnabled.value)
                return;

            UltraVoice.FerrymanCoinTossed = true;

            UltraVoice.Instance.StartCoroutine(Play(__instance));

            static IEnumerator Play(FerrymanFake ferry)
            {
                yield return new WaitForSeconds(0.25f);

                var src = UltraVoice.CreateVoiceSource(
                    ferry,
                    "FerrymanSkip",
                    UltraVoice.FerrymanCoinSkipClip,
                    "Hm?",
                    true
                );

                if (src == null)
                    yield break;

                yield return new WaitForSeconds(1.75f);

                if (!src) yield break;
                UltraVoice.ShowSubtitle("This shall do", src, new Color(0f, 0.66f, 0.77f));

                yield return new WaitForSeconds(1.5f);

                if (!src) yield break;
                UltraVoice.ShowSubtitle("You may pass", src, new Color(0f, 0.66f, 0.77f));
            }
        }
    }

    [HarmonyPatch(typeof(Ferryman), "Update")]
    class FerrymanChatter
    {
        static void Postfix(Ferryman __instance)
        {
            if (!UltraVoice.FerrymanVoiceEnabled.value)
                return;

            if (__instance == null)
                return;

            if (__instance.currentWindup != null)
                return;

            if (!UltraVoice.CheckCooldown(__instance, 6f))
                return;

            if (UltraVoice.IsAgonisOrRudraksha(__instance))
                return;

            if (UltraVoice.TooSoonAfterSpawn(__instance, 3f))
                return;

            if (UnityEngine.Random.Range(0, 1) > 0.75f)
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.FerrymanChatterClips,
                UltraVoice.FerrymanChatterSubs
            );
        }
    }

    [HarmonyPatch(typeof(Ferryman), "PhaseChange")]
    class FerrymanPhaseChange
    {
        static void Postfix(Ferryman __instance)
        {
            if (!UltraVoice.FerrymanVoiceEnabled.value)
                return;

            if (__instance == null)
                return;

            if (UltraVoice.IsAgonisOrRudraksha(__instance))
                return;

            if (UltraVoice.TooSoonAfterSpawn(__instance, 0.25f))
                return;

            if (!UltraVoice.FerrymanPhaseChangePlayed)
                UltraVoice.Instance.StartCoroutine(PlayPhaseChange(__instance));

            static IEnumerator PlayPhaseChange(Ferryman ferry)
            {
                new WaitForSeconds(0.1f);

                var src = UltraVoice.CreateVoiceSource(
                    ferry,
                    "FerrymanPhase",
                    UltraVoice.FerrymanPhaseChangeClip,
                    null,
                    true
                );

                UltraVoice.FerrymanPhaseChangePlayed = true;

                UltraVoice.SpawnVoiceEndTimes[ferry] = Time.time + UltraVoice.FerrymanBossIntroClip.length;

                yield return new WaitForSeconds(1f);

                UltraVoice.ShowSubtitle("I am not finished with you!", src, new Color(0f, 0.66f, 0.77f));
            }
        }
    }

    [HarmonyPatch(typeof(Ferryman), "OnGoLimp")]
    class FerrymanDeath
    {
        static void Postfix(Ferryman __instance)
        {
            if (!UltraVoice.FerrymanVoiceEnabled.value)
                return;

            if (__instance == null)
                return;

            if (UltraVoice.IsAgonisOrRudraksha(__instance))
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.FerrymanDeathClips,
                null,
                true
            );
        }
    }

    [HarmonyPatch(typeof(FerrymanFake), "Update")]
    class FerrymanFakeProximity
    {
        static void Postfix(FerrymanFake __instance)
        {
            if (!UltraVoice.FerrymanVoiceEnabled.value)
                return;

            if (__instance == null)
                return;

            if (StatsManager.Instance.restarts > 0)
                return;

            var player = MonoSingleton<NewMovement>.Instance;

            if (player == null)
                return;

            int id = __instance.GetInstanceID();

            if (UltraVoice.FerrymanFakeProximityTriggered.Contains(id))
                return;

            float dist = Vector3.Distance(
                __instance.transform.position,
                player.transform.position
            );

            if (dist > 60f)
                return;

            UltraVoice.FerrymanFakeProximityTriggered.Add(id);

            UltraVoice.CreateVoiceSource(
                __instance,
                "FerrymanApproach",
                UltraVoice.FerrymanApproachClip,
                "Who goes there?"
            );
        }
    }

    // VIRTUE PATCHES

    [HarmonyPatch(typeof(Drone), "Start")]
    class VirtueSpawn
    {
        static void Postfix(Drone __instance)
        {
            if (!UltraVoice.VirtueVoiceEnabled.value)
                return;

            if (!UltraVoice.IsVirtue(__instance))
                return;

            UltraVoice.EnemySpawnTimes[__instance] = Time.time;

            if (!UltraVoice.CheckCooldown(__instance, 0f))
                return;

            UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(Drone drone)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.3f, 0.5f));

            UltraVoice.PlayRandomVoice(
                drone,
                UltraVoice.VirtueSpawnClips,
                UltraVoice.VirtueSpawnSubs
            );
        }
    }

    [HarmonyPatch(typeof(VirtueInsignia), "Activating")]
    class VirtueAttack
    {
        static void Postfix(VirtueInsignia __instance)
        {
            if (!UltraVoice.VirtueVoiceEnabled.value)
                return;

            Drone drone = null;

            if (__instance.parentEnemy)
                __instance.parentEnemy.TryGetComponent(out drone);
            else if (__instance.parentDrone)
                drone = __instance.parentDrone;

            if (drone == null || !UltraVoice.IsVirtue(drone))
                return;

            if (drone.isEnraged == true)
                return;

            if (!UltraVoice.CheckCooldown(drone, 2f))
                return;

            if (UltraVoice.TooSoonAfterSpawn(drone, 1.5f))
                return;

            if (UnityEngine.Random.Range(0, 1) > 0.25)
                UltraVoice.PlayRandomVoice(
                    drone,
                    UltraVoice.VirtueAttackClips,
                    UltraVoice.VirtueAttackSubs
                );
        }
    }

    [HarmonyPatch(typeof(Drone), "Enrage")]
    class VirtueEnrage
    {
        static void Postfix(Drone __instance)
        {
            if (!UltraVoice.VirtueVoiceEnabled.value)
                return;

            if (!UltraVoice.IsVirtue(__instance))
                return;

            UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(Drone drone)
        {
            yield return new WaitForSeconds(0.1f);

            UltraVoice.PlayRandomVoice(
                drone,
                UltraVoice.VirtueEnrageClips,
                UltraVoice.VirtueEnrageSubs
            );
        }
    }

    [HarmonyPatch(typeof(Drone), "Death")]
    class VirtueDeath
    {
        static void Postfix(Drone __instance)
        {
            if (!UltraVoice.VirtueVoiceEnabled.value)
                return;

            if (!UltraVoice.IsVirtue(__instance))
                return;

            UltraVoice.Instance.StartCoroutine(Play(__instance));
        }

        static IEnumerator Play(Drone drone)
        {
            yield return new WaitForSeconds(0f);

            UltraVoice.PlayRandomVoice(
                drone,
                UltraVoice.VirtueDeathClips,
                null,
                true
            );
        }
    }

    // STREETCLEANER PATCHES

    [HarmonyPatch(typeof(Streetcleaner), "Start")]
    class StreetcleanerSpawnTrack
    {
        static void Postfix(Streetcleaner __instance)
        {
            if (!UltraVoice.StreetcleanerVoiceEnabled.value)
                return;

            UltraVoice.EnemySpawnTimes[__instance] = Time.time;
        }
    }

    [HarmonyPatch(typeof(Streetcleaner), "Update")]
    class StreetcleanerChatter
    {
        static void Postfix(Streetcleaner __instance)
        {
            if (!UltraVoice.StreetcleanerVoiceEnabled.value)
                return;

            if (__instance.dead)
                return;

            if (!UltraVoice.CheckCooldown(__instance, 4f))
                return;

            if (UnityEngine.Random.value > 0.75)
                UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.StreetcleanerChatterClips,
                UltraVoice.StreetcleanerChatterSubs
            );
        }
    }

    [HarmonyPatch(typeof(Streetcleaner), "StartFire")]
    class StreetcleanerFlame
    {
        static void Postfix(Streetcleaner __instance)
        {
            if (!UltraVoice.StreetcleanerVoiceEnabled.value)
                return;

            if (UnityEngine.Random.value > 0.75)
                UltraVoice.PlayRandomVoice(
                     __instance,
                     UltraVoice.StreetcleanerAttackClips,
                     UltraVoice.StreetcleanerAttackSubs
                 );
        }
    }

    [HarmonyPatch(typeof(Streetcleaner), "DeflectShot")]
    class StreetcleanerParry
    {
        static void Postfix(Streetcleaner __instance)
        {
            if (!UltraVoice.StreetcleanerVoiceEnabled.value)
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.StreetcleanerParryClips,
                UltraVoice.StreetcleanerParrySubs
            );
        }
    }

    [HarmonyPatch(typeof(Streetcleaner), "OnGoLimp")]
    class StreetcleanerDeathInterrupt
    {
        static void Postfix(Streetcleaner __instance)
        {
            UltraVoice.InterruptVoices(__instance);
        }
    }

    // GUTTERMAN PATCHES

    [HarmonyPatch(typeof(Gutterman), "Start")]
    class GuttermanSpawn
    {
        static void Postfix(Gutterman __instance)
        {
            if (!UltraVoice.GuttermanVoiceEnabled.value)
                return;

            UltraVoice.EnemySpawnTimes[__instance] = Time.time;

            UltraVoice.PlayRandomVoice(
                 __instance,
                 UltraVoice.GuttermanSpawnClips,
                 UltraVoice.GuttermanSpawnSubs,
                 false
            );
        }
    }

    [HarmonyPatch(typeof(Gutterman), "ShieldBreak")]
    class GuttermanShieldBreak
    {
        static void Postfix(Gutterman __instance)
        {
            if (!UltraVoice.GuttermanVoiceEnabled.value)
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.GuttermanShieldBreakClips,
                UltraVoice.GuttermanShieldBreakSubs,
                false
            );
        }
    }

    [HarmonyPatch(typeof(Gutterman), "Enrage")]
    class GuttermanEnrage
    {
        static void Postfix(Gutterman __instance)
        {
            if (!UltraVoice.GuttermanVoiceEnabled.value)
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.GuttermanEnrageClips,
                UltraVoice.GuttermanEnrageSubs,
                false
            );
        }
    }

    [HarmonyPatch(typeof(Gutterman), "Death")]
    class GuttermanDeath
    {
        static void Postfix(Gutterman __instance)
        {
            if (!UltraVoice.GuttermanVoiceEnabled.value)
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.GuttermanDeathClips,
                null,
                true
            );
        }
    }

    [HarmonyPatch(typeof(Gutterman), "GotParried")]
    class GuttermanParry
    {
        static void Postfix(Gutterman __instance)
        {
            if (!UltraVoice.GuttermanVoiceEnabled.value)
                return;

            if (!UltraVoice.CheckCooldown(__instance, 3f))
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.GuttermanParryClips,
                null,
                true
            );
        }
    }

    // MANNEQUIN PATCHES

    [HarmonyPatch(typeof(Mannequin), "Update")]
    class MannequinChatter
    {
        static void Postfix(Mannequin __instance)
        {
            if (!UltraVoice.CheckCooldown(__instance, 4f))
                return;

            if (!UltraVoice.MannequinVoiceEnabled.value)
                return;

            if (UnityEngine.Random.value > 0.75f)
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.MannequinChatterClips,
                null
            );
        }
    }

    [HarmonyPatch(typeof(Mannequin), "MeleeAttack")]
    class MannequinSwing
    {
        static void Postfix(Mannequin __instance)
        {
            if (!UltraVoice.MannequinVoiceEnabled.value)
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.MannequinChatterClips,
                null
            );
        }
    }

    [HarmonyPatch(typeof(Mannequin), "OnDeath")]
    class MannequinDeath
    {
        static void Postfix(Mannequin __instance)
        {
            if (!UltraVoice.MannequinVoiceEnabled.value)
                return;

            UltraVoice.PlayRandomVoice(
                __instance,
                UltraVoice.MannequinDeathClips,
                null,
                true
            );
        }
    }

    // GUTTERTANK PATCHES


    [HarmonyPatch(typeof(Guttertank), "Start")]
    class GuttertankSpawn
    {
        static void Postfix(Guttertank __instance)
        {
            if (__instance == null)
                return;

            if (!UltraVoice.GuttertankVoiceEnabled.value)
                return;

            if (SceneManager.GetActiveScene().name == UltraVoice.THROUGHTHEMIRROR_SCENE && !UltraVoice.GuttertankSpawnSkippedInMirror)
            {
                UltraVoice.GuttertankSpawnSkippedInMirror = true;
                UltraVoice.EnemySpawnTimes[__instance] = Time.time;
                return;
            }

            UltraVoice.EnemySpawnTimes[__instance] = Time.time;

            UltraVoice.Instance.StartCoroutine(UltraVoice.DelayedVox(() =>
                UltraVoice.PlayRandomVoice(
                    __instance,
                    UltraVoice.GuttertankSpawnClips,
                    UltraVoice.GuttertankSpawnSubs,
                    false
                ),
                () => UltraVoice.GuttertankSpawnClips != null && UltraVoice.GuttertankSpawnClips.Length > 0,
                __instance
            ));
        }
    }

    [HarmonyPatch(typeof(Guttertank), "TargetBeenHit")]
    class GuttertankPunchHitVoice
    {
        static void Postfix(Guttertank __instance)
        {
            if (__instance == null)
                return;

            if (!UltraVoice.GuttertankVoiceEnabled.value)
                return;

            UltraVoice.Instance.StartCoroutine(UltraVoice.DelayedVox(() =>
                        UltraVoice.PlayRandomVoice(
                            __instance,
                            UltraVoice.GuttertankPunchHitClips,
                            UltraVoice.GuttertankPunchHitSubs
                        ),
                    () => UltraVoice.GuttertankPunchHitClips != null && UltraVoice.GuttertankPunchHitClips.Length > 0,
                    __instance
                ));
        }
    }

    [HarmonyPatch(typeof(Guttertank), "FallImpact")]
    class GuttertankFallImpactVoice
    {
        static void Postfix(Guttertank __instance)
        {
            if (__instance == null)
                return;

            if (!UltraVoice.GuttertankVoiceEnabled.value)
                return;

            UltraVoice.Instance.StartCoroutine(UltraVoice.DelayedVox(() =>
                        UltraVoice.PlayRandomVoice(
                            __instance,
                            UltraVoice.GuttertankTripPainClips,
                            null
                        ),
                    () => UltraVoice.GuttertankTripPainClips != null && UltraVoice.GuttertankTripPainClips.Length > 0,
                    __instance
                ));

            UltraVoice.Instance.StartCoroutine(DelayedPunchTripVox(__instance));
        }

        static IEnumerator DelayedPunchTripVox(Guttertank tank)
        {
            yield return new WaitForSeconds(0.9f);

            UltraVoice.PlayRandomVoice(
                tank,
                UltraVoice.GuttertankFrustratedClips,
                UltraVoice.GuttertankFrustratedSubs
            );
        }
    }

    [HarmonyPatch(typeof(Guttertank), "Death")]
    class GuttertankDeath
    {
        static void Postfix(Guttertank __instance)
        {
            if (__instance == null)
                return;

            if (!UltraVoice.GuttertankVoiceEnabled.value)
                return;

            UltraVoice.Instance.StartCoroutine(UltraVoice.DelayedVox(() =>
                        UltraVoice.PlayRandomVoice(
                            __instance,
                            UltraVoice.GuttertankDeathClips,
                            null,
                            true
                        ),
                    () => UltraVoice.GuttertankDeathClips != null && UltraVoice.GuttertankDeathClips.Length > 0,
                    __instance
                ));
        }
    }

    [HarmonyPatch(typeof(MusicManager), "Update")]
    public class MusicManager_Update_Patch
    {
        static AudioClip lastClip;

        static void Postfix(MusicManager __instance)
        {
            var src = __instance.targetTheme;
            if (src == null || src.clip == null)
                return;

            if (src.clip != lastClip)
            {
                lastClip = src.clip;

                if (src.clip.name == "Centaur A-1")
                {
                    UltraVoice.Instance.StartCoroutine(PlayCentaurVoice());
                }
            }
        }

        private static IEnumerator PlayCentaurVoice()
        {
            yield return new WaitForSeconds(1f);

            UltraVoice.ShowSubtitle("TESTING... TESTING. ONE, TWO, THREE...", null);
        }
    }
}
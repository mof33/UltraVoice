using BepInEx;
using HarmonyLib;
using PluginConfig.API;
using PluginConfig.API.Fields;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UltraVoice.Characters;
using UltraVoice.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UltraVoice
{
    [BepInPlugin("com.mel33.ultravoice", "UltraVoice", "1.0.2")]
    [BepInDependency("com.eternalUnion.pluginConfigurator")]
    public class UltraVoicePlugin : BaseUnityPlugin
    {
        public static UltraVoicePlugin Instance;
        public static VoiceManager VoiceManager;
        private Harmony harmony;

        private PluginConfigurator config;

        public static BoolField CerberusVoiceEnabled;
        public static BoolField SwordsmachineVoiceEnabled;
        public static BoolField V2VoiceEnabled;
        public static BoolField StreetcleanerVoiceEnabled;
        public static BoolField MindflayerVoiceEnabled;
        public static BoolField VirtueVoiceEnabled;
        public static BoolField FerrymanVoiceEnabled;
        public static BoolField GuttermanVoiceEnabled;
        public static BoolField MannequinVoiceEnabled;
        public static BoolField GuttertankVoiceEnabled;
        public static BoolField ProvidenceVoiceEnabled;
        public static BoolField SentryVoiceEnabled;
        public static BoolField MauriceVoiceEnabled;
        public static BoolField EarthmoverVoiceEnabled;
        public static BoolField MirrorReaperVoiceEnabled;
        public static BoolField GeryonVoiceEnabled;
        public static BoolField LeviathanVoiceEnabled;
        public static BoolField InsurrectionistVoiceEnabled;
        public static BoolField FilthVoiceEnabled;
        public static BoolField StrayVoiceEnabled;
        public static BoolField SchismVoiceEnabled;
        public static BoolField SoldierVoiceEnabled;
        public static BoolField PowerSubtitleColorEnabled;
        public static FloatField VoiceCooldown;
        public static IntField SubtitleLimit;

        public static EnumField<SwordsmachineVoiceActor> SwordsmachineVoiceActorField;
        public static EnumField<SentryVoiceActor> SentryVoiceActorField;
        public static ConfigPanel TogglesPanel;
        public static ConfigPanel ActorPanel;

        public enum SwordsmachineVoiceActor
        {
            Mel,
            Noto
        }

        public enum SentryVoiceActor
        {
            Noto,
            Goober
        }

        void Awake()
        {
            Instance = this;
            VoiceManager = new VoiceManager();

            config = PluginConfigurator.Create("UltraVoice", "com.mel33.ultravoice");

            // Create panels
            TogglesPanel = new ConfigPanel(config.rootPanel, "Enemy Line Toggles", "toggles");
            ActorPanel = new ConfigPanel(config.rootPanel, "Voice Actors", "actors");

            // Enemy toggles
            CerberusVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Cerberus Voice Lines",
                "cerberusvoice",
                true
            );

            SwordsmachineVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Swordsmachine Voice Lines",
                "swordsmachinevoice",
                true
            );

            V2VoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable V2 Voice Lines",
                "v2voice",
                true
            );

            StreetcleanerVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Streetcleaner Voice Lines",
                "streetcleanervoice",
                true
            );

            MindflayerVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Mindflayer Voice Lines",
                "mindflayervoice",
                true
            );

            VirtueVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Virtue Voice Lines",
                "virtuevoice",
                true
            );

            FerrymanVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Ferryman Voice Lines",
                "ferrymanvoice",
                true
            );

            GuttermanVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Gutterman Voice Lines",
                "guttermanvoice",
                true
            );

            MannequinVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Mannequin Voice Lines",
                "mannequinvoice",
                true
            );

            GuttertankVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Guttertank Voice Lines",
                "guttertankvoice",
                true
            );

            ProvidenceVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Providence Voice Lines",
                "providencevoice",
                true
            );

            SentryVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Sentry Voice Lines",
                "sentryvoice",
                true
            );

            MauriceVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Malicious Face Voice Lines",
                "mauricevoice",
                true
            );

            EarthmoverVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Earthmover Voice Lines",
                "earthmovervoice",
                true
            );

            MirrorReaperVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Mirror Reaper Voice Lines",
                "mirrorreapervoice",
                true
            );

            GeryonVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Geryon Voice Lines",
                "geryonvoice",
                true
            );

            LeviathanVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Leviathan Voice Lines",
                "levivoice",
                true
            );

            InsurrectionistVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Insurrctionist Voice Lines",
                "insurvoice",
                true
            );

            FilthVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Filth Voice Lines",
                "filthvoice",
                true
            );

            StrayVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Filth Voice Lines",
                "filthvoice",
                true
            );

            SchismVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Filth Voice Lines",
                "filthvoice",
                true
            );

            SoldierVoiceEnabled = new BoolField(
                TogglesPanel,
                "Enable Filth Voice Lines",
                "filthvoice",
                true
            );

            PowerSubtitleColorEnabled = new BoolField(
                config.rootPanel,
                "Enable Power Subtitle Color",
                "powersubcolor",
                true
            );

            VoiceCooldown = new FloatField(
                config.rootPanel,
                "Voice Cooldown",
                "cooldown",
                0.25f,
                0f,
                0.5f
            );

            SubtitleLimit = new IntField(
                config.rootPanel, 
                "Max Allowed Subtitles",
                "subtitlelimit",
                5,
                1,
                10
            );

            UltraVoicePlugin.SwordsmachineVoiceActorField = new EnumField<SwordsmachineVoiceActor>(
                ActorPanel,
                "Swordsmachine Voice Actor",
                "smvoiceactor",
                SwordsmachineVoiceActor.Mel
            );

            UltraVoicePlugin.SentryVoiceActorField = new EnumField<SentryVoiceActor>(
                ActorPanel,
                "Sentry Voice Actor",
                "svoiceactor",
                SentryVoiceActor.Goober
            );

            UltraVoicePlugin.SwordsmachineVoiceActorField.SetEnumDisplayName(SwordsmachineVoiceActor.Mel, "Mel");
            UltraVoicePlugin.SwordsmachineVoiceActorField.SetEnumDisplayName(SwordsmachineVoiceActor.Noto, "Noto");

            UltraVoicePlugin.SentryVoiceActorField.SetEnumDisplayName(SentryVoiceActor.Goober, "Goober");
            UltraVoicePlugin.SentryVoiceActorField.SetEnumDisplayName(SentryVoiceActor.Noto, "Noto");

            LoadAssets();

            harmony = new Harmony("com.mel33.ultravoice");
            harmony.PatchAll();

            SceneManager.sceneLoaded += OnSceneLoaded;

            Logger.LogInfo("UltraVoice loaded successfully!");
        }

        void OnDestroy()
        {
            harmony.UnpatchSelf();
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ResetCharacterStates();
        }

        void ResetCharacterStates()
        {
            SwordsmachineCharacter.FirstFightDone = false;
            SwordsmachineCharacter.FirstFightLinePlayed = false;

            V2Character.V2IntroTime = -999f;
            V2Character.V2CutsceneVoicePlayed = false;
            V2Character.V2SecondVoiceRestartPlayed = false;
            V2Character.V2DeathPlayed = false;

            FerrymanCharacter.FerrymanCoinTossed = false;
            FerrymanCharacter.FerrymanPhaseChangePlayed = false;

            GuttertankCharacter.GuttertankSpawnInMirror = false;

            GeryonCharacter.EnrageLinePlayed = false;
            GeryonCharacter.CanRestartFight = false;
        }

        void LoadAssets()
        {
            // Load character voice lines from embedded resources
            CerberusCharacter.LoadVoiceLines(Logger);
            SwordsmachineCharacter.LoadVoiceLines(Logger);
            V2Character.LoadVoiceLines(Logger);
            MindflayerCharacter.LoadVoiceLines(Logger);
            VirtueCharacter.LoadVoiceLines(Logger);
            StreetcleanerCharacter.LoadVoiceLines(Logger);
            FerrymanCharacter.LoadVoiceLines(Logger);
            MannequinCharacter.LoadVoiceLines(Logger);
            GuttermanCharacter.LoadVoiceLines(Logger);
            GuttertankCharacter.LoadVoiceLines(Logger);
            ProvidenceCharacter.LoadVoiceLines(Logger);
            SentryCharacter.LoadVoiceLines(Logger);
            MaliciousFaceCharacter.LoadVoiceLines(Logger);
            EarthmoverCharacter.LoadVoiceLines(Logger);
            MirrorReaperCharacter.LoadVoiceLines(Logger);
            GeryonCharacter.LoadVoiceLines(Logger);
            LeviathanCharacter.LoadVoiceLines(Logger);
            InsurrectionistCharacter.LoadVoiceLines(Logger);
        }

        public static AudioClip LoadClip(string resourcePath)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();

                if (!resourcePath.StartsWith("UltraVoice.Resources."))
                    resourcePath = $"UltraVoice.Resources.{resourcePath}";

                using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    if (stream == null)
                    {
                        Instance.Logger.LogWarning($"UltraVoice missing clip: {resourcePath}");
                        return null;
                    }

                    byte[] audioData = new byte[stream.Length];
                    stream.Read(audioData, 0, audioData.Length);

                    AudioClip clip = WavUtility.ToAudioClip(audioData);

                    if (clip == null)
                    {
                        Instance.Logger.LogWarning($"UltraVoice failed to convert clip: {resourcePath}");
                        return null;
                    }

                    return clip;
                }
            }
            catch (System.Exception e)
            {
                Instance.Logger.LogError($"UltraVoice error loading clip {resourcePath}: {e}");
                return null;
            }
        }

        public static IEnumerator DelayedVox(System.Action playAction, System.Func<bool> ready, UnityEngine.Component attached)
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
    }
}

[HarmonyPatch(typeof(GabrielVoice), "Hurt")]
class GabrielHurtPatch
{
    static bool Prefix() => false;
}
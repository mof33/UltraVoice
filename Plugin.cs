using BepInEx;
using HarmonyLib;
using PluginConfig.API;
using PluginConfig.API.Fields;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UltraVoice.Characters;
using UltraVoice.Utilities;

namespace UltraVoice
{
    [BepInPlugin("com.yourname.ultravoice", "UltraVoice", "0.9.8")]
    [BepInDependency("com.eternalUnion.pluginConfigurator")]
    public class UltraVoicePlugin : BaseUnityPlugin
    {
        public static UltraVoicePlugin Instance;
        public static VoiceManager VoiceManager;

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
        public static FloatField VoiceCooldown;
        public static FloatField VoiceVolume;
        public static EnumField<SwordsmachineVoiceActor> SwordsmachineVoiceActorField;
        public static ConfigPanel TogglesPanel;
        public static ConfigPanel SlidersPanel;
        public static ConfigPanel ActorPanel;

        public enum SwordsmachineVoiceActor
        {
            Mel,
            Noto
        }

        private static Dictionary<ICharacter, Dictionary<string, AudioClip[]>> characterVoiceLines =
            new Dictionary<ICharacter, Dictionary<string, AudioClip[]>>();

        void Awake()
        {
            Instance = this;
            VoiceManager = new VoiceManager();

            config = PluginConfigurator.Create("UltraVoice", "com.yourname.ultravoice");

            config.SetIconWithURL("https://storage.filebin.net/filebin/b20425983c28fd7feab09818ce6af10c2e766bd0e547ab3bd40a9709c9474171?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=GK352fd2505074fc9dde7fd2cb%2F20260331%2Fhel1-dc4%2Fs3%2Faws4_request&X-Amz-Date=20260331T220455Z&X-Amz-Expires=900&X-Amz-SignedHeaders=host&response-cache-control=max-age%3D900&response-content-disposition=inline%3B%20filename%3D%22icon.png%22&response-content-type=image%2Fpng&x-id=GetObject&X-Amz-Signature=965a1e646897a43091c19c99eaae70175fefe00de01754a84238fa971ba6780b");

            // Create panels
            TogglesPanel = new ConfigPanel(config.rootPanel, "Enemy Line Toggles", "toggles");
            SlidersPanel = new ConfigPanel(config.rootPanel, "Audio Settings", "sliders");
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

            VoiceCooldown = new FloatField(
                SlidersPanel,
                "Voice Cooldown",
                "cooldown",
                0.25f,
                0f,
                0.35f
            );

            VoiceVolume = new FloatField(
                SlidersPanel,
                "Voice Volume",
                "volume",
                1f,
                0f,
                1f
            );

            UltraVoicePlugin.SwordsmachineVoiceActorField = new EnumField<SwordsmachineVoiceActor>(
                ActorPanel,
                "Swordsmachine Voice Actor",
                "smvoiceactor",
                SwordsmachineVoiceActor.Mel
            );

            UltraVoicePlugin.SwordsmachineVoiceActorField.SetEnumDisplayName(SwordsmachineVoiceActor.Mel, "Mel");
            UltraVoicePlugin.SwordsmachineVoiceActorField.SetEnumDisplayName(SwordsmachineVoiceActor.Noto, "Noto");

            LoadAssets();

            new Harmony("com.yourname.ultravoice").PatchAll();

            SceneManager.sceneLoaded += OnSceneLoaded;

            Logger.LogInfo("UltraVoice loaded successfully!");
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
        }

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

            // Load character voice lines
            CerberusCharacter.LoadVoiceLines(bundle, Logger);
            SwordsmachineCharacter.LoadVoiceLines(bundle, Logger);
            V2Character.LoadVoiceLines(bundle, Logger);
            MindflayerCharacter.LoadVoiceLines(bundle, Logger);
            VirtueCharacter.LoadVoiceLines(bundle, Logger);
            StreetcleanerCharacter.LoadVoiceLines(bundle, Logger);
            FerrymanCharacter.LoadVoiceLines(bundle, Logger);
            MannequinCharacter.LoadVoiceLines(bundle, Logger);
            GuttermanCharacter.LoadVoiceLines(bundle, Logger);
            GuttertankCharacter.LoadVoiceLines(bundle, Logger);
            ProvidenceCharacter.LoadVoiceLines(bundle, Logger);
            SentryCharacter.LoadVoiceLines(bundle, Logger);
            MaliciousFaceCharacter.LoadVoiceLines(bundle, Logger);
            EarthmoverCharacter.LoadVoiceLines(bundle, Logger);
        }

        public static AudioClip LoadClip(AssetBundle bundle, string name)
        {
            var clip = bundle.LoadAsset<AudioClip>(name);

            if (clip == null)
                Instance.Logger.LogWarning($"UltraVoice missing clip: {name}");

            return clip;
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
using HarmonyLib;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class SchismCharacter
    {
        public static AudioClip[] ChatterClips;
        public static AudioClip AttackClip;
        public static AudioClip DeathClip;

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Schism.sch_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Schism.sch_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Schism.sch_Chatter1.wav"),
            };

            AttackClip = UltraVoicePlugin.LoadClip("Schism.sch_Attack.wav");
            DeathClip = UltraVoicePlugin.LoadClip("Schism.sch_Death.wav");

            logger.LogInfo("Schism voice lines loaded successfully!");
        }
    }


    [HarmonyPatch(typeof(ZombieProjectiles), "Update")]
    class SchismChatterPatch
    {
        static void Postfix(ZombieProjectiles __instance)
        {
            if (!UltraVoicePlugin.SchismVoiceEnabled.value) return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (__instance.eid.enemyType != EnemyType.Schism) return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
                return;

            VoiceManager.PlayRandomVoice(__instance, "Schism",
                SchismCharacter.ChatterClips,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(ZombieProjectiles), nameof(ZombieProjectiles.SpawnProjectile))]
    class SchismThrowPatch
    {
        static void Postfix(ZombieProjectiles __instance)
        {
            if (!UltraVoicePlugin.StrayVoiceEnabled.value) return;

            if (__instance.eid.enemyType != EnemyType.Schism) return;

            VoiceManager.CreateVoiceSource(__instance, "Schism",
                SchismCharacter.AttackClip,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(ZombieProjectiles), nameof(ZombieProjectiles.OnGoLimp))]
    class SchismDeathPatch
    {
        static void Postfix(ZombieProjectiles __instance)
        {
            if (!UltraVoicePlugin.StrayVoiceEnabled.value) return;

            if (__instance.eid.enemyType != EnemyType.Schism) return;

            VoiceManager.CreateVoiceSource(__instance, "Schism",
                SchismCharacter.DeathClip,
                null,
                true,
                randomPitch: true
            );
        }
    }
}

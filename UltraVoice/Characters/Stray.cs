using HarmonyLib;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class StrayCharacter
    {
        public static AudioClip[] ChatterClips;
        public static AudioClip AttackClip;
        public static AudioClip DeathClip;

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Stray.str_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Stray.str_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Stray.str_Chatter1.wav"),
            };

            AttackClip = UltraVoicePlugin.LoadClip("Stray.str_Attack.wav");
            DeathClip = UltraVoicePlugin.LoadClip("Stray.str_Death.wav");

            logger.LogInfo("Stray voice lines loaded successfully!");
        }
    }


    [HarmonyPatch(typeof(ZombieProjectiles), "Update")]
    class StrayChatterPatch
    {
        static void Postfix(ZombieProjectiles __instance)
        {
            if (!UltraVoicePlugin.StrayVoiceEnabled.value) return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (__instance.eid.enemyType != EnemyType.Stray) return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (UnityEngine.Random.Range(0f, 1f) < 0.75f)
                return;

            VoiceManager.PlayRandomVoice(__instance, "Mannequin",
                StrayCharacter.ChatterClips,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(ZombieProjectiles), nameof(ZombieProjectiles.ThrowProjectile))]
    class StrayThrowPatch
    {
        static void Postfix(ZombieProjectiles __instance)
        {
            if (!UltraVoicePlugin.StrayVoiceEnabled.value) return;

            if (__instance.eid.enemyType != EnemyType.Stray) return;

            VoiceManager.CreateVoiceSource(__instance, "Mannequin",
                StrayCharacter.AttackClip,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(ZombieProjectiles), nameof(ZombieProjectiles.OnGoLimp))]
    class StrayDeathPatch
    {
        static void Postfix(ZombieProjectiles __instance)
        {
            if (!UltraVoicePlugin.StrayVoiceEnabled.value) return;

            if (__instance.eid.enemyType != EnemyType.Stray) return;

            VoiceManager.CreateVoiceSource(__instance, "Mannequin",
                StrayCharacter.DeathClip,
                null,
                true,
                randomPitch: true
            );
        }
    }
}

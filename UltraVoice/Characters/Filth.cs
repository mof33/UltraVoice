using HarmonyLib;
using UnityEngine;
using UltraVoice.Utilities;

namespace UltraVoice.Characters
{
    public class FilthCharacter
    {
        public static AudioClip[] ChatterClips;
        public static AudioClip AttackClip;
        public static AudioClip DeathClip;

        public static void LoadVoiceLines(BepInEx.Logging.ManualLogSource logger)
        {
            ChatterClips = new AudioClip[]
            {
                UltraVoicePlugin.LoadClip("Filth.fil_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Filth.fil_Chatter1.wav"),
                UltraVoicePlugin.LoadClip("Filth.fil_Chatter1.wav"),
            };

            AttackClip = UltraVoicePlugin.LoadClip("Filth.fil_Attack.wav");
            DeathClip = UltraVoicePlugin.LoadClip("Filth.fil_Death.wav");

            logger.LogInfo("Filth voice lines loaded successfully!");
        }
    }

    [HarmonyPatch(typeof(ZombieMelee), nameof(ZombieMelee.Swing))]
    class FilthAttackPatch
    {
        static void Postfix(ZombieMelee __instance)
        {
            if (!UltraVoicePlugin.FilthVoiceEnabled.value) return;

            VoiceManager.CreateVoiceSource(
                __instance, "Filth",
                FilthCharacter.AttackClip,
                null,
                randomPitch: true
            );
        }
    }

    [HarmonyPatch(typeof(ZombieMelee), nameof(ZombieMelee.JumpAttack))]
    class FilthJumpAttackPatch
    {
        static void Postfix(ZombieMelee __instance)
        {
            if (!UltraVoicePlugin.FilthVoiceEnabled.value) return;

            VoiceManager.CreateVoiceSource(
                __instance, "Filth",
                FilthCharacter.AttackClip,
                null,
                randomPitch: true
            );
        }
    }


    [HarmonyPatch(typeof(ZombieMelee), nameof(ZombieMelee.OnGoLimp))]
    class FilthDeathPatch
    {
        static void Postfix(ZombieMelee __instance)
        {
            if (!UltraVoicePlugin.FilthVoiceEnabled.value) return;

            VoiceManager.CreateVoiceSource(
                __instance, "Filth",
                FilthCharacter.DeathClip,
                null,
                randomPitch: true
            );
        }
    }


    [HarmonyPatch(typeof(ZombieMelee), "Update")]
    class FilthChatterPatch
    {
        static void Postfix(ZombieMelee __instance)
        {
            if (!UltraVoicePlugin.FilthVoiceEnabled.value) return;

            if (ULTRAKILL.Cheats.BlindEnemies.Blind)
                return;

            if (!VoiceManager.CheckCooldown(__instance, 4f))
                return;

            if (Random.Range(0f, 1f) < 0.75f)
                return;

            VoiceManager.PlayRandomVoice(__instance, "Filth",
                FilthCharacter.ChatterClips,
                null,
                randomPitch: true
            );
        }
    }
}

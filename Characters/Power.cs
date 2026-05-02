using HarmonyLib;
using TMPro;
using UnityEngine;

namespace UltraVoice.Characters
{
    [HarmonyPatch(typeof(SubtitleController), nameof(SubtitleController.DisplaySubtitle),
        new[] { typeof(string), typeof(AudioSource), typeof(bool) })]
    public class PowerSubtitlePatch
    {
        private static readonly Color PowerYellow = new Color(0.855f, 0.776f, 0.384f); // #dac762

        private static readonly string[] PowerLines = new[]
        {
            // Intro
            "Be afraid, machine.",
            "Here shall be your grave.",
            "It is over, machine!",
            "Surrender or perish!",
            "Lay down and die!",
            // Enrage
            "Bastard!",
            "You piece of SHIT!",
            "Just DIE already!",
            "Why won't you die!?",
            "God DAMN it!",
            // Taunt
            "This lowly thing could never have bested him!",
            "An inconvenience at best.",
            "This is a waste of my time!",
            "Just another worthless object.",
            // Cheap Shot
            "PAY ATTENTION!",
            "Wait your TURN!",
            "WRONG TARGET!",
            // Weapons
            "Rapier!",
            "Greatsword!",
            "Spear!",
            "Over here!",
            "Glaive!",
            "Take THIS!"
        };

        public static void Postfix(string caption, AudioSource audioSource, bool ignoreSetting)
        {
            // Check if this is a Power line
            bool isPowerLine = false;
            foreach (var line in PowerLines)
            {
                if (caption == line)
                {
                    isPowerLine = true;
                    break;
                }
            }

            if (!isPowerLine)
                return;

            // Find the most recently created subtitle and color it
            SubtitleController controller = MonoSingleton<SubtitleController>.Instance;
            if (controller != null && controller.previousSubtitle != null)
            {
                TMP_Text textComponent = controller.previousSubtitle.GetComponentInChildren<TMP_Text>();
                if (textComponent != null)
                {
                    textComponent.color = PowerYellow;
                }
            }
        }
    }
}
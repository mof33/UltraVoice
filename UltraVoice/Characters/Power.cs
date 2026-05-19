using HarmonyLib;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UltraVoice.Characters
{
    [HarmonyPatch(typeof(SubtitleController), nameof(SubtitleController.DisplaySubtitle),
        new[] { typeof(string), typeof(AudioSource), typeof(bool) })]
    public class PowerSubtitlePatch
    {
        private static readonly Color PowerYellow = new Color(0.855f, 0.776f, 0.384f);

        private static readonly string[] PowerLines = new[]
        {
            "Be afraid, machine.",
            "Here shall be your grave.",
            "It is over, machine!",
            "Surrender or perish!",
            "Lay down and die!",
            "Bastard!",
            "You piece of SHIT!",
            "Just DIE already!",
            "Why won't you die!?",
            "God DAMN it!",
            "This lowly thing could never have bested him!",
            "An inconvenience at best.",
            "This is a waste of my time!",
            "Just another worthless object.",
            "PAY ATTENTION!",
            "Wait your TURN!",
            "WRONG TARGET!",
            "Rapier!",
            "Greatsword!",
            "Spear!",
            "Over here!",
            "Glaive!",
            "Take THIS!",
            "HALT!",
            "Where is Gabriel and what have you done to him?",
            "Enough!",
            "Your insolence must be punished.",
            "There is no escape from Gabriel's children.",
            "WHERE IS HE!?"
        };

        public static void Postfix(string caption)
        {
            bool isPowerLine = PowerLines.Contains(caption);

            if (!isPowerLine)
                return;

            if (caption == "Enough!" && SceneHelper.CurrentScene != "Level 8-3")
                return;

            var controller = MonoSingleton<SubtitleController>.Instance;
            var subtitle = controller.previousSubtitle;

            var text = subtitle.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.color = PowerYellow;
            }
        }
    }
}
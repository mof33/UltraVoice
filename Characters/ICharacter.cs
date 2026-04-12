using UnityEngine;

namespace UltraVoice.Characters
{
    public interface ICharacter
    {
        string CharacterName { get; }
        void PlayVoiceLine(string actionName, bool interrupt = false);
    }
}

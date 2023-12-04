using UnityEngine;

namespace PixelCrushers.DialogueSystem
{
    public class ResetTypewriterSpeed : MonoBehaviour
    {
        public AbstractTypewriterEffect typewriterEffect;

        private float originalCharsPerSec = 0;

        private void Start()
        {
            if (typewriterEffect != null && !DialogueSystemController.isWarmingUp)
            {
                Debug.Log("Recording " + typewriterEffect.name + " original chars per second as " + originalCharsPerSec, typewriterEffect);
                originalCharsPerSec = typewriterEffect.charactersPerSecond;
            }
        }

        void OnConversationLine(Subtitle subtitle)
        {
            if (typewriterEffect != null)
            {
                if (originalCharsPerSec == 0) originalCharsPerSec = typewriterEffect.charactersPerSecond;
                Debug.Log("Resetting " + typewriterEffect.name + " to " + originalCharsPerSec + " chars per second", typewriterEffect);
                typewriterEffect.charactersPerSecond = originalCharsPerSec;
            }
        }
    }
}
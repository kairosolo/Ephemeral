using UnityEngine;
using TMPro;
namespace PixelCrushers.DialogueSystem
{
    public class StandardUIContinueButtonSpeedUp : StandardUIContinueButtonFastForward
    {

        public int fasterSpeed = 100;
        bool playing = false;
        public override void OnFastForward()
        {
            if (playing == false && (typewriterEffect != null) && typewriterEffect.isPlaying)
            {
                playing = true;
                Debug.Log("Speeding up " + typewriterEffect.name + " to " + fasterSpeed, typewriterEffect);
                var completeText = DialogueManager.currentConversationState.subtitle.formattedText.text;
                var textUI = typewriterEffect.GetComponent<TMP_Text>();
                var richTextColorTag = "color=<#FFFFFF>";
                var startIndex = textUI.text.IndexOf(richTextColorTag);

                var textSoFar = Tools.StripRichTextCodes(textUI.text.Substring(0, startIndex + richTextColorTag.Length));
                var charsSoFar = textSoFar.Length;
                typewriterEffect.charactersPerSecond = fasterSpeed;
                typewriterEffect.StartTyping(completeText, charsSoFar);
            }
            else
            {
                playing = false;
                base.OnFastForward();
            }
        }
    }
}
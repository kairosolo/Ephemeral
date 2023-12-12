using Language.Lua;
using UnityEngine;
using UnityEngine.Audio;

namespace PixelCrushers.DialogueSystem.VisualNovelFramework
{
    public class OptionsPanel : GeneralPanel
    {
        [SerializeField] private DialogueSkipper dialogueSkipper;

        [SerializeField] private AbstractTypewriterEffect[] typewriterEffects;
        [SerializeField] private AudioMixer musicMixer;
        [SerializeField] private AudioMixer sfxMixer;
        [SerializeField] private GameObject settingsPanel;

        [Header("Default Options Values")]
        public float defaultMusicVolume = .5f;

        public float defaultSFXVolume = .5f;
        public float defaultTextSpeed = 50;

        [Header("Options UI Controls")]
        public UnityEngine.UI.Slider musicVolumeSlider;

        public UnityEngine.UI.Slider sfxVolumeSlider;
        public UnityEngine.UI.Slider textSpeedSlider;

        public float musicVolume
        {
            get { return PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : defaultMusicVolume; }
            set { PlayerPrefs.SetFloat("MusicVolume", value); }
        }

        public float sfxVolume
        {
            get { return PlayerPrefs.HasKey("SfxVolume") ? PlayerPrefs.GetFloat("SfxVolume") : defaultSFXVolume; }
            set { PlayerPrefs.SetFloat("SfxVolume", value); }
        }

        public float TextSpeed
        {
            get { return PlayerPrefs.HasKey("TextSpeed") ? PlayerPrefs.GetFloat("TextSpeed") : defaultTextSpeed; }
            set { PlayerPrefs.SetFloat("TextSpeed", value); }
        }

        protected override void Start()
        {
            if (musicVolumeSlider != null) musicVolumeSlider.value = musicVolume;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVolume;
            if (textSpeedSlider != null) textSpeedSlider.value = TextSpeed;

            musicMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            sfxMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        }

        public override void Open()
        {
            settingsPanel.gameObject.SetActive(true);
        }

        public override void Close()
        {
            settingsPanel.gameObject.SetActive(false);
            dialogueSkipper.SkipDialogue();
        }

        public void MusicVolumeChanged()
        {
            SetVolume(musicVolumeSlider, GameObject.Find("Music Audio Source"), "MusicVolume");
            musicMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
        }

        public void SfxVolumeChanged()
        {
            SetVolume(sfxVolumeSlider, DialogueManager.Instance.gameObject, "SfxVolume");
            sfxMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        }

        private void SetVolume(UnityEngine.UI.Slider slider, GameObject audioSourceObject, string playerPrefsKey)
        {
            if (slider == null) return;
            var audioSource = (audioSourceObject != null) ? audioSourceObject.GetComponent<AudioSource>() : null;
            if (audioSource != null) audioSource.volume = slider.value;
            PlayerPrefs.SetFloat(playerPrefsKey, slider.value);
        }

        public void TypewriterSpeedChanged()
        {
            if (textSpeedSlider == null || typewriterEffects == null) return;
            foreach (AbstractTypewriterEffect typewriterEffect in typewriterEffects)
            {
                typewriterEffect.SetSpeed(textSpeedSlider.value);
                for (int i = 10; i <= textSpeedSlider.value; i++)
                {
                }
                if (textSpeedSlider.value >= 10)
                {
                    typewriterEffect.fullPauseDuration = 1.5f;
                    typewriterEffect.quarterPauseDuration = .35f;
                }
                if (textSpeedSlider.value >= 40)
                {
                    typewriterEffect.fullPauseDuration = 1f;
                    typewriterEffect.quarterPauseDuration = .25f;
                }
                if (textSpeedSlider.value >= 60)
                {
                    typewriterEffect.fullPauseDuration = .75f;
                    typewriterEffect.quarterPauseDuration = .15f;
                }
                if (textSpeedSlider.value >= 80)
                {
                    typewriterEffect.fullPauseDuration = .5f;
                    typewriterEffect.quarterPauseDuration = .1f;
                }
            }
            PlayerPrefs.SetFloat("TextSpeed", textSpeedSlider.value);
            DialogueManager.DisplaySettings.subtitleSettings.subtitleCharsPerSecond = Mathf.Min(textSpeedSlider.value, DialogueManager.DisplaySettings.subtitleSettings.subtitleCharsPerSecond);
        }
    }
}
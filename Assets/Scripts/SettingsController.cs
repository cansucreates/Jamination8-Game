using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [Header("UI")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Mixer")]
    public AudioMixer audioMixer; // GameAudioMixer
    public string musicParam = "MusicVolume";
    public string sfxParam = "SFXVolume";

    const string PREF_MUSIC = "musicVolume";
    const string PREF_SFX = "sfxVolume";

    private void Start()
    {
        float music = PlayerPrefs.GetFloat(PREF_MUSIC, 0.75f);
        float sfx = PlayerPrefs.GetFloat(PREF_SFX, 0.75f);

        musicSlider.value = music;
        sfxSlider.value = sfx;

        ApplyMusicVolume(music);
        ApplySfxVolume(sfx);

        musicSlider.onValueChanged.AddListener(OnMusicChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxChanged);
    }

    private void OnMusicChanged(float value)
    {
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat(PREF_MUSIC, value);
    }

    private void OnSfxChanged(float value)
    {
        ApplySfxVolume(value);
        PlayerPrefs.SetFloat(PREF_SFX, value);
    }

    private void ApplyMusicVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20;
        audioMixer.SetFloat(musicParam, dB);
    }

    private void ApplySfxVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20;
        audioMixer.SetFloat(sfxParam, dB);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}

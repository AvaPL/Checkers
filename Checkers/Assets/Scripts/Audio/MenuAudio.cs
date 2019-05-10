using System.Collections;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    public AudioSource MenuMusic;
    public AudioSource ButtonClick;
    public AudioSource SliderValueChange;
    public float MusicFadeDurationInSeconds;
    public int MusicFadeSteps;

    private int volume = 100;
    private float initialMenuMusicVolume;
    private float initialButtonClickVolume;
    private float initialSliderValueChangeVolume;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Volume"))
            volume = PlayerPrefs.GetInt("Volume");
    }

    private void Start()
    {
        SetInitialVolumeValues();
        SetVolume();
    }

    private void SetInitialVolumeValues()
    {
        initialMenuMusicVolume = MenuMusic.volume;
        initialButtonClickVolume = ButtonClick.volume;
        initialSliderValueChangeVolume = SliderValueChange.volume;
    }

    private void SetVolume()
    {
        MenuMusic.volume = initialMenuMusicVolume * volume / 100;
        ButtonClick.volume = initialButtonClickVolume * volume / 100;
        SliderValueChange.volume = initialSliderValueChangeVolume * volume / 100;
    }

    public void FadeMenuMusic()
    {
        StartCoroutine(SmoothMusicFade());
    }

    private IEnumerator SmoothMusicFade()
    {
        float volumePercentage = 1.0f;
        float fadeStepValue = volumePercentage / MusicFadeSteps;
        float timeStep = MusicFadeDurationInSeconds / MusicFadeSteps;
        while (volumePercentage > 0)
        {
            volumePercentage -= fadeStepValue;
            volume = Mathf.FloorToInt(volume * volumePercentage);
            SetVolume();
            yield return new WaitForSeconds(timeStep);
        }
    }

    public void PlayButtonClickSound()
    {
        ButtonClick.Play();
    }

    public void PlaySliderValueChange()
    {
        SliderValueChange.Play();
    }

    public void ChangeVolume(int newVolume)
    {
        volume = newVolume;
        SetVolume();
    }
}
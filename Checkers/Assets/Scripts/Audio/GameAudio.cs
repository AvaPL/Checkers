using System.Collections;
using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public AudioSource GameMusic;
    public AudioSource PawnSound;
    public AudioSource ButtonClick;
    public AudioSource PromotionSound;
    public float MusicFadeDurationInSeconds;
    public int MusicFadeSteps;

    private int volume = 100;
    private float initialGameMusicVolume;
    private float initialPawnSoundVolume;
    private float initialButtonClickVolume;
    private float initialPromotionSound;

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
        initialGameMusicVolume = GameMusic.volume;
        initialPawnSoundVolume = PawnSound.volume;
        initialButtonClickVolume = ButtonClick.volume;
        initialPromotionSound = PromotionSound.volume;
    }

    private void SetVolume()
    {
        GameMusic.volume = initialGameMusicVolume * volume / 100;
        PawnSound.volume = initialPawnSoundVolume * volume / 100;
        ButtonClick.volume = initialButtonClickVolume * volume / 100;
        PromotionSound.volume = initialPromotionSound * volume / 100;
    }

    public void FadeGameMusic()
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

    public void PlayPawnSound()
    {
        PawnSound.Play();
    }

    public void PlayButtonClickSound()
    {
        ButtonClick.Play();
    }

    public void PlayPromotionSound()
    {
        PromotionSound.Play();
    }
}
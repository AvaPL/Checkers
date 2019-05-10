using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public MenuAudio MenuAudio;

    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }

    public void UpdateVolume()
    {
        int volume = Mathf.RoundToInt(volumeSlider.value);
        MenuAudio.ChangeVolume(volume);
    }
}
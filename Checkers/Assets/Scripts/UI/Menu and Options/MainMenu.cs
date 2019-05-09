using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public Animator TitleScreenAnimator;

    public void Play(bool vsCPU)
    {
        TitleScreenAnimator.SetTrigger("PlayGame");
        PlayerPrefs.SetInt("VsCPU", vsCPU ? 1 : 0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
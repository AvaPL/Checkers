using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneLoader : MonoBehaviour
{
    public MenuAudio MenuAudio;

    public void LoadPlayScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void FadeMenuMusic()
    {
        MenuAudio.FadeMenuMusic();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play(bool vsCPU)
    {
        PlayerPrefs.SetInt("VsCPU", vsCPU ? 1 : 0);
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }



    public void ExitGame()
    {
        Application.Quit();
    }
}
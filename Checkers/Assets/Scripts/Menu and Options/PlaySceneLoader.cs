using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySceneLoader : MonoBehaviour
{
    public void LoadPlayScene()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
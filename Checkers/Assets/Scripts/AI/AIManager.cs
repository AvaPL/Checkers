using UnityEngine;

public class AIManager : MonoBehaviour
{
    public CPUPlayer CPUPlayer;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("VsCPU") == 1) return;
        CPUPlayer.enabled = false;
        gameObject.SetActive(false);
    }
}
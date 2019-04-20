using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public GameObject Board;
    public TextMeshProUGUI WinnerText;

    public void SetWinnerText(PawnColor winnerPawnColor)
    {
        WinnerText.text = winnerPawnColor.ToString().ToUpper() + " WINS";
    }

    public void DisableBoard()
    {
        Board.SetActive(false);
    }

    public void ReturnToMenu()
    {
        Debug.Log("Returning to menu");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game");
        Application.Quit();
    }
}
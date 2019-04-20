using TMPro;
using UnityEngine;

public class TurnTextChanger : MonoBehaviour
{
    private TextMeshProUGUI turnText;

    private void Start()
    {
        turnText = GetComponent<TextMeshProUGUI>();
    }

    public void ChangeTurnText(PawnColor pawnColor)
    {
        turnText.text = pawnColor.ToString().ToUpper() + "'S TURN";
    }
}
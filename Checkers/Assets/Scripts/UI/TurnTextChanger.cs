using TMPro;
using UnityEngine;

public class TurnTextChanger : MonoBehaviour
{
    private TextMeshProUGUI turnText;
    private Animator textAnimator;

    private void Start()
    {
        turnText = GetComponent<TextMeshProUGUI>();
        textAnimator = GetComponent<Animator>();
    }

    public void ChangeTurnText(PawnColor pawnColor)
    {
        turnText.text = pawnColor.ToString().ToUpper() + "'S TURN";
        textAnimator.SetTrigger("NextTurn");
    }
}
using UnityEngine;

public class TurnHandler : MonoBehaviour
{
    public PawnColor StartingPawnColor;
    public TurnTextChanger TurnTextChanger;
    public GameOverPanel GameOverPanel;

    private PawnColor turn;
    private int whitePawnCount;
    private int blackPawnCount;

    private void Awake()
    {
        turn = StartingPawnColor;
    }

    private void Start()
    {
        int boardSize = GetComponent<TilesGenerator>().BoardSize;
        int pawnRows = GetComponent<PawnsGenerator>().PawnRows;
        whitePawnCount = blackPawnCount = Mathf.CeilToInt(boardSize * pawnRows / 2f);
    }

    public void NextTurn()
    {
        turn = turn == PawnColor.White ? PawnColor.Black : PawnColor.White;
        TurnTextChanger.ChangeTurnText(turn);
    }

    public PawnColor GetTurn()
    {
        return turn;
    }

    public void DecrementPawnCount(GameObject pawn)
    {
        var pawnColor = pawn.GetComponent<PawnProperties>().PawnColor;
        if (pawnColor == PawnColor.White)
            --whitePawnCount;
        else
            --blackPawnCount;
        CheckVictory();
    }

    private void CheckVictory()
    {
        if (whitePawnCount == 0)
            EndGame(PawnColor.Black);
        else if (blackPawnCount == 0)
            EndGame(PawnColor.White);
    }

    private void EndGame(PawnColor winnerPawnColor)
    {
        GameOverPanel.gameObject.SetActive(true);
        GameOverPanel.SetWinnerText(winnerPawnColor);
    }

    public void Forfeit()
    {
        if (turn == PawnColor.White)
            EndGame(PawnColor.Black);
        else if (turn == PawnColor.Black)
            EndGame(PawnColor.White);
    }
}
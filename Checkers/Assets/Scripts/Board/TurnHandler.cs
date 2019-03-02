using UnityEngine;

public class TurnHandler : MonoBehaviour
{
    public PawnColor StartingPawnColor;

    private PawnColor turn;
    private int whitePawnCount;
    private int blackPawnCount;

    private void Awake()
    {
        turn = StartingPawnColor;
        int boardSize = GetComponent<TilesGenerator>().BoardSize;
        int pawnRows = GetComponent<PawnsGenerator>().PawnRows;
        whitePawnCount = blackPawnCount = Mathf.CeilToInt(boardSize * pawnRows / 2f);
    }

    public void NextTurn()
    {
        //TODO: Add text in UI and camera movement.
        turn = turn == PawnColor.White ? PawnColor.Black : PawnColor.White;
        Debug.Log("Turn: " + turn);
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
        //TODO: Add information in UI.
        if (whitePawnCount == 0)
            Debug.Log("Black won.");
        else if (blackPawnCount == 0)
            Debug.Log("White won.");
    }
}
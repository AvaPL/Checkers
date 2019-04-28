using UnityEngine;

public class CPUPlayer : MonoBehaviour
{
    public MoveTreeBuilder MoveTreeBuilder;
    private TileGetter tileGetter;

    private void Start()
    {
        tileGetter = GetComponent<TileGetter>();
    }

    public void DoPlayerMove(Move move)
    {
        MoveTreeBuilder.DoPlayerMove(move);
    }

    public void DoCPUMove()
    {
        Move move = MoveTreeBuilder.ChooseNextCPUMove();
        Debug.Log("Doing CPU move from: " + move.From.Column + ", " + move.From.Row + " to " + move.To.Column + ", " +
                  move.To.Row);
        var fromTileClickDetector = tileGetter.GetTile(move.From).GetComponent<TileClickDetector>();
        var toTileClickDetector = tileGetter.GetTile(move.To).GetComponent<TileClickDetector>();
        fromTileClickDetector.ClickTile();
        toTileClickDetector.ClickTile();
    }
}
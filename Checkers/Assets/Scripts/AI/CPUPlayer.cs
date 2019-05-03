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
        if (!MoveTreeBuilder.HasNextMove()) return; //TODO: Add forfeit when no moves available.
        Move move = MoveTreeBuilder.ChooseNextCPUMove();
        var fromTileClickDetector = tileGetter.GetTile(move.From).GetComponent<TileClickDetector>();
        var toTileClickDetector = tileGetter.GetTile(move.To).GetComponent<TileClickDetector>();
        fromTileClickDetector.ClickTile();
        toTileClickDetector.ClickTile();
    }
}
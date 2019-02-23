using UnityEngine;

public class PawnProperties : MonoBehaviour
{
    public PawnColor PawnColor;
    public bool IsKing;

    public TileIndex GetTileIndex()
    {
        return GetComponentInParent<TileProperties>().GetTileIndex();
    }

    public void PromoteToKing()
    {
        IsKing = true;
        Debug.Log("Pawn promoted to king.");
    }
}
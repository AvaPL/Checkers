using UnityEngine;

public class PawnProperties : MonoBehaviour
{
    public PawnColor PawnColor;
    public bool IsKing;

    public TileIndex GetTileIndex()
    {
        return GetComponentInParent<TileProperties>().GetTileIndex();
    }
}
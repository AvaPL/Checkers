using UnityEngine;

public class PawnProperties : MonoBehaviour
{
    public PawnColor PawnColor;
    public bool IsKing;
    public GameObject Crown;

    public TileIndex GetTileIndex()
    {
        return GetComponentInParent<TileProperties>().GetTileIndex();
    }

    public void PromoteToKing()
    {
        //TODO: Add particle effect.
        IsKing = true;
        Instantiate(Crown, transform);
        Debug.Log("Pawn promoted to king.");
    }
}
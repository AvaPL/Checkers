using UnityEngine;

public class TileProperties : MonoBehaviour
{
    public bool IsOccupied()
    {
        return GetComponentInChildren<IPawnProperties>() != null;
    }

    public GameObject GetPawn()
    {
        return GetComponentInChildren<IPawnProperties>().gameObject;
    }

    public TileIndex GetTileIndex()
    {
        return new TileIndex(transform.parent.GetSiblingIndex(), transform.GetSiblingIndex());
    }
}
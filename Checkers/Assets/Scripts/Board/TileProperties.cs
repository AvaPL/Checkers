using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProperties : MonoBehaviour
{
    public bool IsOccupied()
    {
        return GetComponentInChildren<PawnProperties>() != null;
    }

    public GameObject GetPawn()
    {
        return GetComponentInChildren<PawnProperties>().gameObject;
    }

    public Vector2 GetTileIndex()
    {
        Vector2 tileIndex;
        tileIndex.x = transform.parent.GetSiblingIndex();
        tileIndex.y = transform.GetSiblingIndex();
        return tileIndex;
    }
}

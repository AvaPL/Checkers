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

    public TileIndex GetTileIndex()
    {
        return new TileIndex(transform.parent.GetSiblingIndex(), transform.GetSiblingIndex());
    }
}
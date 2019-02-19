using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGetter : MonoBehaviour
{
    public GameObject GetTile(int columnIndex, int tileIndex)
    {
        return transform.GetChild(columnIndex).GetChild(tileIndex).gameObject;
    }
}
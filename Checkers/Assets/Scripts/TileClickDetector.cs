using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClickDetector : MonoBehaviour
{
    void OnMouseDown()
    {
        int columnIndex = transform.parent.GetSiblingIndex();
        int rowIndex = transform.GetSiblingIndex();
        Debug.Log("Tile clicked: " + columnIndex + ", " + rowIndex);
    }
}
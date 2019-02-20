using UnityEngine;

public class TileClickDetector : MonoBehaviour
{
    private void OnMouseDown()
    {
        LogTileIndex();
    }

    public void LogTileIndex()
    {
        int columnIndex = transform.parent.GetSiblingIndex();
        int rowIndex = transform.GetSiblingIndex();
        Debug.Log("Tile clicked: " + columnIndex + ", " + rowIndex);
    }
}
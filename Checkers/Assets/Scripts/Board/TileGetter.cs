using UnityEngine;

public class TileGetter : MonoBehaviour
{
    public GameObject GetTile(int columnIndex, int rowIndex)
    {
        return transform.GetChild(columnIndex).GetChild(rowIndex).gameObject;
    }
}
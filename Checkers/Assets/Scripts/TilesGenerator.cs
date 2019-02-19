using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesGenerator : MonoBehaviour
{
    public float TileSize;
    public int BoardSize;
    public GameObject WhiteTile;
    public GameObject BlackTile;

    void Start()
    {
        CreateTileColumns();
        CreateTiles();
    }

    void CreateTileColumns()
    {
        for (var i = 0; i < BoardSize; ++i)
            CreateTileColumn(i);
    }

    void CreateTileColumn(int columnIndex)
    {
        GameObject TileColumn = new GameObject("TileColumn" + columnIndex);
        TileColumn.transform.parent = this.gameObject.transform;
        TileColumn.transform.position = Vector3.right * columnIndex * TileSize;
    }

    void CreateTiles()
    {
        for (var i = 0; i < BoardSize; ++i)
        {
            for (var j = 0; j < BoardSize; ++j)
                CreateTile(i, j);
        }
    }

    void CreateTile(int columnIndex, int tileIndex)
    {
        var currentChild = transform.GetChild(columnIndex);
        GameObject tileToInstantiate = (columnIndex + tileIndex) % 2 == 0 ? WhiteTile : BlackTile;
        GameObject instantiatedTile = Instantiate(tileToInstantiate,
            currentChild.position + Vector3.forward * tileIndex * TileSize, tileToInstantiate.transform.rotation,
            currentChild);
        instantiatedTile.transform.localScale *= TileSize;
    }
}
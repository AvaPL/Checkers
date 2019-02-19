using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesGenerator : MonoBehaviour
{
    public float TileSize;
    public int BoardSize;
    public GameObject Tile;
    public Material WhiteMaterial;
    public Material BlackMaterial;

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
        var column = transform.GetChild(columnIndex);
        GameObject instantiatedTile = Instantiate(Tile, column.position + Vector3.forward * tileIndex * TileSize,
            Tile.transform.rotation, column);
        instantiatedTile.name = "Tile" + tileIndex;
        instantiatedTile.transform.localScale *= TileSize;
        instantiatedTile.GetComponent<Renderer>().material =
            (columnIndex + tileIndex) % 2 == 0 ? WhiteMaterial : BlackMaterial;
    }
}
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
        GameObject tileColumn = new GameObject("TileColumn" + columnIndex);
        tileColumn.transform.parent = this.gameObject.transform;
        tileColumn.transform.position = Vector3.right * columnIndex * TileSize;
    }

    void CreateTiles()
    {
        for (var columnIndex = 0; columnIndex < BoardSize; ++columnIndex)
        {
            for (var rowIndex = 0; rowIndex < BoardSize; ++rowIndex)
                CreateTile(columnIndex, rowIndex);
        }
    }

    void CreateTile(int columnIndex, int rowIndex)
    {
        var columnTransform = transform.GetChild(columnIndex);
        GameObject instantiatedTile = Instantiate(Tile,
            columnTransform.position + Vector3.forward * rowIndex * TileSize, Tile.transform.rotation,
            columnTransform);
        instantiatedTile.name = "Tile" + rowIndex;
        instantiatedTile.transform.localScale *= TileSize;
        instantiatedTile.GetComponent<Renderer>().material =
            (columnIndex + rowIndex) % 2 != 0 ? WhiteMaterial : BlackMaterial;
    }
}
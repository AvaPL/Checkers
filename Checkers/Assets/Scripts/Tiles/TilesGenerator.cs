using UnityEngine;

public class TilesGenerator : MonoBehaviour
{
    public float TileSize;
    public int BoardSize;
    public GameObject Tile;
    public Material WhiteMaterial;
    public Material BlackMaterial;

    private void Start()
    {
        CreateTileColumns();
        CreateTiles();
    }

    private void CreateTileColumns()
    {
        for (var i = 0; i < BoardSize; ++i)
            CreateTileColumn(i);
    }

    private void CreateTileColumn(int columnIndex)
    {
        GameObject tileColumn = new GameObject("TileColumn" + columnIndex);
        tileColumn.transform.parent = this.gameObject.transform;
        tileColumn.transform.position = Vector3.right * columnIndex * TileSize;
    }

    private void CreateTiles()
    {
        for (var columnIndex = 0; columnIndex < BoardSize; ++columnIndex)
        {
            for (var rowIndex = 0; rowIndex < BoardSize; ++rowIndex)
                CreateTile(columnIndex, rowIndex);
        }
    }

    private void CreateTile(int columnIndex, int rowIndex)
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
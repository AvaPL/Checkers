using UnityEngine;

public class TilesGenerator : MonoBehaviour, ITilesGenerator
{
    public int BoardSize { get; private set; } = 8;
    public GameObject Tile;
    public Material WhiteMaterial;
    public Material BlackMaterial;

    private void Start()
    {
        CreateTileColumns();
        CreateTiles();
    }

    private void Awake()
    {
        if (PlayerPrefs.HasKey("BoardSize"))
            BoardSize = PlayerPrefs.GetInt("BoardSize");
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
        tileColumn.transform.position = tileColumn.transform.parent.position + Vector3.right * columnIndex;
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
            columnTransform.position + Vector3.forward * rowIndex, Tile.transform.rotation,
            columnTransform);
        instantiatedTile.name = "Tile" + rowIndex;
        instantiatedTile.GetComponent<Renderer>().material =
            (columnIndex + rowIndex) % 2 != 0 ? WhiteMaterial : BlackMaterial;
    }
}
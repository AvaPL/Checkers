using UnityEngine;

public class PawnsGenerator : MonoBehaviour
{
    public int PawnRows;
    public GameObject Pawn;
    public Material WhiteMaterial;
    public Material BlackMaterial;

    private TileGetter _tileGetter;
    private int _boardSize;

    private void Start()
    {
        _tileGetter = GetComponent<TileGetter>();
        _boardSize = GetComponent<TilesGenerator>().BoardSize;
        GenerateWhitePawns();
        GenerateBlackPawns();
    }

    private void GenerateWhitePawns()
    {
        for (var rowIndex = 0; rowIndex < _boardSize && rowIndex < PawnRows; ++rowIndex)
        {
            for (var columnIndex = 0; columnIndex < _boardSize; ++columnIndex)
                if ((columnIndex + rowIndex) % 2 == 0)
                    GeneratePawn(columnIndex, rowIndex, WhiteMaterial);
        }
    }

    private void GeneratePawn(int columnIndex, int rowIndex, Material material)
    {
        Transform tileTransform = _tileGetter.GetTile(columnIndex, rowIndex).transform;
        GameObject instantiatedPawn = Instantiate(Pawn, tileTransform.position, Pawn.transform.rotation, tileTransform);
        instantiatedPawn.transform.localScale *= GetComponent<TilesGenerator>().TileSize;
        instantiatedPawn.GetComponent<Renderer>().material = material;
    }

    private void GenerateBlackPawns()
    {
        for (var rowIndex = _boardSize - 1; rowIndex >= 0 && rowIndex >= _boardSize - PawnRows; --rowIndex)
        {
            for (var columnIndex = _boardSize - 1; columnIndex >= 0; --columnIndex)
            {
                if ((rowIndex + columnIndex) % 2 == 0)
                    GeneratePawn(columnIndex, rowIndex, BlackMaterial);
            }
        }
    }
}
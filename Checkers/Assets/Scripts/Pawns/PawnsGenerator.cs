using UnityEngine;

public class PawnsGenerator : MonoBehaviour
{
    public int PawnRows;
    public GameObject Pawn;
    public Material WhiteMaterial;
    public Material BlackMaterial;

    private TileGetter tileGetter;
    private int boardSize;

    private void Awake()
    {
        tileGetter = GetComponent<TileGetter>();
        boardSize = GetComponent<TilesGenerator>().BoardSize;
    }

    private void Start()
    {
        GenerateWhitePawns();
        GenerateBlackPawns();
    }

    private void GenerateWhitePawns()
    {
        for (var rowIndex = 0; rowIndex < boardSize && rowIndex < PawnRows; ++rowIndex)
        {
            for (var columnIndex = 0; columnIndex < boardSize; ++columnIndex)
                if ((columnIndex + rowIndex) % 2 == 0)
                    GeneratePawn(columnIndex, rowIndex, WhiteMaterial);
        }
    }

    private void GeneratePawn(int columnIndex, int rowIndex, Material material)
    {
        Transform tileTransform = tileGetter.GetTile(columnIndex, rowIndex).transform;
        GameObject instantiatedPawn = Instantiate(Pawn, tileTransform.position, Pawn.transform.rotation, tileTransform);
        instantiatedPawn.GetComponent<Renderer>().material = material;
    }

    private void GenerateBlackPawns()
    {
        for (var rowIndex = boardSize - 1; rowIndex >= 0 && rowIndex >= boardSize - PawnRows; --rowIndex)
        {
            for (var columnIndex = boardSize - 1; columnIndex >= 0; --columnIndex)
            {
                if ((rowIndex + columnIndex) % 2 == 0)
                    GeneratePawn(columnIndex, rowIndex, BlackMaterial);
            }
        }
    }
}

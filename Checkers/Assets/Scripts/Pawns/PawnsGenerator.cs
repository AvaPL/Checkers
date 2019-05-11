using UnityEngine;

public class PawnsGenerator : MonoBehaviour
{
    public int PawnRows { get; private set; } = 3;
    public GameObject Pawn;
    public Material WhiteMaterial;
    public Material BlackMaterial;

    private TileGetter tileGetter;
    private int boardSize;

    private void Awake()
    {
        tileGetter = GetComponent<TileGetter>();
        boardSize = GetComponent<ITilesGenerator>().BoardSize;
        if (PlayerPrefs.HasKey("PawnRows"))
            PawnRows = PlayerPrefs.GetInt("PawnRows");
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
                    GeneratePawn(columnIndex, rowIndex, PawnColor.White);
        }
    }

    private void GeneratePawn(int columnIndex, int rowIndex, PawnColor pawnColor)
    {
        Transform tileTransform = tileGetter.GetTile(columnIndex, rowIndex).transform;
        GameObject instantiatedPawn = Instantiate(Pawn, tileTransform.position, Pawn.transform.rotation, tileTransform);
        instantiatedPawn.GetComponent<Renderer>().material =
            pawnColor == PawnColor.White ? WhiteMaterial : BlackMaterial;
        instantiatedPawn.GetComponent<IPawnProperties>().PawnColor = pawnColor;
    }

    private void GenerateBlackPawns()
    {
        for (var rowIndex = boardSize - 1; rowIndex >= 0 && rowIndex >= boardSize - PawnRows; --rowIndex)
        {
            for (var columnIndex = boardSize - 1; columnIndex >= 0; --columnIndex)
                if ((rowIndex + columnIndex) % 2 == 0)
                    GeneratePawn(columnIndex, rowIndex, PawnColor.Black);
        }
    }
}
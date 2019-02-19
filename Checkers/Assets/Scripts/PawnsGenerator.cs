using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnsGenerator : MonoBehaviour
{
    public int PawnRows;
    public GameObject Pawn;
    public Material WhiteMaterial;
    public Material BlackMaterial;

    private TileGetter tileGetter;
    private int boardSize;

    void Start()
    {
        tileGetter = GetComponent<TileGetter>();
        boardSize = GetComponent<TilesGenerator>().BoardSize;
        GenerateWhitePawns();
        GenerateBlackPawns();
    }

    void GenerateWhitePawns()
    {
        for (var rowIndex = 0; rowIndex < boardSize && rowIndex < PawnRows; ++rowIndex)
        {
            for (var columnIndex = 0; columnIndex < boardSize; ++columnIndex)
                if ((columnIndex + rowIndex) % 2 == 0)
                    GeneratePawn(columnIndex, rowIndex, WhiteMaterial);
        }
    }

    void GeneratePawn(int columnIndex, int rowIndex, Material material)
    {
        Transform tileTransform = tileGetter.GetTile(columnIndex, rowIndex).transform;
        GameObject instantiatedPawn = Instantiate(Pawn, tileTransform.position, Pawn.transform.rotation, tileTransform);
        instantiatedPawn.transform.localScale *= GetComponent<TilesGenerator>().TileSize;
        instantiatedPawn.GetComponent<Renderer>().material = material;
    }

    void GenerateBlackPawns()
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
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
        for (var i = 0; i < boardSize && i < PawnRows; ++i)
        {
            for (var j = 0; j < boardSize; ++j)
                if ((i + j) % 2 != 0)
                    GeneratePawn(i, j, WhiteMaterial);
        }
    }

    void GeneratePawn(int columnIndex, int tileIndex, Material material)
    {
        Transform tileTransform = tileGetter.GetTile(columnIndex, tileIndex).transform;
        GameObject instantiatedPawn = Instantiate(Pawn, tileTransform.position, Pawn.transform.rotation, tileTransform);
        instantiatedPawn.transform.localScale *= GetComponent<TilesGenerator>().TileSize;
        instantiatedPawn.GetComponent<Renderer>().material = material;
    }

    void GenerateBlackPawns()
    {
        for (var i = boardSize - 1; i >= 0 && i >= boardSize - PawnRows; --i)
        {
            for (var j = boardSize - 1; j >= 0; --j)
            {
                if ((i + j) % 2 != 0)
                    GeneratePawn(i, j, BlackMaterial);
            }
        }
    }
}
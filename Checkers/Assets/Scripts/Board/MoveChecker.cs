using System.Collections.Generic;
using UnityEngine;

public class MoveChecker : MonoBehaviour
{
    private List<GameObject> whitePawns = new List<GameObject>();
    private List<GameObject> blackPawns = new List<GameObject>();
    private int boardSize;
    private PawnMoveValidator pawnMoveValidator;
    private TileGetter tileGetter;
    private GameObject pawnToCheck;

    private void Awake()
    {
        boardSize = GetComponent<TilesGenerator>().BoardSize;
        pawnMoveValidator = GetComponent<PawnMoveValidator>();
        tileGetter = GetComponent<TileGetter>();
    }

    private void Start()
    {
        var pawnsProperties = GetComponentsInChildren<PawnProperties>();
        foreach (var element in pawnsProperties)
        {
            if (element.PawnColor == PawnColor.White)
                whitePawns.Add(element.gameObject);
            else
                blackPawns.Add(element.gameObject);
        }
    }

    public bool PawnsHaveCapturingMove(PawnColor pawnsColor)
    {
        var pawnsToCheck = pawnsColor == PawnColor.White ? whitePawns : blackPawns;
        foreach (var pawn in pawnsToCheck)
        {
            if (pawn == null)
                continue;
            if (PawnHasCapturingMove(pawn))
                return true;
        }

        return false;
    }

    public bool PawnHasCapturingMove(GameObject pawn)
    {
        pawnToCheck = pawn;
        TileIndex checkingDirectionInIndex = new TileIndex(1, 1);
        if (HasCapturingMoveOnDiagonal(checkingDirectionInIndex))
            return true;
        checkingDirectionInIndex = new TileIndex(-1, 1);
        if (HasCapturingMoveOnDiagonal(checkingDirectionInIndex))
            return true;
        return false;
    }

    private bool HasCapturingMoveOnDiagonal(TileIndex checkingDirectionInIndex)
    {
        for (var tileIndexToCheck = GetFirstTileIndexToCheck(checkingDirectionInIndex);
            IsIndexValid(tileIndexToCheck);
            tileIndexToCheck += checkingDirectionInIndex)
        {
            var tileToCheck = tileGetter.GetTile(tileIndexToCheck);
            if (pawnMoveValidator.IsCapturingMove(pawnToCheck, tileToCheck))
            {
                Debug.Log("Detected capturing move to: " + tileIndexToCheck.Column + ", " + tileIndexToCheck.Row);
                return true;
            }
        }

        return false;
    }

    private TileIndex GetFirstTileIndexToCheck(TileIndex checkingDirectionInIndex)
    {
        var firstTileIndexToCheck = pawnToCheck.GetComponent<PawnProperties>().GetTileIndex();
        while (IsIndexValid(firstTileIndexToCheck - checkingDirectionInIndex))
        {
            firstTileIndexToCheck -= checkingDirectionInIndex;
        }

        return firstTileIndexToCheck;
    }

    private bool IsIndexValid(TileIndex tileIndex)
    {
        return tileIndex.Column >= 0 && tileIndex.Column < boardSize && tileIndex.Row >= 0 && tileIndex.Row < boardSize;
    }

    public bool PawnHasAnyMove(GameObject pawn)
    {
        pawnToCheck = pawn;
        return PawnHasNoncapturingMove() || PawnHasCapturingMove(pawn);
    }

    private bool PawnHasNoncapturingMove()
    {
        TileIndex checkingDirectionInIndex = new TileIndex(1, 1);
        if (HasNoncapturingMoveOnDiagonal(checkingDirectionInIndex))
            return true;
        checkingDirectionInIndex = new TileIndex(-1, 1);
        if (HasNoncapturingMoveOnDiagonal(checkingDirectionInIndex))
            return true;
        return false;
    }

    private bool HasNoncapturingMoveOnDiagonal(TileIndex checkingDirectionInIndex)
    {
        var pawnTileIndex = pawnToCheck.GetComponent<PawnProperties>().GetTileIndex();
        var firstTileIndexToCheck = pawnTileIndex - checkingDirectionInIndex;
        if (IsIndexValid(firstTileIndexToCheck) && IsMoveValid(firstTileIndexToCheck))
            return true;
        var secondTileIndexToCheck = pawnTileIndex + checkingDirectionInIndex;
        if (IsIndexValid(secondTileIndexToCheck) && IsMoveValid(secondTileIndexToCheck))
            return true;
        return false;
    }

    private bool IsMoveValid(TileIndex targetTileIndex)
    {
        var tileToCheck = tileGetter.GetTile(targetTileIndex);
        return pawnMoveValidator.IsValidMove(pawnToCheck, tileToCheck);
    }
}
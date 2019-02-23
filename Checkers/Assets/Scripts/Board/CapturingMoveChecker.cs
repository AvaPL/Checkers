using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturingMoveChecker : MonoBehaviour
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

    public bool PawnHasCapturingMove(GameObject pawn)
    {
        pawnToCheck = pawn;
        TileIndex checkingDirectionInIndex = new TileIndex(1, 1);
        if (HasCapturingMoveOnDiagonal(checkingDirectionInIndex))
            return true;
        checkingDirectionInIndex = new TileIndex(-1, 1);
        if (HasCapturingMoveOnDiagonal(checkingDirectionInIndex))
            return true;
        Debug.Log("Pawn does not have capturing move.");
        return false;
    }

    private bool HasCapturingMoveOnDiagonal(TileIndex checkingDirectionInIndex)
    {
        var tileIndexToCheck = GetFirstTileIndexToCheck(checkingDirectionInIndex);
        while (IsIndexValid(tileIndexToCheck))
        {
            Debug.Log("Checking move to: " + tileIndexToCheck.Column + ", " + tileIndexToCheck.Row);
            var tileToCheck = tileGetter.GetTile(tileIndexToCheck);
            if (pawnMoveValidator.IsCapturingMove(pawnToCheck, tileToCheck))
                return true;
            tileIndexToCheck += checkingDirectionInIndex;
        }

        Debug.Log("Checked for capturing move on diagonal.");
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
}
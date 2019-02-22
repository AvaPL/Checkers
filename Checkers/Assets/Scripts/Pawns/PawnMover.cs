using System.Collections;
using UnityEngine;

public class PawnMover : MonoBehaviour
{
    public float MovementSmoothing;
    public float PositionDifferenceTolerance;

    private GameObject lastClickedTile;
    private GameObject lastClickedPawn;
    private bool isPawnMoving;

    public void TileClicked(GameObject tile)
    {
        Debug.Log("Tile clicked");
        lastClickedTile = tile;
        if (CanPawnBeMoved() && IsValidMove())
            MovePawn();
    }

    private bool CanPawnBeMoved()
    {
        return lastClickedPawn != null && !isPawnMoving;
    }

    private bool IsValidMove()
    {
        var positionDifference = lastClickedTile.GetComponent<TileProperties>().GetTileIndex() -
                                 lastClickedPawn.GetComponentInParent<TileProperties>().GetTileIndex();
        var pawnProperties = lastClickedPawn.GetComponent<PawnProperties>();
        if (pawnProperties.PawnColor == PawnColor.White)
        {
            Debug.Log("White position difference: " + positionDifference);
            return Mathf.Approximately(Mathf.Abs(positionDifference.x), Mathf.Abs(positionDifference.y)) &&
                   Mathf.Approximately(positionDifference.y, 1);
        }
        else
        {
            Debug.Log("Black position difference: " + positionDifference);
            return Mathf.Approximately(Mathf.Abs(positionDifference.x), Mathf.Abs(positionDifference.y)) &&
                   Mathf.Approximately(positionDifference.y, -1);
        }
    }

    private void MovePawn()
    {
        lastClickedPawn.transform.SetParent(lastClickedTile.transform);
        StartCoroutine(AnimatePawnMove());
    }

    public void PawnClicked(GameObject pawn)
    {
        if (pawn != lastClickedPawn)
        {
            Debug.Log("Pawn selected.");
            lastClickedPawn = pawn;
        }
        else
        {
            Debug.Log("Pawn unselected.");
            UnselectPawn();
        }
    }

    private void UnselectPawn()
    {
        lastClickedPawn = null;
    }

    IEnumerator AnimatePawnMove()
    {
        isPawnMoving = true;
        var pawnTransform = lastClickedPawn.transform;
        var targetTransform = lastClickedPawn.transform.parent;
        while (Vector3.Distance(pawnTransform.position, targetTransform.position) > PositionDifferenceTolerance)
        {
            pawnTransform.position = Vector3.Lerp(pawnTransform.position, targetTransform.position,
                MovementSmoothing * Time.deltaTime);
            yield return null;
        }

        UnselectPawn();
        isPawnMoving = false;
    }
}
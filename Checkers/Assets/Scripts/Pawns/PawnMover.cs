using System.Collections;
using UnityEngine;

public class PawnMover : MonoBehaviour
{
    public float HorizontalMovementSmoothing;
    public float VerticalMovementSmoothing;
    public float PositionDifferenceTolerance;

    private TileGetter tileGetter;
    private GameObject lastClickedTile;
    private GameObject lastClickedPawn;
    private bool isPawnMoving;
    private TileIndex targetTileIndex;
    private TileIndex currentTileIndex;
    private TileIndex positionDifferenceInIndex;
    private GameObject pawnToCapture;
    private float scale;

    private void Awake()
    {
        tileGetter = GetComponent<TileGetter>();
        scale = GetComponent<TilesGenerator>().Scale;
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

    public void TileClicked(GameObject tile)
    {
        Debug.Log("Tile clicked");
        lastClickedTile = tile;
        if (!CanPawnBeMoved())
            return;
        if (IsValidMove())
            MovePawn();
        else if (IsCapturingMove())
            CapturePawn();
    }

    private bool CanPawnBeMoved()
    {
        return lastClickedPawn != null && !isPawnMoving;
    }

    private bool IsValidMove()
    {
        SetIndexes();
        if (!IsMoveDiagonal())
            return false;
        else if (!IsPawnKing())
            return positionDifferenceInIndex.Row == GetPawnRowMoveDirection();
        else
            return IsPathCollidingWithOtherPawns();
    }

    private void SetIndexes()
    {
        targetTileIndex = lastClickedTile.GetComponent<TileProperties>().GetTileIndex();
        currentTileIndex = lastClickedPawn.GetComponent<PawnProperties>().GetTileIndex();
        positionDifferenceInIndex = targetTileIndex - currentTileIndex;
    }

    private bool IsMoveDiagonal()
    {
        return Mathf.Abs(positionDifferenceInIndex.Column) == Mathf.Abs(positionDifferenceInIndex.Row);
    }

    private bool IsPawnKing()
    {
        return lastClickedPawn.GetComponent<PawnProperties>().IsKing;
    }

    private int GetPawnRowMoveDirection()
    {
        var pawnProperties = lastClickedPawn.GetComponent<PawnProperties>();
        return pawnProperties.PawnColor == PawnColor.White ? 1 : -1;
    }

    private bool IsPathCollidingWithOtherPawns()
    {
        var moveDirectionInIndex = GetDiagonalMoveDirectionInIndex();
        for (var checkedTileIndex = currentTileIndex + moveDirectionInIndex;
            checkedTileIndex != targetTileIndex;
            checkedTileIndex += moveDirectionInIndex)
            if (IsTileOccupied(checkedTileIndex))
                return false;

        return true;
    }

    private TileIndex GetDiagonalMoveDirectionInIndex()
    {
        //Move direction means TileIndex with both values equal to +-1.
        return new TileIndex(positionDifferenceInIndex.Column / Mathf.Abs(positionDifferenceInIndex.Column),
            positionDifferenceInIndex.Row / Mathf.Abs(positionDifferenceInIndex.Row));
    }

    private bool IsTileOccupied(TileIndex tileIndex)
    {
        return tileGetter.GetTile(tileIndex).GetComponent<TileProperties>().IsOccupied();
    }

    private void MovePawn()
    {
        ChangeMovedPawnParent();
        StartCoroutine(AnimatePawnMove());
    }

    private void ChangeMovedPawnParent()
    {
        lastClickedPawn.transform.SetParent(lastClickedTile.transform);
    }

    private IEnumerator AnimatePawnMove()
    {
        isPawnMoving = true;
        var targetPosition = lastClickedPawn.transform.parent.position;
        yield return MoveHorizontal(targetPosition);
        UnselectPawn(); //TODO: Should be handled in turn changing class, leaving pawn selected is easier for multi-capturing.
        isPawnMoving = false;
    }

    private IEnumerator MoveHorizontal(Vector3 targetPosition)
    {
        var pawnTransform = lastClickedPawn.transform;
        while (Vector3.Distance(pawnTransform.position, targetPosition) > PositionDifferenceTolerance)
        {
            pawnTransform.position = Vector3.Lerp(pawnTransform.position, targetPosition,
                HorizontalMovementSmoothing * Time.deltaTime);
            yield return null;
        }
    }

    private bool IsCapturingMove()
    {
        SetIndexes();
        if (!IsMoveDiagonal())
            return false;
        return IsCapturePositionChangeValid() && IsOpponentsPawnOnOneBeforeTargetTile();
    }

    private bool IsCapturePositionChangeValid()
    {
        return (!IsPawnKing() && Mathf.Abs(positionDifferenceInIndex.Row) == 2) ||
               (IsPawnKing() && Mathf.Abs(positionDifferenceInIndex.Row) >= 2);
    }

    private bool IsOpponentsPawnOnOneBeforeTargetTile()
    {
        var moveDirectionInIndex = GetDiagonalMoveDirectionInIndex();
        for (var checkedTileIndex = currentTileIndex + moveDirectionInIndex;
            checkedTileIndex != targetTileIndex;
            checkedTileIndex += moveDirectionInIndex)
            if (IsTileOccupied(checkedTileIndex) && checkedTileIndex != targetTileIndex - moveDirectionInIndex)
                return false;
        SetPawnToCapture();
        return IsPawnToCaptureDifferentColorThanLastClickedPawn();
    }

    private void SetPawnToCapture()
    {
        var moveDirectionInIndex = GetDiagonalMoveDirectionInIndex();
        pawnToCapture = tileGetter.GetTile(targetTileIndex - moveDirectionInIndex).GetComponent<TileProperties>().GetPawn();
    }

    private bool IsPawnToCaptureDifferentColorThanLastClickedPawn()
    {
        return pawnToCapture.GetComponent<PawnProperties>().PawnColor !=
               lastClickedPawn.GetComponent<PawnProperties>().PawnColor;
    }

    private void CapturePawn()
    {
        ChangeMovedPawnParent();
        StartCoroutine(AnimatePawnCapture());
    }

    private IEnumerator AnimatePawnCapture()
    {
        isPawnMoving = true;
        yield return DoCaptureMovement();
        Destroy(pawnToCapture);
        UnselectPawn(); //TODO: Should be handled in turn changing class, leaving pawn selected is easier for multi-capturing.
        isPawnMoving = false;
    }

    private IEnumerator DoCaptureMovement()
    {
        var targetPosition = lastClickedPawn.transform.position + Vector3.up * scale;
        yield return MoveVertical(targetPosition);
        targetPosition = lastClickedPawn.transform.parent.position + Vector3.up * scale;
        yield return MoveHorizontal(targetPosition);
        targetPosition = lastClickedPawn.transform.position - Vector3.up * scale;
        yield return MoveVertical(targetPosition);
    }

    private IEnumerator MoveVertical(Vector3 targetPosition)
    {
        var pawnTransform = lastClickedPawn.transform;
        while (Vector3.Distance(pawnTransform.position, targetPosition) > PositionDifferenceTolerance)
        {
            pawnTransform.position = Vector3.Lerp(pawnTransform.position, targetPosition,
                VerticalMovementSmoothing * Time.deltaTime);
            yield return null;
        }
    }
}
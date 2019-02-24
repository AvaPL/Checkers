using System.Collections;
using UnityEngine;

public class PawnMover : MonoBehaviour
{
    public float HorizontalMovementSmoothing;
    public float VerticalMovementSmoothing;
    public float PositionDifferenceTolerance;

    private GameObject lastClickedTile;
    private GameObject lastClickedPawn;
    private PawnMoveValidator pawnMoveValidator;
    private CapturingMoveChecker capturingMoveChecker;
    private PromotionChecker promotionChecker;
    private bool isPawnMoving;
    private float scale;

    private void Awake()
    {
        scale = GetComponent<TilesGenerator>().Scale;
        pawnMoveValidator = GetComponent<PawnMoveValidator>();
        capturingMoveChecker = GetComponent<CapturingMoveChecker>();
        promotionChecker = GetComponent<PromotionChecker>();
    }

    public void PawnClicked(GameObject pawn)
    {
        if (isPawnMoving) return;
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
        if (isPawnMoving) return;
        Debug.Log("Tile clicked");
        lastClickedTile = tile;
        if (lastClickedPawn == null)
            return;
        if (MoveIsValidAndPawnCannotCapture())
            MovePawn();
        else if (pawnMoveValidator.IsCapturingMove(lastClickedPawn, lastClickedTile))
            CapturePawn();
    }

    private bool MoveIsValidAndPawnCannotCapture()
    {
        return pawnMoveValidator.IsValidMove(lastClickedPawn, lastClickedTile) &&
               !capturingMoveChecker.PawnHasCapturingMove(lastClickedPawn);
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
        promotionChecker.CheckPromotion(lastClickedPawn);
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

    private void CapturePawn()
    {
        ChangeMovedPawnParent();
        StartCoroutine(AnimatePawnCapture());
    }

    private IEnumerator AnimatePawnCapture()
    {
        isPawnMoving = true;
        yield return DoCaptureMovement();
        Destroy(pawnMoveValidator.GetPawnToCapture());
        promotionChecker.CheckPromotion(lastClickedPawn);
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
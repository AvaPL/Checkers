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
    private TurnHandler turnHandler;
    private bool isPawnMoving;
    private bool isMoveMulticapturing;
    private float scale;

    private void Awake()
    {
        scale = GetComponent<TilesGenerator>().Scale;
        pawnMoveValidator = GetComponent<PawnMoveValidator>();
        capturingMoveChecker = GetComponent<CapturingMoveChecker>();
        promotionChecker = GetComponent<PromotionChecker>();
        turnHandler = GetComponent<TurnHandler>();
    }

    public void PawnClicked(GameObject pawn)
    {
        if (!CanPawnBeSelected(pawn))
            return;
        //TODO: Add selected pawn highlight.
        if (pawn != lastClickedPawn)
        {
            Debug.Log("Pawn selected.");
            lastClickedPawn = pawn;
        }
        else
        {
            Debug.Log("Pawn unselected.");
            lastClickedPawn = null;
        }
    }

    private bool CanPawnBeSelected(GameObject pawn)
    {
        return !isPawnMoving && turnHandler.GetTurn() == GetPawnColor(pawn) && !isMoveMulticapturing;
    }

    private PawnColor GetPawnColor(GameObject pawn)
    {
        return pawn.GetComponent<PawnProperties>().PawnColor;
    }

    public void TileClicked(GameObject tile)
    {
        //TODO: Add available moves highlight.
        if (isPawnMoving) return;
        Debug.Log("Tile clicked");
        lastClickedTile = tile;
        if (lastClickedPawn == null)
            return;
        if (MoveIsValidAndNoncapturing())
            MovePawn();
        else if (pawnMoveValidator.IsCapturingMove(lastClickedPawn, lastClickedTile))
            CapturePawn();
    }

    private bool MoveIsValidAndNoncapturing()
    {
        PawnColor turn = turnHandler.GetTurn();
        if (capturingMoveChecker.PawnsHaveCapturingMove(turn))
            return false;
        return pawnMoveValidator.IsValidMove(lastClickedPawn, lastClickedTile);
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
        EndTurn();
        isPawnMoving = false;
    }

    private void EndTurn()
    {
        lastClickedPawn = null;
        isMoveMulticapturing = false;
        turnHandler.NextTurn();
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
        RemoveCapturedPawn();
        yield return null; //Waiting additional frame for captured pawn destruction.
        promotionChecker.CheckPromotion(lastClickedPawn);
        CheckForMulticapturingMove();
        isPawnMoving = false;
    }

    private void RemoveCapturedPawn()
    {
        GameObject pawnToCapture = pawnMoveValidator.GetPawnToCapture();
        turnHandler.DecrementPawnCount(pawnToCapture);
        Destroy(pawnToCapture);
    }

    private void CheckForMulticapturingMove()
    {
        if (capturingMoveChecker.PawnHasCapturingMove(lastClickedPawn))
            isMoveMulticapturing = true;
        else
            EndTurn();
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
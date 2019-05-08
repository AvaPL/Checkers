using UnityEngine;

public class AIPawnMover : MonoBehaviour
{
    private Move lastMove;
    private GameObject lastSelectedTile;
    private GameObject lastSelectedPawn;
    private PawnMoveValidator pawnMoveValidator;
    private MoveChecker moveChecker;
    private PromotionChecker promotionChecker;
    private TileGetter tileGetter;
    private bool isMoveMulticapturing;

    private void Awake()
    {
        pawnMoveValidator = GetComponent<PawnMoveValidator>();
        moveChecker = GetComponent<MoveChecker>();
        promotionChecker = GetComponent<PromotionChecker>();
        tileGetter = GetComponent<TileGetter>();
    }

    public void DoMove(Move move)
    {
        lastMove = move;
        SetPawnAndTile();
        ChooseAndDoMove();
        ResetLastValues();
    }

    private void SetPawnAndTile()
    {
        lastSelectedPawn = GetLastMovePawn();
        lastSelectedTile = GetLastMoveTile();
    }

    private GameObject GetLastMovePawn()
    {
        GameObject pawnTile = tileGetter.GetTile(lastMove.From);
        return GetPawnFromTile(pawnTile);
    }

    private GameObject GetPawnFromTile(GameObject pawnTile)
    {
        return pawnTile.GetComponent<TileProperties>().GetPawn();
    }

    private GameObject GetLastMoveTile()
    {
        return tileGetter.GetTile(lastMove.To);
    }

    private void ChooseAndDoMove()
    {
        if (IsMoveCapturing())
            CapturePawn();
        else
            MovePawn();
    }

    private bool IsMoveCapturing()
    {
        return pawnMoveValidator.IsCapturingMove(lastSelectedPawn, lastSelectedTile);
    }

    private void CapturePawn()
    {
        ChangePawnParent();
        DeactivateAndAddCapturedPawn();
        CheckPromotion();
        CheckMulticapture();
    }

    private void CheckPromotion()
    {
        if (lastSelectedPawn.GetComponent<IPawnProperties>().IsKing) return;
        promotionChecker.CheckPromotion(lastSelectedPawn);
        if (lastSelectedPawn.GetComponent<IPawnProperties>().IsKing)
            lastMove.WasPawnPromoted = true;
    }

    private void ChangePawnParent()
    {
        lastSelectedPawn.transform.SetParent(lastSelectedTile.transform);
        lastSelectedPawn.transform.position = lastSelectedPawn.transform.parent.position;
    }

    private void DeactivateAndAddCapturedPawn()
    {
        GameObject capturedPawn = pawnMoveValidator.GetPawnToCapture();
        lastMove.CapturedPawn = capturedPawn;
        capturedPawn.SetActive(false);
    }

    private void CheckMulticapture()
    {
        lastMove.IsMulticapturing = moveChecker.PawnHasCapturingMove(lastSelectedPawn);
    }

    private void MovePawn()
    {
        ChangePawnParent();
        CheckPromotion();
    }

    private void ResetLastValues()
    {
        lastMove = null;
        lastSelectedPawn = null;
        lastSelectedTile = null;
    }

    public void UndoMove(Move move)
    {
        lastMove = move;
        SetUndoPawnAndTile();
        MovePawn();
        ActivateCapturedPawn();
        UndoPromotion();
        ResetLastValues();
    }

    private void SetUndoPawnAndTile()
    {
        lastSelectedPawn = GetUndoMovePawn();
        lastSelectedTile = GetUndoMoveTile();
    }

    private GameObject GetUndoMovePawn()
    {
        GameObject pawnTile = tileGetter.GetTile(lastMove.To);
        return GetPawnFromTile(pawnTile);
    }

    private GameObject GetUndoMoveTile()
    {
        return tileGetter.GetTile(lastMove.From);
    }

    private void ActivateCapturedPawn()
    {
        GameObject capturedPawn = lastMove.CapturedPawn;
        if (capturedPawn != null)
            capturedPawn.SetActive(true);
    }

    private void UndoPromotion()
    {
        if (lastMove.WasPawnPromoted)
            GetLastMovePawn().GetComponent<IPawnProperties>().IsKing = false;
    }
}
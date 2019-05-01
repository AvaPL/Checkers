using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveTreeBuilder : MonoBehaviour
{
    public int MoveTreeDepth;

    private MoveChecker moveChecker;
    private AIPawnMover aiPawnMover;
    private TileGetter tileGetter;
    private TreeNode<Move> moveTree;
    private LinkedList<GameObject> whitePawns = new LinkedList<GameObject>();
    private LinkedList<GameObject> blackPawns = new LinkedList<GameObject>();
    private int whitePawnsCount;
    private int blackPawnsCount;

    private void Awake()
    {
        moveChecker = GetComponent<MoveChecker>();
        aiPawnMover = GetComponent<AIPawnMover>();
        tileGetter = GetComponent<TileGetter>();
    }

    private void Start()
    {
        InitializePawns();
        CountPawns();
    }

    private void InitializePawns()
    {
        var pawnsProperties = GetComponentsInChildren<PawnProperties>();
        foreach (var element in pawnsProperties)
        {
            if (element.PawnColor == PawnColor.White)
                whitePawns.AddLast(element.gameObject);
            else
                blackPawns.AddLast(element.gameObject);
        }
    }

    private void CountPawns()
    {
        whitePawnsCount = whitePawns.Count;
        blackPawnsCount = blackPawns.Count;
    }

    public void DoPlayerMove(Move playerMove)
    {
//        if (moveTree == null)
        CreateMoveTree(playerMove);
//        else
//            ChooseMoveInTree(playerMove);
        DoMove(playerMove);
    }

    private void CreateMoveTree(Move initialMove)
    {
        moveTree = new TreeNode<Move>(initialMove);
        AddMovesToTreeNode(moveTree, MoveTreeDepth);
    }

    private void AddMovesToTreeNode(TreeNode<Move> treeNode, int depth)
    {
        DoMove(treeNode.Value);
        if (depth != 0)
            AddPossibleMoves(treeNode, depth);
        AssignMoveScore(treeNode);
        UndoMove(treeNode.Value);
    }

    private PawnColor GetPawnColor(GameObject pawn)
    {
        return pawn.GetComponent<PawnProperties>().PawnColor;
    }

    private GameObject GetPawnFromTreeNode(TreeNode<Move> treeNode)
    {
        return tileGetter.GetTile(treeNode.Value.To).GetComponent<TileProperties>().GetPawn();
    }

    private void DoMove(Move move)
    {
        aiPawnMover.DoMove(move);
//        Debug.Log("Move from " + move.From.Column + ", " + move.From.Row + " to " + move.To.Column + ", " +
//                  move.To.Row);
        DecrementPawnsCountIfCapturing(move);
    }

    private void DecrementPawnsCountIfCapturing(Move move)
    {
        if (move.CapturedPawn == null) return;
        if (GetPawnColor(move.CapturedPawn) == PawnColor.White)
            --whitePawnsCount;
        else
            --blackPawnsCount;
    }

    private void AddPossibleMoves(TreeNode<Move> treeNode, int depth)
    {
        if (treeNode.Value.IsMoveMulticapturing)
            ContinueMulticapturingMove(treeNode, depth);
        else
            AddNewMove(treeNode, depth);
    }

    private void ContinueMulticapturingMove(TreeNode<Move> treeNode, int depth)
    {
        GameObject pawn = GetPawnFromTreeNode(treeNode);
        var capturingMoves = moveChecker.GetPawnCapturingMoves(pawn);
        TileIndex pawnTileIndex = pawn.GetComponent<PawnProperties>().GetTileIndex();
        foreach (var moveIndex in capturingMoves)
        {
            var move = new Move(pawnTileIndex, moveIndex);
            var moveTreeNode = treeNode.AddChild(move);
            AddMovesToTreeNode(moveTreeNode, depth - 1);
        }
    }

    private void AddNewMove(TreeNode<Move> treeNode, int depth)
    {
        foreach (var pawn in GetPawnsToCheck(treeNode))
        {
            if (!pawn.activeInHierarchy) continue;
            var moves = GetPawnMoves(pawn);
            TileIndex pawnTileIndex = pawn.GetComponent<PawnProperties>().GetTileIndex();
            foreach (var moveIndex in moves)
            {
                var move = new Move(pawnTileIndex, moveIndex);
                var moveTreeNode = treeNode.AddChild(move);
                AddMovesToTreeNode(moveTreeNode, depth - 1);
            }
        }
    }

    private LinkedList<GameObject> GetPawnsToCheck(TreeNode<Move> treeNode)
    {
        GameObject pawn = GetPawnFromTreeNode(treeNode);
        PawnColor pawnColor = GetPawnColor(pawn);
        return pawnColor == PawnColor.White ? blackPawns : whitePawns; //Opposite pawns should be checked.
    }

    private LinkedList<TileIndex> GetPawnMoves(GameObject pawn)
    {
        PawnColor pawnColorToCheck = GetPawnColor(pawn);
        bool pawnsHaveCapturingMove = moveChecker.PawnsHaveCapturingMove(pawnColorToCheck);
        return pawnsHaveCapturingMove
            ? moveChecker.GetPawnCapturingMoves(pawn)
            : moveChecker.GetPawnNoncapturingMoves(pawn);
    }

    private void AssignMoveScore(TreeNode<Move> treeNode)
    {
        if (treeNode.Children.Count == 0)
            treeNode.Value.Score = whitePawnsCount - blackPawnsCount;
        else
            AssignMoveScoreByPawnColor(treeNode);
//        Debug.Log("Assigned score for move from " + treeNode.Value.From.Column + ", " + treeNode.Value.From.Row +
//                  " to " + treeNode.Value.To.Column + ", " + treeNode.Value.To.Row + ": " + treeNode.Value.Score);
    }

    private void AssignMoveScoreByPawnColor(TreeNode<Move> treeNode)
    {
        PawnColor pawnColor = GetPawnColor(GetPawnFromTreeNode(treeNode));
        treeNode.Value.Score = pawnColor == PawnColor.White
            ? treeNode.Children.Min(move => move.Value.Score)
            : treeNode.Children.Max(move => move.Value.Score);
    }

    private void UndoMove(Move move)
    {
        aiPawnMover.UndoMove(move);
//        Debug.Log("Undo move from " + move.From.Column + ", " + move.From.Row + " to " + move.To.Column + ", " +
//                  move.To.Row);
        IncrementPawnsCountIfCapturing(move);
    }

    private void IncrementPawnsCountIfCapturing(Move move)
    {
        if (move.CapturedPawn == null) return;
        if (GetPawnColor(move.CapturedPawn) == PawnColor.White)
            ++whitePawnsCount;
        else
            ++blackPawnsCount;
    }

//    private void ChooseMoveInTree(Move move)
//    {
//        ChangeMoveTreeRoot(move);
//        FillTreeNode(moveTree, MoveTreeDepth);
//    }

//    private void ChangeMoveTreeRoot(Move move)
//    {
//        foreach (var moveNode in moveTree.Children)
//        {
//            if (!PositionChangeIsDifferent(move, moveNode.Value))
//            {
//                moveTree = moveNode;
//                break;
//            }
//        }
//    }

//    private bool PositionChangeIsDifferent(Move firstMove, Move secondMove)
//    {
//        return !(firstMove.From == secondMove.From && firstMove.To == secondMove.To);
//    }

//    private void FillTreeNode(TreeNode<Move> moveTreeNode, int depth)
//    {
//        if (depth == 1)
//            AddMovesToTreeNode(moveTreeNode, depth);
//        else
//            FillEachChildOfTreeNode(moveTreeNode, depth);
//    }

//    private void FillEachChildOfTreeNode(TreeNode<Move> moveTreeNode, int depth)
//    {
//        DoMove(moveTreeNode.Value);
//        foreach (var move in moveTreeNode.Children)
//            FillTreeNode(move, depth - 1);
//        AssignMoveScore(moveTreeNode);
//        UndoMove(moveTreeNode.Value);
//    }

    public bool HasNextMove()
    {
        return moveTree.Children.Count > 0;
    }

    public Move ChooseNextCPUMove()
    {
        Move move = ChooseOptimalCPUMove();
//        ChooseMoveInTree(move);
        CreateMoveTree(move);
        DoMove(move);
        return move;
    }

    private Move ChooseOptimalCPUMove()
    {
        Debug.Log("Possible number of moves from this position: " + moveTree.Children.Count);
        int minimalScore = moveTree.Children.Min(move => move.Value.Score);
        Debug.Log("Minimal score: " + minimalScore);
        var movesWithMinimalScore = moveTree.Children.Where(move => move.Value.Score == minimalScore).ToArray();
        Debug.Log("Moves with minimal score: " + movesWithMinimalScore.Length);
        int moveIndex = Random.Range(0, movesWithMinimalScore.Length - 1);
        return movesWithMinimalScore[moveIndex].Value;
    }
}
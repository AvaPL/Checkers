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
        CreateMoveTree(playerMove);
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

    private void DoMove(Move move)
    {
        aiPawnMover.DoMove(move);
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

    private PawnColor GetPawnColor(GameObject pawn)
    {
        return pawn.GetComponent<PawnProperties>().PawnColor;
    }

    private void AddPossibleMoves(TreeNode<Move> treeNode, int depth)
    {
        if (treeNode.Value.IsMulticapturing)
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

    private GameObject GetPawnFromTreeNode(TreeNode<Move> treeNode)
    {
        return tileGetter.GetTile(treeNode.Value.To).GetComponent<TileProperties>().GetPawn();
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
    }

    private void AssignMoveScoreByPawnColor(TreeNode<Move> treeNode)
    {
        treeNode.Value.Score = IsMoveByMaximizingPlayer(treeNode)
            ? treeNode.Children.Max(move => move.Value.Score)
            : treeNode.Children.Min(move => move.Value.Score);
    }

    private bool IsMoveByMaximizingPlayer(TreeNode<Move> treeNode)
    {
        /* White|Multicapturing|Value
         *   1  |      1       |  1
         *   1  |      0       |  0
         *   0  |      1       |  0
         *   0  |      0       |  1
         */
        PawnColor pawnColor = GetPawnColor(GetPawnFromTreeNode(treeNode));
        return (pawnColor == PawnColor.White) == treeNode.Value.IsMulticapturing; //XNOR
    }

    private void UndoMove(Move move)
    {
        aiPawnMover.UndoMove(move);
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

    public bool HasNextMove()
    {
        return moveTree.Children.Count > 0;
    }

    public Move ChooseNextCPUMove()
    {
        Move move = ChooseOptimalCPUMove();
        if (move.IsMulticapturing) //Tree will be created after white move if move is not multicapturing.
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
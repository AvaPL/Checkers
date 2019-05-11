using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveTreeBuilder : MonoBehaviour
{
    private MoveChecker moveChecker;
    private AIPawnMover aiPawnMover;
    private TileGetter tileGetter;
    private int moveTreeDepth = 3;
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
        if (PlayerPrefs.HasKey("Difficulty"))
            moveTreeDepth = PlayerPrefs.GetInt("Difficulty");
    }

    private void Start()
    {
        InitializePawns();
        CountPawns();
    }

    private void InitializePawns()
    {
        var pawnsProperties = GetComponentsInChildren<IPawnProperties>();
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
        AddMovesToTreeNode(moveTree, moveTreeDepth, int.MinValue, int.MaxValue);
    }

    private void AddMovesToTreeNode(TreeNode<Move> treeNode, int depth, int alpha, int beta)
    {
        DoMove(treeNode.Value);
        if (depth != 0)
            AddPossibleMoves(treeNode, depth, alpha, beta);
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
        return pawn.GetComponent<IPawnProperties>().PawnColor;
    }

    private void AddPossibleMoves(TreeNode<Move> treeNode, int depth, int alpha, int beta)
    {
        if (treeNode.Value.IsMulticapturing)
            ContinueMulticapturingMove(treeNode, depth, alpha, beta);
        else
            AddNewMove(treeNode, depth, alpha, beta);
    }

    private void ContinueMulticapturingMove(TreeNode<Move> treeNode, int depth, int alpha, int beta)
    {
        GameObject pawn = GetPawnFromTreeNode(treeNode);
        var capturingMoves = GetPawnCapturingMoves(pawn);
        TileIndex pawnTileIndex = pawn.GetComponent<IPawnProperties>().GetTileIndex();
        foreach (var moveIndex in capturingMoves)
        {
            var move = new Move(pawnTileIndex, moveIndex);
            var moveTreeNode = treeNode.AddChild(move);
            AddMovesToTreeNode(moveTreeNode, depth - 1, alpha, beta);
            SetAlphaAndBeta(IsMoveByMaximizingPlayer(treeNode), moveTreeNode.Value.Score, ref alpha, ref beta);
            if (beta <= alpha)
                return;
        }
    }

    private IEnumerable<TileIndex> GetPawnCapturingMoves(GameObject pawn)
    {
        var pawnCapturingMoves = moveChecker.GetPawnCapturingMoves(pawn);
        return pawnCapturingMoves.OrderBy(element => Random.value);
    }

    private GameObject GetPawnFromTreeNode(TreeNode<Move> treeNode)
    {
        return tileGetter.GetTile(treeNode.Value.To).GetComponent<TileProperties>().GetPawn();
    }

    private void SetAlphaAndBeta(bool isMoveByMaximizingPlayer, int score, ref int alpha, ref int beta)
    {
        if (isMoveByMaximizingPlayer)
            alpha = Mathf.Max(alpha, score);
        else
            beta = Mathf.Min(beta, score);
    }

    private void AddNewMove(TreeNode<Move> treeNode, int depth, int alpha, int beta)
    {
        foreach (var pawn in GetPawnsToCheck(treeNode))
        {
            if (!pawn.activeInHierarchy) continue;
            var moves = GetPawnMoves(pawn);
            TileIndex pawnTileIndex = pawn.GetComponent<IPawnProperties>().GetTileIndex();
            foreach (var moveIndex in moves)
            {
                var move = new Move(pawnTileIndex, moveIndex);
                var moveTreeNode = treeNode.AddChild(move);
                AddMovesToTreeNode(moveTreeNode, depth - 1, alpha, beta);
                SetAlphaAndBeta(IsMoveByMaximizingPlayer(treeNode), moveTreeNode.Value.Score, ref alpha, ref beta);
                if (beta <= alpha)
                    return;
            }
        }
    }

    private IEnumerable<GameObject> GetPawnsToCheck(TreeNode<Move> treeNode)
    {
        GameObject pawn = GetPawnFromTreeNode(treeNode);
        PawnColor pawnColor = GetPawnColor(pawn);
        var pawnsToCheck = pawnColor == PawnColor.White ? blackPawns : whitePawns; //Opposite pawns should be checked.
        return pawnsToCheck.OrderBy(element => Random.value);
    }

    private IEnumerable<TileIndex> GetPawnMoves(GameObject pawn)
    {
        PawnColor pawnColorToCheck = GetPawnColor(pawn);
        bool pawnsHaveCapturingMove = moveChecker.PawnsHaveCapturingMove(pawnColorToCheck);
        var pawnMoves = pawnsHaveCapturingMove
            ? moveChecker.GetPawnCapturingMoves(pawn)
            : moveChecker.GetPawnNoncapturingMoves(pawn);
        return pawnMoves.OrderBy(element => Random.value);
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
        int minimalScore = moveTree.Children.Min(move => move.Value.Score);
        var moveWithMinimalScore = moveTree.Children.First(move => move.Value.Score == minimalScore);
        return moveWithMinimalScore.Value;
    }
}
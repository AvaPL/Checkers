using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveTreeBuilder : MonoBehaviour
{
    public int MoveTreeDepth;

    private LinkedList<GameObject> whitePawns = new LinkedList<GameObject>();
    private LinkedList<GameObject> blackPawns = new LinkedList<GameObject>();
    private MoveChecker moveChecker;
    private AIPawnMover aiPawnMover;
    private TileGetter tileGetter;
    private TreeNode<Move> moveTree;

    private void Awake()
    {
        moveChecker = GetComponent<MoveChecker>();
        aiPawnMover = GetComponent<AIPawnMover>();
        tileGetter = GetComponent<TileGetter>();
    }

    private void Start()
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

    public void DoPlayerMove(Move playerMove)
    {
        if (moveTree == null)
            InitializeMoveTree(playerMove);
        else
            ChooseMoveInTree(playerMove);
        aiPawnMover.DoMove(playerMove);
    }

    private void ChooseMoveInTree(Move move)
    {
        ChangeMoveTreeRoot(move);
        FillMoveTree(moveTree, MoveTreeDepth);
    }

    private void InitializeMoveTree(Move initialMove)
    {
        moveTree = new TreeNode<Move>(initialMove);
        AddMovesToTreeNode(moveTree, MoveTreeDepth);
    }

    private void AddMovesToTreeNode(TreeNode<Move> treeNode, int depth)
    {
        if (depth == 0) return;

        Debug.Log("Move from " + treeNode.Value.From.Column + ", " + treeNode.Value.From.Row + " to " +
                  treeNode.Value.To.Column + ", " + treeNode.Value.To.Row);
        aiPawnMover.DoMove(treeNode.Value);

        if (treeNode.Value.IsMoveMulticapturing)
            ContinueMulticapturingMove(treeNode, depth);
        else
            AddNewMove(treeNode, depth);

        Debug.Log("Undo move from " + treeNode.Value.From.Column + ", " + treeNode.Value.From.Row + " to " +
                  treeNode.Value.To.Column + ", " + treeNode.Value.To.Row);
        aiPawnMover.UndoMove(treeNode.Value);
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
            Debug.Log("Added multicapturing move from " + pawnTileIndex.Column + ", " + pawnTileIndex.Row + " to " +
                      moveIndex.Column + ", " + moveIndex.Row +
                      " ------------------------------------------------------------------------------------------------------------------------------------------");
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
                Debug.Log("Added move from " + pawnTileIndex.Column + ", " + pawnTileIndex.Row + " to " +
                          moveIndex.Column + ", " + moveIndex.Row);
                AddMovesToTreeNode(moveTreeNode, depth - 1);
            }
        }
    }

    private LinkedList<GameObject> GetPawnsToCheck(TreeNode<Move> treeNode)
    {
        GameObject pawn = GetPawnFromTreeNode(treeNode);
        PawnColor pawnColor = pawn.GetComponent<PawnProperties>().PawnColor;
        return pawnColor == PawnColor.White ? blackPawns : whitePawns; //Opposite pawns should be checked.
    }

    private LinkedList<TileIndex> GetPawnMoves(GameObject pawn)
    {
        PawnColor pawnColorToCheck = pawn.GetComponent<PawnProperties>().PawnColor;
        bool pawnsHaveCapturingMove = moveChecker.PawnsHaveCapturingMove(pawnColorToCheck);
        Debug.Log(pawnColorToCheck + " pawns have capturing move: " + pawnsHaveCapturingMove);
        return pawnsHaveCapturingMove
            ? moveChecker.GetPawnCapturingMoves(pawn)
            : moveChecker.GetPawnNoncapturingMoves(pawn);
    }

    private void ChangeMoveTreeRoot(Move move)
    {
        foreach (var moveNode in moveTree.Children)
        {
            if (!PositionChangeIsDifferent(move, moveNode.Value))
            {
                moveTree = moveNode;
                break;
            }
        }
    }

    private bool PositionChangeIsDifferent(Move firstMove, Move secondMove)
    {
        return !(firstMove.From == secondMove.From && firstMove.To == secondMove.To);
    }

    private void FillMoveTree(TreeNode<Move> moveTreeNode, int depth)
    {
        if (depth == 1)
            AddMovesToTreeNode(moveTreeNode, depth);
        else
            foreach (var move in moveTreeNode.Children)
                FillMoveTree(moveTreeNode, depth - 1);
    }

    public Move ChooseNextCPUMove()
    {
        Move move = ChooseRandomCPUMove(); //TODO: Change choosing random move to minimax algorithm.
        ChooseMoveInTree(move);
        aiPawnMover.DoMove(move);
        return move;
    }

    private Move ChooseRandomCPUMove()
    {
        Debug.Log("Possible number of moves from this position: " + moveTree.Children.Count);
        int moveIndex = Random.Range(0, moveTree.Children.Count - 1);
        return moveTree.Children.ElementAt(moveIndex).Value;
    }
}
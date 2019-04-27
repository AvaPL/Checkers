using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public int MovesTreeDepth;

    private LinkedList<GameObject> whitePawns = new LinkedList<GameObject>();
    private LinkedList<GameObject> blackPawns = new LinkedList<GameObject>();
    private MoveChecker moveChecker;
    private AIPawnMover aiPawnMover;
    private TileGetter tileGetter;
    private TreeNode<Move> movesTree;

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

        StartCoroutine(TestMoves());
    }

    private IEnumerator TestMoves()
    {
        Debug.Log("Testing moves initialization...");
        yield return new WaitForSeconds(3);
        Debug.Log("Testing moves.");
        TestMovesTree();
    }

    private void TestMovesTree()
    {
        //TODO: Initialize with first white move.
        var initialMove = new Move(new TileIndex(1, 1), new TileIndex(0, 2));
        movesTree = new TreeNode<Move>(initialMove);
        AddMovesToTreeNode(movesTree, MovesTreeDepth, blackPawns);
    }

    private void AddMovesToTreeNode(TreeNode<Move> treeNode, int depth, LinkedList<GameObject> pawnsToCheck)
    {
        //TODO: Cleanup.
        if (depth == 0) return;

        aiPawnMover.DoMove(treeNode.Value);
        Debug.Log("Move from " + treeNode.Value.From.Column + ", " + treeNode.Value.From.Row + " to " +
                  treeNode.Value.To.Column + ", " + treeNode.Value.To.Row);

        PawnColor pawnColorToCheck = pawnsToCheck == whitePawns ? PawnColor.White : PawnColor.Black;

        if (treeNode.Value.IsMoveMulticapturing)
        {
            GameObject pawn = tileGetter.GetTile(treeNode.Value.To).GetComponent<TileProperties>().GetPawn();
            var capturingMoves = moveChecker.GetPawnCapturingMoves(pawn);
            TileIndex pawnTileIndex = pawn.GetComponent<PawnProperties>().GetTileIndex();
            foreach (var moveIndex in capturingMoves)
            {
                var move = new Move(pawnTileIndex, moveIndex);
                var moveTreeNode = movesTree.AddChild(move);
                Debug.Log("Added " + pawnColorToCheck + " multicapturing move from " + pawnTileIndex.Column + ", " +
                          pawnTileIndex.Row + " to " + moveIndex.Column + ", " + moveIndex.Row +
                          " ------------------------------------------------------------------------------------------------------------------------------------------");
                AddMovesToTreeNode(moveTreeNode, depth - 1, pawnsToCheck);
            }
        }
        else
        {
            bool pawnsHaveCapturingMove = moveChecker.PawnsHaveCapturingMove(pawnColorToCheck);
            Debug.Log(pawnColorToCheck + " pawns have capturing move: " + pawnsHaveCapturingMove);

            foreach (var pawn in pawnsToCheck)
            {
                if (!pawn.activeInHierarchy) continue;

                var moves = pawnsHaveCapturingMove
                    ? moveChecker.GetPawnCapturingMoves(pawn)
                    : moveChecker.GetPawnNoncapturingMoves(pawn);

                TileIndex pawnTileIndex = pawn.GetComponent<PawnProperties>().GetTileIndex();
                foreach (var moveIndex in moves)
                {
                    var move = new Move(pawnTileIndex, moveIndex);
                    var moveTreeNode = movesTree.AddChild(move);
                    Debug.Log("Added " + pawnColorToCheck + " move from " + pawnTileIndex.Column + ", " +
                              pawnTileIndex.Row +
                              " to " + moveIndex.Column + ", " + moveIndex.Row);
                    AddMovesToTreeNode(moveTreeNode, depth - 1, pawnsToCheck == whitePawns ? blackPawns : whitePawns);
                }
            }
        }

        Debug.Log("Undo move from " + treeNode.Value.From.Column + ", " + treeNode.Value.From.Row + " to " +
                  treeNode.Value.To.Column + ", " + treeNode.Value.To.Row);
        aiPawnMover.UndoMove(treeNode.Value);
    }
}
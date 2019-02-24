using UnityEngine;

public class CameraPivotSetter : MonoBehaviour
{
    public GameObject Board;

    private int boardSize;
    private float scale;

    private void Start()
    {
        var tilesGenerator = Board.GetComponent<TilesGenerator>();
        boardSize = tilesGenerator.BoardSize;
        scale = tilesGenerator.Scale;
        SetPivotInCenter();
    }

    private void SetPivotInCenter()
    {
        var centerValue = boardSize * scale / 2 - scale / 2;
        transform.position = new Vector3(centerValue, 0, centerValue);
    }
}
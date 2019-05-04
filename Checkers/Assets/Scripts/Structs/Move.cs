using UnityEngine;

public class Move
{
    public TileIndex From { get; }
    public TileIndex To { get; }
    public GameObject CapturedPawn { get; set; }
    public bool IsMulticapturing { get; set; }
    public bool WasPawnPromoted { get; set; }
    public int Score { get; set; }

    public Move(TileIndex from, TileIndex to)
    {
        From = from;
        To = to;
    }
}
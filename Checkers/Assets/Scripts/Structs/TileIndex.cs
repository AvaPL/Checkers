using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileIndex
{
    public int Column { get; }
    public int Row { get; }

    public TileIndex(int column, int row)
    {
        Column = column;
        Row = row;
    }

    public static TileIndex operator +(TileIndex firstTileIndex, TileIndex secondTileIndex)
    {
        return new TileIndex(firstTileIndex.Column + secondTileIndex.Column,
            firstTileIndex.Row + secondTileIndex.Row);
    }

    public static TileIndex operator -(TileIndex firstTileIndex, TileIndex secondTileIndex)
    {
        return new TileIndex(firstTileIndex.Column - secondTileIndex.Column,
            firstTileIndex.Row - secondTileIndex.Row);
    }
}
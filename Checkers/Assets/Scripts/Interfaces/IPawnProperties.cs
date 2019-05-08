using UnityEngine;

public interface IPawnProperties
{
    GameObject gameObject { get; }
    PawnColor PawnColor { get; set; }
    bool IsKing { get; set; }

    TileIndex GetTileIndex();

    void PromoteToKing();

    void AddPawnSelection();

    void RemovePawnSelection();
}
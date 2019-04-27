using UnityEngine;

public class PromotionChecker : MonoBehaviour
{
    private int boardSize;

    private void Start()
    {
        boardSize = GetComponent<TilesGenerator>().BoardSize;
    }

    public void CheckPromotion(GameObject pawnToCheck)
    {
        var pawnProperties = pawnToCheck.GetComponent<PawnProperties>();
        if (pawnProperties.IsKing)
            return;
        var tileIndex = pawnProperties.GetTileIndex();
        int promotionRow = GetPromotionRow(pawnProperties);
        if (tileIndex.Row == promotionRow)
            pawnProperties.PromoteToKing();
    }

    private int GetPromotionRow(PawnProperties pawnProperties)
    {
        return pawnProperties.PawnColor == PawnColor.White ? boardSize - 1 : 0;
    }
}
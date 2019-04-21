using UnityEngine;
using UnityEngine.UI;

public class PawnRowsLimiter : MonoBehaviour
{
    private Slider pawnRowsSlider;

    private void Awake()
    {
        pawnRowsSlider = GetComponent<Slider>();
    }

    public void LimitMaxPawnRows(Slider BoardSizesSlider)
    {
        int boardSize = Mathf.RoundToInt(BoardSizesSlider.value);
        pawnRowsSlider.maxValue = (boardSize - 1) / 2;
        if (pawnRowsSlider.value > pawnRowsSlider.maxValue)
            pawnRowsSlider.value = pawnRowsSlider.maxValue;
    }
}
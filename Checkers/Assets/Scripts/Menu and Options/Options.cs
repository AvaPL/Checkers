using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Slider VolumeSlider;
    public Slider BoardSizeSlider;
    public Slider PawnRowsSlider;

    private int volume = 100;
    private int boardSize = 8;
    private int pawnRows = 3;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Volume"))
            volume = PlayerPrefs.GetInt("Volume");
        if (PlayerPrefs.HasKey("BoardSize"))
            boardSize = PlayerPrefs.GetInt("BoardSize");
        if (PlayerPrefs.HasKey("PawnRows"))
            pawnRows = PlayerPrefs.GetInt("PawnRows");
    }

    private void Start()
    {
        VolumeSlider.value = volume;
        BoardSizeSlider.value = boardSize;
        PawnRowsSlider.value = pawnRows;
    }

    private void OnDisable()
    {
        LoadSliderValues();
        SavePlayerPrefs();
    }

    private void LoadSliderValues()
    {
        volume = Mathf.RoundToInt(VolumeSlider.value);
        boardSize = Mathf.RoundToInt(BoardSizeSlider.value);
        pawnRows = Mathf.RoundToInt(PawnRowsSlider.value);
    }

    private void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt("Volume", volume);
        PlayerPrefs.SetInt("BoardSize", boardSize);
        PlayerPrefs.SetInt("PawnRows", pawnRows);
    }
}
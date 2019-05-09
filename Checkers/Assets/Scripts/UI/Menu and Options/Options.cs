using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public Slider VolumeSlider;
    public Slider BoardSizeSlider;
    public Slider PawnRowsSlider;
    public Slider DifficultySlider;

    private int volume = 100;
    private int boardSize = 8;
    private int pawnRows = 3;
    private int difficulty = 3;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Volume"))
            volume = PlayerPrefs.GetInt("Volume");
        if (PlayerPrefs.HasKey("BoardSize"))
            boardSize = PlayerPrefs.GetInt("BoardSize");
        if (PlayerPrefs.HasKey("PawnRows"))
            pawnRows = PlayerPrefs.GetInt("PawnRows");
        if (PlayerPrefs.HasKey("Difficulty"))
            difficulty = PlayerPrefs.GetInt("Difficulty");
    }

    private void Start()
    {
        VolumeSlider.value = volume;
        BoardSizeSlider.value = boardSize;
        PawnRowsSlider.value = pawnRows;
        DifficultySlider.value = difficulty;
    }

    private void OnDisable()
    {
        LoadValues();
        SavePlayerPrefs();
    }

    private void LoadValues()
    {
        volume = Mathf.RoundToInt(VolumeSlider.value);
        boardSize = Mathf.RoundToInt(BoardSizeSlider.value);
        pawnRows = Mathf.RoundToInt(PawnRowsSlider.value);
        difficulty = Mathf.RoundToInt(DifficultySlider.value);
    }

    private void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt("Volume", volume);
        PlayerPrefs.SetInt("BoardSize", boardSize);
        PlayerPrefs.SetInt("PawnRows", pawnRows);
        PlayerPrefs.SetInt("Difficulty", difficulty);
    }
}
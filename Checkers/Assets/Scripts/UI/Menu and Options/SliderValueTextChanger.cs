using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueTextChanger : MonoBehaviour
{
    public TextMeshProUGUI ValueText;

    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void OnValueChanged()
    {
        ValueText.text = Mathf.RoundToInt(slider.value).ToString();
    }
}

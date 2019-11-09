using UnityEngine;
using UnityEngine.UI;

public class _fill_bar : MonoBehaviour
{

    // Unity UI References
    public Slider slider;
    public Text displayText;

    private float currentValue = 0f;
    public float CurrentValue
    {
        get
        {
            return currentValue;
        }
        set
        {
            currentValue = value;
            slider.value = currentValue;
            displayText.text = (slider.value * 100).ToString("0.00") + "%";
        }
    }

    void Start()
    {
        CurrentValue = 0f;
    }

    void Update()
    {
        
    }
}
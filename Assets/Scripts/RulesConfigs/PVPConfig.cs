using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PVPConfig : RulesConfig
{
    Rating rating;

    public Slider MinSlider;
    public TextMeshProUGUI MinValue;
    public Slider MaxSlider;
    public TextMeshProUGUI MaxValue;

    protected override void Start()
    {
        base.Start();
        const int min = 2;
        const int max = 10;
        rating = new Rating(min, max);

        MinSlider.minValue = min;
        MinSlider.maxValue = rank;
        MinSlider.value = min;
        MinValue.text = min.ToString();

        MaxSlider.minValue = rank;
        MaxSlider.maxValue = max;
        MaxSlider.value = max;
        MaxValue.text = max.ToString();
    }

    public void SetMinValue()
    {
        rating.min = Mathf.RoundToInt(MinSlider.value);
        MinValue.text = rating.min.ToString();
    }

    public void SetMaxValue()
    {
        rating.max = Mathf.RoundToInt(MaxSlider.value);
        MaxValue.text = rating.max.ToString();
    }

    public override void Play()
    {
        base.Play();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _fill;
    [SerializeField] private Color _barColor = Color.green;

    /// <summary>
    /// Sets the maximum HP value on the slider and initializes the fill color.
    /// </summary>
    public void SetMaxHP(int hp)
    {
        _slider.maxValue = hp;
        _slider.value = hp;
        _fill.color = _barColor;
    }

    /// <summary>
    /// Updates the current HP value on the slider.
    /// </summary>
    public void SetHP(int hp)
    {
        _slider.value = hp;
    }
}

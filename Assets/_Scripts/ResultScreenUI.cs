using UnityEngine;
using TMPro;

public class ResultScreenUI : MonoBehaviour
{
    [Header("Элементы")]
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TextMeshProUGUI _resultText;

    private void Start()
    {
        // Прячем панель в начале
        if (_resultPanel != null)
        {
            _resultPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Показывает экран победы.
    /// </summary>
    public void ShowVictory()
    {
        if (_resultPanel != null)
            _resultPanel.SetActive(true);

        if (_resultText != null)
        {
            _resultText.text = "ПОБЕДА!";
            _resultText.color = Color.yellow;
        }
    }

    /// <summary>
    /// Показывает экран поражения.
    /// </summary>
    public void ShowDefeat()
    {
        if (_resultPanel != null)
            _resultPanel.SetActive(true);

        if (_resultText != null)
        {
            _resultText.text = "ПОРАЖЕНИЕ";
            _resultText.color = Color.red;
        }
    }
}

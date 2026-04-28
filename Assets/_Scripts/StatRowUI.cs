using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Один ряд стата в панели прокачки.
/// Показывает: "Сила   5  [+]"
/// Кнопка [+] тратит 1 очко стата через SaveSystem.
/// </summary>
public class StatRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private Button          _plusButton;

    private string _statKey;
    private string _heroId;

    /// <summary>
    /// Заполняет строку данными.
    /// statKey: "strength" | "agility" | "intellect" | "endurance"
    /// </summary>
    public void Setup(string displayName, string statKey, int value, string heroId)
    {
        _statKey = statKey;
        _heroId  = heroId;

        if (_nameText  != null) _nameText.text  = displayName;
        if (_valueText != null) _valueText.text = value.ToString();

        if (_plusButton != null)
        {
            _plusButton.onClick.RemoveAllListeners();
            _plusButton.onClick.AddListener(OnPlusClicked);
        }
    }

    /// <summary>
    /// Блокирует/разблокирует кнопку [+].
    /// Вызывается когда нет свободных очков.
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (_plusButton != null) _plusButton.interactable = interactable;
    }

    private void OnPlusClicked()
    {
        bool spent = SaveSystem.Instance.SpendStatPoint(_heroId, _statKey);
        if (!spent) return;

        // Находим HeroEquipUIV2 выше по иерархии и просим обновиться
        HeroEquipUIV2 ui = GetComponentInParent<HeroEquipUIV2>(true);
        if (ui != null) ui.RefreshAfterStatSpend();

        Debug.Log($"[StatRowUI] +1 {_statKey} для {_heroId}", this);
    }
}

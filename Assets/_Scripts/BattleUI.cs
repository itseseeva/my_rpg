using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    [Header("Кнопки способностей")]
    [SerializeField] private Button[] _abilityButtons = new Button[4];
    [SerializeField] private TextMeshProUGUI[] _abilityButtonTexts = new TextMeshProUGUI[4];

    [Header("Менеджеры")]
    [SerializeField] private BattleManager _battleManager;
    [SerializeField] private TurnOrderManager _turnManager;

    private void Start()
    {
        // Подписываем кнопки
        for (int i = 0; i < _abilityButtons.Length; i++)
        {
            int index = i; // важно для замыкания!
            _abilityButtons[i].onClick.AddListener(() => 
                OnAbilityButtonPressed(index));
        }

        // Задержка чтобы все юниты успели загрузиться
        Invoke(nameof(RefreshUI), 0.1f);
    }

    /// <summary>
    /// Обновляет состояние UI в зависимости от текущего хода.
    /// </summary>
    public void RefreshUI()
    {
        UnitController current = _turnManager.GetCurrentUnit();
        if (current == null) return;
        
        bool isHeroTurn = _turnManager.IsHeroTurn();

        for (int i = 0; i < _abilityButtons.Length; i++)
        {
            AbilityEffect ability = current.GetAbility(i);

            if (!isHeroTurn || ability == null)
            {
                _abilityButtons[i].interactable = false;
                _abilityButtonTexts[i].text = ability != null ? ability.AbilityName : "—";
            }
            else
            {
                _abilityButtons[i].interactable = true;
                _abilityButtonTexts[i].text = ability.AbilityName;
            }
        }
    }

    private void OnAbilityButtonPressed(int index)
    {
        UnitController current = _turnManager.GetCurrentUnit();
        if (current == null) return;

        current.SelectAbility(index);
        _battleManager.OnAttackButtonPressed();
        RefreshUI();
    }
}

using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Менеджер очереди")]
    [SerializeField] private TurnOrderManager _turnManager;

    [Header("UI результата")]
    [SerializeField] private ResultScreenUI _resultScreen;

    private bool _battleOver = false;

    private void Start()
    {
        Debug.Log("[BattleManager] === БОЙ НАЧАЛСЯ ===", this);
        HighlightCurrentUnit();
    }

    /// <summary>
    /// Вызывается при нажатии кнопки атаки.
    /// </summary>
    public void OnAttackButtonPressed()
    {
        if (_battleOver) return;

        UnitController current = _turnManager.GetCurrentUnit();
        if (current == null) return;

        // Тикаем эффекты текущего юнита (героя) перед атакой
        current.GetComponent<EffectSystem>()?.ProcessEffects();

        // Если ход героя — атакуем врага
        if (_turnManager.IsHeroTurn())
        {
            UnitController target = TargetingSystem.Instance.GetSelectedTarget();

            // Если никто не выбран — берём первого врага
            if (target == null)
                target = _turnManager.GetFirstAliveEnemy();

            if (target == null)
            {
                EndBattle(heroWon: true);
                return;
            }

            current.UseAbility(current.SelectedAbilityIndex, target);
            TargetingSystem.Instance.ClearTarget();
            // Debug.Log($"[BattleManager] {current.UnitName} атакует {target.UnitName}!", this); // Этот лог можно закомментировать, т.к. способности логируют себя сами.

            if (target.CurrentHP <= 0)
            {
                if (_turnManager.GetFirstAliveEnemy() == null)
                {
                    EndBattle(heroWon: true);
                    return;
                }
            }

            _turnManager.NextTurn();
            HighlightCurrentUnit();
            EnemyTurn();
        }
    }

    private void EnemyTurn()
    {
        UnitController current = _turnManager.GetCurrentUnit();
        if (current == null) return;

        // Тикаем эффекты текущего юнита (врага) перед атакой
        current.GetComponent<EffectSystem>()?.ProcessEffects();

        if (!_turnManager.IsHeroTurn())
        {
            UnitController target = _turnManager.GetFirstAliveHero();
            if (target == null)
            {
                EndBattle(heroWon: false);
                return;
            }

            // Подсвечиваем врага
            current.SetHighlight(true);
            
            // Запускаем с задержкой
            StartCoroutine(EnemyAttackDelay(current, target));
        }
    }

    private System.Collections.IEnumerator EnemyAttackDelay(UnitController current, UnitController target)
    {
        // Ждём 0.8 секунды — видно подсветку врага
        yield return new WaitForSeconds(0.8f);

        current.UseAbility(current.SelectedAbilityIndex, target);

        if (_turnManager.GetFirstAliveHero() == null)
        {
            EndBattle(heroWon: false);
            yield break;
        }

        _turnManager.NextTurn();
        HighlightCurrentUnit();
        Object.FindAnyObjectByType<BattleUI>()?.RefreshUI();
    }

    private void EndBattle(bool heroWon)
    {
        _battleOver = true;

        // Убираем кнопки способностей
        Object.FindAnyObjectByType<BattleUI>()?.gameObject.SetActive(false);

        if (heroWon)
        {
            Debug.Log("[BattleManager] === ПОБЕДА! ===", this);
            if (_resultScreen != null) _resultScreen.ShowVictory();
        }
        else
        {
            Debug.Log("[BattleManager] === ПОРАЖЕНИЕ ===", this);
            if (_resultScreen != null) _resultScreen.ShowDefeat();
        }
    }

    private void HighlightCurrentUnit()
    {
        // Сбрасываем подсветку всех
        foreach (var unit in _turnManager.HeroTeam)
            unit.SetHighlight(false);
        foreach (var unit in _turnManager.EnemyTeam)
            unit.SetHighlight(false);

        // Подсвечиваем текущего
        UnitController current = _turnManager.GetCurrentUnit();
        if (current != null)
            current.SetHighlight(true);
    }
}

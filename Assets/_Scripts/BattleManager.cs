using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Менеджер очереди")]
    [SerializeField] private TurnOrderManager _turnManager;

    private bool _battleOver = false;

    private void Start()
    {
        Debug.Log("[BattleManager] === БОЙ НАЧАЛСЯ ===", this);
    }

    /// <summary>
    /// Вызывается при нажатии кнопки атаки.
    /// </summary>
    public void OnAttackButtonPressed()
    {
        if (_battleOver) return;

        UnitController current = _turnManager.GetCurrentUnit();
        if (current == null) return;

        // Если ход героя — атакуем врага
        if (_turnManager.IsHeroTurn())
        {
            UnitController target = _turnManager.GetFirstAliveEnemy();
            if (target == null)
            {
                EndBattle(heroWon: true);
                return;
            }

            current.UseAbility(current.SelectedAbilityIndex, target);
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
            EnemyTurn();
        }
    }

    private void EnemyTurn()
    {
        UnitController current = _turnManager.GetCurrentUnit();
        if (current == null) return;

        if (!_turnManager.IsHeroTurn())
        {
            UnitController target = _turnManager.GetFirstAliveHero();
            if (target == null)
            {
                EndBattle(heroWon: false);
                return;
            }

            current.UseAbility(current.SelectedAbilityIndex, target);
            // Debug.Log($"[BattleManager] {current.UnitName} атакует {target.UnitName}!", this);

            if (_turnManager.GetFirstAliveHero() == null)
            {
                EndBattle(heroWon: false);
                return;
            }

            _turnManager.NextTurn();
        }
    }

    private void EndBattle(bool heroWon)
    {
        _battleOver = true;
        if (heroWon)
            Debug.Log("[BattleManager] === ПОБЕДА! ===", this);
        else
            Debug.Log("[BattleManager] === ПОРАЖЕНИЕ ===", this);
    }
}

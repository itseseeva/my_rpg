using UnityEngine;
using System.Collections.Generic;

public class TurnOrderManager : MonoBehaviour
{
    [Header("Команды")]
    [SerializeField] private List<UnitController> _heroTeam = new List<UnitController>();
    [SerializeField] private List<UnitController> _enemyTeam = new List<UnitController>();

    private List<UnitController> _turnQueue = new List<UnitController>();
    private int _currentTurnIndex = 0;

    private void Start()
    {
        BuildQueue();
    }

    private void BuildQueue()
    {
        _turnQueue.Clear();
        _currentTurnIndex = 0;

        // Чередуем: герой → враг → герой → враг...
        int maxCount = Mathf.Max(_heroTeam.Count, _enemyTeam.Count);
        for (int i = 0; i < maxCount; i++)
        {
            if (i < _heroTeam.Count) _turnQueue.Add(_heroTeam[i]);
            if (i < _enemyTeam.Count) _turnQueue.Add(_enemyTeam[i]);
        }

        Debug.Log("[TurnOrderManager] === ОЧЕРЕДЬ ХОДОВ ===", this);
        foreach (var unit in _turnQueue)
        {
            Debug.Log($"[TurnOrderManager] → {unit.UnitName}", this);
        }
    }

    /// <summary>
    /// Возвращает текущего ходящего юнита, пропуская мёртвых.
    /// </summary>
    public UnitController GetCurrentUnit()
    {
        // Пропускаем мёртвых
        while (_currentTurnIndex < _turnQueue.Count &&
               !_turnQueue[_currentTurnIndex].gameObject.activeSelf)
        {
            _currentTurnIndex++;
        }

        if (_currentTurnIndex >= _turnQueue.Count)
            return null;

        return _turnQueue[_currentTurnIndex];
    }

    /// <summary>
    /// Передает ход следующему юниту.
    /// </summary>
    public void NextTurn()
    {
        _currentTurnIndex++;

        // Если дошли до конца — начинаем новый раунд
        if (_currentTurnIndex >= _turnQueue.Count)
        {
            _currentTurnIndex = 0;
            Debug.Log("[TurnOrderManager] === НОВЫЙ РАУНД ===", this);
        }

        // Пропускаем мёртвых
        while (!_turnQueue[_currentTurnIndex].gameObject.activeSelf)
        {
            _currentTurnIndex++;
            if (_currentTurnIndex >= _turnQueue.Count)
            {
                _currentTurnIndex = 0;
                break;
            }
        }

        Debug.Log($"[TurnOrderManager] Ход: {_turnQueue[_currentTurnIndex].UnitName}", this);
    }

    public bool IsHeroTurn()
    {
        UnitController current = GetCurrentUnit();
        return _heroTeam.Contains(current);
    }

    public UnitController GetFirstAliveEnemy()
    {
        return _enemyTeam.Find(e => e.gameObject.activeSelf);
    }

    public UnitController GetFirstAliveHero()
    {
        return _heroTeam.Find(h => h.gameObject.activeSelf);
    }
}

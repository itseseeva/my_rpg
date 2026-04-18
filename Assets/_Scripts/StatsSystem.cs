using UnityEngine;
using System.Collections.Generic;

public class StatsSystem : MonoBehaviour
{
    // Базовые статы берём с UnitController
    private UnitController _unit;

    // Список временных бонусов (баффы, артефакты)
    private Dictionary<string, int> _attackBonuses = new Dictionary<string, int>();
    private Dictionary<string, int> _defenseBonuses = new Dictionary<string, int>();
    private Dictionary<string, int> _hpBonuses = new Dictionary<string, int>();

    private void Awake()
    {
        _unit = GetComponent<UnitController>();
    }

    // Добавить бонус к атаке (например от артефакта)
    public void AddAttackBonus(string source, int value)
    {
        _attackBonuses[source] = value;
        Debug.Log($"[StatsSystem] {_unit.UnitName}: +{value} атаки от {source}", this);
    }

    // Убрать бонус (когда бафф закончился)
    public void RemoveAttackBonus(string source)
    {
        if (_attackBonuses.ContainsKey(source))
            _attackBonuses.Remove(source);
    }

    // Добавить бонус к защите
    public void AddDefenseBonus(string source, int value)
    {
        _defenseBonuses[source] = value;
        Debug.Log($"[StatsSystem] {_unit.UnitName}: +{value} защиты от {source}", this);
    }

    public void RemoveDefenseBonus(string source)
    {
        if (_defenseBonuses.ContainsKey(source))
            _defenseBonuses.Remove(source);
    }

    // Получить итоговую атаку
    public int GetFinalAttack()
    {
        int total = _unit.AttackPower;
        foreach (var bonus in _attackBonuses.Values)
            total += bonus;
        return total;
    }

    // Получить итоговую защиту
    public int GetFinalDefense()
    {
        int total = _unit.Defense;
        foreach (var bonus in _defenseBonuses.Values)
            total += bonus;
        return total;
    }

    // Получить итоговый максимальный HP
    public int GetFinalMaxHP()
    {
        int total = _unit.MaxHP;
        foreach (var bonus in _hpBonuses.Values)
            total += bonus;
        return total;
    }
}

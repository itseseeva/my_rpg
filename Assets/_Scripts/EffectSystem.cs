using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Компонент для хранения и обработки эффектов (баффов/дебаффов).
/// Должен висеть на каждом юните.
/// </summary>
public class EffectSystem : MonoBehaviour
{
    private UnitController _unit;
    private List<BaseEffect> _activeEffects = new();

    private void Awake()
    {
        _unit = GetComponent<UnitController>();
    }

    // Добавить эффект на юнита
    public void AddEffect(BaseEffect effect)
    {
        _activeEffects.Add(effect);
        effect.OnApply(_unit);
    }

    // Вызывается в начале каждого хода этого юнита
    public void ProcessEffects()
    {
        List<BaseEffect> expired = new();

        foreach (var effect in _activeEffects)
        {
            effect.OnTick(_unit);

            if (effect.Duration <= 0)
                expired.Add(effect);
        }

        // Убираем истёкшие эффекты
        foreach (var effect in expired)
        {
            effect.OnExpire(_unit);
            _activeEffects.Remove(effect);
        }
    }

    public bool HasEffects() => _activeEffects.Count > 0;
}

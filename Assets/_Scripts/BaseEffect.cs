using UnityEngine;

/// <summary>
/// Базовый класс для всех эффектов (баффов, дебаффов, периодического урона).
/// </summary>
public abstract class BaseEffect
{
    public string EffectName;
    public int Duration; // сколько ходов осталось
    
    // Вызывается когда эффект накладывается
    public virtual void OnApply(UnitController unit)
    {
        Debug.Log($"[Effect] {unit.UnitName} получил эффект: {EffectName}");
    }
    
    // Вызывается каждый ход
    public abstract void OnTick(UnitController unit);
    
    // Вызывается когда эффект заканчивается
    public virtual void OnExpire(UnitController unit)
    {
        Debug.Log($"[Effect] {unit.UnitName}: эффект {EffectName} закончился");
    }
}

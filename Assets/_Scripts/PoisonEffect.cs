using UnityEngine;

/// <summary>
/// Эффект периодического урона (Яд).
/// </summary>
public class PoisonEffect : BaseEffect
{
    public int DamagePerTurn;

    public PoisonEffect(int damage, int duration)
    {
        EffectName = "Яд";
        DamagePerTurn = damage;
        Duration = duration;
    }

    public override void OnTick(UnitController unit)
    {
        unit.TakeDamage(DamagePerTurn);
        Debug.Log($"[Effect] {unit.UnitName} получает {DamagePerTurn} урона от яда!");
        Duration--;
    }
}

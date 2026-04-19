using UnityEngine;

/// <summary>
/// Эффект увеличения атаки на определённое количество ходов.
/// </summary>
public class AttackBuffEffect : BaseEffect
{
    public int BonusAttack;

    public AttackBuffEffect(int bonus, int duration)
    {
        EffectName = "Бафф атаки";
        BonusAttack = bonus;
        Duration = duration;
    }

    public override void OnApply(UnitController unit)
    {
        unit.GetComponent<StatsSystem>()?.AddAttackBonus(EffectName, BonusAttack);
        Debug.Log($"[Effect] {unit.UnitName}: +{BonusAttack} к атаке на {Duration} ходов!");
    }

    public override void OnTick(UnitController unit)
    {
        Duration--;
    }

    public override void OnExpire(UnitController unit)
    {
        unit.GetComponent<StatsSystem>()?.RemoveAttackBonus(EffectName);
        Debug.Log($"[Effect] {unit.UnitName}: бафф атаки закончился");
    }
}

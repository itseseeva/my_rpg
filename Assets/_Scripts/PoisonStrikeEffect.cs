using UnityEngine;

[CreateAssetMenu(fileName = "PoisonStrike", menuName = "RPG/Abilities/Poison Strike")]
public class PoisonStrikeEffect : AbilityEffect
{
    [Header("Урон")]
    [SerializeField] private float _damageMultiplier = 0.8f;

    [Header("Яд")]
    [SerializeField] private int _poisonDamagePerTurn = 10;
    [SerializeField] private int _poisonDuration = 3;

    public override void Execute(UnitController caster, UnitController target)
    {
        StatsSystem casterStats = caster.GetComponent<StatsSystem>();
        StatsSystem targetStats = target.GetComponent<StatsSystem>();

        int attack = casterStats != null ? 
            casterStats.GetFinalAttack() : caster.AttackPower;
        int defense = targetStats != null ? 
            targetStats.GetFinalDefense() : target.Defense;

        // Наносим урон
        int damage = Mathf.Max(1, 
            Mathf.RoundToInt(attack * _damageMultiplier) - defense);
        target.TakeDamage(damage);

        // Накладываем яд
        target.GetComponent<EffectSystem>()?.AddEffect(
            new PoisonEffect(_poisonDamagePerTurn, _poisonDuration));

        Debug.Log($"[Ability] {caster.UnitName} использует {AbilityName} → {damage} урона + яд!", caster);
    }
}

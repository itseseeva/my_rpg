using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "RPG/Abilities/Melee Attack")]
public class MeleeAttackEffect : AbilityEffect
{
    [SerializeField] private float _damageMultiplier = 1.0f;

    public override void Execute(UnitController caster, UnitController target)
    {
        StatsSystem casterStats = caster.GetComponent<StatsSystem>();
        StatsSystem targetStats = target.GetComponent<StatsSystem>();

        int attack = casterStats != null ? casterStats.GetFinalAttack() : caster.AttackPower;
        int defense = targetStats != null ? targetStats.GetFinalDefense() : target.Defense;

        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(attack * _damageMultiplier) - defense);
        target.TakeDamage(finalDamage);

        Debug.Log($"[Ability] {caster.UnitName} использует {AbilityName} → {finalDamage} урона!", caster);
    }
}

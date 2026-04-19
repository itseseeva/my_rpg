using UnityEngine;

[CreateAssetMenu(fileName = "WarCry", menuName = "RPG/Abilities/War Cry")]
public class WarCryEffect : AbilityEffect
{
    [Header("Бафф")]
    [SerializeField] private int _attackBonus = 15;
    [SerializeField] private int _duration = 3;

    public override void Execute(UnitController caster, UnitController target)
    {
        // Баффаем самого кастера
        caster.GetComponent<EffectSystem>()?.AddEffect(
            new AttackBuffEffect(_attackBonus, _duration));

        Debug.Log($"[Ability] {caster.UnitName} использует {AbilityName} → +{_attackBonus} атаки на {_duration} ходов!", caster);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "RPG/Abilities/Heal")]
public class HealEffect : AbilityEffect
{
    [SerializeField] private int _healAmount = 30;

    public override void Execute(UnitController caster, UnitController target)
    {
        int healed = Mathf.Min(_healAmount, target.MaxHP - target.CurrentHP);
        
        // Вызываем новый метод Heal в UnitController вместо прямого доступа к свойству
        target.Heal(healed);

        Debug.Log($"[Ability] {caster.UnitName} лечит {target.UnitName} на {healed} HP. " +
                  $"HP: {target.CurrentHP}/{target.MaxHP}", caster);
    }
}

using UnityEngine;

public abstract class AbilityEffect : ScriptableObject
{
    [Header("Описание")]
    [SerializeField] private string _abilityName = "Способность";
    [SerializeField] private string _description = "Описание способности";

    public string AbilityName => _abilityName;
    public string Description => _description;

    /// <summary>
    /// Выполняет эффект способности. Переопределяется конкретными скиллами.
    /// </summary>
    public abstract void Execute(UnitController caster, UnitController target);
}

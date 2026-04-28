using UnityEngine;

public abstract class AbilityEffect : ScriptableObject
{
    [Header("Описание")]
    [SerializeField] private string _abilityName = "Способность";
    [SerializeField] private string _description = "Описание способности";
    [SerializeField] private Sprite _icon;

    public string AbilityName => _abilityName;
    public string Description => _description;
    public Sprite Icon => _icon;

    /// <summary>
    /// Выполняет эффект способности. Переопределяется конкретными скиллами.
    /// </summary>
    public abstract void Execute(UnitController caster, UnitController target);
}

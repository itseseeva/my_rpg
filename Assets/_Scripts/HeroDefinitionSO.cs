using UnityEngine;

[CreateAssetMenu(fileName = "NewHero", menuName = "RPG/Hero Definition")]
public class HeroDefinitionSO : ScriptableObject
{
    [Header("Основное")]
    [SerializeField] private string _heroName = "Новый герой";
    [SerializeField] private int _maxHP      = 100;
    [SerializeField] private int _attack     = 20;
    [SerializeField] private int _defense    = 5;
    [SerializeField] private int _baseSpeed  = 100;
    [SerializeField] private int _baseMana   = 50;

    [Header("Способности")]
    [SerializeField] private AbilityEffect[] _abilities;

    public string HeroName  => _heroName;
    public int MaxHP         => _maxHP;
    public int Attack        => _attack;
    public int Defense       => _defense;
    public int BaseSpeed     => _baseSpeed;
    public int BaseMana      => _baseMana;
    public AbilityEffect[] Abilities => _abilities;
}

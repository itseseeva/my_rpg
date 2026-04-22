using UnityEngine;
using System.Collections.Generic;

public class LoadoutResolver : MonoBehaviour
{
    public static LoadoutResolver Instance;

    [Header("Составы команд")]
    public List<HeroLoadout> heroLoadouts = new List<HeroLoadout>();
    public List<HeroLoadout> enemyLoadouts = new List<HeroLoadout>();

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Применяет артефакты героя к его StatsSystem.
    /// Вызывается перед боем для каждого юнита.
    /// </summary>
    public void ApplyLoadout(UnitController unit, HeroLoadout loadout)
    {
        if (loadout == null) return;

        StatsSystem stats = unit.GetComponent<StatsSystem>();
        if (stats == null) return;

        // Применяем бонусы артефактов
        int attackBonus = loadout.GetTotalAttackBonus();
        int defenseBonus = loadout.GetTotalDefenseBonus();
        int hpBonus = loadout.GetTotalHPBonus();

        if (attackBonus > 0)
            stats.AddAttackBonus("Artifacts", attackBonus);

        if (defenseBonus > 0)
            stats.AddDefenseBonus("Artifacts", defenseBonus);

        Debug.Log($"[Loadout] {unit.UnitName} — " +
                  $"атака +{attackBonus}, " +
                  $"защита +{defenseBonus}, " +
                  $"HP +{hpBonus}", unit);
    }

    /// <summary>
    /// Применяет все загрузки для команды героев.
    /// </summary>
    public void ApplyAllLoadouts(
        List<UnitController> heroes, 
        List<UnitController> enemies)
    {
        for (int i = 0; i < heroes.Count; i++)
        {
            if (i < heroLoadouts.Count)
                ApplyLoadout(heroes[i], heroLoadouts[i]);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (i < enemyLoadouts.Count)
                ApplyLoadout(enemies[i], enemyLoadouts[i]);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class HeroLoadout
{
    public HeroDefinitionSO heroDefinition;
    public List<ArtifactDefinitionSO> equippedArtifacts 
        = new List<ArtifactDefinitionSO>();

    /// <summary>
    /// Считает суммарный бонус атаки от всех артефактов.
    /// </summary>
    public int GetTotalAttackBonus()
    {
        int total = 0;
        foreach (var artifact in equippedArtifacts)
            if (artifact != null)
                total += artifact.bonusAttack;
        return total;
    }

    /// <summary>
    /// Считает суммарный бонус защиты от всех артефактов.
    /// </summary>
    public int GetTotalDefenseBonus()
    {
        int total = 0;
        foreach (var artifact in equippedArtifacts)
            if (artifact != null)
                total += artifact.bonusDefense;
        return total;
    }

    /// <summary>
    /// Считает суммарный бонус HP от всех артефактов.
    /// </summary>
    public int GetTotalHPBonus()
    {
        int total = 0;
        foreach (var artifact in equippedArtifacts)
            if (artifact != null)
                total += artifact.bonusHP;
        return total;
    }
}

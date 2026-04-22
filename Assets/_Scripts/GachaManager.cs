using UnityEngine;
using System.Collections.Generic;

public class GachaManager : MonoBehaviour
{
    public static GachaManager Instance;

    [Header("Стоимость крутки")]
    public int goldCostPerRoll = 100;
    public int crystalCostPerRoll = 10;

    [Header("UI")]
    public CurrencyUI currencyUI;

    [Header("Пул артефактов")]
    public List<ArtifactDefinitionSO> commonPool = new List<ArtifactDefinitionSO>();
    public List<ArtifactDefinitionSO> rarePool = new List<ArtifactDefinitionSO>();
    public List<ArtifactDefinitionSO> epicPool = new List<ArtifactDefinitionSO>();
    public List<ArtifactDefinitionSO> legendaryPool = new List<ArtifactDefinitionSO>();

    [Header("Шансы (должны давать 100)")]
    public float commonChance = 60f;
    public float rareChance = 25f;
    public float epicChance = 12f;
    public float legendaryChance = 3f;

    // Инвентарь игрока — полученные артефакты
    private List<ArtifactDefinitionSO> _playerInventory 
        = new List<ArtifactDefinitionSO>();

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Крутит гачу и возвращает случайный артефакт.
    /// </summary>
    public ArtifactDefinitionSO Roll()
    {
        // Бросаем кубик 0-100
        float roll = Random.Range(0f, 100f);

        // Определяем редкость
        ArtifactRarity rarity;
        if (roll < legendaryChance)
            rarity = ArtifactRarity.Legendary;
        else if (roll < legendaryChance + epicChance)
            rarity = ArtifactRarity.Epic;
        else if (roll < legendaryChance + epicChance + rareChance)
            rarity = ArtifactRarity.Rare;
        else
            rarity = ArtifactRarity.Common;

        // Берём случайный артефакт нужной редкости
        ArtifactDefinitionSO result = GetRandomFromPool(rarity);

        if (result != null)
        {
            _playerInventory.Add(result);
            
            // Сохраняем артефакт
            SaveSystem.Instance?.AddArtifactToInventory(result.artifactName);
            
            Debug.Log($"[Gacha] Выпал {rarity}: {result.artifactName}!");
        }
        else
        {
            Debug.Log($"[Gacha] Пул {rarity} пуст!");
        }

        return result;
    }

    /// <summary>
    /// Возвращает случайный артефакт из пула нужной редкости.
    /// </summary>
    private ArtifactDefinitionSO GetRandomFromPool(ArtifactRarity rarity)
    {
        List<ArtifactDefinitionSO> pool = rarity switch
        {
            ArtifactRarity.Common    => commonPool,
            ArtifactRarity.Rare      => rarePool,
            ArtifactRarity.Epic      => epicPool,
            ArtifactRarity.Legendary => legendaryPool,
            _ => commonPool
        };

        if (pool.Count == 0) return null;
        return pool[Random.Range(0, pool.Count)];
    }

    /// <summary>
    /// Возвращает инвентарь игрока.
    /// </summary>
    public List<ArtifactDefinitionSO> GetInventory()
    {
        return _playerInventory;
    }

    /// <summary>
    /// Крутка за золото.
    /// </summary>
    public void OnRollGoldPressed()
    {
        if (!SaveSystem.Instance.SpendGold(goldCostPerRoll))
            return;

        ArtifactDefinitionSO result = Roll();
        if (result != null)
        {
            SaveSystem.Instance.AddArtifactToInventory(result.artifactName);
            currencyUI?.RefreshUI();
            Debug.Log($"[Gacha] Инвентарь: {_playerInventory.Count} артефактов");
        }
    }

    /// <summary>
    /// Крутка за кристаллы.
    /// </summary>
    public void OnRollCrystalsPressed()
    {
        if (!SaveSystem.Instance.SpendCrystals(crystalCostPerRoll))
            return;

        ArtifactDefinitionSO result = Roll();
        if (result != null)
        {
            SaveSystem.Instance.AddArtifactToInventory(result.artifactName);
            currencyUI?.RefreshUI();
            Debug.Log($"[Gacha] Инвентарь: {_playerInventory.Count} артефактов");
        }
    }
}

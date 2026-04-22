using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("Панель инвентаря")]
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Transform _itemsContainer;
    [SerializeField] private GameObject _itemPrefab;

    [Header("База артефактов")]
    [SerializeField] private List<ArtifactDefinitionSO> _allArtifacts;

    void Start()
    {
        _inventoryPanel.SetActive(false);
    }

    /// <summary>
    /// Открывает инвентарь и заполняет его артефактами игрока.
    /// </summary>
    public void OpenInventory()
    {
        _inventoryPanel.SetActive(true);
        RefreshInventory();
    }

    /// <summary>
    /// Закрывает инвентарь.
    /// </summary>
    public void CloseInventory()
    {
        _inventoryPanel.SetActive(false);
    }

    /// <summary>
    /// Очищает и заново заполняет список артефактов.
    /// </summary>
    private void RefreshInventory()
    {
        // Удаляем старые элементы
        foreach (Transform child in _itemsContainer)
            Destroy(child.gameObject);

        // Получаем ID артефактов игрока
        var save = SaveSystem.Instance.GetSaveData();
        var ownedIds = save.ownedArtifactIds;

        Debug.Log($"[Inventory] Показываем {ownedIds.Count} артефактов");

        // Создаём кнопку для каждого артефакта
        foreach (string id in ownedIds)
        {
            ArtifactDefinitionSO artifact = FindArtifactById(id);
            if (artifact == null) continue;

            CreateItemButton(artifact);
        }
    }

    /// <summary>
    /// Ищет артефакт в базе по имени.
    /// </summary>
    private ArtifactDefinitionSO FindArtifactById(string id)
    {
        return _allArtifacts.Find(a => a.artifactName == id);
    }

    /// <summary>
    /// Создаёт кнопку артефакта в списке.
    /// </summary>
    private void CreateItemButton(ArtifactDefinitionSO artifact)
    {
        GameObject itemGO = Instantiate(_itemPrefab, _itemsContainer);
        TextMeshProUGUI text = itemGO.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = $"{artifact.artifactName}\n" +
                       $"⚔️+{artifact.bonusAttack}  " +
                       $"🛡️+{artifact.bonusDefense}  " +
                       $"❤️+{artifact.bonusHP}";
        }
    }
}

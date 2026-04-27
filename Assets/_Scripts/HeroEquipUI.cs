using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HeroEquipUI : MonoBehaviour
{
    [Header("Главная панель")]
    [SerializeField] private GameObject _screen;

    [Header("Левая панель — герой")]
    [SerializeField] private TextMeshProUGUI _heroNameText;
    [SerializeField] private Transform _slotsGrid;
    [SerializeField] private Button _prevHeroButton;
    [SerializeField] private Button _nextHeroButton;

    [Header("Правая панель — инвентарь")]
    [SerializeField] private Transform _inventoryGrid;

    [Header("Данные")]
    [SerializeField] private HeroDefinitionSO[] _allHeroes;
    [SerializeField] private List<ArtifactDefinitionSO> _allArtifacts;

    [Header("Префаб кнопки")]
    [SerializeField] private GameObject _itemElementPrefab;

    private int _currentHeroIndex = 0;

    void Start()
    {
        _screen.SetActive(false);
        _prevHeroButton.onClick.AddListener(PrevHero);
        _nextHeroButton.onClick.AddListener(NextHero);
    }

    public void OpenScreen()
    {
        _screen.SetActive(true);
        Refresh();
        Debug.Log("[HeroEquip] Экран открыт");
    }

    public void CloseScreen()
    {
        _screen.SetActive(false);
        Debug.Log("[HeroEquip] Экран закрыт");
    }

    private void PrevHero()
    {
        _currentHeroIndex--;
        if (_currentHeroIndex < 0)
            _currentHeroIndex = _allHeroes.Length - 1;
        Refresh();
    }

    private void NextHero()
    {
        _currentHeroIndex++;
        if (_currentHeroIndex >= _allHeroes.Length)
            _currentHeroIndex = 0;
        Refresh();
    }

    private void Refresh()
    {
        if (_allHeroes == null || _allHeroes.Length == 0)
        {
            Debug.Log("[HeroEquip] Массив героев пуст!", this);
            return;
        }

        HeroDefinitionSO hero = _allHeroes[_currentHeroIndex];

        // HeroName — с большой буквы, так как это публичное свойство
        string heroId = hero.HeroName;
        HeroSaveData saveData = SaveSystem.Instance.GetOrCreateHeroData(heroId);

        _heroNameText.text = hero.HeroName;

        RefreshSlots(heroId, saveData);
        RefreshInventory(heroId);
    }

    private void RefreshSlots(string heroId, HeroSaveData saveData)
    {
        foreach (Transform child in _slotsGrid)
            Destroy(child.gameObject);

        for (int i = 0; i < 4; i++)
        {
            bool hasArtifact = i < saveData.equippedArtifactIds.Count;
            string artifactId = hasArtifact ? saveData.equippedArtifactIds[i] : null;

            GameObject slot = Instantiate(_itemElementPrefab, _slotsGrid);
            TextMeshProUGUI label = slot.GetComponentInChildren<TextMeshProUGUI>();

            if (hasArtifact)
            {
                ArtifactDefinitionSO artifact = FindArtifact(artifactId);
                label.text = artifact != null ? artifact.artifactName : artifactId;

                string idCopy = artifactId;
                string heroIdCopy = heroId;
                slot.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SaveSystem.Instance.UnequipArtifact(heroIdCopy, idCopy);
                    Debug.Log($"[HeroEquip] Снят артефакт {idCopy} с {heroIdCopy}", this);
                    Refresh();
                });
            }
            else
            {
                label.text = $"Слот {i + 1}";
                slot.GetComponent<Button>().interactable = false;
            }
        }
    }

    private void RefreshInventory(string heroId)
    {
        foreach (Transform child in _inventoryGrid)
            Destroy(child.gameObject);

        List<string> freeIds = SaveSystem.Instance.GetFreeArtifacts();
        
        // ДОБАВЬ ЭТИ ДВЕ СТРОЧКИ:
        Debug.Log($"[HeroEquip] Свободных артефактов: {freeIds.Count}", this);
        Debug.Log($"[HeroEquip] InventoryGrid: {_inventoryGrid.name}", this);

        if (freeIds.Count == 0)
        {
            GameObject empty = Instantiate(_itemElementPrefab, _inventoryGrid);
            empty.GetComponentInChildren<TextMeshProUGUI>().text = "Инвентарь пуст";
            empty.GetComponent<Button>().interactable = false;
            return;
        }

        foreach (string artifactId in freeIds)
        {
            ArtifactDefinitionSO artifact = FindArtifact(artifactId);
            string displayName = artifact != null ? artifact.artifactName : artifactId;

            GameObject card = Instantiate(_itemElementPrefab, _inventoryGrid);
            card.GetComponentInChildren<TextMeshProUGUI>().text = displayName;
            
            Debug.Log($"[HeroEquip] Создал карточку: {displayName}, parent: {card.transform.parent.name}", this);

            string idCopy = artifactId;
            string heroIdCopy = heroId;
            card.GetComponent<Button>().onClick.AddListener(() =>
            {
                bool success = SaveSystem.Instance.EquipArtifact(heroIdCopy, idCopy);
                if (success)
                {
                    Debug.Log($"[HeroEquip] Надет артефакт {idCopy} на {heroIdCopy}", this);
                    Refresh();
                }
            });
        }
    }

    // artifactName используется как ID — так работает твой SaveSystem
    private ArtifactDefinitionSO FindArtifact(string artifactId)
    {
        for (int i = 0; i < _allArtifacts.Count; i++)
        {
            if (_allArtifacts[i].artifactName == artifactId)
                return _allArtifacts[i];
        }
        Debug.Log($"[HeroEquip] Артефакт не найден: {artifactId}", this);
        return null;
    }
}
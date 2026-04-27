using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Новый экран героев с экипировкой (V2).
/// Структура: roster слева, герой+слоты в центре, статы/инвентарь справа, скиллы снизу.
/// </summary>
public class HeroEquipUIV2 : MonoBehaviour
{
    [Header("Главная панель")]
    [SerializeField] private GameObject _screen;

    [Header("Зоны экрана")]
    [SerializeField] private Transform _rosterPanel;
    [SerializeField] private Transform _leftSlotsParent;
    [SerializeField] private Transform _rightSlotsParent;
    [SerializeField] private Transform _skillsBar;

    [Header("Правая панель — режимы")]
    [SerializeField] private GameObject _statsView;
    [SerializeField] private GameObject _inventoryView;
    [SerializeField] private GameObject _artifactInfoView;

    [Header("Правая панель — инфа артефакта")]
    [SerializeField] private Image _artifactIcon;
    [SerializeField] private TextMeshProUGUI _artifactNameText;
    [SerializeField] private TextMeshProUGUI _rarityText;
    [SerializeField] private TextMeshProUGUI _bonusesText;
    [SerializeField] private Button _equipButton;
    [SerializeField] private Button _upgradeButton;

    [Header("Правая панель — статы")]
    [SerializeField] private TextMeshProUGUI _heroNameText;
    [SerializeField] private TextMeshProUGUI _statsText;

    [Header("Правая панель — инвентарь")]
    [SerializeField] private Transform _inventoryGrid;

    [Header("Префабы")]
    [SerializeField] private GameObject _itemElementPrefab;
    [SerializeField] private GameObject _heroButtonPrefab;

    [Header("Данные")]
    [SerializeField] private HeroDefinitionSO[] _allHeroes;
    [SerializeField] private List<ArtifactDefinitionSO> _allArtifacts;

    private int _currentHeroIndex = 0;

    void Start()
    {
        _screen.SetActive(false);
    }

    // ─────────────────────────────────────────────
    public void OpenScreen()
    {
        _screen.SetActive(true);
        Refresh();
        ShowStats();
        Debug.Log("[HeroEquipV2] Экран открыт");
    }

    public void CloseScreen()
    {
        _screen.SetActive(false);
        Debug.Log("[HeroEquipV2] Экран закрыт");
    }

    // ─────────────────────────────────────────────
    /// <summary>
    /// Переключает героя по индексу (вызывается из roster кнопок).
    /// </summary>
    public void SelectHero(int index)
    {
        if (index < 0 || index >= _allHeroes.Length) return;
        _currentHeroIndex = index;
        Refresh();
        ShowStats();
    }

    // ─────────────────────────────────────────────
    /// <summary>
    /// Главное обновление — наполняет все панели для текущего героя.
    /// </summary>
    private void Refresh()
    {
        if (_allHeroes == null || _allHeroes.Length == 0)
        {
            Debug.Log("[HeroEquipV2] Массив героев пуст!", this);
            return;
        }

        HeroDefinitionSO hero = _allHeroes[_currentHeroIndex];
        string heroId = hero.HeroName;
        HeroSaveData saveData = SaveSystem.Instance.GetOrCreateHeroData(heroId);

        RefreshRoster();
        RefreshSlots(heroId, saveData);
        RefreshStats(hero, saveData);
        RefreshInventory(heroId);
    }

    // ─────────────────────────────────────────────
    /// <summary>
    /// Заполняет левую панель — список всех героев.
    /// </summary>
    private void RefreshRoster()
    {
        foreach (Transform child in _rosterPanel)
            Destroy(child.gameObject);

        for (int i = 0; i < _allHeroes.Length; i++)
        {
            int indexCopy = i;
            HeroDefinitionSO hero = _allHeroes[i];

            GameObject btn = Instantiate(_heroButtonPrefab, _rosterPanel);
            TextMeshProUGUI label = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = hero.HeroName;

            btn.GetComponent<Button>().onClick.AddListener(() => SelectHero(indexCopy));
        }
    }

    // ─────────────────────────────────────────────
    /// <summary>
    /// Заполняет 4 слота — 2 слева и 2 справа от модели героя.
    /// </summary>
    private void RefreshSlots(string heroId, HeroSaveData saveData)
    {
        foreach (Transform child in _leftSlotsParent) Destroy(child.gameObject);
        foreach (Transform child in _rightSlotsParent) Destroy(child.gameObject);

        for (int i = 0; i < 6; i++)
        {
            // Слоты 0, 1, 2 — слева; 3, 4, 5 — справа
            Transform parent = i < 3 ? _leftSlotsParent : _rightSlotsParent;

            bool hasArtifact = i < saveData.equippedArtifactIds.Count;
            string artifactId = hasArtifact ? saveData.equippedArtifactIds[i] : null;

            GameObject slot = Instantiate(_itemElementPrefab, parent);
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
                    Refresh();
                });
            }
            else
            {
                label.text = $"Слот {i + 1}";
                // Клик на пустой слот → открыть инвентарь справа
                slot.GetComponent<Button>().onClick.AddListener(ShowInventory);
            }
        }
    }

    // ─────────────────────────────────────────────
    /// <summary>
    /// Обновляет текст статов героя в правой панели.
    /// </summary>
    private void RefreshStats(HeroDefinitionSO hero, HeroSaveData saveData)
    {
        if (_heroNameText != null)
            _heroNameText.text = hero.HeroName;

        if (_statsText != null)
        {
            _statsText.text =
                $"HP: {hero.MaxHP}\n" +
                $"Атака: {hero.Attack}\n" +
                $"Защита: {hero.Defense}\n" +
                $"Уровень: {saveData.level}";
        }
    }

    // ─────────────────────────────────────────────
    /// <summary>
    /// Заполняет правую панель свободными артефактами из инвентаря.
    /// </summary>
    private void RefreshInventory(string heroId)
    {
        if (_inventoryGrid == null) return;

        foreach (Transform child in _inventoryGrid) Destroy(child.gameObject);

        List<string> freeIds = SaveSystem.Instance.GetFreeArtifacts();

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

            string idCopy = artifactId;
            string heroIdCopy = heroId;

            GameObject card = Instantiate(_itemElementPrefab, _inventoryGrid);
            card.GetComponentInChildren<TextMeshProUGUI>().text = displayName;
            card.GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowArtifactInfo(idCopy, heroIdCopy);
            });
        }
    }

    // ─── Переключение режимов правой панели ──────
    public void ShowStats()
    {
        if (_inventoryView != null) _inventoryView.SetActive(true);
        if (_statsView != null) _statsView.SetActive(true);
        if (_artifactInfoView != null) _artifactInfoView.SetActive(false);
    }

    public void ShowInventory()
    {
        // Инвентарь всегда виден, ничего не делаем
        ShowStats();
    }

    // ─────────────────────────────────────────────
    private ArtifactDefinitionSO FindArtifact(string artifactId)
    {
        for (int i = 0; i < _allArtifacts.Count; i++)
            if (_allArtifacts[i].artifactName == artifactId)
                return _allArtifacts[i];
        return null;
    }

    /// <summary>
    /// Показывает инфу о выбранном артефакте внизу правой панели.
    /// </summary>
    private void ShowArtifactInfo(string artifactId, string heroId)
    {
        ArtifactDefinitionSO artifact = FindArtifact(artifactId);
        if (artifact == null) return;

        // Прячем статы, показываем инфу артефакта
        if (_statsView != null) _statsView.SetActive(false);
        if (_artifactInfoView != null) _artifactInfoView.SetActive(true);

        // Заполняем данные
        if (_artifactIcon != null && artifact.icon != null)
            _artifactIcon.sprite = artifact.icon;

        if (_artifactNameText != null)
            _artifactNameText.text = artifact.artifactName;

        if (_rarityText != null)
            _rarityText.text = artifact.rarity.ToString();

        if (_bonusesText != null)
            _bonusesText.text =
                $"Атака: +{artifact.bonusAttack}\n" +
                $"Защита: +{artifact.bonusDefense}\n" +
                $"HP: +{artifact.bonusHP}";

        // Кнопка Надеть
        if (_equipButton != null)
        {
            _equipButton.onClick.RemoveAllListeners();
            _equipButton.onClick.AddListener(() =>
            {
                if (SaveSystem.Instance.EquipArtifact(heroId, artifactId))
                {
                    Refresh();
                    ShowStats();
                }
            });
        }

        // Кнопка Улучшить (пока заглушка)
        if (_upgradeButton != null)
        {
            _upgradeButton.onClick.RemoveAllListeners();
            _upgradeButton.onClick.AddListener(() =>
            {
                Debug.Log("[HeroEquipV2] Улучшение пока не реализовано", this);
            });
        }
    }
}

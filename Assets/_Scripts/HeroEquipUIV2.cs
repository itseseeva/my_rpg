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
    [SerializeField] private GameObject _skillButtonPrefab;

    [Header("Данные")]
    [SerializeField] private HeroDefinitionSO[] _allHeroes;
    [SerializeField] private List<ArtifactDefinitionSO> _allArtifacts;

    [Header("Кнопки")]
    [SerializeField] private Button _equipBestButton; // «Надеть лучшее»

    [Header("Upgrade View — панель прокачки")]
    [SerializeField] private GameObject     _upgradeView;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Slider          _xpBar;
    [SerializeField] private TextMeshProUGUI _xpText;
    [SerializeField] private TextMeshProUGUI _statPointsText;
    [SerializeField] private Transform       _statRowsParent;
    [SerializeField] private GameObject      _statRowPrefab;
    [SerializeField] private Button          _toggleUpgradeButton; // кнопка «Прокачка»
    [SerializeField] private Button          _giveXPTestButton;    // кнопка «+100 XP» (тест)

    [Header("Попап скилла")]
    [SerializeField] private GameObject _skillPopup;
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillDescriptionText;
    [SerializeField] private Button _popupCloseButton;

    private int _currentHeroIndex = 0;

    // Флаг — показываем ли сейчас панель прокачки (вместо инвентаря)
    private bool _upgradeViewActive = false;

    void Start()
    {
        _screen.SetActive(false);
        if (_skillPopup != null) _skillPopup.SetActive(false);
        if (_popupCloseButton != null)
            _popupCloseButton.onClick.AddListener(() => _skillPopup.SetActive(false));

        if (_equipBestButton != null)
            _equipBestButton.onClick.AddListener(EquipBest);

        // Upgrade panel
        if (_upgradeView != null)      _upgradeView.SetActive(false);
        if (_toggleUpgradeButton != null)
            _toggleUpgradeButton.onClick.AddListener(ToggleUpgradeView);
        if (_giveXPTestButton != null)
            _giveXPTestButton.onClick.AddListener(GiveTestXP);
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
        RefreshSkills(hero);
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

            // ── Цвет редкости через ArtifactSlotUI ──
            ArtifactSlotUI slotUI = slot.GetComponent<ArtifactSlotUI>();

            if (hasArtifact)
            {
                ArtifactDefinitionSO artifact = FindArtifact(artifactId);
                if (slotUI != null)
                    slotUI.Setup(artifact);
                else if (slot.GetComponentInChildren<TextMeshProUGUI>() is TextMeshProUGUI lbl)
                    lbl.text = artifact != null ? artifact.artifactName : artifactId;

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
                if (slotUI != null)
                    slotUI.SetEmpty($"Слот {i + 1}");
                else if (slot.GetComponentInChildren<TextMeshProUGUI>() is TextMeshProUGUI lbl)
                    lbl.text = $"Слот {i + 1}";

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
            // Считаем производные статы через калькулятор
            HeroStats s = HeroStatsCalculator.Compute(hero, saveData);

            _statsText.text =
                $"Уровень: {saveData.level}\n" +
                $"──────────────\n" +
                $"HP:       {s.MaxHP}\n" +
                $"Мана:     {s.MaxMana}\n" +
                $"Физ.ATK:  {s.PhysATK}\n" +
                $"Маг.ATK:  {s.MagATK}\n" +
                $"Защита:   {s.DEF}\n" +
                $"Скорость: {s.Speed:F0}\n" +
                $"Крит:     {s.CritChance:F1}%\n" +
                $"Сопр.:    {s.Resistance:F0}";
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

            // ── Цвет редкости через ArtifactSlotUI ──
            ArtifactSlotUI slotUI = card.GetComponent<ArtifactSlotUI>();
            if (slotUI != null)
                slotUI.Setup(artifact);
            else if (card.GetComponentInChildren<TextMeshProUGUI>() is TextMeshProUGUI lbl)
                lbl.text = displayName;

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
        {
            _rarityText.text  = RarityColorHelper.GetRarityLabel(artifact.rarity);
            _rarityText.color = RarityColorHelper.GetBorderColor(artifact.rarity);
        }

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

    /// <summary>
    /// Заполняет SkillsBar 4 кнопками способностей текущего героя.
    /// </summary>
    private void RefreshSkills(HeroDefinitionSO hero)
    {
        if (_skillsBar == null) return;

        foreach (Transform child in _skillsBar)
            Destroy(child.gameObject);

        AbilityEffect[] abilities = hero.Abilities;

        for (int i = 0; i < 4; i++)
        {
            GameObject btn = Instantiate(_skillButtonPrefab, _skillsBar);
            TextMeshProUGUI label = btn.GetComponentInChildren<TextMeshProUGUI>();

            bool hasSkill = abilities != null && i < abilities.Length && abilities[i] != null;

            if (hasSkill)
            {
                AbilityEffect ability = abilities[i];
                label.text = ability.AbilityName;

                AbilityEffect abilityCopy = ability; // для замыкания
                btn.GetComponent<Button>().onClick.AddListener(() =>
                {
                    ShowSkillPopup(abilityCopy);
                });
            }
            else
            {
                label.text = "—";
                btn.GetComponent<Button>().interactable = false;
            }
        }
    }

    /// <summary>
    /// Показывает попап с описанием скилла.
    /// </summary>
    private void ShowSkillPopup(AbilityEffect ability)
    {
        if (_skillPopup == null) return;

        _skillPopup.SetActive(true);

        if (_skillNameText != null)
            _skillNameText.text = ability.AbilityName;

        if (_skillDescriptionText != null)
            _skillDescriptionText.text = ability.Description;
    }

    // ─────────────────────────────────────────────────────────────────
    /// <summary>
    /// Автоматически надевает лучшие артефакты для текущего героя.
    /// Алгоритм: снять всё → собрать пул (надетые + свободные) →
    /// отсортировать по суммарному стату → надеть топ-6.
    /// </summary>
    public void EquipBest()
    {
        if (_allHeroes == null || _allHeroes.Length == 0) return;

        HeroDefinitionSO hero  = _allHeroes[_currentHeroIndex];
        string heroId          = hero.HeroName;
        HeroSaveData saveData  = SaveSystem.Instance.GetOrCreateHeroData(heroId);

        // 1. Собираем пул: текущие надетые + свободные в инвентаре
        List<string> pool = new List<string>(saveData.equippedArtifactIds);
        pool.AddRange(SaveSystem.Instance.GetFreeArtifacts());

        // 2. Убираем из пула те которых нет в каталоге (защита от мусора)
        List<ArtifactDefinitionSO> candidates = new List<ArtifactDefinitionSO>();
        for (int i = 0; i < pool.Count; i++)
        {
            ArtifactDefinitionSO art = FindArtifact(pool[i]);
            if (art != null) candidates.Add(art);
        }

        // 3. Сортировка по суммарному стату (без LINQ — мобайл-friendly)
        for (int i = 0; i < candidates.Count - 1; i++)
        {
            for (int j = i + 1; j < candidates.Count; j++)
            {
                int powerI = candidates[i].bonusAttack + candidates[i].bonusDefense + candidates[i].bonusHP;
                int powerJ = candidates[j].bonusAttack + candidates[j].bonusDefense + candidates[j].bonusHP;
                if (powerJ > powerI)
                {
                    (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
                }
            }
        }

        // 4. Снимаем всё
        SaveSystem.Instance.UnequipAllArtifacts(heroId);

        // 5. Надеваем топ-6
        int equipped = 0;
        for (int i = 0; i < candidates.Count && equipped < 6; i++)
        {
            if (SaveSystem.Instance.EquipArtifact(heroId, candidates[i].artifactName))
                equipped++;
        }

        Debug.Log($"[HeroEquipV2] EquipBest: {heroId} — надето {equipped} артефактов", this);

        Refresh();
        ShowStats();
    }

    // ═══════════════════════════════════════════════════════════════
    //  ПАНЕЛЬ ПРОКАЧКИ
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Переключает между инвентарём и панелью прокачки.
    /// Вызывается кнопкой «Прокачка».
    /// </summary>
    public void ToggleUpgradeView()
    {
        _upgradeViewActive = !_upgradeViewActive;

        if (_inventoryView != null) _inventoryView.SetActive(!_upgradeViewActive);
        if (_upgradeView   != null) _upgradeView.SetActive(_upgradeViewActive);

        if (_upgradeViewActive) RefreshUpgradeView();

        Debug.Log($"[HeroEquipV2] Прокачка: {(_upgradeViewActive ? "открыта" : "закрыта")}", this);
    }

    /// <summary>
    /// Обновляет всю панель прокачки: уровень, XP-бар, строки статов.
    /// </summary>
    private void RefreshUpgradeView()
    {
        if (_allHeroes == null || _allHeroes.Length == 0) return;

        HeroDefinitionSO hero   = _allHeroes[_currentHeroIndex];
        string heroId           = hero.HeroName;
        HeroSaveData saveData   = SaveSystem.Instance.GetOrCreateHeroData(heroId);

        bool isMax = saveData.level >= HeroStatsCalculator.MaxLevel;

        // ── Уровень ──
        if (_levelText != null)
            _levelText.text = isMax
                ? $"Уровень {saveData.level} (MAX)"
                : $"Уровень {saveData.level}";

        // ── XP прогресс-бар ──
        float progress = HeroStatsCalculator.GetLevelProgress(saveData.experience, saveData.level);
        if (_xpBar != null)
        {
            _xpBar.minValue    = 0f;
            _xpBar.maxValue    = 1f;
            _xpBar.value       = progress;
            _xpBar.interactable = false;
        }

        // ── XP текст ──
        if (_xpText != null)
        {
            if (isMax)
            {
                _xpText.text = "MAX LEVEL";
            }
            else
            {
                int inLevel = HeroStatsCalculator.GetXPWithinLevel(saveData.experience, saveData.level);
                int needed  = HeroStatsCalculator.GetXPNeededForCurrentLevel(saveData.level);
                _xpText.text = $"{inLevel} / {needed} XP";
            }
        }

        // ── Очки статов ──
        if (_statPointsText != null)
            _statPointsText.text = $"Очки: {saveData.statPoints}";

        // ── Строки статов ──
        if (_statRowsParent == null || _statRowPrefab == null) return;

        foreach (Transform child in _statRowsParent)
            Destroy(child.gameObject);

        bool hasPoints = saveData.statPoints > 0;
        SpawnStatRow("Сила",         "strength",  saveData.strength,  heroId, hasPoints);
        SpawnStatRow("Ловкость",     "agility",   saveData.agility,   heroId, hasPoints);
        SpawnStatRow("Интеллект",    "intellect", saveData.intellect, heroId, hasPoints);
        SpawnStatRow("Выносливость", "endurance", saveData.endurance, heroId, hasPoints);
    }

    /// <summary>
    /// Создаёт одну строку стата в панели прокачки.
    /// </summary>
    private void SpawnStatRow(string displayName, string statKey, int value,
                              string heroId, bool interactable)
    {
        GameObject row   = Instantiate(_statRowPrefab, _statRowsParent);
        StatRowUI  rowUI = row.GetComponent<StatRowUI>();
        if (rowUI != null)
        {
            rowUI.Setup(displayName, statKey, value, heroId);
            rowUI.SetInteractable(interactable);
        }
    }

    /// <summary>
    /// Вызывается из StatRowUI после траты очка — обновляет статы и панель прокачки.
    /// </summary>
    public void RefreshAfterStatSpend()
    {
        HeroDefinitionSO hero   = _allHeroes[_currentHeroIndex];
        HeroSaveData saveData   = SaveSystem.Instance.GetOrCreateHeroData(hero.HeroName);
        RefreshStats(hero, saveData);
        RefreshUpgradeView();
    }

    /// <summary>
    /// Тестовая кнопка — даёт 100 XP текущему герою.
    /// УДАЛИТЬ перед релизом или оставить для тестов.
    /// </summary>
    private void GiveTestXP()
    {
        if (_allHeroes == null || _allHeroes.Length == 0) return;
        string heroId = _allHeroes[_currentHeroIndex].HeroName;
        SaveSystem.Instance.AddXP(heroId, 100);
        Refresh();
        if (_upgradeViewActive) RefreshUpgradeView();
        Debug.Log($"[HeroEquipV2] [ТЕСТ] +100 XP для {heroId}", this);
    }
}

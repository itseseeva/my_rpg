using UnityEngine;

public class UnitController : MonoBehaviour
{
    [Header("Карточка героя")]
    [SerializeField] private HeroDefinitionSO _heroData;

    [Header("UI")]
    [SerializeField] private HPBarUI _hpBarPrefab;
    private HPBarUI _hpBar;

    private Renderer _renderer;
    private Color _defaultColor;

    // Свойства (PascalCase) автоматически скрыты в инспекторе и доступны для других скриптов
    public string UnitName { get; private set; }
    public int MaxHP { get; private set; }
    public int CurrentHP { get; private set; }
    public int AttackPower { get; private set; }
    public int Defense { get; private set; }

    // Индекс выбранной способности
    public int SelectedAbilityIndex { get; private set; } = 0;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            // Создаём уникальный материал для каждого юнита
            _renderer.material = new Material(_renderer.material);
            _defaultColor = _renderer.material.color;
        }

        if (_heroData == null) return;

        // Берём данные с карточки.
        UnitName = _heroData.HeroName;
        MaxHP = _heroData.MaxHP;
        CurrentHP = _heroData.MaxHP;
        AttackPower = _heroData.Attack;
        Defense = _heroData.Defense;

        Debug.Log($"[UnitController] {UnitName} появился с {CurrentHP} HP", this);
    }

    private void Start()
    {
        // Создаём HP бар над головой
        if (_hpBarPrefab != null)
        {
            _hpBar = Instantiate(_hpBarPrefab, 
                    transform.position + Vector3.up * 1.5f, 
                    Quaternion.identity);
            _hpBar.transform.localScale = new Vector3(0.012f, 0.02f, 0.02f);

            // Поворачиваем к камере
            if (Camera.main != null)
            {
                _hpBar.transform.LookAt(Camera.main.transform);
                _hpBar.transform.Rotate(0, 180f, 0);
            }

            Canvas worldCanvas = GameObject.Find("WorldSpaceCanvas").GetComponent<Canvas>();
            _hpBar.transform.SetParent(worldCanvas.transform);
            _hpBar.SetMaxHP(MaxHP);
            _hpBar.SetHP(MaxHP);
        }
    }

    /// <summary>
    /// Атакует выбранную цель.
    /// Перенесли логику расчета защиты и баффов из-за внедрения StatsSystem.
    /// </summary>
    public void Attack(UnitController target)
    {
        StatsSystem myStats = GetComponent<StatsSystem>();
        StatsSystem targetStats = target.GetComponent<StatsSystem>();

        int finalAttack = myStats != null ? myStats.GetFinalAttack() : AttackPower;
        int finalDefense = targetStats != null ? targetStats.GetFinalDefense() : target.Defense;

        // Здесь происходит вычет защиты! Урон не может быть меньше 1
        int damage = Mathf.Max(1, finalAttack - finalDefense);
        
        target.TakeDamage(damage);
    }

    /// <summary>
    /// Возвращает способность героя по индексу, если она существует.
    /// </summary>
    public AbilityEffect GetAbility(int index)
    {
        if (_heroData == null || _heroData.Abilities == null) 
            return null;
        if (index >= _heroData.Abilities.Length) 
            return null;
        return _heroData.Abilities[index];
    }

    /// <summary>
    /// Вызывает способность по её индексу из карточки героя.
    /// </summary>
    /// <param name="abilityIndex">Индекс способности в массиве.</param>
    /// <param name="target">Цель применения способности.</param>
    public void UseAbility(int abilityIndex, UnitController target)
    {
        if (_heroData == null || _heroData.Abilities == null || abilityIndex >= _heroData.Abilities.Length)
        {
            Debug.Log("[UnitController] Способность не найдена или карточка пуста!", this);
            return;
        }

        AbilityEffect ability = _heroData.Abilities[abilityIndex];
        ability.Execute(this, target);
    }

    /// <summary>
    /// Выбирает способность для применения в следующий ход.
    /// </summary>
    public void SelectAbility(int index)
    {
        if (_heroData == null || _heroData.Abilities == null || index >= _heroData.Abilities.Length)
        {
            Debug.Log("[UnitController] Невозможно выбрать способность: индекс вне диапазона!", this);
            return;
        }

        SelectedAbilityIndex = index;
        Debug.Log($"[UnitController] {UnitName} выбрал способность: {_heroData.Abilities[index].AbilityName}", this);
    }

    /// <summary>
    /// Получает урон (теперь мы передаем сюда уже "чистый" рассчитанный урон из метода Attack).
    /// </summary>
    /// <param name="damage">Чистый урон для вычитания из HP.</param>
    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;

        if (_hpBar != null)
            _hpBar.SetHP(CurrentHP);
        
        Debug.Log($"[UnitController] {UnitName} получил {damage} урона. HP: {CurrentHP}/{MaxHP}", this);

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"[UnitController] {UnitName} погиб!", this);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Восстанавливает здоровье юниту.
    /// </summary>
    /// <param name="amount">Количество HP.</param>
    public void Heal(int amount)
    {
        CurrentHP += amount;
        CurrentHP = Mathf.Min(CurrentHP, MaxHP); // Защита от перелечивания выше максимума
    }

    /// <summary>
    /// Устанавливает подсветку юнита (желтый цвет для активного).
    /// </summary>
    public void SetHighlight(bool active)
    {
        if (_renderer != null)
        {
            _renderer.material.color = active ? Color.yellow : _defaultColor;
        }
    }
}

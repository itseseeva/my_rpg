using UnityEngine;

public class UnitController : MonoBehaviour
{
    [Header("Карточка героя")]
    [SerializeField] private HeroDefinitionSO _heroData;

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
        if (_heroData == null) return;

        // Берём данные с карточки.
        UnitName = _heroData.HeroName;
        MaxHP = _heroData.MaxHP;
        CurrentHP = _heroData.MaxHP;
        AttackPower = _heroData.Attack;
        Defense = _heroData.Defense;

        Debug.Log($"[UnitController] {UnitName} появился с {CurrentHP} HP", this);
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
}

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Менеджер сетки поля боя.
/// Хранит позиции юнитов и расставляет их по ячейкам.
/// </summary>
public class GridSystem : MonoBehaviour
{
    public static GridSystem Instance { get; private set; }

    [Header("Размер сетки")]
    [SerializeField] private int _columns = 5;
    [SerializeField] private int _rows = 2;

    [Header("Настройки позиций")]
    [SerializeField] private float _cellSize = 2f;
    [SerializeField] private Vector3 _heroRowStart = new Vector3(-4f, 0f, -2f);
    [SerializeField] private Vector3 _enemyRowStart = new Vector3(-4f, 0f, 2f);

    // Словарь: позиция на сетке → юнит
    private Dictionary<Vector2Int, UnitController> _grid = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Регистрирует юнита на клетке сетки и устанавливает его физическую позицию.
    /// </summary>
    public void PlaceUnit(UnitController unit, int col, bool isHero)
    {
        Vector2Int gridPos = new Vector2Int(col, isHero ? 0 : 1);
        _grid[gridPos] = unit;

        Vector3 startPos = isHero ? _heroRowStart : _enemyRowStart;
        
        // Справа налево чтобы совпадало с видом камеры
        Vector3 worldPos = startPos + new Vector3(col * _cellSize, 0f, 0f);
        worldPos.y = 0.5f;
        unit.transform.position = worldPos;

        Debug.Log($"[Grid] {unit.UnitName} → клетка ({col}, {(isHero ? "герои" : "враги")})", unit);
    }

    /// <summary>
    /// Возвращает юнита на клетке.
    /// </summary>
    public UnitController GetUnit(int col, bool isHero)
    {
        Vector2Int gridPos = new Vector2Int(col, isHero ? 0 : 1);
        _grid.TryGetValue(gridPos, out UnitController unit);
        return unit;
    }

    /// <summary>
    /// Убирает юнита с сетки, когда он умирает.
    /// </summary>
    public void RemoveUnit(UnitController unit)
    {
        Vector2Int? keyToRemove = null;
        foreach (var pair in _grid)
        {
            if (pair.Value == unit)
            {
                keyToRemove = pair.Key;
                break;
            }
        }
        if (keyToRemove.HasValue)
            _grid.Remove(keyToRemove.Value);
    }
}

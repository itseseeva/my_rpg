using UnityEngine;
using UnityEngine.InputSystem;

public class TargetingSystem : MonoBehaviour
{
    public static TargetingSystem Instance { get; private set; }

    [SerializeField] private TurnOrderManager _turnManager;

    private UnitController _selectedTarget;
    private UnitController _previousTarget;
    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _mainCamera = Camera.main;

        if (_turnManager == null)
        {
            _turnManager = Object.FindAnyObjectByType<TurnOrderManager>();
        }
    }

    private void Update()
    {
        // Ждём клик мышкой через новую систему ввода (New Input System)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_mainCamera == null) return;

            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = _mainCamera.ScreenPointToRay(mousePos);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                UnitController unit = hit.collider.GetComponent<UnitController>();
                if (unit != null)
                {
                    TrySelectTarget(unit);
                }
            }
        }
    }

    private void TrySelectTarget(UnitController unit)
    {
        // Можно выбирать только врагов, когда ход героя
        if (_turnManager == null || !_turnManager.IsHeroTurn()) return;
        if (!unit.gameObject.activeSelf) return;

        // Проверяем, что это враг, а не герой
        if (_turnManager.EnemyTeam.Contains(unit))
        {
            // Снимаем подсветку с предыдущей цели
            if (_previousTarget != null)
                _previousTarget.SetHighlight(false);

            // Подсвечиваем новую цель
            _selectedTarget = unit;
            _previousTarget = unit;
            unit.SetHighlight(true);

            Debug.Log($"[Targeting] Выбрана цель: {unit.UnitName}", this);
        }
    }

    /// <summary>
    /// Возвращает текущую выбранную цель. Автоматически сбрасывается, если цель уже мертва.
    /// </summary>
    public UnitController GetSelectedTarget()
    {
        // Если цель мертва — сбрасываем
        if (_selectedTarget != null && !_selectedTarget.gameObject.activeSelf)
        {
            _selectedTarget = null;
        }
        return _selectedTarget;
    }

    /// <summary>
    /// Принудительно очищает текущую выбранную цель.
    /// </summary>
    public void ClearTarget()
    {
        if (_previousTarget != null)
        {
            _previousTarget.SetHighlight(false);
            _previousTarget = null;
        }
        _selectedTarget = null;
    }
}

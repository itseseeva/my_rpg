using UnityEngine;

/// <summary>
/// Автоматически расставляет юнитов по сетке при старте.
/// </summary>
public class UnitPlacer : MonoBehaviour
{
    [Header("Команды")]
    [SerializeField] private UnitController[] _heroTeam = new UnitController[5];
    [SerializeField] private UnitController[] _enemyTeam = new UnitController[5];

    private void Start()
    {
        PlaceTeams();
    }

    private void PlaceTeams()
    {
        if (GridSystem.Instance == null)
        {
            Debug.LogError("[UnitPlacer] GridSystem.Instance не найден!");
            return;
        }

        for (int i = 0; i < _heroTeam.Length; i++)
        {
            if (_heroTeam[i] != null)
                GridSystem.Instance.PlaceUnit(_heroTeam[i], i, true);
        }

        for (int i = 0; i < _enemyTeam.Length; i++)
        {
            if (_enemyTeam[i] != null)
                GridSystem.Instance.PlaceUnit(_enemyTeam[i], i, false);
        }
    }
}

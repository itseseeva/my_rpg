using UnityEngine;

public class HeroEquipUI : MonoBehaviour
{
    [Header("Главная панель")]
    [SerializeField] private GameObject _screen;

    void Start()
    {
        _screen.SetActive(false);
    }

    public void OpenScreen()
    {
        _screen.SetActive(true);
        Debug.Log("[HeroEquip] Экран открыт");
    }

    public void CloseScreen()
    {
        _screen.SetActive(false);
        Debug.Log("[HeroEquip] Экран закрыт");
    }
}

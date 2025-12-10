using UnityEngine;
using UnityEngine.Rendering;

public class HpSystem : MonoBehaviour, ISubSystem //체력 시스템
{
    private GameManager gameManager;
    [SerializeField] private HpUI hpUI;
    private int maxHp = 100;
    private int currentHp;

    public void Initialize(GameManager gameManager)
    {
        this.gameManager = gameManager;
        currentHp = maxHp;

        hpUI.SetHP(currentHp);
    }

    public void Deinitialize()
    {
        gameManager = null;
    }

    public void TakeDamage(int amount)
    {
        currentHp = Mathf.Max(0, currentHp - amount);
        hpUI.SetHP(currentHp);
        hpUI.DamageAnimation();

        if (currentHp <= 0)
        {
            gameManager.OnGameOver();
        }
    }
}
using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour // 골드 ui view
{
    [SerializeField] TMP_Text goldText;

    public void Refresh(int gold)
    {
        goldText.SetText($"{gold}");
    }
}
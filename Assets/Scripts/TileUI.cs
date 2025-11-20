using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileUI : MonoBehaviour // 타일 ui view
{
    [SerializeField] private Image bgImage;
    [SerializeField] private TextMeshProUGUI valueText;
    private Dictionary<int, Color> colors = new Dictionary<int, Color>()
    {
        { 0, new Color(0.90f, 0.90f, 0.90f) },
        { 2, new Color(0.93f, 0.89f, 0.85f) },
        { 4, new Color(0.93f, 0.88f, 0.78f) },
        { 8, new Color(0.95f, 0.69f, 0.47f) },
        { 16, new Color(0.96f, 0.58f, 0.39f) },
        { 32, new Color(0.96f, 0.49f, 0.37f) },
        { 64, new Color(0.96f, 0.37f, 0.23f) },
        { 128, new Color(0.93f, 0.81f, 0.45f) },
        { 256, new Color(0.93f, 0.80f, 0.38f) },
        { 512, new Color(0.93f, 0.78f, 0.31f) },
        { 1024, new Color(0.93f, 0.77f, 0.25f) },
        { 2048, new Color(0.93f, 0.76f, 0.18f) },
        { 4096, new Color(0.50f, 0.80f, 0.18f) },
        { 8192, new Color(0.50f, 0.75f, 0.15f) },
        { 16384, new Color(0.50f, 0.65f, 0.10f) },
        
        //16384 보다 큰 모든 값
        {-1, new Color(0.2f, 0.50f, 0.73f)}
    };
    private int value;
    public int Value => value;

    public void SetValue(int value)
    {
        if(value == 0)
            valueText.text = "";
        else
            valueText.text = value.ToString();
        
        UpdateColor(value);
    }

    void UpdateColor(int value)
    {
        Color col;
        if (!colors.TryGetValue(value, out col))
        {
            col = colors[-1];
        }
        bgImage.color = col;
    }
}

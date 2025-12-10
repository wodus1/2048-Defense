using TMPro;
using UnityEngine;

public class MonsterSystemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text breaktimeText;
    private string originalText;

    private void Start()
    {
        originalText = breaktimeText.text;
    }

    public void SetActive(bool active)
    {
        breaktimeText.gameObject.SetActive(active);
    }

    public void SetTimer(int time)
    {
        string timer = originalText.Replace("[time]", time.ToString());
        breaktimeText.text = timer;
    }
}

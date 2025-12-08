using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour //강화 시스템 ui view
{
    [SerializeField] private GameObject upgradeUIPanel;
    [SerializeField] private Button effect1Button;
    [SerializeField] private Button effect2Button;
    [SerializeField] private TMP_Text effect1TitleText;
    [SerializeField] private TMP_Text effect1DescriptionText;
    [SerializeField] private TMP_Text effect2TitleText;
    [SerializeField] private TMP_Text effect2DescriptionText;

    private UpgradeSystem upgradeSystem;
    private Effect currentEffect1;
    private Effect currentEffect2;

    public void Awake()
    {
        effect1Button.onClick.RemoveAllListeners();
        effect1Button.onClick.AddListener(OnClickEffect1);

        effect2Button.onClick.RemoveAllListeners();
        effect2Button.onClick.AddListener(OnClickEffect2);
    }

    public void Initialize(UpgradeSystem upgradeSystem, Effect effect1, Effect effect2)
    {
        this.upgradeSystem = upgradeSystem;
        this.currentEffect1 = effect1;
        this.currentEffect2 = effect2;

        upgradeUIPanel.SetActive(true);

        effect1TitleText.text = effect1.title;
        effect1DescriptionText.text = effect1.description;

        effect2TitleText.text = effect2.title;
        effect2DescriptionText.text = effect2.description;

        this.upgradeSystem.Pause();
    }

    private void OnClickEffect1()
    {
        if (currentEffect1 == null || upgradeSystem == null) return;

        currentEffect1.Apply(upgradeSystem);
        upgradeUIPanel.SetActive(false);

        upgradeSystem.Resume();
    }

    private void OnClickEffect2()
    {
        if (currentEffect2 == null || upgradeSystem == null) return;

        currentEffect2.Apply(upgradeSystem);
        upgradeUIPanel.SetActive(false);

        upgradeSystem.Resume();
    }
}

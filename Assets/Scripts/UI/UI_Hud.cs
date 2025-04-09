using UnityEngine;
using UnityEngine.UI;

public class UI_Hud : UI
{
    [SerializeField] private Image costBar;

    [SerializeField] private FloatEventChannel OnCostChanged;

    protected override void Awake()
    {
        base.Awake();

        OnCostChanged.UnregisterListener(SetCostBar);

        OnCostChanged.RegisterListener(SetCostBar);
    }

    void SetCostBar(float ratio)
    {
        costBar.fillAmount = ratio;
    }
}

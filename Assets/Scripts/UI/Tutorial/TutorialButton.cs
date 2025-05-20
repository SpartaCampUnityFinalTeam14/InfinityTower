using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : TutorialStep
{
    protected override void Awake()
    {
        base.Awake();

        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    public override void OnStep()
    {
        UIManager.Instance.GetUI<UI_LobbyTutorial>().SetMaskPosition(GetComponent<RectTransform>());
    }

    public override void OnClicked()
    {
        UIManager.Instance.GetUI<UI_LobbyTutorial>().NextStep(order);
    }
}

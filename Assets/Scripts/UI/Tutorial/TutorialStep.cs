using UnityEngine;

public abstract class TutorialStep : MonoBehaviour
{
    public int order;
    public string explanation;

    protected virtual void Awake()
    {
        UIManager.Instance.ShowUI<UI_LobbyTutorial>().steps.Add(this);
    }

    public abstract void OnStep();

    public abstract void OnClicked();
}

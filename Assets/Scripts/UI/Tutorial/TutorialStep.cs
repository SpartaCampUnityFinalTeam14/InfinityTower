using UnityEngine;

public abstract class TutorialStep : MonoBehaviour
{
    public int order;
    public string explanation;

    protected void Start()
    {
        UIManager.Instance.GetUI<UI_LobbyTutorial>().steps.Add(this);
    }

    public abstract void OnStep();

    public abstract void OnClicked();
}

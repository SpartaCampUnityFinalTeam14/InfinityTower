using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Main : UI
{
    [SerializeField] private NestedScrollManager scrollManager;
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private GameObject tutorialBackground;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [SerializeField] private EventChannel OnGoldChanged;

    protected override void Awake()
    {
        base.Awake();

        yesButton.onClick.AddListener(TutorialStart);
        noButton.onClick.AddListener(TutorialSkip);

        UnregisterListeners();
        RegisterListeners();

        UpdateGold();
    }

    private void Start()
    {
        if (!SaveManager.Instance.playerData.isTutorialAlreadySeen)
        {
            SaveManager.Instance.playerData.isTutorialAlreadySeen = true;
            SaveManager.Instance.SavePlayerData();
            tutorialBackground.SetActive(true);
        }
        else tutorialBackground.SetActive(false);
    }

    public void TutorialStart()
    {
        tutorialBackground.SetActive(false);
        UIManager.Instance.ShowUI<UI_LobbyTutorial>().StartStep();
    }

    public void TutorialSkip()
    {
        tutorialBackground.SetActive(false);
        UIManager.Instance.ShowUI<UI_LobbyTutorial>().Close();
    }

    IEnumerator StartTutorial()
    {
        yield return new WaitForEndOfFrame();

        UIManager.Instance.GetUI<UI_LobbyTutorial>().StartStep();
    }

    void UnregisterListeners()
    {
        OnGoldChanged.UnregisterListener(UpdateGold);
    }

    void RegisterListeners()
    {
        OnGoldChanged.RegisterListener(UpdateGold);
    }

    void UpdateGold()
    {
        int gold = SaveManager.Instance.playerData.gold;
        goldText.text = string.Format("{0:N0}", gold);
    }

    public override void Clear()
    {
        UnregisterListeners(); ;
        scrollManager.UnregisterListeners();

        base.Clear();
    }
}

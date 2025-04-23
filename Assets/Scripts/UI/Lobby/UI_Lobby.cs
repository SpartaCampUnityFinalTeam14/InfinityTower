using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Lobby : UI
{
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private Image championImage;
    [SerializeField] private TextMeshProUGUI championNameText;

    [SerializeField] private Button stageStartButton;
    [SerializeField] private Button championSelectButton;
    [SerializeField] private Button deckSelectButton;
    [SerializeField] private Button artifactButton;
    [SerializeField] private Button gachaButton;

    [SerializeField] private IntEventChannel OnChampionSelected;

    protected override void Awake()
    {
        base.Awake();

        goldText.text = SaveManager.Instance.playerData.gold.ToString();
        stageStartButton.onClick.AddListener(() => GameManager.Instance.LoadScene("KSM_Stage"));
        championSelectButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Deck>().SetDeckTab(false));
        deckSelectButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Deck>().SetDeckTab(true));
        artifactButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Artifact>());
        gachaButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Gacha>());

        UnregisterListeners();
        RegisterListeners();

        SetChampion(SaveManager.Instance.playerData.selectedChampionIndex);
    }

    void UnregisterListeners()
    {
        OnChampionSelected.UnregisterListener(SetChampion);
    }

    void RegisterListeners()
    {
        OnChampionSelected.RegisterListener(SetChampion);
    }

    void SetChampion(int index)
    {
        //스프라이트 세팅
        championNameText.text = DataManager.Instance.championDict[index].name;
    }

    public override void Clear()
    {
        base.Clear();

        UnregisterListeners();
    }
}

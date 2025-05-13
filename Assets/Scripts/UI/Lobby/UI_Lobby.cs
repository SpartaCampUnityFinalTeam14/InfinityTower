using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : UI, ScrollPanel
{
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private RectTransform championBackgroundTransform;
    [SerializeField] private Image championImage;
    [SerializeField] private TextMeshProUGUI championNameText;

    [SerializeField] private Button stageStartButton;
    [SerializeField] private Button championSelectButton;
    [SerializeField] private Button deckSelectButton;
    [SerializeField] private Button artifactButton;
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button optionButton;

    [SerializeField] private IntEventChannel OnChampionSelected;
    [SerializeField] private EventChannel OnGoldChanged;

    protected override void Awake()
    {
        base.Awake();

        UpdateGold();

        stageStartButton.onClick.AddListener(() => GameManager.Instance.LoadScene("KSM_Stage"));
        //championSelectButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Deck>().SetDeckTab(false));
        deckSelectButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Deck>().UpdateTab());
        artifactButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Artifact>().UpdateGold());
        gachaButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Gacha>().ResetPanel());
        optionButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_Option>());

        UnregisterListeners();
        RegisterListeners();

        SetChampion(SaveManager.Instance.playerData.selectedChampionIndex);
    }

    void UnregisterListeners()
    {
        OnChampionSelected.UnregisterListener(SetChampion);
        OnGoldChanged.UnregisterListener(UpdateGold);
    }

    void RegisterListeners()
    {
        OnChampionSelected.RegisterListener(SetChampion);
        OnGoldChanged.RegisterListener(UpdateGold);
    }

    public void ResetPanel() => UpdateGold();

    void SetChampion(int index)
    {
        //스프라이트 세팅
        RotateSlotRandom();

        championNameText.text = DataManager.Instance.championDict[index].name;
    }

    void RotateSlotRandom()
    {
        float randomRotZ = Random.Range(-5f, 5f);
        Vector3 curRot = championBackgroundTransform.eulerAngles;
        championBackgroundTransform.eulerAngles = new Vector3(curRot.x, curRot.y, randomRotZ);
    }

    void UpdateGold()
    {
        int gold = SaveManager.Instance.playerData.gold;
        goldText.text = string.Format("{0:N0}", gold);
    }

    public override void Clear()
    {
        base.Clear();

        UnregisterListeners();
    }
}

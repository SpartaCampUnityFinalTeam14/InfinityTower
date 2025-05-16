using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour, ScrollPanel
{
    [SerializeField] private TextMeshProUGUI goldText;

    [SerializeField] private Image stageImage;
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private Button leftStageButton;
    [SerializeField] private Button rightStageButton;
    [SerializeField] private Button stageStartButton;

    [SerializeField] private EventChannel OnGoldChanged;

    protected void Awake()
    {
        UpdateGold();

        leftStageButton.onClick.AddListener(() => SetStage(SaveManager.Instance.playerData.selectedStageIndex - 1));
        rightStageButton.onClick.AddListener(() => SetStage(SaveManager.Instance.playerData.selectedStageIndex + 1));
        stageStartButton.onClick.AddListener(StartStage);

        UnregisterListeners();
        RegisterListeners();
    }

    private void Start()
    {
        SetStage(SaveManager.Instance.playerData.selectedStageIndex);
    }

    void SetStage(int id)
    {
        if (id < 0 || id >= DataManager.Instance.stageDict.Count) return;

        SaveManager.Instance.playerData.selectedStageIndex = id;
        SaveManager.Instance.SavePlayerData();

        //stageImage.sprite = 
        stageNameText.text = DataManager.Instance.stageDict[id].name;
    }

    void UnregisterListeners()
    {
        OnGoldChanged.UnregisterListener(UpdateGold);
    }

    void RegisterListeners()
    {
        OnGoldChanged.RegisterListener(UpdateGold);
    }

    public void ResetPanel() => UpdateGold();

    void UpdateGold()
    {
        int gold = SaveManager.Instance.playerData.gold;
        goldText.text = string.Format("{0:N0}", gold);
    }

    void StartStage()
    {
        foreach(int id in SaveManager.Instance.playerData.selectedTowerIndex)
        {
            if(id < 0)
            {
                UIManager.Instance.ShowUI<UI_Alert>().Alert("타워가 전부 편성돼 있어야 합니다.");
                return;
            }
        }

        GameManager.Instance.LoadScene("KSM_Stage");
    }

    private void OnDestroy()
    {
        UnregisterListeners();
    }
}

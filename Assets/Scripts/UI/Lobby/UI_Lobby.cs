using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Lobby : MonoBehaviour, ScrollPanel
{
    [SerializeField] private Image stageImage;
    [SerializeField] private TextMeshProUGUI stageNameText;
    [SerializeField] private Button leftStageButton;
    [SerializeField] private Button rightStageButton;
    [SerializeField] private Button stageStartButton;

    protected void Awake()
    {
        leftStageButton.onClick.AddListener(() => SetStage(SaveManager.Instance.playerData.selectedStageIndex - 1));
        rightStageButton.onClick.AddListener(() => SetStage(SaveManager.Instance.playerData.selectedStageIndex + 1));
        stageStartButton.onClick.AddListener(StartStage);
    }

    private void Start()
    {
        SetStage(SaveManager.Instance.playerData.selectedStageIndex);
    }

    void SetStage(int id)
    {
        if (!SaveManager.Instance.playerData.playableStages.Contains(id)) return;
        if (id < 0 || id >= DataManager.Instance.stageDict.Count) return;

        SaveManager.Instance.playerData.selectedStageIndex = id;
        SaveManager.Instance.SavePlayerData();

        stageImage.sprite = Resources.Load<Sprite>($"Icons/Stage/Stage_{id}");
        stageNameText.text = DataManager.Instance.stageDict[id].name;

        if (SaveManager.Instance.playerData.playableStages.IndexOf(id) <= 0) leftStageButton.gameObject.SetActive(false);
        else leftStageButton.gameObject.SetActive(true);

        if (SaveManager.Instance.playerData.playableStages.IndexOf(id) >= SaveManager.Instance.playerData.playableStages.Count - 1) rightStageButton.gameObject.SetActive(false);
        else rightStageButton.gameObject.SetActive(true);
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

    public void ResetPanel()
    {

    }
}

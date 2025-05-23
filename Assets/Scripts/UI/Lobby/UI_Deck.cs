using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Deck : MonoBehaviour, ScrollPanel
{
    private TabController tabController;

    [SerializeField] private UI_TowerSelect towerSelect;
    [SerializeField] private UI_ChampionSelect championSelect;

    [SerializeField] private RectTransform championBackgroundTransform;
    [SerializeField] private Image championImage;
    [SerializeField] private Button championButton;
    [SerializeField] private TextMeshProUGUI championNameText;

    [SerializeField] private GameObject towerSelectMask;
    [SerializeField] private Button towerSelectMaskButton;

    [SerializeField] private List<UI_SelectedTowerSlot> selectedTowerSlots = new(5);

    [SerializeField] private IntEventChannel OnTowerSelected;
    [SerializeField] private IntEventChannel OnTowerSlotSelected;
    [SerializeField] private IntEventChannel OnChampionSelected;
    [SerializeField] private BoolEventChannel OnScrollStateChanged;

    private int selectedTowerIndex = -1;

    protected void Awake()
    {
        tabController = GetComponent<TabController>();

        towerSelect.deck = this;
        championSelect.deck = this;

        championButton.onClick.AddListener(() => UIManager.Instance.ShowStackUI<UI_ChampionInfo>().Init(SaveManager.Instance.playerData.selectedChampionIndex));
        towerSelectMaskButton.onClick.AddListener(() => 
        {
            towerSelectMask.SetActive(false);
            OnScrollStateChanged.RaiseEvent(true);
            selectedTowerIndex = -1;
        });

        UnregisterListeners();
        RegisterListeners();

        InitSelectedTowers();
        SetChampion(SaveManager.Instance.playerData.selectedChampionIndex);
    }

    void UnregisterListeners()
    {
        OnTowerSelected.UnregisterListener(SelectedTower);
        OnTowerSlotSelected.UnregisterListener(SelectedSlot);
        OnChampionSelected.UnregisterListener(SetChampion);
    }

    void RegisterListeners()
    {
        OnTowerSelected.RegisterListener(SelectedTower);
        OnTowerSlotSelected.RegisterListener(SelectedSlot);
        OnChampionSelected.RegisterListener(SetChampion);
    }

    public void ResetScrolls()
    {
        towerSelect.ResetScroll();
        championSelect.ResetScroll();
    }

    public void ResetPanel() => UpdateTab();

    public void UpdateTab()
    {
        towerSelect.ResetAllSlot();
        championSelect.ResetAllSlot();
    }

    void SetChampion(int index)
    {
        //RotateChampionSlotRandom();

        championImage.sprite = Resources.Load<Sprite>($"Icons/Champion/Champion_{index}");
        championNameText.text = DataManager.Instance.championDict[index].name;
    }

    void RotateChampionSlotRandom()
    {
        float randomRotZ = Random.Range(-5f, 5f);
        Vector3 curRot = championBackgroundTransform.eulerAngles;
        championBackgroundTransform.eulerAngles = new Vector3(curRot.x, curRot.y, randomRotZ);
    }

    void InitSelectedTowers()
    {
        for(int i = 0; i < SaveManager.Instance.playerData.selectedTowerIndex.Count; i++)
        {
            selectedTowerSlots[i].SetSelectedTower(SaveManager.Instance.playerData.selectedTowerIndex[i]);
            selectedTowerSlots[i].Init(i, SaveManager.Instance.playerData.selectedTowerIndex[i], this);
        }
    }

    void SelectedTower(int index)
    {
        selectedTowerIndex = index;
        towerSelectMask.SetActive(true);
        OnScrollStateChanged.RaiseEvent(false);
    }

    void SelectedSlot(int index)
    {
        if (selectedTowerIndex < 0)
        {
            int towerId = selectedTowerSlots[index].towerId;
            if (towerId < 0) return;
            UIManager.Instance.ShowStackUI<UI_TowerInfo>().Init(towerId);
            return;
        }

        SelectTower(index, selectedTowerIndex);
        selectedTowerIndex = -1;

        towerSelectMask.SetActive(false);
        OnScrollStateChanged.RaiseEvent(true);
    }

    void SelectTower(int selectedPos, int selectedTowerIndex)
    {
        int posTowerIndex = selectedTowerSlots[selectedPos].towerId;
        int exIndex = SaveManager.Instance.playerData.AddTower(selectedPos, selectedTowerIndex);//교체된 타워가 원래 있던 슬롯 위치

        if(exIndex != -1)
        {//타워가 swap되어서, 이전 위치가 존재한다면
            //이전 위치의 슬롯을 업데이트시켜줌
            selectedTowerSlots[exIndex].SetSelectedTower(posTowerIndex);
        }

        //현재 선택된 슬롯 업데이트
        selectedTowerSlots[selectedPos].SetSelectedTower(selectedTowerIndex);
    }

    private void OnDestroy()
    {
        UnregisterListeners();

        towerSelect.Clear();
        championSelect.Clear();
    }
}

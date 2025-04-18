using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Deck : UI
{
    [SerializeField] private UI_TowerSelect towerSelect;
    [SerializeField] private UI_ChampionSelect championSelect;

    [SerializeField] private Button closeButton;
    [SerializeField] private Button towerTab;
    [SerializeField] private Button championTab;

    [SerializeField] private List<UI_SelectedTowerSlot> selectedTowerSlots = new(5);
    [SerializeField] private Image championImage;
    [SerializeField] private TextMeshProUGUI championNameText;

    [SerializeField] private GameObject towerSelectMask;
    [SerializeField] private Button towerSelectMaskButton;

    [SerializeField] private IntEventChannel OnTowerSelected;
    [SerializeField] private IntEventChannel OnTowerSlotSelected;
    [SerializeField] private IntEventChannel OnChampionSelected;


    private int selectedTowerIndex = -1;

    protected override void Awake()
    {
        base.Awake();

        towerSelect.deck = this;
        championSelect.deck = this;

        closeButton.onClick.AddListener(() => UIManager.Instance.HideUI<UI_Deck>());
        towerTab.onClick.AddListener(() => SetDeckTab(true));
        championTab.onClick.AddListener(() => SetDeckTab(false));
        towerSelectMaskButton.onClick.AddListener(() => {
            towerSelectMask.SetActive(false);
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

    public void SetDeckTab(bool isTower)
    {
        if (isTower)
        {
            towerSelect.gameObject.SetActive(true);
            championSelect.gameObject.SetActive(false);
        }
        else
        {
            towerSelect.gameObject.SetActive(false);
            championSelect.gameObject.SetActive(true);
        }
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
    }

    void SelectedSlot(int index)
    {
        if (selectedTowerIndex < 0) return;

        SelectTower(index, selectedTowerIndex);
        selectedTowerIndex = -1;

        towerSelectMask.SetActive(false);
    }

    public void SelectTower(int selectedPos, int selectedTowerIndex)
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

    public void SetChampion(int index)
    {
        //스프라이트 세팅해야 함
        championNameText.text = DataManager.Instance.championDict[index].name;
    }

    public override void Clear()
    {
        base.Clear();

        UnregisterListeners();

        towerSelect.Clear();
        championSelect.Clear();
    }
}

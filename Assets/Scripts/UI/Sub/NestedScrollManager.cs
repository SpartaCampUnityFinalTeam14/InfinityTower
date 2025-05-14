using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface ScrollPanel
{
    public void ResetPanel();
}

public class NestedScrollManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private List<GameObject> panels = new();
    [SerializeField] private Scrollbar scrollBar;
    [SerializeField] private List<RectTransform> tabRects;

    [SerializeField] private ScrollChild towerScroll;
    [SerializeField] private ScrollChild championScroll;
    [SerializeField] private ScrollChild artifactScroll;

    [SerializeField] private BoolEventChannel OnScrollStateChanged;

    public int originalTabSize = 180;

    private int count;
    private List<float> panelPoses = new();
    private float distance;
    private float half;

    private bool isDragging;
    private int curIndex = 2;
    private int targetIndex = 2;

    private bool isDragPossible = true;

    private void Awake()
    {
        UnregisterListeners();
        RegisterListeners();

        panelPoses.Clear();

        count = content.childCount;
        distance = 1f / (count - 1);
        half = distance * 0.5f;
        for (int i = count - 1; i >= 0; i--)
        {
            panelPoses.Add(distance * i);
        }

        towerScroll.scrollManager = this;
        championScroll.scrollManager = this;
        artifactScroll.scrollManager = this;
    }

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        scrollBar.value = panelPoses[targetIndex];
    }

    private void Update()
    {
        if (!isDragging) scrollBar.value = Mathf.Lerp(scrollBar.value, panelPoses[targetIndex], Time.deltaTime * 10f);
    }

    public void UnregisterListeners()
    {
        OnScrollStateChanged.UnregisterListener(SetDragPossible);
    }

    void RegisterListeners()
    {
        OnScrollStateChanged.RegisterListener(SetDragPossible);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDragPossible) return;

        isDragging = true;

        curIndex = FindPos();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragPossible) return;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragPossible) return;

        isDragging = false;

        if (curIndex == FindPos())
        {//드래그로 패널 절반 안 넘어가도 드래그 속도 빠르면 넘기는 용도
            float deltaY = eventData.delta.y;

            if (deltaY > 18 && curIndex < count - 1)
            {//아래로 드래그
                ChangeTab(targetIndex + 1);
            }
            else if (deltaY < -18 && curIndex > 0)
            {//위로 드래그
                ChangeTab(targetIndex - 1);
            }
        }
        else SetPos(ref targetIndex);
    }

    int FindPos()
    {
        for (int i = count - 1; i >= 0; i--)
        {
            if (panelPoses[i] - half <= scrollBar.value && scrollBar.value <= panelPoses[i] + half)
            {
                return i;
            }
        }

        return curIndex;
    }

    void SetPos(ref int Index)
    {//드래그로 패널 절반 넘어갔는지 확인 용도
        for (int i = count - 1; i >= 0; i--)
        {
            if (panelPoses[i] - half < scrollBar.value && scrollBar.value < panelPoses[i] + half)
            {
                ChangeTab(i);
                Index = i;
                return;
            }
        }
    }

    public void OnTabClicked(int index)
    {
        if (targetIndex == index) return;

        ChangeTab(index);
    }

    public void ChangeTab(int index)
    {
        if (!isDragPossible) return;

        RectTransform curButton = tabRects[targetIndex];
        curButton.DOSizeDelta(new Vector2(curButton.sizeDelta.x, originalTabSize), 0.3f)
            .SetEase(Ease.OutQuart);
        curButton.GetComponent<Image>().DOColor(Color.white, 0.3f);
        curButton.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 48;
        curButton.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Normal;

        targetIndex = index;

        RectTransform targetButton = tabRects[targetIndex];
        targetButton.DOSizeDelta(new Vector2(targetButton.sizeDelta.x, originalTabSize * 2), 0.3f)
            .SetEase(Ease.OutQuart);
        targetButton.GetComponent<Image>().DOColor(new Color(239 / 255f, 189 / 255f, 137 / 255f), 0.3f);
        targetButton.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 72;
        targetButton.GetChild(0).GetComponent<TextMeshProUGUI>().fontStyle = TMPro.FontStyles.Bold;

        panels[targetIndex].GetComponent<ScrollPanel>().ResetPanel();

        ResetScrollChilds();
    }

    void ResetScrollChilds()
    {
        towerScroll.ResetScroll();
        championScroll.ResetScroll();
        artifactScroll.ResetScroll();
    }

    void SetDragPossible(bool flag)
    {
        isDragPossible = flag;
        scrollRect.enabled = flag;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITowerInfo : UI
{
    [Header("TowerInfo")]
    [SerializeField] GameObject bg;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI statInfo;
    [SerializeField] Button btnClose;
    [SerializeField] Button btnUpgrade;
    [SerializeField] Button btnRemove;

    BaseTower seletedTower;

    public void Init(Transform seleted, TowerData data)
    {
        SetUIPos(seleted);

        seletedTower = seleted.GetComponent<BaseTower>();
        
        title.text = data.name;
        description.text = data.description;
        
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < data.statTypes.Count; i++)
        {
            sb.AppendLine($"{(StatType)data.statTypes[i]}: {data.statValue[i]}");
        }
        statInfo.text = sb.ToString();
    }

    public void OnCloseClick()
    {
        seletedTower.CloseTowerInfo();
        Hide();
    }

    public void OnUpgradeClick()
    {
        seletedTower.UpgradeTower();
    }

    public void OnRemoveClick()
    {
        seletedTower.RemoveTower();

        // 지웠으면 닫아주기
        OnCloseClick();
    }

    void SetUIPos(Transform seleted)
    {
        RectTransform canvasRect = GetComponent<RectTransform>();
        RectTransform rectBg = bg.GetComponent<RectTransform>();
        
        Vector2 screenPos = Camera.main.WorldToScreenPoint(seleted.position);

        // 스크린 좌표 - 로컬 좌표 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,
            out Vector2 localPos
        );

        // UI 오브젝트 위치 설정
        CheckUIOutOfVeiw(rectBg, localPos, new Vector2(400, -200));
        CheckUIOutOfVeiw(btnUpgrade.GetComponent<RectTransform>(), localPos, new Vector2(-100, 100));
        CheckUIOutOfVeiw(btnRemove.GetComponent<RectTransform>(), localPos, new Vector2(100, 100));
    }

    void CheckUIOutOfVeiw(RectTransform rc, Vector2 localPos, Vector2 offsetPos)
    {
        RectTransform canvasRect = GetComponent<RectTransform>();
        Vector3 screenPos = canvasRect.TransformPoint(localPos + offsetPos);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        // UI 중심의 뷰포트 위치
        Vector2 viewCenter = Camera.main.WorldToViewportPoint(worldPos);

        // UI의 절반 크기를 뷰포트 단위로 환산
        Vector2 scaleFactor = new Vector2(
           rc.lossyScale.x / Camera.main.pixelWidth,
           rc.lossyScale.y / Camera.main.pixelHeight
        );

        Vector2 viewHalfSize = new Vector2(
            (rc.sizeDelta.x * 0.5f) * scaleFactor.x,
            (rc.sizeDelta.y * 0.5f) * scaleFactor.y
        );

        // x축 범위 체크
        if (viewCenter.x - viewHalfSize.x < 0 || viewCenter.x + viewHalfSize.x > 1)
        {
            offsetPos.x *= -1f;
        }

        // y축 범위 체크
        if (viewCenter.y - viewHalfSize.y < 0 || viewCenter.y + viewHalfSize.y > 1)
        {
            offsetPos.y *= -1f;
        }

        rc.localPosition = localPos + offsetPos;
    }
}

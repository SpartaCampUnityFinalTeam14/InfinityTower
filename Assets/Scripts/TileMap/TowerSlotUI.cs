using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI 연결")]
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text costText;

    [Header("타워 설정")]
    private int towerID;
    private GameObject previewPrefab;
    private GameObject placedTowerPrefab;

    private GameObject previewObj;
    
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TMP_Text cooldownText;
    
    [SerializeField] private GameObject coverOverlay;
    
    private float cooldownDuration;
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    private float requiredCost;
    
    GameObject rangePrefab;
    RangeIndicator rangeIndicator;
    protected TowerData towerData;

    [SerializeField] private EventChannel OnResetTowerCoolDown;

    private void Awake()
    {
        rangePrefab = Resources.Load<GameObject>("Prefabs/Tower/RangeIndicator");

        UnregisterListeners();
        RegisterListeners();
    }

    void UnregisterListeners()
    {
        OnResetTowerCoolDown.UnregisterListener(ResetCooldown);
    }

    void RegisterListeners()
    {
        OnResetTowerCoolDown.RegisterListener(ResetCooldown);
    }

    public void Init(int id)
    {
        towerID = id;
        cooldownDuration = DataManager.Instance.towerDict[towerID].GetStatValue(StatType.towerCooldown);
        requiredCost = DataManager.Instance.towerDict[towerID].GetStatValue(StatType.cost);
        towerData = DataManager.Instance.towerDict[towerID];

        cooldownOverlay.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
        
        // 이름 단순 표시 (원하면 Resources/타워 데이터로 확장 가능)
        nameText.text = DataManager.Instance.towerDict[id].name;
        costText.text = requiredCost.ToString();

        // 프리팹 로드 (예: Prefabs/TowerGhost_1, Prefabs/Tower_1)
        Sprite icon = Resources.Load<Sprite>($"Icons/Tower/Tower_{towerID}");
        if (icon)
            iconImage.sprite = icon;

        previewPrefab = Resources.Load<GameObject>($"Prefabs/Tower/TowerGhost_{towerID}");
        placedTowerPrefab = Resources.Load<GameObject>($"Prefabs/Tower/Tower_{towerID}");

        if (previewPrefab == null)
            Debug.LogError($"❌ previewPrefab 로드 실패: TowerGhost_{towerID}");

        if (placedTowerPrefab == null)
            Debug.LogError($"❌ placedTowerPrefab 로드 실패: Tower_{towerID}");
        
    }
    
    private void Update()
    {
        cooldownTimer -= Time.deltaTime;
        float ratio = Mathf.Clamp01(cooldownTimer / cooldownDuration);

        cooldownOverlay.fillAmount = ratio;
        cooldownText.text = $"{cooldownTimer:F1}s";

        // 3️⃣ 쿨타임 종료 처리
        if (cooldownTimer <= 0f)
        {
            isOnCooldown = false;

            cooldownOverlay.gameObject.SetActive(false);
            cooldownText.gameObject.SetActive(false);

            // 회전 리셋
            iconImage.transform.rotation = Quaternion.identity;
        }
        
        // 💰 코스트 부족 시 어둡게 처리
        bool isEnough = StageManager.Instance.CurrentCost >= requiredCost;
        
        // 커버로 막아주기
        coverOverlay.SetActive(!isEnough);
    }

    public void ResetCooldown()
    {
        cooldownTimer = 0f;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsCostEnough())
        {
            Debug.LogWarning("❌ 코스트 부족으로 드래그 취소");
            return;
        }
        
        if (isOnCooldown)
        {
            Debug.LogWarning("⏳ 쿨타임 중입니다!");
            return;
        }

        if (previewPrefab == null)
        {
            Debug.LogError("❌ previewPrefab이 null입니다!!");
            return;
        }

        // ✅ previewObj 인스턴스 생성
        previewObj = Instantiate(previewPrefab);
        if (previewObj != null)
        {
            Vector3 spawnPos = previewObj.transform.position;
            spawnPos.z = 0f;
            previewObj.transform.position = spawnPos;

            // ✅ 사거리 표시 RangeIndicator를 previewObj의 자식으로 붙임
            var go = PoolManager.Instance.Get(rangePrefab, 1, previewObj.transform); // 🔥 포인트
            rangeIndicator = go.GetComponent<RangeIndicator>();

            if (rangeIndicator == null)
            {
                Debug.LogError("❌ RangeIndicator 컴포넌트가 없습니다!");
                return;
            }

            rangeIndicator.gameObject.SetActive(true);
            rangeIndicator.Init(towerData.GetStatValue(StatType.attackRange) / previewObj.transform.localScale.x);
        }

        TilemapManager.Instance.ShowAllPlaceableCells();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (previewObj == null)
            return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;
        previewObj.transform.position = worldPos;

        Vector3Int cellPos = TilemapManager.Instance.tilemap.WorldToCell(worldPos);
        bool canPlace = TilemapManager.Instance.CanPlaceAt(cellPos);

        var renderers = previewObj.GetComponentsInChildren<SpriteRenderer>();
        foreach (var r in renderers)
        {
            if (r.GetComponentInParent<RangeIndicator>() != null) continue;
            r.color = canPlace ? new Color(0f, 1f, 0f, 0.8f) : new Color(1f, 0f, 0f, 0.8f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3Int cellPos = TilemapManager.Instance.tilemap.WorldToCell(worldPos);
        cellPos.z = 0;

        if (TilemapManager.Instance.CanPlaceAt(cellPos))
        {
            if (IsCostEnough() && !isOnCooldown)
            {
                Vector3 spawnPos = TilemapManager.Instance.tilemap.CellToWorld(cellPos) +
                                   TilemapManager.Instance.tilemap.cellSize / 2;

                if(IsCostEnough()) 
                    StageManager.Instance.UseCost((int)DataManager.Instance.towerDict[towerID].GetStatValue(StatType.cost));

                var tower = PoolManager.Instance.Get(placedTowerPrefab).GetComponent<BaseTower>();
                tower.transform.position = spawnPos;
                
                // 셀 정보 타워에 전달
                tower.SetCellPos(cellPos);

                // 타워 정보 저장
                StageManager.Instance.CurFloor.AddTowerInfo(tower);

                // 셀 등록
                TilemapManager.Instance.RegisterOccupiedCell(cellPos);

                StartCooldown();
            }
        }


        if (previewObj != null)
            Destroy(previewObj);
        
        TilemapManager.Instance.ClearIndicators();
    }
    
    bool IsCostEnough()
    {
        int cost = (int)DataManager.Instance.towerDict[towerID].GetStatValue(StatType.cost);

        return StageManager.Instance.CheckCost(cost);
    }
    
    private void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownDuration;

        cooldownOverlay.fillAmount = 1f;
        cooldownOverlay.gameObject.SetActive(true);
        cooldownText.gameObject.SetActive(true);
        cooldownText.text = Mathf.CeilToInt(cooldownTimer).ToString();
    }
    private void OnDestroy()
    {
        UnregisterListeners();
    }
}

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

    public void Init(int id)
    {
        towerID = id;
        cooldownDuration = DataManager.Instance.towerDict[towerID].GetStatValue(StatType.towerCooldown);
        requiredCost = DataManager.Instance.towerDict[towerID].GetStatValue(StatType.cost);

        // 슬롯 정보 StageManager에 전달
        StageManager.Instance.AddTowerSlot(this);

        cooldownOverlay.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);
        
        // 이름 단순 표시 (원하면 Resources/타워 데이터로 확장 가능)
        nameText.text = $"타워 {towerID}";

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

        previewObj = Instantiate(previewPrefab);

        if (previewObj != null)
        {
            Vector3 spawnPos = previewObj.transform.position;
            spawnPos.z = 0f;
            previewObj.transform.position = spawnPos;
        }
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
            r.color = canPlace ? new Color(0f, 1f, 0f, 0.8f) : new Color(1f, 0f, 0f, 0.8f); // 초록 or 빨강
        }   
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector3Int cellPos = TilemapManager.Instance.tilemap.WorldToCell(worldPos);

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

}

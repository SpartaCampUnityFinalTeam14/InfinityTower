using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image iconImage;
    public TMP_Text nameText;
    
    public Image cooldownOverlay; // ⏳ 회색 오버레이 (Fill)
    public TMP_Text cooldownText; // 🕒 쿨타임 텍스트

    public GameObject rangePreviewPrefab; // ✅ 범위 미리보기 프리팹 (반투명 원)

    private Skill skill;
    private ISkillUser caster;

    private GameObject rangePreviewObj;   // ✅ 드래그 중 표시할 범위 원
    private TargetPositionSkill tpSkill;  // ✅ 캐스팅한 위치 지정 스킬 저장
    
    private ActiveSkill activeSkill;

    public void Init(Skill skill, ISkillUser caster)
    {
        this.skill = skill;
        this.caster = caster;

        nameText.text = skill.skillName;
        iconImage.sprite = Resources.Load<Sprite>($"Icons/{skill.skillName}");

        if (skill is ActiveSkill active)
            activeSkill = active;

        if (skill is AutoTargetSkill)
            GetComponent<Button>().onClick.AddListener(() => skill.Use(caster));
    }
    
    private void Update()
    {
        if (activeSkill == null) return;

        if (activeSkill.CanUse())
        {
            cooldownOverlay.fillAmount = 0f;
            cooldownText.text = "";
        }
        else
        {
            float remain = activeSkill.RemainingCooldown;
            cooldownOverlay.fillAmount = remain / activeSkill.cooldown;
            cooldownText.text = $"{remain:F1}s";
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("🧪 스킬 드래그 시작");
        
        if (skill is not TargetPositionSkill targetSkill) return;

        tpSkill = targetSkill;
        
        // ✅ 쿨타임 검사 추가
        if (!tpSkill.CanUse())
        {
            Debug.LogWarning($"⛔ {tpSkill.skillName}은(는) 쿨타임 중입니다. 남은 시간: {tpSkill.RemainingCooldown:F1}s");
            tpSkill = null; // 사용 안함
            return;
        }
        
        if (rangePreviewPrefab != null)
        {
            Debug.Log("🧪 범위 미리보기 프리팹 로드 성공");
            rangePreviewObj = Instantiate(rangePreviewPrefab);
            float radius = tpSkill.range;
        
            // ✅ 스프라이트 크기가 지름 1일 경우 → scale = (range*2)
            rangePreviewObj.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rangePreviewObj == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;
        rangePreviewObj.transform.position = worldPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (rangePreviewObj != null)
            Destroy(rangePreviewObj);

        if (tpSkill == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
        worldPos.z = 0;

        bool success = tpSkill.TryStartSkill(caster, worldPos); // ✅ 쿨타임 포함 실행

        if (success)
            Debug.Log("✅ 스킬 정상 실행됨");

        tpSkill = null;
    }
}

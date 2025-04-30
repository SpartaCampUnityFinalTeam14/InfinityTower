using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;



public class UIAbility : UI
{
    //[SerializeField] Transform layout;
    [SerializeField] List<AbilitySlot> abilitySlots;
    [SerializeField] List<RectTransform> listEndPos;
    [SerializeField] float slideInterval = 0.5f;
    [SerializeField] float slideDuration = 1f;

    Vector3[] arrstartPos;
    WaitForSecondsRealtime wait;
    const string prefabPath = "Ability/AbilitySlot";

    private void Start()
    {
        arrstartPos = new Vector3[abilitySlots.Count];
        for (int i = 0; i < abilitySlots.Count; i++)
        {
            arrstartPos[i] = abilitySlots[i].transform.position;
        }

        wait = new WaitForSecondsRealtime(1f);
    }

    public override void Show()
    {
        base.Show();

        StageManager.Instance.timeScaleManager.PushTimeScale(0f);

        StageManager.Instance.CurFloor.isPerkSelected = false;
    }

    public override void Hide()
    {
        base.Hide();

        StageManager.Instance.timeScaleManager.PopTimeScale();

        StageManager.Instance.CurFloor.isPerkSelected = true;
    }

    public void DrawAbility(int drawCount = 3)
    {
        AbilityData data;
        List<AbilityData> listDraw = new List<AbilityData>();
        
        while (listDraw.Count < drawCount)
        {
            // 레어도
            int rarity = StageManager.Instance.abilityManager.GetRandomRarity();

            // 특성 뽑기
            data = StageManager.Instance.abilityManager.GetRandomAbility(rarity);

            if (data == null) continue;

            // 이미 뽑은 특성인지 중복 체크
            if (!listDraw.Contains(data))
                listDraw.Add(data);
        }

        // 특성 생성
        InitAbilitySlot(listDraw);
    }

    void InitAbilitySlot(List<AbilityData> datas)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            // 특성 UI 초기화
            abilitySlots[i].Init(datas[i]);

            // 특성 시작 위치 초기화
            abilitySlots[i].transform.position = arrstartPos[i];

            // 이벤트 구독
            abilitySlots[i].actionClick += SeletedAbility;
        }

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        // 0.5초 대기
        seq.AppendInterval(slideInterval)
            // 슬롯 1번 배치
            .Append(abilitySlots[0].transform.DOMove(listEndPos[0].transform.position, slideDuration))
            // 1초 후 슬롯 2번 배치
            .Insert(slideInterval * 2, abilitySlots[1].transform.DOMove(listEndPos[1].transform.position, slideDuration))
            // 1.5초후 슬롯 3번 배치
            .Insert(slideInterval * 3, abilitySlots[2].transform.DOMove(listEndPos[2].transform.position, slideDuration))
            // 슬롯이 전부 올라오면 특성 선택 활성화
            .AppendCallback(() =>
            {
                abilitySlots[0].EnabledButton(true);
                abilitySlots[1].EnabledButton(true);
                abilitySlots[2].EnabledButton(true);
            });
    }

    public void SeletedAbility(AbilitySlot selected)
    {
        // 매니저에 특성 추가
        StageManager.Instance.abilityManager.AddAbillity(selected.Data);

        // 특성 종료 연출
        StartCoroutine(CloseAbilityUI(selected));
    }

    IEnumerator CloseAbilityUI(AbilitySlot selected)
    {
        for (int i = 0; i < abilitySlots.Count; i++)
        {
            abilitySlots[i].EnabledButton(false);

            if (abilitySlots[i].Equals(selected))
                abilitySlots[i].EnabledOutline(true);
            else
                abilitySlots[i].FadeOut();

        }

        yield return wait;

        Hide();
    }
}

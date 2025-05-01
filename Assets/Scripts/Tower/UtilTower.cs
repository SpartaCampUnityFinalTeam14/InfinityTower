using System.Collections;
using UnityEngine;

public class UtilityTower : BaseTower
{
    private float buffAmount;
    private float buffDuration;
    private Coroutine buffCoroutine;

    protected override void Start()
    {
        base.Start();
        foreach (EffectBase T in myEffect)
        {
            if (T.statusID != (int)StatType.costHeal) continue;
            float[] effectValues = towerData.effectInfo[towerData.effectID.IndexOf(T.statusID)];
            buffAmount = effectValues[0];   // ex: +0.5
            buffDuration = effectValues[1];    // ex: 5초
        }
    }

    protected override void Activate()
    {
        // 여러 타워가 동시에 영향을 미칠 수 있게 조정
        if (buffCoroutine == null)
        {
            buffCoroutine = StartCoroutine(ApplyCostRecoveryBuff());
        }
    }

    IEnumerator ApplyCostRecoveryBuff()
    {
        // 타워가 추가되면 버프를 적용, 중복되는 버프는 기존값에 누적 적용
        StageManager.Instance.AddCostRecoveryMultiplier(buffAmount);

        float timer = 0f;
        while (timer < buffDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        StageManager.Instance.RemoveCostRecoveryMultiplier(buffAmount);
        buffCoroutine = null;
    }

    private void OnDestroy()
    {
        // 타워가 제거되면 코스트 버프도 제거
        if (buffCoroutine != null)
        {
            StopCoroutine(buffCoroutine);
            StageManager.Instance.RemoveCostRecoveryMultiplier(buffAmount);
            buffCoroutine = null;
        }
    }
}

using DG.Tweening;
using System.Collections;
using UnityEngine;

public class UI_FloorLoading : UI
{
    [Header("Setting")]
    [SerializeField] float openShopDelay;
    [SerializeField] float startDelay;
    [SerializeField] float endDelay;
    [SerializeField] float moveDuration;
    [SerializeField] float fadeInDuration;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform parent;

    [Header("MovePath")]
    [SerializeField] RectTransform startPos;
    [SerializeField] RectTransform middlePos;
    [SerializeField] RectTransform endPos;

    GameObject player;
    RectTransform playerRect;
    Animator anim;

    public override void Show()
    {
        base.Show();

        if (!player)
        {
            GameObject GO = Resources.Load<GameObject>($"Prefabs/Hero/Character_{StageManager.Instance.selectedChampion}");
            if (!GO) Debug.LogError("Player Load Error");

            player = Instantiate(GO, parent);
            anim = player.GetComponentInChildren<Animator>();
            playerRect = player.GetComponent<RectTransform>();
        }

        StageManager.Instance.timeScaleManager.PushTimeScale(0f);

        canvasGroup.alpha = 1f;
        playerRect.position = startPos.position;

        StartCoroutine(DelayOpenShop());
    }

    public void SequenceStart()
    {
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        seq.AppendInterval(startDelay);

        seq.AppendCallback(() => anim.SetTrigger("Move"));
        seq.Append(playerRect.DOMove(middlePos.position, moveDuration).SetEase(Ease.Linear));
        seq.Append(playerRect.DOMove(endPos.position, moveDuration).SetEase(Ease.Linear));
        seq.AppendInterval(endDelay);

        seq.Append(canvasGroup.DOFade(0f, fadeInDuration).OnComplete(() =>
        {
            Hide();

            StageManager.Instance.isIntroEnd = true;
            StageManager.Instance.timeScaleManager.PopTimeScale();
        }));

    }

    IEnumerator DelayOpenShop()
    {
        yield return new WaitForSecondsRealtime(openShopDelay);

        UIManager.Instance.ShowUI<UI_Shop>();
    }
}

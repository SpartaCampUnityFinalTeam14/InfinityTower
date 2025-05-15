using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollChild : ScrollRect
{
    private bool isMovingVertical;
    public NestedScrollManager scrollManager;
    private ScrollRect parentScrollRect;

    protected override void Start()
    {
        if(Application.isPlaying) parentScrollRect = scrollManager.GetComponent<ScrollRect>();

        base.Start();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        isMovingVertical = Mathf.Abs(eventData.delta.x) < Mathf.Abs(eventData.delta.y);


        if (isMovingVertical)
        {
            scrollManager.OnBeginDrag(eventData);
            parentScrollRect.OnBeginDrag(eventData);
        }
        else base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (isMovingVertical)
        {
            scrollManager.OnDrag(eventData);
            parentScrollRect.OnDrag(eventData);
        }
        else base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (isMovingVertical)
        {
            scrollManager.OnEndDrag(eventData);
            parentScrollRect.OnEndDrag(eventData);
        }
        else base.OnEndDrag(eventData);
    }

    public void ResetScroll()
    {
        this.horizontalScrollbar.value = 0;
    }
}

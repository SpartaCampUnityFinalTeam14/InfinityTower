using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;


public abstract class BaseTower : Poolable
{
    public TowerData towerData;
    public float cooldownTimer;

    [SerializeField] protected Animator anim;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    GameObject rangePrefab;
    RangeIndicator rangeIndicator;

    protected virtual void Awake()
    {
        rangePrefab = Resources.Load<GameObject>("Prefabs/Tower/RangeIndicator");
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Initialize(TowerData data)
    {
        towerData = data;
        cooldownTimer = 0f;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        ShowTowerInfo();

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0)
        {
            cooldownTimer = towerData.coolTime;
            Activate();
        }
    }

    public abstract void Activate(); //실제행동은 하위 클래스에서 정의

    public void UpgradeTower()
    {
        Debug.Log("타워 업그레이드");
        // 타워 레벨업 로직
    }

    public void RemoveTower()
    {
        StageManager.Instance.GetCost(towerData.cost);
        PoolManager.Instance.Release(this);
    }

    public void CloseTowerInfo()
    {
        PoolManager.Instance.Release(rangeIndicator);
        Time.timeScale = 1f;
    }

    void ShowTowerInfo()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.GetComponent<BaseTower>().Equals(this))
            {
                var ui = UIManager.Instance.ShowUI<UITowerInfo>();
                ui.Init(transform, towerData);

                // 사거리표시
                rangeIndicator = PoolManager.Instance.Get(rangePrefab, 1, transform).GetComponent<RangeIndicator>();
                rangeIndicator.Init(towerData.range);

                Time.timeScale = 0.2f;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, towerData.range);
    }
}

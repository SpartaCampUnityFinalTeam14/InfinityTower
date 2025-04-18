using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private float speed = 30f;
    private float damage;
    private Vector2 lastDirection;
    private bool isTargetLost = false;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    void Update()
    {
        // 타겟이 존재할 경우 > 추적
        if (!isTargetLost && target != null && target.gameObject.activeInHierarchy)
        {
            lastDirection = (target.position - transform.position).normalized;

            transform.Translate(lastDirection * speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target.position) < 0.2f)
            {
                MonsterBase monster = target.GetComponent<MonsterBase>();

                if (monster != null)
                {
                    monster.TakeDamage(damage);
                }

                Destroy(gameObject);
            }
        }

        else
        {
            // 타겟 잃었을 경우 > 마지막 방향으로 직진
            isTargetLost = true;
            transform.Translate(lastDirection * speed * Time.deltaTime);

            // 몇 초 후 자연 소멸
            StartCoroutine(FadeAndDestroy());
        }

        //Vector2 dir = (target.position - transform.position).normalized;
        //transform.Translate(dir * speed * Time.deltaTime);

        //if (Vector2.Distance(transform.position, target.position) < 0.2f)
        //{
        //    MonsterBase monster = target.GetComponent<MonsterBase>();

        //    if (monster != null)
        //    {
        //        monster.TakeDamage(damage);
        //    }

        //    Destroy(gameObject);
        //}
    }

    IEnumerator FadeAndDestroy()
    {
        // 코루틴 중복 방지
        if (GetComponent<SpriteRenderer>().color.a < 1f)
            yield break;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color color = sr.color;

        //yield return new WaitForSeconds(0.5f); // 조금 날아간 뒤 사라짐 시작

        for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
        {
            color.a = Mathf.Lerp(1, 0, t);
            sr.color = color;
            yield return null;
        }

        Destroy(gameObject);
    }

}

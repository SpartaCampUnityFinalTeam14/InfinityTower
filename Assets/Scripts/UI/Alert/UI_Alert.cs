using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Alert : UI
{
    [SerializeField] private GameObject alertOriginal;
    
    public void Alert(string text)
    {
        PoolManager.Instance.Get(alertOriginal, 5, transform).GetComponent<UI_AlertPrefab>().Init(text);
    }
}

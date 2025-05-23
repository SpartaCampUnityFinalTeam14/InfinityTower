using UnityEngine;
using UnityEngine.EventSystems;
public class Scene : MonoBehaviour
{
    protected void Awake()
    {
        Init();
        QualitySettings.vSyncCount = 0;

        // targetFrameRate가 설정 안 됐거나 60 미만이면 60으로 설정
        if (Application.targetFrameRate < 60 || Application.targetFrameRate == -1)
        {
            Application.targetFrameRate = 60;
        }
    }

    protected virtual void Init()
    {
        Clear();

        if (FindObjectOfType<EventSystem>() == null) Util.InstantiatePrefab("UI/EventSystem");
    }

    public virtual void Clear()
    {
        
    }
}

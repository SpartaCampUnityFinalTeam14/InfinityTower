using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AnalyticsManager : MonoBehaviour
{
    private static bool isInitialized = false;
    private static AnalyticsManager instance;

    // Unity 실행 시 자동으로 AnalyticsManager 생성
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitOnLoad()
    {
        if (instance == null)
        {
            GameObject analyticsObj = new GameObject("@AnalyticsManager");
            instance = analyticsObj.AddComponent<AnalyticsManager>();
            DontDestroyOnLoad(analyticsObj);
        }
    }

    private async void Awake()
    {
        if (!isInitialized)
        {
            await InitializeAnalytics();
        }
    }

    private async Task InitializeAnalytics()
    {
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            isInitialized = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Analytics Initialization Failed: {e.Message}");
        }
    }

    /// <summary>
    /// 커스텀 이벤트 전송 (Dictionary 포함)
    /// </summary>
    public static void SendEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("⚠️ Analytics not initialized.");
            return;
        }

        if (parameters == null)
            parameters = new Dictionary<string, object>();

        AnalyticsService.Instance.CustomData(eventName, parameters);
        Debug.Log($"📤 Sent Custom Event: {eventName}");
    }
}
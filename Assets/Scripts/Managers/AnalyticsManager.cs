using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    private static bool isInitialized = false;
    private static AnalyticsManager instance;

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

    protected override async void Awake()
    {
        base.Awake();

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

    /// <summary>
    /// 세션 및 유저 관련 강제 초기화 (테스트용)
    /// </summary>
    public static void ResetAnalyticsSession()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("⚠️ Analytics not initialized. Cannot reset session.");
            return;
        }

        AnalyticsService.Instance.StopDataCollection();
        PlayerPrefs.DeleteKey("unity.player_session_id");
        PlayerPrefs.Save();

        AnalyticsService.Instance.StartDataCollection();
        Debug.Log("🔄 Analytics session forcibly reset");
    }
}

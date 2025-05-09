using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FTPDownloadUI : MonoBehaviour
{
    public Image progressFill;
    public TextMeshProUGUI progressText;
    
    private FTPFolderDownloader downloader;

    private async void Start()
    {
        progressFill.fillAmount = 0;
        // downloader가 유니티 인스펙터에서 연결되지 않은 경우 자동 할당
        if (downloader == null)
            downloader = FindObjectOfType<FTPFolderDownloader>();

        if (downloader != null)
        {
            Debug.Log("▶️ 다운로드 시작 요청");
            await downloader.StartDownload();
        }
        else
        {
            Debug.LogError("❌ FTPFolderDownloader가 씬에 없음");
        }
    }
    
    private void OnEnable()
    {
        FTPFolderDownloader.OnDownloadProgress += UpdateProgress;
    }

    private void OnDisable()
    {
        FTPFolderDownloader.OnDownloadProgress -= UpdateProgress;
    }

    private void UpdateProgress(float progress)
    {
        progressFill.fillAmount = progress;
        progressText.text = $"{progress * 100f:0.0}%";
    }
}
using System.IO;
using UnityEngine;

[System.Serializable]
public class FTPConfig
{
    public string host;
    public string username;
    public string password;
    
    public static FTPConfig LoadFTPConfig()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "ftp_config.json");

        if (!File.Exists(path))
        {
            Debug.LogError("❌ ftp_config.json이 StreamingAssets에 없습니다.");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<FTPConfig>(json);
    }
}

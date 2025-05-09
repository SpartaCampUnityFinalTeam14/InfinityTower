using System;
using UnityEngine;
using FluentFTP;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

public class FTPFolderDownloader : MonoBehaviour
{
    public static event Action<float> OnDownloadProgress;

    public async Task StartDownload()
    {
        await DownloadAllAsync(); // 🔥 이렇게 해야 정상적으로 await 체인이 작동
    }

    private async Task DownloadAllAsync()
    {
        FTPConfig config = FTPConfig.LoadFTPConfig();
        if (config == null) return;

        string[] rootFolders = { "/Arts", "/External" };
        string unityAssetsPath = Application.dataPath;
        var client = new FtpClient(config.host, new NetworkCredential(config.username, config.password));
        client.Encoding = System.Text.Encoding.UTF8;
        client.EncryptionMode = FtpEncryptionMode.None;
        client.DataConnectionType = FtpDataConnectionType.PASV;

        try
        {
            await client.ConnectAsync();

            foreach (string remoteFolder in rootFolders)
            {
                string localPath = Path.Combine(unityAssetsPath, Path.GetFileName(remoteFolder));
                await DownloadDirectoryRecursive(client, remoteFolder, localPath);
            }
            Debug.Log("✅ 모든 FTP 폴더 다운로드 완료");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall += UnityEditor.AssetDatabase.Refresh;
            #endif
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ FTP 연결 또는 다운로드 실패: " + ex.Message);
        }
        finally
        {
            client.Disconnect();
        }
    }


    private async Task DownloadDirectoryRecursive(FtpClient client, string remotePath, string localPath)
    {
        int downloaded = 0;
        
        var listing = await client.GetListingAsync(remotePath, FtpListOption.Recursive);
        int totalFiles = listing.Count(item => item.Type == FtpFileSystemObjectType.File); // 파일만

        foreach (var item in listing)
        {
            string relativePath = item.FullName.Substring(remotePath.Length).TrimStart('/');
            string localFullPath = Path.Combine(localPath, relativePath).Replace("/", Path.DirectorySeparatorChar.ToString());

            if (item.Type == FtpFileSystemObjectType.Directory)
            {
                Directory.CreateDirectory(localFullPath);
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localFullPath));
                await client.DownloadFileAsync(localFullPath, item.FullName);
                downloaded++;

                float progress = (float)downloaded / totalFiles;
                // UI 쪽으로 넘기기
                Debug.Log("asdasd");
                OnDownloadProgress?.Invoke(progress);
            }
        }
    }
}

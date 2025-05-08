#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using FluentFTP;
using System.Net;
using System.IO;
using System;

public class FTPFolderUploader : EditorWindow
{
    private static FtpClient client;
    private static string localRootPath;
    private static string remoteRootPath;
    private static string[] files;
    private static int currentIndex = 0;
    private static bool isUploading = false;
    private static string currentFile = "";

    [MenuItem("Tools/Upload Folder to FTP")]
    public static void UploadFolderToFTP()
    {
        localRootPath = EditorUtility.OpenFolderPanel("업로드할 폴더 선택", Application.dataPath, "");
        if (string.IsNullOrEmpty(localRootPath))
        {
            Debug.Log("🚫 폴더 선택 취소됨");
            return;
        }

        remoteRootPath = "/" + Path.GetFileName(localRootPath); // 루트 포함

        files = Directory.GetFiles(localRootPath, "*", SearchOption.AllDirectories);
        currentIndex = 0;
        
        FTPConfig config = FTPConfig.LoadFTPConfig();

        client = new FtpClient(config.host, new NetworkCredential(config.username, config.password));
        client.EncryptionMode = FtpEncryptionMode.None;
        client.DataConnectionType = FtpDataConnectionType.PASV;
        client.ConnectTimeout = 5000;
        client.ReadTimeout = 5000;
        client.SocketKeepAlive = true;
        client.RetryAttempts = 1;

        try
        {
            client.Connect();
            Debug.Log("✅ FTP 연결 성공");
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ FTP 연결 실패: " + ex.Message);
            return;
        }

        isUploading = true;
        GetWindow<FTPFolderUploader>("FTP Upload").Show();
        EditorApplication.update += UploadNextFile;
    }

    void OnGUI()
    {
        if (!isUploading)
        {
            GUILayout.Label("⏳ 폴더 선택 후 업로드 시작하세요", EditorStyles.boldLabel);
            return;
        }

        GUILayout.Label("📤 FTP 업로드 중...", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("현재 파일:", currentFile);
        float progress = (float)currentIndex / Mathf.Max(files.Length, 1);
        EditorGUILayout.LabelField($"진행률: {currentIndex} / {files.Length}");
        EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), progress, $"{(int)(progress * 100)}%");
        Repaint();
    }

    private static void UploadNextFile()
    {
        if (currentIndex >= files.Length)
        {
            Debug.Log("🎉 모든 업로드 완료");
            client.Disconnect();
            isUploading = false;
            EditorApplication.update -= UploadNextFile;
            return;
        }

        string localFile = files[currentIndex];
        string relativePath = Path.GetRelativePath(localRootPath, localFile);
        string remoteFilePath = $"{remoteRootPath}/{relativePath}".Replace("\\", "/");
        string remoteDir = Path.GetDirectoryName(remoteFilePath).Replace("\\", "/");

        currentFile = relativePath;

        try
        {
            client.CreateDirectory(remoteDir, true);
            client.UploadFile(localFile, remoteFilePath, FtpExists.Overwrite, false);
            Debug.Log($"📤 업로드 완료: {remoteFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ 업로드 실패: {remoteFilePath} - {ex.Message}");
            if (ex.InnerException != null)
                Debug.LogError("🔍 Inner: " + ex.InnerException.Message);
        }

        currentIndex++;
    }
}
#endif

using UnityEngine;

public static class AutoProcessKiller 
{
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        Application.quitting += KillZombieProcess;
    }

    private static void KillZombieProcess()
    {
        if (GameDataManager.Instance != null)
        {
            PlayerData data = GameDataManager.Instance.CapturePlayerData();
            SaveSystem.SaveGame(data);
        }
#if !UNITY_EDITOR
        
        System.Threading.Thread.Sleep(700); 
        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif
    }
}
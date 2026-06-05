using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string saveFile = Application.persistentDataPath + "/PlayerSave.json";

    public static void SaveGame(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFile, json);
        Debug.Log("Đã lưu game thành công tại: " + saveFile);
    }

    public static PlayerData LoadGame()
    {
        if (File.Exists(saveFile))
        {
            string json = File.ReadAllText(saveFile);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return null;
    }

    public static bool HasSaveFile()
    {
        return File.Exists(saveFile);
    }

    public static void DeleteSave()
    {
        if (File.Exists(saveFile))
        {
            File.Delete(saveFile);
            Debug.Log("Đã xóa file save cũ");
        }
    }
}
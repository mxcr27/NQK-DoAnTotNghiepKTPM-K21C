using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    
    [Header("Danh sách phòng")]
    public List<RoomManager> allRooms = new List<RoomManager>();

    private void Awake()
    {
        Instance = this;

        RoomManager[] rooms = GetComponentsInChildren<RoomManager>();
        allRooms.AddRange(rooms);
    }
}
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Cài đặt Phòng")]
    public GameObject roomContent; 
    public GameObject roomCamera; 

    private void Start()
    {
        if (roomContent == null && transform.childCount > 0)
            roomContent = transform.GetChild(0).gameObject;

        if (roomCamera == null)
        {
            var cam = GetComponentInChildren<Unity.Cinemachine.CinemachineCamera>(true);
            if (cam != null) roomCamera = cam.gameObject;
        }

        if (roomContent != null) roomContent.SetActive(false);
        if (roomCamera != null) roomCamera.SetActive(false);

        Collider2D roomCollider = GetComponent<Collider2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && roomCollider != null)
        {
            if (roomCollider.OverlapPoint(player.transform.position))
            {
                if (roomContent != null) roomContent.SetActive(true);
                if (roomCamera != null) roomCamera.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (roomContent != null) roomContent.SetActive(true);
            if (roomCamera != null) roomCamera.SetActive(true); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (roomContent != null) roomContent.SetActive(false);
            if (roomCamera != null) roomCamera.SetActive(false); 
        }
    }
}
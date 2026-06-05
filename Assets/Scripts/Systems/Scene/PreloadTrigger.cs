using UnityEngine;
public class PreloadTrigger : MonoBehaviour
{
    public SeamlessSceneLoader targetDoor;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetDoor != null) {
            targetDoor.StartPreloadingScene();
        }
    }
}
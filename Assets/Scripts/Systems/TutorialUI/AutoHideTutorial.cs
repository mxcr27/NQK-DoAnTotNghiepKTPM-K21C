using UnityEngine;

[RequireComponent(typeof(Canvas))] 
public class AutoHideTutorial : MonoBehaviour
{
    private Canvas myCanvas;

    void Awake()
    {
        myCanvas = GetComponent<Canvas>();
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            myCanvas.enabled = false; 
        }
        else
        {
            myCanvas.enabled = true;  
        }
    }
}
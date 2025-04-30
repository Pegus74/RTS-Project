using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject canvas1; 
    public GameObject canvas2; 

    void Start()
    {
      
        canvas1.SetActive(true);
        canvas2.SetActive(false);
    }

    public void SwitchCanvas()
    {
      
        if (canvas1.activeSelf)
        {
            canvas1.SetActive(false);
            canvas2.SetActive(true);
        }
        else
        {
            canvas1.SetActive(true);
            canvas2.SetActive(false);
        }
    }
}

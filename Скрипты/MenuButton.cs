using UnityEngine;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject canvas1; // Первый Canvas
    public GameObject canvas2; // Второй Canvas

    void Start()
    {
        // Убедитесь, что первый Canvas активен, а второй скрыт
        canvas1.SetActive(true);
        canvas2.SetActive(false);
    }

    public void SwitchCanvas()
    {
        Debug.Log("Hello: " );
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

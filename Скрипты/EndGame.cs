using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public GameObject canvas; 

    void OnDestroy()
    {
       
        if (canvas != null)
        {
           
            canvas.SetActive(true);
        }
    }
}

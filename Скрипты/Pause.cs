using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{

    public GameObject canvas1;

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (canvas1.activeSelf)
            {
                canvas1.SetActive(false);
              
            }
            else
            {
                canvas1.SetActive(true);
               
            }

        }

    }
    void CloseMenu()
    {
        if (canvas1.activeSelf)
        {
            canvas1.SetActive(false);

        }
        else
        {
            canvas1.SetActive(true);

        }
    }
}

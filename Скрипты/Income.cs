using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Income : MonoBehaviour
{
    public int income = 5;
    void Start()
    {
        InvokeRepeating("IncomeRate", 1.0f, 3.0f);
    }

    void IncomeRate()
    {
        ResourceManager.Instance.IncreaseResource(ResourceManager.ResourcesType.Gold, income);
    }
}

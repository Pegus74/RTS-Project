using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthTracker : MonoBehaviour
{
    public Slider HealthBarSlider;
    public Image sliderFill;

    public Material greenEmission;
    public Material yellowEmission;
    public Material redEmission;
    private Coroutine smoothHealthChangeCoroutine;

    public void UpdateSliderValue(float currentHealth, float maxHealth)
    {
        if (gameObject.activeInHierarchy) 
        {
            float healthPercentage = Mathf.Clamp01(currentHealth / maxHealth);         
            if (smoothHealthChangeCoroutine != null)
            {
                StopCoroutine(smoothHealthChangeCoroutine);
            }          
            smoothHealthChangeCoroutine = StartCoroutine(SmoothHealthChange(HealthBarSlider.value, healthPercentage, 0.5f));
            UpdateColor(healthPercentage);
        }
     
    }

    private IEnumerator SmoothHealthChange(float startValue, float targetValue, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            HealthBarSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        HealthBarSlider.value = targetValue;
        smoothHealthChangeCoroutine = null;
    }


    private void UpdateColor(float healthPercentage)
    {
        if (healthPercentage >= 0.6f)
        {
            sliderFill.material = greenEmission;
        }
        else if (healthPercentage >= 0.3f)
        {
            sliderFill.material = yellowEmission;
        }
        else
        {
            sliderFill.material = redEmission;
        }
    }

}

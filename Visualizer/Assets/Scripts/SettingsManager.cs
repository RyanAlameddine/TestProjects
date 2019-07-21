using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField]
    NodeManager nodeManager;
    [SerializeField]
    InputField min;
    [SerializeField]
    InputField max;

    void Start()
    {
        
    }

    public void SettingsToggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        min.text = ((int) nodeManager.GradientRange.x).ToString();

        max.text = ((int) nodeManager.GradientRange.y).ToString();
    }

    public void SetRange()
    {
        int minVal;
        int maxVal;
        if(!(int.TryParse(min.text, out minVal) && int.TryParse(max.text, out maxVal)))
        {
            return;
        }


        nodeManager.GradientRange.x = minVal;
        nodeManager.GradientRange.y = maxVal;
        nodeManager.ResetNodeGradients();
    }

    public bool IsFocused
    {
        get
        {
            return min.isFocused || max.isFocused;
        }
    }
}

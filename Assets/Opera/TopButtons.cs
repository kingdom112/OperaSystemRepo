using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TopButtons : MonoBehaviour
{

    public RectTransform panel;


    void Start()
    {
        Hide();
    }

    void Update()
    {
        if(panel.gameObject.activeInHierarchy)
        {
            if (Input.GetMouseButton(0))
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(panel, Input.mousePosition) == false)
                {
                    Hide();
                }
            }
        } 
      
    }



    public void Show()
    {
        panel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
    }
}

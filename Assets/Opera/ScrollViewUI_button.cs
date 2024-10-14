using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewUI_button : MonoBehaviour
{
    public ScrollViewUI scrollViewUI;
    public int buttonNumber = 0;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnButtonPressed()
    {
        if(scrollViewUI)
        {
            scrollViewUI.OnOneButtonPressed(this, buttonNumber);
        }
    }
}

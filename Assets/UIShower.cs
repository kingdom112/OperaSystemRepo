using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShower : MonoBehaviour
{

    public RectTransform ui;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ui != null)
        {
            Debug.Log("offset min : " + ui.offsetMin + "   , offset max: " + ui.offsetMax);
            Debug.Log("anchoredPosition : " + ui.anchoredPosition );
        }
    }
}

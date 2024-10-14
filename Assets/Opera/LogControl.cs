using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogControl : MonoBehaviour
{

    public Animator anim;

    public GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Button_Log()
    {
        anim.Play("logAnimation");
        panel.GetComponent<Image>().raycastTarget = false;
    }

}

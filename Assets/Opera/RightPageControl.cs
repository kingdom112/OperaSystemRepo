using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightPageControl : MonoBehaviour
{

    public GameObject normalRight;
    public GameObject AnlyzeRight;

    public void Button_ChangeToPageNormal()
    {
        normalRight.SetActive(true);
        AnlyzeRight.SetActive(false);
    }
    public void Button_ChangeToPageAnlyze()
    {
        normalRight.SetActive(false);
        AnlyzeRight.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

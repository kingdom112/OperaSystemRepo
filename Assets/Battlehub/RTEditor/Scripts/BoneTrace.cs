using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneTrace : MonoBehaviour
{
    GameObject bonetest;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))//当鼠标按下时
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitinfo;
            if(Physics.Raycast(ray,out hitinfo))
            {
                bonetest = hitinfo.collider.gameObject;
                Debug.Log("ray on"+ bonetest.name);
            }
        }
    }
}

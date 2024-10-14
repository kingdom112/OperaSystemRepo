using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightStackMemberStartButton : MonoBehaviour
{

    public GameObject prefab; 
    public RightStackController controller;
    private GameObject createdGA = null;

    private void Awake()
    { 
    }

    public void ButtonAction()
    {


        if (createdGA == null)
        {
            createdGA = Instantiate<GameObject>(prefab);
            controller.GetInStack(createdGA.GetComponent<RectTransform>());
            createdGA.GetComponent<RightStackMember>().Show();
        }
        else
        {
            if (createdGA.GetComponent<RightStackMember>().CanDestroy)
            {
                controller.OutStack(createdGA.GetComponent<RectTransform>());
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

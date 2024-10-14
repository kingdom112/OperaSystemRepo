using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gaFollower : MonoBehaviour
{

    public Transform target = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (target)
            transform.position = target.position;
    }

}

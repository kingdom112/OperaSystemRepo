using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionControl : MonoBehaviour
{
    [System.Serializable]
    public class PosSets
    {
        public string name;
        public Vector3 pos_master;
        public Vector3 pos_student;
    }

    public VFPlayer_InGame vfPlayer;
    public List<PosSets> posSets = new List<PosSets>();
    public int now = 0;

    private float distanceScale = 1f;

    private void Awake()
    {
        Set(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ChangeDistanceScale(float _value)
    {
        distanceScale = _value;
        Refresh();
    }

    public void Refresh()
    {
        Set(now);
    }

    public void Set(int num)
    {
        if (num >= posSets.Count)
        {
            return;
        }
        now = num; 
        if(vfPlayer.ch_master.hasInit)
        {
            if(vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count)
            {
                Vector3 newPos = vfPlayer.animShowList[vfPlayer.ch_master.createdGA_No].pos + posSets[now].pos_master * distanceScale;
                vfPlayer.ch_master.createdGA.transform.position = newPos;
                for(int i=0; i< vfPlayer.ch_master.added_createdGA.Count; i++)
                {
                   
                }
            }
            else
            {
                Vector3 newPos =  posSets[now].pos_master;
                vfPlayer.ch_master.createdGA.transform.position = newPos;
                for (int i = 0; i < vfPlayer.ch_master.added_createdGA.Count; i++)
                {
                  
                }
            }
        }
        if (vfPlayer.ch_student.hasInit)
        {
            if (vfPlayer.ch_student.createdGA_No < vfPlayer.animShowList.Count)
            {
                Vector3 newPos = vfPlayer.animShowList[vfPlayer.ch_student.createdGA_No].pos + posSets[now].pos_student * distanceScale;
                vfPlayer.ch_student.createdGA.transform.position = newPos;
                for (int i = 0; i < vfPlayer.ch_student.added_createdGA.Count; i++)
                {
                
                }
            }
            else
            {
                Vector3 newPos = posSets[now].pos_student; 
                vfPlayer.ch_student.createdGA.transform.position = newPos;
                for (int i = 0; i < vfPlayer.ch_student.added_createdGA.Count; i++)
                {
                  
                }
            }
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

/// <summary>
/// 一个用于显示圆弧的脚本
/// </summary>
public class RoundAngleShower : MonoBehaviour
{
    public class Data
    {
        public Vector3 pos_start;
        public Vector3 pos_end;
        public Vector3 dir_start;
        public Vector3 dir_end;
        public float radius;
        public int layer;

        public Data(Vector3 _pos_start, Vector3 _pos_end, Vector3 _dir_start, Vector3 _dir_end, float _radius, int _layer)
        {
            pos_start = _pos_start;
            pos_end = _pos_end;
            dir_start = _dir_start;
            dir_end = _dir_end;
            radius = _radius;
            layer = _layer;
        }
    }
    public GameObject SplineC_Prefab;
    public Color textColor = Color.green;
    List<GameObject> createdSplineC = new List<GameObject>();
    List<float> angs = new List<float>();
    List<Vector3> posList = new List<Vector3>();
    List<Data> datas = new List<Data>();


    CameraController cameraController1 = null;
    private void Awake()
    { 
        cameraController1 = GameObject.FindObjectOfType<CameraController>();
    }


    /// <summary>
    /// 刷新显示
    /// </summary>
    public void RefreshShow(List<Data> _datas)
    {
        datas = _datas;
        ReSizeToVFPlayer();//reset size 
        for(int i=0; i<datas.Count; i++)
        {
            int count = 10;
            SplineComputer sp1 = createdSplineC[i].GetComponent<SplineComputer>();
            sp1.gameObject.layer = datas[i].layer;
            SplinePoint[] sps = new SplinePoint[count + 1];
            for (int j = 0; j <= count; j++)
            {
                Debug.DrawRay(datas[i].pos_start, Vector3.Slerp(datas[i].dir_start, datas[i].dir_end, (float)j / (float)count), Color.yellow);
                sps[j].position = datas[i].pos_start + (Vector3.Slerp(datas[i].dir_start, datas[i].dir_end, (float)j / (float)count)) * 0.2f;
                sps[j].color = Color.yellow;
                sps[j].size = 0.02f;
            }
            if (cameraController1.screenMode == CameraController.ScreenMode.Two)
            {
                posList[i] = cameraController1.cam_2.WorldToScreenPoint(datas[i].pos_start);
            }
            else
            {
                posList[i] = Camera.main.WorldToScreenPoint(datas[i].pos_start);
            }
            angs[i] = Vector3.Angle(datas[i].dir_start, datas[i].dir_end);
            sp1.SetPoints(sps);
        }
     
    }
    /// <summary>
    /// 使各种列表和VFPlayer里的数据数量一致
    /// </summary>
    private void ReSizeToVFPlayer()
    {
        int count = datas.Count;
        if (createdSplineC.Count < count)
        {
            for (int i = 0; i < count - createdSplineC.Count; i++)
            {
                GameObject ga1 = Instantiate<GameObject>(SplineC_Prefab);
                ga1.transform.position = Vector3.zero;
                createdSplineC.Add(ga1);
            }
        }
        for (int i = 0; i < count; i++)
        {
            createdSplineC[i].gameObject.SetActive(true);
        }
        for (int i = count; i < createdSplineC.Count; i++)
        {
            createdSplineC[i].gameObject.SetActive(false);
        }

        if (angs.Count < count)
        {
            for (int i = 0; i < count - angs.Count; i++)
            {
                angs.Add(0);
            }
        }
        if (angs.Count > count)
        {
            for (int i = angs.Count - 1; i >= count; i--)
            {
                angs.RemoveAt(i);
            }
        }

        if (posList.Count < count)
        {
            for (int i = 0; i < count - posList.Count; i++)
            {
                posList.Add(Vector3.zero);
            }
        }
        if (posList.Count > count)
        {
            for (int i = posList.Count - 1; i >= count; i--)
            {
                posList.RemoveAt(i);
            }
        }
    }

    private void OnGUI()
    {
        if (cameraController1 != null)
        {
            for (int i = 0; i < angs.Count; i++)
            {
                Vector3 v1 = posList[i];
                float w1 = 100f;
                float h1 = 100f;
                GUI.color = textColor;
                GUI.Label(new Rect(v1.x - 0f * w1, Screen.height - v1.y - 0f * h1, w1, h1), "" + angs[i].ToString()); 
            }
        }

    }
}

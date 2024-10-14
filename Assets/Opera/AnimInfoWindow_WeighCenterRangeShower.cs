using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 实时显示重心范围的组件。
/// </summary>
public class AnimInfoWindow_WeighCenterRangeShower : MonoBehaviour
{
    public Transform p1T, p2T, p3T;
    public AnimInfoWindowControl control;
    public Transform circleRangeGO;
    JAnimationSystem.BoneMatch_Player bonePlayer1;
    Transform boneRoot;
    Transform boneRoot_source;

    private void Awake()
    {
        circleRangeGO.SetParent(null);
        p1T.SetParent(null);
        p1T.transform.localScale = Vector3.one * 0.1f;
        p2T.SetParent(null);
        p2T.transform.localScale = Vector3.one * 0.1f;
        p3T.SetParent(null);
        p3T.transform.localScale = Vector3.one * 0.1f;
    }
    void Start()
    {
        bool _visible = false;
        if(control.isMaster)
        {
            bonePlayer1 = control.vfPlayer.CreatFollowerBone_master(_visible);
            boneRoot_source = control.vfPlayer.ch_master.bm_p1.GetRootTransform();
        }
        else
        {
            bonePlayer1 = control.vfPlayer.CreatFollowerBone_student(_visible);
            boneRoot_source = control.vfPlayer.ch_student.bm_p1.GetRootTransform();
        }
        if(bonePlayer1 == null)
        {
            Debug.LogError("create ERROR!!!");
            return;
        }
        boneRoot = bonePlayer1.GetRootTransform();
        boneRoot.name = "AnimInfoWindow_WeighCenterRangeShower_Bone";
    }
    private void OnDestroy()
    {
        if(circleRangeGO != null)
            Destroy(circleRangeGO.gameObject);
        if (p1T != null)
            Destroy(p1T.gameObject);
        if (p2T != null)
            Destroy(p2T.gameObject);
        if (p3T != null)
            Destroy(p3T.gameObject);
    }
    void Update()
    {
        float time1 = 0f, time2 = 0f, time3 = 0f;
        if (control.isMaster)
        {
            time1 = Mathf.Clamp(control.vfPlayer.ch_master.timer - 0.5f, 0, control.vfPlayer.ch_master.time);
            time2 = control.vfPlayer.ch_master.timer;
            time3 = Mathf.Clamp(control.vfPlayer.ch_master.timer + 0.5f, 0, control.vfPlayer.ch_master.time); 
        }
        else
        {
            time1 = Mathf.Clamp(control.vfPlayer.ch_student.timer - 0.5f, 0, control.vfPlayer.ch_student.time);
            time2 = control.vfPlayer.ch_student.timer;
            time3 = Mathf.Clamp(control.vfPlayer.ch_student.timer + 0.5f, 0, control.vfPlayer.ch_student.time);
        }
        if(Mathf.Abs(time1 - time2) <= 0.0001f || Mathf.Abs(time3 - time2) <= 0.0001f)
        {
            //两点不能求圆形。
            circleRangeGO.gameObject.SetActive(false);
            p1T.gameObject.SetActive(false);
            p2T.gameObject.SetActive(false);
            p3T.gameObject.SetActive(false);
        }
        else
        {
            bonePlayer1.PlayDataInThisFrame_Curve(time1);
            Vector3 weightCenter1 = GetWeightCenter(boneRoot);
            bonePlayer1.PlayDataInThisFrame_Curve(time2);
            Vector3 weightCenter2 = GetWeightCenter(boneRoot);
            bonePlayer1.PlayDataInThisFrame_Curve(time3);
            Vector3 weightCenter3 = GetWeightCenter(boneRoot);
            float _radius = GetCircleRadius_Min(weightCenter1, weightCenter2, weightCenter3, weightCenter2);
            Vector3 _scale = Vector3.one * _radius * 2f;
            _scale.y = 0.01f;
            p2T.position = weightCenter2 - boneRoot.position + boneRoot_source.position;
            p1T.position = weightCenter1 - weightCenter2 + p2T.position; 
            p3T.position = weightCenter3 - weightCenter2 + p2T.position; 
            circleRangeGO.position = p2T.position;// 
            circleRangeGO.localScale = _scale;
            circleRangeGO.gameObject.SetActive(true);
            p1T.gameObject.SetActive(true);
            p2T.gameObject.SetActive(true);
            p3T.gameObject.SetActive(true);
        }
       


    }
    private float GetCircleRadius_Min(Vector3 pt1, Vector3 pt2, Vector3 pt3, Vector3 rootPos)
    {
        Vector2 p1v, p2v, p3v, pRootv;
        p1v.x = pt1.x;
        p1v.y = pt1.z;
        p2v.x = pt2.x;
        p2v.y = pt2.z;
        p3v.x = pt3.x;
        p3v.y = pt3.z;
        pRootv.x = rootPos.x;
        pRootv.y = rootPos.z;
        float r1 = Vector2.Distance(pRootv, p1v);
        float r2 = Vector2.Distance(pRootv, p2v);
        float r3 = Vector2.Distance(pRootv, p3v);
        return Mathf.Max(r1, r2, r3);
    }
    /*
    private float GetCircleRadius(Vector3 pt1, Vector3 pt2, Vector3 pt3, Vector3 rootPos, out Vector2  centerOffset)
    {
        Vector2 p1v, p2v, p3v;
        p1v.x = pt1.x;
        p1v.y = pt1.z;
        p2v.x = pt2.x;
        p2v.y = pt2.z;
        p3v.x = pt3.x;
        p3v.y = pt3.z;
        Vector2 _center;
        float _radius = findCircle1(p1v, p2v, p3v, out _center);
        Vector2 _rootPos_y0 = new Vector2(rootPos.x, rootPos.z);
        centerOffset = _rootPos_y0 - _center;//不知道这个减这个，顺序对不对
        return _radius;
    }
    float findCircle1(Vector2 pt1, Vector2 pt2, Vector2 pt3, out Vector2 _center)
    {
        //定义两个点，分别表示两个中点
        Vector2 midpt1, midpt2;
        //求出点1和点2的中点
        midpt1 = Vector2.Lerp(pt1, pt2, 0.5f);
        //求出点3和点1的中点
        midpt2 = Vector2.Lerp(pt1, pt3, 0.5f);
        //求出分别与直线pt1pt2，pt1pt3垂直的直线的斜率
        float k1 = -(pt2.x - pt1.x) / (pt2.y - pt1.y);
        float k2 = -(pt3.x - pt1.x) / (pt3.y - pt1.y);
        //然后求出过中点midpt1，斜率为k1的直线方程（既pt1pt2的中垂线）：y - midPt1.y = k1( x - midPt1.x)
        //以及过中点midpt2，斜率为k2的直线方程（既pt1pt3的中垂线）：y - midPt2.y = k2( x - midPt2.x)
        //定义一个圆的数据的结构体对象CD
        //CircleData CD;
        Vector2 center;
        //连立两条中垂线方程求解交点得到：
        center.x = (midpt2.y - midpt1.y - k2 * midpt2.x + k1 * midpt1.x) / (k1 - k2);
        center.y = midpt1.y + k1 * (midpt2.y - midpt1.y - k2 * midpt2.x + k2 * midpt1.x) / (k1 - k2);
        //用圆心和其中一个点求距离得到半径：
        float radius = Vector2.Distance(center, pt1);
        _center = center;
        return radius;
    }
    */
    private Vector3 GetWeightCenter(Transform _boneRoot)
    {
        Vector3 posAll = Vector3.zero;
        int boneCount = 0;
        GetAllBonePosAdd(_boneRoot, ref posAll, ref boneCount);
        posAll = posAll / boneCount;
        posAll.y = 0;
        return posAll;
    }

    /// <summary>
    /// 获取所有骨骼的位置加和
    /// </summary>
    void GetAllBonePosAdd(Transform bone, ref Vector3 posAll, ref int boneCount)
    {
        if (bone.GetComponent<SphereCollider>() != null)
        {
            posAll += bone.position;
            boneCount++;
        }
        for (int i = 0; i < bone.childCount; i++)
        {
            GetAllBonePosAdd(bone.GetChild(i), ref posAll, ref boneCount);
        }
    }

}

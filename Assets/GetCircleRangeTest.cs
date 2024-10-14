using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCircleRangeTest : MonoBehaviour
{

    public Transform p1, p2, p3;
    public Transform circle;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 p1v, p2v, p3v;
        p1v.x = p1.position.x;
        p1v.y = p1.position.z;
        p2v.x = p2.position.x;
        p2v.y = p2.position.z;
        p3v.x = p3.position.x;
        p3v.y = p3.position.z;
        findCircle1(p1v, p2v, p3v);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 p1v, p2v, p3v;
        p1v.x = p1.position.x;
        p1v.y = p1.position.z;
        p2v.x = p2.position.x;
        p2v.y = p2.position.z;
        p3v.x = p3.position.x;
        p3v.y = p3.position.z;
        findCircle1(p1v, p2v, p3v);
    }

    float findCircle1(Vector2 pt1, Vector2 pt2, Vector2 pt3)
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


        circle.position = new Vector3(center.x, 0, center.y);
        circle.localScale = radius * Vector3.one * 2;

        return radius;
    }
}

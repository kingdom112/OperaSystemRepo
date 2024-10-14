using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLDrawBone : MonoBehaviour
{
    public Transform followTarget;
    public Transform boneTarget;
    public Color mainColor = Color.blue;
    public Vector3 posOffset = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void drawWire_G(Vector3 _pos, float radius)
    {
        //顶点数组
        Vector3[] ballVertices = new Vector3[182];
        //三角形数组
        int[] ballTriangles = new int[1080];
        /*水平每18度、垂直每18度确定一个顶点，
        顶部和底部各一个顶点，一共是9x20+2=182个顶点。
        每一环与相邻的下一环为一组，之间画出40个三角形，一共8组。
        顶部和底部各与相邻环画20个三角形，总三角形数量40x8+20x2=360,
        三角形索引数量360x3=1080*/


        int verticeCount = 0;
        for (int vD = 18; vD < 180; vD += 18)
        {
            float circleHeight =
            radius * Mathf.Cos(vD * Mathf.Deg2Rad);
            float circleRadius =
            radius * Mathf.Sin(vD * Mathf.Deg2Rad);
            for (int hD = 0; hD < 360; hD += 18)
            {
                ballVertices[verticeCount] =
                new Vector3(
                circleRadius * Mathf.Cos(hD * Mathf.Deg2Rad),
                circleHeight,
                circleRadius * Mathf.Sin(hD * Mathf.Deg2Rad));
                verticeCount++;
            }
        }
        ballVertices[180] = new Vector3(0, radius, 0);
        ballVertices[181] = new Vector3(0, -radius, 0);
        //ball.vertices = ballVertices;


        int triangleCount = 0;
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < 20; i++)
            {
                ballTriangles[triangleCount++] =
                j * 20 + i;
                ballTriangles[triangleCount++] =
                (j + 1) * 20 + (i == 19 ? 0 : i + 1);
                ballTriangles[triangleCount++] =
                (j + 1) * 20 + i;
                ballTriangles[triangleCount++] =
                j * 20 + i;
                ballTriangles[triangleCount++] =
                j * 20 + (i == 19 ? 0 : i + 1);
                ballTriangles[triangleCount++] =
                (j + 1) * 20 + (i == 19 ? 0 : i + 1);
            }
        }
        for (int i = 0; i < 20; i++)
        {
            ballTriangles[triangleCount++] =
            180;
            ballTriangles[triangleCount++] =
            (i == 19 ? 0 : i + 1);
            ballTriangles[triangleCount++] =
            i;
            ballTriangles[triangleCount++] =
            181;
            ballTriangles[triangleCount++] =
            160 + i;
            ballTriangles[triangleCount++] =
            160 + (i == 19 ? 0 : i + 1);
        }


        // 设置颜色
        //GL.Color(_color);
        for (int i=0; i< ballTriangles.Length; i+=3)
        {
            Vector3 point1 = ballVertices[ballTriangles[i]] + _pos;
            Vector3 point2 = ballVertices[ballTriangles[i + 1]] + _pos;
            Vector3 point3 = ballVertices[ballTriangles[i + 2]] + _pos;
            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawLine(point2, point3);
            Gizmos.DrawLine(point1, point3);
        } 
    }

    void drawWire(Vector3 _pos, Color _color, float radius)
    {
        //顶点数组
        Vector3[] ballVertices = new Vector3[182];
        //三角形数组
        int[] ballTriangles = new int[1080];
        /*水平每18度、垂直每18度确定一个顶点，
        顶部和底部各一个顶点，一共是9x20+2=182个顶点。
        每一环与相邻的下一环为一组，之间画出40个三角形，一共8组。
        顶部和底部各与相邻环画20个三角形，总三角形数量40x8+20x2=360,
        三角形索引数量360x3=1080*/


        int verticeCount = 0;
        for (int vD = 18; vD < 180; vD += 18)
        {
            float circleHeight =
            radius * Mathf.Cos(vD * Mathf.Deg2Rad);
            float circleRadius =
            radius * Mathf.Sin(vD * Mathf.Deg2Rad);
            for (int hD = 0; hD < 360; hD += 18)
            {
                ballVertices[verticeCount] =
                new Vector3(
                circleRadius * Mathf.Cos(hD * Mathf.Deg2Rad),
                circleHeight,
                circleRadius * Mathf.Sin(hD * Mathf.Deg2Rad));
                verticeCount++;
            }
        }
        ballVertices[180] = new Vector3(0, radius, 0);
        ballVertices[181] = new Vector3(0, -radius, 0);
        //ball.vertices = ballVertices;


        int triangleCount = 0;
        for (int j = 0; j < 8; j++)
        {
            for (int i = 0; i < 20; i++)
            {
                ballTriangles[triangleCount++] =
                j * 20 + i;
                ballTriangles[triangleCount++] =
                (j + 1) * 20 + (i == 19 ? 0 : i + 1);
                ballTriangles[triangleCount++] =
                (j + 1) * 20 + i;
                ballTriangles[triangleCount++] =
                j * 20 + i;
                ballTriangles[triangleCount++] =
                j * 20 + (i == 19 ? 0 : i + 1);
                ballTriangles[triangleCount++] =
                (j + 1) * 20 + (i == 19 ? 0 : i + 1);
            }
        }
        for (int i = 0; i < 20; i++)
        {
            ballTriangles[triangleCount++] =
            180;
            ballTriangles[triangleCount++] =
            (i == 19 ? 0 : i + 1);
            ballTriangles[triangleCount++] =
            i;
            ballTriangles[triangleCount++] =
            181;
            ballTriangles[triangleCount++] =
            160 + i;
            ballTriangles[triangleCount++] =
            160 + (i == 19 ? 0 : i + 1);
        }

        //GL.LINES 画线
        GL.Begin(GL.LINES);
        // 设置颜色
        GL.Color(_color); 
        for (int i = 0; i < ballTriangles.Length; i += 3)
        { 
            Vector3 point1 = ballVertices[ballTriangles[i]] + _pos;
            Vector3 point2 = ballVertices[ballTriangles[i + 1]] + _pos;
            Vector3 point3 = ballVertices[ballTriangles[i + 2]] + _pos;
            GL.Vertex3(point1.x, point1.y, point1.z);
            GL.Vertex3(point2.x, point2.y, point2.z);
            GL.Vertex3(point3.x, point3.y, point3.z);
        }
        GL.End(); 
    }

    //划线使用的材质球
    static Material lineMaterial;
    /// <summary>
    /// 创建一个材质球
    /// </summary>
    static void CreateLineMaterial()
    {
        //如果材质球不存在
        if (!lineMaterial)
        {
            //用代码的方式实例一个材质球
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            //设置参数
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //设置参数
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            //设置参数
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }


    private void DrawBone(Transform _t, Color _color, Vector3 _pos)
    {
        if (_t == null) return;

        if (_t.GetComponent<Renderer>() != null && _t.GetComponent<Renderer>().enabled == true)
        {
            drawWire(_pos, _color, _t.localScale.x);
        }

        for (int i=0; i<_t.childCount; i++)
        {
            //GL.LINES 画线
            GL.Begin(GL.LINES);
            // 设置颜色
            GL.Color(_color);
            Vector3 _drawPos = _pos;
            Vector3 npos = _pos + _t.GetChild(i).position - _t.position; 
            //画线起始点
            GL.Vertex3(_drawPos.x, _drawPos.y, _drawPos.z); 
            // 划线重点
            GL.Vertex3(npos.x, npos.y, npos.z);

            GL.End();

            DrawBone(_t.GetChild(i), _color, npos);
        }
    }
    private void DrawBoneG(Transform _t, Color _color, Vector3 _pos)
    {
        if (_t == null) return;

        if (_t.GetComponent<Renderer>() != null && _t.GetComponent<Renderer>().enabled == true)
        {
            drawWire_G(_pos, _t.localScale.x);
        }

        for (int i = 0; i < _t.childCount; i++)
        { 
            // 设置颜色
            //GL.Color(_color);
            Vector3 _drawPos = _pos;
            Vector3 npos = _pos + _t.GetChild(i).position - _t.position ;
            Gizmos.DrawLine(_drawPos, npos);

            DrawBoneG(_t.GetChild(i), _color, npos);
        }
    }
    private void OnDrawGizmos()
    {
        if (followTarget != null)
        {
            DrawBoneG(boneTarget, mainColor, followTarget.position + posOffset);
        }
    }

    /// <summary>
    /// 使用GL画线的回调
    /// </summary>
    public void OnRenderObject()
    {
        //创建材质球
        CreateLineMaterial();
        //激活第一个着色器通过（在这里，我们知道它是唯一的通过）
        lineMaterial.SetPass(0);
        //渲染入栈  在Push——Pop之间写GL代码
        GL.PushMatrix();

        if(followTarget != null)
        {
            DrawBone(boneTarget, mainColor, followTarget.position + posOffset);
        } 
        /*
        //矩阵相乘，将物体坐标转化为世界坐标
        GL.MultMatrix(transform.localToWorldMatrix);

        // 开始画线  在Begin——End之间写画线方式
        //GL.LINES 画线
        GL.Begin(GL.LINES);
        for (int i = 0; i < lineCount; ++i)
        {
            float a = i / (float)lineCount;
            float angle = a * Mathf.PI * 2;
            // 设置颜色
            GL.Color(new Color(a, 1 - a, 0, 0.8F));
            //画线起始点
            GL.Vertex3(0, 0, 0);
            // 划线重点
            GL.Vertex3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }
        GL.End();*/



        //渲染出栈
        GL.PopMatrix();
    }
}

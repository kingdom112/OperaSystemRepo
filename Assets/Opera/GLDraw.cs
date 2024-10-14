using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using Dreamteck.Splines;
using JAnimCurves;

public class GLDraw : MonoBehaviour
{

    [System.Serializable]
    public class CurvesClass
    {
        public Transform boneT;
        public JAnimationData.BoneType boneType = JAnimationData.BoneType.unknowType;
        public AnimationCurve curve_x = null;
        public AnimationCurve curve_y = null;
        public AnimationCurve curve_z = null;
        public float timer = 0f;

        public bool CanUse
        {
            get
            {
                if(boneT != null && curve_x != null && curve_y != null && curve_z != null && boneType != JAnimationData.BoneType.unknowType)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public CurvesClass()
        {
            boneT = null;
            boneType = JAnimationData.BoneType.unknowType;
            curve_x = null;
            curve_y = null;
            curve_z = null;
            timer = 0f;
        }
    }
    private List<CurvesClass> curvesList = new List<CurvesClass>();

    private VFPlayer_InGame vfPlayer;
    public float lenmult = 10f;


    //划线的数量
    private int lineCount = 100;
    //每条线的长度
    private float radius = 3.0f;
    //划线使用的材质球
    static Material lineMaterial;

    private void Awake()
    {
        vfPlayer = GameObject.FindObjectOfType<VFPlayer_InGame>(); 
    }


    public void ReSizeToVFPlayer(int count)
    {
        if (curvesList.Count < count)
        {
            for (int i = 0; i < count - curvesList.Count; i++)
            {
                curvesList.Add(new CurvesClass());
            }
        }
        if (curvesList.Count > count)
        {
            for (int i = curvesList.Count - 1; i >= count; i--)
            {
                curvesList.RemoveAt(i);
            }
        }
    }

    public void Check()
    {
        int index1 = 0;
        for (int i = 0; i < vfPlayer.ch_master.selectBones.Count; i++)
        {
            VFPlayer_InGame.CharacterSelectBone bone1 = vfPlayer.ch_master.selectBones[i];
            if(curvesList[index1].boneType != bone1.Type || curvesList[index1].CanUse == false)
            {
                //无效，重新加载
                int p_i = vfPlayer.ch_master.bm_p1.GetPlayerIndexByBoneType(bone1.Type);
                BoneOnePlayer oneplayer1 = vfPlayer.ch_master.bm_p1.bonePlayers[p_i];
                AnimOneCurve_InGame curve_x = GetSavedCurve(true, oneplayer1, animCurveType.speed_world_x);
                AnimOneCurve_InGame curve_y = GetSavedCurve(true, oneplayer1, animCurveType.speed_world_y);
                AnimOneCurve_InGame curve_z = GetSavedCurve(true, oneplayer1, animCurveType.speed_world_z);
                if(curve_x == null || curve_y == null || curve_z == null)
                {
                    //加载失败，重算
                    GetCurveAndSave(true, vfPlayer.ch_master.bm_p1, oneplayer1);
                }
                else
                {
                    curvesList[index1].boneType = bone1.Type;
                    curvesList[index1].curve_x = curve_x.curve;
                    curvesList[index1].curve_y = curve_y.curve;
                    curvesList[index1].curve_z = curve_z.curve;
                    curvesList[index1].boneT = oneplayer1.boneOneMatch.matchedT;
                    curvesList[index1].timer = vfPlayer.ch_master.timer;
                }
            }
            else
            {
                curvesList[index1].timer = vfPlayer.ch_master.timer;
            }

            index1++;
        }
        for (int i = 0; i < vfPlayer.ch_student.selectBones.Count; i++)
        {
            VFPlayer_InGame.CharacterSelectBone bone1 = vfPlayer.ch_student.selectBones[i];
            if (curvesList[index1].boneType != bone1.Type || curvesList[index1].curve_x == null ||
                curvesList[index1].curve_y == null || curvesList[index1].curve_z == null)
            {
                //无效，重新加载
                int p_i = vfPlayer.ch_student.bm_p1.GetPlayerIndexByBoneType(bone1.Type);
                BoneOnePlayer oneplayer1 = vfPlayer.ch_student.bm_p1.bonePlayers[p_i];
                AnimOneCurve_InGame curve_x = GetSavedCurve(true, oneplayer1, animCurveType.speed_world_x);
                AnimOneCurve_InGame curve_y = GetSavedCurve(true, oneplayer1, animCurveType.speed_world_y);
                AnimOneCurve_InGame curve_z = GetSavedCurve(true, oneplayer1, animCurveType.speed_world_z);
                if (curve_x == null || curve_y == null || curve_z == null)
                {
                    //加载失败，重算
                    GetCurveAndSave(true, vfPlayer.ch_student.bm_p1, oneplayer1);
                }
                else
                {
                    curvesList[index1].boneType = bone1.Type;
                    curvesList[index1].curve_x = curve_x.curve;
                    curvesList[index1].curve_y = curve_y.curve;
                    curvesList[index1].curve_z = curve_z.curve;
                    curvesList[index1].boneT = oneplayer1.boneOneMatch.matchedT;
                    curvesList[index1].timer = vfPlayer.ch_student.timer;
                }
            }
            else
            {
                curvesList[index1].timer = vfPlayer.ch_student.timer;
            }

            index1++;
        }
    }
     

    public void GetCurveAndSave(bool ismaster,BoneMatch_Player player1, BoneOnePlayer bonePlayer1)
    {
        Debug.Log("GetCurveAndSave");
        if (bonePlayer1 == null) return;
        AnimationCurve Curve_x = new AnimationCurve();
        AnimationCurve Curve_y = new AnimationCurve();
        AnimationCurve Curve_z = new AnimationCurve();
        AnimOneCurve_ForSave saveData1_x = new AnimOneCurve_ForSave(animCurveType.speed_world_x, bonePlayer1.boneOneMatch.boneType);
        AnimOneCurve_ForSave saveData1_y = new AnimOneCurve_ForSave(animCurveType.speed_world_y, bonePlayer1.boneOneMatch.boneType);
        AnimOneCurve_ForSave saveData1_z = new AnimOneCurve_ForSave(animCurveType.speed_world_z, bonePlayer1.boneOneMatch.boneType);

        float fps = 30f;
        float timejiange = 1f / fps;
        int count = (int)((bonePlayer1.animDatas_Time_roa[bonePlayer1.animDatas_Time_roa.Count - 1] - bonePlayer1.animDatas_Time_roa[0]) / timejiange);
        count++;
        float timer = 0f;
        for (int i = 0; i < count; i++)
        {
            player1.PlayDataInThisFrame_Curve(timer);
            Vector3 pos1 = bonePlayer1.boneOneMatch.matchedT.position;
            player1.PlayDataInThisFrame_Curve(timer + timejiange / 6f);
            Vector3 pos2 = bonePlayer1.boneOneMatch.matchedT.position;
            Vector3 dir1 = pos2 - pos1;
            saveData1_x.AddData(dir1.x, timer);
            saveData1_y.AddData(dir1.y, timer);
            saveData1_z.AddData(dir1.z, timer);
            timer += timejiange;
        }

        AnimCurvesSaver saver01 = new AnimCurvesSaver();
        if (ismaster)
        {
            if (vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count)//判断是否为内置数据
            {
                string animName = vfPlayer.animShowList[vfPlayer.ch_master.createdGA_No].name;
                saver01.SaveOne(saveData1_x, animName, bonePlayer1.boneOneMatch.boneType);
                saver01.SaveOne(saveData1_y, animName, bonePlayer1.boneOneMatch.boneType);
                saver01.SaveOne(saveData1_z, animName, bonePlayer1.boneOneMatch.boneType);
            }
        }
        else
        {
            if (vfPlayer.ch_student.createdGA_No < vfPlayer.animShowList.Count)//判断是否为内置数据
            {
                string animName = vfPlayer.animShowList[vfPlayer.ch_student.createdGA_No].name;
                saver01.SaveOne(saveData1_x, animName, bonePlayer1.boneOneMatch.boneType);
                saver01.SaveOne(saveData1_y, animName, bonePlayer1.boneOneMatch.boneType);
                saver01.SaveOne(saveData1_z, animName, bonePlayer1.boneOneMatch.boneType);
            }
        }
         

    }

    public JAnimCurves.AnimOneCurve_InGame GetSavedCurve(bool ismaster, BoneOnePlayer bonePlayer1, JAnimCurves.animCurveType type1)
    {
        if (ismaster)
        {
            if (vfPlayer.ch_master.createdGA_No < vfPlayer.animShowList.Count)//判断是否为内置数据
            {
                string animName = vfPlayer.animShowList[vfPlayer.ch_master.createdGA_No].name;
                JAnimCurves.AnimCurvesSaver saver1 = new JAnimCurves.AnimCurvesSaver();
                JAnimCurves.AnimOneCurve_ForSave savedData1 = saver1.LoadOne(
                    animName, bonePlayer1.boneOneMatch.boneType, type1);
                if (savedData1 != null)
                {
                    JAnimCurves.AnimOneCurve_InGame data1 = new JAnimCurves.AnimOneCurve_InGame(savedData1);
                    return data1;
                }
            }
        }
        else
        {
            if (vfPlayer.ch_student.createdGA_No < vfPlayer.animShowList.Count)//判断是否为内置数据
            {
                string animName = vfPlayer.animShowList[vfPlayer.ch_student.createdGA_No].name;
                JAnimCurves.AnimCurvesSaver saver1 = new JAnimCurves.AnimCurvesSaver();
                JAnimCurves.AnimOneCurve_ForSave savedData1 = saver1.LoadOne(
                    animName, bonePlayer1.boneOneMatch.boneType, type1);
                if (savedData1 != null)
                {
                    JAnimCurves.AnimOneCurve_InGame data1 = new JAnimCurves.AnimOneCurve_InGame(savedData1);
                    return data1;
                }
            }
        }
        return null;
    }










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


        for(int i=0; i<curvesList.Count; i++)
        {
            if(curvesList[i].CanUse)
            {
                GL.Begin(GL.LINES);
                GL.Color(Color.yellow);
                //画线起始点
                GL.Vertex3(curvesList[i].boneT.position.x, curvesList[i].boneT.position.y, curvesList[i].boneT.position.z);
                // 划线
               
                GL.Vertex3(
                    curvesList[i].curve_x.Evaluate(curvesList[i].timer) * lenmult + curvesList[i].boneT.position.x,
                    curvesList[i].curve_y.Evaluate(curvesList[i].timer) * lenmult + curvesList[i].boneT.position.y,
                    curvesList[i].curve_z.Evaluate(curvesList[i].timer) * lenmult + curvesList[i].boneT.position.z);
                GL.End();
            }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using Dreamteck.Splines;
using JAnimCurves;

public class BoneInfoWindowControl : RightStackMember
{
    public List<BoneDealBase> boneDeals = new List<BoneDealBase>();
    public Color mainCurveColor = Color.red;//第一个打开的曲线的颜色
    public Color addedCurvesColor = Color.blue;//后加入的曲线的颜色
    public Text boneName;
    public Text boneIntroduction;
    public Dropdown curveSelectDropdown;
    public GameObject SplineC_Prefab;
    public Text SplineC_TextPrefab;
    public Transform SplineC_TextParentCanvas;
    public GLDraw glDraw;

    private VFPlayer_InGame vfPlayer;  


    List<float> angs = new List<float>();
    List<float> speeds = new List<float>();
    List<Vector3> posList = new List<Vector3>(); 

    List<GameObject> createdSplineC = new List<GameObject>();
    List<Text> createdSplineCText = new List<Text>();
    CameraController cameraController1 = null;
    /// <summary>
    /// 场景视图的Camera.
    /// </summary>
    Camera sceneCamera;

    private void Awake()//画布可视化部分
    {
        vfPlayer = GameObject.FindObjectOfType<VFPlayer_InGame>();
        cameraController1 = GameObject.FindObjectOfType<CameraController>();
        SplineC_TextParentCanvas.SetParent(null);
        SplineC_TextParentCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;//将画布对象 放在场景中 ，3D
        SplineC_TextParentCanvas.GetComponent<RectTransform>().position = Vector3.zero;
      
        //find sceneCamera
        {
            sceneCamera = null;
            Camera[] cams = FindObjectsOfType<Camera>();
            foreach (var _cam1 in cams)
            {
                if (_cam1.gameObject.name == "SceneCamera")
                {
                    sceneCamera = _cam1;
                    break;
                }
            }
        }
       
    }
    private void OnDestroy()
    {
        if(SplineC_TextParentCanvas != null)
            Destroy(SplineC_TextParentCanvas.gameObject);
    }

    private void Start()
    {

        boneDeals.Clear();
        boneDeals.Add(new BoneDeal_AngleCurve());
        boneDeals.Add(new BoneDeal_SpeedCurve());
        boneDeals.Add(new BoneDeal_AccelerationCurve());
        boneDeals.Add(new BoneDeal_RootHeightCurve());
        boneDeals.Add(new BoneDeal_SpineVerticalAngleCurve());
        //boneDeals.Add(new BoneDeal_WeightCenterRange());
        boneDeals.Add(new BoneDeal_ShoulderHipAngleCurve());
        boneDeals.Add(new BoneDeal_ShoulderRoateYSpeed());

        for (int i=0; i<boneDeals.Count; i++)
        {
            boneDeals[i].boneWindow = this;
        }//----------------

        if(vfPlayer.curveShowWindowControl != null)
        {
            Button_OpenWindow();
        }
    }

    /// <summary>
    /// 使各种列表和VFPlayer里的数据数量一致
    /// </summary>
    private void ReSizeToVFPlayer()
    {
        int count = vfPlayer.ch_master.selectBones.Count + vfPlayer.ch_student.selectBones.Count;
        glDraw.ReSizeToVFPlayer(count);
        if (createdSplineC.Count < count)
        {
            int _count1 = count - createdSplineC.Count;
            for (int i=0; i< _count1; i++)
            {
                GameObject ga1 = Instantiate<GameObject>(SplineC_Prefab);
                Text textGa1 = Instantiate<GameObject>(SplineC_TextPrefab.gameObject).GetComponent<Text>();
                textGa1.transform.SetParent(SplineC_TextParentCanvas);
                ga1.transform.position = Vector3.zero;
                createdSplineC.Add(ga1 );
                createdSplineCText.Add(textGa1);
            }
        }
        for(int i=0; i<count; i++)
        { 
            createdSplineC[i].gameObject.SetActive(true);
            createdSplineCText[i].gameObject.SetActive(true);
        }
        for(int i=count; i< createdSplineC.Count; i++)
        { 
            createdSplineC[i].gameObject.SetActive(false);
            createdSplineCText[i].gameObject.SetActive(false);
        }

        if (angs.Count < count)
        {
            int _count1 = count - angs.Count;
            for (int i = 0; i < _count1; i++)
            {
                angs.Add(0);
            }
        }
        if(angs.Count > count)
        {
            for (int i = angs.Count - 1; i >= count; i--)
            {
                angs.RemoveAt(i);
            }
        }

        if (speeds.Count < count)
        {
            int _count1 = count - speeds.Count;
            for (int i = 0; i < _count1; i++)
            {
                speeds.Add(0);
            }
        }
        if (speeds.Count > count)
        {
            for (int i = speeds.Count - 1; i >= count; i--)
            {
                speeds.RemoveAt(i);
            }
        }

        if (posList.Count < count)
        {
            int _count1 = count - posList.Count;
            for (int i = 0; i < _count1; i++)
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
    
    private void Update()
    {
        ReSizeToVFPlayer();//reset size 
        int index1 = 0;
        for(int i=0; i<vfPlayer.ch_master.selectBones.Count; i++)
        {
            Transform boneTarget1 = vfPlayer.ch_master.selectBones[i].Bone;//目标骨骼
            float timer = vfPlayer.ch_master.timer;
            if (cameraController1.screenMode == CameraController.ScreenMode.Two)
            {
                posList[index1] = cameraController1.cam_2.WorldToScreenPoint(boneTarget1.position);
            }
            else
            {
                posList[index1] = Camera.main.WorldToScreenPoint(boneTarget1.position);
            }
            Vector3 v1 = (boneTarget1.parent.position - boneTarget1.position).normalized;
            //Debug.Log("local: " + boneTarget1.name);
            //Debug.Log("parent: " + boneTarget1.parent.name);
            Vector3 v2 = Vector3.zero;
            for (int j = 0; j < boneTarget1.childCount; j++)
            {
                if (vfPlayer.ch_master.bm_p1.boneMatch.GetIndexOfMatchedT(boneTarget1.GetChild(j)) != -1)
                {
                    v2 = (boneTarget1.GetChild(j).position - boneTarget1.position).normalized;
                    //Debug.Log("child: " + boneTarget1.GetChild(j).name);
                    break;
                }
            }
            Debug.DrawRay(boneTarget1.position, v1, Color.blue);
            Debug.DrawRay(boneTarget1.position, v2, Color.green);
            int count = 10;
            SplineComputer sp1 = createdSplineC[index1].GetComponent<SplineComputer>();
            sp1.gameObject.layer = LayerMask.NameToLayer("cam1");//set layer
            SplinePoint[] sps = new SplinePoint[count + 1];
            for (int j = 0; j <= count; j++)
            {
                Debug.DrawRay(boneTarget1.position, Vector3.Slerp(v1, v2, (float)j / (float)count), Color.yellow);
                sps[j].position = boneTarget1.position + (Vector3.Slerp(v1, v2, (float)j / (float)count)) * 0.2f;
                sps[j].color = Color.yellow;
                sps[j].size = 0.02f;
            }
            angs[index1] = Vector3.Angle(v1, v2);
            sp1.SetPoints(sps);
            createdSplineCText[index1].transform.rotation = sceneCamera.transform.rotation;
            createdSplineCText[index1].transform.position = boneTarget1.position + sceneCamera.transform.up * Mathf.Min(0.02f * Vector3.Distance(sceneCamera.transform.position, boneTarget1.position), 0.02f) - sceneCamera.transform.forward * Mathf.Min(0.1f * Vector3.Distance(sceneCamera.transform.position, boneTarget1.position), 0.1f);
            createdSplineCText[index1].transform.localScale = Vector3.one * Mathf.Min(0.0035f * Vector3.Distance(sceneCamera.transform.position, boneTarget1.position), 0.0035f);
            createdSplineCText[index1].text = Vector3.Angle(v1, v2).ToString()+"\n";

            float fps = 30f;
            float jiange1 = 1f / fps;
            jiange1 = jiange1 / 6f;
            int boneIndexInPlayer = vfPlayer.ch_master.bm_p1.boneMatch.GetIndexOfBoneType(vfPlayer.ch_master.selectBones[i].Type);
            Quaternion q1 = vfPlayer.ch_master.bm_p1.bonePlayers[boneIndexInPlayer].EvaluateThisFrame_Curve(timer);
            Quaternion q2 = vfPlayer.ch_master.bm_p1.bonePlayers[boneIndexInPlayer].EvaluateThisFrame_Curve(timer + jiange1);
            speeds[index1] = Quaternion.Angle(q1, q2) / jiange1;
            index1++;
        }
        for (int i = 0; i < vfPlayer.ch_student.selectBones.Count; i++)
        {
            Transform boneTarget1 = vfPlayer.ch_student.selectBones[i].Bone;
            float timer = vfPlayer.ch_student.timer;
            posList[index1] = Camera.main.WorldToScreenPoint(boneTarget1.position);
            Vector3 v1 = (boneTarget1.parent.position - boneTarget1.position).normalized;
            Vector3 v2 = Vector3.zero;
            for (int j = 0; j < boneTarget1.childCount; j++)
            {
                if (vfPlayer.ch_student.bm_p1.boneMatch.GetIndexOfMatchedT(boneTarget1.GetChild(j)) != -1)
                {
                    v2 = (boneTarget1.GetChild(j).position - boneTarget1.position).normalized;
                    break;
                }
            }
            Debug.DrawRay(boneTarget1.position, v1, Color.blue);
            Debug.DrawRay(boneTarget1.position, v2, Color.green);
            int count = 10;
            SplineComputer sp1 = createdSplineC[index1].GetComponent<SplineComputer>();
            sp1.gameObject.layer = LayerMask.NameToLayer("cam2");//set layer
            SplinePoint[] sps = new SplinePoint[count + 1];
            for (int j = 0; j <= count; j++)
            {
                Debug.DrawRay(boneTarget1.position, Vector3.Slerp(v1, v2, (float)j / (float)count), Color.yellow);
                sps[j].position = boneTarget1.position + (Vector3.Slerp(v1, v2, (float)j / (float)count)) * 0.2f;
                sps[j].color = Color.yellow;
                sps[j].size = 0.02f;
            }
            angs[index1] = Vector3.Angle(v1, v2);
            sp1.SetPoints(sps);

            float fps = 30f;
            float jiange1 = 1f / fps;
            jiange1 = jiange1 / 6f;
            int boneIndexInPlayer = vfPlayer.ch_student.bm_p1.boneMatch.GetIndexOfBoneType(vfPlayer.ch_student.selectBones[i].Type);
            Quaternion q1 = vfPlayer.ch_student.bm_p1.bonePlayers[boneIndexInPlayer].EvaluateThisFrame_Curve(timer);
            Quaternion q2 = vfPlayer.ch_student.bm_p1.bonePlayers[boneIndexInPlayer].EvaluateThisFrame_Curve(timer + jiange1);
            speeds[index1] = Quaternion.Angle(q1, q2) / jiange1;
            index1++;
        }

        
        int selectedCount = vfPlayer.ch_master.selectBones.Count + vfPlayer.ch_student.selectBones.Count;
        boneName.text = "骨骼曲线";
        boneIntroduction.text = "选择了: [<color=blue>" + selectedCount.ToString() + "</color>]个骨骼";

        for (int i = 0; i < vfPlayer.ch_master.selectBones.Count; i++)
        {
            vfPlayer.ch_master.selectBones[i].BoneRender.material.color = Color.red;
            /*for (int j = 0; j < vfPlayer.ch_master.selectBones[i].Bone.childCount; j++)
            {
                Transform child1 = vfPlayer.ch_master.selectBones[i].Bone.GetChild(j);
                if (child1.GetComponent<Renderer>() != null)
                {
                    Color c1 = child1.GetComponent<Renderer>().material.color;
                    c1.r = 1f;
                    c1.g = 0f;
                    c1.b = 0f;
                    child1.GetComponent<Renderer>().material.color = c1;
                    break;
                }
            }*/
        }
        for (int i = 0; i < vfPlayer.ch_student.selectBones.Count; i++)
        {
            vfPlayer.ch_student.selectBones[i].BoneRender.material.color = Color.red;
        }

        //glDraw.Check();
    } 
    /// <summary>
    /// 在销毁时，由RightStackController调用
    /// </summary>
    public override void BeforeDestroy()
    { 
        for(int i=0; i<vfPlayer.ch_master.selectBones.Count; i++)
        {
            vfPlayer.ch_master.selectBones[i].BoneRender.material.color =
                vfPlayer.ch_master.selectBones[i].SrcColor;
        }
        for (int i = 0; i < vfPlayer.ch_student.selectBones.Count; i++)
        {
            vfPlayer.ch_student.selectBones[i].BoneRender.material.color =
                vfPlayer.ch_student.selectBones[i].SrcColor;
        }
        vfPlayer.ch_master.selectBones.Clear();
        vfPlayer.ch_student.selectBones.Clear();
        for(int i=0;i<createdSplineC.Count; i++)
        { 
            Destroy(createdSplineC[i]); 
        }
        createdSplineC.Clear();
        vfPlayer.HideCurveShowWindow();
    }

   


    
    private IEnumerator ie_openWindow()
    {
        vfPlayer.SetStop(true);
        if(curveSelectDropdown.value < boneDeals.Count)
        {
            yield return StartCoroutine(boneDeals[curveSelectDropdown.value].DealCalculate());
        }
        else
        {
            Debug.LogError("ERROR!!   curveSelectDropdown.value >= boneDeals.Count");
            yield break;
        } 

        if (curveSelectDropdown.value == 0)//夹角
        {
            
        } 
        else if (curveSelectDropdown.value == 1)//speed
        {
            
        }
        else if (curveSelectDropdown.value == 2)//加速度
        {
           
        }
        else if (curveSelectDropdown.value == 3)//根骨高度
        {
            
        }
        else if (curveSelectDropdown.value == 4)//脊柱垂直角度
        {
           
        }
        else if (curveSelectDropdown.value == 5)//肩胯角
        {

        }
        else if (curveSelectDropdown.value == 6)
        {

        }
        /*
        else if (curveSelectDropdown.value == 5)
        {
            Color mainColor1 = Color.white;
            List<Color> colors = new List<Color>();
            AnimationCurve curve1 = null;
            List<AnimationCurve> curves = new List<AnimationCurve>();
            for (int i = 0; i < vfPlayer.ch_master.selectBones.Count; i++)
            {
                int index1 = vfPlayer.ch_master.GetIndexOfSelectBone(vfPlayer.ch_master.selectBones[i].Type);
                if (index1 != -1)
                {
                    int i_player = vfPlayer.ch_master.bm_p1.GetPlayerIndexByBoneType(vfPlayer.ch_master.selectBones[i].Type);
                    if (i_player != -1)
                    {
                        if (curve1 == null)
                        {
                            curve1 = vfPlayer.ch_master.bm_p1.bonePlayers[i_player].GetCurve3_Roa_X();
                            mainColor1 = Color.red;
                        }
                        else
                        {
                            curves.Add(vfPlayer.ch_master.bm_p1.bonePlayers[i_player].GetCurve3_Roa_X());
                            colors.Add(Color.red);
                        }
                    }
                }
            }
            for (int i = 0; i < vfPlayer.ch_student.selectBones.Count; i++)
            {
                int index1 = vfPlayer.ch_student.GetIndexOfSelectBone(vfPlayer.ch_student.selectBones[i].Type);
                if (index1 != -1)
                {
                    int i_player = vfPlayer.ch_student.bm_p1.GetPlayerIndexByBoneType(vfPlayer.ch_student.selectBones[i].Type);
                    if (i_player != -1)
                    {
                        if (curve1 == null)
                        {
                            curve1 = vfPlayer.ch_student.bm_p1.bonePlayers[i_player].GetCurve3_Roa_X();
                            mainColor1 = Color.blue;
                        }
                        else
                        {
                            curves.Add(vfPlayer.ch_student.bm_p1.bonePlayers[i_player].GetCurve3_Roa_X());
                            colors.Add(Color.blue);
                        }
                    }
                }
            }
            vfPlayer.curveShowWindowControl.SetCurve(curve1, mainColor1);
            vfPlayer.curveShowWindowControl.SetCurves(curves, colors);
        }
        else if (curveSelectDropdown.value == 6)
        {
            Color mainColor1 = Color.white;
            List<Color> colors = new List<Color>();
            AnimationCurve curve1 = null;
            List<AnimationCurve> curves = new List<AnimationCurve>();
            for (int i = 0; i < vfPlayer.ch_master.selectBones.Count; i++)
            {
                int index1 = vfPlayer.ch_master.GetIndexOfSelectBone(vfPlayer.ch_master.selectBones[i].Type);
                if (index1 != -1)
                {
                    int i_player = vfPlayer.ch_master.bm_p1.GetPlayerIndexByBoneType(vfPlayer.ch_master.selectBones[i].Type);
                    if (i_player != -1)
                    {
                        if (curve1 == null)
                        {
                            curve1 = vfPlayer.ch_master.bm_p1.bonePlayers[i_player].GetCurve3_Roa_Y();
                            mainColor1 = Color.red;
                        }
                        else
                        {
                            curves.Add(vfPlayer.ch_master.bm_p1.bonePlayers[i_player].GetCurve3_Roa_Y());
                            colors.Add(Color.red);
                        }
                    }
                }
            }
            for (int i = 0; i < vfPlayer.ch_student.selectBones.Count; i++)
            {
                int index1 = vfPlayer.ch_student.GetIndexOfSelectBone(vfPlayer.ch_student.selectBones[i].Type);
                if (index1 != -1)
                {
                    int i_player = vfPlayer.ch_student.bm_p1.GetPlayerIndexByBoneType(vfPlayer.ch_student.selectBones[i].Type);
                    if (i_player != -1)
                    {
                        if (curve1 == null)
                        {
                            curve1 = vfPlayer.ch_student.bm_p1.bonePlayers[i_player].GetCurve3_Roa_Y();
                            mainColor1 = Color.blue;
                        }
                        else
                        {
                            curves.Add(vfPlayer.ch_student.bm_p1.bonePlayers[i_player].GetCurve3_Roa_Y());
                            colors.Add(Color.blue);
                        }
                    }
                }
            }
            vfPlayer.curveShowWindowControl.SetCurve(curve1, mainColor1);
            vfPlayer.curveShowWindowControl.SetCurves(curves, colors);
        }
        else if (curveSelectDropdown.value == 7)
        {
            Color mainColor1 = Color.white;
            List<Color> colors = new List<Color>();
            AnimationCurve curve1 = null;
            List<AnimationCurve> curves = new List<AnimationCurve>();
            for (int i = 0; i < vfPlayer.ch_master.selectBones.Count; i++)
            {
                int index1 = vfPlayer.ch_master.GetIndexOfSelectBone(vfPlayer.ch_master.selectBones[i].Type);
                if (index1 != -1)
                {
                    int i_player = vfPlayer.ch_master.bm_p1.GetPlayerIndexByBoneType(vfPlayer.ch_master.selectBones[i].Type);
                    if (i_player != -1)
                    {
                        if (curve1 == null)
                        {
                            curve1 = vfPlayer.ch_master.bm_p1.bonePlayers[i_player].GetCurve3_Roa_Z();
                            mainColor1 = Color.red;
                        }
                        else
                        {
                            curves.Add(vfPlayer.ch_master.bm_p1.bonePlayers[i_player].GetCurve3_Roa_Z());
                            colors.Add(Color.red);
                        }
                    }
                }
            }
            for (int i = 0; i < vfPlayer.ch_student.selectBones.Count; i++)
            {
                int index1 = vfPlayer.ch_student.GetIndexOfSelectBone(vfPlayer.ch_student.selectBones[i].Type);
                if (index1 != -1)
                {
                    int i_player = vfPlayer.ch_student.bm_p1.GetPlayerIndexByBoneType(vfPlayer.ch_student.selectBones[i].Type);
                    if (i_player != -1)
                    {
                        if (curve1 == null)
                        {
                            curve1 = vfPlayer.ch_student.bm_p1.bonePlayers[i_player].GetCurve3_Roa_Z();
                            mainColor1 = Color.blue;
                        }
                        else
                        {
                            curves.Add(vfPlayer.ch_student.bm_p1.bonePlayers[i_player].GetCurve3_Roa_Z());
                            colors.Add(Color.blue);
                        }
                    }
                }
            }
            vfPlayer.curveShowWindowControl.SetCurve(curve1, mainColor1);
            vfPlayer.curveShowWindowControl.SetCurves(curves, colors);
        }
        */

        if (vfPlayer.ch_master.selectBones.Count == 0 && 
            vfPlayer.ch_student.selectBones.Count == 0 &&
             boneDeals[curveSelectDropdown.value].needSelectBone)
        {
            vfPlayer.curveShowWindowControl.UseNoCurveMask(true);
        }
        else
        {
            vfPlayer.curveShowWindowControl.UseNoCurveMask(false);
        }

        vfPlayer.SetStop(false);
        
        yield return new WaitForSeconds(0.001f); 
    }

    /// <summary>
    /// 打开详细的窗口
    /// </summary>
    public void Button_OpenWindow()
    {
        if (curveSelectDropdown.value < boneDeals.Count)
        {
            int needCalculateCount = boneDeals[curveSelectDropdown.value].NeedCalculateCount();
            if (needCalculateCount == 0)
            {
                StartCoroutine(ie_openWindow());
            }
            else//需要计算
            {
                vfPlayer.m_checkWindow.Show(delegate ()
                {   
                    Debug.Log("check");
                    StartCoroutine(ie_openWindow());
                }, delegate ()
                {
                    Debug.Log("cancel");
                }, "有"+needCalculateCount+"个数据需要计算，确认吗？", "确认窗口");
            }
        }
        else
        {
            Debug.LogError("curveSelectDropdown.value >= boneDeals.Count !!!!!!");
        }  
    }

     
    

    public void CurveShowTypeChange(int _type)
    {
        if(vfPlayer.curveShowWindowControl != null)
        {
            if (vfPlayer.curveShowWindowControl.gameObject.activeInHierarchy)
            {
                Button_OpenWindow();
            }
        } 
    }


    private void OnGUI()
    {
        if( cameraController1 != null)
        {
            /*
            for(int i=0; i<angs.Count; i++)
            {
                Vector3 v1 = posList[i];
                float w1 = 100f;
                float h1 = 100f;
                GUI.color = Color.green;
                GUI.Label(new Rect(v1.x - 0f * w1, Screen.height - v1.y - 0f * h1, w1, h1), "角: " + angs[i].ToString());
                GUI.Label(new Rect(v1.x - 0f * w1, Screen.height - v1.y - 0f * h1 + 40f, w1, h1), "速度: " + speeds[i].ToString());
            }
            */
        }
      
    }

    public void OushiDis()
    {
        Debug.Log("oushi button test");
    }
    
    //inputfield control
   
}

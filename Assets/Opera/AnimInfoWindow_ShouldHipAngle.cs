using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JAnimationSystem;
using Dreamteck.Splines;
using JAnimCurves;

public class AnimInfoWindow_ShouldHipAngle : MonoBehaviour
{
    public SplineComputer splineC1, splineC2, spline_Angle;
    public Text splineText;
    public AnimInfoWindowControl control;
    private Transform shoulderL, shoulderR;
    private Transform hipL, hipR;
    public Transform SplineC_TextParentCanvas;
    /// <summary>
    /// 场景视图的Camera.
    /// </summary>
    Camera sceneCamera;

    void Start()
    {
        SplineC_TextParentCanvas.gameObject.name = "AnimInfoWindow_ShouldHipAngle_Canvas";
        SplineC_TextParentCanvas.SetParent(null);
        SplineC_TextParentCanvas.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
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
        {
            Destroy(SplineC_TextParentCanvas.gameObject);
        }
      
    }

    void Update()
    {
        CheckTransforms();
        CalculateLines();
    }

    public bool IsShowing()
    {
        return splineC1.gameObject.activeSelf;
    }
    public void ShowOrHide()
    {
        if (IsShowing())
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    public void Show()
    {
       
        splineText.gameObject.SetActive(true);
        splineC1.gameObject.SetActive(true);
        splineC2.gameObject.SetActive(true);
        spline_Angle.gameObject.SetActive(true);
    }
    public void Hide()
    {
        splineText.gameObject.SetActive(false);
        splineC1.gameObject.SetActive(false);
        splineC2.gameObject.SetActive(false);
        spline_Angle.gameObject.SetActive(false);
    }

    void CalculateLines()
    {
        Vector3 shoulderDir = shoulderR.position - shoulderL.position;
        shoulderDir.Normalize();
        Vector3 hipDir = hipR.position - hipL.position;
        hipDir.Normalize();

      
        SplinePoint[] sps = new SplinePoint[2];
        for (int j = 0; j < 2; j++)
        {
            sps[j].position = shoulderR.position - shoulderDir * j * 1f;
            sps[j].color = Color.yellow;
            sps[j].size = 0.01f;
        }
        splineC1.SetPoints(sps);

        Vector3 endPos1 = shoulderR.position - shoulderDir * 1f;
        SplinePoint[] sps2 = new SplinePoint[2];
        for (int j = 0; j < 2; j++)
        {
            sps2[j].position = endPos1 + hipDir * j * 1f;
            sps2[j].color = Color.yellow;
            sps2[j].size = 0.01f;
        }
        splineC2.SetPoints(sps2);
        int countAngle = 5;
        SplinePoint[] sps_Angle = new SplinePoint[countAngle + 1]; 
        for (int j = 0; j <= countAngle; j++)
        { 
            sps_Angle[j].position = endPos1 + (Vector3.Slerp(shoulderDir, hipDir, (float)j / (float)countAngle)) * 0.2f;
            sps_Angle[j].color = Color.yellow;
            sps_Angle[j].size = 0.01f;
        }
        spline_Angle.SetPoints(sps_Angle);

        splineText.transform.rotation = sceneCamera.transform.rotation;
        splineText.transform.position = endPos1 + sceneCamera.transform.up * Mathf.Min(0.02f * Vector3.Distance(sceneCamera.transform.position, endPos1), 0.02f) - sceneCamera.transform.forward * Mathf.Min(0.1f * Vector3.Distance(sceneCamera.transform.position, endPos1), 0.1f);
        splineText.transform.localScale = Vector3.one * Mathf.Min(0.35f * Vector3.Distance(sceneCamera.transform.position, endPos1), 0.35f);
        splineText.text = Vector3.Angle(shoulderDir, hipDir).ToString() + "\n";
    }

    void CheckTransforms()
    {
        if(shoulderL == null)
        {
            if(control.isMaster)
            {
                shoulderL = control.vfPlayer.ch_master.bm_p1.boneMatch.GetT_ByBoneType(JAnimationData.BoneType.leftArm); 
            }
            else
            {
                shoulderL = control.vfPlayer.ch_student.bm_p1.boneMatch.GetT_ByBoneType(JAnimationData.BoneType.leftArm); 
            }
        }
        if (shoulderR == null)
        {
            if (control.isMaster)
            { 
                shoulderR = control.vfPlayer.ch_master.bm_p1.boneMatch.GetT_ByBoneType(JAnimationData.BoneType.rightArm);
            }
            else
            { 
                shoulderR = control.vfPlayer.ch_student.bm_p1.boneMatch.GetT_ByBoneType(JAnimationData.BoneType.rightArm);
            }
        }
        if (hipL == null)
        {
            if (control.isMaster)
            {
                hipL = control.vfPlayer.ch_master.bm_p1.boneMatch.GetT_ByBoneType(JAnimationData.BoneType.leftUpLeg);
            }
            else
            {
                hipL = control.vfPlayer.ch_student.bm_p1.boneMatch.GetT_ByBoneType(JAnimationData.BoneType.leftUpLeg);
            }
        }
        if (hipR == null)
        {
            if (control.isMaster)
            {
                hipR = control.vfPlayer.ch_master.bm_p1.boneMatch.GetT_ByBoneType(JAnimationData.BoneType.rightUpLeg);
            }
            else
            {
                hipR = control.vfPlayer.ch_student.bm_p1.boneMatch.GetT_ByBoneType(JAnimationData.BoneType.rightUpLeg);
            }
        }
    }
}

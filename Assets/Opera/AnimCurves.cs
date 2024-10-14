using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JVehicleFrameSystem;
using System.IO;


namespace JAnimCurves 
{
    public enum animCurveType
    {
        speed,
        angle,
        acceleration,
        speed_world_x,
        speed_world_y,
        speed_world_z,
        rootHeight,
        spineVerticalAngle,
        weightCenterRange,
        shoulderHipAngle,
        shoulderRoateYSpeed
    }
    [System.Serializable]
    public class AnimOneCurve_ForSave
    {
        public List<float> curveDataList = new List<float>();
        public List<float> curveTimeList = new List<float>();
        public float min = 0f;
        public float max = 0f;
        public animCurveType type = animCurveType.speed;
        public JAnimationData.BoneType boneType;

        public AnimOneCurve_ForSave(animCurveType _type, JAnimationData.BoneType _boneType)
        {
            curveDataList = new List<float>();
            curveTimeList = new List<float>();
            type = _type;
            boneType = _boneType;
        }
        public void AddData(float data1, float time1)
        {
            if(curveDataList.Count == 0)
            {
                max = data1;
                min = data1;
            }
            else
            {
                if(data1 > max) max = data1;
                if (data1 < min) min = data1;
            }
            curveDataList.Add(data1);
            curveTimeList.Add(time1);
        }
    }
    [System.Serializable]
    public class AnimOneCurve_InGame
    {
        public AnimationCurve curve = null;
        public float min = 0f;
        public float max = 0f;
        public animCurveType type = animCurveType.speed;
        public JAnimationData.BoneType boneType;
         
        public AnimOneCurve_InGame(AnimOneCurve_ForSave data1)
        {
            curve = new AnimationCurve();
            type = data1.type;
            boneType = data1.boneType;

            for (int i = 0; i < data1.curveDataList.Count; i++)
            {
                curve.AddKey(data1.curveTimeList[i], data1.curveDataList[i]);
            }
            max = data1.max;
            min = data1.min;
        }
    }
   
 

    [System.Serializable]
    public class AnimCurvesSaver
    { 

        public static string AnimCurvesSavePath
        {
            get
            {
                return Application.persistentDataPath + "/AnimCurves";
            }
        }

       


        
        public AnimOneCurve_ForSave LoadOne(string fileName, JAnimationData.BoneType boneType, animCurveType type)
        {
            if (string.IsNullOrEmpty(fileName) == false)
            {
                string path = AnimCurvesSavePath;
                if (IOHelper.IsDirectoryExists(path))
                {
                    string fileFullName = path + "/" + fileName + "_" + boneType.ToString() + "_" + type.ToString() + ".JAnimCurves";
                    if (IOHelper.IsFileExists(fileFullName))
                    {
                        string st1 = (string)IOHelper.GetData(fileFullName, typeof(string));

                        if (!string.IsNullOrEmpty(st1))
                        {
                            Debug.Log("Loaded AnimCurve from [" + fileFullName + "]");
                            return JsonUtility.FromJson<AnimOneCurve_ForSave>(st1);
                        }
                    }
                }
            }
            Debug.LogWarning("Loaded AnimCurve from [" + AnimCurvesSavePath + "/" + fileName + "_" + boneType.ToString() + "_" + type.ToString() + ".JAnimCurves" + "] failed !!!!");
            return null;
        }
        public AnimOneCurve_ForSave LoadOne(string fullfilename)
        {
            if (string.IsNullOrEmpty(fullfilename) == false)
            {
                string path = AnimCurvesSavePath;
                if (IOHelper.IsDirectoryExists(path))
                {
                    string fileFullName = path + "/" + fullfilename;
                    if (IOHelper.IsFileExists(fileFullName))
                    {
                        string st1 = (string)IOHelper.GetData(fileFullName, typeof(string));

                        if (!string.IsNullOrEmpty(st1))
                        {
                            Debug.Log("Loaded AnimCurve from [" + fileFullName + "]");
                            return JsonUtility.FromJson<AnimOneCurve_ForSave>(st1);
                        }
                    }
                }
            }
            return null;
        }
        public void SaveOne(AnimOneCurve_ForSave data, string fileName, JAnimationData.BoneType boneType)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            string path = AnimCurvesSavePath;
            if (!IOHelper.IsDirectoryExists(path))
            {
                IOHelper.CreateDirectory(path);
            }
            string fileFullName = path + "/" + fileName+"_"+ boneType.ToString()+"_"+data.type.ToString() + ".JAnimCurves";
            if (IOHelper.IsFileExists(fileFullName))
            {
                //删除原有
                File.Delete(fileFullName);
            }

            string st1 = JsonUtility.ToJson(data);
            IOHelper.SetData(fileFullName, st1);
            Debug.Log("Saved at [" + fileFullName + "]");
        }
    }

  
}

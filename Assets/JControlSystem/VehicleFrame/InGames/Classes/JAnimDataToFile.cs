using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JVehicleFrameSystem;
using System.IO;

public class JAnimDataToFile
{

    [System.Serializable]
    public class JAnimDataSaved
    {
        //public string introduction = "";
        public JAnimationData.JAD jad;
        public BoneFramework boneFramework;
        public string modelPrefabName;

        public JAnimDataSaved()
        {
            modelPrefabName = "";
            //introduction = "";
        }
    
    }

    [System.Serializable]
    public class JAnimRanges
    {
        /// <summary>
        /// 一些标定选段
        /// </summary>
        public List<AnimRangeControl.SelectOne> selectRanges = new List<AnimRangeControl.SelectOne>();

        public JAnimRanges()
        {
            selectRanges = new List<AnimRangeControl.SelectOne>();
        }
    }

    /// <summary>
    /// ( JADFileName是指没有后缀和前面路径的JAD录制数据文件名 )
    /// </summary> 
    public static void SaveJAnimRanges (string path, string JADFileName, JAnimRanges rangesData)
    {
        if (string.IsNullOrEmpty(path)) return;
        if (string.IsNullOrEmpty(JADFileName)) return;
        string fileFullName = path + "/" + JADFileName + ".JARanges";
        if(IOHelper.IsDirectoryExists(path) == false)
        {
            IOHelper.CreateDirectory(path);
        }
        if (IOHelper.IsFileExists(fileFullName))
        {
            File.Delete(fileFullName);
        }
        JAnimRanges _data = new JAnimRanges();
        _data.selectRanges = rangesData.selectRanges;
        string st1 = JsonUtility.ToJson(_data);
        IOHelper.SetData(fileFullName, st1);
        Debug.Log("Saved [" + JADFileName + "]'s Ranges at [" + fileFullName + "]");
    }

    /// <summary>
    ///  ( JADFileName是指没有后缀和前面路径的JAD录制数据文件名 )
    /// </summary> 
    public static JAnimRanges LoadJAnimRanges(string path, string JADFileName)
    {
        if (string.IsNullOrEmpty(path)) return null;
        if (string.IsNullOrEmpty(JADFileName)) return null;
        string fileFullName = path + "/" + JADFileName + ".JARanges";
        if (IOHelper.IsFileExists(fileFullName))
        {
            string st1 = (string)IOHelper.GetData(fileFullName, typeof(string)); 
            if (!string.IsNullOrEmpty(st1))
            {
                Debug.Log("Loaded JARanges from [" + fileFullName + "]");
                return JsonUtility.FromJson<JAnimRanges>(st1);
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    public static string JADSavePath
    {
        get
        {
            
            return Application.persistentDataPath + "/Recodes";
        }
    }

  

    /// <summary>
    /// _rewrite表示是否覆盖已存在的文件。 这个函数会返回修正的文件名。
    /// </summary> 
    public static string SaveJAD(string path, string fileName, JAnimationData _jAnimData, bool _rewrite)
    {
        return SaveJAD(path, fileName, _jAnimData.jAD, _jAnimData.jAD.boneFramework, _rewrite, "");
    }


    /// <summary>
    /// _rewrite表示是否覆盖已存在的文件。 这个函数会返回修正的文件名。
    /// </summary> 
    public static string SaveJAD( string path, string fileName, JAnimationData.JAD _jAD, BoneFramework _boneFramework, bool _rewrite, string _modelPrefabName = "")
    {
        if (string.IsNullOrEmpty(fileName)) return "";
        if(! IOHelper.IsDirectoryExists(path))
        {
            IOHelper.CreateDirectory(path);
        }
        string filePathName = path + "/" + fileName;
        string trueFileName = fileName;
        if (IOHelper.IsFileExists(filePathName + ".JADF"))
        {
            if (_rewrite == false)//不覆盖
            {
                int i = 1;
                string newFilePathName = filePathName + "_" + i.ToString();
                trueFileName = fileName + "_" + i.ToString();
                for (; i < 100; i++)
                {
                    if (IOHelper.IsFileExists(newFilePathName + ".JADF") == false)
                    {
                        filePathName = newFilePathName;
                        break;
                    }
                    else
                    {
                        newFilePathName = filePathName + "_" + (i + 1).ToString();
                        trueFileName = fileName + "_" + (i + 1).ToString();
                        if (i == 99)
                        {
                            Debug.Log("Error ! The same name has exist here for 100 ! Can not create new file !");
                            return "";
                        }
                    }
                }
            }
            else//覆盖
            {
                //删除原有
                File.Delete(filePathName + ".JADF");
            }
        }
        
        JAnimDataSaved dataSaved = new JAnimDataSaved();
        dataSaved.jad = _jAD;
        dataSaved.boneFramework = _boneFramework;
        dataSaved.modelPrefabName = _modelPrefabName;

        string st1 = JsonUtility.ToJson(dataSaved);
        IOHelper.SetData(filePathName + ".JADF", st1);
        Debug.Log("Saved [" + trueFileName + "] at [" + filePathName + ".JADF" + "]");
        return trueFileName + ".JADF";
    }


    public static JAnimDataSaved LoadJAD (string path, string fileName)
    {
        if (!IOHelper.IsDirectoryExists(path))
        {
            return null;
        }

        string filePath = path + "/" + fileName + ".JADF";
        Debug.Log("filePath: " + filePath);
        string st1 = (string)IOHelper.GetData(filePath, typeof(string));
     
        if(!string.IsNullOrEmpty(st1))
        {
            Debug.Log("Loaded from [" + filePath + "]");
            return JsonUtility.FromJson<JAnimDataSaved>(st1);
        }
        else
        {
            return null;
        }
    }

}

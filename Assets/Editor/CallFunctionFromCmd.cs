using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CallFunctionFromCmd
{
    
    public static void CallMyFunction()
    {
        string scenePath = "Assets/Scenes/ui TEST1.unity";
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject obj = GameObject.Find("pythonUnityTest");
        if(obj != null)
        {
            ZPythonUnityTest pyUnityTestScript = obj.GetComponent<ZPythonUnityTest>();
            if(pyUnityTestScript != null)
            {
                pyUnityTestScript.qibaScoreCal();
            }
            else
            {
                Debug.LogError("zpythonUnitytest not found on the Gameobject");
            }
        }
        else
        {
            Debug.LogError("GameObject Not Found");
        }
    }

    public static void CalCulateAngleDiff()
    {
        string scenePath = "Assets/Scenes/ui TEST1.unity";
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        GameObject obj = GameObject.Find("pythonUnityTest");
        if (obj != null)
        {
            ZPythonUnityTest pyUnityTestScript = obj.GetComponent<ZPythonUnityTest>();
            if (pyUnityTestScript != null)
            {
                pyUnityTestScript.AngleDiffDataCal();
            }
            else
            {
                Debug.LogError("zpythonUnitytest not found on the Gameobject");
            }
        }
        else
        {
            Debug.LogError("GameObject Not Found");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class oushiDIsCurveShow : MonoBehaviour
{
   
    private AnimationCurve oushiDisCurve = new AnimationCurve();
    string dataFilePath;
    public Color curveColor = Color.blue;
    public Color xColor = Color.black;
    public Color yColor = Color.black;
    //重新编译
    public float curveWidth = 2f;
    public float axisWidth = 2f;

    void Start()
    {
        // 从文件中读取曲线数据
        ReadCurveDataFromFile();
        OnGUI();
    }
    void OnGUI()
    {
        var curveRect = GUILayoutUtility.GetRect(800, 500);
        curveRect.position = this.transform.position;
        oushiDisCurve = EditorGUI.CurveField(curveRect, oushiDisCurve);
        DrawCurve();
    }
    void DrawCurve()
    {
        Vector2 curvePos = new Vector2(500, 500);
        var curveRect = new Rect(curvePos, GetComponent<RectTransform>().rect.size);
        var xPadding = 20f;
        var yPadding = 20f;

        // Draw x-axis
        var xAxisStart = new Vector2(xPadding, curveRect.height - yPadding);
        var xAxisEnd = new Vector2(curveRect.width - xPadding, curveRect.height - yPadding);

        Handles.color = xColor;
        Handles.DrawAAPolyLine(axisWidth, xAxisStart, xAxisEnd);

        // Draw y-axis
        var yAxisStart = new Vector2(xPadding, curveRect.height - yPadding);
        var yAxisEnd = new Vector2(xPadding, yPadding);

        Handles.color = yColor;
        Handles.DrawAAPolyLine(axisWidth, yAxisStart, yAxisEnd);

        // Draw curve
        Handles.color = curveColor;
        var prevHandle = new Vector2(xPadding, oushiDisCurve.Evaluate(0) * (curveRect.height - yPadding * 2) + yPadding);
        for (float x = 1; x <= curveRect.width - xPadding * 2; x++)
        {
            float t = x / (curveRect.width - xPadding * 2);
            float value = oushiDisCurve.Evaluate(t);
            var handle = new Vector2(x + xPadding, value * (curveRect.height - yPadding * 2) + yPadding);
            Handles.DrawAAPolyLine(curveWidth, prevHandle, handle);
            prevHandle = handle;
        }
    }
    public void ReadCurveDataFromFile()
    {
        // 打开文件并读取数据（Assuming txt data is comma seperated values）
        dataFilePath = Application.dataPath + "/zTestData/oushiDisData.txt";
        using (StreamReader reader = new StreamReader(dataFilePath))
        {
            string line;
            float time, value;
            int count = 0;

            while ((line = reader.ReadLine()) != null)
            {
                string[] tokens = line.Split(':');
                if (tokens.Length == 2 && float.TryParse(tokens[0], out time) && float.TryParse(tokens[1], out value))
                {
                    Debug.Log("time: " + time);
                    Debug.Log("value: " + value);
                    oushiDisCurve.AddKey(time, value);

                    ++count;
                }
            }

            Debug.Log("Read " + count + " key frames for curve data");
        }
    }


}

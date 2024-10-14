using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;
using System.IO;
using XCharts;
public class zoushiDisLIneDraw : MonoBehaviour
{

    public GameObject oushiDisWindow;

    void Start()
    {

        ////oushiDisWindow.SetActive(false);
        //////读取数据
        ////GameObject sceneUI = GameObject.Find("UI");
        ////Debug.Log("UI: " + sceneUI);
        ////oushiDisWindow.transform.parent = sceneUI.transform;
        ////oushiDisWindow.transform.localPosition = Vector3.zero;
        ////oushiDisWindow.transform.localScale = Vector3.one;

        var fileOushiDisDataPath = Application.dataPath + "/zTestData/oushiDisData.txt";
        var sr = new StreamReader(fileOushiDisDataPath);
        List<float> data1 = new List<float>();
        List<float> data2 = new List<float>();

        float minDis = 999;
        float maxDis = 0;
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            var words = line.Split(':');
            var value1 = float.Parse(words[0]);
            var value2 = float.Parse(words[1]);
            data1.Add(value1);
            data2.Add(value2);

            minDis = Mathf.Min(minDis, value2);
            maxDis = Mathf.Max(maxDis, value2);
            //Debug.Log("time: " + value1);
            //Debug.Log("dis: " + value2);
        }
        sr.Close();

        //绘制折线图
        var chart = gameObject.GetComponent<LineChart>();
        if (chart == null)
        {
            chart = gameObject.AddComponent<LineChart>();
            chart.Init();
        }
        //标题
        var title = chart.EnsureChartComponent<Title>();
        title.text = "oushiDis";

        //提示框图例
        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.show = true;

        var legend = chart.EnsureChartComponent<Legend>();
        legend.show = false;

        //坐标轴
        var xAxis = chart.EnsureChartComponent<XAxis>();
        xAxis.splitNumber = 10;
        xAxis.boundaryGap = true;
        xAxis.type = Axis.AxisType.Category;

        var yAxis = chart.EnsureChartComponent<YAxis>();
        yAxis.type = Axis.AxisType.Value;
        yAxis.min = minDis - 10; // y轴数据最小值设置
        yAxis.max = maxDis + 10;
        yAxis.minMaxType = Axis.AxisMinMaxType.Custom;
        yAxis.axisLabel.formatter = "{value:F2}";//设置y轴小数位数精度

        //清空默认
        chart.RemoveData();
        chart.AddSerie<Line>("line");

        //写入数据
        for (int i = 0; i < data1.Count; i++)
        {
            string timeStr = data1[i].ToString();
            chart.AddXAxisData(timeStr);
            chart.AddData(0, data2[i]);
        }
        ////yAxis.axisLabel.numericFormatter ="{value:f1}";

    }

    //将master和student的oushidis画成折线图并在系统中显示
    public void ButtonOushiDis()
    {
        //窗口可视化
        if (oushiDisWindow.activeSelf)
        {
            oushiDisWindow.SetActive(false);
        }
        else
        {
            oushiDisWindow.SetActive(true);
            GameObject sceneUI = GameObject.Find("UI");
            Debug.Log("UI: " + sceneUI);
            oushiDisWindow.transform.parent = sceneUI.transform;
            oushiDisWindow.transform.localPosition = Vector3.zero;
            oushiDisWindow.transform.localScale = Vector3.one;


            //读取数据
            var fileOushiDisDataPath = Application.dataPath + "/zTestData/oushiDisData.txt";
            var sr = new StreamReader(fileOushiDisDataPath);
            List<float> data1 = new List<float>();
            List<float> data2 = new List<float>();
            float minDis = 999f;
            float maxDis = 0.0f;
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var words = line.Split(':');
                var value1 = float.Parse(words[0]);
                var value2 = float.Parse(words[1]);
                data1.Add(value1);
                data2.Add(value2);
                minDis = Mathf.Min(minDis, value2);
                maxDis = Mathf.Max(maxDis, value2);
                //Debug.Log("time: " + value1);
                //Debug.Log("dis: " + value2);
            }
            sr.Close();

            //绘制折线图
            var chart = gameObject.GetComponent<LineChart>();
            if (chart == null)
            {
                chart = gameObject.AddComponent<LineChart>();
                chart.Init();
            }
            //标题
            var title = chart.EnsureChartComponent<Title>();
            title.text = "oushiDis";

            //提示框图例
            var tooltip = chart.EnsureChartComponent<Tooltip>();
            tooltip.show = true;

            var legend = chart.EnsureChartComponent<Legend>();
            legend.show = false;

            //坐标轴
            var xAxis = chart.EnsureChartComponent<XAxis>();
            xAxis.splitNumber = 10;
            xAxis.boundaryGap = true;
            xAxis.type = Axis.AxisType.Category;

            var yAxis = chart.EnsureChartComponent<YAxis>();
            yAxis.type = Axis.AxisType.Value;
            yAxis.min = minDis - 10.0f;
            yAxis.max = maxDis + 10.0f;
            Debug.Log("minDis: " + yAxis.min);
            Debug.Log("maxDis: " + yAxis.max);
            yAxis.minMaxType = Axis.AxisMinMaxType.Custom;

            //清空默认
            chart.RemoveData();
            chart.AddSerie<Line>("line");

            //写入数据
            for (int i = 0; i < data1.Count; i++)
            {
                string timeStr = data1[i].ToString();
                chart.AddXAxisData(timeStr);
                chart.AddData(0, data2[i]);
            }
        }

    }

    public void close()
    {
        oushiDisWindow.SetActive(false);
    }

   
}

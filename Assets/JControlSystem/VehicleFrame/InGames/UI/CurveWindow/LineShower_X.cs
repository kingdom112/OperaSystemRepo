﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineShower_X : MaskableGraphic
{

    public List<float> Lines = new List<float>();
    public JMutiLineText_X textControl;
    public bool showTexts = true;
    public float Xchu = 1;
    public float x_min = 0f;
    public float m_LineWidth = 1;
    [Range(1, 10)]
    public int Acc = 2;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        var rect = this.rectTransform.rect;
        vh.Clear();
         
        for (int i = 0; i < Lines.Count; i++)
        {
            DrawAYLine(vh, rect, Lines[i]);
        }

        DrawTexts();


        for (int i = 0; i < vh.currentVertCount - 4; i += 4)
        {
            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i + 2, i + 3, i);
        }
        //Debug.Log("PopulateMesh..." + vh.currentVertCount);
    }

    private void DrawTexts()
    {
        if (textControl == null) return;

        if (showTexts)
        {
            if (textControl.xPosList.Count < Lines.Count)
            {
                int c1 = Lines.Count - textControl.xPosList.Count;
                for (int k = 0; k < c1; k++)
                {
                    textControl.xPosList.Add(0);
                }
            }
            else if (textControl.xPosList.Count > Lines.Count)
            {
                int c1 = textControl.xPosList.Count - Lines.Count;
                int p1 = textControl.xPosList.Count - 1;
                for (int k = 0; k < c1; k++)
                {
                    textControl.xPosList.RemoveAt(p1);
                    p1--;
                }
            }

            if (Lines.Count > 0)
            {

                for (int i = 0; i < Lines.Count; i++)
                {
                    textControl.drawOffset_X = textControl.rectTransform.rect.width / 2f - textControl.fontSize * 0.25f;
                    textControl.drawOffset_Y = textControl.rectTransform.rect.height;
                    textControl.xPosList[i] = (Lines[i] - x_min) / Xchu * textControl.rectTransform.rect.width - textControl.rectTransform.rect.width * 0.5f;
                }
            }
        }
        else
        {
            textControl.xPosList.Clear();
        }



    }

    private void DrawAYLine(VertexHelper vh, Rect rect, float x)
    {

        Vector2 pos1 = new Vector2((x - x_min) / Xchu * rect.width - rect.width * 0.5f, 0 - rect.height * 0.5f);
        Vector2 pos2 = new Vector2((x - x_min) / Xchu * rect.width - rect.width * 0.5f, rect.height * 0.5f);
        var quad = GenerateQuad(pos1, pos2);
        vh.AddUIVertexQuad(quad);

        /*Vector2 pos_first = new Vector2(rect.xMin, GetYFromY(y - y_min) * rect.height);

        for (float x = rect.xMin + Acc; x < rect.xMax; x += Acc)
        {
            Vector2 pos = new Vector2(x, GetYFromY(y - y_min) * rect.height);
            var quad = GenerateQuad(pos_first, pos);

            vh.AddUIVertexQuad(quad);

            pos_first = pos;
        }

        Vector2 pos_last = new Vector2(rect.xMax, GetYFromY(y - y_min) * rect.height);
        vh.AddUIVertexQuad(GenerateQuad(pos_first, pos_last));*/

        //var quad = GenerateQuad(new Vector2(rect.xMin, GetYFromY(y) * rect.height), new Vector2(rect.xMax, GetYFromY(y) * rect.height));

        // vh.AddUIVertexQuad(quad);

    }

    /*private float GetYFromY(float y)
    {
        return (y / Ychu - rectTransform.pivot.y);
    }*/


    private UIVertex[] GenerateQuad(Vector2 pos1, Vector2 pos2)
    {
        float dis = Vector2.Distance(pos1, pos2);
        float y = m_LineWidth * 0.5f * (pos2.x - pos1.x) / dis;
        float x = m_LineWidth * 0.5f * (pos2.y - pos1.y) / dis;

        if (y <= 0)
            y = -y;
        else
            x = -x;

        UIVertex[] vertex = new UIVertex[4];

        vertex[0].position = new Vector3(pos1.x + x, pos1.y + y);
        vertex[1].position = new Vector3(pos2.x + x, pos2.y + y);
        vertex[2].position = new Vector3(pos2.x - x, pos2.y - y);
        vertex[3].position = new Vector3(pos1.x - x, pos1.y - y);

        for (int i = 0; i < vertex.Length; i++)
        {
            vertex[i].color = color;
        }

        return vertex;
    }
}

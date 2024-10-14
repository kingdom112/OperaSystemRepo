using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowALine : MaskableGraphic
{
    private float y_min = 0f;
    private float y_max = 1f;
    private float y_division = 1f;
    public float m_LineWidth = 1;
    [Range(0.1f, 10f)]
    public float Acc = 1f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        var rect = this.rectTransform.rect;
        vh.Clear();
        /*
        Vector2 pos_first = new Vector2(rect.xMin, CalcY(0, y_min) * rect.height);
        for (float x = rect.xMin + Acc; x < rect.xMax; x += Acc)
        {
            Vector2 pos = new Vector2(x, CalcY((x - rect.xMin) / rect.width, y_min) * rect.height);
            var quad = GenerateQuad(pos_first, pos);
            vh.AddUIVertexQuad(quad);
            pos_first = pos;
        } */

        Vector2 pos_first = new Vector2(rect.xMin, CalcY(0, y_min) * rect.height);
        bool hasOverRange = false;
        float overRange_y = 0f;
        float _cy1 = CurveY(0f);
        if (_cy1 < y_min)
        {
            hasOverRange = true;
            overRange_y = (0f - rectTransform.pivot.y) * rect.height;
        }
        if (_cy1 > y_max)
        {
            hasOverRange = true;
            overRange_y = (1f - rectTransform.pivot.y) * rect.height;
        }

        for (float x = rect.xMin + Acc; x < rect.xMax; x += Acc)
        {
            Vector2 pos = new Vector2(x, CalcY((x - rect.xMin) / rect.width, y_min) * rect.height);
            float _cy = CurveY((x - rect.xMin) / rect.width);

            if (_cy >= y_min && _cy <= y_max)
            {
                if (hasOverRange == false)
                {
                    var quad = GenerateQuad(pos_first, pos);
                    vh.AddUIVertexQuad(quad);
                    pos_first = pos;
                }
                else
                {
                    var quad = GenerateQuad(new Vector2(pos.x - 0.001f, overRange_y), pos);
                    vh.AddUIVertexQuad(quad);
                    pos_first = pos;
                    hasOverRange = false;
                }
            }
            else
            {
                if (hasOverRange == false)
                {
                    if (_cy < y_min)
                    {
                        pos.y = (0f - rectTransform.pivot.y) * rect.height;
                        overRange_y = (0f - rectTransform.pivot.y) * rect.height;
                    }
                    if (_cy > y_max)
                    {
                        pos.y = (1f - rectTransform.pivot.y) * rect.height;
                        overRange_y = (1f - rectTransform.pivot.y) * rect.height;
                    }
                    var quad = GenerateQuad(pos_first, pos);
                    vh.AddUIVertexQuad(quad);
                    hasOverRange = true;
                }
                else
                {
                    if (_cy < y_min && overRange_y == (1f - rectTransform.pivot.y) * rect.height)//之前是上极限现在是下极限
                    {
                        pos.y = (0f - rectTransform.pivot.y) * rect.height;
                        var quad = GenerateQuad(pos, new Vector2(pos.x - 0.001f, (1f - rectTransform.pivot.y) * rect.height));
                        vh.AddUIVertexQuad(quad);
                        overRange_y = (0f - rectTransform.pivot.y) * rect.height;
                    }
                    else if (_cy > y_max && overRange_y == (0f - rectTransform.pivot.y) * rect.height)//之前是下极限现在是上极限
                    {
                        pos.y = (1f - rectTransform.pivot.y) * rect.height;
                        var quad = GenerateQuad(new Vector2(pos.x - 0.001f, (0f - rectTransform.pivot.y) * rect.height), pos);
                        vh.AddUIVertexQuad(quad);
                        overRange_y = (1f - rectTransform.pivot.y) * rect.height;
                    }
                }
            }
        }
        /*
        if(hasOverRange == false)
        {
            Vector2 pos_last = new Vector2(rect.xMin, CalcY(1f, y_min) * rect.height);
            float _cy = CurveY(1f);
            if (_cy < y_min) pos_last.y = 0f;
            if (_cy > y_max) pos_last.y = rect.height;
            var quad1 = GenerateQuad(pos_first, pos_last);
            vh.AddUIVertexQuad(quad1);
        }*/

    }


    /// <summary>
    /// 曲线上Y得真实高度
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private float CurveY(float x)
    {
        return 0.5f;
    }
    private float CalcY(float x, float ymin)
    {
        return ((0.5f- ymin) / y_division - rectTransform.pivot.y);
    }

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

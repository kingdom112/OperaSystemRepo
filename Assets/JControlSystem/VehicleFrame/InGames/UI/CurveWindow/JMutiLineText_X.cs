using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.EventSystems;
using System;
using UnityEngine;
using UnityEngine.UI;


public class JMutiLineText_X : Text
{
    public float drawOffset_X = 0;
    public float drawOffset_Y = 0;
    public List<float> xPosList = new List<float>();

    private FontData fontData = FontData.defaultFontData;



    protected JMutiLineText_X()
    {
        fontData = typeof(Text).GetField("m_FontData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this) as FontData;
    }

    readonly UIVertex[] m_TempVerts = new UIVertex[4];
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (font == null)
            return;
        m_DisableFontTextureRebuiltCallback = true;
        Vector2 extents = rectTransform.rect.size;
        //获取字体的生成规则设置
        var settings = GetGenerationSettings(extents);


        string[] strs = text.Split('\n');
        int lineCount = strs.Length;

        //根据待填充字体、生成规则，生成顶点信息
        cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);
        //取到cachedTextGenerator.verts的顶点信息
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        int vertCount = verts.Count;
        if (vertCount <= 0)
        {
            toFill.Clear();
            return;
        }
        Vector2 roundingOffset = new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel;
        roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
        toFill.Clear();
        if (roundingOffset != Vector2.zero)
        {
            //Debug.Log("xxxxx");
            for (int i = 0; i < vertCount; ++i)
            { 
                //get char number
                int charNum = i / 4;
                //get line number
                int lineNum = -1;
                int charAll = 0;
                for (int j = 0; j < strs.Length; j++)
                {
                    charAll += strs[j].Length;
                    if (charNum < charAll)
                    {
                        lineNum = j;
                        break;
                    }
                }
                if (lineNum > -1 && lineNum < xPosList.Count)
                {
                    int tempVertsIndex = i & 3;
                    //填充顶点信息
                    m_TempVerts[tempVertsIndex] = verts[i];
                    //设置字体偏移
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    m_TempVerts[tempVertsIndex].position.x += xPosList[lineNum] + drawOffset_X;
                    m_TempVerts[tempVertsIndex].position.y += (fontSize + lineSpacing * 2) * lineNum;
                    m_TempVerts[tempVertsIndex].position.y -= drawOffset_Y;

                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);//填充UI顶点面片
                }
 
            }
        }
        else
        {
            for (int i = 0; i < vertCount; ++i)
            {
                //get char number
                int charNum = i / 4; 
                //get line number
                int lineNum = -1;
                int charAll = 0;
                for (int j = 0; j < strs.Length; j++)
                {
                    charAll += strs[j].Length;
                    if (charNum < charAll)
                    {
                        lineNum = j;
                        break;
                    }
                }
                if (lineNum > -1 && lineNum < xPosList.Count)
                {
                    int tempVertsIndex = i & 3;
                    //填充顶点信息
                    m_TempVerts[tempVertsIndex] = verts[i];
                    //设置字体偏移
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += xPosList[lineNum] + drawOffset_X;
                    m_TempVerts[tempVertsIndex].position.y += (fontSize + lineSpacing * 2) * lineNum;
                    m_TempVerts[tempVertsIndex].position.y -= drawOffset_Y;

                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);//填充UI顶点面片
                }

            }
        }
        m_DisableFontTextureRebuiltCallback = false;
    }
}

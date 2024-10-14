using UnityEngine;
using System.Collections.Generic;
using static RuntimeCurveEditor.PostRenderBehaviour;

namespace RuntimeCurveEditor
{
    public struct Knot
    {
        public Vector2 point;
        public bool visible;

        public Knot(Vector2 point, bool visible) {
            this.point = point;
            this.visible = visible;
        }
    }

    public static class Curves
    {
        public static Material lineMaterial;

        public static Dictionary<AnimationCurve, List<ContextMenu>> dictCurvesContextMenus;

        public static List<Knot> activeCurveKnots = new List<Knot>();

        public static float margin;

        static Color LIGHT_GRAY = new Color(0.85f, 0.85f, 0.85f);


        public static Vector2 SampleBezier(float t, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4) {
            return (1 - t) * (1 - t) * (1 - t) * p1 + 3.0F * (1 - t) * (1 - t) * t * p2 + 3.0F * (1 - t) * t * t * p3 + t * t * t * p4;
        }

        public static void DrawBezier(AnimationCurve curve, Color color, Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Rect clipRect, float clip = 1.0f) {
            Vector2 v1m = Vector2.zero;
            Vector2 v2m = Vector2.zero;
            color.a = 1.0f;
            //const float MARGIN = CurveLines.MARGIN_PIXELS * 0.4f;
            int samples = (int)(0.5f * (p4.x - p1.x));
            float invSamples = 1f / samples;
            float t = 0;
            v1m = SampleBezier(t, p1, p2, p3, p4);
            bool on = true;

            GL.Begin(GL.LINES);
            lineMaterial.color = color;
            lineMaterial.SetPass(0);
            do {
                t += invSamples;
                if (t > clip) {
                    on = false;
                    t = clip;
                }
                v2m = SampleBezier(t, p1, p2, p3, p4);
                if (clipRect.Contains(v1m) && clipRect.Contains(v2m)) {
                    GL_Vertex3(v1m.x, v1m.y, 0);
                    GL_Vertex3(v2m.x, v2m.y, 0);
                }
                v1m = v2m;
            } while (on);
            GL.End();
        }


        static void DrawConstant(Color color, Vector2 p1, Vector2 p2, Rect clipRect) {
            Vector2 p = p1;
            p.x = p2.x;
            Vector2 pp = p;
            if (Utils.CohenSutherlandLineClip(clipRect, ref p1, ref p)) {
                DrawConstant(color, p1, p, true);
            }
            if (Utils.CohenSutherlandLineClip(clipRect, ref p2, ref pp)) {
                DrawConstant(color, p2, pp, true);
            }
        }

        static void DrawConstant(Color color, Vector2 p1, Vector2 p2, bool oneLine = false) {
            color.a = 1.0f;
            GL.Begin(GL.LINES);
            lineMaterial.color = color;
            lineMaterial.SetPass(0);
            if (oneLine || (p1.y == p2.y)) {
                GL_Vertex3(p1.x, p1.y, 0);
                GL_Vertex3(p2.x, p2.y, 0);
            } else {
                GL_Vertex3(p1.x, p1.y, 0);
                GL_Vertex3(p2.x, p1.y, 0);
                GL_Vertex3(p2.x, p1.y, 0);
                GL_Vertex3(p2.x, p2.y, 0);
            }
            GL.End();
        }

        static Vector2 GetTangLength(Vector2 p1, Vector2 p2) {
            Vector2 tangLength = Vector2.zero;
            tangLength.x = Mathf.Abs(p1.x - p2.x) * 0.333333f;
            tangLength.y = tangLength.x;
            return tangLength;
        }

        public static void GetControlPoints(Vector2 p1, Vector2 p2, float tangOut, float tangIn, out Vector2 c1, out Vector2 c2) {
            Vector2 tangLength = GetTangLength(p1, p2);
            c1 = p1;
            c2 = p2;
            c1.x += tangLength.x;
            c1.y += tangLength.y * tangOut;
            c2.x -= tangLength.x;
            c2.y -= tangLength.y * tangIn;
        }

        public static void GetTangents(Vector2 p1, Vector2 p2, Vector2 c1, Vector2 c2, out float tangOut, out float tangIn) {
            Vector2 tangLength = GetTangLength(p1, p2);
            tangOut = (c1.y - p1.y) / tangLength.y;
            tangIn = (c2.y - p2.y) / tangLength.y;
        }

        static void DrawLine(Color color, float x1, float y1, float x2, float y2) {
            GL.Begin(GL.LINES);
            lineMaterial.color = color;
            lineMaterial.SetPass(0);
            GL_Vertex3(x1, y1, 0);
            GL_Vertex3(x2, y2, 0);
            GL.End();
        }

        static void DrawCurve(Color color, AnimationCurve curve, bool activeCurve, int selectedKey, Rect entireGridRect, Rect gridClipRect, Rect gradRect, bool isIcon = false, float clip = 1.0f, bool _drawPoints = true) {

            float ratio = entireGridRect.height * gradRect.width / (entireGridRect.width * gradRect.height);

            if (activeCurve) {
                activeCurveKnots.Clear();
            }

            for (int i = 0; i < curve.length; ++i) {
                Vector2 knot = new Vector2(curve[i].time, curve[i].value);
                knot = Utils.Convert(knot, entireGridRect, gradRect);
                bool knotIn = !isIcon && gridClipRect.Contains(knot);

                //outside of the interval, just draw straigt lines outside from the 1st and last key respectively
                if (i == 0) {
                    if (knotIn) {
                        DrawLine(color, gridClipRect.xMin, knot.y, knot.x, knot.y);
                    } else if (!isIcon && (gridClipRect.yMin <= knot.y) && (knot.y <= gridClipRect.yMax) && (gridClipRect.xMax <= knot.x)) {
                        DrawLine(color, gridClipRect.xMin, knot.y, gridClipRect.xMax, knot.y);
                    }
                }

                if (i == curve.length - 1) {
                    if (knotIn) {
                        DrawLine(color, knot.x, knot.y, gridClipRect.xMax, knot.y);
                    } else if (!isIcon && (gridClipRect.yMin <= knot.y) && (knot.y <= gridClipRect.yMax) && (knot.x <= gridClipRect.xMin)) {
                        DrawLine(color, gridClipRect.xMin, knot.y, gridClipRect.xMax, knot.y);
                    }
                }

                if (isIcon && (curve.length == 1)) {
                    DrawLine(color, gridClipRect.xMin, knot.y, gridClipRect.xMax, knot.y);
                }

                if (curve.length > i + 1) {//draw bezier between consecutive keys
                    Vector2 knot2 = new Vector2(curve[i + 1].time, curve[i + 1].value);
                    knot2 = Utils.Convert(knot2, entireGridRect, gradRect);
                    bool knot2In = gridClipRect.Contains(knot2);

                    Vector2 c1 = Vector2.zero;
                    Vector2 c2 = Vector2.zero;
                    float tangOut = curve[i].outTangent;
                    float tangIn = curve[i + 1].inTangent;

                    //TODO it would be nice to have all these tang scaled values calculated only when needed(when ratio or tangs of the point changes)
                    float tangOutScaled = Mathf.Atan(tangOut * ratio);
                    float tangInScaled = Mathf.Atan(tangIn * ratio);

                    if ((tangOut != float.PositiveInfinity) && (tangIn != float.PositiveInfinity)) {
                        GetControlPoints(knot, knot2, tangOut * ratio, tangIn * ratio, out c1, out c2);
                        DrawBezier(curve, color, knot, c1, c2, knot2, gridClipRect, clip);
                    } else {
                        if (knotIn && knot2In) {
                            DrawConstant(color, knot, knot2);
                        } else {
                            DrawConstant(color, knot, knot2, gridClipRect);
                        }
                    }

                    if (activeCurve) {

                        if (knotIn && (selectedKey == i)) {
                            ContextMenu contextMenu = dictCurvesContextMenus[curve][selectedKey];
                            if (!contextMenu.broken || contextMenu.rightTangent.free) {
                                Vector2 tangPeak = new Vector2(knot.x + CurveLines.tangFloat * Mathf.Cos(tangOutScaled), knot.y + CurveLines.tangFloat * Mathf.Sin(tangOutScaled));

                                GL.Begin(GL.LINES);
                                lineMaterial.color = Color.gray;
                                lineMaterial.SetPass(0);
                                GL_Vertex3(knot.x, knot.y, 0);
                                GL_Vertex3(tangPeak.x, tangPeak.y, 0);
                                GL.End();

                                if (_drawPoints)
                                    DrawQuad(Color.gray, tangPeak, margin);
                            }
                        }

                        if (knot2In && (selectedKey == i + 1)) {
                            ContextMenu contextMenu = dictCurvesContextMenus[curve][selectedKey];
                            if (!contextMenu.broken || contextMenu.leftTangent.free) {
                                Vector2 tangPeak = new Vector2(knot2.x - CurveLines.tangFloat * Mathf.Cos(tangInScaled), knot2.y - CurveLines.tangFloat * Mathf.Sin(tangInScaled));

                                GL.Begin(GL.LINES);
                                lineMaterial.color = Color.gray;
                                lineMaterial.SetPass(0);
                                GL_Vertex3(knot2.x, knot2.y, 0);
                                GL_Vertex3(tangPeak.x, tangPeak.y, 0);
                                GL.End();

                                if(_drawPoints)
                                    DrawQuad(Color.gray, tangPeak, margin);
                            }
                        }
                    }
                }

                if (activeCurve) {
                    activeCurveKnots.Add(new Knot(knot, knotIn));
                }

                if (knotIn) {
                    if (activeCurve) {
                        if (selectedKey == i) {
                            if (_drawPoints)
                                DrawQuad(LIGHT_GRAY, knot, 1.33333f * margin);
                        }
                    }
                    if (!isIcon) {
                        if (_drawPoints)
                            DrawQuad(color, knot, margin);
                    }
                }
            }

        }

        public static void DrawCurveForm(Color color, AnimationCurve curve1, AnimationCurve curve2, bool activeCurve1, bool activeCurve2, int selectedKey, Rect entireGridRect, Rect gridRect, Rect gradRect, bool isIcon = false, float clip = 1.0f, bool _drawPoints = true) {
            if (curve2 != null) {
                int samples = (int)entireGridRect.width;
                Color colorTransp = color;
                colorTransp.a *= 0.35f;
                Vector2 v1 = Vector2.zero;
                Vector2 v2 = Vector2.zero;
                Vector2 v1prev;
                Vector2 v2prev;
                float invSamples = 1f / samples;
                float t = 0;
                bool lineIn = GetValues(out v1, out v2, curve1, curve2, entireGridRect, gridRect, gradRect, t);
                bool prevLineIn;
                GL.Begin(GL.QUADS);
                lineMaterial.color = colorTransp;
                lineMaterial.SetPass(0);
                for (int i = 1; i < samples; ++i) {
                    v1prev = v1;
                    v2prev = v2;
                    prevLineIn = lineIn;
                    lineIn = GetValues(out v1, out v2, curve1, curve2, entireGridRect, gridRect, gradRect, t);
                    if (prevLineIn && lineIn) {
                        GL_Vertex3(v1prev.x, v1prev.y, 0);
                        GL_Vertex3(v2prev.x, v2prev.y, 0);
                        GL_Vertex3(v2.x, v2.y, 0);
                        GL_Vertex3(v1.x, v1.y, 0);
                    }
                    t += invSamples;
                }
                GL.End();
                DrawCurve(color, curve2, activeCurve2, selectedKey, entireGridRect, gridRect, gradRect, isIcon, clip, _drawPoints);
            }
            DrawCurve(color, curve1, activeCurve1, selectedKey, entireGridRect, gridRect, gradRect, isIcon, clip, _drawPoints);
        }

        static bool GetValues(out Vector2 v1, out Vector2 v2, AnimationCurve curve1, AnimationCurve curve2, Rect entireGridRect, Rect clipRect, Rect gradRect, float t) {
            v1.x = gradRect.xMin + t * (gradRect.xMax - gradRect.xMin);
            v2.x = v1.x;
            v1.y = curve1.Evaluate(v1.x);
            v1 = Utils.Convert(v1, entireGridRect, gradRect);
            v2.y = curve2.Evaluate(v2.x);
            v2 = Utils.Convert(v2, entireGridRect, gradRect);
            return Utils.CohenSutherlandLineClip(clipRect, ref v1, ref v2);
        }

        static void DrawQuad(Color color, Vector2 pos, float m) {
            GL.Begin(GL.QUADS);
            lineMaterial.color = color;
            lineMaterial.SetPass(0);
            GL_Vertex3(pos.x, pos.y - m, 0);
            GL_Vertex3(pos.x + m, pos.y, 0);
            GL_Vertex3(pos.x, pos.y + m, 0);
            GL_Vertex3(pos.x - m, pos.y, 0);
            GL.End();
        }
    }
}

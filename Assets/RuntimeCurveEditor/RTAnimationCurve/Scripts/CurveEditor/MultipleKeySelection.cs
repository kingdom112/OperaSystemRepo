using System.Collections.Generic;
using UnityEngine;
using static RuntimeCurveEditor.PostRenderBehaviour;

namespace RuntimeCurveEditor
{
    public class MultipleKeySelection : MonoBehaviour, InterfacePostRenderer
    {
        public Material lineMaterial;
        public bool MultipleKeySelectionOn { get; private set; }//true, just while the user select an area 
                                                                //defines the selected area
        Vector2 startPoint;
        Vector2 endPoint;

        List<int> selectedKnots = new List<int>();
        //defines a rectangle that includes all selected knots
        Vector2 leftBottom;
        Vector2 rightTop;
        Vector2 leftPadBottom;
        Vector2 rightPadTop;

        static Color GRAY_TRANSP = new Color(0.63f, 0.63f, 0.63f, 0.35f);

        Vector2 PAD = new Vector2(10, 10);//e.g. useful, when the seleted knots are on the same line

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (MultipleKeySelectionOn) {
                endPoint = Input.mousePosition;

                SetSelectedKnots();

                if (Input.GetMouseButtonUp(0)) {
                    MultipleKeySelectionOn = false;
                }
            }
        }

        public void UpdateSelectedKnots(Rect gridRect) {
            bool first = true;
            foreach (int i in selectedKnots) {
                Knot knot = Curves.activeCurveKnots[i];
                SetRectLimits(knot, first);
                if (first) {
                    first = false;
                }
            }

            if (leftBottom.x < gridRect.xMin) {
                leftBottom.x = gridRect.xMin;
            }
            if (gridRect.xMax < rightTop.x) {
                rightTop.x = gridRect.xMax;
            }
            if (gridRect.yMax < rightTop.y) {
                rightTop.y = gridRect.yMax;
            }
            if (leftBottom.y < gridRect.yMin) {
                leftBottom.y = gridRect.yMin;
            }
        }

        void SetSelectedKnots() {
            selectedKnots.Clear();
            int i = 0;
            bool first = true;
            foreach (Knot knot in Curves.activeCurveKnots) {
                if (knot.visible && Contains(knot.point, startPoint, endPoint)) {
                    selectedKnots.Add(i);
                    SetRectLimits(knot, first);
                    if (first) {
                        first = false;
                    }
                }
                i += 1;
            }
        }

        void SetRectLimits(Knot knot, bool first) {
            if (first) {
                leftBottom = knot.point;
                rightTop = knot.point;
            } else {
                if (knot.point.x < leftBottom.x) {
                    leftBottom.x = knot.point.x;
                }
                if (rightTop.x < knot.point.x) {
                    rightTop.x = knot.point.x;
                }
                if (knot.point.y < leftBottom.y) {
                    leftBottom.y = knot.point.y;
                }
                if (rightTop.y < knot.point.y) {
                    rightTop.y = knot.point.y;
                }

                PadRect();
            }
        }

        public void OnPostRendererPipeline() {
            OnPostRender();
        }

        void OnPostRender() {
            if (MultipleKeySelectionOn) {
                GL.PushMatrix();
                GL.LoadPixelMatrix();

                GL.Begin(GL.QUADS);
                lineMaterial.color = GRAY_TRANSP;
                lineMaterial.SetPass(0);

                GL_Vertex3(startPoint.x, startPoint.y, 0);
                GL_Vertex3(startPoint.x, endPoint.y, 0);
                GL_Vertex3(endPoint.x, endPoint.y, 0);
                GL_Vertex3(endPoint.x, startPoint.y, 0);

                GL.End();

                GL.Begin(GL.LINE_STRIP);
                lineMaterial.color = Color.gray;
                lineMaterial.SetPass(0);

                GL_Vertex3(startPoint.x, startPoint.y, 0);
                GL_Vertex3(startPoint.x, endPoint.y, 0);
                GL_Vertex3(endPoint.x, endPoint.y, 0);
                GL_Vertex3(endPoint.x, startPoint.y, 0);
                GL_Vertex3(startPoint.x, startPoint.y, 0);

                GL.End();

                GL.PopMatrix();
            }

            if (selectedKnots.Count > 1) {
                GL.PushMatrix();
                GL.LoadPixelMatrix();

                GL.Begin(GL.QUADS);
                lineMaterial.color = GRAY_TRANSP;
                lineMaterial.SetPass(0);

                GL_Vertex3(leftPadBottom.x, leftPadBottom.y, 0);
                GL_Vertex3(leftPadBottom.x, rightPadTop.y, 0);
                GL_Vertex3(rightPadTop.x, rightPadTop.y, 0);
                GL_Vertex3(rightPadTop.x, leftPadBottom.y, 0);

                GL.End();
                GL.PopMatrix();
            }
        }

        public void StartMultipleKeySelection() {
            startPoint = Input.mousePosition;
            endPoint = startPoint;
            MultipleKeySelectionOn = true;
        }

        public bool MultipleKeysAreSelected() {
            return selectedKnots.Count > 1;
        }

        public List<int> SelectedKeyIndices() {
            return selectedKnots;
        }

        public void ClearMultipleKeySelection() {
            selectedKnots.Clear();
        }

        public bool InsideSelectedKeys() {
            Vector2 mousePos = Input.mousePosition;
            return Contains(mousePos, leftBottom, rightTop);
        }

        bool Contains(Vector2 point, Vector2 startPoint, Vector2 endPoint) {
            return (((startPoint.x < endPoint.x) && (startPoint.x <= point.x) && (point.x <= endPoint.x)) || ((startPoint.x > endPoint.x) && (startPoint.x >= point.x) && (point.x >= endPoint.x))) &&
                 (((startPoint.y < endPoint.y) && (startPoint.y <= point.y) && (point.y <= endPoint.y)) || ((startPoint.y > endPoint.y) && (startPoint.y >= point.y) && (point.y >= endPoint.y)));
        }

        void PadRect() {
            if (selectedKnots.Count > 1) {
                leftPadBottom = leftBottom - PAD;
                rightPadTop = rightTop + PAD;
            }
        }

    }
}

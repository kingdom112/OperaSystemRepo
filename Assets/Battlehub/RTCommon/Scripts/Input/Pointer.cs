using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.RTCommon
{
    public class Pointer : MonoBehaviour
    {
        //处理的是当在scene界面点击时响应事件
        public virtual Ray Ray//从摄像机放出射线 和target层碰撞返回一个bool值
        {
            get
            {
                Vector2 screenPoint = ScreenPoint;
                Rect pixelRect = m_window.Camera.pixelRect;
                if(!pixelRect.Contains(screenPoint))
                {
                    return new Ray(Vector3.up * float.MaxValue, Vector3.up);
                }
                return m_window.Camera.ScreenPointToRay(screenPoint);
            }
        }

        public virtual Vector2 ScreenPoint
        {
            get { return ScreenPointToViewPoint(m_window.Editor.Input.GetPointerXY(0)); }
        }

        private RenderTextureCamera m_renderTextureCamera;
        private CanvasScaler m_canvasScaler;
        private Canvas m_canvas;

        [SerializeField]
        protected RuntimeWindow m_window;
        protected virtual void Awake()
        {
            Init();
        }

        protected virtual void Start()
        {
            Init();
        }

        protected virtual void Init()
        {
            if (m_window == null)
            {
                m_window = GetComponent<RuntimeWindow>();
            }

            m_canvas = GetComponentInParent<Canvas>();//获取控件所在的父级canvas

            if (m_window.Camera != null)
            {
                m_renderTextureCamera = m_window.Camera.GetComponent<RenderTextureCamera>();
                if (m_renderTextureCamera != null)
                {
                    m_canvasScaler = GetComponentInParent<CanvasScaler>();//控制ui总体大小和密度的控件 影响canvas下元素的缩放比例
                }
            }
        }

        private Vector2 ScreenPointToViewPoint(Vector2 screenPoint)//应该是将视点从屏幕视点转为透视把
        {
            if (m_renderTextureCamera == null || m_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return screenPoint;
            }

            Vector2 viewPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_renderTextureCamera.RectTransform, screenPoint, m_renderTextureCamera.Canvas.worldCamera, out viewPoint);

            if (m_canvasScaler != null)
            {
                viewPoint *= m_canvasScaler.scaleFactor;
            }

            return viewPoint;
        }

        public virtual bool WorldToScreenPoint(Vector3 worldPoint, Vector3 point, out Vector2 result)
        {
            result = m_window.Camera.WorldToScreenPoint(point);
            result = ScreenPointToViewPoint(result);
            return true;
        }

        public virtual bool XY(Vector3 worldPoint, out Vector2 result)
        {
            result = ScreenPoint;
            return true;
        }

        public virtual bool ToWorldMatrix(Vector3 worldPoint, out Matrix4x4 matrix)
        {
            matrix = m_window.Camera.cameraToWorldMatrix;
            return true;
        }

        public static implicit operator Ray(Pointer pointer)
        {
            return pointer.Ray;
        }
    }
}
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace RuntimeCurveEditor
{
    public class PostRenderBehaviour : MonoBehaviour
    {
        readonly List<InterfacePostRenderer> postRenderers = new List<InterfacePostRenderer>();
        public Camera m_camera;
        public static Vector2 HALF_SCREEN;
        public static Vector2 SCALE_VECTOR;

        // Start is called before the first frame update
        void Start() {
            if ((GraphicsSettings.renderPipelineAsset != null) && (GraphicsSettings.renderPipelineAsset.GetType() == System.Type.GetType("UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset, Unity.RenderPipelines.Universal.Runtime"))) {
                SetupPostRendering();
            }
        }

        void SetupPostRendering() {
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
            //m_camera = GetComponent<Camera>();
            foreach (Component component in GetComponents<Component>()) {
                if (component is InterfacePostRenderer) {
                    postRenderers.Add(component as InterfacePostRenderer);
                }
            }
        }

        void OnEndCameraRendering(ScriptableRenderContext context, Camera camera) {
            if (m_camera != null && m_camera == camera) {
                foreach (InterfacePostRenderer postRenderer in postRenderers) {
                    postRenderer.OnPostRendererPipeline();
                }
            }
        }

        public static void GL_Vertex3(float x, float y, float z) {
            if (RenderPipelineManager.currentPipeline == null) {
                GL.Vertex3(x, y, z);
            } else {
                GL.Vertex3(x * SCALE_VECTOR.x - HALF_SCREEN.x, y * SCALE_VECTOR.y - HALF_SCREEN.y, z);
            }
        }
    }
}

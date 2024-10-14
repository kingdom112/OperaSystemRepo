using Battlehub.RTCommon;
using UnityEngine;
using Battlehub.RTEditor;
//还是没太明白什么用处 但是大概能知道是显示面板的问题
public class OperaToolsPanelOverride : EditorOverride
{
    [SerializeField]
    private Transform m_toolsPrefab;

    protected override void OnEditorCreated(object obj)
    {
        OverrideTools();
    }

    protected override void OnEditorExist()
    {
        OverrideTools();

        IRuntimeEditor editor = IOC.Resolve<IRuntimeEditor>();
        if (editor.IsOpened)
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();
            if (m_toolsPrefab != null)
            {
                wm.SetTools(Instantiate(m_toolsPrefab));
            }
        }
    }

    private void OverrideTools()
    {
        IWindowManager wm = IOC.Resolve<IWindowManager>();
        if (m_toolsPrefab != null)
        {
            wm.OverrideTools(m_toolsPrefab);
        }
    }
}

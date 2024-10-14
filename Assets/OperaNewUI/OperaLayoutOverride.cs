using Battlehub.RTCommon;
using Battlehub.UIControls.DockPanels;
using Battlehub.RTEditor;
using UnityEngine;

public class OperaLayoutOverride : EditorOverride
{
    protected override void OnEditorCreated(object obj)
    {
        OverrideDefaultLayout();
    }

    protected override void OnEditorExist()
    {
        OverrideDefaultLayout();

        IRuntimeEditor editor = IOC.Resolve<IRuntimeEditor>();
        if (editor.IsOpened)
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();
            wm.SetLayout(DefaultLayout, RuntimeWindowType.Scene.ToString());//将当前的窗口关闭吗？
        }
    }

    private void OverrideDefaultLayout()
    {
        IWindowManager wm = IOC.Resolve<IWindowManager>();
        wm.OverrideDefaultLayout(DefaultLayout, RuntimeWindowType.Scene.ToString());
    }

    static LayoutInfo DefaultLayout(IWindowManager wm)
    {
        bool isDialog;

        WindowDescriptor sceneWd;
        GameObject sceneContent;
        wm.CreateWindow(RuntimeWindowType.Scene.ToString(), out sceneWd, out sceneContent, out isDialog);//函数参数属性不清楚

        WindowDescriptor gameWd;
        GameObject gameContent;
        wm.CreateWindow(RuntimeWindowType.Game.ToString(), out gameWd, out gameContent, out isDialog);

        WindowDescriptor inspectorWd;
        GameObject inspectorContent;
        wm.CreateWindow(RuntimeWindowType.Inspector.ToString(), out inspectorWd, out inspectorContent, out isDialog);

        WindowDescriptor hierarchyWd;
        GameObject hierarchyContent;
        wm.CreateWindow(RuntimeWindowType.Hierarchy.ToString(), out hierarchyWd, out hierarchyContent, out isDialog);

        WindowDescriptor motionSelectWd;
        GameObject motionSelectContent;
        wm.CreateWindow("MotionSelectWindow", out motionSelectWd, out motionSelectContent, out isDialog);

        WindowDescriptor timelineWd;
        GameObject timelineContent;
        wm.CreateWindow("TimeLineWindow", out timelineWd, out timelineContent, out isDialog);

        WindowDescriptor stackWd;
        GameObject stackContent;
        wm.CreateWindow("StackWindow", out stackWd, out stackContent, out isDialog);

        LayoutInfo layout = new LayoutInfo(true,
              new LayoutInfo(false,
           new LayoutInfo(
               new LayoutInfo(motionSelectContent.transform, motionSelectWd.Header, null, true, false)),
           new LayoutInfo(false,
               new LayoutInfo(sceneContent.transform, sceneWd.Header, sceneWd.Icon, true, false),
               new LayoutInfo(stackContent.transform, stackWd.Header, null, true, false),
               0.75f),
           0.2f),
              new LayoutInfo(timelineContent.transform, timelineWd.Header, null, true, false),
              0.85f);




        return layout;
    }
}

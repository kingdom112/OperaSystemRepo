using Battlehub.RTCommon;
using Battlehub.RTEditor;
using Battlehub.UIControls.MenuControl;
using UnityEngine;
//创建窗口？每个窗口规定好格式与形式？
[MenuDefinition]
public class RegisterOperaWindows : EditorOverride
{
    protected override void OnEditorExist()
    {
        base.OnEditorExist();
        RegisterWindows();
    }

    private void RegisterWindows()
    {
        IWindowManager wm = IOC.Resolve<IWindowManager>();
        wm.RegisterWindow(new CustomWindowDescriptor
        {
            IsDialog = false,
            TypeName = "MotionSelectWindow",
            Descriptor = new WindowDescriptor
            {
                Header = "Motion",
                MaxWindows = 1,
                Icon = Resources.Load<Sprite>("IconNew"),//什么用处
                ContentPrefab = Resources.Load<GameObject>("MotionSelectWindow")
            }
        });

        wm.RegisterWindow(new CustomWindowDescriptor
        {
            IsDialog = false,
            TypeName = "TimeLineWindow",
            Descriptor = new WindowDescriptor
            {
                Header = "TimeLine",
                MaxWindows = 1,
                Icon = Resources.Load<Sprite>("IconNew"),
                ContentPrefab = Resources.Load<GameObject>("TimeLineWindow")
            }
        });

        wm.RegisterWindow(new CustomWindowDescriptor
        {
            IsDialog = false,
            TypeName = "StackWindow",
            Descriptor = new WindowDescriptor
            {
                Header = "Stack",
                MaxWindows = 1,
                Icon = Resources.Load<Sprite>("IconNew"),
                ContentPrefab = Resources.Load<GameObject>("StackWindow")
            }
        });

        wm.RegisterWindow(new CustomWindowDescriptor
        {
            IsDialog = false,
            TypeName = "CurveWindow",
            Descriptor = new WindowDescriptor
            {
                Header = "Curve",
                MaxWindows = 1,
                Icon = Resources.Load<Sprite>("RTE_Views_GameView"),
                ContentPrefab = Resources.Load<GameObject>("CurveWindow")
            }
        });
        wm.RegisterWindow(new CustomWindowDescriptor
        {
            IsDialog = false,
            TypeName = "FBXImportWindow",
            Descriptor = new WindowDescriptor
            {
                Header = "FBXImport",
                MaxWindows = 1,
                Icon = Resources.Load<Sprite>("IconNew"),
                ContentPrefab = Resources.Load<GameObject>("FBXImportWindow")
            }
        });

    }
}

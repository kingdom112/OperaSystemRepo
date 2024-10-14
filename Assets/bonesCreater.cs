using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JAnimationSystem;
using Battlehub.RTHandles;
using Battlehub.RTSL.Interface;
using Battlehub.RTSL;
using Battlehub.RTCommon;
using Battlehub.UIControls;
using TMPro;
using Battlehub.RTEditor;
using System.Reflection;
using Battlehub.Utils;

public class bonesCreater : MonoBehaviour
{

    public GameObject ga;
    public JAnimation_AutoSetBoneMatchData autoData;
    public JAnimationData jad;


    private List<RuntimeAnimation> ras = new List<RuntimeAnimation>();

    // Start is called before the first frame update
    void Start()
    {
        create();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void create()
    {
        BoneMatch match1 = new BoneMatch();
        match1.StartAutoMatch(ga.transform, autoData, jad.jAD);
        BoneMatch_Player matchPlayer1 = new BoneMatch_Player(match1);
        matchPlayer1.SetJAnimationData(jad);

        ras.Clear();

        for(int i=0; i< matchPlayer1.bonePlayers.Count; i++)
        {
            Transform t1 = matchPlayer1.bonePlayers[i].boneOneMatch.matchedT;
            if(t1 == null)
            {
                continue;
            }
            ExposeToEditor exposeToEditor = t1.gameObject.GetComponent<ExposeToEditor>();
            if(exposeToEditor == null)
            {
                exposeToEditor = t1.gameObject.AddComponent<ExposeToEditor>();
            }


            RuntimeAnimationClip clip = new RuntimeAnimationClip();
            clip.name = "clip_" + matchPlayer1.bonePlayers[i].boneOneMatch.boneType.ToString();
            RuntimeAnimationProperty roaProperty1 = GetRotationProperty(t1
                , matchPlayer1.bonePlayers[i].Curve_X_4_Roa
                , matchPlayer1.bonePlayers[i].Curve_Y_4_Roa
                , matchPlayer1.bonePlayers[i].Curve_Z_4_Roa
                , matchPlayer1.bonePlayers[i].Curve_W_4_Roa);
            clip.Add(roaProperty1);
            if (matchPlayer1.bonePlayers[i].boneOneMatch.boneType == JAnimationData.BoneType.hips)
            {
                if (matchPlayer1.bonePlayers[i].Curve_X_Pos != null 
                    && matchPlayer1.bonePlayers[i].Curve_Y_Pos != null
                    && matchPlayer1.bonePlayers[i].Curve_Z_Pos != null)
                {
                    RuntimeAnimationProperty posProperty1 = GetPositionProperty(t1
                        , matchPlayer1.bonePlayers[i].Curve_X_Pos
                        , matchPlayer1.bonePlayers[i].Curve_Y_Pos
                        , matchPlayer1.bonePlayers[i].Curve_Z_Pos);
                    clip.Add(posProperty1);
                }
                else
                {
                    AnimationCurve curve1 = new AnimationCurve();
                    curve1.AddKey(0f, 0f);
                    curve1.AddKey(1f, 0f);
                    RuntimeAnimationProperty posProperty1 = GetPositionProperty(t1
                      , curve1
                      , curve1
                      , curve1);
                    clip.Add(posProperty1);
                }
            }

            RuntimeAnimation ra1 = t1.gameObject.GetComponent<RuntimeAnimation>();
            if(ra1 == null)
            {
                ra1 = t1.gameObject.AddComponent<RuntimeAnimation>();
            }
            ras.Add(ra1);
            RuntimeAnimationListener raListenr1 = t1.gameObject.GetComponent<RuntimeAnimationListener>();
            if (raListenr1 == null)
            {
                raListenr1 = t1.gameObject.AddComponent<RuntimeAnimationListener>();
            }
            raListenr1.control = this;

            ra1.Clips = new List<RuntimeAnimationClip> { clip };
            ra1.ClipIndex = 0;
            ra1.Refresh();
             

        }
    }

 
    public void OnRuntimeAnimationNormalizedTimeChanged(RuntimeAnimation caller)
    {
        if (caller == null) return;
        float time1 = caller.NormalizedTime;
        for (int i=0; i<ras.Count; i++)
        {
            if(ras[i] != null && ras [i] != caller)
            {
                ras[i].MySetNormalizedTime(time1);
            }
        }
    }

    RuntimeAnimationProperty GetPositionProperty(Transform t1, AnimationCurve c_x, AnimationCurve c_y, AnimationCurve c_z)
    {
        RuntimeAnimationProperty component = new RuntimeAnimationProperty();
        component.ComponentDisplayName = "Transform";
        component.ComponentTypeName = "UnityEngine.Transform,UnityEngine.CoreModule";
        component.Children = new List<RuntimeAnimationProperty>();
        component.Component = t1;
        component.AnimationPropertyName = "m_LocalPosition";
        component.PropertyDisplayName = "Position";
        component.PropertyName = "localPosition";

        RuntimeAnimationProperty property = new RuntimeAnimationProperty();
        property.Parent = component;
        property.ComponentTypeName = component.ComponentTypeName;
        property.ComponentDisplayName = "x";
        property.PropertyName = "x";
        property.PropertyDisplayName = "x";
        property.AnimationPropertyName = "x";
        property.Component = component.Component;
        property.Curve = c_x;
        component.Children.Add(property);

        property = new RuntimeAnimationProperty();
        property.Parent = component;
        property.ComponentTypeName = component.ComponentTypeName;
        property.ComponentDisplayName = "y";
        property.PropertyName = "y";
        property.PropertyDisplayName = "y";
        property.AnimationPropertyName = "y";
        property.Component = component.Component;
        property.Curve = c_y;
        component.Children.Add(property);

        property = new RuntimeAnimationProperty();
        property.Parent = component;
        property.ComponentTypeName = component.ComponentTypeName;
        property.ComponentDisplayName = "z";
        property.PropertyName = "z";
        property.PropertyDisplayName = "z";
        property.AnimationPropertyName = "z";
        property.Component = component.Component;
        property.Curve = c_z;
        component.Children.Add(property); 

        return component;
    }

    RuntimeAnimationProperty GetRotationProperty(Transform t1, AnimationCurve c_x, AnimationCurve c_y, AnimationCurve c_z, AnimationCurve c_w)
    {
        RuntimeAnimationProperty component = new RuntimeAnimationProperty();
        component.ComponentDisplayName = "Transform";
        component.ComponentTypeName = "UnityEngine.Transform,UnityEngine.CoreModule";
        component.Children = new List<RuntimeAnimationProperty>();
        component.Component = t1;
        component.AnimationPropertyName = "m_LocalRotation";
        component.PropertyDisplayName = "Rotation";
        component.PropertyName = "localRotation";

        RuntimeAnimationProperty property = new RuntimeAnimationProperty();
        property.Parent = component;
        property.ComponentTypeName = component.ComponentTypeName;
        property.ComponentDisplayName = "x";
        property.PropertyName = "x";
        property.PropertyDisplayName = "x"; 
        property.AnimationPropertyName = "x";
        property.Component = component.Component;
        property.Curve = c_x;
        component.Children.Add(property);

        property = new RuntimeAnimationProperty();
        property.Parent = component;
        property.ComponentTypeName = component.ComponentTypeName;
        property.ComponentDisplayName = "y";
        property.PropertyName = "y";
        property.PropertyDisplayName = "y";
        property.AnimationPropertyName = "y";
        property.Component = component.Component;
        property.Curve = c_y;
        component.Children.Add(property);

        property = new RuntimeAnimationProperty();
        property.Parent = component;
        property.ComponentTypeName = component.ComponentTypeName;
        property.ComponentDisplayName = "z";
        property.PropertyName = "z";
        property.PropertyDisplayName = "z";
        property.AnimationPropertyName = "z";
        property.Component = component.Component;
        property.Curve = c_z;
        component.Children.Add(property);

        property = new RuntimeAnimationProperty();
        property.Parent = component;
        property.ComponentTypeName = component.ComponentTypeName;
        property.ComponentDisplayName = "w";
        property.PropertyName = "w";
        property.PropertyDisplayName = "w";
        property.AnimationPropertyName = "w";
        property.Component = component.Component;
        property.Curve = c_w;
        component.Children.Add(property);

        return component;
    }

    PropertyDescriptor GetRotationDescriptor(Transform t1)
    {
        TransformPropertyConverter converterObj = new TransformPropertyConverter();
        converterObj.Component = t1;

        TransformPropertyConverter converter = (TransformPropertyConverter)converterObj;
        MemberInfo rotation = Strong.PropertyInfo((Transform x) => x.localRotation, "localRotation");
        MemberInfo rotationConverted = Strong.PropertyInfo((TransformPropertyConverter x) => x.localEuler, "localEuler");
        return new PropertyDescriptor("Rotation", converter, rotationConverted, rotation);
    }

}

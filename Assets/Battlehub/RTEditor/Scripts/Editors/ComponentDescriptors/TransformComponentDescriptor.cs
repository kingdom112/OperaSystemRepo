﻿using UnityEngine;
using System.Reflection;
using Battlehub.Utils;
using Battlehub.RTCommon;

namespace Battlehub.RTEditor
{
    public class TransformComponentDescriptor : ComponentDescriptorBase<Transform>
    {
        public override object CreateConverter(ComponentEditor editor)
        {
            TransformPropertyConverter converter = new TransformPropertyConverter();
            converter.Component = (Transform)editor.Component;
            return converter;
        }

        public override PropertyDescriptor[] GetProperties(ComponentEditor editor, object converterObj)
        {
            ILocalization lc = IOC.Resolve<ILocalization>();

            TransformPropertyConverter converter = (TransformPropertyConverter)converterObj;

            MemberInfo position = Strong.PropertyInfo((Transform x) => x.localPosition, "localPosition");
            MemberInfo rotation = Strong.PropertyInfo((Transform x) => x.localRotation, "localRotation");
            MemberInfo rotationConverted = Strong.PropertyInfo((TransformPropertyConverter x) => x.localEuler, "localEuler");
            MemberInfo scale = Strong.PropertyInfo((Transform x) => x.localScale, "localScale");

            return new[]
                {
                    new PropertyDescriptor( lc.GetString("ID_RTEditor_CD_Transform_Position", "Position"), editor.Component, position, position) ,
                    new PropertyDescriptor( lc.GetString("ID_RTEditor_CD_Transform_Rotation", "Rotation"), converter, rotationConverted, rotation),
                    new PropertyDescriptor( lc.GetString("ID_RTEditor_CD_Transform_Scale", "Scale"), editor.Component, scale, scale)
                };
        }
    }

    public class TransformPropertyConverter 
    {
        public Vector3 localEuler
        {
            get
            {
                if(Component == null)
                {
                    return Vector3.zero;
                }
                return Component.localRotation.eulerAngles;
            }
            set
            {
                if (Component == null)
                {
                    return;
                }
                Component.localRotation = Quaternion.Euler(value);
            }
        }
        public Quaternion localRotation
        {
            get
            {
                if (Component == null)
                {
                    return Quaternion.Euler(Vector3.zero);
                }
                return Component.localRotation;
            }
            set
            {
                if (Component == null)
                {
                    return;
                }
                Component.localRotation = value;
            }
        }

        public Transform Component
        {
            get;
            set;
        }
    }
}


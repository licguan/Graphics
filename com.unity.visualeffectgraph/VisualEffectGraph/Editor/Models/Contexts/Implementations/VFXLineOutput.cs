using System.Collections.Generic;
using System.Linq;
using UnityEditor.VFX.Block;
using UnityEngine;
using UnityEngine.Experimental.VFX;

namespace UnityEditor.VFX
{
    [VFXInfo]
    class VFXLineOutput : VFXAbstractParticleOutput
    {
        public override string name { get { return "Line Output"; } }
        public override string codeGeneratorTemplate { get { return RenderPipeTemplate("VFXParticleLines"); } }
        public override VFXTaskType taskType { get { return VFXTaskType.kParticleLineOutput; } }

        [VFXSetting, SerializeField]
        protected bool targetFromAttributes = true;

        protected override IEnumerable<string> filteredOutSettings
        {
            get
            {
                foreach (var setting in base.filteredOutSettings)
                    yield return setting;

                yield return "cullMode";
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            cullMode = CullMode.Off;
        }

        public override IEnumerable<VFXAttributeInfo> attributes
        {
            get
            {
                yield return new VFXAttributeInfo(VFXAttribute.Color,           VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.Alpha,           VFXAttributeMode.Read);
                yield return new VFXAttributeInfo(VFXAttribute.Alive,           VFXAttributeMode.Read);

                if (targetFromAttributes)
                {
                    yield return new VFXAttributeInfo(VFXAttribute.Pivot, VFXAttributeMode.Read);
                    yield return new VFXAttributeInfo(VFXAttribute.AngleX, VFXAttributeMode.Read);
                    yield return new VFXAttributeInfo(VFXAttribute.AngleY, VFXAttributeMode.Read);
                    yield return new VFXAttributeInfo(VFXAttribute.AngleZ, VFXAttributeMode.Read);
                    yield return new VFXAttributeInfo(VFXAttribute.AxisX, VFXAttributeMode.Read);
                    yield return new VFXAttributeInfo(VFXAttribute.AxisY, VFXAttributeMode.Read);
                    yield return new VFXAttributeInfo(VFXAttribute.AxisZ, VFXAttributeMode.Read);

                    foreach (var size in VFXBlockUtility.GetReadableSizeAttributes(GetData()))
                        yield return size;

                    yield return new VFXAttributeInfo(VFXAttribute.Position, VFXAttributeMode.ReadWrite);
                    yield return new VFXAttributeInfo(VFXAttribute.TargetPosition, VFXAttributeMode.Write);
                }
                else
                {
                    yield return new VFXAttributeInfo(VFXAttribute.Position, VFXAttributeMode.Read);
                    yield return new VFXAttributeInfo(VFXAttribute.TargetPosition, VFXAttributeMode.Read);
                }
            }
        }

        public class TargetFromAttributesProperties
        {
            public Vector3 targetOffset = Vector3.up;
        }

        protected override IEnumerable<VFXNamedExpression> CollectGPUExpressions(IEnumerable<VFXNamedExpression> slotExpressions)
        {
            foreach (var exp in base.CollectGPUExpressions(slotExpressions))
                yield return exp;

            if (targetFromAttributes)
                yield return slotExpressions.First(o => o.name == "targetOffset");
        }

        protected override IEnumerable<VFXPropertyWithValue> inputProperties
        {
            get
            {
                var properties = base.inputProperties;
                if (targetFromAttributes)
                    properties = PropertiesFromType("TargetFromAttributesProperties").Concat(properties);

                return properties;
            }
        }

        public override IEnumerable<string> additionalDefines
        {
            get
            {
                foreach (var d in base.additionalDefines)
                    yield return d;

                if (targetFromAttributes)
                    yield return "TARGET_FROM_ATTRIBUTES";
            }
        }
    }
}

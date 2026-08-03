using System;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;

namespace ybaso.Blazor.Server.Editors
{
    [PropertyEditor(typeof(string), "NguTaDiagramPropertyEditor", false)]
    public class NguTaDiagramPropertyEditor : BlazorPropertyEditorBase
    {
        public NguTaDiagramPropertyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model)
        {
        }

        public override NguTaDiagramComponentModel ComponentModel
        {
            get => (NguTaDiagramComponentModel)base.ComponentModel;
        }

        protected override IComponentModel CreateComponentModel()
        {
            return new NguTaDiagramComponentModel();
        }

        protected override void ReadValueCore()
        {
            ComponentModel.Value = PropertyValue as string ?? "Đảm|Bàng|Tiểu|Vị|Đại";
        }

        protected override object GetControlValueCore()
        {
            return ComponentModel.Value;
        }
    }
}
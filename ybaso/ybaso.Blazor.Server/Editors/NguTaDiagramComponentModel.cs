using System;
using DevExpress.ExpressApp.Blazor.Components.Models;
using ybaso.Blazor.Server.Components;

namespace ybaso.Blazor.Server.Editors
{
    public class NguTaDiagramComponentModel : ComponentModelBase
    {
        public string Value { get; set; } = "Đảm|Bàng|Tiểu|Vị|Đại";

        public override Type ComponentType => typeof(NguTaDiagramComponent);
    }
}
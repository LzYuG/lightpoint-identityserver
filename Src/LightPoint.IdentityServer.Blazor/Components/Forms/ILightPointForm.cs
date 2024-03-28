using LightPoint.IdentityServer.Blazor.Components.Forms.Base;
using LightPoint.IdentityServer.Blazor.Components.Forms.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Forms
{
    public interface ILightPointForm<TModel>
        where TModel : class, new()
    {
        FormContent<TModel>? FormContentRef { get; set; }
        string? Title { get; set; }

        #region Params
        TModel? Model { get; set; }
        List<FormItemConfig>? FormItemConfigs { get; set; }
        int LabelColSpan { get; set; }
        int WrapperColSpan { get; set; }
        #endregion



        #region Slots
        RenderFragment? Operation { get; set; }
        RenderFragment<FormItemSlotModel<TModel>>? ChildContent { get; set; }
        #endregion

        #region Actions
        bool Validate();

        Task Submit();

        void Open(TModel? model = null, string title = "");
        #endregion
    }
}

using AntDesign;
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
    partial class LightPointInLineForm<TModel> : ILightPointForm<TModel>
        where TModel : class, new()
    {
        public FormContent<TModel>? FormContentRef { get; set; }
        public string? Title { get; set; }
        #region Params
        [Parameter]
        public TModel? Model { get; set; } = new TModel();
        [Parameter]
        public List<FormItemConfig>? FormItemConfigs { get; set; }
        [Parameter]
        public int LabelColSpan { get; set; } = 6;
        [Parameter]
        public int WrapperColSpan { get; set; } = 18;

        [Parameter]
        public AntLabelAlignType LabelAlign { get; set; }
        [Parameter]
        public string Layout { get; set; } = FormLayout.Horizontal;
        [Parameter]
        public bool ReadOnly { get; set; }

        #endregion

        #region Slots
        [Parameter]
        public RenderFragment? Operation { get; set; }
        [Parameter]
        public RenderFragment<FormItemSlotModel<TModel>>? ChildContent { get; set; } = null;
        #endregion

        #region Events
        [Parameter]
        public EventCallback<TModel> OnSubmit { get; set; }

        private async Task ModelChangedHandle(TModel model)
        {
            Model = model;
            await ModelChanged.InvokeAsync(model);
        }

        [Parameter]
        public EventCallback<TModel> ModelChanged { get; set; }

        #endregion

        #region Actions
        public bool Validate()
        {
            return FormContentRef!.FormRef!.Validate();
        }

        public async Task Submit()
        {
            if (Validate())
            {
                await OnSubmit.InvokeAsync(Model);
            }
            //FormContentRef.FormRef.Submit();
        }

        public void Open(TModel? model = null, string title = "")
        {
            if (model != null)
            {
                Model = model;
                if (title == "")
                {
                    title = "编辑";
                }
            }
            else
            {
                Model = new TModel();
                if (title == "")
                {
                    title = "新增";
                }
            }

            Title = title;
        }
        #endregion
    }
}

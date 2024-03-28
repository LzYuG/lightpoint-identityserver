using AntDesign;
using LightPoint.IdentityServer.Blazor.Components.Forms.Base;
using LightPoint.IdentityServer.Blazor.Components.Forms.Models;
using Microsoft.AspNetCore.Components;
using OneOf;

namespace LightPoint.IdentityServer.Blazor.Components.Forms
{
    partial class LightPointDialogForm<TModel> : ILightPointForm<TModel>
        where TModel : class, new()
    {

        private bool _isVisible = false;
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
        public OneOf<string, double> Width { get; set; } = "30%";
        [Parameter]
        public AntLabelAlignType LabelAlign { get; set; }
        [Parameter]
        public string Layout { get; set; } = FormLayout.Horizontal;
        /// <summary>
        /// Model离屏幕上面的距离
        /// </summary>
        [Parameter]
        public string? Top { get; set; } = "10vh";

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
        public EventCallback<DialogFormSubmitModel<TModel>> OnSubmit { get; set; }

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
                Action close = () => _isVisible = false;
                await OnSubmit.InvokeAsync(new DialogFormSubmitModel<TModel>()
                {
                    Model = Model,
                    Close = close
                });
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
            _isVisible = true;
        }
        #endregion
    }

    public class DialogFormSubmitModel<TModel>
        where TModel : class, new()
    {
        public Action? Close { get; set; }

        public TModel? Model { get; set; }
    }
}

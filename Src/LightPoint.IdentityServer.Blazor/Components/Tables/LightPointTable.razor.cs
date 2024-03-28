using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using System.Linq.Expressions;

namespace LightPoint.IdentityServer.Blazor.Components.Tables
{
    partial class LightPointTable<TModel> where TModel : class, new()
    {
        Func<PaginationTotalContext, string> showTotal = ctx => $"数据总数： {ctx.Total}";

        public Table<TModel>? Table { get; set; }

        #region Slots
        [Parameter]
        public RenderFragment<SlotModel<TModel>>? ChildContent { get; set; } = null;
        #endregion

        #region Params
        [Parameter]
        public TableParameters<TModel>? TableParameters { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        [Parameter]
        public string Height { get; set; } = "calc(100vh - 300px)";

        #endregion

        #region Actions
        private async Task OnSelectedRowsChanged(IEnumerable<TModel> selectedDatas)
        {
            TableParameters!.SelectedDatas = selectedDatas.ToList();

            await TableParametersChanged.InvokeAsync(TableParameters);
        }

        private async Task OnChangePageParams(PaginationEventArgs paginationEventArgs)
        {
            TableParameters!.Length = paginationEventArgs.PageSize;
            TableParameters!.Start = (paginationEventArgs.Page - 1) * TableParameters.Length;
            TableParameters!.PageIndex = paginationEventArgs.Page;

            await TableParametersChanged.InvokeAsync(TableParameters);
            await Search.InvokeAsync();
        }

        private async Task OnChange(QueryModel<TModel> queryModel)
        {
            var lastOrder = queryModel.SortModel.Where(x => x.Sort != null).OrderBy(x => x.Priority).Last();
            TableParameters!.OrderProp = lastOrder.FieldName;
            TableParameters!.IsDesc = lastOrder.Sort == "ascend" ? false : lastOrder.Sort == "descend" ? true : false;
            await TableParametersChanged.InvokeAsync(TableParameters);
            await Search.InvokeAsync();
        }
        #endregion


        #region EventCallback
        [Parameter]
        public EventCallback<TableParameters<TModel>> TableParametersChanged { get; set; }

        [Parameter]
        public EventCallback Search { get; set; }
        #endregion


        private static Expression<Func<TModel, object>> GetPropertyExpression(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(TModel), "x");
            MemberExpression? property = null;
            if (typeof(TModel).GetProperty(propertyName) != null)
            {
                property = Expression.Property(parameter, propertyName);
            }
            else
            {
                property = Expression.Property(parameter, "Id");
            }
            var conversion = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<TModel, object>>(conversion, parameter);
        }
    }
}

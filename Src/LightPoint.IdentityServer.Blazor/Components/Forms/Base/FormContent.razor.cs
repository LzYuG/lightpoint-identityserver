using AntDesign;
using LightPoint.IdentityServer.Blazor.Components.Forms.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Linq.Expressions;

namespace LightPoint.IdentityServer.Blazor.Components.Forms.Base
{
	partial class FormContent<TModel> where TModel : class, new()
	{
		// Editting Model
		[Parameter]
		public TModel Model { get; set; } = new TModel();
		[Parameter]
		public string Layout { get; set; } = FormLayout.Horizontal;
		[Parameter]
		public int LabelColSpan { get; set; } = 6;
		[Parameter]
		public int WrapperColSpan { get; set; } = 18;
		[Parameter]
		public AntLabelAlignType LabelAlign { get; set; }
		public Form<TModel>? FormRef { get; set; }

		#region Slots
		[Parameter]
		public RenderFragment<FormItemSlotModel<TModel>>? ChildContent { get; set; } = null;
		#endregion


		#region Params
		[Parameter]
		public List<FormItemConfig> FormItemConfigs { get; set; } = new List<FormItemConfig>();
		/// <summary>
		/// 在组件修改的时候进行校验
		/// </summary>
		[Parameter]
		public bool ValidateOnChange { get; set; } = true;
		[Parameter]
		public string Size { get; set; } = AntSizeLDSType.Default;
		#endregion


		#region Actions
		#endregion

		#region EventCallBacks
		[Parameter]
		public EventCallback<TModel> ModelChanged { get; set; }

		[Parameter]
		public EventCallback<EditContext> OnSubmit { get; set; }
		[Parameter]
		public EventCallback<EditContext> OnSubmitFailed { get; set; }

		private async Task OnDateRangeChanged(DateRangeChangedEventArgs<DateTime?[]> args, FormItemConfig formItemConfig)
		{
			var setPropertyFunc = CreateSetPropertyAction(formItemConfig.Prop);
			var setEndPropertyFunc = CreateSetPropertyAction(formItemConfig.EndProp!);
			formItemConfig.DateTimePickerHelper = args.Dates;
			setPropertyFunc!(Model, args.Dates[0]!);
			setEndPropertyFunc!(Model, args.Dates[1]!);
			await ModelChanged.InvokeAsync(Model);
		}
		#endregion


		private EventCallback<T> GetEventCallBack<T>(FormItemConfig formItem)
		{
			// 根据Type动态构建EventCallBack
			Action<T> action = async (value) =>
			{
				// Model.GetType().GetProperty(prop)?.SetValue(Model, value);
				var setAction = CreateSetPropertyAction(formItem.Prop);
				setAction!(Model, value!);
				await ModelChanged.InvokeAsync(Model);
				if (formItem.OnValueChangeed != null)
				{
					formItem.OnValueChangeed.Invoke(Model);
				}
			};
			var factory = new EventCallbackFactory();
			var result = factory.Create(Model, action);
			return result;
		}

		private Expression<Func<T>> GetPropertyExpression<T>(string propertyName)
		{
			var property = Expression.Property(Expression.Constant(Model), propertyName);
			return Expression.Lambda<Func<T>>(property);
		}

		private Action<TModel, object>? CreateSetPropertyAction(string propertyName)
		{
			var modelType = typeof(TModel);
			var propertyInfo = modelType.GetProperty(propertyName);

			if (propertyInfo == null)
				return null;

			var modelParameter = Expression.Parameter(modelType);
			var valueParameter = Expression.Parameter(typeof(object));

			var propertyExpression = Expression.Property(modelParameter, propertyInfo);
			var assignExpression = Expression.Assign(propertyExpression, Expression.Convert(valueParameter, propertyInfo.PropertyType));

			return Expression.Lambda<Action<TModel, object>>(assignExpression, modelParameter, valueParameter).Compile();
		}

		private Func<TModel, object>? CreateGetPropertyFunc(string propertyName)
		{
			var modelType = typeof(TModel);
			var propertyInfo = modelType.GetProperty(propertyName);

			if (propertyInfo == null)
				return null;

			var modelParameter = Expression.Parameter(modelType);
			var propertyExpression = Expression.Property(modelParameter, propertyInfo);
			var convertExpression = Expression.Convert(propertyExpression, typeof(object));

			return Expression.Lambda<Func<TModel, object>>(convertExpression, modelParameter).Compile();
		}
	}
}

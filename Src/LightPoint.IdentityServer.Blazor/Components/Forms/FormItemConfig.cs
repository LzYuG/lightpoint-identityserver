using AntDesign;
using LightPoint.IdentityServer.Blazor.Components.Forms.Models;

namespace LightPoint.IdentityServer.Blazor.Components.Forms
{
    public class FormItemConfig
    {
        public string Prop { get; set; } = "";
        public string Label { get; set; } = "";

        public bool Disabled { get; set; }

        public string Placeholder { get; set; } = "";

        public int Span { get; set; } = 24;

        public FormItemType FormItemType { get; set; }

        public List<FormValidationRule> FormValidationRules { get; set; } = new List<FormValidationRule>();
        /// <summary>
        /// 传入当前的表单对象，判断是否需要隐藏当前的表单元素
        /// </summary>
        public Func<object, bool> IsHiddenExpression { get; set; } = x => false;

        public decimal Max { get; set; } = 9999999;

        public decimal Min { get; set; } = 0;
        /// <summary>
        /// 使用多行文本框时的行数
        /// </summary>
        public uint Rows { get; set; } = 2;

        public List<LightPointSelectOption> Options { get; set; } = new List<LightPointSelectOption>();
        /// <summary>
        /// 使用日期区间选择器的时候，使用预定义的数组来存储数据，并将[0]分发给Model.Prop，[1]分发给Model.EndProp
        /// </summary>
        public DateTime?[] DateTimePickerHelper { get; set; } = new DateTime?[2];
        /// <summary>
        /// 将Prop视为Start，EndProp视为End，则开始时间赋值至Model.Prop，结束时间赋值至Model.EndProp
        /// </summary>
        public string? EndProp { get; set; }
        /// <summary>
        /// 使用多选选择器的时候，用于绑定Value的方法，仅支持string
        /// </summary>
        public List<string> MultiSelectorHelper { get; set; } = new List<string>();
        /// <summary>
        /// MultiSelectorHelper = Model.Prop.Split(MultiSelectorSplitChart).ToList();
        /// </summary>
        public string MultiSelectorSplitChart { get; set; } = ",";

		public Action<object>? OnValueChangeed { get; set; }
		/// <summary>
		/// 使用选择器的时候如果属性不是string类型，用于辅助绑定的value
		/// （Bug ：Ant Blazor 其他类型使用选择器的时候无法回绑默认值）
		/// </summary>
		public string? Value { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDesign;
using LightPoint.IdentityServer.Blazor.Components.HumanMachineValidator.Models;
using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE01.SystemResources;
using Microsoft.AspNetCore.Hosting;

namespace LightPoint.IdentityServer.Blazor.Components.HumanMachineValidator
{
    partial class HumanMachineValidator
    {
        [Inject]
        public IWebHostEnvironment? WebHostEnvironment { get; set; }

        private VerifyCodeBase64? VerifyCode { get; set; }
        private VerifyImage? VerifyImage { get; set; }
        [Parameter]
        public ServerCommonConfigDQM? CurrentServerCommonConfig { get; set; }
        [Parameter]
        public EventCallback OnSuccess { get; set; }

        private string? InputCode { get; set; }

        private bool HasErrorState { get; set; }

        public void Refresh()
        {
            if (this.CurrentServerCommonConfig!.HumanMachineVerificationType == HumanMachineVerificationType.随机字符校验码 ||
                this.CurrentServerCommonConfig!.HumanMachineVerificationType == HumanMachineVerificationType.数学计算校验码)
            {
                InputCode = "";
                GetVerifyCode(this.CurrentServerCommonConfig!.HumanMachineVerificationType);
            }
            else if (this.CurrentServerCommonConfig!.HumanMachineVerificationType == HumanMachineVerificationType.滑动验证码)
            {
                SliderValue = 0;
                VerifyImage = VerifyImageHelper.CreateVerifyImage(WebHostEnvironment!, 250, 150);
            }
           
            StateHasChanged();
        }


        private async Task Success()
        {
            await OnSuccess.InvokeAsync();
        }

        #region 字符
        private void GetVerifyCode(HumanMachineVerificationType type)
        {
            VerifyCode verifyCode = VerifyCodeHelper.CreateVerifyCode(WebHostEnvironment!, type);

            var now = DateTime.Now;
            VerifyCode = new VerifyCodeBase64()
            {
                ImageBase64Str = "data:image/jpeg;base64," + Convert.ToBase64String(verifyCode.Image!),
                Result = verifyCode.Result
            };
        }

        private async Task Submit()
        {
            HasErrorState = false;
            if (InputCode!.ToUpper() == VerifyCode!.Result!.ToUpper())
            {
                await Success();
            }
            else
            {
                Refresh();
                HasErrorState = true;
            }
        }
        #endregion

        #region 滑动
        private int SliderValue { get; set; }

        // 更新滑动条的值
        void UpdateSliderValue(ChangeEventArgs e)
        {
            SliderValue = int.Parse(e.Value!.ToString()!);
            StateHasChanged();
        }
        /// <summary>
        /// 停止滑动
        /// </summary>
        /// <param name="e"></param>
        async Task StopUpdateSliderValue(ChangeEventArgs e)
        {
            SliderValue = int.Parse(e.Value!.ToString()!);
            StateHasChanged();
            HasErrorState = false;
            var successRangeMin = VerifyImage!.GapX - 3;
            var successRangeMax = VerifyImage!.GapX + 3;
            if (SliderValue + 25 > successRangeMin && SliderValue + 25 < successRangeMax)
            {
                await Success();
            }
            else
            {
                Refresh();
                HasErrorState = true;
            }
        }
        
        #endregion
    }
}

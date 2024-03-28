using AntDesign;
using LightPoint.IdentityServer.Blazor.Components.Forms;
using LightPoint.IdentityServer.Blazor.Components.Tables;
using LightPoint.IdentityServer.Blazor.Utils;
using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.Shared;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Page
{
    public class PageBase<Tid, TDomainModel, TQueryDto, TCommandDto, TService> : PageBaseContent<Tid, TDomainModel, TQueryDto, TCommandDto, TService>
        where TDomainModel : class, IDomainModelBase<Tid>, new()
        where TQueryDto : class, IQueryDtoBase<Tid>, new()
        where TCommandDto : class, ICommandDtoBase<Tid>, new()
        where TService : IAppService<Tid, TDomainModel, TQueryDto, TCommandDto>
    {
    }

    public class PageBase<Tid, TDomainModel, TQueryDto, TCommandDto> : PageBaseContent<Tid, TDomainModel, TQueryDto, TCommandDto, IAppService<Tid, TDomainModel, TQueryDto, TCommandDto>>
        where TDomainModel : class, IDomainModelBase<Tid>, new()
        where TQueryDto : class, IQueryDtoBase<Tid>, new()
        where TCommandDto : class, ICommandDtoBase<Tid>, new()
    {
    }


    public abstract class PageBaseContent<Tid, TDomainModel, TQueryDto, TCommandDto, TService> : GlobalConfigPageBase
        where TDomainModel : class, IDomainModelBase<Tid>, new()
        where TQueryDto : class, IQueryDtoBase<Tid>, new()
        where TCommandDto : class, ICommandDtoBase<Tid>, new()
        where TService : IAppService<Tid, TDomainModel, TQueryDto, TCommandDto>
    {
        public virtual List<FormItemConfig> FormItemConfigs { get; set; } = new List<FormItemConfig>();
        #region Injects
        [Inject]
        public ModalService? ModalService { get; set; }
        [Inject]
        public TService? Service { get; set; }
        #endregion
        [CascadingParameter]
        public SystemTenantDQM NowSystemTenant { get; set; } = new SystemTenantDQM() { TenantIdentifier = "" };

        [Parameter]
        public LightPointTable<TQueryDto>? LightPointTableRef { get; set; }
        public ILightPointForm<TCommandDto>? LightPointFormRef { get; set; }

        public virtual TableParameters<TQueryDto> TableParameters { get; set; } = new TableParameters<TQueryDto>();


        protected TCommandDto? EditingModel { get; set; }


        public virtual async Task OpenForm(TQueryDto? queryDto)
        {
            await Task.Run(() =>
            {
                if (this.LightPointFormRef == null)
                {
                    return;
                }
                if (queryDto != null)
                {
                    EditingModel = Mapper<TQueryDto, TCommandDto>.MapToNewObj(queryDto);
                    this.LightPointFormRef.Open(EditingModel);
                }
                else
                {
                    EditingModel = new TCommandDto();
                    this.LightPointFormRef.Open(EditingModel);
                }
            });
        }

        public virtual async Task SubmitForm(DialogFormSubmitModel<TCommandDto> submitModel)
        {
            submitModel.Close!();
            await Update(submitModel.Model!);
            await Search();
        }

        public virtual async Task Search()
        {
            TableParameters.Loading = true;
            StateHasChanged();
            var searchExpression = MontageSearchExpression(TableParameters);
            var searchResult = await Service!.GetApiBoCollectionAsyncBySearchData(TableParameters.ToRequestParams(), searchExpression!, true);
            if(searchResult.Datas != null)
            {
                TableParameters.Datas = searchResult.Datas.ToList();
                TableParameters.Total = searchResult.Total;
            }
            TableParameters.Loading = false;
            StateHasChanged();

        }

        public virtual Expression<Func<TDomainModel, bool>>? MontageSearchExpression(TableParameters<TQueryDto> TableParameters)
        {
            return x => x.TenantIdentifier == this.NowSystemTenant.TenantIdentifier;
        }


        public virtual async Task Update(TCommandDto commandDtoBase)
        {
            commandDtoBase.TenantIdentifier = this.NowSystemTenant.TenantIdentifier;

            var res = await Service!.SetAndSaveEntityData(commandDtoBase, true);
            if (res.IsSuccess)
            {
                await Search();
                await Success(res.Message!);
            }
            else
            {
                await Error(res.Message!);
            }
        }


        public virtual async Task Delete(Tid id)
        {
            await Task.Run(() =>
            {
                ModalService!.Confirm(new ConfirmOptions()
                {
                    Title = "确定要删除该数据吗?",
                    Content = "此操作不可撤销!",
                    OnOk = async (arg) =>
                    {
                        await Service!.DeleteBoAsync(x => x.Id!.Equals(id));
                        await Search();
                    },
                    OkType = "danger",
                });
            });
        }


        #region Message
        [Inject]
        public IMessageService? MessageService { get; set; }

        public virtual async Task Success(string message = "操作成功")
        {
            await MessageService!.Success(message);
        }

        public virtual async Task Error(string message = "操作失败")
        {
            await MessageService!.Error(message);

        }

        public virtual async Task Warning(string message)
        {
            await MessageService!.Warning(message);
        }
        #endregion
    }
}

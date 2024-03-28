using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using LightPoint.IdentityServer.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Imps
{
    public class ApplicationUserService : AppService<Guid, ApplicationUser, ApplicationUserDQM, ApplicationUserDCM>, IApplicationUserService
    {
        private readonly IQueryRepository<Guid, ApplicationUser> _queryRepository;
        private readonly ICommandRepository<Guid, ApplicationUser> _commandRepository;
        private readonly IModelMapper<Guid, ApplicationUser, ApplicationUserDQM, ApplicationUserDCM> _modelMapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICommandRepository<Guid, ApplicationUserWithRole> _applicationUserWithRoleCommandRepository;
        private readonly IQueryRepository<Guid, ApplicationUserWithRole> _applicationUserWithRoleQueryRepository;
        private readonly IQueryRepository<int, ApplicationUserClaim> _applicationUserClaimQueryRepository;
        private readonly IQueryRepository<Guid, ApplicationRole> _applicationRoleQueryRepository;

        public ApplicationUserService(IQueryRepository<Guid, ApplicationUser> queryRepository, 
            ICommandRepository<Guid, ApplicationUser> commandRepository, 
            IModelMapper<Guid, ApplicationUser, ApplicationUserDQM, ApplicationUserDCM> modelMapper,
            UserManager<ApplicationUser> userManager, 
            ICommandRepository<Guid, ApplicationUserWithRole> applicationUserWithRoleCommandRepository,
            IQueryRepository<Guid, ApplicationUserWithRole> applicationUserWithRoleQueryRepository,
            IQueryRepository<int, ApplicationUserClaim> applicationUserClaimQueryRepository,
            IQueryRepository<Guid, ApplicationRole> applicationRoleQueryRepository
            ) : base(queryRepository, commandRepository, modelMapper)
        {
            _queryRepository = queryRepository;
            _commandRepository = commandRepository;
            _modelMapper = modelMapper;
            _userManager = userManager;
            _applicationUserWithRoleCommandRepository = applicationUserWithRoleCommandRepository;
            _applicationUserWithRoleQueryRepository = applicationUserWithRoleQueryRepository;
            _applicationUserClaimQueryRepository = applicationUserClaimQueryRepository;
            _applicationRoleQueryRepository = applicationRoleQueryRepository;
        }

        public virtual async Task<DataAccessResult> ValidateCredentials(string userName, string password)
        {
            var userWithIdentityStore = await _userManager.FindByNameAsync(userName);

            // 用户存在且启用
            if (userWithIdentityStore != null && userWithIdentityStore.IsEnable)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(userWithIdentityStore!, password);
                if (checkPasswordResult)
                {
                    return DataAccessResult.Success("密码校验成功");
                }
                else
                {
                    return DataAccessResult.Error("用户名或密码输入错误");
                }
            }

            return DataAccessResult.Error("用户名或密码输入错误");
        }

        public virtual async Task<DataAccessResult> ResetPassword(SystemAccountConfigDQM? systemAccountConfig, string userName, string newPassword)
        {
            if (newPassword == null || !new Regex(systemAccountConfig!.AccountUserPasswordValidationRegex!).IsMatch(newPassword))
            {
                return DataAccessResult.Error("密码格式错误，" + systemAccountConfig!.AccountUserPasswordValidateFailedMessage);
            }

            var userWithIdentityStore = await _userManager.FindByNameAsync(userName);
            // 用户存在且启用
            if (userWithIdentityStore != null && userWithIdentityStore.IsEnable)
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(userWithIdentityStore!);
                if (removePasswordResult.Succeeded)
                {
                    var resetPasswordResult = await _userManager.AddPasswordAsync(userWithIdentityStore!, newPassword);
                    if (resetPasswordResult.Succeeded)
                    {
                        return DataAccessResult.Success("密码修改成功");
                    }
                    else
                    {
                        return DataAccessResult.Error("密码修改失败，系统发生严重错误，请联系管理员");
                    }
                }
                else
                {
                    return DataAccessResult.Error("清除旧密码失败，请联系管理员");
                }
            }
            return DataAccessResult.Error("用户不存在");
        }


        public virtual async Task<DataAccessResult> RegisterUser(SystemAccountConfigDQM? systemAccountConfig, ApplicationUserDCM applicationUser, string password, IEnumerable<ApplicationUserClaimDM> claims, IEnumerable<ApplicationRoleDCM> roles)
        {
            if (applicationUser.UserName == null || !new Regex(systemAccountConfig!.AccountUserNameValidationRegex!).IsMatch(applicationUser!.UserName))
            {
                return DataAccessResult.Error("用户名格式错误，" + systemAccountConfig!.AccountUserNameValidateFailedMessage);
            }

            if (password == null || !new Regex(systemAccountConfig!.AccountUserPasswordValidationRegex!).IsMatch(password))
            {
                return DataAccessResult.Error("密码格式错误，" + systemAccountConfig!.AccountUserPasswordValidateFailedMessage);
            }

            if (await _queryRepository.HasBoAsync(x => x.UserName == applicationUser.UserName && x.Id != applicationUser.Id))
            {
                return DataAccessResult.Error("已有相同用户名的用户");
            }

            var sagas = new List<Func<Task>>();

            var createUserResult = await _userManager.CreateAsync((await _modelMapper.ToDomainModel(applicationUser!))!, password);
            if (!createUserResult.Succeeded)
            {
                return DataAccessResult.Error("用户注册失败");
            }
            sagas.Add(async () => await _userManager.DeleteAsync((await _modelMapper.ToDomainModel(applicationUser!))!));

            if (claims != null && claims.Count() > 0)
            {
                var addClaimsResult = await _userManager.AddClaimsAsync((await _modelMapper.ToDomainModel(applicationUser!))!, claims.Select(x => new System.Security.Claims.Claim(x.ClaimType!, x.ClaimValue!)));
                if (!addClaimsResult.Succeeded)
                {
                    foreach(var saga in sagas)
                    {
                        await saga();
                    }
                    return DataAccessResult.Error("添加用户标识失败");
                }
                else
                {
                    sagas.Prepend(async () => await _userManager.RemoveClaimsAsync((await _modelMapper.ToDomainModel(applicationUser))!, claims.Select(x => new System.Security.Claims.Claim(x.ClaimType!, x.ClaimValue!))));
                }
            }

            if(roles != null && roles.Count() > 0)
            {
                // 添加角色，不能使用UserManager的AddToRoles，AspNetIdentity自身是没有给角色划分租户的
                var addToReolsResult = await AddToRoles(applicationUser, roles);
                if (!addToReolsResult.IsSuccess)
                {
                    foreach (var saga in sagas)
                    {
                        await saga();
                    }
                    return DataAccessResult.Error("添加用户角色失败");
                }
            }

            return DataAccessResult.Success("注册成功");
        }


        public virtual async Task<DataAccessResult> AddToRoles(ApplicationUserDCM applicationUser, IEnumerable<ApplicationRoleDCM> roles)
        {
            if(applicationUser == null || roles == null || roles.Count() == 0)
            {
                return DataAccessResult.Error("数据校验失败");
            }

            List<ApplicationUserWithRole> applicationUserWithRoles = new List<ApplicationUserWithRole>();

            foreach(var role in roles)
            {
                applicationUserWithRoles.Add(new ApplicationUserWithRole()
                {
                    RoleId = role.Id,
                    UserId = applicationUser.Id,
                    Id = Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    TenantIdentifier = role.TenantIdentifier,
                });
            }

            return await _applicationUserWithRoleCommandRepository.UpdateAndSaveAsync(applicationUserWithRoles);
        }

        public virtual async Task<DataAccessResult> RemoveFromRoles(ApplicationUserDCM applicationUser, IEnumerable<ApplicationRoleDCM> roles)
        {
            if (applicationUser == null || roles == null || roles.Count() == 0)
            {
                return DataAccessResult.Error("数据校验失败");
            }

            var roleIds = roles.Select(x => x.Id);

            return await _applicationUserWithRoleCommandRepository.DeleteBoAsync(x => x.UserId == applicationUser.Id && roleIds.Contains(x.RoleId));
        }

        public virtual async Task<List<string>> InRoles(Guid userId, string tenantIdentifier)
        {
            var roleRelations = (await _applicationUserWithRoleQueryRepository.GetBoCollectionAsync(0, 0, x => x.UserId == userId && (x.TenantIdentifier == tenantIdentifier),
                false, x => x.SortCode!)).ToList();

            var roleIds = roleRelations.Select(x => x.RoleId);
            return (await _applicationRoleQueryRepository.GetBoCollectionAsync(0, 0, x => roleIds.Contains(x.Id), false, x => x.SortCode!)).Select(x => x.Name!).ToList();
        }

        public virtual async Task<List<ApplicationUserClaimDM>> GetClaims(Guid userId, string tenantIdentifier)
        {
            var claimRelations = (await _applicationUserClaimQueryRepository.GetBoCollectionAsync(0, 0, x => x.UserId == userId && x.TenantIdentifier == tenantIdentifier,
               false, x => x.SortCode!)).ToList();

            return Mapper<ApplicationUserClaim, ApplicationUserClaimDM>.MapToNewObj(claimRelations);
        }

        /// <summary>
        /// 重新特例用户的更新
        /// 区分更新及注册
        /// </summary>
        /// <param name="apiEntity"></param>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public virtual async Task<DataAccessResult> SetAndSaveEntityData(SystemAccountConfigDQM? systemAccountConfig, ApplicationUserDCM apiEntity)
        {
            if (apiEntity.Password == null || !new Regex(systemAccountConfig!.AccountUserPasswordValidationRegex!).IsMatch(apiEntity.Password))
            {
                return DataAccessResult.Error("密码格式错误，" + systemAccountConfig!.AccountUserPasswordValidateFailedMessage);
            }

            if (apiEntity.UserName == null || !new Regex(systemAccountConfig!.AccountUserNameValidationRegex!).IsMatch(apiEntity.UserName))
            {
                return DataAccessResult.Error("用户名格式错误，" + systemAccountConfig!.AccountUserNameValidateFailedMessage);
            }

            var expressions = LambdaCreater.GetIncludeExpressions<ApplicationUser>();
            var dbData = await _queryRepository.GetBoAsync(x => x.Id == apiEntity.Id);
            if (dbData == null)
            {
                // 注册
                var registerResult = await _userManager.CreateAsync(Mapper<ApplicationUserDCM, ApplicationUser>.MapToNewObj(apiEntity)!, apiEntity.Password!);
                if (registerResult.Succeeded)
                {
                    return DataAccessResult.Success();
                }
                else
                {
                    return DataAccessResult.Error("注册失败，" + string.Join(";", registerResult.Errors));
                }
            }
            else
            {
                // 一些字段不应该在更新的时候被篡改
                apiEntity.PasswordHash = dbData.PasswordHash;
                apiEntity.UserName = dbData.UserName;
                return await base.SetAndSaveEntityData(apiEntity, expressions);
            }
        }
    }
}

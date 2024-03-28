using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.IdentityResource
{
    [Table("IdentityServerIdentityResources")]
    public class IdentityServerIdentityResource : DomainModel<Guid>
    {
        public bool Enabled { get; set; } = true;
        public string? DisplayName { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public List<IdentityServerIdentityResourceClaim>? UserClaims { get; set; }
        public List<IdentityServerIdentityResourceProperty>? Properties { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
        public bool NonEditable { get; set; }

        public override async Task<DomainModelValidationResult> DataValidation(RepositoryFactory repositoryFactory)
        {
            var queryRepository = repositoryFactory.GetQueryRepository<Guid, IdentityServerIdentityResource>();
            if (await queryRepository.HasBoAsync(x => x.TenantIdentifier == this.TenantIdentifier && x.Name == this.Name && x.Id != this.Id))
            {
                return DomainModelValidationResult.Fail("当前租户归属下，已有同名IdentityResource");
            }
            return DomainModelValidationResult.Success();
        }

        public override async Task<DomainModelOperationRusult> BeforeAddOrUpdate(RepositoryFactory repositoryFactory)
        {
            var commandRepository = repositoryFactory.GetCommandRepository<Guid, IdentityServerIdentityResource>();
            var dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerIdentityResourceClaim>(nameof(IdentityServerIdentityResourceClaim.IdentityResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerIdentityResourceProperty>(nameof(IdentityServerIdentityResourceProperty.IdentityResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            return new DomainModelOperationRusult(dataAccessResult);
        }

        public override async Task<DomainModelOperationRusult> BeforeDelete(RepositoryFactory repositoryFactory)
        {
            var commandRepository = repositoryFactory.GetCommandRepository<Guid, IdentityServerIdentityResource>();
            var dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerIdentityResourceClaim>(nameof(IdentityServerIdentityResourceClaim.IdentityResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerIdentityResourceProperty>(nameof(IdentityServerIdentityResourceProperty.IdentityResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            return new DomainModelOperationRusult(dataAccessResult);
        }
    }
}

using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources.ExtensionProperties;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiResource
{
    [Table("IdentityServerApiResources")]
    public class IdentityServerApiResource : DomainModel<Guid>
    {
        public bool Enabled { get; set; } = true;
        public string? DisplayName { get; set; }
        public string? AllowedAccessTokenSigningAlgorithms { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public List<IdentityServerApiResourceSecret> Secrets { get; set; } = new List<IdentityServerApiResourceSecret>();
        public List<IdentityServerApiResourceScope> Scopes { get; set; } = new List<IdentityServerApiResourceScope>();
        public List<IdentityServerApiResourceClaim> UserClaims { get; set; } = new List<IdentityServerApiResourceClaim>();
        public List<IdentityServerApiResourceProperty> Properties { get; set; } = new List<IdentityServerApiResourceProperty>();
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
        public DateTime? LastAccessed { get; set; }
        public bool NonEditable { get; set; }

        public override async Task<DomainModelValidationResult> DataValidation(RepositoryFactory repositoryFactory)
        {
            var queryRepository = repositoryFactory.GetQueryRepository<Guid, IdentityServerApiResource>();
            if (await queryRepository.HasBoAsync(x => x.TenantIdentifier == this.TenantIdentifier && x.Name == this.Name && x.Id != this.Id))
            {
                return DomainModelValidationResult.Fail("当前租户归属下，已有同名ApiResource");
            }
            return DomainModelValidationResult.Success();
        }

        public override async Task<DomainModelOperationRusult> BeforeAddOrUpdate(RepositoryFactory repositoryFactory)
        {
            var commandRepository = repositoryFactory.GetCommandRepository<Guid, IdentityServerApiResource>();
            var dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerApiResourceProperty>(nameof(IdentityServerApiResourceProperty.ApiResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerApiResourceScope>(nameof(IdentityServerApiResourceScope.ApiResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerApiResourceClaim>(nameof(IdentityServerApiResourceClaim.ApiResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerApiResourceSecret>(nameof(IdentityServerApiResourceSecret.ApiResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            return new DomainModelOperationRusult(dataAccessResult);
        }

        public override async Task<DomainModelOperationRusult> BeforeDelete(RepositoryFactory repositoryFactory)
        {
            var commandRepository = repositoryFactory.GetCommandRepository<Guid, IdentityServerApiResource>();
            var dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerApiResourceProperty>(nameof(IdentityServerApiResourceProperty.ApiResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerApiResourceScope>(nameof(IdentityServerApiResourceScope.ApiResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerApiResourceClaim>(nameof(IdentityServerApiResourceClaim.ApiResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerApiResourceSecret>(nameof(IdentityServerApiResourceSecret.ApiResourceId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            return new DomainModelOperationRusult(dataAccessResult);
        }
    }
}

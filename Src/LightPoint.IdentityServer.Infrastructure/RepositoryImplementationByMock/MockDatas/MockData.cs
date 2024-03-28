namespace LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByMock.MockDatas
{
    public class MockData
    {

        public List<T> List<T>() where T : class, new()
        {
            var prop = typeof(MockData).GetProperties().First(x => x.PropertyType == typeof(List<T>));
            return (List<T>)prop.GetValue(this)!;
        }

        public IQueryable<T> Set<T>() where T : class, new()
        {
            var prop = typeof(MockData).GetProperties().First(x => x.PropertyType == typeof(List<T>));
            return ((List<T>)prop.GetValue(this)!).AsQueryable();
        }
    }
}

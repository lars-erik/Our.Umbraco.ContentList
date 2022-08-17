using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Persistence.Repositories;

namespace Our.Umbraco.ContentList.Tests.Support
{
    public class FakeLanguageRepository : FakeReadWriteRepository<int, ILanguage>, ILanguageRepository
    {
        public FakeLanguageRepository() : base(x => x.Id)
        {
        }

        public ILanguage GetByIsoCode(string isoCode)
        {
            return Items.Values.FirstOrDefault(x => x.CultureName == isoCode);
        }

        public int? GetIdByIsoCode(string isoCode, bool throwOnNotFound = true)
        {
            return GetByIsoCode(isoCode).Id;
        }

        public string GetIsoCodeById(int? id, bool throwOnNotFound = true)
        {
            return Get(id.Value).CultureName;
        }

        public string GetDefaultIsoCode()
        {
            return "en-US";
        }

        public int? GetDefaultId()
        {
            return 1;
        }
    }
}
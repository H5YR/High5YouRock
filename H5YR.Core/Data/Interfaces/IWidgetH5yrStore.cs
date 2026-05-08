using H5YR.Core.Data.Entities;

namespace H5YR.Core.Data.Interfaces
{
    public interface IWidgetH5yrStore
    {
        IEnumerable<WidgetH5yr> GetAll();

        IEnumerable<WidgetH5yr> GetRecent(int count);

        IEnumerable<WidgetH5yr> GetRecentBefore(DateTime before, int count);

        int GetSubmissionCountToday(string externalUserId, string authProvider, string targetSiteHost);

        void Save(WidgetH5yr poco);

        void Delete(WidgetH5yr poco);

        int GetTotalCount();
    }
}

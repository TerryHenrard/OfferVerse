using OfferVerse.Models;
using System.Diagnostics.Eventing.Reader;

namespace OfferVerse.DAL.Interfaces
{
    public interface IReportDAL
    {
        public bool InsertReport(Report report);

        public List<Report> GetReports();

        public bool SanctionUser(int userId);

        public bool DeleteReport(int reportId);
    }
}

using OfferVerse.Models;
using System.Diagnostics.Eventing.Reader;

namespace OfferVerse.DAL.Interfaces
{
    public interface IReportDAL
    {
        public bool InsertReport(Report report);
    }
}

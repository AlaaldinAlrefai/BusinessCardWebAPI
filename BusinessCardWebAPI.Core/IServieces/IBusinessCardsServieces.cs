using BusinessCardWebAPI.Core.Data;
using BusinessCardWebAPI.Core.DTO;
using BusinessCardWebAPI.Core.IReposetory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardWebAPI.Core.IServieces
{
    public interface IBusinessCardsServieces:IBusinessCardsReposetory
    {
        // Add this method to handle batch insertion of business cards
        Task AddRangeAsync(IEnumerable<BusinessCards> businessCards);

        // New methods for file imports Csv,Xml 
        Task<List<CreateBusinessCardsDto>> ImportFromCsvAsync(StreamReader stream);
        Task<List<CreateBusinessCardsDto>> ImportFromXmlAsync(StreamReader stream);

        // New methods for file Exports Csv,Xml 
        Task<byte[]> ExportToCsvAsync();
        Task<byte[]> ExportToXmlAsync();

    }
}

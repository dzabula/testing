using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apartment.Application.UseCase.DTO
{
    public class FilterPaginationApartmentDto 
    {
        public string Title { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }   
        public List<int> CategoryIds { get; set; }
        public List<int> CityIds { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

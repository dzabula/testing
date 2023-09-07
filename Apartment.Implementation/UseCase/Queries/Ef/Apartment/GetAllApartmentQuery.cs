using Apartment.Application.UseCase.DTO;
using Apartment.Application.UseCase.Queries.Apartment;
using Apartment.DataAccess;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apartment.Implementation.UseCase.Queries.Ef.Apartment
{
    public class GetAllApartmentQuery : EfBase, IGetAllApartmentsQuery
    {
        private IHttpContextAccessor httpContextAccessor;
        public GetAllApartmentQuery(IHttpContextAccessor httpContextAccessor, ApartmentContext context, IMapper mapper) : base(context, mapper)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public int Id => 11;

        public string Name => "Get All Apartment - EF";

        public string Description => "";

        public IEnumerable<ApartmentDto> Execute(FilterPaginationApartmentDto request)
        {
            var serverDomainName = httpContextAccessor.HttpContext.Request;
            string filePath = $"{serverDomainName.Scheme}://{serverDomainName.Host}/api/uploads/";
          

            var res = Context.Apartments.Include(x => x.CategoryOfApartment)
                .Include(x => x.City)
                .Include(x => x.Author)
                .Include(x => x.Prices)
                .Include(x => x.Thumb)
                .Include(x => x.Rates).AsQueryable();

            res = res.Where(x=> x.DeletedAt == null);

            if (!string.IsNullOrEmpty(request.Title))
            {
                res = res.Where(x => x.Title.Contains(request.Title));
            }

            if( request.MaxPrice > request.MinPrice )
            {
                res = res.Where(x =>  x.Prices.OrderByDescending(y => y.CreatedAt).First().PricePerNight > request.MinPrice
               && x.Prices.OrderByDescending(y => y.CreatedAt).First().PricePerNight < request.MaxPrice);
            };

            if (request.CategoryIds != null && !request.CategoryIds.Contains(0))
            {
                res = res.Where(x => request.CategoryIds.Contains(x.CategoryId));
            }
            if (request.CityIds !=null  && !request.CityIds.Contains(0))
            {
                res = res.Where(x => request.CityIds.Contains(x.CityId));
            }

            if (request.PageNumber == 0 || request.PageSize == 0)
            {
                request.PageNumber = 1;
                request.PageSize = 1000;
            }

            res = res.OrderByDescending(x =>  x.Priority ).ThenByDescending(x=>x.CreatedAt);


            var pagination = res.Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

            var result = pagination.Select(x => new ApartmentDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                GoogleMap = x.GoogleMap,
                User = new UserDto { Id = x.Author.Id, FullName = x.Author.FirstName + x.Author.LastName, Email = x.Author.Email },
                Category = new CategoryDto { Id = x.CategoryId, Name = x.CategoryOfApartment.Name },
                City = new CityDto { Id = x.CityId, Name = x.City.Name },
                File = new FileDto { Id = x.FileId, Alt = x.Thumb.Alt, Path = filePath + x.Thumb.Path, Extension = x.Thumb.Extension, Size = x.Thumb.Size },
                Price = x.Prices.Where(y => y.ApartmentId == x.Id).OrderByDescending(y => y.CreatedAt).Select(y => new PriceDto
                {
                    Id = y.Id,
                    PricePerNight = y.PricePerNight,
                    PricePerNightWeekend = y.PricePerNightWeekend,
                    PriceOnHoliday = y.PriceOnHoliday

                }).FirstOrDefault(),
                Rates = x.Rates.Select(y => new RateDto
                {
                    Id = y.Id,
                    Value = y.Value,
                    UserId = y.UserId
                }),
                Surface = x.Surface,
                Priority = x.Priority

            }).ToList();

            
            return result;

        }
    }
}

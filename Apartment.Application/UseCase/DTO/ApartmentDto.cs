using Apartment.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apartment.Application.UseCase.DTO
{
    public class ApartmentDto : BaseDto
    {

        public string Title { get; set; }
        public string Description { get; set; }
        public string GoogleMap { get; set; }
        public PriceDto Price { get; set; }
        public UserDto User { get; set; }
        public CategoryDto Category { get; set; }
        public CityDto City { get; set; }
        public FileDto File { get; set; }
       // public IEnumerable<PriceDto> Prices { get; set; }
        public IEnumerable<RateDto> Rates { get; set; }
        public IEnumerable<FileDto> Images { get; set; }
        public IEnumerable<ReservationDto> Reservations { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; }
        public IEnumerable<ApartmentSpecificationDto> ApartmentSpecifications { get; set; }
        public int Priority { get; set; }
        public float Surface { get; set; }

    }

    public class CreateApartmentDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string GoogleMap { get; set; }
        public int CategoryId { get; set; }
        public int CityId { get; set; }
        public string File { get; set; }
        public int? FileId { get; set; }
        public PriceDto Price { get; set; }
        public IEnumerable<string> Images { get; set; }
        public int Priority { get; set; }
        public float Surface { get; set; }
        public IEnumerable<ApartmentSpecificationDto> Specification { get; set; }
    }

    public class UpdateApartmentDto : BaseDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string GoogleMap { get; set; }
        public int CategoryId { get; set; }
        public int CityId { get; set; }
        public string File { get; set; } = null;
        public int? FileId { get; set; } = null;
        public PriceUpdateDto Price { get; set; }
        public IEnumerable<int> ImagesIds { get; set; } = null;
        public IEnumerable<string> Images { get; set; } = null;
        public int Priority { get; set; } = 3;
        public float Surface { get; set; }

    }
}

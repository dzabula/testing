using Apartment.Application.Exceptions;
using Apartment.Application.UseCase.Commands.Apartment;
using Apartment.Application.UseCase.DTO;
using Apartment.Application.UseCase.Queries.Apartment;
using Apartment.DataAccess;
using Apartment.Domain;
using Apartment.Domain.Entities;
using Apartment.Implementation.UseCase.UploadImages;
using Apartment.Implementation.Validators;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apartment.Implementation.UseCase.Commands.Ef.Apartment
{
    public class CreateApartmentCommand : EfBase, ICreateApartmentCommand
    {
        private CreateApartmentValidator validatorApartment;
        private AddPriceForApartmentValidator validatorPrice;
        private UploaderImages uploader;
        private IUser author;
     
        public CreateApartmentCommand(Implementation.UseCase.UploadImages.UploaderImages uploader, ApartmentContext context, IMapper mapper, IUser author, CreateApartmentValidator validatorApartment, AddPriceForApartmentValidator validatorPrice) : base(context, mapper)
        {
            this.validatorApartment = validatorApartment;
            this.validatorPrice = validatorPrice;
            this.author = author;
            this.uploader = uploader;
           
        }

        public int Id => 5;

        public string Name => "Create Apartment - EF";

        public string Description => "";

        public void Execute(CreateApartmentDto request)
        {
            if (request is null) throw new BadRequestException();
            if (request.Priority == 0) request.Priority = 3;
            validatorApartment.ValidateAndThrow(request);
            validatorPrice.ValidateAndThrow(request.Price);

            var specification = request.Specification.Select(x => new ApartmentSpecification
            {
                SpecificationId = x.SpecificationId,
                Value = x.Value,
            });

            var newApartment = new Domain.Entities.Apartment
            {
                Title = request.Title,
                Description = request.Description,
                GoogleMap = request.GoogleMap,
                UserId = author.Id,
                CategoryId = request.CategoryId,
                CityId = request.CityId,   
                Priority = request.Priority,
                Surface = request.Surface,
                ApartmentSpecifications = specification.ToList()
                
            };

            /*ADDING THUMB FILE*/
            uploader.UploadImages(new List<string> { request.File },newApartment, true);
            /*END THUMB FILE*/

            /*ADDING OTHER IMAGES*/
            uploader.UploadImages(request.Images, newApartment, false);
            /*END ADDING OTHER IMAGES*/


            /*ADDING PRICE*/
            var newPrice = new Price
            {
                Apartment= newApartment,
                PriceOnHoliday = request.Price.PriceOnHoliday,
                PricePerNight = request.Price.PricePerNight,
                PricePerNightWeekend = request.Price.PricePerNightWeekend
            };
            newApartment.Prices = new List<Domain.Entities.Price>();
            newApartment.Prices.Add(newPrice);
            /*END ADDING PRICE*/


            /*ADDING APARTMENT*/
            Context.Apartments.Add(newApartment);
            Context.SaveChanges();
            /*END APARTMENT*/

            

        }

       
    }
}

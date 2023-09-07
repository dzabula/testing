using Apartment.Application.Exceptions;
using Apartment.Application.UseCase.Commands.Apartment;
using Apartment.Application.UseCase.DTO;
using Apartment.DataAccess;
using Apartment.Domain.Entities;
using Apartment.Implementation.UseCase.UploadImages;
using Apartment.Implementation.Validators;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Apartment.Implementation.UseCase.Commands.Ef.Apartment
{
    public class UpdateApartmentCommand : EfBase,IUpdateApartmentCommand
    {
        private UpdateApartmentValidator validator;
        private UploaderImages uploader;
        public UpdateApartmentCommand(UploaderImages uploader,UpdateApartmentValidator validator, ApartmentContext context, IMapper mapper) : base(context, mapper)
        {
            this.validator = validator;
            this.uploader = uploader;
        }

        public int Id => 23;

        public string Name => "Update Apartment - EF";

        public string Description => "";

        public void Execute(UpdateApartmentDto request)
        {
            validator.ValidateAndThrow(request);
            var oldApartment = Context.Apartments.Include(x=>x.Prices).Include(x=>x.Images).FirstOrDefault(x=>x.Id == request.Id);

            if(oldApartment == null) { throw new EntityNotFoundException("Apartment",request.Id); }

            UpdatePriceIfChanged(oldApartment, request);

            UpdateImagesIfChanged(oldApartment, request);

            oldApartment = Context.Apartments.Find(request.Id);

            if(request.Priority != oldApartment.Priority)
            {
                oldApartment.Priority = request.Priority;
            }

            if(request.Title != oldApartment.Title)
            {
                oldApartment.Title = request.Title;
                
            }
            if (request.Description != oldApartment.Description)
            {
                oldApartment.Description = request.Description;
               
            }
            if (request.GoogleMap != oldApartment.GoogleMap)
            {
                oldApartment.GoogleMap = request.GoogleMap;
            }
            if (request.CityId != oldApartment.CityId)
            {
                oldApartment.CityId = request.CityId;
              
            }
            if (request.CategoryId != oldApartment.CategoryId)
            {
                oldApartment.CategoryId = request.CategoryId;
  
            }
            if (request.Priority != oldApartment.Priority)
            {
                oldApartment.Priority = request.Priority;
   
            }
            if (request.Surface != oldApartment.Surface)
            {
                oldApartment.Surface = request.Surface;

            }

            Context.SaveChanges();

           
        }

        public void UpdatePriceIfChanged(Domain.Entities.Apartment oldApartment, UpdateApartmentDto request)
        {
            var oldPrice = Context.Prices.OrderByDescending(x => x.CreatedAt).FirstOrDefault(x=>x.ApartmentId == request.Id);

            bool isPriceChange = false;
            Price newPrice = oldPrice;

            if (request.Price.PriceOnHoliday != oldPrice.PriceOnHoliday)
            {
                newPrice.PriceOnHoliday = request.Price.PriceOnHoliday;
                isPriceChange = true;
            }
            if (request.Price.PricePerNight != oldPrice.PricePerNight)
            {
                newPrice.PricePerNight = request.Price.PricePerNight;
                isPriceChange = true;
            }
            if (request.Price.PricePerNightWeekend != oldPrice.PricePerNightWeekend)
            {
                newPrice.PricePerNightWeekend = request.Price.PricePerNightWeekend;
                isPriceChange = true;
            }
            if (isPriceChange)
            {
                
                oldApartment.Prices.Remove(Context.Prices.Where(x=>x.ApartmentId == request.Id).OrderByDescending(x=>x.CreatedAt).FirstOrDefault());
                oldApartment.Prices.Add(new Price
                {
                    PriceOnHoliday = newPrice.PriceOnHoliday,
                    PricePerNight = newPrice.PricePerNight,
                    PricePerNightWeekend = newPrice.PricePerNightWeekend,
                }) ;
              //  Context.Entry(oldApartment.Prices).State = EntityState.Modified;


            }

        }
    
        public void UpdateImagesIfChanged(Domain.Entities.Apartment oldApartment, UpdateApartmentDto request)
        {

            var isChangeImages = false;

            
            // Proveravamo da li je sa klijenta stigla ijedna slika koja je vec postojala u apartmanu, jer
            // je omoguceno da klijent ne posalje sve id-eve slika i samim tim ce obrisati neku vec postojecu sliku.
            if (request.ImagesIds != null)
            {
                    if (request.ImagesIds.Count() != oldApartment.Images.Count())
                    {
                        isChangeImages = true;
                    }
            }
     
            // ukoliko je doslo do promene starih slika, svaka slika cji se ID ne nalazi u request
            // se brise.
            if (isChangeImages)
            {
                foreach (var old in oldApartment.Images)
                {
                    if (!request.ImagesIds.Any(x => x == old.Id))
                    {
                        Context.Images.Remove(old);
                    }
                }
               // Context.Entry(oldApartment.Images).State = EntityState.Modified;

            }

            //Nove slike koje su prosledjene sa base64 se aploaduju i dodeljuju apartmanu
            if (request.Images != null && request.Images.Any())
            {
                uploader.UploadImages(request.Images, oldApartment, false);
               // Context.Entry(oldApartment.Images).State = EntityState.Modified;
            }

            //Ukoliko je poslat Id stare Thumb slike ne dolazi do promena
            //ukoliko nije uploadovace se nova thumb slika i dodeliti apartmanu
            if(!string.IsNullOrEmpty(request.File))
            { 
                uploader.UploadImages(new List<string> { request.File },oldApartment,true);
                //Context.Entry(oldApartment.Thumb).State = EntityState.Modified;

            }

        }
    }
}

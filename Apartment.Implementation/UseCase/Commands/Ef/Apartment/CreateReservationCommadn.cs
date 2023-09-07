using Apartment.Application.Exceptions;
using Apartment.Application.Mail;
using Apartment.Application.UseCase.Commands.Apartment;
using Apartment.Application.UseCase.DTO;
using Apartment.DataAccess;
using Apartment.Domain;
using Apartment.Domain.Entities;
using Apartment.Implementation.Validators;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apartment.Implementation.UseCase.Commands.Ef.Apartment
{
    public class CreateReservationCommadn : EfBase, ICreateReservationsCommand
    {
        private IEmailSend emailSender;
        private CreateReservationValidator validator;
        public CreateReservationCommadn(CreateReservationValidator validator,ApartmentContext context, IMapper mapper, IEmailSend email) : base(context, mapper)
        {
            this.validator = validator;
            this.emailSender = email;
        }

        public int Id => 56;

        public string Name => "Create Reservation - EF";

        public string Description => "";

        public void Execute(CreateReservationDto request)
        {
            if (request is null) throw new BadRequestException();
            validator.ValidateAndThrow(request);

            double sumPrice = CalculateSumPrice(request);
            if (request.UserId != null && request.UserId.Value == 0) request.UserId = null;

            var newReservation = new Domain.Entities.Reservation
            {
                ApartmentId = request.ApartmentId,
                UserId = request.UserId,
                From = request.From,
                To = request.To,
                FullName = request.FullName,
                Phone = request.Phone,
                Amount = sumPrice,
                IsPaid = false,
                CreatedAt = DateTime.Now
            };

            if (!string.IsNullOrEmpty(request.Email))
            {
                var apartment = Context.Apartments.Find(request.ApartmentId);
                this.emailSender.Send(new MailDto
                {
                    Title = "Rezervacija Lux Place",
                    To = request.Email,
                    Message = $"Hvala na ukazanom poverenju. Apartman: {apartment.Title}\n Rezervacija glasi na ime: {request.FullName}\n Check In: {request.From.ToString("yyyy-MM-dd")}\n Check Out: {request.To.Value.ToString("yyyy-MM-dd")}\n Ukupna suma za uplatu: {sumPrice}\n Datum Kreiranja Rezervacije: {DateTime.Now}"
                });
            }

            Context.Reservations.Add(newReservation);
            Context.SaveChanges();
            
        }
        
        private double CalculateSumPrice(CreateReservationDto request)
        {
            // Unos cena za dane u nedelji, vikend i praznike
            var apartment = Context.Apartments.Where(x => x.Id == request.ApartmentId).Select(x => new ApartmentDto
            {
                Price = new PriceDto{
                 PricePerNight = x.Prices.OrderByDescending(y=>y.CreatedAt).FirstOrDefault().PricePerNight,
                 PricePerNightWeekend = x.Prices.OrderByDescending(y=>y.CreatedAt).FirstOrDefault().PricePerNightWeekend,
                 PriceOnHoliday = x.Prices.OrderByDescending(y=>y.CreatedAt).FirstOrDefault().PriceOnHoliday,
                }
            }).FirstOrDefault();

            var price = apartment.Price;

            double pricePerNight = price.PricePerNight;
            double pricePerNightWeekend = price.PricePerNightWeekend;
            double priceOnHoliday = price.PriceOnHoliday;

            DateTime dateStart = request.From;

            DateTime dateEnd = request.To.Value;
            double sumPrice = 0;

            // Iteracija kroz sve datume od useljenja do iseljenja
            DateTime currentDate = dateStart;
            while (currentDate < dateEnd)
            {
                // Provera da li je trenutni dan vikend
                bool week = currentDate.DayOfWeek == DayOfWeek.Friday ||
                              currentDate.DayOfWeek == DayOfWeek.Saturday ||
                              currentDate.DayOfWeek == DayOfWeek.Sunday;

                // Provera da li je trenutni dan praznik (ovo treba da se dopuni sa stvarnim praznicima)
                bool praznik = false;

                // Odabir cene na osnovu dana u nedelji, vikenda ili praznika
                if (praznik)
                {
                    sumPrice += priceOnHoliday;
                }
                else if (week)
                {
                    sumPrice += pricePerNightWeekend;
                }
                else
                {
                    sumPrice += pricePerNight;
                }

                // Prelazak na sledeći dan
                currentDate = currentDate.AddDays(1);
            }

            return sumPrice;

        }
    }
}

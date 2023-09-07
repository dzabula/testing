using Apartment.Application.Exceptions;
using Apartment.Application.UseCase.Commands.Apartment;
using Apartment.DataAccess;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apartment.Implementation.UseCase.Commands.Ef.Apartment
{
    public class DeleteReservationCommand : EfBase, IDeleteReservationCommand
    {
        public DeleteReservationCommand(ApartmentContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public virtual int Id => 28;

        public string Name => "Administrator delete reservation - Ef";

        public string Description => "";

        public void Execute(int request)
        {
            var reservationToBeDelete = Context.Reservations.Find(request);
            if (reservationToBeDelete == null) throw new EntityNotFoundException("Reservation",request);
            Context.Reservations.Remove(reservationToBeDelete);
            Context.SaveChanges();
        }
    }
}

using Apartment.Application.Exceptions;
using Apartment.Application.UseCase.Commands.Apartment;
using Apartment.DataAccess;
using Apartment.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apartment.Implementation.UseCase.Commands.Ef.Apartment
{
    public class CancelledReservationCommand : EfBase, ICancelledReservationCommand
    {
        public CancelledReservationCommand(ApartmentContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public int Id => 59;

        public string Name => "Cancelled Reservation - EF";

        public string Description => "Cancel or Recancel reservation";

        public void Execute(int request)
        {
            var query = Context.Reservations.Where(x => x.Id == request).AsQueryable();
            query = AddUserValidationUseCase(query);
            var obj = query.FirstOrDefault();
            if (obj == null) throw new BadRequestException();
            obj.Cancelled = !obj.Cancelled;
            Context.SaveChanges();
        }

        protected virtual IQueryable<Reservation> AddUserValidationUseCase(IQueryable<Reservation> query)
        {
            return query;
        }
    }
}

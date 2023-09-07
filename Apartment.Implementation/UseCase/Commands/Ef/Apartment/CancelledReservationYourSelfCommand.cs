using Apartment.DataAccess;
using Apartment.Domain;
using Apartment.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apartment.Implementation.UseCase.Commands.Ef.Apartment
{
    public class CancelledReservationYourSelfCommand : CancelledReservationCommand
    {
        private IUser user;
        public CancelledReservationYourSelfCommand(ApartmentContext context, IMapper mapper, IUser user) : base(context, mapper)
        {
            this.user = user;
        }

        protected override IQueryable<Reservation> AddUserValidationUseCase(IQueryable<Reservation> query)
        {
            return query.Where(x=> x.UserId == this.user.Id && !x.Cancelled);
        }
    }
}

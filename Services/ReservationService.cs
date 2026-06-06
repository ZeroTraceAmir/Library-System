using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace library_system.Services
{
    internal class ReservationService
    {
        private readonly IReservationRepository reservationRepository;

        public ReservationService(IReservationRepository reservationRepository)
        {
            this.reservationRepository = reservationRepository;
        }
        public List<Reservation> GetAllReservation()
        {
            return reservationRepository.GetAll();
        }
        public Reservation? GetReservationById(int id)
        {
            return reservationRepository.GetById(id);
        }
        public AddReservation(Reservation reservation)
        {
            List<reservation> reservations = reservationRepository.GetAll();
            reservation.Id = reservations.count == 0 ? 1 : reservations.Max(r => r.Id) + 1;
            reservationRepository.Add(reservation);
        }
    }
}

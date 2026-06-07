using library_system.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using library_system.Models;

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
        public void AddReservation(Reservation reservation)
        {
            List<Reservation> reservations = reservationRepository.GetAll();
            reservation.Id = reservations.Count == 0 ? 1 : reservations.Max(r => r.Id) + 1;
            reservationRepository.Add(reservation);
        }
    }
}

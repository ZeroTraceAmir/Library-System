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
        public List<Reservation> GetAllReservations()
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
        public void UpdateReservation(Reservation reservation)
        {
            reservationRepository.Update(reservation);
        }

        public void DeleteReservation(int id)
        {
            reservationRepository.Delete(id);
        }

        public List<Reservation> GetReservationsByCustomerId(int customerId)
        {
            return reservationRepository.GetAll()
            .Where(r => r.CustomerId == customerId)
                .ToList();
        }

        public void ReserveBook(Book book, int customerId)
        {
            Reservation reservation = new Reservation
            {
                Id = reservationRepository.GetAll().Any()?
                reservationRepository.GetAll().Max(r => r.Id) + 1 : 1,

                CustomerId = customerId,
                BookId = book.Id,
                ReservationDate = DateTime.Now,
                IsActive = true
            };

            reservationRepository.Add(reservation);
        }

        public void CancelReservation(int reservationId)
        {
            Reservation? reservation = reservationRepository.GetById(reservationId);

            if (reservation == null)
            {
                return;
            }
            reservation.IsActive = false;

            reservationRepository.Update(reservation);
        }

    }
}

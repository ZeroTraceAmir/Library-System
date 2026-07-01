using System;
using System.Collections.Generic;
using System.Linq;
using library_system.Enums;
using library_system.Interfaces;
using library_system.Models;

namespace library_system.Services
{
    public class NotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository)
        {
            _repository = repository;
        }

        public void CreateOverdueNotification(int customerId, int bookId, DateTime dueDate)
        {
            OverdueNotification notification = new OverdueNotification
            {
                Id = GetNextId(),
                CustomerId = customerId,
                BookId = bookId,
                DueDate = dueDate,
                Type = NotificationType.Overdue,
                CreatedAt = DateTime.Now,
            };

            _repository.Add(notification);
        }

        public void CreateReturnReminderNotification(int customerId, int bookId, DateTime dueDate)
        {
            ReturnReminderNotification notification = new ReturnReminderNotification
            {
                Id = GetNextId(),
                CustomerId = customerId,
                BookId = bookId,
                DueDate = dueDate,
                Type = NotificationType.ReturnReminder,
                CreatedAt = DateTime.Now,
            };

            _repository.Add(notification);
        }

        public void CreateReservationConfirmedNotification(int customerId, int bookId)
        {
            ReservationConfirmedNotification notification = new ReservationConfirmedNotification
            {
                Id = GetNextId(),
                CustomerId = customerId,
                BookId = bookId,
                Type = NotificationType.ReservationConfirmed,
                CreatedAt = DateTime.Now,
            };

            _repository.Add(notification);
        }

        public void CreateReservationCancelledNotification(int customerId, int bookId)
        {
            ReservationCancelledNotification notification = new ReservationCancelledNotification
            {
                Id = GetNextId(),
                CustomerId = customerId,
                BookId = bookId,
                Type = NotificationType.ReservationCancelled,
                CreatedAt = DateTime.Now,
            };

            _repository.Add(notification);
        }

        public void CreateBookBorrowedNotification(int customerId, int bookId)
        {
            BookBorrowedNotification notification = new BookBorrowedNotification
            {
                Id = GetNextId(),
                CustomerId = customerId,
                BookId = bookId,
                Type = NotificationType.BookBorrowed,
                CreatedAt = DateTime.Now,
            };

            _repository.Add(notification);
        }

        public void CheckOverdueAndReminders(List<Loan> loans)
        {
            List<Notification> allNotifications = _repository.GetAll();

            foreach (Loan loan in loans)
            {
                if (loan.ReturnDate != null)
                    continue;

                bool overdueExists = allNotifications.Any(n =>
                    n is OverdueNotification
                    && n.CustomerId == loan.CustomerId
                    && n is BookNotification bn
                    && bn.BookId == loan.BookId
                );

                if (loan.DueDate < DateTime.Now && !overdueExists)
                {
                    CreateOverdueNotification(loan.CustomerId, loan.BookId, loan.DueDate);
                }
                else if (loan.DueDate <= DateTime.Now.AddDays(1) && !overdueExists)
                {
                    bool reminderExists = allNotifications.Any(n =>
                        n is ReturnReminderNotification
                        && n.CustomerId == loan.CustomerId
                        && n is BookNotification bn2
                        && bn2.BookId == loan.BookId
                    );

                    if (!reminderExists)
                        CreateReturnReminderNotification(
                            loan.CustomerId,
                            loan.BookId,
                            loan.DueDate
                        );
                }
            }
        }

        public List<Notification> GetNotificationsForCustomer(int customerId)
        {
            return _repository
                .GetAll()
                .Where(n => n.CustomerId == customerId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public void DeleteNotification(int id)
        {
            _repository.Delete(id);
        }

        private int GetNextId()
        {
            List<Notification> notifications = _repository.GetAll();
            return notifications.Any() ? notifications.Max(n => n.Id) + 1 : 1;
        }
    }
}

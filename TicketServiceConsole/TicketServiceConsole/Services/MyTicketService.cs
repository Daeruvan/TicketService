//-----------------------------------------------------------------------
// <copyright file="MyTicketService.cs" company="Company">
// Copyright (c) Company. All rights reserved.
// </copyright>
// <author>Nathan VanHouten</author>
//-----------------------------------------------------------------------

namespace TicketServiceConsole.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using TicketServiceConsole.Models;

    /// <summary>
    /// Manages the transactions of seats for a ticketed event.
    /// Interacts with the data controller to operate on the ticketed event
    /// </summary>
    public class MyTicketService : TicketService
    {
        /// <summary>
        /// Binds ticketing service to a ticketed event. Does not allow rebind.
        /// </summary>
        /// <param name="ticketedEventName">The name of the ticketed event.</param>
        public void BindTicketServiceToEvent(string ticketedEventName)
        {
            if (string.IsNullOrEmpty(this.TicketedEventName))
            {
                TicketedEventName = ticketedEventName;
            }
            else
            {
                throw new InvalidOperationException("The ticket service is already bound to an event");
            }
        }

        /// <summary>
        /// Gets the number of seats available
        /// </summary>
        /// <returns>The number of seats available</returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "forced dependency on style of TicketService interface")]
        public int numSeatsAvailable()
        {
            return DataController.Instance.GetNumberOfAvailableSeats(TicketedEventName);
        }

        /// <summary>
        /// Finds and holds seats requested by customer
        /// </summary>
        /// <param name="numSeats">The number of seats to be held</param>
        /// <param name="customerEmail">The customer's email address</param>
        /// <returns>The seat hold</returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "forced dependency on style of TicketService interface")]
        public SeatHold findAndHoldSeats(int numSeats, string customerEmail)
        {
            string error = string.Empty;
            if (!RegexUtilities.IsValidEmail(customerEmail))
            {
                error += "Invalid email address. ";
            }

            if (numSeats < 1)
            {
                error += "Cannot hold 0 or less seats.";
            }

            if (!string.IsNullOrEmpty(error))
            {
                throw new ArgumentException(error);
            }

            SeatHold seatHold  = DataController.Instance.FindAndHoldSeats(TicketedEventName, numSeats, customerEmail);
            return seatHold;
        }

        /// <summary>
        /// Reserves previously held seats
        /// </summary>
        /// <param name="seatHoldId">The seat hold Id</param>
        /// <param name="customerEmail">The customer's email address</param>
        /// <returns>The reservation confirmation code</returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "forced dependency on style of TicketService interface")]
        public string reserveSeats(int seatHoldId, string customerEmail)
        {
            return DataController.Instance.ReserveSeats(TicketedEventName, seatHoldId, customerEmail);
        }

        /// <summary>
        /// Gets the name of the Ticketed Event connected to this ticketing service
        /// </summary>
        public string TicketedEventName { get; private set; }
    }
}
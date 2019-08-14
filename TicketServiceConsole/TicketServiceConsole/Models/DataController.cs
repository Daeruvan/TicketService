//-----------------------------------------------------------------------
// <copyright file="DataController.cs" company="Company">
// Copyright (c) Company. All rights reserved.
// </copyright>
// <author>Nathan VanHouten</author>
//-----------------------------------------------------------------------

namespace TicketServiceConsole.Models
{
    using System;
    using System.Collections.Generic;
    using System.Timers;

    /// <summary>
    /// Mock data controller.
    /// Stores one ticketed event, bound to the ticket service.
    /// </summary>
    public class DataController
    {
        /// <summary>
        /// Instance of the data controller singleton
        /// </summary>
        private static DataController instance;

        /// <summary>
        /// Seat hold expiration timers
        /// </summary>
        private Dictionary<(int, string), Timer> seatHoldExpirationTimers = new Dictionary<(int, string), Timer>();

        /// <summary>
        /// Prevents a default instance of the <see cref="DataController" /> class from being created.
        /// </summary>
        private DataController()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the DataController
        /// </summary>
        public static DataController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataController();
                }

                return instance;
            }
        }       

        /// <summary>
        /// Gets or sets the TicketedEvent
        /// </summary>
        private TicketedEvent TicketedEvent { get; set; }

        /// <summary>
        /// Determines whether or not the data controller is initialized
        /// </summary>
        /// <returns>Is the data controller initialized?</returns>
        public bool Initialized()
        {
            return TicketedEvent != null;
        }

        /// <summary>
        /// Loads the ticketed event into the data controller
        /// </summary>
        /// <param name="ticketedEvent">The TicketedEvent</param>
        public void LoadTicketedEvent(TicketedEvent ticketedEvent)
        {
            this.TicketedEvent = ticketedEvent;
        }

        /// <summary>
        /// Gets the number of available seats
        /// </summary>
        /// <param name="ticketedEventName">the name of the ticketed event</param>
        /// <returns>the number of available seats</returns>
        public int GetNumberOfAvailableSeats(string ticketedEventName)
        {
            if (ticketedEventName != TicketedEvent.Name)
            {
                throw new Exception("Data controller failed! Ticket service is not bound to the ticketed event!");
            }

            return TicketedEvent.NumberOfAvailableSeats;
        }

        /// <summary>
        /// Finds and holds the seats for a customer
        /// </summary>
        /// <param name="ticketedEventName">the ticketed event name</param>
        /// <param name="numSeats">the number of seats to hold</param>
        /// <param name="email">the customer's email</param>
        /// <returns>The seat hold object</returns>
        public SeatHold FindAndHoldSeats(string ticketedEventName, int numSeats, string email)
        {
            if (ticketedEventName != TicketedEvent.Name)
            {
                throw new Exception("Data controller failed! Ticket service is not bound to the ticketed event!");
            }

            SeatHold seatHold = TicketedEvent.FindAndHoldSeats(numSeats, email);

              // Create the expiration timer for the seat hold
            Timer expirationTimer = new Timer
            {
                Interval = SeatHold.ExpiresAfter.TotalMilliseconds
            };
              
              // Add a delegate to free the seat hold when the time has elapsed
            expirationTimer.Elapsed += (source, args) => this.FreeSeatHoldOnExpiration(source, args, seatHold);
            expirationTimer.Start();

              // Reference the timer so it doesn't get collected
            this.seatHoldExpirationTimers.Add((seatHold.Id, seatHold.CustomerEmail), expirationTimer);
            return seatHold;
        }

        /// <summary>
        /// Reserves held seats.
        /// </summary>
        /// <param name="ticketedEventName">the ticketed event</param>
        /// <param name="seatHoldId">the seat hold id</param>
        /// <param name="email">the customer's email address</param>
        /// <returns>the reservation confirmation code</returns>
        public string ReserveSeats(string ticketedEventName, int seatHoldId, string email)
        {
            if (ticketedEventName != TicketedEvent.Name)
            {
                throw new Exception("Data controller failed! Ticket service is not bound to the ticketed event!");
            }

            Timer t;
            if (this.seatHoldExpirationTimers.TryGetValue((seatHoldId, email), out t))
            {
                t.Stop();
                this.seatHoldExpirationTimers.Remove((seatHoldId, email));
            }
            else
            {
                Console.WriteLine("Could not find the seat hold expiration timer... the seat hold may have expired.\n");
            }

            return TicketedEvent.ReserveSeats(seatHoldId, email);
        }

        /// <summary>
        /// Determines whether or not the free seat gold operation was successful;
        /// </summary>
        /// <param name="ticketedEventName">the name of the ticketed event</param>
        /// <param name="seatHoldId">the id of the seat hold</param>
        /// <param name="email">customer's email address associated with the seat hold</param>
        /// <returns>Is the seat hold free?</returns>
        public bool FreeSeatHold(string ticketedEventName, int seatHoldId, string email)
        {
            if (ticketedEventName != TicketedEvent.Name)
            {
                throw new Exception("Data controller failed! Ticket service is not bound to the ticketed event!");
            }

            return TicketedEvent.FreeSeatHold(seatHoldId, email);
        }

        /// <summary>
        /// Prints all information about the ticketed event
        /// </summary>
        /// <param name="ticketedEventName">The name of the ticketed event</param>
        public void PrintTicketedEventStatus(string ticketedEventName)
        {
            Console.WriteLine(TicketedEvent.ToString());
        }

        /// <summary>
        /// Frees a seat hold after the timer expires
        /// </summary>
        /// <param name="source">the object that fires the elapsed event</param>
        /// <param name="args">the arguments for the elapsed event</param>
        /// <param name="seatHold">the seat hold that will be freed since it expired</param>
        private void FreeSeatHoldOnExpiration(object source, ElapsedEventArgs args, SeatHold seatHold)
        {
            Timer t = (Timer)source;
            t.Stop();
            Console.WriteLine("SeatHold (" + seatHold.Id.ToString() + "," + seatHold.CustomerEmail + ") " + "expired!");
            Console.WriteLine("Freeing the expired Seathold");
            this.FreeSeatHold(TicketedEvent.Name, seatHold.Id, seatHold.CustomerEmail);
            this.seatHoldExpirationTimers.Remove((seatHold.Id, seatHold.CustomerEmail));
        }
    }
}
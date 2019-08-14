//-----------------------------------------------------------------------
// <copyright file="SeatHold.cs" company="Company">
// Copyright (c) Company. All rights reserved.
// </copyright>
// <author>Nathan VanHouten</author>
//-----------------------------------------------------------------------

namespace TicketServiceConsole.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The Seat Hold object
    /// </summary>
    public class SeatHold
    {
        public SeatHold()
        {
            if (ExpiresAfter == null)
            {
                ExpiresAfter = new TimeSpan(20);
            }
        }

        /// <summary>
        /// The time a seat hold can exist before it expires
        /// </summary>
        public static TimeSpan ExpiresAfter { get; private set; }

        /// <summary>
        /// Gets or sets the seat hold id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the customer email
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Gets or sets the seats being held
        /// </summary>
        public ICollection<Seat> Seats { get; set; }
        
        /// <summary>
        /// Sets the duration of the seat hold
        /// </summary>
        /// <param name="duration">the duration of the seat hold until it expires</param>
        public static void SetSeatHoldDuration(TimeSpan duration)
        {
            ExpiresAfter = duration;
        }

        /// <summary>
        /// Prints the SeatHold
        /// </summary>
        /// <returns>the seat hold information</returns>
        public override string ToString()
        {
            return "==============================================================================\n" +
                   "SeatHold: " + this.Id.ToString() + " " + this.CustomerEmail + '\n' +
                   this.PrintSeats();
        }

        /// <summary>
        /// Prints the seating arrangement, and shows the status of the seats.
        /// </summary>
        /// <returns>The seating arrangement.</returns>
        public string PrintSeats()
        {
            const int ValuesPerRow = 5;
            int valuesInRow = 0;
            string seats = string.Empty;
            foreach (Seat seat in this.Seats)
            {
                seats += "[" + seat.Row.ToString() + "," + seat.Column.ToString() + "]";
                if (valuesInRow++ < ValuesPerRow)
                {
                    seats += " ";
                }
                else
                {
                    seats += "\n";
                    valuesInRow = 0;
                }
            }

            return seats;
        }
    }
}
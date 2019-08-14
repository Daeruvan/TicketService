//-----------------------------------------------------------------------
// <copyright file="Seat.cs" company="Company">
// Copyright (c) Company. All rights reserved.
// </copyright>
// <author>Nathan VanHouten</author>
//-----------------------------------------------------------------------

namespace TicketServiceConsole.Models
{
    /// <summary>
    /// Represents status codes for the seat
    /// </summary>
    public enum SeatStatusType
    {
        /// <summary>
        /// The seat is available
        /// </summary>
        Available = 'a',

        /// <summary>
        /// The seat is held
        /// </summary>
        Held = 'h',

        /// <summary>
        /// The seat is reserved
        /// </summary>
        Reserved = 'r'
    }

    /// <summary>
    /// Information for a seat in a ticketed event
    /// </summary>
    public class Seat
    {
        /// <summary>
        /// Gets or sets the row value of the seat. Index starts at 1.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the column value of the seat. Index starts at 1.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the favorability of the seat
        /// </summary>
        public int Favorability { get; set; }

        /// <summary>
        /// Gets or sets a character indicating the the status of the seat.
        /// a = available
        /// h = held
        /// r = reserved
        /// </summary>
        public char Status { get; set; }
    }
}
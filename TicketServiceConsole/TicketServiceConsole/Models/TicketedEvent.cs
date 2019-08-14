//-----------------------------------------------------------------------
// <copyright file="TicketedEvent.cs" company="Company">
// Copyright (c) Company. All rights reserved.
// </copyright>
// <author>Nathan VanHouten</author>
//-----------------------------------------------------------------------

namespace TicketServiceConsole.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Definition of VenueEvent, which bridges the venue and the ticket service
    /// </summary>
    public class TicketedEvent
    {
        /// <summary>
        /// Data for SeatHoldsCreated prop.
        /// </summary>
        private int seatHoldsCreated;

        /// <summary>
        /// The ticketed event object. 
        /// Stores information and operations for the ticketed event.
        /// </summary>
        /// <param name="name">name of the event</param>
        /// <param name="time">thime of the event</param>
        /// <param name="rows">rows of seats</param>
        /// <param name="cols">columns of seats</param>
        public TicketedEvent(string name, DateTime time, int rows, int cols)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Ticketed event name cannot be null");
            }

            if (time < DateTime.Now)
            {
                throw new ArgumentOutOfRangeException("Cannot create an event time in the past!");
            }

            if (rows < 1)
            {
                throw new ArgumentOutOfRangeException("Rows of seats in the event cannot be less that one");
            }

            if (cols < 1)
            {
                throw new ArgumentOutOfRangeException("Columns of seats in the event cannot be less than one");
            }

            this.NumberOfRows = rows;
            this.NumberOfColumns = cols;
            this.Name = name;
            this.Time = time;
            this.NumberOfAvailableSeats = this.NumberOfRows * this.NumberOfColumns;

            SeatHolds = new List<SeatHold>();
            Reservations = new List<(string, SeatHold)>();
            Seats = new List<Seat>(rows * cols);

              // Initialize the seats 
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int favorability = GetSeatFavorability(row, col, rows, cols);
                    Seats.Add(new Seat()
                    {
                        Row = row + 1,
                        Column = col + 1,
                        Favorability = favorability,
                        Status = (char)SeatStatusType.Available
                    });
                }
            }
        }

        /// <summary>
        /// Gets the seats used in the ticket event
        /// </summary>
        public ICollection<Seat> Seats { get; private set; }

        /// <summary>
        /// Gets the seat holds
        /// </summary>
        public ICollection<SeatHold> SeatHolds { get; private set; }

        /// <summary>
        /// Gets the reservations
        /// </summary>
        public ICollection<(string, SeatHold)> Reservations { get; private set; }

        /// <summary>
        /// Determines seat favorability. This is added primarily so it could be extended later.
        /// The proximity of the seats closest to the front is the most important factor
        /// The second factor is the proximity to the center.
        /// </summary>
        /// <param name="rowNumber">The row number of the seat</param>
        /// <param name="seatNumber">The number of the seat</param>
        /// <returns>The favorability rating of the seat. Lower value indicates more favorable.</returns>
        public static int GetSeatFavorability(int row, int col, int rows, int cols)
        {
            int center = cols / 2;
            // score for the favorability of the row - the closer to the front, the better.
            int rowScore = row * rows;
            // score for the favorability of the column - the closer to the center, the better.
            int columnScore = Math.Abs(col - center);
            return rowScore + columnScore;
        }

        /// <summary>
        /// Finds and hold available seats for the event
        /// </summary>
        /// <param name="numSeats">the number of seats to hold</param>
        /// <param name="customerEmail">The customer email</param>
        /// <returns>The seat hold</returns>
        public SeatHold FindAndHoldSeats(int numSeats, string customerEmail)
        {
            if (!RegexUtilities.IsValidEmail(customerEmail) || string.IsNullOrEmpty(customerEmail))
            {
                throw new ArgumentOutOfRangeException("Invalid email address");
            }

            if (numSeats < 1)
            {
                throw new ArgumentOutOfRangeException("Cannot held 0 or less seats!");
            }

            if (numSeats > this.NumberOfAvailableSeats) 
            {
                throw new ArgumentOutOfRangeException("Not enough available seats. Tried to hold " + numSeats.ToString() + " but there are only " + this.NumberOfAvailableSeats.ToString() +" seats available");
            }

            List<Seat> seats = new List<Seat>(numSeats);
              
              // build a list of the available seats and sort it in order of favorability
            List<Seat> availableSeats = this.Seats
                .Where(x => x.Status == (char)SeatStatusType.Available)
                .ToList();
            availableSeats.Sort(delegate (Seat x, Seat y)
                {
                    if (x.Favorability == y.Favorability) return 0;
                    else if (x.Favorability < y.Favorability) return -1;
                    else return 1;
                });
            
              // not enough seats to hold
            if (availableSeats.Count < numSeats)
            {
                return null;
            }

            seats.AddRange(availableSeats.Take(numSeats));

            SeatHold seatHold = new SeatHold();
            seatHold.Id = this.SeatHoldsCreated++;
            seatHold.CustomerEmail = customerEmail;
            seatHold.Seats = seats;

            
            foreach(Seat seat in seats)
            {
                Seats.First(s => s.Row == seat.Row && s.Column == seat.Column).Status = (char)SeatStatusType.Held;
            }

            this.NumberOfAvailableSeats -= numSeats;
            SeatHolds.Add(seatHold);

            return seatHold;
        }

        /// <summary>
        /// Reserves held seats in a seat hold
        /// </summary>
        /// <param name="seatHoldId">the seat hold id</param>
        /// <param name="email">the customers email</param>
        /// <returns>reservation confirmation</returns>
        public string ReserveSeats(int seatHoldId, string email)
        {
            List<SeatHold> seatHolds = SeatHolds.Where(sh => sh.Id == seatHoldId && sh.CustomerEmail == email).ToList();

            if (SeatHolds.Count < 1)
            {
                throw new Exception("Seat hold was not found given the id and email. Did it expire?");
            }
            else if (SeatHolds.Count > 1)
            {
                throw new Exception("Ambiguous request. More than one seat hold exists with that Id and email.");
            }
            else
            {
                SeatHold seatHold = seatHolds[0];
                SeatHolds.Remove(seatHold);
                string confirmation = ConfirmationCodeFactory.Generate();

                foreach (Seat seat in seatHold.Seats)
                {
                    seat.Status = (char)SeatStatusType.Reserved;
                }

                Reservations.Add((confirmation, seatHold));

                return confirmation;
            }
        }

        /// <summary>
        /// Frees a seat hold, making those seats available.
        /// </summary>
        /// <param name="seatHoldId">the id of the seat hold</param>
        /// <param name="email">the customers email address</param>
        /// <returns>Are the seats free?</returns>
        public bool FreeSeatHold(int seatHoldId, string email)
        {
            List<SeatHold> seatHolds = this.SeatHolds.Where(sh => sh.Id == seatHoldId && sh.CustomerEmail == email).ToList();

            if (this.SeatHolds.Count < 1)
            {
                throw new Exception("Seat hold was not found given the id and email. Did it expire?");
            }
            else if (this.SeatHolds.Count > 1)
            {
                throw new Exception("Ambiguous request. More than one seat hold exists with that Id and email.");
            }
            else
            {
                SeatHold seatHold = seatHolds[0];
                this.SeatHolds.Remove(seatHold);
                foreach (Seat seat in seatHold.Seats)
                {
                    seat.Status = (char)SeatStatusType.Available;
                }

                this.NumberOfAvailableSeats += seatHold.Seats.Count;

                return true;
            }
        }

        /// <summary>
        /// Simple hashing id for the seathold.
        /// </summary>
        /// <returns></returns>
        public int GenerateSeatHoldId()
        {
            return SeatHoldsCreated++;
        }

        /// <summary>
        /// Prints the ticketed event object
        /// </summary>
        /// <returns>the ticketed event information</returns>
        public override string ToString()
        {
            return "==============================================================================\n" +
                   "===========================   TicketedEvent   ================================\n" +
                   "Name: " + Name + '\n' +
                   "Date: " + Time.Date.ToString() + " Time: " + Time.TimeOfDay.ToString() + "\n" +
                   "Available Seats: " + NumberOfAvailableSeats.ToString() + "\n" +
                   PrintSeats() + "\n" +
                   PrintSeatHolds() + "\n" +
                   PrintReservations() + "\n";
        }

        /// <summary>
        /// Prints the seating arrangement
        /// </summary>
        /// <returns>the seating arrangement</returns>
        public string PrintSeats()
        {
            if (Seats == null)
            {
                return string.Empty;
            }

            const string stageSign = "[[  STAGE  ]]";

            string header = string.Empty;
            string output = string.Empty;
            string stage = string.Empty;
            for (int col = 0; col < (NumberOfColumns - stageSign.Length) / 2; col++)
            {
                stage += '-';
            }

            header = stage + stageSign + stage + '\n';
            stage = string.Empty;
            for (int col = 0; col < NumberOfColumns; col++)
            {
                stage += '-';
            }

            header += stage + '\n';

            char[,] seatsValue = new char[NumberOfRows, NumberOfColumns];
            foreach (Seat seat in Seats)
            {
                seatsValue[seat.Row - 1, seat.Column - 1] = seat.Status;
            }

            string seating = string.Empty;
            for (int row = 0; row < NumberOfRows; row++)
            {
                for (int col = 0; col < NumberOfColumns; col++)
                {
                    seating += seatsValue[row, col];
                }

                seating += '\n';
            }

            return header + seating;
        }

        /// <summary>
        /// Prints the seat hold information
        /// </summary>
        /// <returns>string containing seat hold information</returns>
        public string PrintSeatHolds()
        {
            string output = string.Empty;
            output += "======================================================\n" +
                     "Seat Holds:\n";
            foreach (SeatHold seathold in SeatHolds)
            {
                output += seathold.ToString() + '\n';
            }

            return output;
        }

        /// <summary>
        /// Prints the reservation information
        /// </summary>
        /// <returns></returns>
        public string PrintReservations()
        {
            string output = string.Empty;
            output += "======================================================\n" +
                      "Reservations:\n";

            foreach ((string, SeatHold) reservation in Reservations)
            {
                output += "-------------------------------------------------------\n";
                output += "Reservation Confirmation Code: " + reservation.Item1 + "\n";
                output += "Customer Email: " + reservation.Item2.CustomerEmail + "\n";
                output += "Seats: " + reservation.Item2.PrintSeats();
            }

            return output;
        }

        /// <summary>
        /// The name of the event
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The time the event takes place
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets or sets the number of rows in the seating arrangements
        /// </summary>
        public int NumberOfRows { get; set; }

        /// <summary>
        /// Gets or sets the number of columns in the seating arrangement
        /// </summary>
        public int NumberOfColumns { get; set; }

        /// <summary>
        /// Gets the number of seats in the seating arrangements
        /// </summary>
        public int NumberOfSeats
        {
            get
            {
                return this.NumberOfRows * this.NumberOfColumns;
            }
        }

        /// <summary>
        /// The number of available seats
        /// </summary>
        public int NumberOfAvailableSeats { get; set; }

        /// <summary>
        /// 4 Digit number, used to create ids.
        /// </summary>
        public int SeatHoldsCreated
        {
            get
            {
                return seatHoldsCreated;
            }
            set
            {
                seatHoldsCreated = value;
                if (seatHoldsCreated > 9999)
                {
                    seatHoldsCreated = 0;
                }
            }
        }
    }
}
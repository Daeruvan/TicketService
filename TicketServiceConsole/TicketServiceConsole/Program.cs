using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using TicketServiceConsole.Models;
using TicketServiceConsole.Services;

using NUnit.Framework;



namespace TicketServiceConsole
{
    public class ConsoleCommand
    {
        public const string TicketService = "ts";
        public const string Help = "help";
        public const string Exit = "exit";
    }

    public class TicketServiceAction
    {
        public const string Create = "create";
        public const string Print = "print";
        public const string Test = "test";
        public const string GetNumSeatsAvailable = "numseatsavailable";
        public const string FindAndHoldSeats = "findandholdseats";
        public const string ReserveSeats = "reserveseats";
    }

    class Program
    {

        private static MyTicketService ticketService;

        /// <summary>
        /// Used only in test functions
        /// </summary>
        private static TicketedEvent ticketedEvent;

        const string help = "This is a console program that tests a ticket service built on implementing a ticket service interface.\n" +
                            "Commands: \n" +
                            "           ts create     Initializes the TicketService and Event.\n" +
                            "                         optional params: [ticketServiceName = \"Ultimate-Code-Warrior-LIVE-@-Showbox\"] [year = 2019] [month = 9] [day = 17] [hour = 6] [min = 30] [rows = 20'] [cols = 15]\n" +
                            "                         example using default values: ts create\n" +
                            "                         example using custom value  : ts create Cat-Juggling-Competition 2019 8 20 6 00 30 30\n" +
                            "           ts print      Prints the seating arrangement for the event\n" +
                            "           ts test       Runs test cases \n" +
                            "                         Runs unit tests with asserts. Runs all by default. " +
                            "Ticket Service Operations:\n" +
                            "  ts numseatsavailable   Returns the number of available seats\n" +
                            "  ts findandholdseats    Finds and holds the number of seats makes a seat hold using the email address. Expires in 20 seconds." +
                            "                         required params: [numSeats] [customerEmail]\n" +
                            "  ts reserveseats        Reserves the SeatHold, finalizing purchase\n" +
                            "                         required params: [seatHoldId] [customerEmail]\n";



        static void Main(string[] args)
        {
            string command;
            while (true)
            {
                args = Console.ReadLine().Split(" ");

                command = args[0];

                switch(command.ToLower())
                {
                    case ConsoleCommand.TicketService:
                        RunTicketServiceCommand(args.Skip(1).ToArray());
                        break;

                    case ConsoleCommand.Help:
                        ShowHelp();
                        break;

                    case ConsoleCommand.Exit:
                        return;
                }
            }
        }

        public static void RunTicketServiceCommand(string[] args)
        {
            if (args == null)
            {
                Console.WriteLine("\n Invalid command. For information on ticket service  Run command: ts help");
                return;
            }

            string action = args[0];

            switch(action)
            {
                case TicketServiceAction.Create:
                    
                    List<string> paramList = args.Skip(1).ToList();
                    string name = string.Empty;
                    int year, month, day, hour, min, rows, cols;

                    if (paramList.Count > 0 && !string.IsNullOrEmpty(paramList[0]))
                    {
                        name = paramList[0];
                    }
                    else
                    {
                        CreateTicketService();
                        return;
                    }

                    if (paramList.Count > 1 && !string.IsNullOrEmpty(paramList[1]))
                    {
                        if(!int.TryParse(paramList[1], out year))
                        {
                            Console.WriteLine("Invalid command. Could parse year param into int. \n");
                            Console.WriteLine("Proper Use:      ts create     [ticketServiceName = \"Ultimate-Code-Warrior-LIVE-@-Showbox\"] [year = 2019] [month = 9] [day = 17] [hour = 6] [min = 30] [rows = 20'] [cols = 15]\n");
                            return;
                         }
                    }
                    else
                    {
                        CreateTicketService(name);
                        return;
                    }

                    if (paramList.Count > 2 && !string.IsNullOrEmpty(paramList[2]))
                    {
                        if (!int.TryParse(paramList[2], out month))
                        {
                            Console.WriteLine("Invalid command. Could parse month param into int. \n");
                            Console.WriteLine("Proper Use:      ts create     [ticketServiceName = \"Ultimate-Code-Warrior-LIVE-@-Showbox\"] [year = 2019] [month = 9] [day = 17] [hour = 6] [min = 30] [rows = 20'] [cols = 15]\n");
                            return;
                        }
                    }
                    else
                    {
                        CreateTicketService(name, year);
                        return;
                    }

                    if (paramList.Count > 3 && !string.IsNullOrEmpty(paramList[3]))
                    {
                        if (!int.TryParse(paramList[3], out day))
                        {
                            Console.WriteLine("Invalid command. Could parse day param into int. \n");
                            Console.WriteLine("Proper Use:      ts create     [ticketServiceName = \"Ultimate-Code-Warrior-LIVE-@-Showbox\"] [year = 2019] [month = 9] [day = 17] [hour = 6] [min = 30] [rows = 20'] [cols = 15]\n");
                            return;
                        }
                    }
                    else
                    {
                        CreateTicketService(name, year, month);
                        return;
                    }

                    if (paramList.Count > 4 && !string.IsNullOrEmpty(paramList[4]))
                    {
                        if (!int.TryParse(paramList[4], out hour))
                        {
                            Console.WriteLine("Invalid command. Could parse hour param into int. \n");
                            Console.WriteLine("Proper Use:      ts create     [ticketServiceName = \"Ultimate-Code-Warrior-LIVE-@-Showbox\"] [year = 2019] [month = 9] [day = 17] [hour = 6] [min = 30] [rows = 20'] [cols = 15]\n");
                            return;
                        }
                    }
                    else
                    {
                        CreateTicketService(name, year, month, day);
                        return;
                    }

                    if (paramList.Count > 5 && !string.IsNullOrEmpty(paramList[5]))
                    {
                        if (!int.TryParse(paramList[5], out min))
                        {
                            Console.WriteLine("Invalid command. Could parse min param into int. \n");
                            Console.WriteLine("Proper Use:      ts create     [ticketServiceName = \"Ultimate-Code-Warrior-LIVE-@-Showbox\"] [year = 2019] [month = 9] [day = 17] [hour = 6] [min = 30] [rows = 20'] [cols = 15]\n");
                            return;
                        }
                    }
                    else
                    {
                        CreateTicketService(name, year, month, day, hour);
                        return;
                    }

                    if (paramList.Count > 6 && !string.IsNullOrEmpty(paramList[6]))
                    {
                        if (!int.TryParse(paramList[6], out rows))
                        {
                            Console.WriteLine("Invalid command. Could parse rows param into int. \n");
                            Console.WriteLine("Proper Use:      ts create     [ticketServiceName = \"Ultimate-Code-Warrior-LIVE-@-Showbox\"] [year = 2019] [month = 9] [day = 17] [hour = 6] [min = 30] [rows = 20'] [cols = 15]\n");
                            return;
                        }
                    }
                    else
                    {
                        CreateTicketService(name, year, month, day, hour, min);
                        return;
                    }

                    if (paramList.Count > 7 && !string.IsNullOrEmpty(paramList[7]))
                    {
                        if (!int.TryParse(paramList[7], out cols))
                        {
                            Console.WriteLine("Invalid command. Could parse cols param into int. \n");
                            Console.WriteLine("Proper Use:      ts create     [ticketServiceName = \"Ultimate-Code-Warrior-LIVE-@-Showbox\"] [year = 2019] [month = 9] [day = 17] [hour = 6] [min = 30] [rows = 20'] [cols = 15]\n");
                            return;
                        }
                    }
                    else
                    {
                        CreateTicketService(name, year, month, day, hour, min, rows);
                        return;
                    }

                    CreateTicketService(name, year, month, day, hour, min, rows, cols);
                    break;
                case TicketServiceAction.Print:
                    if (ticketService == null)
                    {
                        Console.WriteLine("Invalid command. Ticket service has not been initialized. Run command ts create");
                        return;
                    }
                    DataController.Instance.PrintTicketedEventStatus(ticketService.TicketedEventName);
                    break;
                case TicketServiceAction.Test:
                    RunTests();                    
                    break;
                case TicketServiceAction.FindAndHoldSeats:
                    if (ticketService == null)
                    {
                        Console.WriteLine("Invalid command. Ticket service has not been initialized. Run command ts create");
                        return;
                    }

                    if (args.Count() < 3)
                    {
                        Console.WriteLine("Invalid command. findandholdseats requires two params: [int numSeats] [string email]");
                        return;
                    }
                    string numSeatsStr = args[1];
                    string customerEmail = args[2];
                    int numSeats;
                    if(!int.TryParse(numSeatsStr, out numSeats))
                    {
                        Console.WriteLine("Invalid command. findandholdseats first param must be parseable to int");
                    }
                    FindAndHoldSeats(numSeats, customerEmail);
                    break;
                case TicketServiceAction.GetNumSeatsAvailable:
                    if (ticketService == null)
                    {
                        Console.WriteLine("Invalid command. Ticket service has not been initialized. Run command ts create");
                        return;
                    }
                    GetNumSeatsAvailable();
                    break;
                case TicketServiceAction.ReserveSeats:
                    if (ticketService == null)
                    {
                        Console.WriteLine("Invalid command. Ticket service has not been initialized. Run command ts create");
                        return;
                    }
                    if (args.Count() < 3)
                    {
                        Console.WriteLine("Invalid command. reserveseats requires two params: [int seatHoldId] [string email]");
                        return;
                    }
                    string seatHoldIdStr = args[1];
                    string email = args[2];
                    int seatHoldId;
                    if (!int.TryParse(seatHoldIdStr, out seatHoldId))
                    {
                        Console.WriteLine("Invalid command. reserveseats first param must be parseable to int");
                    }
                    ReserveSeats(seatHoldId, email);
                    break;

                default:
                    Console.WriteLine("Command not recognized");
                    break;
            }
        }

        public static void CreateTicketService(string ticketedEventName = "Ultimate-Code-Warrior-LIVE-@-Showbox", int year = 2019, int month = 9, int day = 17, int hour = 6, int min = 30 , int rows = 20, int cols = 15)
        {
            if (ticketService != null)
            {
                Console.WriteLine("Ticket service already exists. Did you want to replace it with a fresh one? (Y/N) \n");

                string answer = Console.ReadLine();

                switch (answer.ToLower())
                {
                    case "y":
                        ticketService = new MyTicketService();
                        Console.WriteLine("Created new ticket service");
                        break;
                    case "n":
                        Console.WriteLine("Ticket service was not created.\n");
                        return;

                    default:
                        Console.WriteLine("Invalid response (Y/N). Ticket service was not created.\n");
                        break;
                }
            }
            else
            {
                ticketService = new MyTicketService();
            }

            Console.WriteLine("Creating new Ticketed Event");
            DataController.Instance.LoadTicketedEvent(new TicketedEvent(ticketedEventName, new DateTime(year, month, day, hour, min, 0), rows, cols));
            ticketService.BindTicketServiceToEvent(ticketedEventName);
            Console.WriteLine("Ticketed Event created:\n");
            DataController.Instance.PrintTicketedEventStatus(ticketedEventName);
            Console.WriteLine("Binding Ticket Service to ticketed event");
            Console.WriteLine("Ticket Service binded to Ticketed Event. Ready for Service.\n");
        }

        /// <summary>
        /// Runs unit tests
        /// </summary>
        public static void RunTests( )
        {
            Console.WriteLine("===================== RUNNING UNIT TESTS =============================");
            ticketService = null;
            DataController.Instance.LoadTicketedEvent(null);
            ticketedEvent = null;

            TestSetupTicketService();
            TestNumSeatsAvailable();
            TestFindAndHoldSeats();
            TestReserveSeats();

            ticketService = null;
            DataController.Instance.LoadTicketedEvent(null);
            ticketedEvent = null;

            Console.WriteLine("===================== UNIT TESTS PASSED! =============================");

            Console.WriteLine("Running tests resets the ticket service. Use ts create to create another ticket service");
        }

        /// <summary>
        /// Runs the num seats available ticket service
        /// </summary>
        /// <returns>the number of available seats</returns>
        public static int GetNumSeatsAvailable()
        {
            int numAvailable = ticketService.numSeatsAvailable();
            Console.WriteLine("Number of seats available: " + numAvailable.ToString() + "\n");
            return numAvailable;
        }

        /// <summary>
        /// Runs the find and hold seats ticket service
        /// </summary>
        /// <param name="numSeats">the number of seats to be held</param>
        /// <param name="email">the customer email address</param>
        /// <returns>The seathold object</returns>
        public static SeatHold FindAndHoldSeats(int numSeats, string email)
        {
            Console.WriteLine("Attempting to find " + numSeats.ToString() + " for " + email);
            SeatHold seatHold = null;
            try
            {
                seatHold = ticketService.findAndHoldSeats(numSeats, email);
            }
            catch(Exception e)
            {
                Console.WriteLine("SeatHold failed: " + e.Message.ToString());
                return seatHold;
            }
            

            if (seatHold != null)
            {
                Console.WriteLine("Seat hold successful. Expires in " + SeatHold.ExpiresAfter.TotalSeconds.ToString() + " seconds\n");
                Console.WriteLine(seatHold.ToString());
                return seatHold;
            }

            Console.WriteLine("Seat Hold was not successful\n");
            return seatHold;
        }

        /// <summary>
        /// Runs the reserve seats function
        /// </summary>
        /// <param name="seatHoldId">the seat hold id</param>
        /// <param name="email">the customer email</param>
        /// <returns>A confirmation code, if successful</returns>
        public static string ReserveSeats(int seatHoldId, string email)
        {
            string confirmation = string.Empty;
            Console.WriteLine("Attempted to reserve SeatHold: " + seatHoldId.ToString() + " for " + email);
            try
            {
                confirmation = ticketService.reserveSeats(seatHoldId, email);
            }
            catch(Exception e)
            {
                Console.WriteLine("Reservation failed" + e.Message.ToString());
                return confirmation;
            }

            Console.WriteLine("Reservation successful. Confirmation code: " + confirmation);
            return confirmation;
        }

        public static void ShowHelp()
        {
            Console.WriteLine(help);
        }

        /// <summary>
        /// Sets up a sample ticketed event and prints it to the console.
        /// </summary>
        public static void TestSetupTicketService()
        {
            Console.WriteLine("Creating a ticketed event");
            ticketedEvent = new TicketedEvent("Amazing-Concert-@-The-Showbox", new DateTime(2019, 9, 5, 4, 0, 0), 10, 15);
            Console.WriteLine('\n' + ticketedEvent.ToString() + '\n');
            Assert.AreEqual(ticketedEvent.NumberOfAvailableSeats, 150);
            Assert.AreEqual(ticketedEvent.NumberOfSeats, 150);
            Assert.AreEqual(ticketedEvent.NumberOfRows, 10);
            Assert.AreEqual(ticketedEvent.NumberOfColumns, 15);
            Assert.AreEqual(ticketedEvent.Name, "Amazing-Concert-@-The-Showbox");
            Assert.AreEqual(ticketedEvent.Time, new DateTime(2019, 9, 5, 4, 0, 0));
            Assert.AreEqual(ticketedEvent.Seats.Count, 150);
            Assert.IsNotNull(ticketedEvent.SeatHolds);
            Assert.IsNotNull(ticketedEvent.Reservations);
            Assert.AreEqual(ticketedEvent.SeatHoldsCreated, 0);

            Console.WriteLine("Ticketed Event successfully created.");

            Console.WriteLine("Binding Ticketed event to Controller");
            DataController.Instance.LoadTicketedEvent(ticketedEvent);
            Assert.IsTrue(DataController.Instance.Initialized());

            Console.WriteLine("Ticketed Event bound to data controller");

            Console.WriteLine("Creating Ticket Service.");
            ticketService = new MyTicketService();

            Console.WriteLine("Binding ticket service to ticketed event");
            ticketService.BindTicketServiceToEvent(ticketedEvent.Name);

            Console.WriteLine("Test no rebind of ticket service.");
            try
            {
                ticketService.BindTicketServiceToEvent("random-other-event");
                Assert.Fail("Ticket service was rebound, should not happen");
            }
            catch (InvalidOperationException e)
            {
                Assert.IsTrue(e.Message.Contains("The ticket service is already bound to an event"));
            }

            Console.WriteLine("Ticket Service Successfully setup.");
        }

        public static void TestNumSeatsAvailable()
        {
            if (ticketService == null || ticketedEvent == null)
            {
                Console.WriteLine("there is no ticket service. Run ts create or test case 1");
            }

            Assert.AreEqual(ticketedEvent.NumberOfAvailableSeats, ticketService.numSeatsAvailable());
        }

        public static void TestFindAndHoldSeats()
        {
            if (ticketService == null || ticketedEvent == null)
            {
                Console.WriteLine("there is no ticket service. Run ts create or test case 1");
            }

            TimeSpan seatHoldExpiresAfter = SeatHold.ExpiresAfter;

            SeatHold.SetSeatHoldDuration(new TimeSpan(0, 0, 3));
            int seatHoldStartCount = ticketedEvent.SeatHolds.Count;
            int numAvailableStartCount = ticketedEvent.NumberOfAvailableSeats;

            Console.WriteLine("Creating Seat Hold");
            SeatHold seatHold = ticketService.findAndHoldSeats(10, "myemail@domain.com");
            System.Timers.Timer seatHoldExpiration = new System.Timers.Timer(SeatHold.ExpiresAfter.TotalMilliseconds + 1);
            seatHoldExpiration.Elapsed += (source, args) =>
            {
                Console.WriteLine("Seat hold expired!");
                Assert.AreEqual(ticketedEvent.NumberOfAvailableSeats, numAvailableStartCount);
                Assert.AreEqual(ticketedEvent.SeatHolds.Count, seatHoldStartCount);
                seatHoldExpiration.Stop();
            };
            seatHoldExpiration.Start();
            Assert.AreEqual(ticketedEvent.SeatHolds.Count, seatHoldStartCount + 1);
            Assert.AreEqual(seatHold.CustomerEmail, "myemail@domain.com");
            Assert.AreEqual(seatHold.Seats.Count, 10);
            Assert.AreEqual(ticketedEvent.NumberOfAvailableSeats, numAvailableStartCount - 10);

            try
            {
                ticketService.findAndHoldSeats(0, "myemail@domain.com");
                Assert.Fail("find and hold seats failed to throw exception from passing 0 for numseats");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("Cannot hold 0 or less seats."));
            }

            try
            {
                ticketService.findAndHoldSeats(1, "invalidemailaddress");
                Assert.Fail("find and hold seats failed to throw exception after invalid email address passed");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("Invalid email address. "));
            }

            Console.WriteLine("Waiting for seat holds to expire");

              // wait for seat holds to expire. I'd want to use something other than thread sleep in production, but this works for now.
            Thread.Sleep((int)SeatHold.ExpiresAfter.TotalMilliseconds + 100);

             // reset seat hold expiration
            SeatHold.SetSeatHoldDuration(seatHoldExpiresAfter);
        }

        public static void TestReserveSeats()
        {
            if (ticketService == null || ticketedEvent == null)
            {
                Console.WriteLine("there is no ticket service. Run ts create or test case 1");
            }
            int reservationStartCount = ticketedEvent.Reservations.Count;
            int availableSeatStartCount = ticketedEvent.NumberOfAvailableSeats;
            int seatHoldStartCount = ticketedEvent.SeatHolds.Count;

            TimeSpan seatHoldExpiresAfter = SeatHold.ExpiresAfter;

            SeatHold.SetSeatHoldDuration(new TimeSpan(0, 0, 3));

            Console.WriteLine("Creating seat hold");
            SeatHold seatHold = ticketService.findAndHoldSeats(10, "myemail@domain.com");

            Console.WriteLine("Creating reservation");

            string confirmationCode = ticketService.reserveSeats(seatHold.Id, seatHold.CustomerEmail);

            Assert.IsTrue(!string.IsNullOrEmpty(confirmationCode));
            Assert.AreEqual(ticketedEvent.SeatHolds.Count, seatHoldStartCount);
            Assert.AreEqual(ticketedEvent.Reservations.Count, reservationStartCount + 1);
            Assert.AreEqual(ticketedEvent.NumberOfAvailableSeats, availableSeatStartCount - 10);
            Assert.IsTrue(ticketedEvent.Reservations.Contains((confirmationCode, seatHold)));

              // wait for seat holds to expire. I'd want to use something other than thread sleep in production, but this works for now.
            Thread.Sleep((int)SeatHold.ExpiresAfter.TotalMilliseconds + 100);

            // reset seat hold expiration
            SeatHold.SetSeatHoldDuration(seatHoldExpiresAfter);
        }
    }
}
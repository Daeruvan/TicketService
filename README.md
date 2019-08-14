# TicketService
 Ticket Service Coding Challenge 
 
# Assumptions
For this coding challenge, I assumed that given the criteria that our ticket service is serving a single ticketed event, and that the seating arrangement is a
simple 2d matrix. 

For the sake of this assignment, while it would go beyond the scope of what is required to build a database and api, I created a framework that closely mimics what you would 
see from a system that uses a web service and a database context. The responsibilities are well seperated, and the ticket service is very loosely coupled with the ticketed event.

I noticed that there was an interest in testing, so I made sure my code was well handled, that input was checked on the client end (the ticket service) and the back end (the ticketed event and data controller).



# How to Run Tests

The console program is complete with its own instructions. Just run command "help".

To run tests, run command: "ts test"  (without quotes)

# Manual testing
To setup the environment, run command: "ts create"   (optional params: [ticketServiceName = \"Ultimate-Code-Warrior-LIVE-@-Showbox\"] [year = 2019] [month = 9] [day = 17] [hour = 6] [min = 30] [rows = 20'] [cols = 15])

To print the ticketed event status, run command "ts print"

To run ticket service:

ts numavailableseats                    Shows the number of available seats
ts findandholdseats [numSeats] [email]  Creates a seat hold.                     Example: ts findandholdseats 7 a@a.com
ts reserveseats [seatHoldId] [email]    Creates a reservation from a seat hold.  Example: ts reserveseats 0 a@a.com
  

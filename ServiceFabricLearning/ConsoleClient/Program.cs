// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : ConsoleClient
// File             : Program.cs
// Created          : 2017-01-18  7:23 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using BookingManager.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace ConsoleClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IBookingManager bookingManager = ActorProxy.Create<IBookingManager>(new ActorId("1"),
                new Uri("fabric:/TicketDemo/BookingManagerActorService"));

            string key;

            do
            {
                key = Console.ReadKey().KeyChar.ToString().ToLowerInvariant();

                try
                {
                    int n = bookingManager.GetAvailableTicketsAmountAsync().Result;
                    Console.WriteLine(n);

                    bookingManager.AddAvailableTicketAsync(10).Wait();

                    n = bookingManager.GetAvailableTicketsAmountAsync().Result;
                    Console.WriteLine(n);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (key != "n");
        }
    }
}
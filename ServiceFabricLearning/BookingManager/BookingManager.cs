// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : BookingManager
// File             : BookingManager.cs
// Created          : 2017-01-18  6:45 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System.Threading;
using System.Threading.Tasks;
using BookingManager.Interfaces;
using Credit.Kolibre.Foundation.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace BookingManager
{
    /// <remarks>
    ///     This class represents an actor.
    ///     Every ActorID maps to an instance of this class.
    ///     The StatePersistence attribute determines persistence and replication of actor state:
    ///     - Persisted: State is written to disk and replicated.
    ///     - Volatile: State is kept in memory only and replicated.
    ///     - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.None)]
    internal class BookingManager : StatefulActor<BookingManagerState>, IBookingManager
    {
        /// <summary>
        ///     Initializes a new instance of BookingManager
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public BookingManager(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        Task IBookingManager.AddAvailableTicketAsync(int number, CancellationToken cancellationToken)
        {
            State.RemainingTicketsAmount += number;

            return Done;
        }

        Task<int> IBookingManager.GetAvailableTicketsAmountAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(State.RemainingTicketsAmount);
        }

        /// <summary>
        ///     This method is called whenever an actor is activated.
        ///     An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // 继承自 StatefulActor 的 Actor 会在 OnActivate 的过程中，初始化 State
            await base.OnActivateAsync();
        }
    }
}
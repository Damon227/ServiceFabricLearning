// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterActor
// File             : CounterActor.cs
// Created          : 2017-02-14  2:36 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Threading.Tasks;
using CounterActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;

namespace CounterActor
{
    /// <remarks>
    ///     This class represents an actor.
    ///     Every ActorID maps to an instance of this class.
    ///     The StatePersistence attribute determines persistence and replication of actor state:
    ///     - Persisted: State is written to disk and replicated.
    ///     - Volatile: State is kept in memory only and replicated.
    ///     - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class CounterActor : Actor, ICounterActor
    {
        //private IActorTimer _updateTimer;

        /// <summary>
        ///     Initializes a new instance of CounterActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public CounterActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task<string> CountAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));

            ConditionalValue<int> counter = await StateManager.TryGetStateAsync<int>("Counter");
            int number = counter.HasValue ? counter.Value : 0;

            number++;

            await StateManager.SetStateAsync("Counter", number);

            return $"Current number is {number}, from actor {Id} and partition {ActorService.Context.PartitionId}, replica {ActorService.Context.ReplicaId}";
        }

        public Task ResetAsync()
        {
            return StateManager.SetStateAsync("Counter", 0);
        }

        /// <summary>
        ///     This method is called whenever an actor is activated.
        ///     An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            //_updateTimer = RegisterTimer(
            //AutoCountCallbackAsync,                     // Callback method
            //5,                                          // Parameter to pass to the callback method
            //TimeSpan.FromSeconds(5),                    // Amount of time to delay before the callback is invoked
            //TimeSpan.FromSeconds(5));                   // Time interval between invocations of the callback method

            return Task.FromResult(0);
        }

        private Task AutoCountCallbackAsync(object step)
        {
            int number = 0;
            if (step is int)
            {
                number = (int)step;
            }

            return StateManager.AddOrUpdateStateAsync("Counter", 0, (s, i) => i + number);
        }
    }
}
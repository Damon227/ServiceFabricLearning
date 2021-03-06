﻿// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStateful
// File             : CounterStateful.cs
// Created          : 2017-02-14  10:09 AM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using CounterStateful.Interfaces;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace CounterStateful
{
    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class CounterStateful : StatefulService, ICounterService
    {
        public CounterStateful(StatefulServiceContext context)
            : base(context)
        {
        }

        public async Task<string> CountAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));

            IReliableDictionary<string, int> states = await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("states");
            IReliableQueue<string> events = await StateManager.GetOrAddAsync<IReliableQueue<string>>("events");
            int number;

            //using (ITransaction transaction = StateManager.CreateTransaction())
            //{
            //    ConditionalValue<int> counter = await states.TryGetValueAsync(transaction, "Counter");
            //    number = counter.HasValue ? counter.Value : 0;
            //    number++;
            //}

            //await Task.Delay(TimeSpan.FromSeconds(3));

            //using (ITransaction transaction = StateManager.CreateTransaction())
            //{
            //    await states.SetAsync(transaction, "Counter", number);
            //    await events.EnqueueAsync(transaction, $"{DateTime.UtcNow:O} The Counter is {number}.");

            //    await transaction.CommitAsync();
            //}

            using (ITransaction transaction = StateManager.CreateTransaction())
            {
                ConditionalValue<int> counter = await states.TryGetValueAsync(transaction, "Counter");
                number = counter.HasValue ? counter.Value : 0;
                number++;

                await Task.Delay(TimeSpan.FromSeconds(3));

                await states.SetAsync(transaction, "Counter", number);
                await events.EnqueueAsync(transaction, $"{DateTime.UtcNow:O} The Counter is {number}.");

                await transaction.CommitAsync();
            }

            return $"Current number is {number}, from partition {Context.PartitionId} and replica {Context.ReplicaId}";
        }

        public async Task ResetAsync()
        {
            IReliableDictionary<string, int> states = await StateManager.GetOrAddAsync<IReliableDictionary<string, int>>("states");
            IReliableQueue<string> events = await StateManager.GetOrAddAsync<IReliableQueue<string>>("events");

            using (ITransaction transaction = StateManager.CreateTransaction())
            {
                await states.SetAsync(transaction, "Counter", 0);
                await events.EnqueueAsync(transaction, $"{DateTime.UtcNow:O} The Counter is reset.");

                await transaction.CommitAsync();
            }
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        ///     For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[]
            {
                new ServiceReplicaListener(context =>
                    new FabricTransportServiceRemotingListener(context, this)) // 注意第一个参数不要传入 this
            };
        }

        /// <summary>
        ///     This is the main entry point for your service replica.
        ///     This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
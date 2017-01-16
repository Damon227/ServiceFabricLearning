// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStateless
// File             : CounterStateless.cs
// Created          : 2017-02-03  10:49 AM
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
using CounterStateless.Interfaces;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace CounterStateless
{
    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class CounterStateless : StatelessService, ICounterService
    {
        private int _number;

        public CounterStateless(StatelessServiceContext context)
            : base(context)
        {
        }

        public async Task<string> CountAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            _number++;
            return $"Current number is {_number}, from instance {Context.InstanceId}";
        }

        public Task ResetAsync()
        {
            _number = 0;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new[]
            {
                new ServiceInstanceListener(context =>
                    new FabricTransportServiceRemotingListener(context, this)) // 注意第一个参数不要传入 this
            };
        }

        /// <summary>
        ///     This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
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
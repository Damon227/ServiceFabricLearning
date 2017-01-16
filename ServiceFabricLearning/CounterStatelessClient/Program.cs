// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStatelessClient
// File             : Program.cs
// Created          : 2017-02-04  4:18 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Diagnostics.CodeAnalysis;
using CounterStateless.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CounterStatelessClient
{
    internal class Program
    {
        // Service Fabric 的客户端必须使用X64模式编译
        // Service Fabric 暂时对.net Core 支持比较弱

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        private static void Main(string[] args)
        {
            // 1. 引用 Interfaces 类库和 Service Fabric 类库
            // 2. 创建客户端，这里使用的是非安全的方式，在网络隔离的情况下，
            //    客户端和服务端可以使用非安全方式通信

            ICounterService counterService = ServiceProxy.Create<ICounterService>(
                new Uri("fabric:/CounterDemo/CounterStateless"));


            //FabricTransportSettings fabricTransportSettings = new FabricTransportSettings
            //{
            //    // 仅是设置单次的重试时间，如果设置为 2 秒，可以重现错误，抛出 System.TimeoutException
            //    OperationTimeout = TimeSpan.FromMinutes(5)
            //};

            //// 这样设置重试的次数，从而控制整体超时时间
            //// 部分内置错误会无限重试，参考 IExceptionHandler
            //// 如果遇到错误，会再重试 defaultMaxRetryCount 次
            //OperationRetrySettings operationRetrySettings = new OperationRetrySettings(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), 3);

            ////ServiceProxyFactory serviceProxyFactory = new ServiceProxyFactory(c =>
            ////    new FabricTransportServiceRemotingClientFactory(fabricTransportSettings), operationRetrySettings);

            //ServiceProxyFactory serviceProxyFactory = new ServiceProxyFactory(c =>
            //    new FabricTransportServiceRemotingClientFactory(fabricTransportSettings, null, null, new[] { new MyExceptionHandler() }), operationRetrySettings);

            //ICounterService counterService = serviceProxyFactory.CreateServiceProxy<ICounterService>(
            //    new Uri("fabric:/CounterDemo/CounterStateless"));

            counterService.ResetAsync().Wait();

            do
            {
                try
                {
                    Console.WriteLine(counterService.CountAsync().Result);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (true);
        }
    }
}
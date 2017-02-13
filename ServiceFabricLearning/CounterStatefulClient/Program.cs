// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStatefulClient
// File             : Program.cs
// Created          : 2017-02-08  2:29 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Diagnostics.CodeAnalysis;
using CounterStateful.Interfaces;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace CounterStatefulClient
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

            string command = Console.ReadKey().KeyChar.ToString().ToLowerInvariant();
            Console.WriteLine();

            if (command == "0")
            {
                ICounterService counterService = ServiceProxy.Create<ICounterService>(
                    new Uri("fabric:/CounterDemo/CounterStateful"), ServicePartitionKeyFactory.Build(0));

                counterService.ResetAsync().Wait();

                ICounterService shService = ServiceProxy.Create<ICounterService>(
                    new Uri("fabric:/CounterDemo/CounterStateful"), ServicePartitionKeyFactory.Build("Shanghai"));

                ICounterService bjService = ServiceProxy.Create<ICounterService>(
                    new Uri("fabric:/CounterDemo/CounterStateful"), ServicePartitionKeyFactory.Build("Beijing"));

                shService.ResetAsync().Wait();
                bjService.ResetAsync().Wait();
            }

            if (command == "1")
            {
                ICounterService counterService = ServiceProxy.Create<ICounterService>(
                    new Uri("fabric:/CounterDemo/CounterStateful"), ServicePartitionKeyFactory.Build(0));

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

            if (command == "2")
            {
                ICounterService shService = ServiceProxy.Create<ICounterService>(
                    new Uri("fabric:/CounterDemo/CounterStateful"), ServicePartitionKeyFactory.Build("Shanghai"));

                ICounterService bjService = ServiceProxy.Create<ICounterService>(
                    new Uri("fabric:/CounterDemo/CounterStateful"), ServicePartitionKeyFactory.Build("Beijing"));

                do
                {
                    try
                    {
                        Console.WriteLine(shService.CountAsync().Result);
                        Console.WriteLine(shService.CountAsync().Result);
                        Console.WriteLine(bjService.CountAsync().Result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                } while (true);
            }
        }
    }
}
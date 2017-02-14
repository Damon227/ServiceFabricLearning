// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterActorClient
// File             : Program.cs
// Created          : 2017-02-14  3:31 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Diagnostics.CodeAnalysis;
using CounterActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace CounterActorClient
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
                Guid id = Guid.Parse("18B2A607-194A-47CE-BD62-881D196F6DA9");
                ICounterActor counterActor = ActorProxy.Create<ICounterActor>(new ActorId(id), new Uri("fabric:/CounterDemo/CounterActorService"));

                counterActor.ResetAsync().Wait();
            }

            if (command == "1")
            {
                Guid id = Guid.Parse("18B2A607-194A-47CE-BD62-881D196F6DA9");
                ICounterActor counterActor = ActorProxy.Create<ICounterActor>(new ActorId(id), new Uri("fabric:/CounterDemo/CounterActorService"));

                do
                {
                    try
                    {
                        Console.WriteLine(counterActor.CountAsync().Result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                } while (true);
            }

            if (command == "2")
            {
                ICounterActor counterActor = ActorProxy.Create<ICounterActor>(ActorId.CreateRandom(), new Uri("fabric:/CounterDemo/CounterActorService"));

                do
                {
                    try
                    {
                        Console.WriteLine(counterActor.CountAsync().Result);
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
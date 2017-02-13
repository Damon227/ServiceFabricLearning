// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStatefulClient
// File             : ServicePartitionKeyFactory.cs
// Created          : 2017-02-10  11:34 AM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Text;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Client;

namespace CounterStatefulClient
{
    internal class ServicePartitionKeyFactory
    {
        private static readonly Random s_rand = new Random();
        private static readonly object s_randLock = new object();

        internal static ServicePartitionKey Build(string id)
        {
            return new ServicePartitionKey((long)Crc64.ToCrc64(Encoding.UTF8.GetBytes(id)));
        }

        internal static ServicePartitionKey Build(int id)
        {
            return new ServicePartitionKey(id);
        }

        internal static ServicePartitionKey Build(long id)
        {
            return new ServicePartitionKey(id);
        }

        internal static ServicePartitionKey CreateRandom()
        {
            byte[] buffer = new byte[8];
            lock (s_randLock)
                s_rand.NextBytes(buffer);
            return new ServicePartitionKey(BitConverter.ToInt64(buffer, 0));
        }
    }
}
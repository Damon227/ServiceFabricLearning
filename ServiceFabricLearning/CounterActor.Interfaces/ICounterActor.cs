// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterActor.Interfaces
// File             : ICounterActor.cs
// Created          : 2017-02-14  2:36 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace CounterActor.Interfaces
{
    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface ICounterActor : IActor
    {
        Task<string> CountAsync();

        Task ResetAsync();
    }
}
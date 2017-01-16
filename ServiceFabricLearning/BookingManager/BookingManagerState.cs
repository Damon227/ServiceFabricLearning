// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : BookingManager
// File             : BookingManagerState.cs
// Created          : 2017-01-17  4:30 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using Credit.Kolibre.Foundation.ServiceFabric.Actors;

namespace BookingManager
{
    public class BookingManagerState : IActorState
    {
        public int RemainingTicketsAmount { get; set; }
    }
}
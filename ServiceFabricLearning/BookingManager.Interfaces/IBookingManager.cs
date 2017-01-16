// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : BookingManager.Interfaces
// File             : IBookingManager.cs
// Created          : 2017-01-17  4:28 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace BookingManager.Interfaces
{
    /// <summary>
    ///     Ticket 的预订管理器。
    /// </summary>
    public interface IBookingManager : IActor
    {
        /// <summary>
        ///     增加指定数量的可用票。
        /// </summary>
        /// <param name="number">指定的数量，不能为负值。</param>
        /// <param name="cancellationToken">default(CancellationToken)</param>
        Task AddAvailableTicketAsync(int number, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     获取可用的剩余票量。
        /// </summary>
        Task<int> GetAvailableTicketsAmountAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStateless.Interfaces
// File             : ICounterService.cs
// Created          : 2017-02-04  4:05 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CounterStateless.Interfaces
{
    public interface ICounterService : IService
    {
        Task<string> CountAsync();

        Task ResetAsync();
    }
}
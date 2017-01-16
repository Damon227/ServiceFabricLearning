// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStateful.Interfaces
// File             : Class1.cs
// Created          : 2017-02-07  5:52 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace CounterStateful.Interfaces
{
    public interface ICounterService : IService
    {
        Task<string> CountAsync();

        Task ResetAsync();
    }
}
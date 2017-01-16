// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStatelessClient
// File             : MyExceptionHandler.cs
// Created          : 2017-02-06  5:22 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace CounterStatelessClient
{
    internal class MyExceptionHandler : IExceptionHandler
    {
        public bool TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            if (exceptionInformation.Exception is FabricCannotConnectException)
            {
                // <param name="isTransient">
                // Indicates if this is a transient retriable exception.
                // Transient retriable exceptions are those where the communication channel from client
                // to service still exists.
                // Non transient retriable exceptions are those where we need to re-resolve the service endpoint
                // before we retry.
                // </param>
                result = new ExceptionHandlingRetryResult(exceptionInformation.Exception, false, retrySettings, retrySettings.DefaultMaxRetryCount);
                return true;
            }
            result = null;
            return false;
        }
    }
}
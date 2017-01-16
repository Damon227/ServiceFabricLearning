// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStatelessClient
// File             : InternalExceptionHandler.cs
// Created          : 2017-02-07  10:05 AM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace CounterStatelessClient.ExceptionHandler
{
    internal class InternalExceptionHandler : IExceptionHandler
    {
        bool IExceptionHandler.TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            FabricException exception = exceptionInformation.Exception as FabricException;
            if (exception != null)
                return TryHandleFabricException(exception, retrySettings, out result);
            result = null;
            return false;
        }

        private static bool TryHandleFabricException(FabricException fabricException, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            if (fabricException is FabricCannotConnectException || fabricException is FabricEndpointNotFoundException)
            {
                result = new ExceptionHandlingRetryResult(fabricException, false, retrySettings, int.MaxValue);
                return true;
            }
            if (fabricException.ErrorCode.Equals(FabricErrorCode.ServiceTooBusy))
            {
                result = new ExceptionHandlingRetryResult(fabricException, true, retrySettings, int.MaxValue);
                return true;
            }
            result = null;
            return false;
        }
    }
}
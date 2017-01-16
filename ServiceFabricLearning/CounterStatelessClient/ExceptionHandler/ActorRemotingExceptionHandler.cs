﻿// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStatelessClient
// File             : ActorRemotingExceptionHandler.cs
// Created          : 2017-02-06  8:38 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Runtime.Remoting.Messaging;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace CounterStatelessClient.ExceptionHandler
{
    /// <summary>
    ///     This class provide handling of exceptions encountered in communicating with
    ///     service fabric actors over remoted actor interfaces.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This exception handler handles exceptions related to the following scenarios.
    ///     </para>
    ///     <list type="list">
    ///         <item>
    ///             <term>
    ///                 Duplicate Messages:
    ///             </term>
    ///             <description>
    ///                 <para>
    ///                     Operations performed on the actor are retried from the client based on the exception handling logic.
    ///                     These exceptions represent various error condition including service failover. Therefore it is possible
    ///                     for the actors to receive duplicate messages. If a duplicate message is received while previous
    ///                     message is being processed by the actor, runtime return an internal exception to the client.
    ///                     The client then retries the operation to get the result back from the actor. From the actor's
    ///                     perspective duplicate operation will be performed by the clients and it should handle it in the similar
    ///                     manner as if the operation was already processed and then a duplicate message arrived.
    ///                 </para>
    ///                 <para>
    ///                     Exception related to duplicate operation being processed is handled by returning <see cref="T:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult" /> from the
    ///                     <see cref="M:Microsoft.ServiceFabric.Services.Communication.Client.IExceptionHandler.TryHandleException(Microsoft.ServiceFabric.Services.Communication.Client.ExceptionInformation,Microsoft.ServiceFabric.Services.Communication.Client.OperationRetrySettings,Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingResult@)" /> method.
    ///                     The <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.IsTransient" /> property of the <see cref="T:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult" /> is set to true,
    ///                     the <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.RetryDelay" />  property is set to a random value up to <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.OperationRetrySettings.MaxRetryBackoffIntervalOnTransientErrors" />
    ///                     and <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.MaxRetryCount" /> property is set to <see cref="F:System.Int32.MaxValue" />.
    ///                 </para>
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term>
    ///                 <see cref="T:Microsoft.ServiceFabric.Actors.ActorConcurrencyLockTimeoutException" />:
    ///             </term>
    ///             <description>
    ///                 <para>
    ///                     Operations on the actors are performed using a turn based concurrency lock (<see cref="T:Microsoft.ServiceFabric.Actors.Runtime.ActorConcurrencySettings" />)
    ///                     that supports logical call context based reentrancy. In case of the long running actor operations it is
    ///                     possible for acquisition of this lock to time out. The acquisition of the lock can also time out in case of the deadlock
    ///                     situations (actor A and actor B calling each other almost at the same time).
    ///                 </para>
    ///                 <para>
    ///                     The exception related to concurrency lock timeout is handled by returning <see cref="T:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult" /> from the
    ///                     <see cref="M:Microsoft.ServiceFabric.Services.Communication.Client.IExceptionHandler.TryHandleException(Microsoft.ServiceFabric.Services.Communication.Client.ExceptionInformation,Microsoft.ServiceFabric.Services.Communication.Client.OperationRetrySettings,Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingResult@)" /> method
    ///                     if the client performing the operation is not another actor.
    ///                     The <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.IsTransient" /> property of the <see cref="T:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult" /> is set to true,
    ///                     the <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.RetryDelay" />  property is set to a random value up to <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.OperationRetrySettings.MaxRetryBackoffIntervalOnTransientErrors" />
    ///                     and <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.MaxRetryCount" /> property is set to <see cref="F:System.Int32.MaxValue" />.
    ///                 </para>
    ///                 <para>
    ///                     The exception related to concurrency lock timeout is handled by returning <see cref="T:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingThrowResult" /> from the
    ///                     <see cref="M:Microsoft.ServiceFabric.Services.Communication.Client.IExceptionHandler.TryHandleException(Microsoft.ServiceFabric.Services.Communication.Client.ExceptionInformation,Microsoft.ServiceFabric.Services.Communication.Client.OperationRetrySettings,Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingResult@)" /> method,
    ///                     if the client performing the operation is another actor. In the deadlock situations this allows the call chain to unwind all the way
    ///                     back to the original client and the operation is then retried from there.
    ///                 </para>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class ActorRemotingExceptionHandler : IExceptionHandler
    {
        bool IExceptionHandler.TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            Exception exception = exceptionInformation.Exception;
            if (exception is ActorConcurrencyLockTimeoutException)
            {
                if (ActorLogicalCallContext.IsPresent())
                {
                    result = new ExceptionHandlingThrowResult
                    {
                        ExceptionToThrow = exception
                    };
                    return true;
                }
                result = new ExceptionHandlingRetryResult(exception, true, retrySettings, retrySettings.DefaultMaxRetryCount);
                return true;
            }
            result = null;
            return false;
        }
    }

    internal static class ActorLogicalCallContext
    {
        public static bool IsPresent()
        {
            return CallContext.LogicalGetData("_FabActCallContext_") != null;
        }
    }
}
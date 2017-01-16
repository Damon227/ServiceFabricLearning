// ***********************************************************************
// Solution         : ServiceFabricLearning
// Project          : CounterStatelessClient
// File             : ServiceRemotingExceptionHandler.cs
// Created          : 2017-02-06  8:36 PM
// ***********************************************************************
// <copyright>
//     Copyright © 2016 Kolibre Credit Team. All rights reserved.
// </copyright>
// ***********************************************************************

using System;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace CounterStatelessClient.ExceptionHandler
{
    /// <summary>
    ///     This class provide handling of exceptions encountered in communicating with
    ///     a service fabric service over remoted interfaces.
    /// </summary>
    /// <remarks>
    ///     The exceptions are handled as per the description below:
    ///     <list type="table">
    ///         <item>
    ///             <description>
    ///                 The following exceptions indicate service failover. These exceptions are handled by returning <see cref="T:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult" /> from the
    ///                 <see cref="M:Microsoft.ServiceFabric.Services.Communication.Client.IExceptionHandler.TryHandleException(Microsoft.ServiceFabric.Services.Communication.Client.ExceptionInformation,Microsoft.ServiceFabric.Services.Communication.Client.OperationRetrySettings,Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingResult@)" /> method.
    ///                 The <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.IsTransient" /> property of the <see cref="T:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult" /> is set to false,
    ///                 the <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.RetryDelay" />  property is set to a random value up to <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.OperationRetrySettings.MaxRetryBackoffIntervalOnNonTransientErrors" />
    ///                 and <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.MaxRetryCount" /> property is set to <see cref="F:System.Int32.MaxValue" />.
    ///                 <list type="bullet">
    ///                     <item>
    ///                         <description><see cref="T:System.Fabric.FabricNotPrimaryException" />, when the target replica is <see cref="F:Microsoft.ServiceFabric.Services.Communication.Client.TargetReplicaSelector.PrimaryReplica" />.</description>
    ///                     </item>
    ///                     <item>
    ///                         <description>
    ///                             <see cref="T:System.Fabric.FabricNotReadableException" />
    ///                         </description>
    ///                     </item>
    ///                 </list>
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 The following exceptions indicate transient error conditions and handled by returning <see cref="T:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult" /> from the
    ///                 <see cref="M:Microsoft.ServiceFabric.Services.Communication.Client.IExceptionHandler.TryHandleException(Microsoft.ServiceFabric.Services.Communication.Client.ExceptionInformation,Microsoft.ServiceFabric.Services.Communication.Client.OperationRetrySettings,Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingResult@)" /> method.
    ///                 The <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.IsTransient" /> property of the <see cref="T:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult" /> is set to true,
    ///                 the <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.RetryDelay" />  property is set to a random value up to <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.OperationRetrySettings.MaxRetryBackoffIntervalOnTransientErrors" />
    ///                 and <see cref="P:Microsoft.ServiceFabric.Services.Communication.Client.ExceptionHandlingRetryResult.MaxRetryCount" /> property is set to <see cref="F:System.Int32.MaxValue" />.
    ///                 <list type="bullet">
    ///                     <item>
    ///                         <description>
    ///                             <see cref="T:System.Fabric.FabricTransientException" />
    ///                         </description>
    ///                     </item>
    ///                 </list>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class ServiceRemotingExceptionHandler : IExceptionHandler
    {
        //private static readonly string s_traceType = "ServiceRemotingExceptionHandler";
        private readonly string _traceId;

        /// <summary>
        ///     Constructs a ServiceRemotingExceptionHandler with a default trace id.
        /// </summary>
        public ServiceRemotingExceptionHandler()
            : this(null)
        {
        }

        /// <summary>
        ///     Constructs a ServiceRemotingExceptionHandler with a specified trace id.
        /// </summary>
        /// <param name="traceId">
        ///     Id to use in diagnostics traces from this component.
        /// </param>
        public ServiceRemotingExceptionHandler(string traceId)
        {
            _traceId = traceId ?? Guid.NewGuid().ToString();
        }

        bool IExceptionHandler.TryHandleException(ExceptionInformation exceptionInformation, OperationRetrySettings retrySettings, out ExceptionHandlingResult result)
        {
            if (exceptionInformation.Exception is FabricNotPrimaryException)
            {
                if (exceptionInformation.TargetReplica == TargetReplicaSelector.Default)
                {
                    result = new ExceptionHandlingRetryResult(exceptionInformation.Exception, false, retrySettings, int.MaxValue);
                    return true;
                }
                //ServiceTrace.Source.WriteInfo(ServiceRemotingExceptionHandler.TraceType, "{0} Got exception {1} which does not match the replica target : {2}", (object) this._traceId, (object) exceptionInformation.Exception, (object) exceptionInformation.TargetReplica);
                result = null;
                return false;
            }
            if (exceptionInformation.Exception is FabricNotReadableException)
            {
                result = new ExceptionHandlingRetryResult(exceptionInformation.Exception, false, retrySettings, int.MaxValue);
                return true;
            }
            if (exceptionInformation.Exception is FabricTransientException)
            {
                result = new ExceptionHandlingRetryResult(exceptionInformation.Exception, true, retrySettings, int.MaxValue);
                return true;
            }
            result = null;
            return false;
        }
    }
}
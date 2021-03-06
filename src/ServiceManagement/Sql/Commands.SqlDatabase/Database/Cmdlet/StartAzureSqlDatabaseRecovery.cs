﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Commands.SqlDatabase.Database.Cmdlet
{
    using Microsoft.WindowsAzure.Commands.Utilities.Common;
    using Services.Common;
    using Services.Server;
    using System;
    using System.Management.Automation;

    /// <summary>
    /// Issues a new recover request for the specified live or dropped Windows Azure SQL Database.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "AzureSqlDatabaseRecovery", ConfirmImpact = ConfirmImpact.Low)]
    public class StartAzureSqlDatabaseRecovery : CmdletBase
    {
        #region Parameter Sets

        /// <summary>
        /// The parameter set string for connecting using azure subscription and providing a source Database object
        /// </summary>
        internal const string BySourceDatabaseObject =
            "BySourceDatabaseObject";

        /// <summary>
        /// The parameter set string for connecting using azure subscription and providing a source database name
        /// </summary>
        internal const string BySourceDatabaseName =
            "BySourceDatabaseName";

        #endregion

        #region Parameters

        /// <summary>
        /// Gets or sets the name of the server that will host the recovered database.
        /// </summary>
        [Parameter(Mandatory = true,
            ParameterSetName = BySourceDatabaseName,
            HelpMessage = "The name of the server that will host the recovered database.")]
        [ValidateNotNullOrEmpty]
        public string TargetServerName { get; set; }

        /// <summary>
        /// Gets or sets the database object representing the database to recover.
        /// </summary>
        [Parameter(Mandatory = true,
            ValueFromPipeline = true,
            ParameterSetName = BySourceDatabaseObject,
            HelpMessage = "The database object representing the database to recover.")]
        [ValidateNotNull]
        public RecoverableDatabase SourceDatabase { get; set; }

        /// <summary>
        /// Gets or sets the name of the server that had the database to recover.
        /// </summary>
        [Parameter(Mandatory = false,
            ParameterSetName = BySourceDatabaseName,
            HelpMessage = "The name of the server that had the database to recover.")]
        [ValidateNotNullOrEmpty]
        public string SourceServerName { get; set; }

        /// <summary>
        /// Gets or sets the name of the database to recover.
        /// </summary>
        [Parameter(Mandatory = true,
            ParameterSetName = BySourceDatabaseName,
            HelpMessage = "The name of the database to recover.")]
        [ValidateNotNullOrEmpty]
        public string SourceDatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the name of the target database.
        /// </summary>
        [Parameter(Mandatory = false,
            HelpMessage = "The name of the target database.")]
        [ValidateNotNullOrEmpty]
        public string TargetDatabaseName { get; set; }

        #endregion

        /// <summary>
        /// Execute the command.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            this.SourceDatabaseName =
                this.SourceDatabase != null ? this.SourceDatabase.Name :
                this.SourceDatabaseName;

            this.SourceServerName =
                this.SourceDatabase != null ? this.SourceDatabase.ServerName :
                this.SourceServerName;

            this.TargetDatabaseName = this.TargetDatabaseName ?? this.SourceDatabaseName;

            // Get the current subscription data
            var subscription = WindowsAzureProfile.Instance.CurrentSubscription;

            IServerDataServiceContext connectionContext = null;

            // If a database object was piped in, use its connection context...
            if (this.SourceDatabase != null)
            {
                connectionContext = this.SourceDatabase.Context;
            }
            else
            {
                // ... else create a temporary context
                connectionContext = ServerDataServiceCertAuth.Create(this.TargetServerName, subscription);
            }

            string clientRequestId = connectionContext.ClientRequestId;

            try
            {
                RecoverDatabaseOperation operation = connectionContext.RecoverDatabase(
                    this.SourceServerName,
                    this.SourceDatabaseName,
                    this.TargetDatabaseName);

                this.WriteObject(operation);
            }
            catch (Exception ex)
            {
                SqlDatabaseExceptionHandler.WriteErrorDetails(
                    this,
                    clientRequestId,
                    ex);
            }
        }
    }
}

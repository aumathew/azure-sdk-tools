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

namespace Microsoft.WindowsAzure.Commands.ServiceManagement.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Management.Compute;
    using Management.Compute.Models;
    using Model.PersistentVMModel;

    /// <summary>
    /// Get Windows Azure Service ADDomain Extension.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, ADDomainExtensionNoun), OutputType(typeof(IEnumerable<ADDomainExtensionContext>))]
    public class GetAzureServiceADDomainExtensionCommand : BaseAzureServiceADDomainExtensionCmdlet
    {
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true, Mandatory = false, HelpMessage = ServiceNameHelpMessage)]
        [ValidateNotNullOrEmpty]
        public override string ServiceName
        {
            get;
            set;
        }

        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true, Mandatory = false, HelpMessage = SlotHelpMessage)]
        [ValidateSet(DeploymentSlotType.Production, DeploymentSlotType.Staging, IgnoreCase = true)]
        public override string Slot
        {
            get;
            set;
        }

        protected override void ValidateParameters()
        {
            base.ValidateParameters();
            ValidateService();
            ValidateDeployment();
        }

        public void ExecuteCommand()
        {
            ValidateParameters();
            ExecuteClientActionNewSM(
                null,
                CommandRuntime.ToString(),
                () => this.ComputeClient.HostedServices.ListExtensions(this.ServiceName),
                (s, r) =>
                {
                    var extensionRoleList = (from dr in Deployment.Roles
                                             select new ExtensionRole(dr.RoleName)).ToList().Union(new ExtensionRole[] { new ExtensionRole() });

                    return from role in extensionRoleList
                           from extension in r.Extensions
                           where ExtensionManager.CheckNameSpaceType(extension, ExtensionNameSpace, ExtensionType)
                              && ExtensionManager.GetBuilder(Deployment.ExtensionConfiguration).Exist(role, extension.Id)
                           select GetContext(s, role, extension) as ADDomainExtensionContext;
                });
        }

        protected override void OnProcessRecord()
        {
            ExecuteCommand();
        }
    }
}

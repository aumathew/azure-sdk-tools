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

namespace Microsoft.WindowsAzure.Commands.ServiceManagement.IaaS.Extensions
{
    using System;
    using System.Linq;
    using System.Management.Automation;

    [Cmdlet(
        VerbsCommon.Get,
        VirtualMachineDiagnosticsExtensionNoun,
        DefaultParameterSetName = GetDiagnosticsExtensionParamSetName),
    OutputType(
        typeof(VirtualMachineDiagnosticsExtensionContext))]
    public class GetAzureVMDiagnosticsExtensionCommand : VirtualMachineDiagnosticsExtensionCmdletBase
    {
        protected const string GetDiagnosticsExtensionParamSetName = "GetDiagnosticsExtension";

        internal void ExecuteCommand()
        {
            var extensionRefs = GetPredicateExtensionList();
            WriteObject(
                extensionRefs == null ? null : extensionRefs.Select(
                r =>
                {
                    GetDiagnosticsAgentValues(r.ResourceExtensionParameterValues);
                    return new VirtualMachineDiagnosticsExtensionContext
                    {
                        ExtensionName = r.Name,
                        Publisher = r.Publisher,
                        ReferenceName = r.ReferenceName,
                        Version = r.Version,
                        State = r.State,
                        Enabled = !Disable,
                    };
                }));
        }

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();
                ExecuteCommand();
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }
    }
}

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
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Model.PersistentVMModel;

    public class VirtualMachineEnableAccessExtensionCmdletBase : VirtualMachineExtensionCmdletBase
    {
        protected const string VirtualMachineEnableAccessExtensionNoun = "AzureVMEnableAccessExtension";

        protected const string ExtensionDefaultPublisher = "Microsoft.Compute";
        protected const string ExtensionDefaultName = "VMAccessAgent";
        protected const string CurrentExtensionVersion = "0.1";

        protected const string ExtensionDefaultReferenceName = "MyPasswordResetExtension";
        protected const string ExtensionReferenceKeyStr = "VMAccessAgentConfigParameter";

        private const string ConfigurationElem = "Configuration";
        private const string EnabledElem = "Enabled";
        private const string PublicElem = "Public";
        private const string PublicConfigElem = "PublicConfig";
        private const string AccountElem = "Account";
        private const string UserNameElem = "UserName";
        private const string PasswordElem = "Password";

        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }

        public VirtualMachineEnableAccessExtensionCmdletBase()
        {
            Publisher = ExtensionDefaultPublisher;
            ExtensionName = ExtensionDefaultName;
        }

        protected string GetEnableVMAccessAgentConfig()
        {
            XDocument config = null;
            if (Disable)
            {
                config = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement(ConfigurationElem,
                        new XElement(EnabledElem, Disable.ToString().ToLower())
                    )
                );
            }
            else
            {
                config = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement(ConfigurationElem,
                        new XElement(EnabledElem, Disable.ToString().ToLower()),
                        new XElement(PublicElem,
                            new XElement(PublicConfigElem,
                                new XElement(AccountElem,
                                    new XElement(UserNameElem, UserName),
                                    new XElement(PasswordElem, Password)
                                )
                            )
                        )
                    )
                );
            }

            return config.ToString();
        }
        protected void GetEnableVMAccessAgentValues(ResourceExtensionParameterValueList paramVals)
        {
            if (paramVals != null && paramVals.Any())
            {
                GetEnableVMAccessAgentValues(paramVals.FirstOrDefault(r => !string.IsNullOrEmpty(r.Value)));
            }
        }

        protected void GetEnableVMAccessAgentValues(ResourceExtensionParameterValue paramVal)
        {
            if (paramVal != null && !string.IsNullOrEmpty(paramVal.Value))
            {
                GetEnableVMAccessAgentValues(paramVal.Value);
            }
        }

        protected void GetEnableVMAccessAgentValues(string config)
        {
            this.Disable = !bool.Parse(GetConfigValue(config, EnabledElem).ToLower());
            this.UserName = GetConfigValue(config, UserNameElem);
            this.Password = GetConfigValue(config, PasswordElem);
        }
    }
}

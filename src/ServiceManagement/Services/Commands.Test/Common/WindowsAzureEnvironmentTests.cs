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

namespace Microsoft.WindowsAzure.Commands.Test.Common
{
    using Commands.Utilities.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class WindowsAzureEnvironmentTests
    {
        [TestMethod]
        public void GetsHttpsEndpointByDefault()
        {
            // Setup
            string accountName = "myaccount";
            string expected = string.Format(
                WindowsAzureEnvironmentConstants.AzureStorageBlobEndpointFormat,
                "https",
                accountName);
            WindowsAzureEnvironment environment = WindowsAzureEnvironment.PublicEnvironments[EnvironmentName.AzureCloud];

            // Test
            Uri actual = environment.GetStorageBlobEndpoint(accountName);

            // Assert
            Assert.AreEqual(expected, actual.ToString());
        }

        [TestMethod]
        public void GetsHttpEndpoint()
        {
            // Setup
            string accountName = "myaccount";
            string expected = string.Format(
                WindowsAzureEnvironmentConstants.AzureStorageBlobEndpointFormat,
                "http",
                accountName);
            WindowsAzureEnvironment environment = WindowsAzureEnvironment.PublicEnvironments[EnvironmentName.AzureCloud];

            // Test
            Uri actual = environment.GetStorageBlobEndpoint(accountName, false);

            // Assert
            Assert.AreEqual(expected, actual.ToString());
        }

        [TestMethod]
        public void DefaultActiveDirectoryResourceUriIsSameWithServiceEndpoint()
        {
            WindowsAzureEnvironment environment = WindowsAzureEnvironment.PublicEnvironments[EnvironmentName.AzureCloud];
            //Assert
            Assert.AreEqual(true,
                environment.ServiceEndpoint == environment.ActiveDirectoryServiceEndpointResourceId);

            //do same test for china cloud
            WindowsAzureEnvironment chinaEnvironment = WindowsAzureEnvironment.PublicEnvironments[EnvironmentName.AzureChinaCloud];
            Assert.AreEqual(true,
                chinaEnvironment.ServiceEndpoint == chinaEnvironment.ActiveDirectoryServiceEndpointResourceId);

            //verify the resource uri are different between 2 environments
            Assert.AreNotEqual(environment.ActiveDirectoryServiceEndpointResourceId,
                chinaEnvironment.ActiveDirectoryServiceEndpointResourceId);

        }
    }
}

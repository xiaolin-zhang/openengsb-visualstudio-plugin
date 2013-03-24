/***
* Licensed to the Austrian Association for Software Tool Integration (AASTI)
* under one or more contributor license agreements. See the NOTICE file
* distributed with this work for additional information regarding copyright
* ownership. The AASTI licenses this file to you under the Apache License,
* Version 2.0 (the "License"); you may not use this file except in compliance
* with the License. You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
***/

// Guids.cs
// MUST match guids.h
using System;

namespace Org.OpenEngSB.Loom.Csharp.VisualStudio.Plugins.Console
{
    static class GuidList
    {
        public const string guidConsolePkgString = "63bff737-79c0-4e7c-9932-54e9dd84914b";
        public const string guidConsoleCmdSetString = "882e9ccf-4370-4e55-a128-22ed31c2a459";
        public const string guidToolWindowPersistanceString = "45ccd161-1bc4-4ab9-b96b-31815f492925";

        public static readonly Guid guidConsoleCmdSet = new Guid(guidConsoleCmdSetString);
    };
}
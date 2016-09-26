﻿//-----------------------------------------------------------------------
// <copyright file="BizTalkFlatFileToFlatFileMapTester.cs">
//     Copyright (c) 2016 Adam Craven. All rights reserved.
// </copyright>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//-----------------------------------------------------------------------

namespace ChannelAdam.TestFramework.BizTalk
{
    using System;
    using System.Reflection;
    using System.Xml.Linq;

    using ChannelAdam.TestFramework.Xml;
    using Logging;
    using Microsoft.XLANGs.BaseTypes;
    using Reflection;
    using Helpers;

    public class BizTalkFlatFileToFlatFileMapTester : XmlMapTesterBase
    {
        #region Constructors

        public BizTalkFlatFileToFlatFileMapTester(ILogAsserter logAsserter) : base(logAsserter)
        {
        }

        public BizTalkFlatFileToFlatFileMapTester(ISimpleLogger logger, ILogAsserter logAsserter) : base(logger, logAsserter)
        {
        }

        #endregion

        #region Properties

        public string InputFlatFileContents { get; private set; }

        public string ActualOutputFlatFileContents { get; private set; }

        #endregion

        #region Public Methods

        #region Arrange Input

        /// <summary>
        /// Arrange the input flat file from an embedded resource in the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains the resource.</param>
        /// <param name="resourceName">The name of the resource.</param>
        public void ArrangeInputFlatFile(Assembly assembly, string resourceName)
        {
            this.ArrangeInputFlatFileContents(EmbeddedResource.GetAsString(assembly, resourceName));
        }

        /// <summary>
        /// Arrange the input flat file from the given string.
        /// </summary>
        /// <param name="value">The string to use as input.</param>
        public void ArrangeInputFlatFileContents(string value)
        {
            Logger.Log();
            Logger.Log($"The input flat file contents for the map is: {Environment.NewLine}{value}");
            this.InputFlatFileContents = value;
        }

        #endregion Arrange Input

        /// <summary>
        /// Tests the map and performs validation on both the input and output XML.
        /// </summary>
        /// <param name="map">The map.</param>
        public void TestMap(TransformBase map)
        {
            TestMap(map, true, true);
        }

        /// <summary>
        /// Tests the map.
        /// </summary>
        /// <param name="map">The map to execute.</param>
        /// <param name="validateInput">if set to <c>true</c> then the input flat file contents and converted XML is validated.</param>
        /// <param name="validateOutput">if set to <c>true</c> then the output is validated.</param>
        public void TestMap(TransformBase map, bool validateInput, bool validateOutput)
        {
            XNode inputXml = BizTalkXmlFlatFileAdapter.ConvertInputFlatFileContentsToXml(map, this.InputFlatFileContents);

            if (validateInput)
            {
                BizTalkXmlMapTestValidator.ValidateInputXml(map, inputXml, this.Logger);
            }

            Logger.Log("Executing the map " + map.GetType().Name);
            string outputXml = BizTalkXmlMapExecutor.PerformTransform(map, inputXml);
            LogAssert.IsTrue("There was output from the map", !string.IsNullOrWhiteSpace(outputXml));

            base.SetActualOutputXmlFromXmlString(outputXml);
            Logger.Log();
            Logger.Log("Map completed");

            if (validateOutput)
            {
                BizTalkXmlMapTestValidator.ValidateOutputXml(map, this.ActualOutputXml, this.Logger);
            }

            var schemaTree = BizTalkMapSchemaUtility.CreateSchemaTreeAndLoadSchema(map, map.TargetSchemas[0]);
            if (!schemaTree.IsStandardXML)
            {
                this.ActualOutputFlatFileContents = BizTalkXmlFlatFileAdapter.ConvertOutputXmlToFlatFileContents(map, this.ActualOutputXml, validateOutput);
                this.Logger.Log();
                this.Logger.Log($"The actual output flat file contents from the map is: {Environment.NewLine}{this.ActualOutputFlatFileContents}");
            }
        }

        #endregion
    }
}
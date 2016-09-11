﻿//-----------------------------------------------------------------------
// <copyright file="XmlMapTester.cs">
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

namespace ChannelAdam.TestFramework.Xml
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;

    using ChannelAdam.Core.Reflection;
    using ChannelAdam.Core.Xml;
    using ChannelAdam.Logging;

    public class XmlMapTester
    {
        #region Fields

        private readonly ISimpleLogger logger;
        private readonly XmlTester xmlTester;

        #endregion

        #region Constructor / Destructor

        protected XmlMapTester(ILogAsserter logAsserter) : this(new SimpleConsoleLogger(), logAsserter)
        {
        }

        protected XmlMapTester(ISimpleLogger logger, ILogAsserter logAsserter)
        {
            this.logger = logger;
            this.xmlTester = new XmlTester(logAsserter);
            this.xmlTester.ActualXmlChangedEvent += this.XmlTester_ActualXmlChangedEvent;
            this.xmlTester.ExpectedXmlChangedEvent += this.XmlTester_ExpectedXmlChangedEvent;
        }

        ~XmlMapTester()
        {
            this.xmlTester.ActualXmlChangedEvent -= this.XmlTester_ActualXmlChangedEvent;
            this.xmlTester.ExpectedXmlChangedEvent -= this.XmlTester_ExpectedXmlChangedEvent;
        }

        #endregion

        #region Properties

        public XElement InputXml { get; private set; }

        public XElement ActualOutputXml
        {
            get
            {
                return this.xmlTester.ActualXml;
            }

            set
            {
                this.xmlTester.ArrangeActualXml(value);
            }
        }

        public XElement ExpectedOutputXml
        {
            get
            {
                return this.xmlTester.ExpectedXml;
            }
        }

        #endregion

        #region Public Methods

        #region Arrange Input XML

        /// <summary>
        /// Arrange the input XML from an embedded resource in the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains the resource.</param>
        /// <param name="resourceName">The name of the resource.</param>
        public void ArrangeInputXml(Assembly assembly, string resourceName)
        {
            this.ArrangeInputXml(EmbeddedResource.GetAsString(assembly, resourceName));
        }

        /// <summary>
        /// Arrange the input XML from the given XElement.
        /// </summary>
        /// <param name="xmlElement">The XElement to use as input.</param>
        public void ArrangeInputXml(XElement xmlElement)
        {
            this.ArrangeInputXml(xmlElement.ToString());       // Clone it...
        }

        /// <summary>
        /// Arrange the input XML by serialising the given object into XML.
        /// </summary>
        /// <param name="valueToSerialise">The object to serialise as input.</param>
        public void ArrangeInputXml(object valueToSerialise)
        {
            this.ArrangeInputXml(valueToSerialise.SerialiseToXml());
        }

        /// <summary>
        /// Arrange the input XML from the given XML string.
        /// </summary>
        /// <param name="xmlString">The xml to use as input.</param>
        public void ArrangeInputXml(string xmlString)
        {
            this.logger.Log();
            this.logger.Log($"The input XML for the map is: {Environment.NewLine}{xmlString}");
            this.InputXml = xmlString.ToXElement();
        }

        #endregion

        #region Arrange Expected Output XML

        /// <summary>
        /// Arrange the expected output XML from an embedded resource in the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains the resource.</param>
        /// <param name="resourceName">The resource name.</param>
        public void ArrangeExpectedOutputXml(Assembly assembly, string resourceName)
        {
            this.xmlTester.ArrangeExpectedXml(assembly, resourceName);
        }

        /// <summary>
        /// Arrange the expected output XML from the given XElement.
        /// </summary>
        /// <param name="xmlElement">The XElement to use as expected output.</param>
        public void ArrangeExpectedOutputXml(XElement xmlElement)
        {
            this.xmlTester.ArrangeExpectedXml(xmlElement);
        }

        /// <summary>
        /// Arrange the expected output XML by serialising the given object into XML.
        /// </summary>
        /// <param name="valueToSerialise">The object to serialise as XML to be used as the expected output.</param>
        public void ArrangeExpectedOutputXml(object valueToSerialise)
        {
            this.xmlTester.ArrangeExpectedXml(valueToSerialise);
        }

        /// <summary>
        /// Arrange the expected output XML from the given XML string.
        /// </summary>
        /// <param name="xmlString">The XML to be used as the expected output.</param>
        public void ArrangeExpectedOutputXml(string xmlString)
        {
            this.xmlTester.ArrangeExpectedXml(xmlString);
        }

        #endregion

        #region Set Actual Output XML

        /// <summary>
        /// Sets the actual output XML from XML file.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        public void SetActualOutputXmlFromXmlFile(string fileName)
        {
            this.SetActualOutputXmlFromXmlString(File.ReadAllText(fileName));
        }

        /// <summary>
        /// Sets the actual output XML from XML string.
        /// </summary>
        /// <param name="xmlString">The XML string.</param>
        public void SetActualOutputXmlFromXmlString(string xmlString)
        {
            this.xmlTester.ArrangeActualXml(xmlString);
        }

        #endregion

        #region Assertions

        /// <summary>
        /// Assert the Actual Output XML against the Expected Output XML.
        /// </summary>
        public void AssertActualOutputXmlEqualsExpectedOutputXml()
        {
            this.xmlTester.AssertActualXmlEqualsExpectedXml();
        }

        #endregion

        #endregion

        #region Private Methods

        private void XmlTester_ExpectedXmlChangedEvent(object sender, XmlChangedEventArgs e)
        {
            this.logger.Log();
            this.logger.Log($"The expected output XML of the map is: {Environment.NewLine}{e.Xml}");
        }

        private void XmlTester_ActualXmlChangedEvent(object sender, XmlChangedEventArgs e)
        {
            this.logger.Log();
            this.logger.Log($"The actual output XML from the map is: {Environment.NewLine}{e.Xml}");
        }

        #endregion
    }
}

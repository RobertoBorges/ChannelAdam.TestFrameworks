﻿//-----------------------------------------------------------------------
// <copyright file="MappingToFlatFileTesterBase.cs">
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

namespace ChannelAdam.TestFramework.Mapping
{
    using System;
    using System.IO;
    using System.Reflection;

    using Abstractions;
    using ChannelAdam.Logging;

    public abstract class MappingToFlatFileTesterBase : IHasExpectedOutputFlatFileContents, IHasActualOutputFlatFileContents
    {
        #region Fields

        private TextTester textTester;

        #endregion

        #region Constructor / Destructor

        protected MappingToFlatFileTesterBase(ILogAsserter logAsserter) : this(new SimpleConsoleLogger(), logAsserter)
        {
        }

        protected MappingToFlatFileTesterBase(ISimpleLogger logger, ILogAsserter logAsserter)
        {
            this.Logger = logger;
            this.LogAssert = logAsserter;

            this.textTester = new TextTester(logAsserter);
            this.textTester.ActualTextChangedEvent += this.TextTester_ActualTextChangedEvent;
            this.textTester.ExpectedTextChangedEvent += this.TextTester_ExpectedTextChangedEvent;
        }

        ~MappingToFlatFileTesterBase()
        {
            this.textTester.ActualTextChangedEvent -= this.TextTester_ActualTextChangedEvent;
            this.textTester.ExpectedTextChangedEvent -= this.TextTester_ExpectedTextChangedEvent;
        }

        #endregion

        #region Properties

        public string ActualOutputFlatFileContents
        {
            get { return this.textTester.ActualText; }

            set { this.textTester.ArrangeActualText(value); }
        }

        public string ExpectedOutputFlatFileContents
        {
            get { return this.textTester.ExpectedText; }
        }

        protected ILogAsserter LogAssert { get; private set; }

        protected ISimpleLogger Logger { get; private set; }

        #endregion

        #region Public Methods

        #region Arrange Expected Output Flat File Contents

        /// <summary>
        /// Arrange the expected output flat file contents from the given string.
        /// </summary>
        /// <param name="flatFileContents">The flat file contents to be used as the expected output.</param>
        public void ArrangeExpectedOutputFlatFileContents(string flatFileContents)
        {
            this.textTester.ArrangeExpectedText(flatFileContents);
        }

        /// <summary>
        /// Arrange the expected output flat file contents from an embedded resource in the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains the resource.</param>
        /// <param name="resourceName">The resource name.</param>
        public void ArrangeExpectedOutputFlatFileContents(Assembly assembly, string resourceName)
        {
            this.textTester.ArrangeExpectedText(assembly, resourceName);
        }

        #endregion

        #region Set Actual Output Flat File Contents

        /// <summary>
        /// Sets the actual output flat file contents from the contents of the given file.
        /// </summary>
        /// <param name="fileName">The filename.</param>
        public void SetActualOutputFlatFileContentsFromFile(string fileName)
        {
            this.SetActualOutputFlatFileContentsFromString(File.ReadAllText(fileName));
        }

        public void SetActualOutputFlatFileContentsFromString(string flatFileContents)
        {
            this.textTester.ArrangeActualText(flatFileContents);
        }

        #endregion

        #region Assertions

        /// <summary>
        /// Assert the Actual Output flat file contents against the Expected Output flat file contents.
        /// </summary>
        public void AssertActualOutputFlatFileContentsEqualsExpectedOutputFlatFileContents()
        {
            this.textTester.AssertActualTextEqualsExpectedText();
        }

        #endregion

        #endregion

        #region Private Methods

        private void TextTester_ActualTextChangedEvent(object sender, Text.TextChangedEventArgs e)
        {
            this.Logger.Log();
            this.Logger.Log($"The actual output flat file contents from the map is: {Environment.NewLine}{e.Text}");
        }

        private void TextTester_ExpectedTextChangedEvent(object sender, Text.TextChangedEventArgs e)
        {
            this.Logger.Log();
            this.Logger.Log($"The expected output flat file contents of the map is: {Environment.NewLine}{e.Text}");
        }

        #endregion
    }
}

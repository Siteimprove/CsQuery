using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;
using CsQuery.Tests;
using CsQuery.Utility;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

namespace CsQuery.Tests
{
    [SetUpFixture,TestClass]
    public class TestAssemblyConfig
    {
        [OneTimeSetUp]
        public static void AssemblySetup()
        {
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            string solutionFolderTry;
            bool isMSTest = Support.TryGetFilePath("./TestResults/", out solutionFolderTry);

            if (!isMSTest)
            {
                solutionFolderTry = Support.GetFilePath("./CsQuery.Tests/");
            }

            CsQueryTest.SolutionDirectory = Support.CleanFilePath(solutionFolderTry+"/../");
        }
        
        [OneTimeTearDown]
        public static void AssemblyTeardown()
        {
            
        }

        /// <summary>
        /// Set up this test run - configuration of the file name is done in the static constructor so
        /// it's not starting a new file for each test fixture.
        /// </summary>

        [AssemblyInitialize]
        public static void SetupTestRun(TestContext context)
        {
            AssemblySetup();

        }

        [AssemblyCleanup]
        public static void CleanupTestRun()
        {
            AssemblyTeardown();
        }

    }
}

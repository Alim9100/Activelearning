// <copyright file="PexAssemblyInfo.cs">Copyright ©  2019</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("Suttest")]
[assembly: PexInstrumentAssembly("PostSharp.Patterns.Diagnostics.Backends.Log4Net")]
[assembly: PexInstrumentAssembly("PostSharp.Patterns.Common")]
[assembly: PexInstrumentAssembly("System.Core")]
[assembly: PexInstrumentAssembly("log4net")]
[assembly: PexInstrumentAssembly("System.Data")]
[assembly: PexInstrumentAssembly("PostSharp.Patterns.Diagnostics")]
[assembly: PexInstrumentAssembly("PostSharp")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "PostSharp.Patterns.Diagnostics.Backends.Log4Net")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "PostSharp.Patterns.Common")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Core")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "log4net")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "System.Data")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "PostSharp.Patterns.Diagnostics")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "PostSharp")]
[assembly: PexInstrumentAssembly("Suttest.Tests")]
[assembly: PexInstrumentAssembly("Suttest.Tests")]

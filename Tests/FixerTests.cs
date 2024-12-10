using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Analyzers;
using Fixers;
using Microsoft.CodeAnalysis.CSharp;

namespace Tests;

public class FixerTests
{
    [Fact]
    public async Task AddProjectProperty()
    {
        var test = new CSharpCodeFixTest<Analyzer, Fixer, DefaultVerifier>
        {
            CodeFixTestBehaviors = CodeFixTestBehaviors.SkipLocalDiagnosticCheck,
            SolutionTransforms =
            {
                (solution, projectId) =>
                {
                    var project = solution.GetProject(projectId)!;
                    var parseOptions = ((CSharpParseOptions)project.ParseOptions!).WithLanguageVersion(LanguageVersion.CSharp12);
                    return project.WithParseOptions(parseOptions).Solution;
                },
            },
            TestCode =
                """
                public class {|#0:User|} { }
                """,
            FixedCode =
                """
                public class User { }
                """,
        };

        test.TestState.AdditionalFiles.Add(("Test.csproj",
            """"
            <Project>
              <PropertyGroup>
              </PropertyGroup>
            </Project>
            """"));

        test.ExpectedDiagnostics.Add(new DiagnosticResult(Analyzer.MustHaveProjectProperty).WithLocation(0));

        test.FixedState.AdditionalFiles.Add(("Test.csproj",
            """"
            <Project>
              <PropertyGroup>
                <Foo>Bar</Foo>
              </PropertyGroup>
            </Project>
            """"));

        await test.RunAsync();
    }
}

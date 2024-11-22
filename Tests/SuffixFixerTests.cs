using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Analyzers;
using Fixers;

namespace Tests;

public class SuffixFixerTests
{
    [Fact]
    public async Task AddSuffix()
    {
        var test = new CSharpCodeFixTest<SuffixAnalyzer, SuffixFixer, DefaultVerifier>
        {
            TestCode =
            """
            public class {|#0:User|} { }
            """,
            FixedCode =
            """
            public class User1 { }
            """,
        };

        test.ExpectedDiagnostics.Add(new DiagnosticResult(SuffixAnalyzer.CanAddSuffix).WithLocation(0));

        await test.RunAsync();
    }
}

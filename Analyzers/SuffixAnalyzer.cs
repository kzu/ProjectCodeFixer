using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SuffixAnalyzer : DiagnosticAnalyzer
{
    public static DiagnosticDescriptor CanAddSuffix { get; } = new(
        "PCF010",
        "I can add a suffix",
        "I can add a suffix",
        "Build",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(CanAddSuffix);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        if (!context.Symbol.Name.EndsWith("1", StringComparison.Ordinal))
            context.ReportDiagnostic(Diagnostic.Create(CanAddSuffix, context.Symbol.Locations.First()));
    }
}

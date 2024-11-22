using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class Analyzer : DiagnosticAnalyzer
{
    public static DiagnosticDescriptor MustHaveProjectProperty { get; } = new(
        "PCF001",
        "Some arbitrary property must be set",
        "Some arbitrary property is required in project.",
        "Build",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor ProjectFileNotFound { get; } = new(
        "PCF002",
        "Project file must be provided to analyzer",
        "Project file must be provided to analyzer",
        "Build",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(MustHaveProjectProperty, ProjectFileNotFound);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.RegisterAdditionalFileAction(context =>
        {
            // from the additionalfiles, get the one with .csproj extension, load it as XDocument 
            // and report a diagnostic if it doesn't have a Foo property declared with any value
            if (!context.AdditionalFile.Path.EndsWith(".csproj"))
            {
                context.ReportDiagnostic(Diagnostic.Create(ProjectFileNotFound, Location.None));
                return;
            }

            var xml = context.AdditionalFile.GetText(context.CancellationToken).ToString();
            var doc = System.Xml.Linq.XDocument.Parse(xml, System.Xml.Linq.LoadOptions.SetLineInfo);
            var foo = doc.Descendants().FirstOrDefault(e => e.Name.LocalName == "Foo");
            if (foo == null || string.IsNullOrWhiteSpace(foo.Value))
            {
                //context.ReportDiagnostic(Diagnostic.Create(MustHaveProjectProperty, Location.Create(
                //    context.AdditionalFile.Path, new Microsoft.CodeAnalysis.Text.TextSpan(), new Microsoft.CodeAnalysis.Text.LinePositionSpan())));

                // report it in whatever the first type declaration is
                var declaration = context.Compilation.SyntaxTrees
                    .SelectMany(x => x.GetRoot().DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax>())
                    .FirstOrDefault();

                if (declaration != null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(MustHaveProjectProperty, declaration.Identifier.GetLocation()));
                }
            }
        });
    }
}
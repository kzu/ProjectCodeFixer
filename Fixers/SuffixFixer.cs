using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Fixers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp)]
public class SuffixFixer : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("PCF010");
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        // Register a code action that adds "1" as a suffix to the type declaration in the diagnostic
        context.RegisterCodeFix(CodeAction.Create(
            "Add suffix",
            c => AddSuffixAsync(context.Document, context.Span, c),
            nameof(SuffixFixer)), context.Diagnostics);

        return Task.CompletedTask;
    }

    // Action that adds "1" as a suffix to the type declaration in the diagnostic
    private static async Task<Document> AddSuffixAsync(Document document, TextSpan span, CancellationToken c)
    {
        var root = await document.GetSyntaxRootAsync(c);
        var node = root.FindNode(span) as TypeDeclarationSyntax;
        if (node == null)
            return document;

        var semanticModel = await document.GetSemanticModelAsync(c);

        var symbol = semanticModel.GetDeclaredSymbol(node, c);
        if (symbol == null)
            return document;

        var newRoot = root.ReplaceNode(node, node.WithIdentifier(SyntaxFactory.Identifier(symbol.Name + "1")));

        return document.WithSyntaxRoot(newRoot);
    }
}

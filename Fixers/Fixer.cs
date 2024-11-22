using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis;
using System.Composition;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CodeActions;
using System.IO;
using System;

namespace Fixers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp)]
public class Fixer : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create("PCF001");
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        context.RegisterCodeFix(CodeAction.Create(
            "Add property to project",
            c => CreateChangedSolutionAsync(context, c),
            nameof(Fixer)), context.Diagnostics);

        File.AppendAllLines(Environment.ExpandEnvironmentVariables("%TEMP%\\ProjectCodeFixer.log"), ["RegisterCodeFixesAsync"]);

        return Task.CompletedTask;
    }

    private async Task<Solution> CreateChangedSolutionAsync(CodeFixContext context, CancellationToken cancellation)
    {
        var projectDoc = context.Document.Project.AdditionalDocuments
            .FirstOrDefault(doc => doc.FilePath.EndsWith(".csproj"));

        if (projectDoc == null)
        {
            File.AppendAllLines(Environment.ExpandEnvironmentVariables("%TEMP%\\ProjectCodeFixer.log"), [".csproj not found in additional files"]);
            return context.Document.Project.Solution;
        }

        var xmlDoc = XDocument.Parse((await projectDoc.GetTextAsync(cancellation)).ToString());
        var props = xmlDoc.Root.Element("PropertyGroup");
        if (props == null)
        {
            props = new XElement("PropertyGroup");
            xmlDoc.Root.Add(props);
        }
        props.Add(new XElement("Foo", "Bar"));

        var text = await projectDoc.GetTextAsync(cancellation);
        var newDoc = context.Document.Project.Solution
            .WithAdditionalDocumentText(projectDoc.Id, SourceText.From(
                xmlDoc.ToString(), text.Encoding))
            .GetProject(context.Document.Project.Id)
            .GetDocument(context.Document.Id);

        File.AppendAllLines(Environment.ExpandEnvironmentVariables("%TEMP%\\ProjectCodeFixer.log"), ["Returned new XML with Foo=Bar property"]);

        return newDoc.Project.Solution;
    }
}
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RaidMonitor.Tests.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TestBaseAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor _rule = new(
        id: "RMT001",
        title: "Test class must inherit from TestBase",
        messageFormat: "Test class '{0}' must inherit from TestBase",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "All [TestFixture] classes should derive from TestBase for shared verification logic.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);
    }

    private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
    {
        var declaration = (ClassDeclarationSyntax)context.Node;

        var symbol = context.SemanticModel.GetDeclaredSymbol(declaration);

        if (symbol is null)
        {
            return;
        }

        if (!symbol.GetAttributes().Any(x => x.AttributeClass?.Name == "TestFixtureAttribute"))
        {
            return;
        }

        if (symbol.BaseType?.ToDisplayString() is "RaidMonitor.Tests.Common.TestBase")
        {
            return;
        }

        var diagnostic = Diagnostic.Create(_rule, declaration.Identifier.GetLocation(), symbol.Name);

        context.ReportDiagnostic(diagnostic);
    }
}

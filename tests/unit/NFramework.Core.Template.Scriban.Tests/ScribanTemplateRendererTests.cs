using NFramework.Core.Template.Abstractions;
using NFramework.Core.Template.Scriban;
using Shouldly;
using Xunit;

namespace NFramework.Core.Template.Scriban.Tests;

/// <summary>
/// Unit tests for ScribanTemplateRenderer
/// </summary>
public class ScribanTemplateRendererTests
{
    #region Basic Rendering Tests

    [Fact]
    public async Task RenderAsync_WithSimpleVariable_ShouldReplaceVariable()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "Hello {{name}}!";
        var data = new TestTemplateData { Name = "World" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Hello World!");
    }

    [Fact]
    public async Task RenderAsync_WithMultipleVariables_ShouldReplaceAllVariables()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{greeting}} {{name}}, your order {{orderId}} is ready!";
        var data = new OrderTemplateData
        {
            Greeting = "Hello",
            Name = "John",
            OrderId = "12345",
        };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Hello John, your order 12345 is ready!");
    }

    [Fact]
    public async Task RenderAsync_WithUnknownVariable_ShouldKeepVariableAsIs()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "Hello {{unknown}}!";
        var data = new TestTemplateData { Name = "World" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Hello {{unknown}}!");
    }

    [Fact]
    public async Task RenderAsync_WithEmptyTemplate_ShouldReturnEmptyString()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "";
        var data = new TestTemplateData { Name = "World" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public async Task RenderAsync_WithNullData_ShouldThrowArgumentException()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "Hello World!";

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => renderer.RenderAsync(template, null!));

        exception.ParamName.ShouldBe("data");
        exception.Message.ShouldContain("Data cannot be null");
    }

    [Fact]
    public async Task RenderAsync_WithCancellation_ShouldCancelOperation()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "Hello {{name}}!";
        var data = new TestTemplateData { Name = "World" };
        var cts = new CancellationTokenSource();

        // Act
        cts.Cancel();

        // Assert
        await Should.ThrowAsync<OperationCanceledException>(() => renderer.RenderAsync(template, data, cts.Token));
    }

    #endregion

    #region Built-in String Function Tests

    [Fact]
    public async Task RenderAsync_WithCamelCaseFunction_ShouldConvertToCamelCase()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{name | camel_case}}";
        var data = new TestTemplateData { Name = "user name" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("userName");
    }

    [Fact]
    public async Task RenderAsync_WithPascalCaseFunction_ShouldConvertToPascalCase()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{name | pascal_case}}";
        var data = new TestTemplateData { Name = "user name" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("UserName");
    }

    [Fact]
    public async Task RenderAsync_WithSnakeCaseFunction_ShouldConvertToSnakeCase()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{name | snake_case}}";
        var data = new TestTemplateData { Name = "UserName" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("user_name");
    }

    [Fact]
    public async Task RenderAsync_WithKebabCaseFunction_ShouldConvertToKebabCase()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{text | kebab_case}}";
        var data = new TestTemplateData { Name = "userName" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("user-name");
    }

    [Fact]
    public async Task RenderAsync_WithMultipleFunctions_ShouldApplyAllFunctions()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{name | snake_case | pascal_case}}";
        var data = new TestTemplateData { Name = "user-name" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("User_Name");
    }

    [Fact]
    public async Task RenderAsync_WithFunctionOnNonStringValue_ShouldHandleGracefully()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{count | camel_case}}";
        var data = new TestTemplateData { Count = 123 };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("123");
    }

    [Fact]
    public async Task RenderAsync_WithFunctionOnNullValue_ShouldReturnEmptyString()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{name | camel_case}}";
        var data = new TestTemplateData { Name = null };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("");
    }

    #endregion

    #region Conditional Logic Tests

    [Fact]
    public async Task RenderAsync_WithIfStatement_TrueCondition_ShouldRenderTrueBranch()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{#if active}}Active{{/if}}";
        var data = new TestTemplateData { IsActive = true };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Active");
    }

    [Fact]
    public async Task RenderAsync_WithIfStatement_FalseCondition_ShouldNotRender()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{#if active}}Active{{/if}}";
        var data = new TestTemplateData { IsActive = false };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public async Task RenderAsync_WithIfElseStatement_TrueCondition_ShouldRenderTrueBranch()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{#if active}}Active{{else}}Inactive{{/if}}";
        var data = new TestTemplateData { IsActive = true };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Active");
    }

    [Fact]
    public async Task RenderAsync_WithIfElseStatement_FalseCondition_ShouldRenderFalseBranch()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{#if active}}Active{{else}}Inactive{{/if}}";
        var data = new TestTemplateData { IsActive = false };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Inactive");
    }

    [Fact]
    public async Task RenderAsync_WithNullHandling_ShouldRenderFallback()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{name ?? 'Guest'}}";
        var data = new TestTemplateData { Name = null };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Guest");
    }

    [Fact]
    public async Task RenderAsync_WithValueProvided_ShouldUseValueNotFallback()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{name ?? 'Guest'}}";
        var data = new TestTemplateData { Name = "John" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("John");
    }

    #endregion

    #region Custom Function Tests

    [Fact]
    public async Task RenderAsync_WithCustomFunction_ShouldExecuteCustomFunction()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        renderer.AddCustomFunction(
            "format_date",
            (context, args) =>
            {
                if (args.Count == 0)
                    return "No date provided";

                // Simple mock date formatting
                var dateStr = args[0]?.ToString() ?? "";
                if (DateTime.TryParse(dateStr, out var date))
                {
                    return date.ToString("yyyy-MM-dd");
                }

                return "Invalid date";
            }
        );

        var template = "{{format_date date}}";
        var data = new DateTemplateData { Date = "2023-12-25" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("2023-12-25");
    }

    [Fact]
    public async Task RenderAsync_WithCustomFunctionMultipleArgs_ShouldHandleMultipleArguments()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        renderer.AddCustomFunction(
            "repeat_string",
            (context, args) =>
            {
                if (args.Count < 2)
                    return "";

                var text = args[0]?.ToString() ?? "";
                var count = args.Count > 1 ? Convert.ToInt32(args[1]) : 1;

                return string.Join("", Enumerable.Repeat(text, count));
            }
        );

        var template = "{{repeat_string text count}}";
        var data = new RepeatTemplateData { Text = "Hello", Count = 3 };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("HelloHelloHello");
    }

    [Fact]
    public async Task RenderAsync_WithUnknownCustomFunction_ShouldFailGracefully()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "{{unknown_function arg}}";
        var data = new TestTemplateData { Name = "test" };

        // Act & Assert
        var exception = await Should.ThrowAsync<InvalidOperationException>(() => renderer.RenderAsync(template, data));

        exception.Message.ShouldContain("Function 'unknown_function' not found");
    }

    #endregion

    #region TemplateExtension Tests

    [Fact]
    public void TemplateExtension_ShouldReturnCorrectExtension()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();

        // Act & Assert
        renderer.TemplateExtension.ShouldBe(".sb.html");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task RenderAsync_WithComplexTemplate_ShouldHandleComplexity()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template =
            @"
{{#if user.IsActive}}
  Hello {{user.Name}}! Welcome to {{user.Company}}.
  Your account has {{user.OrderCount}} orders.
{{else}}
  Please activate your account to continue.
{{/if}}";

        var data = new ComplexTemplateData
        {
            User = new UserProfile
            {
                Name = "John Doe",
                IsActive = true,
                Company = "Acme Corp",
                OrderCount = 5,
            },
        };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldContain("Hello John Doe!");
        result.ShouldContain("Welcome to Acme Corp.");
        result.ShouldContain("Your account has 5 orders.");
        result.ShouldNotContain("Please activate your account");
    }

    [Fact]
    public async Task RenderAsync_WithVeryLargeTemplate_ShouldHandleWithoutIssues()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var largeContent = new string('a', 10000);
        var template = $"Content: {{text}}";
        var data = new TestTemplateData { Text = largeContent };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldContain(largeContent);
    }

    [Fact]
    public async Task RenderAsync_WithUnicodeCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        var renderer = new ScribanTemplateRenderer();
        var template = "Hello {{name}}! 你好 {{greeting}}!";
        var data = new UnicodeTemplateData { Name = "世界", Greeting = "世界" };

        // Act
        var result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Hello 世界! 你好 世界!");
    }

    #endregion

    #region Test Data Classes

    private record TestTemplateData : ITemplateData
    {
        public string? Name { get; set; }
        public int? Count { get; set; }
        public bool IsActive { get; set; }
        public string? Text { get; set; }
    }

    private record OrderTemplateData : ITemplateData
    {
        public string? Greeting { get; set; }
        public string? Name { get; set; }
        public string? OrderId { get; set; }
    }

    private record DateTemplateData : ITemplateData
    {
        public string? Date { get; set; }
    }

    private record RepeatTemplateData : ITemplateData
    {
        public string? Text { get; set; }
        public int Count { get; set; }
    }

    private record ComplexTemplateData : ITemplateData
    {
        public UserProfile? User { get; set; }
    }

    private record UnicodeTemplateData : ITemplateData
    {
        public string? Name { get; set; }
        public string? Greeting { get; set; }
    }

    private record UserProfile
    {
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public string? Company { get; set; }
        public int OrderCount { get; set; }
    }

    #endregion
}

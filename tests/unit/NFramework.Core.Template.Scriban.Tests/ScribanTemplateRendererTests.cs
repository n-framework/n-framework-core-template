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
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "Hello {{name}}!";
        TestTemplateData data = new TestTemplateData { Name = "World" };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Hello World!");
    }

    [Fact]
    public async Task RenderAsync_WithMultipleVariables_ShouldReplaceAllVariables()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{greeting}} {{name}}, your order {{orderId}} is ready!";
        OrderTemplateData data = new OrderTemplateData
        {
            Greeting = "Hello",
            Name = "John",
            OrderId = "12345",
        };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Hello John, your order 12345 is ready!");
    }

    [Fact]
    public async Task RenderAsync_WithUnknownVariable_ShouldKeepVariableAsIs()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "Hello {{unknown}}!";
        TestTemplateData data = new TestTemplateData { Name = "World" };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Hello {{unknown}}!");
    }

    [Fact]
    public async Task RenderAsync_WithEmptyTemplate_ShouldReturnEmptyString()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "";
        TestTemplateData data = new TestTemplateData { Name = "World" };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public async Task RenderAsync_WithNullData_ShouldThrowArgumentException()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "Hello World!";

        // Act & Assert
        ArgumentException exception = await Should.ThrowAsync<ArgumentException>(() =>
            renderer.RenderAsync(template, null!)
        );

        exception.ParamName.ShouldBe("data");
        exception.Message.ShouldContain("Data cannot be null");
    }

    [Fact]
    public async Task RenderAsync_WithCancellation_ShouldCancelOperation()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "Hello {{name}}!";
        TestTemplateData data = new TestTemplateData { Name = "World" };
        CancellationTokenSource cts = new CancellationTokenSource();

        // Act
        cts.Cancel();

        // Assert
        _ = await Should.ThrowAsync<OperationCanceledException>(() => renderer.RenderAsync(template, data, cts.Token));
    }

    #endregion

    #region Built-in String Function Tests

    [Fact]
    public async Task RenderAsync_WithCamelCaseFunction_ShouldConvertToCamelCase()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{name | camel_case}}";
        TestTemplateData data = new TestTemplateData { Name = "user name" };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("userName");
    }

    [Fact]
    public async Task RenderAsync_WithPascalCaseFunction_ShouldConvertToPascalCase()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{name | pascal_case}}";
        TestTemplateData data = new TestTemplateData { Name = "user name" };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("UserName");
    }

    [Fact]
    public async Task RenderAsync_WithSnakeCaseFunction_ShouldConvertToSnakeCase()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{name | snake_case}}";
        TestTemplateData data = new TestTemplateData { Name = "UserName" };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("user_name");
    }

    [Fact]
    public async Task RenderAsync_WithKebabCaseFunction_ShouldConvertToKebabCase()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{text | kebab_case}}";
        TestTemplateData data = new TestTemplateData { Name = "userName" };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("user-name");
    }

    [Fact]
    public async Task RenderAsync_WithMultipleFunctions_ShouldApplyAllFunctions()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{name | snake_case | pascal_case}}";
        TestTemplateData data = new TestTemplateData { Name = "user-name" };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("User_Name");
    }

    [Fact]
    public async Task RenderAsync_WithFunctionOnNonStringValue_ShouldHandleGracefully()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{count | camel_case}}";
        TestTemplateData data = new TestTemplateData { Count = 123 };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("123");
    }

    [Fact]
    public async Task RenderAsync_WithFunctionOnNullValue_ShouldReturnEmptyString()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{name | camel_case}}";
        TestTemplateData data = new TestTemplateData { Name = null };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("");
    }

    #endregion

    #region Conditional Logic Tests

    [Fact]
    public async Task RenderAsync_WithIfStatement_TrueCondition_ShouldRenderTrueBranch()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{#if active}}Active{{/if}}";
        TestTemplateData data = new TestTemplateData { IsActive = true };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Active");
    }

    [Fact]
    public async Task RenderAsync_WithIfStatement_FalseCondition_ShouldNotRender()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{#if active}}Active{{/if}}";
        TestTemplateData data = new TestTemplateData { IsActive = false };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public async Task RenderAsync_WithIfElseStatement_TrueCondition_ShouldRenderTrueBranch()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{#if active}}Active{{else}}Inactive{{/if}}";
        TestTemplateData data = new TestTemplateData { IsActive = true };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Active");
    }

    [Fact]
    public async Task RenderAsync_WithIfElseStatement_FalseCondition_ShouldRenderFalseBranch()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{#if active}}Active{{else}}Inactive{{/if}}";
        TestTemplateData data = new TestTemplateData { IsActive = false };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Inactive");
    }

    [Fact]
    public async Task RenderAsync_WithNullHandling_ShouldRenderFallback()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{name ?? 'Guest'}}";
        TestTemplateData data = new TestTemplateData { Name = null };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Guest");
    }

    [Fact]
    public async Task RenderAsync_WithValueProvided_ShouldUseValueNotFallback()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{name ?? 'Guest'}}";
        TestTemplateData data = new TestTemplateData { Name = "John" };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("John");
    }

    #endregion

    #region Custom Function Tests

    [Fact]
    public async Task RenderAsync_WithCustomFunction_ShouldExecuteCustomFunction()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        renderer.AddCustomFunction(
            "format_date",
            (context, args) =>
            {
                if (args.Count == 0)
                    return "No date provided";

                // Simple mock date formatting
                string dateStr = args[0]?.ToString() ?? "";
                if (DateTime.TryParse(dateStr, out DateTime date))
                {
                    return date.ToString("yyyy-MM-dd");
                }

                return "Invalid date";
            }
        );

        string template = "{{format_date date}}";
        DateTemplateData data = new DateTemplateData { Date = "2023-12-25" };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("2023-12-25");
    }

    [Fact]
    public async Task RenderAsync_WithCustomFunctionMultipleArgs_ShouldHandleMultipleArguments()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        renderer.AddCustomFunction(
            "repeat_string",
            (context, args) =>
            {
                if (args.Count < 2)
                    return "";

                string text = args[0]?.ToString() ?? "";
                int count = args.Count > 1 ? Convert.ToInt32(args[1]) : 1;

                return string.Join("", Enumerable.Repeat(text, count));
            }
        );

        string template = "{{repeat_string text count}}";
        RepeatTemplateData data = new RepeatTemplateData { Text = "Hello", Count = 3 };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldBe("HelloHelloHello");
    }

    [Fact]
    public async Task RenderAsync_WithUnknownCustomFunction_ShouldFailGracefully()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "{{unknown_function arg}}";
        TestTemplateData data = new TestTemplateData { Name = "test" };

        // Act & Assert
        InvalidOperationException exception = await Should.ThrowAsync<InvalidOperationException>(() =>
            renderer.RenderAsync(template, data)
        );

        exception.Message.ShouldContain("Function 'unknown_function' not found");
    }

    #endregion

    #region TemplateExtension Tests

    [Fact]
    public void TemplateExtension_ShouldReturnCorrectExtension()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();

        // Act & Assert
        renderer.TemplateExtension.ShouldBe(".sbn");
    }

    #endregion

    #region Edge Cases

    [Fact]
    public async Task RenderAsync_WithComplexTemplate_ShouldHandleComplexity()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template =
            @"
{{#if user.IsActive}}
  Hello {{user.Name}}! Welcome to {{user.Company}}.
  Your account has {{user.OrderCount}} orders.
{{else}}
  Please activate your account to continue.
{{/if}}";

        ComplexTemplateData data = new ComplexTemplateData
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
        string result = await renderer.RenderAsync(template, data);

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
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string largeContent = new string('a', 10000);
        string template = $"Content: {{text}}";
        TestTemplateData data = new TestTemplateData { Text = largeContent };

        // Act
        string result = await renderer.RenderAsync(template, data);

        // Assert
        result.ShouldContain(largeContent);
    }

    [Fact]
    public async Task RenderAsync_WithUnicodeCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        ScribanTemplateRenderer renderer = new ScribanTemplateRenderer();
        string template = "Hello {{name}}! 你好 {{greeting}}!";
        UnicodeTemplateData data = new UnicodeTemplateData { Name = "世界", Greeting = "世界" };

        // Act
        string result = await renderer.RenderAsync(template, data);

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

using NFramework.Core.Template.Abstractions;
using Shouldly;
using Xunit;

namespace NFramework.Core.Template.Abstractions.Tests;

/// <summary>
/// Unit tests for template engine abstractions
/// </summary>
public class TemplateAbstractionsTests
{
    #region ITemplateEngine Tests

    [Fact]
    public void ITemplateEngine_ShouldHaveRenderAsyncMethod()
    {
        // Arrange & Act
        var engine = new Mock<ITemplateEngine>();

        // Assert
        engine
            .Setup(e => e.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("rendered content");
    }

    [Fact]
    public void ITemplateEngine_ShouldHaveRenderFileAsyncMethod()
    {
        // Arrange & Act
        var engine = new Mock<ITemplateEngine>();

        // Assert
        engine
            .Setup(e => e.RenderFileAsync(It.IsAny<TemplateFileRenderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("rendered content");
    }

    [Fact]
    public void ITemplateEngine_ShouldHaveRenderFilesAsyncMethod()
    {
        // Arrange & Act
        var engine = new Mock<ITemplateEngine>();

        // Assert
        engine
            .Setup(e => e.RenderFilesAsync(It.IsAny<TemplateFilesRenderRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "output1", "output2" });
    }

    #endregion

    #region ITemplateRenderer Tests

    [Fact]
    public void ITemplateRenderer_ShouldHaveRenderAsyncMethod()
    {
        // Arrange & Act
        var renderer = new Mock<ITemplateRenderer>();

        // Assert
        renderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("rendered content");
    }

    [Fact]
    public void ITemplateRenderer_ShouldHaveTemplateExtensionProperty()
    {
        // Arrange & Act
        var renderer = new Mock<ITemplateRenderer>();
        renderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        // Assert
        renderer.Object.TemplateExtension.ShouldBe(".sb.html");
    }

    #endregion

    #region TemplateRenderRequest Tests

    [Fact]
    public void TemplateRenderRequest_WithValidData_ShouldCreateInstance()
    {
        // Arrange
        var template = "Hello {{name}}!";
        var data = new TestTemplateData { Name = "World" };

        // Act
        var request = new TemplateRenderRequest(template, data);

        // Assert
        request.Template.ShouldBe(template);
        request.Data.ShouldBe(data);
    }

    [Fact]
    public void TemplateRenderRequest_WithNullTemplate_ShouldThrowArgumentException()
    {
        // Arrange
        var data = new TestTemplateData { Name = "World" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => new TemplateRenderRequest(null!, data));

        exception.ParamName.ShouldBe("Template");
        exception.Message.ShouldContain("Template cannot be null");
    }

    [Fact]
    public void TemplateRenderRequest_WithNullData_ShouldThrowArgumentException()
    {
        // Arrange
        var template = "Hello World!";

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() => new TemplateRenderRequest(template, null!));

        exception.ParamName.ShouldBe("Data");
        exception.Message.ShouldContain("Data cannot be null");
    }

    #endregion

    #region TemplateFileRenderRequest Tests

    [Fact]
    public void TemplateFileRenderRequest_WithValidData_ShouldCreateInstance()
    {
        // Arrange
        var templateFilePath = "templates/User.sb.html";
        var templateDirectoryPath = "templates";
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string> { { "ProjectName", "MyApp" }, { "EntityName", "User" } };
        var data = new TestTemplateData { Name = "Test" };

        // Act
        var request = new TemplateFileRenderRequest(
            templateFilePath,
            templateDirectoryPath,
            outputDirectoryPath,
            replacements,
            data
        );

        // Assert
        request.TemplateFilePath.ShouldBe(templateFilePath);
        request.TemplateDirectoryPath.ShouldBe(templateDirectoryPath);
        request.OutputDirectoryPath.ShouldBe(outputDirectoryPath);
        request.PathVariableReplacements.ShouldBe(replacements);
        request.Data.ShouldBe(data);
    }

    [Fact]
    public void TemplateFileRenderRequest_WithNullTemplateFilePath_ShouldThrowArgumentException()
    {
        // Arrange
        var templateDirectoryPath = "templates";
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFileRenderRequest(null!, templateDirectoryPath, outputDirectoryPath, replacements, data)
        );

        exception.ParamName.ShouldBe("TemplateFilePath");
        exception.Message.ShouldContain("TemplateFilePath cannot be null");
    }

    [Fact]
    public void TemplateFileRenderRequest_WithEmptyTemplateFilePath_ShouldThrowArgumentException()
    {
        // Arrange
        var templateDirectoryPath = "templates";
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFileRenderRequest("", templateDirectoryPath, outputDirectoryPath, replacements, data)
        );

        exception.ParamName.ShouldBe("TemplateFilePath");
        exception.Message.ShouldContain("TemplateFilePath cannot be empty");
    }

    [Fact]
    public void TemplateFileRenderRequest_WithNullTemplateDirectoryPath_ShouldThrowArgumentException()
    {
        // Arrange
        var templateFilePath = "templates/User.sb.html";
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFileRenderRequest(templateFilePath, null!, outputDirectoryPath, replacements, data)
        );

        exception.ParamName.ShouldBe("TemplateDirectoryPath");
        exception.Message.ShouldContain("TemplateDirectoryPath cannot be null");
    }

    [Fact]
    public void TemplateFileRenderRequest_WithEmptyTemplateDirectoryPath_ShouldThrowArgumentException()
    {
        // Arrange
        var templateFilePath = "templates/User.sb.html";
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFileRenderRequest(templateFilePath, "", outputDirectoryPath, replacements, data)
        );

        exception.ParamName.ShouldBe("TemplateDirectoryPath");
        exception.Message.ShouldContain("TemplateDirectoryPath cannot be empty");
    }

    [Fact]
    public void TemplateFileRenderRequest_WithNullOutputDirectoryPath_ShouldThrowArgumentException()
    {
        // Arrange
        var templateFilePath = "templates/User.sb.html";
        var templateDirectoryPath = "templates";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFileRenderRequest(templateFilePath, templateDirectoryPath, null!, replacements, data)
        );

        exception.ParamName.ShouldBe("OutputDirectoryPath");
        exception.Message.ShouldContain("OutputDirectoryPath cannot be null");
    }

    [Fact]
    public void TemplateFileRenderRequest_WithEmptyOutputDirectoryPath_ShouldThrowArgumentException()
    {
        // Arrange
        var templateFilePath = "templates/User.sb.html";
        var templateDirectoryPath = "templates";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFileRenderRequest(templateFilePath, templateDirectoryPath, "", replacements, data)
        );

        exception.ParamName.ShouldBe("OutputDirectoryPath");
        exception.Message.ShouldContain("OutputDirectoryPath cannot be empty");
    }

    [Fact]
    public void TemplateFileRenderRequest_WithNullData_ShouldThrowArgumentException()
    {
        // Arrange
        var templateFilePath = "templates/User.sb.html";
        var templateDirectoryPath = "templates";
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string>();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFileRenderRequest(
                templateFilePath,
                templateDirectoryPath,
                outputDirectoryPath,
                replacements,
                null!
            )
        );

        exception.ParamName.ShouldBe("Data");
        exception.Message.ShouldContain("Data cannot be null");
    }

    #endregion

    #region TemplateFilesRenderRequest Tests

    [Fact]
    public void TemplateFilesRenderRequest_WithValidData_ShouldCreateInstance()
    {
        // Arrange
        var templateFilePaths = new List<string> { "templates/User.sb.html", "templates/Project.sb.html" };
        var templateDirectoryPath = "templates";
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string> { { "ProjectName", "MyApp" } };
        var data = new TestTemplateData { Name = "Test" };

        // Act
        var request = new TemplateFilesRenderRequest(
            templateFilePaths,
            templateDirectoryPath,
            outputDirectoryPath,
            replacements,
            data
        );

        // Assert
        request.TemplateFilePaths.ShouldBe(templateFilePaths);
        request.TemplateDirectoryPath.ShouldBe(templateDirectoryPath);
        request.OutputDirectoryPath.ShouldBe(outputDirectoryPath);
        request.PathVariableReplacements.ShouldBe(replacements);
        request.Data.ShouldBe(data);
    }

    [Fact]
    public void TemplateFilesRenderRequest_WithNullTemplateFilePaths_ShouldThrowArgumentException()
    {
        // Arrange
        var templateDirectoryPath = "templates";
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFilesRenderRequest(null!, templateDirectoryPath, outputDirectoryPath, replacements, data)
        );

        exception.ParamName.ShouldBe("TemplateFilePaths");
        exception.Message.ShouldContain("TemplateFilePaths cannot be null");
    }

    [Fact]
    public void TemplateFilesRenderRequest_WithEmptyTemplateFilePaths_ShouldThrowArgumentException()
    {
        // Arrange
        var templateDirectoryPath = "templates";
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFilesRenderRequest(
                new List<string>(),
                templateDirectoryPath,
                outputDirectoryPath,
                replacements,
                data
            )
        );

        exception.ParamName.ShouldBe("TemplateFilePaths");
        exception.Message.ShouldContain("TemplateFilePaths cannot be empty");
    }

    [Fact]
    public void TemplateFilesRenderRequest_WithNullTemplateDirectoryPath_ShouldThrowArgumentException()
    {
        // Arrange
        var templateFilePaths = new List<string> { "templates/User.sb.html" };
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throws<ArgumentException>(() =>
            new TemplateFilesRenderRequest(templateFilePaths, null!, outputDirectoryPath, replacements, data)
        );

        exception.ParamName.ShouldBe("TemplateDirectoryPath");
        exception.Message.ShouldContain("TemplateDirectoryPath cannot be null");
    }

    [Fact]
    public void TemplateFilesRenderRequest_WithEmptyTemplateDirectoryPath_ShouldThrowArgumentException()
    {
        // Arrange
        var templateFilePaths = new List<string> { "templates/User.sb.html" };
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFilesRenderRequest(templateFilePaths, "", outputDirectoryPath, replacements, data)
        );

        exception.ParamName.ShouldBe("TemplateDirectoryPath");
        exception.Message.ShouldContain("TemplateDirectoryPath cannot be empty");
    }

    [Fact]
    public void TemplateFilesRenderRequest_WithNullOutputDirectoryPath_ShouldThrowArgumentException()
    {
        // Arrange
        var templateFilePaths = new List<string> { "templates/User.sb.html" };
        var templateDirectoryPath = "templates";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFilesRenderRequest(templateFilePaths, templateDirectoryPath, null!, replacements, data)
        );

        exception.ParamName.ShouldBe("OutputDirectoryPath");
        exception.Message.ShouldContain("OutputDirectoryPath cannot be null");
    }

    [Fact]
    public void TemplateFilesRenderRequest_WithEmptyOutputDirectoryPath_ShouldThrowArgumentException()
    {
        // Arrange
        var templateFilePaths = new List<string> { "templates/User.sb.html" };
        var templateDirectoryPath = "templates";
        var replacements = new Dictionary<string, string>();
        var data = new TestTemplateData { Name = "Test" };

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFilesRenderRequest(templateFilePaths, templateDirectoryPath, "", replacements, data)
        );

        exception.ParamName.ShouldBe("OutputDirectoryPath");
        exception.Message.ShouldContain("OutputDirectoryPath cannot be empty");
    }

    [Fact]
    public void TemplateFilesRenderRequest_WithNullData_ShouldThrowArgumentException()
    {
        // Arrange
        var templateFilePaths = new List<string> { "templates/User.sb.html" };
        var templateDirectoryPath = "templates";
        var outputDirectoryPath = "output";
        var replacements = new Dictionary<string, string>();

        // Act & Assert
        var exception = Should.Throw<ArgumentException>(() =>
            new TemplateFilesRenderRequest(
                templateFilePaths,
                templateDirectoryPath,
                outputDirectoryPath,
                replacements,
                null!
            )
        );

        exception.ParamName.ShouldBe("Data");
        exception.Message.ShouldContain("Data cannot be null");
    }

    #endregion

    #region Test Data Classes

    /// <summary>
    /// Test implementation of ITemplateData for testing purposes
    /// </summary>
    private record TestTemplateData : ITemplateData
    {
        public string? Name { get; set; }
        public int? Count { get; set; }
        public bool IsActive { get; set; }
    }

    #endregion
}

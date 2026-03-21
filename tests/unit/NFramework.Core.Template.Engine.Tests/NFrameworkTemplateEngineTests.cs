using NFramework.Core.Template.Abstractions;
using NFramework.Core.Template.Engine;
using Shouldly;
using Xunit;

namespace NFramework.Core.Template.Engine.Tests;

/// <summary>
/// Unit tests for NFrameworkTemplateEngine
/// </summary>
public class NFrameworkTemplateEngineTests
{
    #region RenderAsync Tests

    [Fact]
    public async Task RenderAsync_WithValidTemplateAndData_ShouldRenderContent()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");
        mockRenderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Hello World!");

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object);
        var template = "Hello {{name}}!";
        var data = new TestTemplateData { Name = "World" };

        // Act
        var result = await engine.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Hello World!");
        mockRenderer.Verify(r => r.RenderAsync(template, data, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RenderAsync_WithNullTemplate_ShouldThrowArgumentException()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        var engine = new NFrameworkTemplateEngine(mockRenderer.Object);
        var data = new TestTemplateData { Name = "World" };

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => engine.RenderAsync(null!, data));

        exception.ParamName.ShouldBe("template");
        exception.Message.ShouldContain("Template cannot be null");
    }

    [Fact]
    public async Task RenderAsync_WithEmptyTemplate_ShouldRenderEmptyString()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        mockRenderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("");

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object);
        var data = new TestTemplateData { Name = "World" };

        // Act
        var result = await engine.RenderAsync("", data);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public async Task RenderAsync_WithNullData_ShouldThrowArgumentException()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        var engine = new NFrameworkTemplateEngine(mockRenderer.Object);
        var template = "Hello World!";

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => engine.RenderAsync(template, null!));

        exception.ParamName.ShouldBe("data");
        exception.Message.ShouldContain("Data cannot be null");
    }

    [Fact]
    public async Task RenderAsync_WithCancellation_ShouldCancelOperation()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        var cts = new CancellationTokenSource();

        // Setup to simulate cancellation
        mockRenderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), cts.Token))
            .ReturnsAsync<string>(task =>
            {
                await Task.Delay(100, cts.Token);
                return "Hello World!";
            });

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object);
        var template = "Hello {{name}}!";
        var data = new TestTemplateData { Name = "World" };

        // Act
        cts.Cancel();

        // Assert
        await Should.ThrowAsync<OperationCanceledException>(() => engine.RenderAsync(template, data, cts.Token));
    }

    #endregion

    #region RenderFileAsync Tests

    [Fact]
    public async Task RenderFileAsync_WithValidRequest_ShouldRenderAndWriteFile()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        var fileSystem = new MockFileSystem();
        var templateContent = "Hello {{name}}!";
        fileSystem.WriteAllText("/templates/User.sb.html", templateContent);

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        var request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/User.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        var result = await engine.RenderFileAsync(request);

        // Assert
        result.ShouldBe("/output/User.cs");
        mockRenderer.Verify(
            r => r.RenderAsync(templateContent, request.Data, It.IsAny<CancellationToken>()),
            Times.Once
        );
        fileSystem.FileExists("/output/User.cs").ShouldBeTrue();
        fileSystem.ReadAllText("/output/User.cs").ShouldBe("Hello World!");
    }

    [Fact]
    public async Task RenderFileAsync_WithNonExistentTemplateFile_ShouldThrowArgumentException()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        var fileSystem = new MockFileSystem();
        var engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);

        var request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/NonExistent.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act & Assert
        var exception = await Should.ThrowAsync<ArgumentException>(() => engine.RenderFileAsync(request));

        exception.ParamName.ShouldBe("request.TemplateFilePath");
        exception.Message.ShouldContain("Template file does not exist");
    }

    [Fact]
    public async Task RenderFileAsync_WithEmptyTemplateFile_ShouldRenderEmptyContent()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        mockRenderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("");

        var fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("/templates/Empty.sb.html", "");

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        var request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/Empty.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        var result = await engine.RenderFileAsync(request);

        // Assert
        result.ShouldBe("/output/.cs");
        fileSystem.ReadAllText("/output/.cs").ShouldBe("");
    }

    [Fact]
    public async Task RenderFileAsync_WithPathVariableReplacements_ShouldApplyReplacements()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        var fileSystem = new MockFileSystem();
        var templateContent = "Hello {{name}}!";
        fileSystem.WriteAllText("/templates/{{ProjectName}}.sb.html", templateContent);

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        var request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/{{ProjectName}}.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string> { { "ProjectName", "MyApp" } },
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        var result = await engine.RenderFileAsync(request);

        // Assert
        result.ShouldBe("/output/MyApp.cs");
    }

    [Fact]
    public async Task RenderFileAsync_WithOutputDirectoryNotExisting_ShouldCreateDirectory()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        var fileSystem = new MockFileSystem();
        var templateContent = "Hello World!";
        fileSystem.WriteAllText("/templates/Test.sb.html", templateContent);

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        var request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/Test.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/new/output/dir",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        var result = await engine.RenderFileAsync(request);

        // Assert
        result.ShouldBe("/new/output/dir/Test.cs");
        fileSystem.DirectoryExists("/new/output/dir").ShouldBeTrue();
    }

    [Fact]
    public async Task RenderFileAsync_WithCancellation_ShouldCancelOperation()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        var cts = new CancellationTokenSource();

        mockRenderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), cts.Token))
            .ReturnsAsync<string>(task =>
            {
                await Task.Delay(100, cts.Token);
                return "Hello World!";
            });

        var fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("/templates/Test.sb.html", "Hello {{name}}!");

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        var request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/Test.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        cts.Cancel();

        // Assert
        await Should.ThrowAsync<OperationCanceledException>(() => engine.RenderFileAsync(request, cts.Token));
    }

    #endregion

    #region RenderFilesAsync Tests

    [Fact]
    public async Task RenderFilesAsync_WithValidRequest_ShouldProcessAllFiles()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        var fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("/templates/User.sb.html", "User: {{name}}");
        fileSystem.WriteAllText("/templates/Project.sb.html", "Project: {{projectName}}");

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        var request = new TemplateFilesRenderRequest(
            templateFilePaths: new List<string> { "/templates/User.sb.html", "/templates/Project.sb.html" },
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "User", ProjectName = "MyApp" }
        );

        // Act
        var results = await engine.RenderFilesAsync(request);

        // Assert
        results.ShouldContain("/output/User.cs");
        results.ShouldContain("/output/Project.cs");
        results.Count.ShouldBe(2);

        fileSystem.FileExists("/output/User.cs").ShouldBeTrue();
        fileSystem.FileExists("/output/Project.cs").ShouldBeTrue();
    }

    [Fact]
    public async Task RenderFilesAsync_WithOneFileFailing_ShouldNotCreatePartialOutput()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        var fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("/templates/Valid.sb.html", "Valid content");

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        var request = new TemplateFilesRenderRequest(
            templateFilePaths: new List<string> { "/templates/Valid.sb.html", "/templates/Invalid.sb.html" },
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "Test" }
        );

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(() => engine.RenderFilesAsync(request));

        // Verify no files were created
        fileSystem.FileExists("/output/Valid.cs").ShouldBeFalse();
    }

    [Fact]
    public async Task RenderFilesAsync_WithCancellation_ShouldCancelGracefully()
    {
        // Arrange
        var mockRenderer = new Mock<ITemplateRenderer>();
        var cts = new CancellationTokenSource();

        var fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("/templates/File1.sb.html", "Content 1");
        fileSystem.WriteAllText("/templates/File2.sb.html", "Content 2");

        var engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        var request = new TemplateFilesRenderRequest(
            templateFilePaths: new List<string> { "/templates/File1.sb.html", "/templates/File2.sb.html" },
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "Test" }
        );

        // Act
        cts.CancelAfter(10);

        // Assert
        await Should.ThrowAsync<OperationCanceledException>(() => engine.RenderFilesAsync(request, cts.Token));
    }

    #endregion

    #region Test Data Classes

    /// <summary>
    /// Test implementation of ITemplateData for testing purposes
    /// </summary>
    private record TestTemplateData : ITemplateData
    {
        public string? Name { get; set; }
        public string? ProjectName { get; set; }
    }

    #endregion
}

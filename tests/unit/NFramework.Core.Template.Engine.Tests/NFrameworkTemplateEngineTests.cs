using NFramework.Core.Template.Abstractions;
using NFramework.Core.Template.Abstractions.Models;
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
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        _ = mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");
        _ = mockRenderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Hello World!");

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object);
        string template = "Hello {{name}}!";
        TestTemplateData data = new TestTemplateData { Name = "World" };

        // Act
        string result = await engine.RenderAsync(template, data);

        // Assert
        result.ShouldBe("Hello World!");
        mockRenderer.Verify(r => r.RenderAsync(template, data, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RenderAsync_WithNullTemplate_ShouldThrowArgumentException()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object);
        TestTemplateData data = new TestTemplateData { Name = "World" };

        // Act & Assert
        ArgumentException exception = await Should.ThrowAsync<ArgumentException>(() => engine.RenderAsync(null!, data));

        exception.ParamName.ShouldBe("template");
        exception.Message.ShouldContain("Template cannot be null");
    }

    [Fact]
    public async Task RenderAsync_WithEmptyTemplate_ShouldRenderEmptyString()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        _ = mockRenderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("");

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object);
        TestTemplateData data = new TestTemplateData { Name = "World" };

        // Act
        string result = await engine.RenderAsync("", data);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public async Task RenderAsync_WithNullData_ShouldThrowArgumentException()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object);
        string template = "Hello World!";

        // Act & Assert
        ArgumentException exception = await Should.ThrowAsync<ArgumentException>(() =>
            engine.RenderAsync(template, null!)
        );

        exception.ParamName.ShouldBe("data");
        exception.Message.ShouldContain("Data cannot be null");
    }

    [Fact]
    public async Task RenderAsync_WithCancellation_ShouldCancelOperation()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        CancellationTokenSource cts = new CancellationTokenSource();

        // Setup to simulate cancellation
        _ = mockRenderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), cts.Token))
            .Returns(async () =>
            {
                await Task.Delay(100, cts.Token);
                return "Hello World!";
            });

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object);
        string template = "Hello {{name}}!";
        TestTemplateData data = new TestTemplateData { Name = "World" };

        // Act
        cts.Cancel();

        // Assert
        _ = await Should.ThrowAsync<OperationCanceledException>(() => engine.RenderAsync(template, data, cts.Token));
    }

    #endregion

    #region RenderFileAsync Tests

    [Fact]
    public async Task RenderFileAsync_WithValidRequest_ShouldRenderAndWriteFile()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        _ = mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        MockFileSystem fileSystem = new MockFileSystem();
        string templateContent = "Hello {{name}}!";
        fileSystem.WriteAllText("/templates/User.sb.html", templateContent);

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        TemplateFileRenderRequest request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/User.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        string result = await engine.RenderFileAsync(request);

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
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        MockFileSystem fileSystem = new MockFileSystem();
        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);

        TemplateFileRenderRequest request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/NonExistent.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act & Assert
        ArgumentException exception = await Should.ThrowAsync<ArgumentException>(() => engine.RenderFileAsync(request));

        exception.ParamName.ShouldBe("request.TemplateFilePath");
        exception.Message.ShouldContain("Template file does not exist");
    }

    [Fact]
    public async Task RenderFileAsync_WithEmptyTemplateFile_ShouldRenderEmptyContent()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        _ = mockRenderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("");

        MockFileSystem fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("/templates/Empty.sb.html", "");

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        TemplateFileRenderRequest request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/Empty.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        string result = await engine.RenderFileAsync(request);

        // Assert
        result.ShouldBe("/output/.cs");
        fileSystem.ReadAllText("/output/.cs").ShouldBe("");
    }

    [Fact]
    public async Task RenderFileAsync_WithPathVariableReplacements_ShouldApplyReplacements()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        _ = mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        MockFileSystem fileSystem = new MockFileSystem();
        string templateContent = "Hello {{name}}!";
        fileSystem.WriteAllText("/templates/{{ProjectName}}.sb.html", templateContent);

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        TemplateFileRenderRequest request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/{{ProjectName}}.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string> { { "ProjectName", "MyApp" } },
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        string result = await engine.RenderFileAsync(request);

        // Assert
        result.ShouldBe("/output/MyApp.cs");
    }

    [Theory]
    [InlineData("/templates/User.sbn", "/output/User.cs")]
    [InlineData("/templates/User.scriban", "/output/User.cs")]
    [InlineData("/templates/Page.sbn-html", "/output/Page.html")]
    [InlineData("/templates/Page.sbn-htm", "/output/Page.htm")]
    [InlineData("/templates/Page.sbnhtml", "/output/Page.html")]
    [InlineData("/templates/Page.sbnhtm", "/output/Page.htm")]
    [InlineData("/templates/Readme.sbn-txt", "/output/Readme.txt")]
    [InlineData("/templates/Readme.sbntxt", "/output/Readme.txt")]
    [InlineData("/templates/User.sbn-cs", "/output/User.cs")]
    [InlineData("/templates/User.sbncs", "/output/User.cs")]
    public async Task RenderFileAsync_WithSupportedTemplateExtensions_ShouldGenerateExpectedOutputPath(
        string templateFilePath,
        string expectedOutputPath
    )
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        _ = mockRenderer.Setup(r => r.TemplateExtension).Returns(".sbn");

        MockFileSystem fileSystem = new MockFileSystem();
        fileSystem.WriteAllText(templateFilePath, "Hello {{name}}!");

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        TemplateFileRenderRequest request = new TemplateFileRenderRequest(
            templateFilePath: templateFilePath,
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        string result = await engine.RenderFileAsync(request);

        // Assert
        result.ShouldBe(expectedOutputPath);
        fileSystem.FileExists(expectedOutputPath).ShouldBeTrue();
    }

    [Fact]
    public async Task RenderFileAsync_WithOutputDirectoryNotExisting_ShouldCreateDirectory()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        _ = mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        MockFileSystem fileSystem = new MockFileSystem();
        string templateContent = "Hello World!";
        fileSystem.WriteAllText("/templates/Test.sb.html", templateContent);

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        TemplateFileRenderRequest request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/Test.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/new/output/dir",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        string result = await engine.RenderFileAsync(request);

        // Assert
        result.ShouldBe("/new/output/dir/Test.cs");
        fileSystem.DirectoryExists("/new/output/dir").ShouldBeTrue();
    }

    [Fact]
    public async Task RenderFileAsync_WithCancellation_ShouldCancelOperation()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        CancellationTokenSource cts = new CancellationTokenSource();

        _ = mockRenderer
            .Setup(r => r.RenderAsync(It.IsAny<string>(), It.IsAny<ITemplateData>(), cts.Token))
            .Returns(async () =>
            {
                await Task.Delay(100, cts.Token);
                return "Hello World!";
            });

        MockFileSystem fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("/templates/Test.sb.html", "Hello {{name}}!");

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        TemplateFileRenderRequest request = new TemplateFileRenderRequest(
            templateFilePath: "/templates/Test.sb.html",
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "World" }
        );

        // Act
        cts.Cancel();

        // Assert
        _ = await Should.ThrowAsync<OperationCanceledException>(() => engine.RenderFileAsync(request, cts.Token));
    }

    #endregion

    #region RenderFilesAsync Tests

    [Fact]
    public async Task RenderFilesAsync_WithValidRequest_ShouldProcessAllFiles()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        _ = mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        MockFileSystem fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("/templates/User.sb.html", "User: {{name}}");
        fileSystem.WriteAllText("/templates/Project.sb.html", "Project: {{projectName}}");

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        TemplateFilesRenderRequest request = new TemplateFilesRenderRequest(
            templateFilePaths: new List<string> { "/templates/User.sb.html", "/templates/Project.sb.html" },
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "User", ProjectName = "MyApp" }
        );

        // Act
        IReadOnlyList<string> results = await engine.RenderFilesAsync(request);

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
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        _ = mockRenderer.Setup(r => r.TemplateExtension).Returns(".sb.html");

        MockFileSystem fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("/templates/Valid.sb.html", "Valid content");

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        TemplateFilesRenderRequest request = new TemplateFilesRenderRequest(
            templateFilePaths: new List<string> { "/templates/Valid.sb.html", "/templates/Invalid.sb.html" },
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "Test" }
        );

        // Act & Assert
        _ = await Should.ThrowAsync<ArgumentException>(() => engine.RenderFilesAsync(request));

        // Verify no files were created
        fileSystem.FileExists("/output/Valid.cs").ShouldBeFalse();
    }

    [Fact]
    public async Task RenderFilesAsync_WithCancellation_ShouldCancelGracefully()
    {
        // Arrange
        Mock<ITemplateRenderer> mockRenderer = new Mock<ITemplateRenderer>();
        CancellationTokenSource cts = new CancellationTokenSource();

        MockFileSystem fileSystem = new MockFileSystem();
        fileSystem.WriteAllText("/templates/File1.sb.html", "Content 1");
        fileSystem.WriteAllText("/templates/File2.sb.html", "Content 2");

        NFrameworkTemplateEngine engine = new NFrameworkTemplateEngine(mockRenderer.Object, fileSystem);
        TemplateFilesRenderRequest request = new TemplateFilesRenderRequest(
            templateFilePaths: new List<string> { "/templates/File1.sb.html", "/templates/File2.sb.html" },
            templateDirectoryPath: "/templates",
            outputDirectoryPath: "/output",
            pathVariableReplacements: new Dictionary<string, string>(),
            data: new TestTemplateData { Name = "Test" }
        );

        // Act
        cts.CancelAfter(10);

        // Assert
        _ = await Should.ThrowAsync<OperationCanceledException>(() => engine.RenderFilesAsync(request, cts.Token));
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

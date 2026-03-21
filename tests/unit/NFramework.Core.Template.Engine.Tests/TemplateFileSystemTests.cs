using NFramework.Core.Template.Engine;
using NFramework.Core.Template.Engine.Abstractions;
using Shouldly;
using Xunit;

namespace NFramework.Core.Template.Engine.Tests;

/// <summary>
/// Unit tests for TemplateFileSystem
/// </summary>
public class TemplateFileSystemTests : IDisposable
{
    private readonly string _testDirectory;

    public TemplateFileSystemTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"TemplateFileSystemTests_{Guid.NewGuid():N}");
        _ = Directory.CreateDirectory(_testDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }

    #region FileExists Tests

    [Fact]
    public void FileExists_WithExistingFile_ShouldReturnTrue()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = Path.Combine(_testDirectory, "existing.txt");
        File.WriteAllText(filePath, "content");

        // Act
        bool result = fileSystem.FileExists(filePath);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void FileExists_WithNonExistingFile_ShouldReturnFalse()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = Path.Combine(_testDirectory, "nonexistent.txt");

        // Act
        bool result = fileSystem.FileExists(filePath);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region ReadAllTextAsync Tests

    [Fact]
    public async Task ReadAllTextAsync_WithExistingFile_ShouldReturnContent()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = Path.Combine(_testDirectory, "readable.txt");
        string expectedContent = "Hello, World!";
        File.WriteAllText(filePath, expectedContent);

        // Act
        string result = await fileSystem.ReadAllTextAsync(filePath, CancellationToken.None);

        // Assert
        result.ShouldBe(expectedContent);
    }

    [Fact]
    public async Task ReadAllTextAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = Path.Combine(_testDirectory, "readable.txt");
        File.WriteAllText(filePath, "content");
        CancellationTokenSource cts = new();
        cts.Cancel();

        // Act & Assert
        _ = await Should.ThrowAsync<OperationCanceledException>(() => fileSystem.ReadAllTextAsync(filePath, cts.Token));
    }

    #endregion

    #region WriteAllTextAsync Tests

    [Fact]
    public async Task WriteAllTextAsync_WithValidPath_ShouldWriteContent()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = Path.Combine(_testDirectory, "writable.txt");
        string content = "Hello, World!";

        // Act
        await fileSystem.WriteAllTextAsync(filePath, content, CancellationToken.None);

        // Assert
        string result = File.ReadAllText(filePath);
        result.ShouldBe(content);
    }

    [Fact]
    public async Task WriteAllTextAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = Path.Combine(_testDirectory, "writable.txt");
        CancellationTokenSource cts = new();
        cts.Cancel();

        // Act & Assert
        _ = await Should.ThrowAsync<OperationCanceledException>(() =>
            fileSystem.WriteAllTextAsync(filePath, "content", cts.Token)
        );
    }

    [Fact]
    public async Task WriteAllTextAsync_WithEmptyContent_ShouldWriteEmptyFile()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = Path.Combine(_testDirectory, "empty.txt");

        // Act
        await fileSystem.WriteAllTextAsync(filePath, "", CancellationToken.None);

        // Assert
        File.Exists(filePath).ShouldBeTrue();
        string result = File.ReadAllText(filePath);
        result.ShouldBe("");
    }

    #endregion

    #region DirectoryExists Tests

    [Fact]
    public void DirectoryExists_WithExistingDirectory_ShouldReturnTrue()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string dirPath = Path.Combine(_testDirectory, "existing");
        _ = Directory.CreateDirectory(dirPath);

        // Act
        bool result = fileSystem.DirectoryExists(dirPath);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void DirectoryExists_WithNonExistingDirectory_ShouldReturnFalse()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string dirPath = Path.Combine(_testDirectory, "nonexistent");

        // Act
        bool result = fileSystem.DirectoryExists(dirPath);

        // Assert
        result.ShouldBeFalse();
    }

    #endregion

    #region CreateDirectory Tests

    [Fact]
    public void CreateDirectory_WithNewPath_ShouldCreateDirectory()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string dirPath = Path.Combine(_testDirectory, "new_directory");

        // Act
        fileSystem.CreateDirectory(dirPath);

        // Assert
        Directory.Exists(dirPath).ShouldBeTrue();
    }

    [Fact]
    public void CreateDirectory_WithNestedPath_ShouldCreateAllDirectories()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string dirPath = Path.Combine(_testDirectory, "level1", "level2", "level3");

        // Act
        fileSystem.CreateDirectory(dirPath);

        // Assert
        Directory.Exists(dirPath).ShouldBeTrue();
    }

    [Fact]
    public void CreateDirectory_WithExistingDirectory_ShouldNotThrow()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string dirPath = Path.Combine(_testDirectory, "existing");
        _ = Directory.CreateDirectory(dirPath);

        // Act & Assert
        Should.NotThrow(() => fileSystem.CreateDirectory(dirPath));
        Directory.Exists(dirPath).ShouldBeTrue();
    }

    #endregion

    #region CombinePaths Tests

    [Fact]
    public void CombinePaths_WithTwoPaths_ShouldReturnCombinedPath()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string path1 = "/base";
        string path2 = "subfolder";

        // Act
        string result = fileSystem.CombinePaths(path1, path2);

        // Assert
        result.ShouldBe(Path.Combine(path1, path2));
    }

    [Fact]
    public void CombinePaths_WithThreePaths_ShouldReturnCombinedPath()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string path1 = "/base";
        string path2 = "subfolder";
        string path3 = "file.txt";

        // Act
        string result = fileSystem.CombinePaths(path1, path2, path3);

        // Assert
        result.ShouldBe(Path.Combine(path1, path2, path3));
    }

    [Fact]
    public void CombinePaths_WithEmptyPaths_ShouldHandleGracefully()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();

        // Act
        string result = fileSystem.CombinePaths("", "file.txt");

        // Assert
        result.ShouldBe("file.txt");
    }

    #endregion

    #region GetDirectoryName Tests

    [Fact]
    public void GetDirectoryName_WithFilePath_ShouldReturnDirectory()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = "/base/subfolder/file.txt";

        // Act
        string? result = fileSystem.GetDirectoryName(filePath);

        // Assert
        result.ShouldBe("/base/subfolder");
    }

    [Fact]
    public void GetDirectoryName_WithRootPath_ShouldReturnNull()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = "/file.txt";

        // Act
        string? result = fileSystem.GetDirectoryName(filePath);

        // Assert
        result.ShouldBe("/");
    }

    [Fact]
    public void GetDirectoryName_WithFileNameOnly_ShouldReturnEmpty()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = "file.txt";

        // Act
        string? result = fileSystem.GetDirectoryName(filePath);

        // Assert
        result.ShouldBe("");
    }

    #endregion

    #region GetFileName Tests

    [Fact]
    public void GetFileName_WithFullPath_ShouldReturnFileName()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = "/base/subfolder/file.txt";

        // Act
        string result = fileSystem.GetFileName(filePath);

        // Assert
        result.ShouldBe("file.txt");
    }

    [Fact]
    public void GetFileName_WithFileNameOnly_ShouldReturnFileName()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = "file.txt";

        // Act
        string result = fileSystem.GetFileName(filePath);

        // Assert
        result.ShouldBe("file.txt");
    }

    [Fact]
    public void GetFileName_WithEmptyPath_ShouldReturnEmpty()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();
        string filePath = "";

        // Act
        string result = fileSystem.GetFileName(filePath);

        // Assert
        result.ShouldBe("");
    }

    #endregion

    #region DirectorySeparatorChar Tests

    [Fact]
    public void DirectorySeparatorChar_ShouldMatchPathDirectorySeparatorChar()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();

        // Act
        char result = fileSystem.DirectorySeparatorChar;

        // Assert
        result.ShouldBe(Path.DirectorySeparatorChar);
    }

    [Fact]
    public void AltDirectorySeparatorChar_ShouldMatchPathAltDirectorySeparatorChar()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();

        // Act
        char result = fileSystem.AltDirectorySeparatorChar;

        // Assert
        result.ShouldBe(Path.AltDirectorySeparatorChar);
    }

    #endregion

    #region ITemplateFileSystem Interface Tests

    [Fact]
    public void TemplateFileSystem_ShouldImplementITemplateFileSystem()
    {
        // Arrange
        TemplateFileSystem fileSystem = new();

        // Act & Assert
        _ = fileSystem.ShouldBeAssignableTo<ITemplateFileSystem>();
    }

    #endregion
}

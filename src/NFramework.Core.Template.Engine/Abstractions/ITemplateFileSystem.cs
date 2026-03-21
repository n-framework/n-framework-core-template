namespace NFramework.Core.Template.Engine.Abstractions;

public interface ITemplateFileSystem
{
    bool FileExists(string path);

    Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken);

    Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken);

    bool DirectoryExists(string path);

    void CreateDirectory(string path);

    string CombinePaths(string path1, string path2);

    string CombinePaths(string path1, string path2, string path3);

    string? GetDirectoryName(string path);

    string GetFileName(string path);

    char DirectorySeparatorChar { get; }

    char AltDirectorySeparatorChar { get; }
}

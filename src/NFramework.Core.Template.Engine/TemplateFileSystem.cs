using NFramework.Core.Template.Engine.Abstractions;

namespace NFramework.Core.Template.Engine;

public sealed class TemplateFileSystem : ITemplateFileSystem
{
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
    {
        return File.ReadAllTextAsync(path, cancellationToken);
    }

    public Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken)
    {
        return File.WriteAllTextAsync(path, contents, cancellationToken);
    }

    public bool DirectoryExists(string path)
    {
        return Directory.Exists(path);
    }

    public void CreateDirectory(string path)
    {
        _ = Directory.CreateDirectory(path);
    }

    public string CombinePaths(string path1, string path2)
    {
        return Path.Combine(path1, path2);
    }

    public string CombinePaths(string path1, string path2, string path3)
    {
        return Path.Combine(path1, path2, path3);
    }

    public string? GetDirectoryName(string path)
    {
        return Path.GetDirectoryName(path);
    }

    public string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;

    public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;
}

using NFramework.Core.Template.Engine.Abstractions;

namespace NFramework.Core.Template.Engine.Tests;

public sealed class MockFileSystem : ITemplateFileSystem
{
    private readonly Dictionary<string, string> _files = new(StringComparer.Ordinal);
    private readonly HashSet<string> _directories = new(StringComparer.Ordinal);

    public bool FileExists(string path)
    {
        return _files.ContainsKey(path);
    }

    public async Task<string> ReadAllTextAsync(string path, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Delay(20, cancellationToken);

        if (!_files.TryGetValue(path, out string? value))
        {
            throw new FileNotFoundException($"File not found: {path}", path);
        }

        return value;
    }

    public async Task WriteAllTextAsync(string path, string contents, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Delay(20, cancellationToken);
        string? directory = GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            CreateDirectory(directory);
        }

        _files[path] = contents;
    }

    public bool DirectoryExists(string path)
    {
        return _directories.Contains(path);
    }

    public void CreateDirectory(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        string[] parts = path.Split(DirectorySeparatorChar, AltDirectorySeparatorChar);
        string current = path.StartsWith(DirectorySeparatorChar) ? DirectorySeparatorChar.ToString() : string.Empty;
        foreach (string part in parts)
        {
            if (part.Length == 0)
            {
                continue;
            }

            current = current.Length == 0 ? part : CombinePaths(current, part);
            _ = _directories.Add(current);
        }

        _ = _directories.Add(path);
    }

    public string CombinePaths(string path1, string path2)
    {
        if (path1.Length == 0)
        {
            return path2;
        }

        if (path2.Length == 0)
        {
            return path1;
        }

        if (path2.StartsWith(DirectorySeparatorChar) || path2.StartsWith(AltDirectorySeparatorChar))
        {
            return path2;
        }

        string separator =
            path1.EndsWith(DirectorySeparatorChar)
            || path2.StartsWith(DirectorySeparatorChar)
            || path2.StartsWith(AltDirectorySeparatorChar)
                ? string.Empty
                : DirectorySeparatorChar.ToString();

        return path1 + separator + path2;
    }

    public string CombinePaths(string path1, string path2, string path3)
    {
        return CombinePaths(CombinePaths(path1, path2), path3);
    }

    public string? GetDirectoryName(string path)
    {
        int separatorIndex = Math.Max(
            path.LastIndexOf(DirectorySeparatorChar),
            path.LastIndexOf(AltDirectorySeparatorChar)
        );
        return separatorIndex < 0 ? string.Empty : path[..separatorIndex];
    }

    public string GetFileName(string path)
    {
        int separatorIndex = Math.Max(
            path.LastIndexOf(DirectorySeparatorChar),
            path.LastIndexOf(AltDirectorySeparatorChar)
        );
        return separatorIndex < 0 ? path : path[(separatorIndex + 1)..];
    }

    public char DirectorySeparatorChar => '/';

    public char AltDirectorySeparatorChar => '\\';

    public void WriteAllText(string path, string contents)
    {
        string? directory = GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
        {
            CreateDirectory(directory);
        }

        _files[path] = contents;
    }

    public string ReadAllText(string path)
    {
        if (!_files.TryGetValue(path, out string? value))
        {
            throw new FileNotFoundException($"File not found: {path}", path);
        }

        return value;
    }
}

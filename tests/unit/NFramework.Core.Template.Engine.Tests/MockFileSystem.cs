using System.IO.Abstractions;

namespace NFramework.Core.Template.Engine.Tests;

/// <summary>
/// Simple mock file system for testing purposes
/// </summary>
public class MockFileSystem : IFileSystem
{
    private readonly Dictionary<string, string> _files = new();
    private readonly HashSet<string> _directories = new();

    public IFile File => new MockFile(this);
    public IDirectory Directory => new MockDirectory(this);
    public IPath Path => new MockPath();

    public void WriteAllText(string path, string contents)
    {
        EnsureDirectoryExists(Path.GetDirectoryName(path)!);
        _files[path] = contents;
    }

    public string ReadAllText(string path)
    {
        if (!_files.ContainsKey(path))
        {
            throw new FileNotFoundException($"File not found: {path}");
        }
        return _files[path];
    }

    public bool FileExists(string path)
    {
        return _files.ContainsKey(path);
    }

    public bool DirectoryExists(string path)
    {
        return _directories.Contains(path);
    }

    public void CreateDirectory(string path)
    {
        if (!DirectoryExists(path))
        {
            _directories.Add(path);
        }
    }

    private void EnsureDirectoryExists(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        var parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var currentPath = "";

        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part))
                continue;

            currentPath = Path.Combine(currentPath, part);
            if (!DirectoryExists(currentPath))
            {
                CreateDirectory(currentPath);
            }
        }
    }
}

internal class MockFile : IFile
{
    private readonly MockFileSystem _fileSystem;

    public MockFile(MockFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public void WriteAllText(string path, string contents)
    {
        _fileSystem.WriteAllText(path, contents);
    }

    public string ReadAllText(string path)
    {
        return _fileSystem.ReadAllText(path);
    }

    public bool Exists(string path)
    {
        return _fileSystem.FileExists(path);
    }

    public void Delete(string path)
    {
        _fileSystem._files.Remove(path);
    }
}

internal class MockDirectory : IDirectory
{
    private readonly MockFileSystem _fileSystem;

    public MockDirectory(MockFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public bool Exists(string path)
    {
        return _fileSystem.DirectoryExists(path);
    }

    public void CreateDirectory(string path)
    {
        _fileSystem.CreateDirectory(path);
    }

    public string[] GetFiles(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly
    )
    {
        return _fileSystem
            ._files.Keys.Where(f => f.StartsWith(path))
            .Where(f => searchPattern == "*" || f.EndsWith(searchPattern))
            .ToArray();
    }

    public string[] GetDirectories(string path)
    {
        return _fileSystem._directories.Where(d => d.StartsWith(path) && d != path).ToArray();
    }

    public void Delete(string path, bool recursive = false)
    {
        if (recursive)
        {
            var directoriesToDelete = _fileSystem._directories.Where(d => d.StartsWith(path)).ToList();

            foreach (var dir in directoriesToDelete)
            {
                _fileSystem._directories.Remove(dir);
            }
        }
        else
        {
            _fileSystem._directories.Remove(path);
        }
    }
}

internal class MockPath : IPath
{
    public string Combine(string path1, string path2)
    {
        if (string.IsNullOrEmpty(path1))
            return path2;
        if (string.IsNullOrEmpty(path2))
            return path1;

        var separator =
            path1.EndsWith(Path.DirectorySeparatorChar.ToString())
            || path2.StartsWith(Path.DirectorySeparatorChar.ToString())
                ? ""
                : Path.DirectorySeparatorChar.ToString();

        return path1 + separator + path2;
    }

    public string GetDirectoryName(string path)
    {
        var lastSeparator = Math.Max(
            path.LastIndexOf(Path.DirectorySeparatorChar),
            path.LastIndexOf(Path.AltDirectorySeparatorChar)
        );

        if (lastSeparator >= 0)
        {
            return path.Substring(0, lastSeparator);
        }

        return string.Empty;
    }

    public string GetFileName(string path)
    {
        var lastSeparator = Math.Max(
            path.LastIndexOf(Path.DirectorySeparatorChar),
            path.LastIndexOf(Path.AltDirectorySeparatorChar)
        );

        if (lastSeparator >= 0)
        {
            return path.Substring(lastSeparator + 1);
        }

        return path;
    }

    public string GetExtension(string path)
    {
        var fileName = GetFileName(path);
        var lastDot = fileName.LastIndexOf('.');

        if (lastDot >= 0)
        {
            return fileName.Substring(lastDot);
        }

        return string.Empty;
    }

    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;
    public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;
    public char VolumeSeparatorChar => Path.VolumeSeparatorChar;
}

using System.Runtime.CompilerServices;

namespace iSukces.Templates.Builder;

internal class CsMerge(string[] header)
{
    private static IEnumerable<string> GetNamespaces()
    {
        yield return "System.Collections.Generic";
        yield return "System.Linq";
        yield return "Microsoft.VisualStudio.TextTemplating";
    }

    private static string GetProjectPath(string projectFileName, [CallerFilePath] string? path = null)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentNullException(nameof(path));
        var dir = new FileInfo(path).Directory;
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, projectFileName)))
                return dir.FullName;
            dir = dir.Parent;
        }

        throw new Exception($"Cannot find project file {projectFileName}");
    }


    private IEnumerable<string> GetLines()
    {
        foreach (var ns in GetNamespaces().OrderBy(a => a))
            yield return $"<#@ import namespace=\"{ns}\" #>";
        foreach (var headerLine in header) yield return "// " + headerLine;

        yield return "<#+";
        yield return "#nullable enable";
        yield return "";

        foreach (var src in _sources)
        {
            var lines = new CsFileReader().GetLines(src);
            foreach (var line in lines)
                yield return line;
        }

        yield return "#>";
    }


    public CsMerge WithProject(string projectFileName)
    {
        _directory = GetProjectPath(projectFileName);
        return this;
    }

    public CsMerge WithSource(string filename, Fla flags = Fla.None)
    {
        _sources.Add(new CsFile(Path.Combine(_directory, filename), flags));
        return this;
    }


    public void WriteTo(string outputFilename)
    {
        var lines = GetLines().ToArray();
        outputFilename = Path.Combine(_directory, outputFilename);
        new FileInfo(outputFilename).Directory?.Create();
        File.WriteAllLines(outputFilename, lines);
    }

    private string _directory;
    private readonly List<CsFile> _sources = new();
}

[Flags]
public enum Fla
{
    None,
    IgnoreAllEmptyLines = 1
}

internal record struct CsFile(string Filename, Fla Flags);

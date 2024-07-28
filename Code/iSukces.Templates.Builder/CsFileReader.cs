using System.Text.RegularExpressions;

namespace iSukces.Templates.Builder;

internal class CsFileReader
{
    private static bool SkipDirectives(string line)
    {
        var m = DirectiveRegex.Match(line);
        if (!m.Success) return false;
        var code = m.Groups[1].Value;
        return code is "region" or "endregion";
    }

    public IEnumerable<string> GetLines(CsFile file)
    {
        var lines = File.ReadAllLines(file.Filename);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                if (file.Flags.HasFlag(Fla.IgnoreAllEmptyLines))
                    continue;
                if (_canSendEmptyLines)
                    _sendEmptLine = true;
                continue;
            }

            if (_isInHeader)
            {
                if (UsingOrNamespaceRegex.IsMatch(line))
                    continue;
                _isInHeader = false;
            }

            if (SkipDirectives(line))
                continue;
            {
                var m = DirectiveRegex.Match(line);
                if (m.Success)
                {
                    var code = m.Groups[1].Value;
                    if (code is "region" or "endregion")
                        continue;
                }
            }

            if (_sendEmptLine)
            {
                _sendEmptLine = false;
                yield return "";
            }

            _canSendEmptyLines = true;
            yield return line;
        }

        yield return "";
    }

    #region Fields

    private const           string UsingOrNamespaceFilter = @"^\s*(using|namespace)\s+[^;]+\s*;";
    private const           string DirectiveFilter        = @"^\s*#([^\s]+)\s*(.*)$";
    private static readonly Regex  UsingOrNamespaceRegex  = new(UsingOrNamespaceFilter, RegexOptions.Compiled);
    private static readonly Regex  DirectiveRegex         = new(DirectiveFilter, RegexOptions.Compiled);
    private                 bool   _canSendEmptyLines;


    private bool _isInHeader = true;
    private bool _sendEmptLine;

    #endregion
}
namespace iSukces.Templates.Builder.Common;

public class CodeWriterBase
{
    protected void Close(bool addNl, string append = "")
    {
        DecIndent();
        WriteLine("}" + append);
        if (addNl)
            WriteLine();
    }

    public CodeWriterBase DecIndent()
    {
        if (_indent < 1)
            throw new InvalidOperationException("Indent is already 0");
        _indent--;
        return UpdateIndent();
    }


    public CodeWriterBase IncIndent()
    {
        _indent++;
        return UpdateIndent();
    }

    protected CodeWriterBase Open(string x)
    {
        if (!string.IsNullOrEmpty(x))
            WriteLine(x);
        WriteLine("{");
        IncIndent();
        return this;
    }

    protected void CheckArgumentNullOrEmpty(string argumentName)
    {
        SingleLineIf($"string.IsNullOrEmpty({argumentName})", 
            "throw new ArgumentException(\"\", nameof("+argumentName+"));");

    }

    protected CodeWriterBase SingleLineIf(string condition, string statement)
    {
        WriteLine("if (" + condition + ")");
        IncIndent();
        WriteLine(statement);
        DecIndent();
        return this;
    }

    private CodeWriterBase UpdateIndent()
    {
        IndentString = _indent > 0 ? new string(' ', _indent * 4) : "";
        return this;
    }

    protected void WriteCaseExpressionStatement(string open, IEnumerable<CaseExpressionItem> get1, string appendClose)
    {
        Open(open);
        var lines = get1.ToArray();
        for (var index = 0; index < lines.Length; index++)
        {
            var item  = lines[index];
            var comma = index == lines.Length - 1 ? "" : ",";
            WriteLine($"{item.Condition} => {item.Value}{comma}");
        }

        Close(false, appendClose);
    }

    public CodeWriterBase WriteLine(string text = "")
    {
        if (string.IsNullOrEmpty(text))
            Output!.WriteLine("");
        else
            Output!.WriteLine(IndentString + text);
        return this;
    }


    private string IndentString { get; set; } = string.Empty;

    #region Fields

    private int _indent;

    protected TextTransformation? Output;

    #endregion
}
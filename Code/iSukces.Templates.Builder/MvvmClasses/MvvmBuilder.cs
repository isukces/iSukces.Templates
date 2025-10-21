using iSukces.Templates.Builder.Common;

namespace iSukces.Templates.Builder.MvvmClasses;

public class MvvmBuilder : CodeWriterBase
{
    private IEnumerable<string> GetDeclarationItem()
    {
        yield return Visibility;
        if (IsPartial)
            yield return "partial";
        yield return "class";
        if (string.IsNullOrEmpty(GenericType))
            yield return ClassName;
        else
            yield return ClassName + "<" + GenericType + ">";
    }

    private Context Prepare()
    {
        var c = new Context();
        if (Features.HasFlag(Features.INotifyPropertyChanged))
        {
            c.AddUsing("System.ComponentModel");
            c.AddUsing("System.Runtime.CompilerServices");
            c.AddEvent(() =>
            {
                WriteLine("public event PropertyChangedEventHandler? PropertyChanged;");
            });
            c.AddMethod("OnPropertyChanged", () =>
            {
                Open("protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)");
                WriteLine("PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));");
                Close(false);
            });

            c.AddMethod("SetAndNotify", () =>
            {
                Open(
                    "protected bool SetAndNotify<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)");
                WriteLine("if (EqualityComparer<T>.Default.Equals(field, value)) return false;");
                WriteLine("field = value;");
                WriteLine("OnPropertyChanged(propertyName);");
                if (!string.IsNullOrEmpty(AfterOnPropertyChanged))
                    WriteLine(AfterOnPropertyChanged);
                WriteLine("return true;");
                Close(false);
            });
        }

        foreach (var i in Properties)
        {
            var pi = new Member("", () =>
            {
                var fieldName = "_" + FirstLower(i.Name);
                Open($"public {i.Type} {i.Name}");
                WriteLine($"get => {fieldName};");
                WriteLine($"set => SetAndNotify(ref {fieldName}, value);");
                Close(true);
                var code = "private " + i.Type + " " + fieldName;
                if (!string.IsNullOrEmpty(i.InitValueExpression))
                    code += " = " + i.InitValueExpression;
                WriteLine(code + ';');
            }, MemberType.Property);
            c.Add(pi);
        }

        return c;
    }

    private static string FirstLower(string x) => x[0].ToString().ToLower() + x.Substring(1);

    public MvvmBuilder WithProperty(string type, string name, string initValueExpression = "")
    {
        Properties.Add(new Property(type, name, initValueExpression));
        return this;
    }

    public void Write(TextTransformation p)
    {
        Output = p;

        var c = Prepare();

        WriteLine("// hello");
        foreach (var u in c.GetUsings())
            WriteLine("using " + u + ";");
        WriteLine();
        WriteLine($"namespace {Namespace};");
        WriteLine();
        Open(Declaration);

        foreach (var m in c.GetMembers())
        {
            m.Write();
            WriteLine();
        }

        Close(false);
    }

    #region Properties

    public Features Features { get; set; }

    public List<Property> Properties { get; } = new();

    public string? Namespace   { get; set; }
    public string  ClassName   { get; set; } = "MyClass";
    public string? GenericType { get; set; }
    public string? Visibility  { get; set; }
    public bool    IsPartial   { get; set; }

    public string AfterOnPropertyChanged { get; set; } = "";

    public string Declaration
    {
        get { return string.Join(" ", GetDeclarationItem().Where(a => !string.IsNullOrEmpty(a))); }
    }

    #endregion

    private sealed class Member(string name, Action write, MemberType kind)
    {
        #region Properties

        public string     Name  { get; } = name;
        public Action     Write { get; } = write;
        public MemberType Kind  { get; } = kind;

        #endregion
    }

    private enum MemberType
    {
        Method,
        Property,
        Event
    }

    private sealed class Context
    {
        public void Add(Member member)
        {
            _members.Add(member);
        }

        public void AddEvent(Action action)
        {
            _members.Add(new Member("", action, MemberType.Event));
        }

        public void AddMethod(string name, Action action)
        {
            _members.Add(new Member(name, action, MemberType.Method));
        }

        public void AddUsing(string ns)
        {
            _usings.Add(ns);
        }

        public IEnumerable<Member> GetMembers()
        {
            return _members
                .OrderBy(a => a.Kind)
                .ThenBy(a => a.Name);
        }

        public IEnumerable<string> GetUsings()
        {
            return _usings.OrderBy(a => a);
        }

        #region Fields

        private readonly HashSet<string> _usings = [];
        private readonly List<Member> _members = [];

        #endregion
    }
}

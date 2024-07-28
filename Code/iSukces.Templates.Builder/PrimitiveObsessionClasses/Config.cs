namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;

public static class Config
{
    public static string          Namespace       { get; set; } = "NoNamespace";
    public static Features        IgnoreFeatures  { get; set; }
    public static Features        IncludeFeatures { get; set; }
    public static HashSet<string> ImplicitUsings  { get; } = new();
}
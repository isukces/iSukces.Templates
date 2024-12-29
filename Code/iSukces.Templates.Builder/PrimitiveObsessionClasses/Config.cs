namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;

public static class Config
{
    public static string          Namespace       { get; set; } = "NoNamespace";
    public static Features        IgnoreFeatures  { get; set; }
    public static Features        IncludeFeatures { get; set; }
    public static HashSet<string> ImplicitUsings  { get; } = new();

    /// <summary>
    ///     Default conversion to primitive type
    /// </summary>
    public static TypeConversion ConvertToPrimitive { get; set; } = TypeConversion.Explicit;

    /// <summary>
    ///     Default conversion from primitive type
    /// </summary>
    public static TypeConversion ConvertFromPrimitive { get; set; } = TypeConversion.Explicit;

    /// <summary>
    ///     Default conversion from primitive type
    /// </summary>
    public static string? ToStringExpression { get; set; } 
}
namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;

[Flags]
public enum Features
{
    None                            = 0,
    PrimitiveWrapper                = 1,
    Comparable                      = 2,
    ComparablePrimitive             = 4,
    EquatablePrimitive              = 8,
    NewtonsoftJsonSerializer        = 16,
    Parse                           = 32,
    ImplicitConversionToPrimitive   = 64,
    ImplicitConversionFromPrimitive = 128,
    RelativeOperators               = 256,
    EqualityOperators               = 512,
    All                             = 1024 - 1
}
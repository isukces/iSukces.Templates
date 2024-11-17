namespace iSukces.Templates.Builder.PrimitiveObsessionClasses;



[Flags]
public enum Features
{
    None                            = 0,
    PrimitiveWrapper                = 1,
    Comparable                      = 2,
    ComparablePrimitive             = 4,
    EquatablePrimitive              = 8,
    ImplicitConversionToPrimitive   = 16,
    ImplicitConversionFromPrimitive = 32,
    RelativeOperators               = 64,
    EqualityOperators               = 128,
    ComparableObject                = 256,
    NewtonsoftJsonSerializer        = 512,
    Parse                           = 1024,
    All                             = 2047
}

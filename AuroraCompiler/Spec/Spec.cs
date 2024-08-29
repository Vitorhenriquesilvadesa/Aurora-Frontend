namespace AuroraCompiler.Spec;

using Assertion;

public class Spec
{
    public static readonly uint Enabled = 1u;
    public static readonly uint Disabled = 0u;

    public uint Bit { get; private set; }

    private Spec(uint bit)
    {
        Bit = bit;
    }

    public static implicit operator Spec(uint bit)
    {
        Assertion.Check(bit is 0 or 1, "Cannot instantiate flag with non-binary value.");
        return new Spec(bit);
    }

    public static implicit operator uint(Spec spec)
    {
        return spec.Bit;
    }
}
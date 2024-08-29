namespace AuroraCompiler.Types;

public class NullLiteral
{
    private NullLiteral()
    {
    }

    public static NullLiteral Instance { get; private set; }
}
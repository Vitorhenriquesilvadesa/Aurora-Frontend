namespace AuroraCompiler.Assertion;

public static class Assertion
{
    public static void Check(bool condition, string message)
    {
        if (!condition)
        {
            throw new AssertionFailException(message);
        }
    }
}
namespace Nightstorm.Core.Exceptions;

/// <summary>
/// Exception thrown when a game rule is violated.
/// </summary>
public class GameRuleViolationException : Exception
{
    public GameRuleViolationException()
    {
    }

    public GameRuleViolationException(string message) : base(message)
    {
    }

    public GameRuleViolationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

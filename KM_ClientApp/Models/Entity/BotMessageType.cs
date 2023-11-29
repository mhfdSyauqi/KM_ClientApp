namespace KM_ClientApp.Models.Entity;

public record BotMessageType(string value)
{
    public static readonly BotMessageType CLOSING = new("CLOSING");
    public static readonly BotMessageType FEEDBACK = new("FEEDBACK");
    public static readonly BotMessageType IDLE = new("IDLE");
    public static readonly BotMessageType LAYER_ONE = new("LAYER_ONE");
    public static readonly BotMessageType LAYER_THREE = new("LAYER_THREE");
    public static readonly BotMessageType LAYER_TWO = new("LAYER_TWO");
    public static readonly BotMessageType MAIL_SENDED = new("MAIL_SENDED");
    public static readonly BotMessageType NOT_FOUND = new("NOT_FOUND");
    public static readonly BotMessageType REASKED = new("REASKED");
    public static readonly BotMessageType SEARCHED = new("SEARCHED");
    public static readonly BotMessageType SOLVED = new("SOLVED");
    public static readonly BotMessageType SUGGESTION = new("SUGGESTION");
    public static readonly BotMessageType WELCOME = new("WELCOME");
}

namespace MES.Presentation.UI.Messages;

/// <summary>
/// A simple courier object to send commands from the Header to the active Screen.
/// </summary>
public class HeaderActionMessage
{
    // What happened? ("Add", "Edit", "Delete", "Refresh", "Print")
    public string ActionType { get; }

    public HeaderActionMessage(string actionType)
    {
        ActionType = actionType;
    }
}
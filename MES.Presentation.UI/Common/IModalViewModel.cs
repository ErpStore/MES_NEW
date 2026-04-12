namespace MES.Presentation.UI.Common;

public interface IModalViewModel
{
    /// <summary>
    /// Any ViewModel that wants to be a Dialog MUST have this property
    /// </summary>
    Action<bool> CloseAction { get; set; }
}
namespace MES.Presentation.UI.Common
{
    public interface IViewModelFactory
    {
        /// <summary>
        /// Creates a fresh instance of the requested ViewModel type
        /// </summary>

        T Create<T>() where T : BaseViewModel;
    }
}

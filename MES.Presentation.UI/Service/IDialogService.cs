namespace MES.Presentation.UI.Service
{
    public interface IDialogService
    {
        /// <summary>
        /// 1. For complex forms (User Edit, Order Edit)
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        bool? ShowDialog<TViewModel>(TViewModel viewModel);

        /// <summary>
        /// 2. For simple alerts (Success, Error, Warning)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        void ShowMessage(string message, string title);

        /// <summary>
        /// 3. For simple questions (Yes/No)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        bool ShowConfirmation(string message, string title);
    }
}

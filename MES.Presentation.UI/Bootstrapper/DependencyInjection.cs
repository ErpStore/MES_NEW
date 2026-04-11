using MES.ApplicationLayer.User.Handlers;
using MES.Infrastructure.Data;
using MES.Presentation.UI.Common;
using MES.Presentation.UI.Controls;
using MES.Presentation.UI.Modules.Materials.ViewModel;
using MES.Presentation.UI.Modules.Materials.ViewModels;
using MES.Presentation.UI.Modules.Materials.Views;
using MES.Presentation.UI.Modules.Recipe.ViewModel;
using MES.Presentation.UI.Modules.Recipe.Views;
using MES.Presentation.UI.Modules.Recipe;
using MES.Presentation.UI.Modules.UserManagement.ViewModels;
using MES.Presentation.UI.Modules.UserManagement.Views;
using MES.Presentation.UI.Navigation;
using MES.Presentation.UI.Service;
using MES.Presentation.UI.Shell;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NavigationService = MES.Presentation.UI.Navigation.NavigationService;
using MES.Presentation.UI.Modules.Order.ViewModel;
using MES.Presentation.UI.Modules.Order.Views;

namespace MES.Presentation.UI.Bootstrapper
{
    public static class DependencyInjection
    {
        const string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MES_Trikala_DB;Trusted_Connection=True;MultipleActiveResultSets=true";

        public static IServiceCollection AddPresentationUI(this IServiceCollection services) // Change IServiceContainer to IServiceCollection
        {
            // Register UI services and view models here
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<ShellViewModel>();

            services.AddLogging(configure =>
            {
                configure.AddDebug();   // Logs to Visual Studio Output window
                configure.AddConsole(); // Logs to Console window (if available)
            });

            // ===== Session Service =====
            services.AddSingleton<ICurrentUserService, CurrentUserService>();

            // ===== Controls =====
            services.AddSingleton<HeaderBarViewModel>();
            services.AddSingleton<SideMenuViewModel>();

            // ===== Navigation =====
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IViewModelFactory, ViewModelFactory>();
            services.AddSingleton<IDialogService, DialogService>();

            // 2. Register ViewModels (Transient = New one every time)
            services.AddTransient<UsersViewModel>();
            services.AddTransient<UsersListViewModel>();
            services.AddTransient<UserEditViewModel>();
            services.AddTransient<UserListDepartmentsViewModel>();
            services.AddTransient<UserGroupEditViewModel>();
            services.AddTransient<UserGroupRightsViewModel>();
            services.AddTransient<UserRightsViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MaterialGroupListViewModel>();
            services.AddTransient<MaterialManagementListViewModel>();
            services.AddTransient<MaterialsViewModel>();
            services.AddTransient<MaterialManagementEditViewModel>();
            services.AddTransient<MaterialManagementEditView>();
            services.AddTransient<MaterialGroupEditViewModel>();
            services.AddTransient<FeedingPathEditViewModel>();
            services.AddTransient<FeedingPathListViewModel>();
            services.AddTransient<FeedingPathEditView>();
            services.AddTransient<RecipeListViewModel>();
            services.AddTransient<RecipeEditViewModel>();
            services.AddTransient<RecipeEditView>();
            services.AddTransient<RecipesViewModel>();
            services.AddTransient<RecipesView>();
            services.AddTransient<RecipeProcessListViewModel>();
            services.AddTransient<RecipeProcessEditViewModel>();
            services.AddTransient<RecipeProcessEditView>();
            services.AddTransient<OrderManagementListViewModel>();
            services.AddTransient<OrderManagementEditViewModel>();
            services.AddTransient<OrderManagementEditView>();
            services.AddTransient<OrderManagementViewModel>();
            services.AddTransient<OrdersView>();

            // 3. MAP VIEWMODELS TO VIEWS
            DialogService.RegisterDialog<UserEditViewModel, UserEditView>();
            DialogService.RegisterDialog<UserGroupEditViewModel, UserGroupEditView>();
            DialogService.RegisterDialog<UserGroupRightsViewModel, UserGroupRightsView>();
            DialogService.RegisterDialog<LoginViewModel, LoginView>();
            DialogService.RegisterDialog<MaterialGroupEditViewModel, MaterialGroupEditView>();
            DialogService.RegisterDialog<MaterialManagementEditViewModel, MaterialManagementEditView>();
            DialogService.RegisterDialog<FeedingPathEditViewModel, FeedingPathEditView>();
            DialogService.RegisterDialog<RecipeEditViewModel, RecipeEditView>();
            DialogService.RegisterDialog<RecipeProcessEditViewModel, RecipeProcessEditView>();
            DialogService.RegisterDialog<OrderManagementEditViewModel, OrderManagementEditView>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SaveUserHandler).Assembly));
            services.AddDbContext<MesDbContext>(options =>
            {
                options.UseSqlServer(ConnectionString);
            }, ServiceLifetime.Transient);
            return services;
        }
    }
}

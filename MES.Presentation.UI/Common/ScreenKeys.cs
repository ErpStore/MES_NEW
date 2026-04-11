namespace MES.Presentation.UI.Common
{
    /// <summary>
    /// Constant screen key strings used to identify each screen in the user rights system.
    /// These keys must match the ScreenKey values stored in the UserGroupRights table.
    /// </summary>
    public static class ScreenKeys
    {
        public const string Overview = "Overview";
        public const string Users = "Users";
        public const string UserGroups = "UserGroups";
        public const string MaterialGroup = "MaterialGroup";
        public const string MaterialManagement = "MaterialManagement";
        public const string FeedingPath = "FeedingPath";
        public const string RecipeManagement = "RecipeManagement";
        public const string RecipeProcess = "RecipeProcess";
        public const string OrderManagement = "OrderManagement";
    }

    /// <summary>
    /// Application-wide constants.
    /// </summary>
    public static class AppConstants
    {
        /// <summary>
        /// The user ID of the built-in administrator account that has full system access.
        /// </summary>
        public const int AdminUserId = 1;
    }
}

using System.ComponentModel;

namespace hoohub.Enums
{
	public enum EventTypes
	{
        [Description("")]
        AppStarted,

        [Description("")]
		Warning,

		[Description("")]
		Error,

        [Description("")]
        UserCreated,

        [Description("")]
		UserLoggedIn,

		[Description("")]
		UserLoggedOut,

		[Description("")]
		UserPasswordReset,

		[Description("")]
		UserLockedOut,

		[Description("")]
		ComicCreated,

		[Description("")]
		ComicUpdated,

		[Description("")]
		ComicDeleted
	}
}

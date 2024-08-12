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
        UserLoginAttempt,

        [Description("")]
		UserLoggedIn,

		[Description("")]
		UserLoggedOut,

		[Description("")]
		UserPasswordReset,

		[Description("")]
		UserLockedOut,

        [Description("")]
        UserPasswordResetRequested,

        [Description("")]
        TwoFactorChallengeIssued,

		[Description("")]
        TwoFactorCodeIssued,

        [Description("")]
		ComicCreated,

		[Description("")]
		ComicUpdated,

		[Description("")]
		ComicDeleted
	}
}

using System.ComponentModel;

namespace hoohub.Enums
{
    /// <summary>
    /// Custom event types tracked by HooHub.
    /// </summary>
    public enum EventTypes
	{
        [Description("App started")]
        AppStarted,

        [Description("Warning")]
		Warning,

		[Description("Error")]
		Error,

        [Description("User created")]
        UserCreated,

        [Description("User login attempt")]
        UserLoginAttempt,

        [Description("User logged in")]
		UserLoggedIn,

		[Description("User logged out")]
		UserLoggedOut,

		[Description("User password reset")]
		UserPasswordReset,

		[Description("User locked out")]
		UserLockedOut,

        [Description("User password reset requested")]
        UserPasswordResetRequested,

        [Description("User profile updated")]
        UserUpdated,

        [Description("Two factor challenge issued")]
        TwoFactorChallengeIssued,

		[Description("Two factor code issued")]
        TwoFactorCodeIssued,

        [Description("Comic created")]
		ComicCreated,

		[Description("Comic updated")]
		ComicUpdated,

		[Description("Comic deleted")]
		ComicDeleted,

		[Description("Comic released")]
		ComicReleased,

		[Description("Maintenance")]
        Maintenance,

        [Description("Email sent")]
        EmailSent,

        [Description("Unknown")]
        Unknown,

        [Description("Comic hearted")]
        ComicLikeCreated,

        [Description("Comic unhearted")]
        ComicLikeDeleted
    }
}

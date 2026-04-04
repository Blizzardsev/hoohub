#pragma warning disable CS8602
#pragma warning disable CS8604

using hoohub.Configuration;
using hoohub.Data;
using hoohub.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUglify.Helpers;

namespace hoohub.Services
{
    /// <summary>
    /// Handles all background activity and sync operations with Jira.
    /// </summary>
    public class AppBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Initialises a new instance of the <see cref="AppBackgroundService"/> class.
        /// </summary>
        /// <param name="scopeFactory"><see cref="IServiceScopeFactory"/> for providing the scope to access other services.</param>
        public AppBackgroundService(IServiceScopeFactory scopeFactory) : base()
        {
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Ongoing app background processing; handles Jira issue sync operations and clearing stale data.
        /// </summary>
        /// <param name="stoppingToken"><see cref="CancellationToken"/> for stopping the process.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var _scope = _scopeFactory.CreateScope();
                var _hooContext = _scope.ServiceProvider.GetRequiredService<HooHubContext>();
                var _appSettings = _scope.ServiceProvider.GetService<AppSettings>();
                var _userManager = _scope.ServiceProvider.GetService<UserManager<HooHubUser>>();
                var _smtpService = _scope.ServiceProvider.GetService<SmtpService>();

                try
                {
                    // Remove stale application event lines
                    var eventLinesToRemove = _hooContext.Events.AsEnumerable().Where(eventItem => eventItem.GetAge().TotalDays >= 30);
                    if (eventLinesToRemove.Any())
                    {
                        _hooContext.Events.RemoveRange(eventLinesToRemove);
                        await _hooContext.Events.AddAsync(new Event(
                            eventType: EventTypes.Maintenance,
                            details: $"{eventLinesToRemove.Count()} stale application event {(eventLinesToRemove.Count() == 1 ? "line was" : "lines were")} deleted."));
                    }

                    // Publish any scheduled comics
                    var comicsToPublish = _hooContext.Comics
                        .Where(comic => comic.PublishDate <= DateTime.UtcNow && comic.IsHidden)
                        .AsEnumerable();
                    comicsToPublish.ForEach(async comic =>
                    {
                        comic.IsHidden = false;
                        comic.PublishDate = DateTime.UtcNow;
                        comic.ScheduledDate = null;
                        await _hooContext.Events.AddAsync(new Event(
                            eventType: EventTypes.ComicReleased,
                            details: $"Comic GUID {comic.Id} ({comic.GetComicDisplayName()}) met scheduled date and was released."));
                    });

                    // Make sure the blacklist stays manageable
                    _appSettings.BlockedIpAddressRange.RemoveAll(blockedIpAddress => (DateTimeOffset.Now - blockedIpAddress.BlockedDate).TotalDays >= 1);
                }
                catch (Exception maintenanceException)
                {
                    await _hooContext.Events.AddAsync(new Event(
                        eventType: EventTypes.Error,
                        details: $"One or more maintenance tasks failed: {maintenanceException.Message}",
                        stackTrace: JsonConvert.SerializeObject(value: maintenanceException.StackTrace, formatting: Formatting.Indented)));

                    var adminUser = await _hooContext.Users.SingleOrDefaultAsync(user => string.Equals(user.Email, _appSettings.SiteAdmin, StringComparison.InvariantCultureIgnoreCase));
                    if (adminUser != null)
                    {
                        try
                        {
                            _smtpService.SendEmail(
                                name: adminUser.Handle,
                                address: adminUser.Email,
                                subject: "One or more errors have occurred",
                                body: TemplateService.GetTemplateSubstitutions(
                                    template: Properties.Resources.ErrorsTemplate,
                                    substitutions: new Dictionary<string, string>()
                                    {
                                        { "{text}", $"One or more errors occurred while performing maintenance tasks at {FormattingService.GetDateTimeAsString(DateTime.UtcNow)}: check logs." },
                                        { "{buttonUrl}", $"{_appSettings.SiteBaseUrl}/Manage" }
                                    }));
                        }
                        catch (Exception emailException)
                        {
                            await _hooContext.Events.AddAsync(new Event(
                                eventType: EventTypes.Error,
                                details: $"Failed to send email to {adminUser.Email}: {emailException.Message}",
                                stackTrace: JsonConvert.SerializeObject(value: emailException.StackTrace, formatting: Formatting.Indented)));
                        }
                    }
                }

                await _hooContext.SaveChangesAsync();
                await Task.Delay(300 * 1000, stoppingToken);
            }
        }
    }
}
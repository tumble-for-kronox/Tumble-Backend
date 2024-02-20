using KronoxAPI.Controller;
using TumbleHttpClient;

namespace KronoxAPI.Model.Users;

public class AvailableUserEvent : UserEvent
{
    public string? Id { get; }

    private string? ParticipatorId { get; }

    private string? SupportId { get; }

    public string AnonymousCode { get; }

    public bool IsRegistered { get; }

    public bool SupportAvailable { get; }

    public bool RequiresChoosingLocation { get; }

    public DateTime LastSignupDate { get; }

    /// <summary>
    /// For use as default or in case an event is not found/can't be parsed.
    /// </summary>
    /// <returns><see cref="AvailableUserEvent"/> wiht all values set as "N/A".</returns>
    public static AvailableUserEvent NotAvailable => new("N/A", "N/A", DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, "N/A", null, null, "N/A", false, false, false);

    public AvailableUserEvent(string title, string type, DateTime lastSignupDate, DateTime eventStart, DateTime eventEnd, string? id, string? participatorId, string? supportId, string anonymousCode, bool isRegistered, bool supportAvailable, bool requiresChoosingLocation) : base(title, type, eventStart, eventEnd)
    {
        Id = id;
        ParticipatorId = participatorId;
        SupportId = supportId;
        AnonymousCode = anonymousCode;
        SupportAvailable = supportAvailable;
        IsRegistered = isRegistered;
        LastSignupDate = lastSignupDate;
        RequiresChoosingLocation = requiresChoosingLocation;
    }

    /// <summary>
    /// Register a given user based on <paramref name="userSessionToken"/> to a given event.
    /// </summary>
    /// <returns>A <see cref="bool"/> describing if the registering was successful.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<bool> Register(IKronoxRequestClient client)
    {
        if (string.IsNullOrEmpty(Id)) throw new NullReferenceException("_id cannot be null or an empty string when registering.");

        return await KronoxPushController.UserEventRegisterAsync(client , Id);
    }

    /// <summary>
    /// Unregister a given user based on <paramref name="userSessionToken"/> from a given event.
    /// </summary>
    /// <returns>A <see cref="bool"/> describing if the unregistering was successful.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<bool> Unregister(IKronoxRequestClient client)
    {
        if (string.IsNullOrEmpty(Id)) throw new NullReferenceException("_id cannot be null or an empty string when unregistering.");

        return await KronoxPushController.UserEventUnregisterAsync(client, Id);
    }

    /// <summary>
    /// Add support to a given event, on behalf of a given user based on <paramref name="userSessionToken"/>.
    /// </summary>
    /// <returns>A <see cref="bool"/> describing if the support was successfully added.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<bool> AddSupport(KronoxRequestClient client)
    {
        if (string.IsNullOrEmpty(SupportId)) throw new NullReferenceException("supportId cannot be null or an empty string when adding support.");

        if (string.IsNullOrEmpty(ParticipatorId)) throw new NullReferenceException("participatorId cannot be null or an empty string when adding support.");

        return await KronoxPushController.UserEventAddSupportAsync(client, ParticipatorId, SupportId);
    }

    /// <summary>
    /// Remove support to a given event, on behalf of a given user based on <paramref name="userSessionToken"/>.
    /// </summary>
    /// <returns>A <see cref="bool"/> describing if the support was successfully removed.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<bool> RemoveSupport(KronoxRequestClient client)
    {
        if (string.IsNullOrEmpty(Id)) throw new NullReferenceException("id cannot be null or an empty string when removing support.");

        if (string.IsNullOrEmpty(SupportId)) throw new NullReferenceException("supportId cannot be null or an empty string when removing support.");

        if (string.IsNullOrEmpty(ParticipatorId)) throw new NullReferenceException("participatorId cannot be null or an empty string when removing support.");

        return await KronoxPushController.UserEventRemoveSupportAsync(client, Id, ParticipatorId, SupportId);
    }
}

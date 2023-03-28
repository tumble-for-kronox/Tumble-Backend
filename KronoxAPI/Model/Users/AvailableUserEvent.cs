using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KronoxAPI.Model.Schools;
using KronoxAPI.Controller;

namespace KronoxAPI.Model.Users;

public class AvailableUserEvent : UserEvent
{
    private readonly string _id;
    private readonly string? _participatorId;
    private readonly string? _supportId;
    private readonly string _anonymousCode;
    private readonly DateTime _lastSignupDate;

    private readonly bool _isRegistered;
    private readonly bool _supportAvailable;
    private readonly bool _requiresChoosingLocation;
    public string Id => _id;

    public string? ParticipatorId => _participatorId;

    public string? SupportId => _supportId;

    public string AnonymousCode => _anonymousCode;

    public bool IsRegistered => _isRegistered;

    public bool SupportAvailable => _supportAvailable;

    public bool RequiresChoosingLocation => _requiresChoosingLocation;

    public DateTime LastSignupDate => _lastSignupDate;

    /// <summary>
    /// For use as default or in case an event is not found/can't be parsed.
    /// </summary>
    /// <returns><see cref="AvailableUserEvent"/> wiht all values set as "N/A".</returns>
    public static AvailableUserEvent NotAvailable => new("N/A", "N/A", DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, "N/A", null, null, "N/A", false, false, false);

    public AvailableUserEvent(string title, string type, DateTime lastSignupDate, DateTime eventStart, DateTime eventEnd, string id, string? participatorId, string? supportId, string anonymousCode, bool isRegistered, bool supportAvailable, bool requiresChoosingLocation) : base(title, type, eventStart, eventEnd)
    {
        _id = id;
        _participatorId = participatorId;
        _supportId = supportId;
        _anonymousCode = anonymousCode;
        _supportAvailable = supportAvailable;
        _isRegistered = isRegistered;
        _lastSignupDate = lastSignupDate;
        _requiresChoosingLocation = requiresChoosingLocation;
    }

    /// <summary>
    /// Register a given user based on <paramref name="userSessionToken"/> to a given event.
    /// </summary>
    /// <param name="school"></param>
    /// <param name="userSessionToken"></param>
    /// <returns>A <see cref="bool"/> describing if the registering was successful.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<bool> Register(School school, string userSessionToken)
    {
        if (_id == null || _id == string.Empty) throw new NullReferenceException("_id cannot be null or an empty string when registering.");

        return await KronoxPushController.UserEventRegister(school, userSessionToken, Id);
    }

    /// <summary>
    /// Unregister a given user based on <paramref name="userSessionToken"/> from a given event.
    /// </summary>
    /// <param name="school"></param>
    /// <param name="userSessionToken"></param>
    /// <returns>A <see cref="bool"/> describing if the unregistering was successful.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<bool> Unregister(School school, string userSessionToken)
    {
        if (_id == null || _id == string.Empty) throw new NullReferenceException("_id cannot be null or an empty string when unregistering.");

        return await KronoxPushController.UserEventUnregister(school, userSessionToken, Id);
    }

    /// <summary>
    /// Add support to a given event, on behalf of a given user based on <paramref name="userSessionToken"/>.
    /// </summary>
    /// <param name="school"></param>
    /// <param name="userSessionToken"></param>
    /// <returns>A <see cref="bool"/> describing if the support was successfully added.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<bool> AddSupport(School school, string userSessionToken)
    {
        if (_supportId == null || _supportId == string.Empty) throw new NullReferenceException("supportId cannot be null or an empty string when adding support.");

        if (_participatorId == null || _participatorId == string.Empty) throw new NullReferenceException("participatorId cannot be null or an empty string when adding support.");

        return await KronoxPushController.UserEventAddSupport(school, userSessionToken, _participatorId, _supportId);
    }

    /// <summary>
    /// Remove support to a given event, on behalf of a given user based on <paramref name="userSessionToken"/>.
    /// </summary>
    /// <param name="school"></param>
    /// <param name="userSessionToken"></param>
    /// <returns>A <see cref="bool"/> describing if the support was successfully removed.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task<bool> RemoveSupport(School school, string userSessionToken)
    {
        if (_id == null || _id == string.Empty) throw new NullReferenceException("id cannot be null or an empty string when removing support.");

        if (_supportId == null || _supportId == string.Empty) throw new NullReferenceException("supportId cannot be null or an empty string when removing support.");

        if (_participatorId == null || _participatorId == string.Empty) throw new NullReferenceException("participatorId cannot be null or an empty string when removing support.");

        return await KronoxPushController.UserEventRemoveSupport(school, userSessionToken, _id, _participatorId, _supportId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.Modules.Events
{
    using DotNetNuke.Entities.Modules.Settings;
    using DotNetNuke.Modules.Events;

    /// <summary>
    ///     The <see cref="SettingsRepository{T}" /> used for storing and retrieving <see cref="EventModuleSettings" />
    /// </summary>
    public class EventModuleSettingsRepository : SettingsRepository<EventModuleSettings>
    { }
}
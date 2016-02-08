#region Copyright (C) 2007-2015 Team MediaPortal

/*
    Copyright (C) 2007-2015 Team MediaPortal
    http://www.team-mediaportal.com

    This file is part of MediaPortal 2

    MediaPortal 2 is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal 2 is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal 2. If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;
using MediaPortal.Common.Settings;

namespace MediaPortal.Common.PluginManager.Discovery
{
  /// <summary>
  /// Settings class used to persist information on plugin states between system restarts.
  /// </summary>
  public class PluginManagerSettings
  {
    #region Fields

    protected List<Guid> _userDisabledPlugins = new List<Guid>();

    #endregion

    public void AddUserDisabledPlugin(Guid pluginId)
    {
      if (!_userDisabledPlugins.Contains(pluginId))
        _userDisabledPlugins.Add(pluginId);
    }

    public void RemoveUserDisabledPlugin(Guid pluginId)
    {
      _userDisabledPlugins.Remove(pluginId);
    }

    [Setting(SettingScope.User)]
    public List<Guid> UserDisabledPlugins
    {
      get { return _userDisabledPlugins; }
      set { _userDisabledPlugins = value; }
    }
  }
}
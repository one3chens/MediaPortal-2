#region Copyright (C) 2007-2017 Team MediaPortal

/*
    Copyright (C) 2007-2017 Team MediaPortal
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

using MediaPortal.Common.Settings;
using System;
using System.Collections.Generic;

namespace MediaPortal.UiComponents.Neptune.Settings
{
  public class NeptuneSettings
  {
    [Setting(SettingScope.User)]
    public HomeGroup Plugins { get; set; }

    public static HomeGroup CreateDefaultPluginGroup()
    {
      HomeGroup pluginGroup = new HomeGroup(new[]
      {
        new Guid("C33E39CC-910E-41C8-BFFD-9ECCD340B569"), // OnlineVideos
        new Guid("80D2E2CC-BAAA-4750-807B-F37714153751"), // Movies
        new Guid("A4DF2DF6-8D66-479a-9930-D7106525EB07"), // Videos
        new Guid("30F57CBA-459C-4202-A587-09FFF5098251"), // Series

        new Guid("93442DF7-186D-42e5-A0F5-CF1493E68F49"), // Browse Media
        new Guid("17D2390E-5B05-4fbd-89F6-24D60CEB427F"), // Browse Local (exclusive)
       
        new Guid("55556593-9FE9-436c-A3B6-A971E10C9D44"), // Images
      
        new Guid("30715D73-4205-417f-80AA-E82F0834171F"), // Audio
        new Guid("E00B8442-8230-4D7B-B871-6AC77755A0D5"), // PartyMusicPlayer
        new Guid("2DED75C0-5EAE-4E69-9913-6B50A9AB2956"), // WebRadio
      
        new Guid("B4A9199F-6DD4-4bda-A077-DE9C081F7703"), // TV Home
        new Guid("A298DFBE-9DA8-4C16-A3EA-A9B354F3910C"), // Neptune EPG Link
        new Guid("7F52D0A1-B7F8-46A1-A56B-1110BBFB7D51"), // Neptune Recordings Link
        new Guid("87355E05-A15B-452A-85B8-98D4FC80034E"), // Neptune Schedules Link
        new Guid("D91738E9-3F85-443B-ABBD-EF01731734AD"), // Neptune Program Search Link

        new Guid("BB49A591-7705-408F-8177-45D633FDFAD0"), // News
        new Guid("E34FDB62-1F3E-4aa9-8A61-D143E0AF77B5"), // Weather

        new Guid("F6255762-C52A-4231-9E67-14C28735216E"), // Settings
      });
      return pluginGroup;
    }
  }

  public class HomeGroup
  {
    public HomeGroup()
    {
    }

    public HomeGroup(IEnumerable<Guid> actions)
    {
      Actions = new List<Guid>(actions);
    }

    public List<Guid> Actions { get; set; }
  }
}

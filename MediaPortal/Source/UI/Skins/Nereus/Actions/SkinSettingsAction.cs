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

using MediaPortal.UI.Presentation.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPortal.Common.Localization;
using MediaPortal.Common;
using HomeEditor.Actions;

namespace MediaPortal.UiComponents.Nereus.Actions
{
  public class SkinSettingsAction : AbstractConfigurationAction
  {
    public const string CONFIG_LOCATION = "/Appearance/Skin/SkinSettings";

    public override IResourceString DisplayTitle
    {
      get { return LocalizationHelper.CreateResourceString("[SkinSettings.Configuration.SkinSettings]"); }
    }

    protected override string ConfigLocation
    {
      get { return CONFIG_LOCATION; }
    }
  }
}

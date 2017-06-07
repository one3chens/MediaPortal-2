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

using MediaPortal.UI.Presentation.DataObjects;

namespace MediaPortal.UiComponents.Neptune.Models.HomeItems
{
  /// <summary>
  /// Container for a group of <see cref="HomeItem"/>s to be displayed on the home screen
  /// </summary>
  public class HomeGroupItem : ListItem
  {
    protected ItemsList _items = new ItemsList();

    public ItemsList Items
    {
      get { return _items; }
    }
  }

  /// <summary>
  /// Container for 4 tiles to be displayed in a uniform size.
  /// </summary>
  public class UniformTileGroup : HomeGroupItem
  {

  }

  /// <summary>
  /// Container for 3 poster tiles and a banner tile.
  /// </summary>
  public class PosterBannerGroup : HomeGroupItem
  {

  }

  /// <summary>
  /// Container for 2 large poster tiles.
  /// </summary>
  public class PosterGroup : HomeGroupItem
  {

  }
}
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

using System;
using MediaPortal.Common.General;
using MediaPortal.UI.Presentation.DataObjects;

namespace MediaPortal.UiComponents.Neptune.Models.HomeItems
{
  /// <summary>
  /// Holds a single item of a <see cref="HomeGroupItem"/> 
  /// </summary>
  public class HomeItem : ListItem
  {
    protected AbstractProperty _itemProperty = new WProperty(typeof(object), null);
    protected AbstractProperty _itemIdProperty = new WProperty(typeof(Guid), Guid.Empty);

    public HomeItem()
    {

    }

    public HomeItem(Guid itemId, object item)
    {
      UpdateItem(itemId, item);
    }

    public void UpdateItem(Guid itemId, object item)
    {
      ItemId = itemId;
      Item = item;

      ListItem listItem = item as ListItem;
      Command = listItem != null ? listItem.Command : null;
    }

    public AbstractProperty ItemProperty
    {
      get { return _itemProperty; }
    }

    public object Item
    {
      get { return _itemProperty.GetValue(); }
      set { _itemProperty.SetValue(value); }
    }

    public AbstractProperty ItemIdProperty
    {
      get { return _itemIdProperty; }
    }

    public Guid ItemId
    {
      get { return (Guid)_itemIdProperty.GetValue(); }
      set { _itemIdProperty.SetValue(value); }
    }
  }
}

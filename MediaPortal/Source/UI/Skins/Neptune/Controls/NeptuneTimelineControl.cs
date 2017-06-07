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

using MediaPortal.UI.SkinEngine.Controls.Panels;
using MediaPortal.UI.SkinEngine.Controls.Visuals;
using SharpDX;
using System.Collections.Generic;

namespace MediaPortal.UiComponents.Neptune.Controls
{
  /// <summary>
  /// A custom <see cref="AnimatedStackPanel"/> that can scroll to a child control of a <see cref="ListViewItem"/>.
  /// </summary>
  public class NeptuneTimelineControl : AnimatedStackPanel
  {
    /// <summary>
    /// Slightly modified SetScrollIndex that starts the scroll animation at the last offset rather than the last index
    /// as we scroll to an offset when bringing a sub item into view.
    /// TODO: Implement this in <see cref="AnimatedStackPanel"/> as it seems to make quick scrolling smoother. 
    /// </summary>
    /// <param name="childIndex"></param>
    /// <param name="first"></param>
    /// <param name="force"></param>
    public override void SetScrollIndex(double childIndex, bool first, bool force)
    {
      if (force)
      {
        base.SetScrollIndex(childIndex, first, force);
      }
      else
      {
        _isAnimating = true;
        float offset = _actualPhysicalOffset;
        _startOffsetX = offset + (first ? _actualFirstVisibleChildIndex : _actualLastVisibleChildIndex - (offset > 0 ? 1 : 0));
        _diffOffsetX = childIndex - _startOffsetX;
        _scrollingToFirst = first;
        FireEvent(SCROLL_EVENT, RoutingStrategyEnum.VisualTree);
      }
    }

    /// <summary>
    /// Caculates the offset of a sub item relative to the position of the parent list item.
    /// </summary>
    /// <param name="group">The parent list item.</param>
    /// <param name="groupChildBounds">The sub item's bounds.</param>
    /// <param name="first">Whether we are scrolling towards the first item.</param>
    /// <returns></returns>
    protected float CalculateOffset(FrameworkElement group, RectangleF groupChildBounds, bool first)
    {
      if (first)
      {
        float leftOffset = groupChildBounds.Left - group.ActualBounds.Left;
        if (leftOffset <= 0)
          return 0;
        return leftOffset / group.ActualBounds.Width;
      }
      else
      {
        float rightOffset = groupChildBounds.Right - group.ActualBounds.Left;
        if (rightOffset <= 0)
          return 0;
        return (rightOffset / group.ActualBounds.Width) - 1;
      }
    }

    /// <summary>
    /// Caclulates the index and offset to scroll to to bring the <paramref name="element"/> into view.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="elementBounds"></param>
    protected override void BringIntoView(UIElement element, ref RectangleF elementBounds)
    {
      IItemProvider itemProvider = ItemProvider;
      if (itemProvider == null)
      {
        base.BringIntoView(element, elementBounds);
        return;
      }

      if (_doScroll)
      {
        IList<FrameworkElement> arrangedItemsCopy;
        int arrangedStart;
        int oldFirstViewableChild;
        int oldLastViewableChild;
        float currentOffset;
        lock (Children.SyncRoot)
        {
          arrangedItemsCopy = new List<FrameworkElement>(_arrangedItems);
          arrangedStart = _arrangedItemsStartIndex;
          oldFirstViewableChild = _actualFirstVisibleChildIndex - arrangedStart;
          oldLastViewableChild = _actualLastVisibleChildIndex - arrangedStart;
          currentOffset = _actualPhysicalOffset;
        }
        if (arrangedStart < 0)
          return;
        int index = 0;
        foreach (FrameworkElement currentChild in arrangedItemsCopy)
        {
          if (InVisualPath(currentChild, element))
          {
            float leftOffset = CalculateOffset(currentChild, elementBounds, true);
            float rightOffset = CalculateOffset(currentChild, elementBounds, false);
            bool first;
            if (index + leftOffset < oldFirstViewableChild + currentOffset)
              first = true;
            else if (index + rightOffset <= oldLastViewableChild + currentOffset - (currentOffset > 0 ? 1 : 0))
              // Already visible
              break;
            else
              first = false;

            SetScrollIndex(index + arrangedStart + (first ? leftOffset : rightOffset), first, false);
            // Adjust the scrolled element's bounds; Calculate the difference between positions of childen at old/new child indices
            float extendsInOrientationDirection = (float)SumActualExtendsInOrientationDirection(arrangedItemsCopy, Orientation,
                first ? oldFirstViewableChild : oldLastViewableChild, index);
            if (Orientation == Orientation.Horizontal)
              elementBounds.X -= extendsInOrientationDirection;
            else
              elementBounds.Y -= extendsInOrientationDirection;
            break;
          }
          index++;
        }
      }
    }
  }
}

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

using MediaPortal.Common;
using MediaPortal.Common.General;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.Messaging;
using MediaPortal.Common.Settings;
using MediaPortal.Extensions.UserServices.FanArtService.Client.Models;
using MediaPortal.UI.Presentation.DataObjects;
using MediaPortal.UI.Presentation.Workflow;
using MediaPortal.UI.SkinEngine.MpfElements;
using MediaPortal.UiComponents.Media.Models.Navigation;
using MediaPortal.UiComponents.Neptune.Models.HomeItems;
using MediaPortal.UiComponents.Neptune.Settings;
using MediaPortal.UiComponents.SkinBase.General;
using MediaPortal.UiComponents.SkinBase.Models;
using MediaPortal.Utilities;
using MediaPortal.Utilities.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaPortal.UiComponents.Neptune.Models
{
  /// <summary>
  /// Backing model for the Neptune homescreen.
  /// </summary>
  public class NeptuneHomeModel : MenuModel
  {
    #region Consts

    public const string STR_HOMEMENU_MODEL_ID = "F3CF9C2A-5B95-49A5-98DA-12328692AA18";
    public static readonly Guid HOMEMENU_MODEL_ID = new Guid(STR_HOMEMENU_MODEL_ID);

    public static readonly Guid HOME_STATE_ID = new Guid("7F702D9C-F2DD-42da-9ED8-0BA92F07787F");

    protected const int MAX_ITEMS_PER_GROUP = 4;
    protected const int LATEST_MEDIA_REFRESH_INTERVAL = 300;

    #endregion

    #region Protected members

    protected bool _isInitialized;
    protected DateTime _lastLatestMediaRefresh = DateTime.MinValue;

    protected ItemsList _homeMenuItems;
    protected ICollection<Guid> _currentPluginIds;
    protected ICollection<Guid> _currentMovieIds;
    protected ICollection<Guid> _currentSeriesIds;
    protected ICollection<Guid> _currentAudioIds;

    protected readonly DelayedEvent _delayedMenuUpdateEvent;

    #endregion

    #region Init/Dispose

    public NeptuneHomeModel()
    {
      _homeMenuItems = new ItemsList();
      _currentPluginIds = new List<Guid>();
      _currentMovieIds = new List<Guid>();
      _currentSeriesIds = new List<Guid>();
      _currentAudioIds = new List<Guid>();

      //Update menu items only if no more requests are following after 200 ms
      _delayedMenuUpdateEvent = new DelayedEvent(200);
      _delayedMenuUpdateEvent.OnEventHandler += OnMenuUpdate;

      SubscribeToMessages();
    }

    public override void Dispose()
    {
      base.Dispose();
      if (_delayedMenuUpdateEvent != null)
        _delayedMenuUpdateEvent.Dispose();
    }

    private void SubscribeToMessages()
    {
      if (_messageQueue != null)
        _messageQueue.MessageReceived += OnMessageReceived;
    }

    private void OnMessageReceived(AsynchronousMessageQueue queue, SystemMessage message)
    {
      if (message.ChannelName == WorkflowManagerMessaging.CHANNEL)
      {
        WorkflowManagerMessaging.MessageType messageType = (WorkflowManagerMessaging.MessageType)message.MessageType;
        if (messageType == WorkflowManagerMessaging.MessageType.StatePushed)
        {
          if (((NavigationContext)message.MessageData[WorkflowManagerMessaging.CONTEXT]).WorkflowState.StateId == HOME_STATE_ID)
            UpdateMenu();
        }
        else if (messageType == WorkflowManagerMessaging.MessageType.NavigationComplete)
        {
          var context = ServiceRegistration.Get<IWorkflowManager>().CurrentNavigationContext;
          if (context != null && context.WorkflowState.StateId == HOME_STATE_ID)
            UpdateMenu();
        }
      }
    }

    #endregion

    #region Properties

    public ItemsList HomeMenuItems
    {
      get { return _homeMenuItems; }
    }

    protected LatestMediaModel LatestMediaModel
    {
      get { return GetModel<LatestMediaModel>(LatestMediaModel.LATEST_MEDIA_MODEL_ID); }
    }

    protected FanArtBackgroundModel FanArtBackgroundModel
    {
      get { return GetModel<FanArtBackgroundModel>(FanArtBackgroundModel.FANART_MODEL_ID); }
    }

    protected T GetModel<T>(Guid modelId) where T : class
    {
      return ServiceRegistration.Get<IWorkflowManager>().GetModel(modelId) as T;
    }

    #endregion

    #region Menu updating

    protected void InitMenu()
    {
      if (_isInitialized)
        return;
      _isInitialized = true;

      //Latest media change handlers
      LatestMediaModel lm = LatestMediaModel;
      if (lm != null)
      {
        lm.Movies.ObjectChanged += OnLatestMediaChanged;
        lm.Series.ObjectChanged += OnLatestMediaChanged;
        lm.Audio.ObjectChanged += OnLatestMediaChanged;
        LatestMediaModel.UpdateItems();
      }

      //Plugin item change handler
      MenuItems.ObjectChanged += MenuItemsOnObjectChanged;
    }

    private void MenuItemsOnObjectChanged(IObservable observable)
    {
      var context = ServiceRegistration.Get<IWorkflowManager>().CurrentNavigationContext;
      if (context != null && context.WorkflowState.StateId == HOME_STATE_ID)
        UpdateMenu();
    }

    protected void UpdateMenu()
    {
      _delayedMenuUpdateEvent.EnqueueEvent(this, EventArgs.Empty);
    }

    private void OnMenuUpdate(object sender, EventArgs e)
    {
      InitMenu();

      bool listNeedsUpdate = false;

      //Plugins
      List<HomeItem> pluginItems = GetSortedPluginItems();
      listNeedsUpdate |= TryUpdateCurrentItems(pluginItems, _currentPluginIds);

      //Latest media
      var lm = LatestMediaModel;

      //Movies
      List<HomeItem> movieItems = GetLatestMediaItems(lm.Movies, MAX_ITEMS_PER_GROUP);
      listNeedsUpdate |= TryUpdateCurrentItems(movieItems, _currentMovieIds);

      //Series
      List<HomeItem> seriesItems = GetLatestMediaItems(lm.Series, 2);
      listNeedsUpdate |= TryUpdateCurrentItems(seriesItems, _currentSeriesIds);

      //Audio
      List<HomeItem> audioItems = GetLatestMediaItems(lm.Audio, MAX_ITEMS_PER_GROUP);
      listNeedsUpdate |= TryUpdateCurrentItems(audioItems, _currentAudioIds);

      if (!listNeedsUpdate)
        return;

      _homeMenuItems.Clear();
      AddHomeGroups<UniformTileGroup>(pluginItems, true);
      AddHomeGroups<PosterBannerGroup>(movieItems, false);
      AddHomeGroups<PosterGroup>(seriesItems, false, 2);
      AddHomeGroups<UniformTileGroup>(audioItems, false);
      _homeMenuItems.FireChange();
    }

    protected void AddHomeGroups<T>(IEnumerable<HomeItem> items, bool reverse, int maxItemsPerGroup = MAX_ITEMS_PER_GROUP) where T : HomeGroupItem, new()
    {
      T currentGroup = null;
      foreach (HomeItem item in items)
      {
        //Create a new group if max items reached
        if (currentGroup == null)
        {
          currentGroup = new T();
          if (reverse)
            _homeMenuItems.Insert(0, currentGroup);
          else
            _homeMenuItems.Add(currentGroup);
        }

        if (reverse)
          currentGroup.Items.Insert(0, item);
        else
          currentGroup.Items.Add(item);

        if (currentGroup.Items.Count == maxItemsPerGroup)
          currentGroup = null;
      }

      if (currentGroup == null)
        return;

      //Fill the rest of the last group with empty items
      while (currentGroup.Items.Count < maxItemsPerGroup)
      {
        if (reverse)
          currentGroup.Items.Insert(0, new HomeItem());
        else
          currentGroup.Items.Add(new HomeItem());
      }
    }

    protected bool TryUpdateCurrentItems(ICollection<HomeItem> items, ICollection<Guid> currentIds)
    {
      if (items.Count == currentIds.Count && items.All(i => currentIds.Contains(i.ItemId)))
        return false;

      currentIds.Clear();
      CollectionUtils.AddAll(currentIds, items.Select(i => i.ItemId));
      return true;
    }

    #endregion

    #region Plugins

    protected List<HomeItem> GetSortedPluginItems()
    {
      List<HomeItem> pluginItems = new List<HomeItem>();

      WorkflowAction action;
      lock (MenuItems.SyncRoot)
        foreach (ListItem menuItem in MenuItems)
          if (TryGetWorkflowAction(menuItem, out action))
            pluginItems.Add(new HomeItem(action.ActionId, menuItem));

      List<Guid> pluginPositions = GetSortedPluginIds();
      pluginItems.Sort((x, y) => PluginPositionComparer(x, y, pluginPositions));
      return pluginItems;
    }

    protected static int PluginPositionComparer(HomeItem x, HomeItem y, IList<Guid> pluginPositions)
    {
      int indexX = pluginPositions.IndexOf(x.ItemId);
      int indexY = pluginPositions.IndexOf(y.ItemId);

      if (indexX < 0)
        return indexY < 0 ? 0 : 1;
      if (indexY < 0)
        return -1;
      return indexX.CompareTo(indexY);
    }

    protected static bool TryGetWorkflowAction(ListItem item, out WorkflowAction action)
    {
      object actionObj;
      if (item.AdditionalProperties.TryGetValue(Consts.KEY_ITEM_ACTION, out actionObj))
      {
        action = actionObj as WorkflowAction;
        return action != null;
      }
      action = null;
      return false;
    }

    #endregion

    #region Latest media

    private void OnLatestMediaChanged(IObservable observable)
    {
      UpdateMenu();
    }

    protected List<HomeItem> GetLatestMediaItems(IEnumerable<ListItem> mediaListItems, int maxItems)
    {
      List<HomeItem> items = new List<HomeItem>();
      if (maxItems < 1)
        return items;

      foreach (ListItem mediaListItem in mediaListItems)
      {
        MediaItem mediaItem;
        if (!TryGetMediaItem(mediaListItem, out mediaItem))
          continue;

        items.Add(new HomeItem(mediaItem.MediaItemId, mediaListItem));
        if (items.Count == maxItems)
          break;
      }
      return items;
    }

    protected static bool TryGetMediaItem(ListItem mediaListItem, out MediaItem mediaItem)
    {
      PlayableMediaItem pmi = mediaListItem as PlayableMediaItem;
      if (pmi != null)
      {
        mediaItem = pmi.MediaItem;
        return mediaItem != null;
      }

      PlayableContainerMediaItem pcmi = mediaListItem as PlayableContainerMediaItem;
      if (pcmi != null)
      {
        mediaItem = pcmi.MediaItem;
        return mediaItem != null;
      }

      mediaItem = null;
      return false;
    }

    #endregion

    #region Selection change handlers

    public void SetSelectedItem(object sender, SelectionChangedEventArgs e)
    {
      UpdateSelectedFanArtItem(e.FirstAddedItem as ListItem);
    }

    public void SetSelectedHomeItem(object item)
    {
      HomeItem homeItem = item as HomeItem;
      object fanartItem = homeItem != null ? homeItem.Item : item;
      UpdateSelectedFanArtItem(fanartItem as ListItem);
    }

    protected void UpdateSelectedFanArtItem(ListItem item)
    {
      if (item != null)
      {
        var fbm = FanArtBackgroundModel;
        if (fbm != null)
          fbm.SelectedItem = item;
      }
    }

    #endregion

    #region Settings

    protected List<Guid> GetSortedPluginIds()
    {
      var sm = ServiceRegistration.Get<ISettingsManager>();
      var settings = sm.Load<NeptuneSettings>();
      HomeGroup group = settings.Plugins;
      if (group == null)
      {
        group = NeptuneSettings.CreateDefaultPluginGroup();
        settings.Plugins = group;
        sm.Save(settings);
      }
      return group.Actions;
    }

    #endregion
  }
}

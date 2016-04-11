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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MediaPortal.Common;
using MediaPortal.Common.Localization;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement.Helpers;
using MediaPortal.Common.PathManager;
using MediaPortal.Extensions.OnlineLibraries.Libraries.AudioDbV1.Data;
using MediaPortal.Extensions.OnlineLibraries.Matches;
using MediaPortal.Extensions.OnlineLibraries.TheAudioDB;
using System.Linq;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;

namespace MediaPortal.Extensions.OnlineLibraries
{
  public class MusicTheAudioDbMatcher : BaseMatcher<TrackMatch, string>
  {
    #region Static instance

    public static MusicTheAudioDbMatcher Instance
    {
      get { return ServiceRegistration.Get<MusicTheAudioDbMatcher>(); }
    }

    #endregion

    #region Constants

    public static string CACHE_PATH = ServiceRegistration.Get<IPathManager>().GetPath(@"<DATA>\TheAudioDB\");
    protected static string _matchesSettingsFile = Path.Combine(CACHE_PATH, "Matches.xml");
    protected static TimeSpan MAX_MEMCACHE_DURATION = TimeSpan.FromHours(12);

    protected override string MatchesSettingsFile
    {
      get { return _matchesSettingsFile; }
    }

    #endregion

    #region Fields

    protected DateTime _memoryCacheInvalidated = DateTime.MinValue;
    protected ConcurrentDictionary<string, AudioDbTrack> _memoryCache = new ConcurrentDictionary<string, AudioDbTrack>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Contains the initialized TheAudioDbWrapper.
    /// </summary>
    private TheAudioDbWrapper _audioDb;

    #endregion

    public bool FindAndUpdateTrack(TrackInfo trackInfo)
    {
      AudioDbTrack trackDetails;
      if (
        /* Best way is to get details by any unique id */
        MatchByAnyId(trackInfo, out trackDetails) ||
        TryMatch(trackInfo.TrackName, trackInfo.Artists.Select(a => a.Name).ToList(), trackInfo.Album, trackInfo.TrackNum, false, out trackDetails)
        )
      {
        string id = null;
        if (trackDetails != null)
        {
          id = trackDetails.TrackId.ToString();

          MetadataUpdater.SetOrUpdateId(ref trackInfo.AudioDbId, trackDetails.TrackId);
          MetadataUpdater.SetOrUpdateId(ref trackInfo.MusicBrainzId, trackDetails.MusicBrainzID);
          MetadataUpdater.SetOrUpdateId(ref trackInfo.AlbumAudioDbId, trackDetails.AlbumId.HasValue ? trackDetails.AlbumId.Value : 0);
          MetadataUpdater.SetOrUpdateId(ref trackInfo.AlbumMusicBrainzId, trackDetails.MusicBrainzAlbumID);

          MetadataUpdater.SetOrUpdateString(ref trackInfo.TrackName, trackDetails.Track, true);
          MetadataUpdater.SetOrUpdateString(ref trackInfo.Album, trackDetails.Album, true);
          MetadataUpdater.SetOrUpdateValue(ref trackInfo.TrackNum, trackDetails.TrackNumber);
          MetadataUpdater.SetOrUpdateValue(ref trackInfo.DiscNum, trackDetails.CD.HasValue ? trackDetails.CD.Value : 0);
          MetadataUpdater.SetOrUpdateValue(ref trackInfo.TotalRating, trackDetails.Rating.HasValue ? trackDetails.Rating.Value : 0);
          MetadataUpdater.SetOrUpdateValue(ref trackInfo.RatingCount, trackDetails.RatingCount.HasValue ? trackDetails.RatingCount.Value : 0);

          if (trackDetails.ArtistId.HasValue)
          {
            MetadataUpdater.SetOrUpdateList(trackInfo.Artists, ConvertToPersons(trackDetails.ArtistId.Value, trackDetails.MusicBrainzArtistID, 
              trackDetails.Artist, PersonOccupation.Artist), true);
            MetadataUpdater.SetOrUpdateList(trackInfo.AlbumArtists, ConvertToPersons(trackDetails.ArtistId.Value, trackDetails.MusicBrainzArtistID, 
              trackDetails.Artist, PersonOccupation.Artist), true);
          }

          MetadataUpdater.SetOrUpdateList(trackInfo.Genres, new List<string>(new string[] { trackDetails.Genre }), true);

          AudioDbAlbum album;
          if (trackDetails.AlbumId.HasValue && _audioDb.GetAlbumFromId(trackDetails.AlbumId.Value, out album))
          {
            if(album.LabelId.HasValue)
              MetadataUpdater.SetOrUpdateList(trackInfo.MusicLabels, ConvertToCompanys(album.LabelId.Value, album.Label, CompanyType.MusicLabel), true);
          }
        }

        if (!string.IsNullOrEmpty(id))
          ScheduleDownload(id);
        return true;
      }
      return false;
    }

    private List<PersonInfo> ConvertToPersons(long artistId, string mbArtistId, string artist, PersonOccupation occupation)
    {
      if (artistId == 0 || string.IsNullOrEmpty(artist))
        return new List<PersonInfo>();

      return new List<PersonInfo> {
        new PersonInfo()
        {
          AudioDbId = artistId,
          MusicBrainzId = mbArtistId,
          Name = artist,
          Occupation = occupation
        }
      };
    }

    private List<CompanyInfo> ConvertToCompanys(long companyId, string company, CompanyType type)
    {
      if (companyId == 0 || string.IsNullOrEmpty(company))
        return new List<CompanyInfo>();

      return new List<CompanyInfo> {
        new CompanyInfo()
        {
          AudioDbId = companyId,
          Name = company,
          Type = type
        }
      };
    }

    private bool MatchByAnyId(TrackInfo trackInfo, out AudioDbTrack trackDetails)
    {
      if ((trackInfo.AudioDbId > 0 && _audioDb.GetTrackFromId(trackInfo.AudioDbId, out trackDetails)) ||
        (!string.IsNullOrEmpty(trackInfo.MusicBrainzId) && _audioDb.GetTrackFromMBId(trackInfo.MusicBrainzId, out trackDetails)))
      {
        var onlineMatch = new TrackMatch
        {
          Id = trackDetails.TrackId.ToString(),
          ItemName = trackInfo.TrackName,
          TrackName = trackDetails.Track,
          RecordingName = trackDetails.Track,
          ReleaseName = trackDetails.Album
        };
        _storage.TryAddMatch(onlineMatch);

        return true;
      }
      trackDetails = null;
      return false;
    }

    protected bool TryMatch(string title, List<string> artists, string album, int trackNum, bool cacheOnly, out AudioDbTrack trackDetail)
    {
      trackDetail = null;
      try
      {
        // Prefer memory cache
        CheckCacheAndRefresh();
        if (_memoryCache.TryGetValue(title, out trackDetail))
          return true;

        // Load cache or create new list
        List<TrackMatch> matches = _storage.GetMatches();

        // Init empty
        trackDetail = null;

        // Use cached values before doing online query
        TrackMatch match = matches.Find(m =>
          string.Equals(m.TrackName, title, StringComparison.OrdinalIgnoreCase) ||
          string.Equals(m.TrackName, title, StringComparison.OrdinalIgnoreCase));
        ServiceRegistration.Get<ILogger>().Debug("TheAudioDbMatcher: Try to lookup track \"{0}\" from cache: {1}", title, match != null && string.IsNullOrEmpty(match.Id) == false);

        // Try online lookup
        if (!Init())
          return false;

        // If this is a known movie, only return the track details.
        if (match != null)
          return !string.IsNullOrEmpty(match.Id) && _audioDb.GetTrackFromId(Convert.ToInt64(match.Id), out trackDetail);

        if (cacheOnly)
          return false;

        List<AudioDbTrack> tracks;
        if (_audioDb.SearchTrackUnique(title, artists, album, trackNum, out tracks))
        {
          AudioDbTrack trackResult = tracks[0];
          ServiceRegistration.Get<ILogger>().Debug("TheAudioDbMatcher: Found unique online match for \"{0}\": \"{1}\"", title, trackResult.Track);
          if (_audioDb.GetTrackFromId(trackResult.TrackId, out trackDetail))
          {
            var onlineMatch = new TrackMatch
            {
              Id = trackDetail.TrackId.ToString(),
              ItemName = trackDetail.Track,
              TrackName = trackDetail.Track,
              RecordingName = trackDetail.Track,
              ReleaseName = trackDetail.Album
            };
            _storage.TryAddMatch(onlineMatch);
            return true;
          }
        }
        ServiceRegistration.Get<ILogger>().Debug("TheAudioDbMatcher: No unique match found for \"{0}\"", title);
        // Also save "non matches" to avoid retrying
        _storage.TryAddMatch(new TrackMatch { ItemName = title, TrackName = title });
        return false;
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Debug("TheAudioDbMatcher: Exception while processing track {0}", ex, title);
        return false;
      }
      finally
      {
        if (trackDetail != null)
          _memoryCache.TryAdd(title, trackDetail);
      }
    }

    /// <summary>
    /// Check if the memory cache should be cleared and starts an online update of (file-) cached series information.
    /// </summary>
    private void CheckCacheAndRefresh()
    {
      if (DateTime.Now - _memoryCacheInvalidated <= MAX_MEMCACHE_DURATION)
        return;
      _memoryCache.Clear();
      _memoryCacheInvalidated = DateTime.Now;

      // TODO: when updating movie information is implemented, start here a job to do it
    }

    public override bool Init()
    {
      if (!base.Init())
        return false;

      if (_audioDb != null)
        return true;

      _audioDb = new TheAudioDbWrapper();
      // Try to lookup online content in the configured language
      CultureInfo currentCulture = ServiceRegistration.Get<ILocalization>().CurrentCulture;
      _audioDb.SetPreferredLanguage(currentCulture.TwoLetterISOLanguageName);
      return _audioDb.Init(CACHE_PATH);
    }

    protected override void DownloadFanArt(string albumId)
    {
      try
      {
        ServiceRegistration.Get<ILogger>().Debug("TheAudioDbMatcher Download: Started for ID {0}", albumId);

        if (!Init())
          return;

        int taadbId = 0;
        if (!int.TryParse(albumId, out taadbId))
          return;

        AudioDbTrack track;
        if (!_audioDb.GetTrackFromId(taadbId, out track))
          return;

        AudioDbArtist artist;
        if (!track.ArtistId.HasValue || !_audioDb.GetArtistFromId(track.ArtistId.Value, out artist))
          return;

        AudioDbAlbum album;
        if (!track.AlbumId.HasValue || !_audioDb.GetAlbumFromId(track.AlbumId.Value, out album))
          return;

        // Save Cover
        ServiceRegistration.Get<ILogger>().Debug("TheAudioDbMatcher Download: Begin saving fanarts for ID {0}", albumId);
        _audioDb.DownloadImage(album.AlbumId, album.AlbumThumb, "Covers");
        _audioDb.DownloadImage(album.AlbumId, album.AlbumCDart, "CDArt");

        ServiceRegistration.Get<ILogger>().Debug("TheAudioDbMatcher Download: Begin saving artist banners for ID {0}", albumId);
        if(!string.IsNullOrEmpty(artist.ArtistBanner)) _audioDb.DownloadImage(artist.ArtistId, artist.ArtistBanner, "Banners");
        if (!string.IsNullOrEmpty(artist.ArtistFanart)) _audioDb.DownloadImage(artist.ArtistId, artist.ArtistFanart, "Backdrops");
        if (!string.IsNullOrEmpty(artist.ArtistFanart2)) _audioDb.DownloadImage(artist.ArtistId, artist.ArtistFanart2, "Backdrops");
        if (!string.IsNullOrEmpty(artist.ArtistFanart3)) _audioDb.DownloadImage(artist.ArtistId, artist.ArtistFanart3, "Backdrops");
        if (!string.IsNullOrEmpty(artist.ArtistLogo)) _audioDb.DownloadImage(artist.ArtistId, artist.ArtistBanner, "Logos");
        if (!string.IsNullOrEmpty(artist.ArtistThumb)) _audioDb.DownloadImage(artist.ArtistId, artist.ArtistBanner, "Thumbnails");

        ServiceRegistration.Get<ILogger>().Debug("TheAudioDbMatcher Download: Finished ID {0}", albumId);

        // Remember we are finished
        FinishDownloadFanArt(albumId);
      }
      catch (Exception ex)
      {
        ServiceRegistration.Get<ILogger>().Debug("TheAudioDbMatcher: Exception downloading FanArt for ID {0}", ex, albumId);
      }
    }
  }
}

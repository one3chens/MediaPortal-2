﻿#region Copyright (C) 2007-2012 Team MediaPortal

/*
    Copyright (C) 2007-2012 Team MediaPortal
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
using System.Globalization;
using System.IO;
using MediaPortal.Common;
using MediaPortal.Common.Logging;
using MediaPortal.Common.MediaManagement;
using MediaPortal.Common.MediaManagement.DefaultItemAspects;
using MediaPortal.Common.ResourceAccess;
using MediaPortal.Common.Services.ResourceAccess.StreamedResourceToLocalFsAccessBridge;
using MediaPortal.Plugins.MP2Extended.Attributes;
using MediaPortal.Plugins.MP2Extended.Exceptions;
using MediaPortal.Plugins.MP2Extended.Utils;
using MediaPortal.Plugins.MP2Extended.WSS.StreamInfo;
using MediaPortal.Plugins.Transcoding.Aspects;
using MediaPortal.Plugins.Transcoding.Service;
using MediaPortal.Plugins.Transcoding.Service.Metadata.Streams;
using MediaPortal.Plugins.SlimTv.Interfaces;
using MediaPortal.Plugins.MP2Extended.ResourceAccess.TAS.Timeshiftings;
using MediaPortal.Plugins.SlimTv.Interfaces.Items;
using MediaPortal.Plugins.SlimTv.Interfaces.ResourceProvider;
using MediaPortal.Plugins.Transcoding.Service.Analyzers;
using MediaPortal.Plugins.Transcoding.Service.Metadata;

namespace MediaPortal.Plugins.MP2Extended.ResourceAccess.WSS.json.StreamInfo
{
  [ApiFunctionDescription(Type = ApiFunctionDescription.FunctionType.Json, Summary = "")]
  [ApiFunctionParam(Name = "itemId", Type = typeof(string), Nullable = false)]
  internal class GetMediaInfo
  {
    private const string UNDEFINED = "undef";

    public WebMediaInfo Process(string itemId)
    {
      if (itemId == null)
        throw new BadRequestException("GetMediaInfo: itemId is null");

      Guid mediaItemId;
      long duration = 0;
      string container = string.Empty;
      List<WebVideoStream> webVideoStreams = new List<WebVideoStream>();
      List<WebAudioStream> webAudioStreams = new List<WebAudioStream>();
      List<WebSubtitleStream> webSubtitleStreams = new List<WebSubtitleStream>();

      if (Guid.TryParse(itemId, out mediaItemId) == false)
      {
        int channelIdInt;
        if (int.TryParse(itemId, out channelIdInt))
        {
          string identifier = "MP2Ext Sample - " + itemId;

          if (!ServiceRegistration.IsRegistered<ITvProvider>())
            throw new BadRequestException("GetMediaInfo: ITvProvider not found");

          IChannelAndGroupInfo channelAndGroupInfo = ServiceRegistration.Get<ITvProvider>() as IChannelAndGroupInfo;

          IChannel channel;
          if (!channelAndGroupInfo.GetChannel(channelIdInt, out channel))
            throw new BadRequestException(string.Format("GetMediaInfo: Couldn't get channel with Id: {0}", channelIdInt));

          ITimeshiftControlEx timeshiftControl = ServiceRegistration.Get<ITvProvider>() as ITimeshiftControlEx;

          MediaItem mediaItem;
          if (!timeshiftControl.StartTimeshift(identifier, SlotControl.GetSlotIndex(identifier), channel, out mediaItem))
            throw new BadRequestException("GetMediaInfo: Couldn't start timeshifting");

          MetadataContainer streamInfo;
          try
          {
            string resourcePathStr = (string)mediaItem[ProviderResourceAspect.Metadata].GetAttributeValue(ProviderResourceAspect.ATTR_RESOURCE_ACCESSOR_PATH);
            var resourcePath = ResourcePath.Deserialize(resourcePathStr);
            IResourceAccessor stra = SlimTvResourceProvider.GetResourceAccessor(resourcePath.BasePathSegment.Path);
            if (stra is ILocalFsResourceAccessor)
            {
              streamInfo = MediaAnalyzer.ParseVideoFile((ILocalFsResourceAccessor)stra);
            }
            else
            {
              streamInfo = MediaAnalyzer.ParseVideoStream((INetworkResourceAccessor)stra);
            }
          }
          finally
          {
            timeshiftControl.StopTimeshift(identifier, SlotControl.GetSlotIndex(identifier));
            SlotControl.DeleteSlotIndex(identifier);
          }

          duration = 0;

          if (streamInfo.IsVideo)
          {
            container = streamInfo.Metadata.VideoContainerType.ToString().ToUpperInvariant();

            // Video
            WebVideoStream webVideoStream = new WebVideoStream();
            webVideoStream.Codec = Convert.ToString(streamInfo.Video.Codec);
            webVideoStream.DisplayAspectRatio = Convert.ToDecimal(streamInfo.Video.AspectRatio);
            webVideoStream.DisplayAspectRatioString = AspectRatioHelper.AspectRatioToString(Convert.ToDecimal(streamInfo.Video.AspectRatio));
            webVideoStream.Height = Convert.ToInt32(streamInfo.Video.Height);
            webVideoStream.Width = Convert.ToInt32(streamInfo.Video.Width);
            webVideoStream.ID = streamInfo.Video.StreamIndex;
            webVideoStream.Index = 0;
            //webVideoStream.Interlaced = true;
            webVideoStreams.Add(webVideoStream);

            //Audio
            for (int i = 0; i < streamInfo.Audio.Count; i++)
            {
              WebAudioStream webAudioStream = new WebAudioStream();
              webAudioStream.Channels = streamInfo.Audio[i].Channels;
              webAudioStream.Codec = Convert.ToString(streamInfo.Audio[i].Codec);
              webAudioStream.ID = streamInfo.Audio[i].StreamIndex;
              webAudioStream.Index = i;
              string language = string.IsNullOrEmpty(streamInfo.Audio[i].Language) ? UNDEFINED : streamInfo.Audio[i].Language;
              webAudioStream.Language = language;
              if (language != UNDEFINED)
              {
                webAudioStream.LanguageFull = new CultureInfo(language).EnglishName;
                if (string.IsNullOrEmpty(webAudioStream.Codec) == false) webAudioStream.Title = webAudioStream.Codec.ToUpperInvariant();
              }
              webAudioStreams.Add(webAudioStream);
            }

            //Subtitles
            for (int i = 0; i < streamInfo.Subtitles.Count; i++)
            {
              WebSubtitleStream webSubtitleStream = new WebSubtitleStream();
              webSubtitleStream.Filename = "embedded";
              webSubtitleStream.ID = streamInfo.Subtitles[i].StreamIndex;
              webSubtitleStream.Index = webSubtitleStreams.Count;
              string language = string.IsNullOrEmpty(streamInfo.Audio[i].Language) ? UNDEFINED : streamInfo.Subtitles[i].Language;
              webSubtitleStream.Language = language;
              if (language != UNDEFINED)
              {
                webSubtitleStream.LanguageFull = new CultureInfo(language).EnglishName;
              }
              webSubtitleStreams.Add(webSubtitleStream);
            }
          }
          else if (streamInfo.IsAudio)
          {
            container = streamInfo.Metadata.AudioContainerType.ToString().ToUpperInvariant();
          }
        }
        else
        {
          throw new BadRequestException(String.Format("GetMediaInfo: No media found with id: {0}", itemId));
        }
      }
      else
      {
        ISet<Guid> necessaryMIATypes = new HashSet<Guid>();
        necessaryMIATypes.Add(MediaAspect.ASPECT_ID);
        necessaryMIATypes.Add(ProviderResourceAspect.ASPECT_ID);
        necessaryMIATypes.Add(ImporterAspect.ASPECT_ID);

        ISet<Guid> optionalMIATypes = new HashSet<Guid>();
        optionalMIATypes.Add(VideoAspect.ASPECT_ID);
        optionalMIATypes.Add(AudioAspect.ASPECT_ID);
        optionalMIATypes.Add(ImageAspect.ASPECT_ID);
        optionalMIATypes.Add(TranscodeItemAudioAspect.ASPECT_ID);
        optionalMIATypes.Add(TranscodeItemImageAspect.ASPECT_ID);
        optionalMIATypes.Add(TranscodeItemVideoAspect.ASPECT_ID);
        optionalMIATypes.Add(TranscodeItemVideoAudioAspect.ASPECT_ID);

        MediaItem item = GetMediaItems.GetMediaItemById(itemId, necessaryMIATypes, optionalMIATypes);

        if (item == null)
          throw new BadRequestException(String.Format("GetMediaInfo: No MediaItem found with id: {0}", itemId));

        // decide which type of media item we have
        if (item.Aspects.ContainsKey(VideoAspect.ASPECT_ID))
        {
          var videoAspect = item[VideoAspect.Metadata];
          duration = Convert.ToInt64(videoAspect.GetAttributeValue(VideoAspect.ATTR_DURATION) ?? 0);

          // Video
          WebVideoStream webVideoStream = new WebVideoStream();
          webVideoStream.Codec = Convert.ToString(videoAspect.GetAttributeValue(VideoAspect.ATTR_VIDEOENCODING) ?? string.Empty);
          webVideoStream.DisplayAspectRatio = Convert.ToDecimal(videoAspect.GetAttributeValue(VideoAspect.ATTR_ASPECTRATIO) ?? 0);
          webVideoStream.DisplayAspectRatioString = AspectRatioHelper.AspectRatioToString(Convert.ToDecimal(videoAspect.GetAttributeValue(VideoAspect.ATTR_ASPECTRATIO) ?? 0));
          webVideoStream.Height = Convert.ToInt32(videoAspect.GetAttributeValue(VideoAspect.ATTR_HEIGHT) ?? 0);
          webVideoStream.Width = Convert.ToInt32(videoAspect.GetAttributeValue(VideoAspect.ATTR_WIDTH) ?? 0);
          webVideoStreams.Add(webVideoStream);

          if (item.Aspects.ContainsKey(TranscodeItemVideoAspect.ASPECT_ID))
          {
            var transcodeVideoAspect = item[TranscodeItemVideoAspect.Metadata];
            object videoStream = transcodeVideoAspect.GetAttributeValue(TranscodeItemVideoAspect.ATTR_STREAM);
            webVideoStream.ID = int.Parse(videoStream.ToString());
            webVideoStream.Index = 0;
            //webVideoStream.Interlaced = transcodeVideoAspect[TranscodeItemVideoAspect.];

            container = (string)transcodeVideoAspect[TranscodeItemVideoAspect.ATTR_CONTAINER];

            // Audio
            IList<MultipleMediaItemAspect> transcodeItemVideoAudioAspects;
            if (MediaItemAspect.TryGetAspects(item.Aspects, TranscodeItemVideoAudioAspect.Metadata, out transcodeItemVideoAudioAspects))
            {
              for (int i = 0; i < transcodeItemVideoAudioAspects.Count; i++)
              {
                object audioStream = transcodeItemVideoAudioAspects[i].GetAttributeValue(TranscodeItemVideoAudioAspect.ATTR_AUDIOSTREAM);
                object audioChannel = transcodeItemVideoAudioAspects[i].GetAttributeValue(TranscodeItemVideoAudioAspect.ATTR_AUDIOCHANNEL);
                object audioCodec = transcodeItemVideoAudioAspects[i].GetAttributeValue(TranscodeItemVideoAudioAspect.ATTR_AUDIOCODEC);
                object audioLanguage = transcodeItemVideoAudioAspects[i].GetAttributeValue(TranscodeItemVideoAudioAspect.ATTR_AUDIOLANGUAGE);

                WebAudioStream webAudioStream = new WebAudioStream();
                if (audioChannel != null)
                {
                  webAudioStream.Channels = Convert.ToInt32(audioChannel);
                }
                if (audioCodec != null)
                  webAudioStream.Codec = audioCodec != null ? audioCodec.ToString() : (string)transcodeItemVideoAudioAspects[0].GetAttributeValue(TranscodeItemVideoAudioAspect.ATTR_AUDIOCODEC);
                webAudioStream.ID = int.Parse(audioStream.ToString());
                webAudioStream.Index = i;
                if (audioLanguage != null)
                {
                  string language = (string)audioLanguage == string.Empty ? UNDEFINED : audioLanguage.ToString();
                  webAudioStream.Language = language;
                  if (language != UNDEFINED)
                  {
                    webAudioStream.LanguageFull = new CultureInfo(language).EnglishName;
                    if (string.IsNullOrEmpty(webAudioStream.Codec) == false) webAudioStream.Title = webAudioStream.Codec.ToUpperInvariant();
                  }
                }
                webAudioStreams.Add(webAudioStream);
              }
            }

            // Subtitles
            IList<MultipleMediaItemAspect> transcodeItemVideoEmbeddedAspects;
            if (MediaItemAspect.TryGetAspects(item.Aspects, TranscodeItemVideoEmbeddedAspect.Metadata, out transcodeItemVideoEmbeddedAspects))
            {
              for (int i = 0; i < transcodeItemVideoEmbeddedAspects.Count; i++)
              {
                object subtitleLanguage = transcodeItemVideoEmbeddedAspects[i].GetAttributeValue(TranscodeItemVideoEmbeddedAspect.ATTR_EMBEDDED_SUBLANGUAGE);
                object subtitleStream = transcodeItemVideoEmbeddedAspects[i].GetAttributeValue(TranscodeItemVideoEmbeddedAspect.ATTR_EMBEDDED_SUBSTREAM);

                WebSubtitleStream webSubtitleStream = new WebSubtitleStream();
                webSubtitleStream.Filename = "embedded";
                webSubtitleStream.ID = int.Parse(subtitleStream.ToString());
                webSubtitleStream.Index = webSubtitleStreams.Count;
                if (subtitleLanguage != null)
                {
                  string language = (string)subtitleLanguage == string.Empty ? UNDEFINED : (string)subtitleLanguage;
                  webSubtitleStream.Language = language;
                  webSubtitleStream.LanguageFull = language;
                  if (language != UNDEFINED) webSubtitleStream.LanguageFull = new CultureInfo(language).EnglishName;
                }
                webSubtitleStreams.Add(webSubtitleStream);
              }
            }

            IResourceAccessor mediaItemAccessor = item.GetResourceLocator().CreateAccessor();
            if (mediaItemAccessor is IFileSystemResourceAccessor)
            {
              using (var fsra = (IFileSystemResourceAccessor)mediaItemAccessor.Clone())
              {
                if (fsra.IsFile)
                  using (var lfsra = StreamedResourceToLocalFsAccessBridge.GetLocalFsResourceAccessor(fsra))
                  {
                    List<SubtitleStream> externalSubtitles = MediaConverter.FindExternalSubtitles(lfsra);
                    if (externalSubtitles != null)
                      for (int i = 0; i < externalSubtitles.Count; i++)
                      {
                        WebSubtitleStream webSubtitleStream = new WebSubtitleStream();
                        webSubtitleStream.Filename = Path.GetFileName(externalSubtitles[i].Source);
                        webSubtitleStream.ID = -1;
                        webSubtitleStream.Index = webSubtitleStreams.Count;
                        if (string.IsNullOrEmpty(externalSubtitles[i].Language) == false)
                        {
                          webSubtitleStream.Language = externalSubtitles[i].Language;
                          webSubtitleStream.LanguageFull = new CultureInfo(externalSubtitles[i].Language).EnglishName;
                        }
                        else
                        {
                          webSubtitleStream.Language = UNDEFINED;
                          webSubtitleStream.LanguageFull = UNDEFINED;
                        }
                        webSubtitleStreams.Add(webSubtitleStream);
                      }
                  }
              }
            }
          }
        }

        // Audio File
        if (item.Aspects.ContainsKey(AudioAspect.ASPECT_ID))
        {
          var audioAspect = item[AudioAspect.Metadata];
          duration = (long)audioAspect[AudioAspect.ATTR_DURATION];
          if (item.Aspects.ContainsKey(TranscodeItemAudioAspect.ASPECT_ID))
          {
            container = (string)item[TranscodeItemAudioAspect.Metadata][TranscodeItemAudioAspect.ATTR_CONTAINER];
          }
        }

        // Image File
        if (item.Aspects.ContainsKey(ImageAspect.ASPECT_ID))
        {
          var imageAspect = item[ImageAspect.Metadata];
          if (item.Aspects.ContainsKey(TranscodeItemImageAspect.ASPECT_ID))
          {
            container = (string)item[TranscodeItemImageAspect.Metadata][TranscodeItemImageAspect.ATTR_CONTAINER];
          }
        }
      }

      WebMediaInfo webMediaInfo = new WebMediaInfo
      {
        Duration = duration * 1000,
        Container = container,
        VideoStreams = webVideoStreams,
        AudioStreams = webAudioStreams,
        SubtitleStreams = webSubtitleStreams
      };

      return webMediaInfo;
    }

    internal static ILogger Logger
    {
      get { return ServiceRegistration.Get<ILogger>(); }
    }
  }
}

<?xml version="1.0" encoding="utf-8"?>
<Grid
  xmlns="www.team-mediaportal.com/2008/mpf/directx"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  xmlns:fanart="clr-namespace:MediaPortal.Extensions.UserServices.FanArtService.Client;assembly=FanArtService.Client"
  xmlns:media="clr-namespace:MediaPortal.UiComponents.Media.Models.AspectWrappers;assembly=Media"
  xmlns:aspect="clr-namespace:MediaPortal.Common.MediaManagement.DefaultItemAspects;assembly=MediaPortal.Common"
  x:Name="MediaItemDetails" HorizontalAlignment="Stretch" VerticalAlignment="Stretch">

  <Grid.Resources>
    <fanart:FanArtImageSourceConverter x:Key="FanArtImageSourceConverter"/>
    <MultiAspectAttributeConverter x:Key="MultiAspectAttributeConverter"/>
  </Grid.Resources>
  
  <Grid.RowDefinitions>
    <RowDefinition Height="Auto"/>
    <RowDefinition Height="150"/>
    <RowDefinition Height="50"/>
  </Grid.RowDefinitions>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="1250"/>
    <ColumnDefinition Width="*"/>
  </Grid.ColumnDefinitions>
  
  <Image x:Name="thumb" Grid.Row="0" Grid.RowSpan="3" Grid.Column="1" VerticalAlignment="Top" Stretch="Uniform" Margin="-50,-30,-5,-96" Opacity="1">
  <Image.OpacityMask>
        <LinearGradientBrush StartPoint="0,0.5" EndPoint="1,0.5" >
          <GradientStop Offset="0.0" Color="#00000000" />
          <GradientStop Offset="1.0" Color="#FF000000" />
        </LinearGradientBrush>
      </Image.OpacityMask>
  <Image.Source>
    <ImageSourceWrapper Delay="0" DelayInOut="true" Transition="transitions\dissolve" TransitionDuration="0"
		Source="{Binding Source={StaticResource FanArtBackgroundModel}, Path=ImageSource, Converter={StaticResource FanArtImageSourceConverter}, ConverterParameter=FanArt;640;480;False}"
		FallbackSource="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}" />
    </Image.Source>
  </Image>
  <Image Source="FanartOverlay.png" Grid.Row="0" Grid.RowSpan="3" Grid.Column="1" Margin="-50,-30,-5,-96"/>    
  
  <!-- SERIES NAME -->
  
  <media:EpisodeAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}"
                              Grid.Column="0" Grid.Row="0" Margin="0,-10,-150,0" HorizontalAlignment="Stretch" VerticalAlignment="Top">
    <media:EpisodeAspectWrapper.Template>
      <ControlTemplate>
        <StackPanel Orientation="Horizontal" VerticalAlignment="Stretch" HorizontalAlignment="Stretch"
                    DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}">
          <Label x:Name="SeriesNameLabel" Color="{ThemeResource TextColor}" FontFamily="DefaultBold" FontSize="{ThemeResource StandardFontSize}" 
                 VerticalAlignment="Stretch" Margin="10,15,0,0">
            <Label.Content>
              <Binding Path="SeriesName" Converter="{StaticResource StringFormatConverter}" ConverterParameter="{}{0}"/>
            </Label.Content>
          </Label>
          <Label Content=" – " Color="{ThemeResource TextColor}" FontFamily="DefaultBold" FontSize="{ThemeResource StandardFontSize}" Margin="10,15,0,0"/>
          <Label x:Name="EpisodeNameLabel" Color="{ThemeResource TextColor}" FontFamily="DefaultBold" FontSize="{ThemeResource StandardFontSize}"
                 VerticalAlignment="Stretch" Margin="10,15,0,0">
            <Label.Content>
              <Binding Path="EpisodeName" Converter="{StaticResource StringFormatConverter}" ConverterParameter="{}{0}"/>
            </Label.Content>
          </Label>
        </StackPanel>
      </ControlTemplate>
    </media:EpisodeAspectWrapper.Template>
  </media:EpisodeAspectWrapper>
  
  <!-- SERIES DESCRIPTION -->
          
  <media:VideoAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}" Grid.Column="0" Grid.Row="1">
    <media:VideoAspectWrapper.Template>
      <ControlTemplate>
        <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}">
          <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
          </Grid.RowDefinitions>
          <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*"/>
          </Grid.ColumnDefinitions>
          <Label x:Name="Description" Wrap="True" Scroll="Auto" ScrollDelay="8"
                 HorizontalAlignment="Stretch" Opacity="0.6"
                 Content="{Binding StoryPlot}" Color="{ThemeResource TextColor}" FontSize="{ThemeResource StandardFontSize}" Margin="10,5,-150,10"/>
        </Grid>
      </ControlTemplate>
    </media:VideoAspectWrapper.Template>
  </media:VideoAspectWrapper>

  
		  	 
    <!-- FirstAired -->
  <!--
  <media:MediaAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
        <media:MediaAspectWrapper.Template>
          <ControlTemplate>
              <Label x:Name="FirstAired" Color="{ThemeResource TextColor}" FontFamily="DefaultBold"
                     DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}" Margin="0,0,10,0"
                     Content="{Binding Path=RecordingTime, Converter={StaticResource DateFormatConverter}, ConverterParameter=d}" />
          </ControlTemplate>
        </media:MediaAspectWrapper.Template>
      </media:MediaAspectWrapper>
-->
    
<!--
    <Label x:Name="Genre" TextTrimming="WordEllipsis" Color="{ThemeResource TextColor}" Opacity="0.6">
      <Label.Content>
        <MultiBinding Converter="{StaticResource MultiAspectAttributeConverter}">
          <Binding Source="{StaticResource FanArtBackgroundModel}" Path="MediaItem"/>
          <Binding Source="{x:Static aspect:GenreAspect.ATTR_GENRE}" />
        </MultiBinding>
      </Label.Content>
    </Label>
-->

  <Grid Grid.Column="0" Grid.Row="2" Margin="10,0,0,0">
    <Grid.ColumnDefinitions>
    <ColumnDefinition Width="*"/>
    <ColumnDefinition Width="1120"/>
  </Grid.ColumnDefinitions>
            <!-- Rounded mask (stretches to fill Grid) -->
            <Border
                Name="mask"
                Background="White"
                CornerRadius="18"
				Opacity="0.25"
            />
  
  <StackPanel Grid.Column="0" Margin="0,-2,0,0" HorizontalAlignment="Center" VerticalAlignment="Center">
  
    <!-- 
	########################################################################################################################################
    ############################################################### RUNTIME ################################################################
	########################################################################################################################################
    -->
	
    <media:VideoStreamAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
      <media:VideoStreamAspectWrapper.Template>
        <ControlTemplate>
          <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}">
            <StackPanel Orientation="Horizontal">
              <Label x:Name="Duration" Color="{ThemeResource TextColor}" Opacity="1" FontSize="{ThemeResource DateTimeFontSize}" FontFamily="DefaultBold"
                     IsVisible="{Binding Path=Duration,Converter={StaticResource ReferenceToBoolConverter}}">
                <Label.Content>
                  <MultiBinding Converter="{StaticResource ExpressionMultiValueConverter}" ConverterParameter="{}{0}/60">
                    <Binding Path="Duration"/>
                  </MultiBinding>
                </Label.Content>
              </Label>
              <Label Content=" " IsVisible="{Binding Path=Duration,Converter={StaticResource ReferenceToBoolConverter}}"/>
              <Label Content="[Media.Minutes]" Color="{ThemeResource TextColor}" Opacity="1" FontSize="{ThemeResource DateTimeFontSize}" FontFamily="DefaultBold"
                     IsVisible="{Binding Path=Duration,Converter={StaticResource ReferenceToBoolConverter}}"/>
					 
            </StackPanel>
          </Grid>
        </ControlTemplate>
      </media:VideoStreamAspectWrapper.Template>
    </media:VideoStreamAspectWrapper>
  </StackPanel>
  </Grid>
  
  
  
<!--
      <media:UserDataWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}"
                             HorizontalAlignment="Left" VerticalAlignment="Center">
        <media:UserDataWrapper.Template>
          <ControlTemplate>
            <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}"
                  VerticalAlignment="Center" HorizontalAlignment="Left" Margin="15,5,0,5"
                  IsVisible="{Binding Path=PlayPercentage,Converter={StaticResource ReferenceToBoolConverter}}">
              <Grid.RowDefinitions>
                <RowDefinition Height="*"/>
              </Grid.RowDefinitions>
              <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
              </Grid.ColumnDefinitions>
              <ProgressBar Height="12" Width="120" HorizontalAlignment="Stretch" VerticalAlignment="Stretch" Margin="0,0,10,0"
                           Value="{Binding PlayPercentage}" Style="{ThemeResource PercentageProgressBarStyle}"/>
              <Label Grid.Column="1" Color="{ThemeResource TextColor}" Opacity="0.6" VerticalAlignment="Center"
                     FontSize="{ThemeResource SmallerFontSize}" Margin="0,0,0,5">
                <Label.Content>
                  <Binding Path="PlayPercentage" Converter="{StaticResource StringFormatConverter}" ConverterParameter="{}{0:F0}%"/>
                </Label.Content>
              </Label>
            </Grid>
          </ControlTemplate>
        </media:UserDataWrapper.Template>
      </media:UserDataWrapper>
-->
    
  
</Grid>

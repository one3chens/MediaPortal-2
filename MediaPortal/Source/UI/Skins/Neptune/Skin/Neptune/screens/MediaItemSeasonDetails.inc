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

  <media:SeasonAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}"
                              Grid.Column="0" Grid.Row="0" Margin="0,-10,-150,0" HorizontalAlignment="Stretch" VerticalAlignment="Top">
    <media:SeasonAspectWrapper.Template>
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
          <Label x:Name="SeasonLabel" Color="{ThemeResource TextColor}" Content="[Media.SeasonRootViewName]" Opacity="1" Margin="10,15,0,0" FontFamily="DefaultBold" FontSize="{ThemeResource StandardFontSize}" />
          <Label x:Name="SeasonNumber" Color="{ThemeResource TextColor}" Opacity="1" Margin="10,15,0,0" FontFamily="DefaultBold" FontSize="{ThemeResource StandardFontSize}" >
            <Label.Content>
              <Binding Path="Season" Converter="{StaticResource StringFormatConverter}" ConverterParameter="{} {0}"/>
            </Label.Content>
          </Label>
        </StackPanel>
      </ControlTemplate>
    </media:SeasonAspectWrapper.Template>
  </media:SeasonAspectWrapper>  
  
  
  <!-- SERIES DESCRIPTION -->
          
  <media:SeasonAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}" Grid.Column="0" Grid.Row="1">
    <media:SeasonAspectWrapper.Template>
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
                 Content="{Binding Description}" Color="{ThemeResource TextColor}" FontSize="{ThemeResource StandardFontSize}" Margin="10,5,-150,10"/>
        </Grid>
      </ControlTemplate>
    </media:SeasonAspectWrapper.Template>
  </media:SeasonAspectWrapper>  



  <Grid Grid.Column="0" Grid.Row="2" Margin="0,0,0,0">
    <Grid.ColumnDefinitions>
    <ColumnDefinition Width="*"/>
    <ColumnDefinition Width="1120"/>
  </Grid.ColumnDefinitions>
            <!-- Rounded mask (stretches to fill Grid) -->
			<!--
            <Border
                Name="mask"
                Background="White"
                CornerRadius="18"
				Opacity="0.25"
            />
			-->
  
  <StackPanel Grid.Column="0" Margin="10,0,0,0" HorizontalAlignment="Left" VerticalAlignment="Center">
  
    <!-- 
	########################################################################################################################################
    ############################################################ EPISODES NUMBER ###########################################################
	########################################################################################################################################
    -->
	<!--
    <media:MediaAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
      <media:MediaAspectWrapper.Template>
        <ControlTemplate>
          <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}">
            <StackPanel Orientation="Horizontal" DataContext="{Binding Source={StaticResource FanArtBackgroundModel},Path=SelectedItem}">
              <Label Content="{Binding Episodes}" Margin="0,0,8,0" Color="{ThemeResource TextColor}" Opacity="0.9" FontSize="{ThemeResource ListFontSize}" FontFamily="DefaultBold"/>
              <Label Content="[Media.EpisodeItemsHeader]" Margin="0,0,8,0" Color="{ThemeResource TextColor}" Opacity="0.9" FontSize="{ThemeResource ListFontSize}" FontFamily="DefaultBold"
                     IsVisible="{Binding Path=Episodes,Converter={StaticResource ReferenceToBoolConverter}}"/>
            </StackPanel>
          </Grid>
        </ControlTemplate>
      </media:MediaAspectWrapper.Template>
    </media:MediaAspectWrapper>
	-->
  </StackPanel>
  </Grid>
    


</Grid>

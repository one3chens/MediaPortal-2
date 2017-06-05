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
  
  <!-- MOVIE NAME -->
  <StackPanel Grid.Column="0" Grid.Row="0" Margin="0,-10,-150,0" HorizontalAlignment="Stretch" VerticalAlignment="Top">
  <StackPanel Orientation="Horizontal">
  <media:SeriesAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
    <media:SeriesAspectWrapper.Template>
      <ControlTemplate>
          <Label x:Name="SeriesNameLabel" Color="{ThemeResource TextColor}" FontFamily="DefaultBold" FontSize="{ThemeResource StandardFontSize}" 
		         Margin="10,15,0,0" VerticalAlignment="Top"
                 Content="{Binding SeriesName}" DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}"/>
      </ControlTemplate>
    </media:SeriesAspectWrapper.Template>
  </media:SeriesAspectWrapper>
  
  <media:MediaAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}" >
        <media:MediaAspectWrapper.Template>
          <ControlTemplate>
            <DockPanel DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}" LastChildFill="True">
              <Label x:Name="Year" HorizontalAlignment="Left" Margin="8,15,0,0" Color="{ThemeResource TextColor}" FontSize="{ThemeResource StandardFontSize}" FontFamily="DefaultBold"
                     Content="{Binding Path=RecordingTime, Converter={StaticResource DateFormatConverter}, ConverterParameter=yyyy}"/>
            </DockPanel>
          </ControlTemplate>
        </media:MediaAspectWrapper.Template>
      </media:MediaAspectWrapper>
  </StackPanel>
  </StackPanel>
  
  
   <!-- MOVIE DESCRIPTION -->
  
  <media:SeriesAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}" Grid.Column="0" Grid.Row="1">
    <media:SeriesAspectWrapper.Template>
      <ControlTemplate>
        <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}">
          <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
          </Grid.RowDefinitions>
          <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*"/>
          </Grid.ColumnDefinitions>
          <Label x:Name="MediaItemDescription" Wrap="True" Scroll="Auto" ScrollDelay="8"
                 HorizontalAlignment="Stretch" Opacity="0.6"
                 Content="{Binding Description}" Color="{ThemeResource TextColor}" FontSize="{ThemeResource StandardFontSize}" Margin="10,5,-150,10"/>
        </Grid>
      </ControlTemplate>
    </media:SeriesAspectWrapper.Template>
  </media:SeriesAspectWrapper>

	

	<!-- 
	########################################################################################################################################
    ################################### OTHER ITEM DETAILS LIKE RATING AND RUNTIME IN STACK PANEL ##########################################
	########################################################################################################################################
    -->	

  <Grid Grid.Column="0" Grid.Row="2" Margin="10,0,0,0">
    <Grid.ColumnDefinitions>
    <ColumnDefinition Width="*"/>
  </Grid.ColumnDefinitions>
           
		
  
  
  
    <!-- 
	########################################################################################################################################
    ############################################################# EPISODE INFO #############################################################
	########################################################################################################################################
    -->
	
  <Grid Grid.Column="0">
		<Grid.RowDefinitions>
		<RowDefinition Height="*"/>
	</Grid.RowDefinitions>
	<Grid.ColumnDefinitions>
		<ColumnDefinition Width="210"/>
		<ColumnDefinition Width="220"/>
	</Grid.ColumnDefinitions>
	
	 <!-- Rounded mask (stretches to fill Grid) -->
            <Border
                Name="mask"
                Background="White"
                CornerRadius="18"
				Opacity="0.25"
            />
	 <!-- Rounded mask (stretches to fill Grid) -->
            <Border Grid.Column="1"
                Name="mask"
                Background="White"
                CornerRadius="18"
				Opacity="0.25"
            />
  
                
      <StackPanel Orientation="Horizontal" HorizontalAlignment="Center" VerticalAlignment="Center" Grid.Column="0" DataContext="{Binding Source={StaticResource FanArtBackgroundModel},Path=SelectedItem}">
        <Label Content="{Binding Seasons}" Margin="0,-4,8,0" Color="{ThemeResource TextColor}" Opacity="0.6"/>
        <Label Content="[Media.SeasonItemsHeader]" Margin="0,-4,0,0" Color="{ThemeResource TextColor}" Opacity="0.6"
               IsVisible="{Binding Path=Seasons,Converter={StaticResource ReferenceToBoolConverter}}"/>
      </StackPanel>
      <StackPanel Orientation="Horizontal" HorizontalAlignment="Center" VerticalAlignment="Center" Grid.Column="1" DataContext="{Binding Source={StaticResource FanArtBackgroundModel},Path=SelectedItem}">
        <Label Content="{Binding Episodes}" Margin="0,-4,8,0" Color="{ThemeResource TextColor}" Opacity="0.6"/>
        <Label Content="[Media.EpisodeItemsHeader]" Margin="0,-4,0,0" Color="{ThemeResource TextColor}" Opacity="0.6"
               IsVisible="{Binding Path=Episodes,Converter={StaticResource ReferenceToBoolConverter}}"/>
      </StackPanel>
    </Grid>

  </Grid>	

  
  
  
<!--
  <StackPanel Grid.Column="0" Grid.Row="1" Margin="-10,0,0,0" HorizontalAlignment="Stretch" VerticalAlignment="Stretch">

    <StackPanel Orientation="Horizontal">
      
      <media:SeriesAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
        <media:SeriesAspectWrapper.Template>
          <ControlTemplate>
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Left" VerticalAlignment="Center" Margin="0,6,0,5"
                        DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}">
              <StarRatingPanel x:Name="RatingReadOnly" IsReadOnly="True" StarReadOnlyTemplate="{StaticResource StarReadOnlyTemplate}"
                               Maximum="5" VerticalAlignment="Center"
                               IsVisible="{Binding Path=TotalRating,Converter={StaticResource ReferenceToBoolConverter}}">
                <StarRatingPanel.Value>
                  <MultiBinding Converter="{StaticResource ExpressionMultiValueConverter}" ConverterParameter="{}{0}/2">
                    <Binding Path="TotalRating"/>
                  </MultiBinding>
                </StarRatingPanel.Value>
                <StarRatingPanel.LayoutTransform>
                  <TransformGroup>
                    <ScaleTransform ScaleX="1.7" ScaleY="1.5"/>
                  </TransformGroup>
                </StarRatingPanel.LayoutTransform>
              </StarRatingPanel>
              <Label x:Name="RatingCountLabel" Color="{ThemeResource TextColor}" Opacity="0.6" VerticalAlignment="Center"
                     IsVisible="{Binding Path=TotalRating,Converter={StaticResource ReferenceToBoolConverter}}"
                     FontSize="{ThemeResource SmallerFontSize}" Margin="15,0,5,0">
                <Label.Content>
                  <Binding Path="TotalRating" Converter="{StaticResource StringFormatConverter}" ConverterParameter="{}({0:F1})"/>
                </Label.Content>
              </Label>
            </StackPanel>
          </ControlTemplate>
        </media:SeriesAspectWrapper.Template>
      </media:SeriesAspectWrapper>
    </StackPanel>

    <Label x:Name="Genre" TextTrimming="WordEllipsis" Color="{ThemeResource TextColor}" Opacity="0.6">
      <Label.Content>
        <MultiBinding Converter="{StaticResource MultiAspectAttributeConverter}">
          <Binding Source="{StaticResource FanArtBackgroundModel}" Path="MediaItem"/>
          <Binding Source="{x:Static aspect:GenreAspect.ATTR_GENRE}" />
        </MultiBinding>
      </Label.Content>
    </Label> 
   

  </StackPanel>

  <media:UserDataWrapper Grid.Column="1" MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}"
                         HorizontalAlignment="Right" VerticalAlignment="Center">
    <media:UserDataWrapper.Template>
      <ControlTemplate>
        <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}"
              VerticalAlignment="Center" HorizontalAlignment="Right" Margin="0,-10,0,0"
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
                 IsVisible="{Binding Path=PlayPercentage,Converter={StaticResource ReferenceToBoolConverter}}"
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

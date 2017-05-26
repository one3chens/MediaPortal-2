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
  <media:MovieAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
    <media:MovieAspectWrapper.Template>
      <ControlTemplate>
        <DockPanel DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}" LastChildFill="True">
		
          <Label x:Name="MediaItemName" Margin="10,15,0,0" TextTrimming="WordEllipsis"
                  VerticalAlignment="Top" FontSize="{ThemeResource StandardFontSize}" FontFamily="DefaultBold"
                  Content="{Binding MovieName}" Color="{ThemeResource TextColor}"/>
        </DockPanel>
      </ControlTemplate>
    </media:MovieAspectWrapper.Template>
  </media:MovieAspectWrapper>
  
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
  
  <Label x:Name="MediaItemDescription" Wrap="True" Grid.Column="0" Grid.Row="1"
        TextTrimming="WordEllipsis" HorizontalAlignment="Stretch" Opacity="0.6"
        Content="{Binding Source={StaticResource FanArtBackgroundModel}, Path=ItemDescription}"
        Color="{ThemeResource TextColor}" FontSize="{ThemeResource StandardFontSize}" Margin="10,5,-150,10" />

		
	<!-- 
	########################################################################################################################################
    ################################### OTHER ITEM DETAILS LIKE RATING AND RUNTIME IN STACK PANEL ##########################################
	########################################################################################################################################
    -->	
  <!-- OTHER ITEM DETAILS LIKE RATING AND RUNTIME -->
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
	########################################################################################################################################
    ######################################################## PERCENT WATCHED ###############################################################
	########################################################################################################################################
    -->
	

		
		
</Grid>

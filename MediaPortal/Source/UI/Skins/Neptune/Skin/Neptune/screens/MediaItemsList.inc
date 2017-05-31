<?xml version="1.0" encoding="utf-8"?>
<Grid
    xmlns="www.team-mediaportal.com/2008/mpf/directx"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    DataContext="{Binding Source={Model 4CDD601F-E280-43b9-AD0A-6D7B2403C856},Path=NavigationData.CurrentScreenData}"
    > <!-- Model = MediaNavigationModel -->
  <Grid.Resources>
    <Model x:Key="MediaModel" Id="4CDD601F-E280-43b9-AD0A-6D7B2403C856"/>
  </Grid.Resources>
  <Grid.RowDefinitions>
    <RowDefinition Height="*"/>
  </Grid.RowDefinitions>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="*"/>
  </Grid.ColumnDefinitions>
  
  <Rectangle Fill="Black" Margin="0,-40,-2,-4" Stroke="Black" Opacity="0.35" Grid.Row="0" Grid.Column="0"/>

  <!-- All KeyBindings are included from shared resource -->
  <Include Source="screens\MediaItemsFilterKeyBindings.inc"/>

  <!--######################################### PAGE INDICATOR FOR LIST VIEWS ################################################-->
  
  <DockPanel DataContext="{Binding Source={Model 4CDD601F-E280-43b9-AD0A-6D7B2403C856},Path=NavigationData.CurrentScreenData}"
     VerticalAlignment="Top" HorizontalAlignment="Left" LastChildFill="false" Margin="25,-20,0,0"
     IsVisible="{Binding IsItemsValid}">
    <!---Grid+CoverView-->
    <StackPanel DockPanel.Dock="Left" Orientation="Horizontal">
      <Label HorizontalAlignment="Center" VerticalAlignment="Center" Margin="0,0,10,0" FontFamily="DefaultBold"
             Color="{ThemeResource TextColor}" FontSize="{ThemeResource SmallFontSize}" Opacity="0.6">
        <Label.Content>
          <MultiBinding Converter="{StaticResource GetIndexMultiConverter}" ConverterParameter="1">
            <Binding Source="{StaticResource FanArtBackgroundModel}" Path="SelectedItem"/>
            <Binding Source="{Binding ElementName=MediaItemsList}" Path="ItemsSource"/>
          </MultiBinding>
        </Label.Content>
      </Label>
      <Label Color="{ThemeResource TextColor}" FontFamily="DefaultBold" Content="| " FontSize="{ThemeResource SmallerFontSize}" Opacity="0.6"/>
      <Label Color="{ThemeResource TextColor}" FontFamily="DefaultBold" Content="{Binding NumItems}" FontSize="{ThemeResource SmallFontSize}" Opacity="0.6"/>
    </StackPanel>
  </DockPanel>

  <Grid HorizontalAlignment="Stretch" VerticalAlignment="Stretch" IsVisible="{Binding IsItemsValid}">
    <Grid.RowDefinitions>
      <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
	  <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>

    <ListView x:Name="MediaItemsList" Style="{ThemeResource MediaItemsListListViewStyle}" ItemsSource="{Binding Items}" RestoreFocus="True"
        HorizontalAlignment="Stretch" VerticalAlignment="Stretch" IsVisible="{Binding ShowList}" Margin="-1,0,-123,0" Grid.Column="1">
      <ListView.Resources>
        <!--<ResourceWrapper x:Key="VerticalScrollbarRenderTransform">
          <TranslateTransform X="40" />
        </ResourceWrapper>-->
        <Command x:Key="Menu_Command" Source="{StaticResource MediaModel}" Path="Select"
            Parameters="{LateBoundValue BindingValue={Binding}}"/>
      </ListView.Resources>
    </ListView>
    <Label IsVisible="{Binding ShowListHint}"
        HorizontalAlignment="Center" VerticalAlignment="Center" Wrap="true"
        Content="{Binding ListHint}" Color="{ThemeResource HintTextColor}" FontSize="{ThemeResource ListFontSize}"/>
  </Grid>
  <Label IsVisible="{Binding !IsItemsValid}"
      HorizontalAlignment="Center" VerticalAlignment="Center"
      Content="[Media.ViewInvalid]" Color="{ThemeResource ErrorTextColor}"/>
</Grid>


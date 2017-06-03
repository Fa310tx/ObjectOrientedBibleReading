using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ObjectOrientedBibleReading.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Settings : Page
	{
		public ApplicationDataContainer roamingsettings = ApplicationData.Current.RoamingSettings;

		public Settings()
		{
			this.InitializeComponent();
		}

		// what to do when the page is navigated to
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// handle the desktop titlebar back button
			Frame rootFrame = Window.Current.Content as Frame;
			// only show the back button if you can go back
			if (rootFrame.CanGoBack)
			{
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
			}
			else
			{
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
			}
		}

		// set TextBlock FontSize and then change if the user has preferences
		private void SetFontSize(object sender, RoutedEventArgs e)
		{
			var textblock = (TextBlock)sender;

			// load user preferences
			// get the device name
			var deviceinfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
			var devicename = deviceinfo.FriendlyName;
			// check to see if the preference is there
			if (roamingsettings.Values.ContainsKey(devicename + "_FontSize"))
			{
				var fontsize = Convert.ToInt32(roamingsettings.Values[devicename + "_FontSize"]);
				textblock.FontSize = fontsize;
				FontSizeComboBox.SelectedIndex = fontsize - 10;
			}
			else
			{
				// set a default
				textblock.FontSize = 20;
				FontSizeComboBox.SelectedIndex = 20;
			}
		}

		private void FontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var newfontsize = FontSizeComboBox.SelectedIndex + 10;
			LoremIpsum.FontSize = newfontsize;

			// store the user preferences
			// get the device name
			var deviceinfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
			var devicename = deviceinfo.FriendlyName;
			// set the preference
			roamingsettings.Values[devicename + "_FontSize"] = newfontsize.ToString();
		}

		// set TextBlock FontSize and then change if the user has preferences
		private void SetVersion(object sender, RoutedEventArgs e)
		{
			// load user preferences
			// check to see if the preference is there
			if (roamingsettings.Values.ContainsKey("Version"))
			{
				var version = roamingsettings.Values["Version"].ToString();
				VersionComboBox.SelectedItem = version.Remove(0, 2);
			}
			else
			{
				// set a default
				VersionComboBox.SelectedItem = "kjv";
			}
		}

		private void Version_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var version = "b_" + VersionComboBox.SelectedItem;

			// store the user preferences
			// set the preference
			roamingsettings.Values["Version"] = version.ToString();
		}
	}
}
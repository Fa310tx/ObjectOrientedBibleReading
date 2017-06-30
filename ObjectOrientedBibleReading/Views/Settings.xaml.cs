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

			List<string> FontSizes = new List<string>();
			for (int i = 0; i < 50; i++)
			{
				FontSizes.Add(i.ToString());
			}

			List<string> VersionAbbreviations = new List<string>
			{
				"asv1901",
				"bbe",
				"cjb",
				"cev",
				"darby",
				"erv",
				"gnv",
				"gw",
				"gnt",
				"hnv",
				"hcsb",
				"kjv",
				"mkjv",
				"nasb",
				"ncv",
				"net",
				"nirv",
				"nkjv",
				"nlt",
				"nrsv",
				"rsv",
				"amp",
				"esv",
				"tlb",
				"msg",
				"niv",
				"tev",
				"tniv",
				"wbt",
				"web",
				"ylt",
			};
			List<string> VersionNames = new List<string>
			{
				"American Standard Version",
				"Bible In Basic English",
				"Complete Jewish Bible",
				"Contemporary English Version",
				"Darby Bible",
				"Easy-To-Read Version",
				"Geneva Bible",
				"God's Word",
				"Good News Translation",
				"Hebrew Names Version Of The World English Bible",
				"Holman Christian Standard Bible",
				"King James Version",
				"Modern King James Version",
				"New American Standard Bible",
				"New Century Version",
				"New English Translation",
				"New International Reader's Version",
				"New King James Version",
				"New Living Translation",
				"New Revised Standard Version",
				"Revised Standard Version",
				"The Amplified Bible",
				"The English Standard Version",
				"The Living Bible",
				"The Message",
				"The New International Version",
				"Today's English Version",
				"Today's New International Version",
				"Webster's Bible Translation",
				"World English Bible",
				"Young's Literal Translation",
			};
			Dictionary<string, string> VersionDictionary = new Dictionary<string, string>()
			{
				{ "asv1901", "American Standard Version" },
				{ "bbe", "Bible In Basic English" },
				{ "cjb", "Complete Jewish Bible" },
				{ "cev", "Contemporary English Version" },
				{ "darby", "Darby Bible" },
				{ "erv", "Easy-To-Read Version" },
				{ "gnv", "Geneva Bible" },
				{ "gw", "God's Word" },
				{ "gnt", "Good News Translation" },
				{ "hnv", "Hebrew Names Version Of The World English Bible" },
				{ "hcsb", "Holman Christian Standard Bible" },
				{ "kjv", "King James Version" },
				{ "mkjv", "Modern King James Version" },
				{ "nasb", "New American Standard Bible" },
				{ "ncv", "New Century Version" },
				{ "net", "New English Translation" },
				{ "nirv", "New International Reader's Version" },
				{ "nkjv", "New King James Version" },
				{ "nlt", "New Living Translation" },
				{ "nrsv", "New Revised Standard Version" },
				{ "rsv", "Revised Standard Version" },
				{ "amp", "The Amplified Bible" },
				{ "esv", "The English Standard Version" },
				{ "tlb", "The Living Bible" },
				{ "msg", "The Message" },
				{ "niv", "The New International Version" },
				{ "tev", "Today's English Version" },
				{ "tniv", "Today's New International Version" },
				{ "wbt", "Webster's Bible Translation" },
				{ "web", "World English Bible" },
				{ "ylt", "Young's Literal Translation" },
			};

			// connect the data to the interface
			// data binding for today's chapter
			FontSizeComboBox.DataContext = FontSizes;
			// data binding for reading schedule
			VersionComboBox.DataContext = VersionAbbreviations;
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
				FontSizeComboBox.SelectedIndex = fontsize;
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
			var newfontsize = FontSizeComboBox.SelectedIndex;
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
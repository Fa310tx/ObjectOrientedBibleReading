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
			// take the dictionary<string, string>, turn it into an ienumerable<keyvaluepair<string, string>>, use linq to output the contents, format a new string based on those contents, turn it into list<string>
			// "Bible In Basic English (bbe)
			var VersionList = VersionDictionary.AsEnumerable().Select(x => string.Format("{0} ({1})", x.Value, x.Key.ToUpper())).ToList();

			// connect the data to the interface
			// data binding for today's chapter
			FontSizeComboBox.DataContext = FontSizes;
			// data binding for reading schedule
			VersionComboBox.DataContext = VersionList;
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

				switch (version)
				{
					case "b_asv1901":
						VersionComboBox.SelectedIndex = 0;
						break;
					case "b_bbe":
						VersionComboBox.SelectedIndex = 1;
						break;
					case "b_cjb":
						VersionComboBox.SelectedIndex = 2;
						break;
					case "b_cev":
						VersionComboBox.SelectedIndex = 3;
						break;
					case "b_darby":
						VersionComboBox.SelectedIndex = 4;
						break;
					case "b_erv":
						VersionComboBox.SelectedIndex = 5;
						break;
					case "b_gnv":
						VersionComboBox.SelectedIndex = 6;
						break;
					case "b_gw":
						VersionComboBox.SelectedIndex = 7;
						break;
					case "b_gnt":
						VersionComboBox.SelectedIndex = 8;
						break;
					case "b_hnv":
						VersionComboBox.SelectedIndex = 9;
						break;
					case "b_hcsb":
						VersionComboBox.SelectedIndex = 10;
						break;
					case "b_kjv":
						VersionComboBox.SelectedIndex = 11;
						break;
					case "b_mkjv":
						VersionComboBox.SelectedIndex = 12;
						break;
					case "b_nasb":
						VersionComboBox.SelectedIndex = 13;
						break;
					case "b_ncv":
						VersionComboBox.SelectedIndex = 14;
						break;
					case "b_net":
						VersionComboBox.SelectedIndex = 15;
						break;
					case "b_nirv":
						VersionComboBox.SelectedIndex = 16;
						break;
					case "b_nkjv":
						VersionComboBox.SelectedIndex = 17;
						break;
					case "b_nlt":
						VersionComboBox.SelectedIndex = 18;
						break;
					case "b_nrsv":
						VersionComboBox.SelectedIndex = 19;
						break;
					case "b_rsv":
						VersionComboBox.SelectedIndex = 20;
						break;
					case "b_amp":
						VersionComboBox.SelectedIndex = 21;
						break;
					case "b_esv":
						VersionComboBox.SelectedIndex = 22;
						break;
					case "b_tlb":
						VersionComboBox.SelectedIndex = 23;
						break;
					case "b_msg":
						VersionComboBox.SelectedIndex = 24;
						break;
					case "b_niv":
						VersionComboBox.SelectedIndex = 25;
						break;
					case "b_tev":
						VersionComboBox.SelectedIndex = 26;
						break;
					case "b_tniv":
						VersionComboBox.SelectedIndex = 27;
						break;
					case "b_wbt":
						VersionComboBox.SelectedIndex = 28;
						break;
					case "b_web":
						VersionComboBox.SelectedIndex = 29;
						break;
					case "b_ylt":
						VersionComboBox.SelectedIndex = 30;
						break;
					default:
						// default is kjv
						VersionComboBox.SelectedIndex = 11;
						break;
				}
			}
			else
			{
				// set a default
				VersionComboBox.SelectedIndex = 11;
			}
		}

		private void Version_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var selecteditem = VersionComboBox.SelectedItem.ToString();
			// single quotes make it type Char while double quotes make it type String
			Char delimiter = '(';
			// create an array from the string separated by the delimiter
			String[] selecteditemparts = selecteditem.Split(delimiter);
			// take off the beginning character
			var selecteditemvalue = selecteditemparts[0].Substring(0, (selecteditemparts[0].Length - 1));
			// take off the beginning and ending character
			var selecteditemkey = selecteditemparts[1].Substring(0, (selecteditemparts[1].Length - 1));
			var version = "b_" + selecteditemkey.ToLower();

			// store the user preferences
			// set the preference
			roamingsettings.Values["Version"] = version.ToString();
		}
	}
}
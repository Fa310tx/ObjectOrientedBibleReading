using PortableClasses;
using PortableClasses.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
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
	public sealed partial class MainPage : Page
	{
		public ObservableCollection<ChapterItem> ChapterCollection { get; set; }
		public ObservableCollection<VerseItem> VerseCollection { get; set; }

		// load user preferences
		public ApplicationDataContainer roamingsettings = ApplicationData.Current.RoamingSettings;

		public MainPage()
		{
			this.InitializeComponent();

			ChapterCollection = new ObservableCollection<ChapterItem>();
			VerseCollection = new ObservableCollection<VerseItem>();
		}

		// what to do when the page is navigated to
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ChapterCollection.Clear();
			VerseCollection.Clear();

			// check to see if the preference is there
			if (roamingsettings.Values.ContainsKey("Version"))
			{
				var version = roamingsettings.Values["Version"].ToString();
				await MainPageViewModel.PopulateCollections(ChapterCollection, VerseCollection, version);
			}
			else
			{
				await MainPageViewModel.PopulateCollections(ChapterCollection, VerseCollection);
			}

			// Initialize the share contract
			RegisterForShare();

			// register the background tasks
			RegisterTasks();

			// shows the StatusBar
			ShowStatusBar();
		}

		// click events
		// commandbar primary buttons
		// refresh button
		private async void Refresh(object sender, RoutedEventArgs e)
		{
			ChapterCollection.Clear();
			VerseCollection.Clear();

			// check to see if the preference is there
			if (roamingsettings.Values.ContainsKey("Version"))
			{
				var version = roamingsettings.Values["Version"].ToString();
				await MainPageViewModel.PopulateCollections(ChapterCollection, VerseCollection, version);
			}
			else
			{
				await MainPageViewModel.PopulateCollections(ChapterCollection, VerseCollection);
			}
		}

		// share button
		private void Share(object sender, RoutedEventArgs e)
		{
			DataTransferManager.ShowShareUI();
		}

		// share content
		private void RegisterForShare()
		{
			DataTransferManager datatransfermanager = DataTransferManager.GetForCurrentView();
			datatransfermanager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.ShareTextHandler);
		}
		private async void ShareTextHandler(DataTransferManager sender, DataRequestedEventArgs e)
		{
			DataRequest request = e.Request;
			// The Title is mandatory
			request.Data.Properties.Title = "Bible Verse";
			request.Data.Properties.Description = "This Bible verse was shared from the Daily Bible Reading app.";
			// see if a verse is selected
			try
			{
				// get the selected content
				// the extra () part is called casting
				// in this instance, todayschapter.SelectedItem is of type Object and type String doesn't know what to do with it
				// ToString() was attempted, but it gave the object's name instead if its content
				var reference = ((VerseItem)Verses.SelectedItem).VerseReference;
				var text = ((VerseItem)Verses.SelectedItem).text;
				string selected = reference + "\r\n" + text;
				// share text
				request.Data.SetText(selected);
				// share html
				/* not using as it allows less sharing options
				string html = "<p><b>" + reference + "</b><br/>" + text + "</p>";
				string htmlformat = HtmlFormatHelper.CreateHtmlFormat(html);
				request.Data.SetHtmlFormat(htmlformat);
				*/
			}
			catch
			{
				// pop up an error message
				MessageDialog dialog = new MessageDialog("Please select a verse.");
				await dialog.ShowAsync();
			}
		}

		// create the text for the verses
		// StringBuilder is a much more effecient way to concatenate strings than string = string + string
		StringBuilder texttoplay = new StringBuilder();
		private void CreateSsml()
		{
			// empty the string
			texttoplay.Clear();

			// create text for the audio player
			// ssml allows things like language, voice, and speed
			texttoplay.Append("<?xml version='1.0' encoding='utf-8'?>");
			texttoplay.Append("<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-us'>");
			texttoplay.Append("<prosody rate='0.85'>");
			// fill in the content from the ObservableCollection
			foreach (VerseItem verse in VerseCollection)
			{
				// <s> defines a sentence
				texttoplay.Append("<s>" + verse.text + "</s>");
			}
			texttoplay.Append("</prosody>");
			texttoplay.Append("</speak>");
		}
		// audio controls
		// play for the first time
		private void PlayFirstText(object sender, RoutedEventArgs e)
		{
			// create the SSML for the audio player
			CreateSsml();

			// play the text
			Helper.PlaySsmlAsync(TextPlayer, texttoplay.ToString());
			TextPlayer.Play();
			PlayFirstButton.Visibility = Visibility.Collapsed;
			PlayFromPauseButton.Visibility = Visibility.Collapsed;
			PauseButton.Visibility = Visibility.Visible;
		}
		// pause what is playing
		private void PauseText(object sender, RoutedEventArgs e)
		{
			TextPlayer.Pause();
			PlayFirstButton.Visibility = Visibility.Collapsed;
			PlayFromPauseButton.Visibility = Visibility.Visible;
			PauseButton.Visibility = Visibility.Collapsed;
		}
		// start playing again
		private void PlayPausedText(object sender, RoutedEventArgs e)
		{
			TextPlayer.Play();
			PlayFirstButton.Visibility = Visibility.Collapsed;
			PlayFromPauseButton.Visibility = Visibility.Collapsed;
			PauseButton.Visibility = Visibility.Visible;
		}
		// when playing is finished
		private void PlayEnded(object sender, RoutedEventArgs e)
		{
			PlayFirstButton.Visibility = Visibility.Visible;
			PlayFromPauseButton.Visibility = Visibility.Collapsed;
			PauseButton.Visibility = Visibility.Collapsed;
		}

		// commandbar secondary buttons
		// website
		private void OpenBibleReadingPage(object sender, RoutedEventArgs e)
		{
			Helper.OpenExternalPage("http://flcbranson.org/php/mlmBibleReading.php?site=flcb&pageKey=head4");
		}

		// feedback
		private void SendEmail(object sender, RoutedEventArgs e)
		{
			Helper.SendEmail("jj@anti-exe.com", "Daily%20Bible%20Reading");
		}

		// more apps
		private void MoreFromDeveloper(object sender, RoutedEventArgs e)
		{
			Helper.MoreFromDeveloper("Joshua Jackson");
		}

		// settings page
		private void Settings(object sender, RoutedEventArgs e)
		{
			if (this.Frame != null)
			{
				this.Frame.Navigate(typeof(Settings));
			}
		}

		// hamburger button (only use with SplitView)
		private void SplitViewButton_Click(object sender, RoutedEventArgs e)
		{
			// toggles the visibility of SplitView.Pane
			MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
		}

		// selecting a chapter
		private void Chapter_ItemClick(object sender, ItemClickEventArgs e)
		{
			// clear the collection
			VerseCollection.Clear();

			// the object sent from the click
			var selectedchapter = (ChapterItem)e.ClickedItem;

			// iterate through the object
			foreach (var verse in selectedchapter.verses)
			{
				var verseitem = new VerseItem();

				verseitem.VerseReference = selectedchapter.ChapterReference + ":" + verse.verse;
				verseitem.verse = verse.verse;
				verseitem.text = verse.text;

				// add the item
				VerseCollection.Add(verseitem);
			}

			// store that a chapter has been read (or, at least, clicked)
			roamingsettings.Values[selectedchapter.date] = "hasbeenread";
			selectedchapter.HasBeenRead = true;
		}

		// initialize background tasks
		private void RegisterTasks()
		{
			Helper.RegisterBackgroundTask("BackgroundTasks.DownloadContent", "DownloadContent", new TimeTrigger(60, false), null);
			Helper.RegisterBackgroundTask("BackgroundTasks.UpdateTiles", "UpdateTiles", new TimeTrigger(60, false), null);
		}

		// show the StatusBar
		private async void ShowStatusBar()
		{
			// if the device even has a status bar
			if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
			{
				// you don't want to type this over and over
				var statusbar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
				// shows the StatusBar (alternatively HideAsync() hides it)
				await statusbar.ShowAsync();
				statusbar.BackgroundColor = Windows.UI.Colors.Transparent;
				//statusbar.BackgroundOpacity = 1;
				//statusbar.ForegroundColor = Windows.UI.Colors.Black;
			}
		}

		// set TextBlock FontSize and then change if the user has preferences
		private void SetFontSize(object sender, RoutedEventArgs e)
		{
			var textblock = (TextBlock)sender;
			// set a default
			textblock.FontSize = 20;

			// load user preferences
			// get the device name
			var deviceinfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
			var devicename = deviceinfo.FriendlyName;
			// check to see if the preference is there
			if (roamingsettings.Values.ContainsKey(devicename + "_FontSize"))
			{
				var fontsize = Convert.ToInt32(roamingsettings.Values[devicename + "_FontSize"]);
				textblock.FontSize = fontsize;
			}
		}
	}
}
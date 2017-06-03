using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace PortableClasses.ViewModels
{
	public class TileViewModel
	{
		private static void UpdateTiles(string _reference, string _date)
		{
			// adaptive tiles (only works with windows 10)
			// create an xml string
			string xml = "<tile>" +
"	<visual branding='nameAndLogo'>" +
"		<binding template='TileSmall' branding='none'>" +
"           <image placement='background' src='Assets/Square71x71Logo.png'/>" +
"        </binding>" +
"		<binding template='TileMedium' displayName='Bible Reading'>" +
//"           <image placement='peek' src='Assets/Square150x150Logo.png'/>" +
"           <text hint-style='captionsubtle'>" + DateTime.Parse(_date).ToString("dddd") + "</text>" +
"			<text hint-wrap='true' hint-style='base'>" + _reference + "</text>" +
"		</binding>" +
"		<binding template='TileWide'>" +
"           <image placement='peek' src='Assets/Wide310x150Logo.png'/>" +
"           <text hint-style='captionsubtle'>" + DateTime.Parse(_date).ToString("D") + "</text>" +
"			<text hint-wrap='true' hint-style='base'>" + _reference + "</text>" +
"		</binding>" +
"		<binding template='TileLarge'>" +
"           <image src='Assets/Wide310x150Logo.png'/>" +
"           <text hint-style='captionsubtle'>" + DateTime.Parse(_date).ToString("D") + "</text>" +
"			<text hint-wrap='true' hint-style='subtitle'>" + _reference + "</text>" +
"		</binding>" +
"	</visual>" +
"</tile>";

			// turn that string into an xml object
			XmlDocument tilexml = new XmlDocument();
			tilexml.LoadXml(xml);

			// update the tile with the xml object
			var updater = TileUpdateManager.CreateTileUpdaterForApplication();
			updater.EnableNotificationQueue(true);
			updater.Clear();
			updater.Update(new TileNotification(tilexml));
		}

		// update the live tiles
		internal static void TileContent(RootObject rootobject)
		{
			// great examples of working with dates (http://www.dotnetperls.com/datetime)
			string today = DateTime.Today.ToString("yyyy-MM-dd");
			DateTime date = DateTime.Today;
			DayOfWeek day = date.DayOfWeek;

			// no chapter on the weekend
			if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
			{
				string chapterversereference = "No chapter today";

				// update the live tile
				UpdateTiles(chapterversereference, today);
			}
			else
			{
				// iterate through the chapters
				foreach (var chapter in rootobject.chapters)
				{
					// do something with the results
					// Output to debugger
					//Debug.WriteLine("DeserializeJson-Result: " + chapter.date + " - " + chapter.book + "  " + chapter.chapter);
					// create a popup with the results
					//MessageDialog dialog = new MessageDialog("DeserializeJson-Result: " + chapter.date + " - " + chapter.book + "  " + chapter.chapter);
					//await dialog.ShowAsync();

					// reading schedule
					// great examples of working with dates (http://www.dotnetperls.com/datetime)
					string chapterdate = chapter.date;
					// Friday, November 3, 2013
					string chapterdatereadable = DateTime.Parse(chapterdate).ToString("D");
					// 3/23/2012 10:24 PM
					string chapterdatecurrent = DateTime.Parse(chapterdate).ToString("d") + " " + DateTime.Now.ToString("t");
					string chapterbook = chapter.book;
					int chapterchapter = chapter.chapter;
					string chapterreference = chapterbook + " " + chapterchapter;

					// update the live tile
					if (chapterdate == today)
					{
						// update the live tile
						UpdateTiles(chapterreference, chapterdate);
					}
				}
			}
		}

		public static async void GetTileContentAsync()
		{
			// the the daily bible reading information for the whole week surrounding today
			// great examples of working with dates (http://www.dotnetperls.com/datetime)
			DateTime today = DateTime.Today;
			// use just today by default
			DateTime begindate = today;
			DateTime enddate = today;
			DayOfWeek day = today.DayOfWeek;
			// redefine variables
			if (day == DayOfWeek.Sunday)
			{
				begindate = today.AddDays(-6);
				enddate = today.AddDays(5);
			}
			if (day == DayOfWeek.Monday)
			{
				begindate = today;
				enddate = today.AddDays(4);
			}
			if (day == DayOfWeek.Tuesday)
			{
				begindate = today.AddDays(-1);
				enddate = today.AddDays(3);
			}
			if (day == DayOfWeek.Wednesday)
			{
				begindate = today.AddDays(-2);
				enddate = today.AddDays(2);
			}
			if (day == DayOfWeek.Thursday)
			{
				begindate = today.AddDays(-3);
				enddate = today.AddDays(1);
			}
			if (day == DayOfWeek.Friday)
			{
				begindate = today.AddDays(-4);
				enddate = today;
			}
			if (day == DayOfWeek.Saturday)
			{
				begindate = today.AddDays(-5);
				enddate = today.AddDays(6);
			}

			var apiresults = await Api.GetApiResultsAsync(begindate, enddate);

			// update the live tile
			TileContent(apiresults);
		}
	}
}
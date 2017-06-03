using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace PortableClasses.ViewModels
{
	public class MainPageViewModel
	{
		public async static Task PopulateCollections(ObservableCollection<ChapterItem> chaptercollection, ObservableCollection<VerseItem> versecollection, string version = "b_kjv")
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

			var apiresults = await Api.GetApiResultsAsync(begindate, enddate, version);
			var chapters = apiresults.chapters;
			foreach (var chapter in chapters)
			{
				var chapteritem = new ChapterItem();

				chapteritem.book = chapter.book;
				chapteritem.chapter = chapter.chapter;
				chapteritem.ChapterReference = chapteritem.book + " " + chapteritem.chapter;
				// great examples of working with dates (http://www.dotnetperls.com/datetime)
				// chapter.date is 2001-12-25 and chapteritem.date is Sunday, December 25, 2001
				chapteritem.date = DateTime.Parse(chapter.date).ToString("D");
				// check to see if the user has read the chapter
				ApplicationDataContainer roamingsettings = ApplicationData.Current.RoamingSettings;
				if (roamingsettings.Values.ContainsKey(chapteritem.date))
				{
					chapteritem.HasBeenRead = true;
				}
				else
				{
					chapteritem.HasBeenRead = false;
				}
				// check to see if the chapter is today's chapter
				if (chapteritem.date == today.ToString("D"))
				{
					chapteritem.IsTodaysChapter = true;
					chapteritem.HasBeenRead = true;
					// store that a chapter has been read
					roamingsettings.Values[chapteritem.date] = "hasbeenread";
				}
				else
				{
					chapteritem.IsTodaysChapter = false;
				}
				chapteritem.verses = chapter.verses;
				chapteritem.version = chapter.version;
				chaptercollection.Add(chapteritem);

				if (chapteritem.IsTodaysChapter == true)
				{
					foreach (var verse in chapteritem.verses)
					{
						var verseitem = new VerseItem();

						verseitem.text = verse.text;
						verseitem.verse = verse.verse;
						verseitem.VerseReference = chapteritem.ChapterReference + ":" + verseitem.verse;

						versecollection.Add(verseitem);
					}
				}

				// update the live tile
				TileViewModel.TileContent(apiresults);
			}

			// no chapter on the weekend
			// great examples of working with dates (http://www.dotnetperls.com/datetime)
			if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
			{
				var verseitem = new VerseItem();

				verseitem.text = "Come join us in reading the Bible this year. We read one chapter of the Bible each weekday (Monday through Friday).";
				verseitem.VerseReference = "No chapter today";

				versecollection.Add(verseitem);
			}
		}
	}

	public class VerseItem : Verse
	{
		public string VerseReference { get; set; }
	}
	public class ChapterItem : Chapter
	{
		public bool HasBeenRead { get; set; }
		public bool IsTodaysChapter { get; set; }
		public string ChapterReference { get; set; }
	}

	// set the color of today's chapter in the list
	public sealed class HasBeenReadConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value != null && (bool)value)
			{
				// brown is more like a pastel red
				return new SolidColorBrush(Colors.Gray);
			}
			else
			{
				return new SolidColorBrush(Colors.White);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return true;
		}
	}

	// set the color of today's chapter in the list
	public sealed class TodaysChapterConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value != null && (bool)value)
			{
				// brown is more like a pastel red
				return new SolidColorBrush(Colors.Brown);
			}
			else
			{
				return new SolidColorBrush(Colors.Transparent);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return true;
		}
	}
}
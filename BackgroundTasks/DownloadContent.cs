using Newtonsoft.Json;
using PortableClasses;
using System;
using Windows.ApplicationModel.Background;

namespace BackgroundTasks
{
    public sealed class DownloadContent : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // wait for the background task to complete
            var defferal = taskInstance.GetDeferral();

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

			// define variables
			string result = null;
            string textfile = "ApiResults.txt";

			// connect to the api and get the results
			result = await Helper.GetJsonAsync("http://www.anti-exe.com/api/dailybiblereading?begindate=" + begindate + "?enddate=" + enddate);
			// do something with the results
			// Output to debugger
			//Debug.WriteLine("ConnectToJson-Result: " + result);
			// create a popup with the results
			//MessageDialog dialog1 = new MessageDialog("GetJson Result: " + result);
			//await dialog1.ShowAsync();

            // if there is content
            if (result != null)
            {
                // write to a cache file
                await Helper.WriteTextFileAsync(textfile, result);
            }

            // say it's completed
            defferal.Complete();
        }
    }
}
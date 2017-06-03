using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace PortableClasses
{
    public class Helper
    {
        // opens default web browser
        public static async void OpenExternalPage(string _url)
        {
            Uri url = new Uri(_url);
            await Windows.System.Launcher.LaunchUriAsync(url);
        }

        // opens default e-mail client
        // When creating an email link, as with any HTML link, you should use &amp; to represent the ampersand (&).
        // It's also good practice to use %20 in place of all spaces and %0D%0A in place of all carriage returns (i.e. when you want a space, such as when you'd normally use the "Enter" key).
        // When I say that it's "good practice" to do this, I mean, some browsers/email clients behave differently and may or may not show spaces and carriage returns the way you'd like.Therefore, using the method outlined here will ensure that your users see what you intend them to see.
        // example:  mailto:jj@anti-exe.com?Subject=Daily%20Bible%20Reading%20&amp;body=Your%20app%20is%20a%20load%20of%20hooey
        public static async void SendEmail(string _address)
        {
            Uri url = new Uri("mailto:" + _address);
            await Windows.System.Launcher.LaunchUriAsync(url);
        }
        // create additional overloaded methods to allow optional parameters
        public static async void SendEmail(string _address, string _subject)
        {
            Uri url = new Uri("mailto:" + _address + "?subject=" + _subject);
            await Windows.System.Launcher.LaunchUriAsync(url);
        }
        public static async void SendEmail(string _address, string _subject, string _body)
        {
            Uri url = new Uri("mailto:" + _address + "?Subject=" + _subject + "&amp;body=" + _body);
            await Windows.System.Launcher.LaunchUriAsync(url);
        }

        // opens store and shows items by publisher
        public static async void MoreFromDeveloper(string _developer)
        {
            // windows 8
            //Uri url = new Uri("zune:search?publisher=" + _developer);
            // windows 10
            Uri url = new Uri("ms-windows-store://publisher/?name=Joshua Jackson");
            await Windows.System.Launcher.LaunchUriAsync(url);
        }

        // connect to a json api and return the results
        public static async Task<string> GetJsonAsync(string _url)
        {
            // declare an empty variable to be filled later
            string result = null;
            HttpClient httpclient = new HttpClient();
            Uri url = new Uri(_url);

            // Always catch network exceptions for async methods
            try
            {
                result = await httpclient.GetStringAsync(url);

                // do something with the results
                // Output to debugger
                //Debug.WriteLine(result);
                // create a popup with the results
                //MessageDialog dialog = new MessageDialog(result);
                //await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // Details in ex.Message and ex.HResult.
                // do something with the results
                // Output to debugger
                Debug.WriteLine(ex);
                // create a popup with the results
                //MessageDialog dialog = new MessageDialog(result);
                //await dialog.ShowAsync();
            }

            // Once your app is done using the HttpClient object call dispose to free up system resources (the underlying socket and memory used for the object)
            httpclient.Dispose();

            // return the string to Task<>
            return result;
        }

        // read a text file from the app's local folder
        public static async Task<string> ReadTextFileAsync(string _filename)
        {
            // declare an empty variable to be filled later
            string result = null;
            // define where the file resides
            StorageFolder localfolder = ApplicationData.Current.LocalFolder;
            // see if the file exists
            try
            {
                // open the file
                StorageFile textfile = await localfolder.GetFileAsync(_filename);
                // read the file
                result = await FileIO.ReadTextAsync(textfile);
            }
            // if the file doesn't exist
            catch (Exception ex)
            {
                // do something with the results
                // Output to debugger
                Debug.WriteLine(ex);
                // create a popup with the results
                //MessageDialog dialog = new MessageDialog("File doesn't exist");
                //await dialog.ShowAsync();
            }
            // return the contents of the file
            return result;
        }

        // write a text file to the app's local folder
        public static async Task<string> WriteTextFileAsync(string _filename, string _content)
        {
            // declare an empty variable to be filled later
            string result = null;
            try
            {
                // define where the file resides
                StorageFolder localfolder = ApplicationData.Current.LocalFolder;
                // replace the file if it exists
                StorageFile textfile = await localfolder.CreateFileAsync(_filename, CreationCollisionOption.ReplaceExisting);
                // write to the file
                await FileIO.WriteTextAsync(textfile, _content);
                result = _content;
            }
            // if there was a problem
            catch (Exception ex)
            {
                // do something with the results
                // Output to debugger
                Debug.WriteLine(ex);
                // create a popup with the results
                //MessageDialog dialog = new MessageDialog("File doesn't exist");
                //await dialog.ShowAsync();
            }
            // return the contents of the file
            return result;
        }

        // register a new background task
        public static async void RegisterBackgroundTask(string _entrypoint, string _name, IBackgroundTrigger _trigger, IBackgroundCondition _condition)
        {
            // Windows Phone requires RequestAccessAsync
            var result = await BackgroundExecutionManager.RequestAccessAsync();
            if (result == BackgroundAccessStatus.AllowedSubjectToSystemPolicy || result == BackgroundAccessStatus.AlwaysAllowed)
            {
                // check all the tasks that are registered
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    // if this one is...
                    if (task.Value.Name == _name)
                    {
                        // unregister it
                        task.Value.Unregister(true);
                    }
                }

                // create a new task
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                builder.Name = _name;
                builder.TaskEntryPoint = _entrypoint;
                builder.SetTrigger(_trigger);
                if (_condition != null)
                {
                    builder.AddCondition(_condition);
                }

                // register the new task
                var registration = builder.Register();
            }
        }

		/* doesn't work as well as mine
        // utility background task registration (https://msdn.microsoft.com/en-us/library/windows/apps/xaml/JJ553413(v=win.10).aspx)
        // takes no account of windows phone requirements
        // not sure how it handles existing tasks
        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint, string taskName, IBackgroundTrigger trigger, IBackgroundCondition condition)
        {
            // Check for existing registrations of this background task.
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == taskName)
                {
                    // The task is already registered.
                    return (BackgroundTaskRegistration)(cur.Value);
                }
            }

            // Register the background task.
            var builder = new BackgroundTaskBuilder();
            builder.Name = taskName;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = builder.Register();

            return task;
        }
		*/

		// have the app speak a text string
		public static async void PlayTextAsync(MediaElement _mediaelement, string _texttoplay)
		{
			// enable the Microphone capibility in the manifest
			// add using Windows.Media.SpeechSynthesis
			// create a SpeechSynthesizer to convert the text to the speech
			SpeechSynthesizer speechsynthesizer = new SpeechSynthesizer();
			// create the stream of the converted text
			SpeechSynthesisStream speechsynthesisstream = await speechsynthesizer.SynthesizeTextToStreamAsync(_texttoplay);
			// set the play source to the stream
			_mediaelement.SetSource(speechsynthesisstream, speechsynthesisstream.ContentType);
		}
		// have the app speak an ssml string
		public static async void PlaySsmlAsync(MediaElement _mediaelement, string _texttoplay)
        {
            // enable the Microphone capibility in the manifest
            // add using Windows.Media.SpeechSynthesis
            // create a SpeechSynthesizer to convert the text to the speech
            SpeechSynthesizer speechsynthesizer = new SpeechSynthesizer();
            // create the stream of the converted text
            SpeechSynthesisStream speechsynthesisstream = await speechsynthesizer.SynthesizeSsmlToStreamAsync(_texttoplay);
            // set the play source to the stream
            _mediaelement.SetSource(speechsynthesisstream, speechsynthesisstream.ContentType);
        }
    }
}

using PortableClasses.ViewModels;
using System;
using Windows.ApplicationModel.Background;

namespace BackgroundTasks
{
    public sealed class UpdateTiles : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // wait for the background task to complete
            var defferal = taskInstance.GetDeferral();

			TileViewModel.GetTileContentAsync();

			// say it's completed
			defferal.Complete();
        }
    }
}
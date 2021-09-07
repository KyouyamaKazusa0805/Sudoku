#if SIMPLE_PROGRAM
using System.Threading;
using WinRT;
using Microsoft.UI.Dispatching;
using Sudoku.UI;

ComWrappersSupport.InitializeComWrappers();
Application.Start(static ([Discard] p) =>
{
	var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
	SynchronizationContext.SetSynchronizationContext(context);
	_ = new App();
});
#else
using System.Threading;
using Microsoft.UI.Dispatching;
using WinRT;

namespace Sudoku.UI;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		ComWrappersSupport.InitializeComWrappers();
		Application.Start(static ([Discard] p) =>
		{
			var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
			SynchronizationContext.SetSynchronizationContext(context);
			_ = new App();
		});
	}
}

#endif
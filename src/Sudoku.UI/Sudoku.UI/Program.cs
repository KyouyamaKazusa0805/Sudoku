#pragma warning disable IDE0005

using System.Threading;
using Microsoft.UI.Dispatching;
using Sudoku.UI;
using WinRT;

ApplicationInitializationCallback callback = static ([Discard] p) =>
{
	var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
	SynchronizationContext.SetSynchronizationContext(context);
	_ = new App();
};

ComWrappersSupport.InitializeComWrappers();
Application.Start(callback);
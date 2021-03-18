using System;

namespace Codexcite.DurableFunctions.Extensions
{
	public static class CompositeDisposableExtensions
	{
		public static IDisposable DisposeWith(this IDisposable disposable, System.Reactive.Disposables.CompositeDisposable compositeDisposable)
		{
			compositeDisposable.Add(disposable);
			return disposable;
		}
	}
}

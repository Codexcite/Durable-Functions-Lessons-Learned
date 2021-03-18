using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Serilog.Context;

namespace Codexcite.DurableFunctions.Extensions
{
	internal static class DurableContextsExtensions
	{
		public static IDisposable PushLoggerProperties(this IDurableOrchestrationContext context)
		{
			var disposable = new CompositeDisposable();
			LogContext.PushProperty($"durable_{nameof(context.InstanceId)}", context.InstanceId).DisposeWith(disposable);
			LogContext.PushProperty($"durable_{nameof(context.ParentInstanceId)}", context.ParentInstanceId).DisposeWith(disposable);
			LogContext.PushProperty($"durable_{nameof(context.IsReplaying)}", context.IsReplaying).DisposeWith(disposable);
			LogContext.PushProperty($"durable_{nameof(context.Name)}", context.Name).DisposeWith(disposable);
			return disposable;
		}

		public static IDisposable PushLoggerProperties(this IDurableActivityContext context)
		{
			return LogContext.PushProperty($"durable_{nameof(context.InstanceId)}", context.InstanceId);
		}
		public static IDisposable PushLoggerProperties(this IDurableEntityContext context)
		{
			var disposable = new CompositeDisposable();
			LogContext.PushProperty($"durable_{nameof(context.EntityName)}", context.EntityName).DisposeWith(disposable);
			LogContext.PushProperty($"durable_{nameof(context.EntityKey)}", context.EntityKey).DisposeWith(disposable);
			LogContext.PushProperty($"durable_{nameof(context.OperationName)}", context.OperationName).DisposeWith(disposable);
			LogContext.PushProperty($"durable_{nameof(context.HasState)}", context.HasState).DisposeWith(disposable);
			LogContext.PushProperty($"durable_{nameof(context.FunctionBindingContext.FunctionInstanceId)}", context.FunctionBindingContext.FunctionInstanceId).DisposeWith(disposable);
			return disposable;
		}
	}
}

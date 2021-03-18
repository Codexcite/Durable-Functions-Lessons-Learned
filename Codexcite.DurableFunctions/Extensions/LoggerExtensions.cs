using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using Serilog;
using Serilog.Context;
using Serilog.Events;

// ReSharper disable ExplicitCallerInfoArgument

namespace Codexcite.DurableFunctions.Extensions
{
	public enum OperationStep
	{
		Enter,
		Exit,
		Exception,
		Trace
	}

	public static class LoggerExtensions
	{
		public static void Enter(this ILogger logger,
														 LogEventLevel level,
														 object?[]? arguments = null,
														 object? returnValue = null,
														 Exception? exception = null,
														 TimeSpan? time = null,
														 string? message = null,
														 (string Name, object? Value)[]? propertyValues = null,
														 [CallerMemberName] string callerMemberName = "",
														 [CallerFilePath] string callerFilePath = "",
														 [CallerLineNumber] int callerLineNumber = 0)
		{
			logger.OperationWithDetails(level, OperationStep.Enter,
																 arguments,
																 returnValue,
																 exception,
																 time,
																 message,
																 propertyValues,
																 callerMemberName,
																 callerFilePath,
																 callerLineNumber);
		}

		public static void Exception(this ILogger logger,
																 LogEventLevel level,
																 object?[]? arguments = null,
																 object? returnValue = null,
																 Exception? exception = null,
																 TimeSpan? time = null,
																 string? message = null,
																 (string Name, object? Value)[]? propertyValues = null,
																 [CallerMemberName] string callerMemberName = "",
																 [CallerFilePath] string callerFilePath = "",
																 [CallerLineNumber] int callerLineNumber = 0)
		{
			logger.OperationWithDetails(level, OperationStep.Exception,
																 arguments,
																 returnValue,
																 exception,
																 time,
																 message,
																 propertyValues,
																 callerMemberName,
																 callerFilePath,
																 callerLineNumber);
		}

		public static void Exit(this ILogger logger,
														LogEventLevel level,
														object?[]? arguments = null,
														object? returnValue = null,
														Exception? exception = null,
														TimeSpan? time = null,
														string? message = null,
														(string Name, object? Value)[]? propertyValues = null,
														[CallerMemberName] string callerMemberName = "",
														[CallerFilePath] string callerFilePath = "",
														[CallerLineNumber] int callerLineNumber = 0)
		{
			logger.OperationWithDetails(level, OperationStep.Exit,
																 arguments,
																 returnValue,
																 exception,
																 time,
																 message,
																 propertyValues,
																 callerMemberName,
																 callerFilePath,
																 callerLineNumber);
		}

		public static void Trace(this ILogger logger,
														LogEventLevel level,
														object?[]? arguments = null,
														object? returnValue = null,
														Exception? exception = null,
														TimeSpan? time = null,
														string? message = null,
														(string Name, object? Value)[]? propertyValues = null,
														[CallerMemberName] string callerMemberName = "",
														[CallerFilePath] string callerFilePath = "",
														[CallerLineNumber] int callerLineNumber = 0)
		{
			logger.OperationWithDetails(level, OperationStep.Trace,
																 arguments,
																 returnValue,
																 exception,
																 time,
																 message,
																 propertyValues,
																 callerMemberName,
																 callerFilePath,
																 callerLineNumber);
		}

		public static void OperationWithDetails(this ILogger logger,
																					 LogEventLevel level,
																					 OperationStep operation,
																					 object?[]? arguments = null,
																					 object? returnValue = null,
																					 Exception? exception = null,
																					 TimeSpan? time = null,
																					 string? message = null,
																					 (string Name, object? Value)[]? propertyValues = null,
																					 [CallerMemberName] string callerMemberName = "",
																					 [CallerFilePath] string callerFilePath = "",
																					 [CallerLineNumber] int callerLineNumber = 0)
		{
			logger.OperationWithDetails(level, operation.ToString(),
																 arguments,
																 returnValue,
																 exception,
																 time,
																 message,
																 propertyValues,
																 callerMemberName,
																 callerFilePath,
																 callerLineNumber);
		}

		public static void OperationWithDetails(this ILogger logger,
																					 LogEventLevel level,
																					 string operation,
																					 object?[]? arguments = null,
																					 object? returnValue = null,
																					 Exception? exception = null,
																					 TimeSpan? time = null,
																					 string? message = null,
																					 (string Name, object? Value)[]? propertyValues = null,
																					 [CallerMemberName] string callerMemberName = "",
																					 [CallerFilePath] string callerFilePath = "",
																					 [CallerLineNumber] int callerLineNumber = 0)
		{
			using (var compositeDisposable = new CompositeDisposable())
			{
				LogContext.PushProperty("MemberName", callerMemberName).DisposeWith(compositeDisposable);
				LogContext.PushProperty("FilePath", callerFilePath).DisposeWith(compositeDisposable);
				LogContext.PushProperty("LineNumber", callerLineNumber).DisposeWith(compositeDisposable);

				var sb = new StringBuilder("[{Level}] {Operation}: {SourceContext}.{MemberName} ");
				LogContext.PushProperty("Level", level.ToString()).DisposeWith(compositeDisposable);
				LogContext.PushProperty("Operation", operation).DisposeWith(compositeDisposable);
				if (time != null)
				{
					sb.Append("{Time:c} ");
					LogContext.PushProperty("Time", time.Value).DisposeWith(compositeDisposable);
				}

				if (arguments != null && arguments.Length > 0)
				{
					LogContext.PushProperty("Arguments", arguments, destructureObjects: true).DisposeWith(compositeDisposable);
				}

				if (exception != null)
				{
					sb.Append("-* {ExceptionMessage} *- ");
					LogContext.PushProperty("ExceptionMessage", exception.Message).DisposeWith(compositeDisposable);
					LogContext.PushProperty("Exception", exception, destructureObjects: true).DisposeWith(compositeDisposable);
				}

				LogContext.PushProperty("ReturnValue", returnValue, destructureObjects: true).DisposeWith(compositeDisposable);

				if (message != null)
				{
					sb.Append("| ");
					sb.Append(message);
					LogContext.PushProperty("MessageTemplate", message).DisposeWith(compositeDisposable);
				}
				if (propertyValues != null)
				{
					foreach (var propertyValue in propertyValues)
					{
						LogContext.PushProperty(propertyValue.Name, propertyValue.Value).DisposeWith(compositeDisposable);
					}
				}

				if (exception != null)
					logger.Write(level, exception, sb.ToString());
				else
					logger.Write(level, sb.ToString());
			}
		}

		
	}
}
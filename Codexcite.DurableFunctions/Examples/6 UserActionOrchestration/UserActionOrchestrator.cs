using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Codexcite.DurableFunctions.Extensions;
using Codexcite.DurableFunctions.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Serilog.Events;

namespace Codexcite.DurableFunctions.Examples.UserActionOrchestration
{
	public class UserActionOrchestrator : BaseLogged
	{
		private const string EventNameApproved = "APPROVE";
		private readonly TimeSpan _timeToWait = TimeSpan.FromSeconds(30);
		private readonly RetryOptions _retryOptions;

		public UserActionOrchestrator()
		{
			_retryOptions = new RetryOptions(TimeSpan.FromSeconds(1), 10)
										{
											BackoffCoefficient = 2, MaxRetryInterval = TimeSpan.FromSeconds(1),
											Handle = HandleRetry
										};
		}

		private bool HandleRetry(Exception exception) // just to demonstrate retry functionality
		{
			//_log.Exception(LogEventLevel.Error, exception: exception, message: "Handling retry");
			if (exception.InnerException is InfoException infoException) // we get a FunctionFailedException
				_log.Exception(LogEventLevel.Error, exception: infoException, message: $"Handling retry info {infoException.Info}");
				
			return true;
		}

		[FunctionName(nameof(UserActionOrchestrator))]
		public async Task<List<string>> RunOrchestrator(
					[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			using var disposable = context.PushLoggerProperties();
			var phone = context.GetInput<string>();
			var outputs = new List<string>();

			var proxy =
				context.CreateEntityProxy<INotifySubscriptionEntity>(NotifySubscriptionEntity.BuildId(phone, EventNameApproved));
			try
			{
				await proxy.Subscribe(context.InstanceId);

				await context.CallActivityWithRetryAsync(nameof(SendMessageActivity), _retryOptions, phone); // might fail, better include retry mechanism

				var approvalNotification = await proxy.GetLatestNotification()                                  // get latest, maybe already approved (optional)
																 ?? await WaitForUserAction(context, _timeToWait, EventNameApproved); // wait for user approval

				outputs.Add(approvalNotification != null
											? $"We have approval from {phone} at {approvalNotification.Timestamp}"
											: $"No approval from {phone} after waiting {_timeToWait}");
			}
			finally
			{
				await proxy.Unsubscribe(context.InstanceId);
			}
			return outputs;
		}

		private static async Task<NotifyPayload?> WaitForUserAction(IDurableOrchestrationContext context,
																																				TimeSpan timeToWaitForReply,
																																				string eventName)
		{
			var deadline = context.CurrentUtcDateTime.Add(timeToWaitForReply);
			using var cts = new CancellationTokenSource();

			var activityTask = context.WaitForExternalEvent<NotifyPayload>(eventName);
			var timeoutTask = context.CreateTimer(deadline, cts.Token);

			Task winner = await Task.WhenAny(activityTask, timeoutTask);

			if (winner == activityTask)
			{
				// success case
				cts.Cancel();
				return activityTask.Result;
			}

			// timeout case
			return null;
		}
	}
}
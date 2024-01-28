using System.Collections.Generic;
using Xunit;
using Xunit.Internal;
using Xunit.v3;

public class SynchronousMessageBusTests
{
	[Fact]
	public void MessagesAreDispatchedImmediatelyFromBus()
	{
		var msg1 = new _MessageSinkMessage();
		var dispatchedMessages = new List<_MessageSinkMessage>();

		using (var bus = new SynchronousMessageBus(SpyMessageSink.Create(messages: dispatchedMessages)))
			Assert.True(bus.QueueMessage(msg1));

		var message = Assert.Single(dispatchedMessages);
		Assert.Same(msg1, message);
	}

	[Fact]
	public void BusShouldReportShutdownWhenMessageSinkReturnsFalse()
	{
		using var bus = new SynchronousMessageBus(SpyMessageSink.Create(returnResult: false));

		Assert.False(bus.QueueMessage(new _MessageSinkMessage()));
	}

	[Fact]
	public void WhenStopOnFailIsFalse_WithFailedTest_BusShouldNotReportShutdown()
	{
		var testPassedMessage = TestData.TestPassed();
		var testFailedMessage = TestData.TestFailed();
		using var bus = new SynchronousMessageBus(SpyMessageSink.Create(), stopOnFail: false);

		Assert.True(bus.QueueMessage(testPassedMessage));
		Assert.True(bus.QueueMessage(testFailedMessage));
		Assert.True(bus.QueueMessage(testPassedMessage));
	}

	[Fact]
	public void WhenStopOnFailIsTrue_WithFailedTest_BusShouldReportShutdown()
	{
		var testPassedMessage = TestData.TestPassed();
		var testFailedMessage = TestData.TestFailed();
		using var bus = new SynchronousMessageBus(SpyMessageSink.Create(), stopOnFail: true);

		Assert.True(bus.QueueMessage(testPassedMessage));
		Assert.False(bus.QueueMessage(testFailedMessage));
		Assert.False(bus.QueueMessage(testPassedMessage));
	}
}

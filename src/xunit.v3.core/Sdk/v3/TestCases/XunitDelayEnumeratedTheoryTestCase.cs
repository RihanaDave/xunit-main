using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace Xunit.v3;

/// <summary>
/// Represents a test case which runs multiple tests for theory data, either because the
/// data was not enumerable or because the data was not serializable.
/// </summary>
public class XunitDelayEnumeratedTheoryTestCase : XunitTestCase
{
	/// <summary>
	/// Called by the de-serializer; should only be called by deriving classes for de-serialization purposes
	/// </summary>
	[Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
	public XunitDelayEnumeratedTheoryTestCase()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="XunitDelayEnumeratedTheoryTestCase"/> class.
	/// </summary>
	/// <param name="testMethod">The test method this test case belongs to.</param>
	/// <param name="testCaseDisplayName">The display name for the test case.</param>
	/// <param name="uniqueID">The optional unique ID for the test case; if not provided, will be calculated.</param>
	/// <param name="explicit">Indicates whether the test case was marked as explicit.</param>
	/// <param name="traits">The optional traits list.</param>
	/// <param name="sourceFilePath">The optional source file in where this test case originated.</param>
	/// <param name="sourceLineNumber">The optional source line number where this test case originated.</param>
	/// <param name="timeout">The optional timeout for the test case (in milliseconds).</param>
	public XunitDelayEnumeratedTheoryTestCase(
		_ITestMethod testMethod,
		string testCaseDisplayName,
		string uniqueID,
		bool @explicit,
		Dictionary<string, List<string>>? traits = null,
		string? sourceFilePath = null,
		int? sourceLineNumber = null,
		int? timeout = null)
			: base(testMethod, testCaseDisplayName, uniqueID, @explicit, skipReason: null, traits, testMethodArguments: null, sourceFilePath, sourceLineNumber, timeout)
	{ }

	/// <inheritdoc/>
	public override ValueTask<RunSummary> RunAsync(
		ExplicitOption explicitOption,
		IMessageBus messageBus,
		object?[] constructorArguments,
		ExceptionAggregator aggregator,
		CancellationTokenSource cancellationTokenSource) =>
			XunitDelayEnumeratedTheoryTestCaseRunner.Instance.RunAsync(
				this,
				messageBus,
				aggregator,
				cancellationTokenSource,
				TestCaseDisplayName,
				SkipReason,
				explicitOption,
				constructorArguments,
				TestMethodArguments
			);
}

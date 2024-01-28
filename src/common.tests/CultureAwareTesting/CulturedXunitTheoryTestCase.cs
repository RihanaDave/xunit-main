using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Internal;
using Xunit.Sdk;

namespace Xunit.v3;

public class CulturedXunitTheoryTestCase : XunitDelayEnumeratedTheoryTestCase
{
	string? culture;

	/// <summary>
	/// Called by the de-serializer; should only be called by deriving classes for de-serialization purposes
	/// </summary>
	[Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
	public CulturedXunitTheoryTestCase()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="CulturedXunitTheoryTestCase"/> class.
	/// </summary>
	/// <param name="culture">The culture to run the theory under.</param>
	/// <param name="testMethod">The test method this test case belongs to.</param>
	/// <param name="testCaseDisplayName">The display name for the test case.</param>
	/// <param name="uniqueID">The optional unique ID for the test case; if not provided, will be calculated.</param>
	/// <param name="explicit">Indicates whether the test case was marked as explicit.</param>
	/// <param name="skipReason">The optional reason for skipping the test.</param>
	/// <param name="traits">The optional traits list.</param>
	/// <param name="sourceFilePath">The optional source file in where this test case originated.</param>
	/// <param name="sourceLineNumber">The optional source line number where this test case originated.</param>
	/// <param name="timeout">The optional timeout for the test case (in milliseconds).</param>
	public CulturedXunitTheoryTestCase(
		string culture,
		_ITestMethod testMethod,
		string testCaseDisplayName,
		string uniqueID,
		bool @explicit,
		Dictionary<string, List<string>>? traits = null,
		string? sourceFilePath = null,
		int? sourceLineNumber = null,
		int? timeout = null)
			: base(testMethod, $"{testCaseDisplayName}[{culture}]", $"{uniqueID}[{culture}]", @explicit, traits, sourceFilePath, sourceLineNumber, timeout)
	{
		this.culture = Guard.ArgumentNotNull(culture);

		Traits.Add("Culture", Culture);
	}

	public string Culture =>
		this.ValidateNullablePropertyValue(culture, nameof(Culture));

	protected override void Deserialize(IXunitSerializationInfo info)
	{
		base.Deserialize(info);

		culture = Guard.NotNull("Could not retrieve Culture from serialization", info.GetValue<string>("cul"));
	}

	public override ValueTask<RunSummary> RunAsync(
		ExplicitOption explicitOption,
		IMessageBus messageBus,
		object?[] constructorArguments,
		ExceptionAggregator aggregator,
		CancellationTokenSource cancellationTokenSource) =>
			new CulturedXunitTheoryTestCaseRunner(Culture).RunAsync(
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

	protected override void Serialize(IXunitSerializationInfo info)
	{
		base.Serialize(info);

		info.AddValue("cul", Culture);
	}
}

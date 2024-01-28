using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Runner.Common;
using Xunit.v3;

public class TeamCityReporterMessageHandlerTests
{
	public class FailureMessages
	{
		readonly string assemblyID = "assembly-id\t\r\n";
		readonly string classID = "test-class-id\t\r\n";
		readonly string collectionID = "test-collection-id\t\r\n";
		readonly int[] exceptionParentIndices = new[] { -1 };
		readonly string[] exceptionTypes = new[] { "\x2018ExceptionType\x2019" };
		readonly string[] messages = new[] { "This is my message \x2020\t\r\n" };
		readonly string methodID = "test-method-id\t\r\n";
		readonly string[] stackTraces = new[] { "Line 1 \x0d60\r\nLine 2 \x1f64\r\nLine 3 \x999f" };
		readonly string testCaseID = "test-case-id\t\r\n";
		readonly string testID = "test-id\t\r\n";

		[Fact]
		public void ErrorMessage()
		{
			var errorMessage = new _ErrorMessage
			{
				ExceptionParentIndices = exceptionParentIndices,
				ExceptionTypes = exceptionTypes,
				Messages = messages,
				StackTraces = stackTraces
			};
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(errorMessage);

			AssertFailureMessage(handler.Messages.Where(msg => msg.Contains("##teamcity")), "FATAL ERROR");
		}

		[Fact]
		public void TestAssemblyCleanupFailure()
		{
			var collectionStarting = new _TestAssemblyStarting
			{
				AssemblyUniqueID = assemblyID,
				AssemblyPath = @"C:\Foo\Bar.dll"
			};
			var collectionCleanupFailure = new _TestAssemblyCleanupFailure
			{
				AssemblyUniqueID = assemblyID,
				ExceptionParentIndices = exceptionParentIndices,
				ExceptionTypes = exceptionTypes,
				Messages = messages,
				StackTraces = stackTraces
			};
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(collectionStarting);
			handler.OnMessage(collectionCleanupFailure);

			AssertFailureMessage(handler.Messages.Where(msg => msg.Contains("##teamcity")), "Test Assembly Cleanup Failure (C:|0x005CFoo|0x005CBar.dll)", assemblyID);
		}

		[Fact]
		public void TestCaseCleanupFailure()
		{
			var caseStarting = new _TestCaseStarting
			{
				AssemblyUniqueID = assemblyID,
				TestCaseUniqueID = testCaseID,
				TestCaseDisplayName = "MyTestCase\t\r\n",
				TestClassUniqueID = classID,
				TestCollectionUniqueID = collectionID,
				TestMethodUniqueID = methodID
			};
			var caseCleanupFailure = new _TestCaseCleanupFailure
			{
				AssemblyUniqueID = assemblyID,
				ExceptionParentIndices = exceptionParentIndices,
				ExceptionTypes = exceptionTypes,
				Messages = messages,
				StackTraces = stackTraces,
				TestCaseUniqueID = testCaseID,
				TestCollectionUniqueID = collectionID,
				TestClassUniqueID = classID,
				TestMethodUniqueID = methodID
			};
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(caseStarting);
			handler.OnMessage(caseCleanupFailure);

			AssertFailureMessage(handler.Messages.Where(msg => msg.Contains("##teamcity")), "Test Case Cleanup Failure (MyTestCase\t|r|n)", collectionID);
		}

		[Fact]
		public void TestClassCleanupFailure()
		{
			var classStarting = new _TestClassStarting
			{
				AssemblyUniqueID = assemblyID,
				TestClass = "MyType\t\r\n",
				TestClassUniqueID = classID,
				TestCollectionUniqueID = collectionID
			};
			var classCleanupFailure = new _TestClassCleanupFailure
			{
				AssemblyUniqueID = assemblyID,
				ExceptionParentIndices = exceptionParentIndices,
				ExceptionTypes = exceptionTypes,
				Messages = messages,
				StackTraces = stackTraces,
				TestCollectionUniqueID = collectionID,
				TestClassUniqueID = classID
			};
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(classStarting);
			handler.OnMessage(classCleanupFailure);

			AssertFailureMessage(handler.Messages.Where(msg => msg.Contains("##teamcity")), "Test Class Cleanup Failure (MyType\t|r|n)", collectionID);
		}

		[Fact]
		public void TestCleanupFailure()
		{
			var testStarting = new _TestStarting
			{
				AssemblyUniqueID = assemblyID,
				TestCaseUniqueID = testCaseID,
				TestClassUniqueID = classID,
				TestDisplayName = "MyTest\t\r\n",
				TestCollectionUniqueID = collectionID,
				TestMethodUniqueID = methodID,
				TestUniqueID = testID
			};
			var testCleanupFailure = new _TestCleanupFailure
			{
				AssemblyUniqueID = assemblyID,
				ExceptionParentIndices = exceptionParentIndices,
				ExceptionTypes = exceptionTypes,
				Messages = messages,
				StackTraces = stackTraces,
				TestCaseUniqueID = testCaseID,
				TestCollectionUniqueID = collectionID,
				TestClassUniqueID = classID,
				TestMethodUniqueID = methodID,
				TestUniqueID = testID
			};
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(testStarting);
			handler.OnMessage(testCleanupFailure);

			AssertFailureMessage(handler.Messages.Where(msg => msg.Contains("##teamcity")), "Test Cleanup Failure (MyTest\t|r|n)", collectionID);
		}

		[Fact]
		public void TestCollectionCleanupFailure()
		{
			var collectionStarting = new _TestCollectionStarting
			{
				AssemblyUniqueID = assemblyID,
				TestCollectionDisplayName = "FooBar\t\r\n",
				TestCollectionUniqueID = collectionID
			};
			var collectionCleanupFailure = new _TestCollectionCleanupFailure
			{
				AssemblyUniqueID = assemblyID,
				ExceptionParentIndices = exceptionParentIndices,
				ExceptionTypes = exceptionTypes,
				Messages = messages,
				StackTraces = stackTraces,
				TestCollectionUniqueID = collectionID
			};
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(collectionStarting);
			handler.OnMessage(collectionCleanupFailure);

			AssertFailureMessage(handler.Messages.Where(msg => msg.Contains("##teamcity")), "Test Collection Cleanup Failure (FooBar\t|r|n (test-collection-id\t|r|n))", collectionID);
		}

		[Fact]
		public void TestMethodCleanupFailure()
		{
			var methodStarting = new _TestMethodStarting
			{
				AssemblyUniqueID = assemblyID,
				TestClassUniqueID = classID,
				TestCollectionUniqueID = collectionID,
				TestMethod = "MyMethod\t\r\n",
				TestMethodUniqueID = methodID,
			};
			var methodCleanupFailure = new _TestMethodCleanupFailure
			{
				AssemblyUniqueID = assemblyID,
				ExceptionParentIndices = exceptionParentIndices,
				ExceptionTypes = exceptionTypes,
				Messages = messages,
				StackTraces = stackTraces,
				TestCollectionUniqueID = collectionID,
				TestClassUniqueID = classID,
				TestMethodUniqueID = methodID
			};
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(methodStarting);
			handler.OnMessage(methodCleanupFailure);

			AssertFailureMessage(handler.Messages.Where(msg => msg.Contains("##teamcity")), "Test Method Cleanup Failure (MyMethod\t|r|n)", collectionID);
		}

		static void AssertFailureMessage(
			IEnumerable<string> messages,
			string messageType,
			string? flowId = null)
		{
			var message = messages.Last();

			Assert.Equal(
				$"[Raw] => ##teamcity[message timestamp='2023-05-03T21:12:00.000+0000'{(flowId is null ? "" : $" flowId='{TeamCityReporterMessageHandler.TeamCityEscape(flowId)}'")} status='ERROR' text='|[{messageType}|] |0x2018ExceptionType|0x2019: |0x2018ExceptionType|0x2019 : This is my message |0x2020\t|r|n' errorDetails='Line 1 |0x0d60|r|nLine 2 |0x1f64|r|nLine 3 |0x999f']",
				message
			);
		}
	}

	public class OnMessage_TestAssemblyStarting_TestAssemblyFinished
	{
		[Fact]
		public static void StartsAndEndsFlowAndSuite()
		{
			var startingMessage = TestData.TestAssemblyStarting(assemblyUniqueID: "assembly-id\t\r\n", assemblyPath: @"/path/to\test-assembly.exe");
			var finishedMessage = TestData.TestAssemblyFinished(assemblyUniqueID: "assembly-id\t\r\n");
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(startingMessage);
			handler.OnMessage(finishedMessage);

			Assert.Collection(
				handler.Messages.Where(msg => msg.Contains("##teamcity")),
				msg => Assert.Equal("[Raw] => ##teamcity[flowStarted timestamp='2023-05-03T21:12:00.000+0000' flowId='assembly-id\t|r|n']", msg),
				msg => Assert.Equal("[Raw] => ##teamcity[testSuiteStarted timestamp='2023-05-03T21:12:00.000+0000' flowId='assembly-id\t|r|n' name='/path/to|0x005Ctest-assembly.exe']", msg),
				msg => Assert.Equal("[Raw] => ##teamcity[testSuiteFinished timestamp='2023-05-03T21:12:00.000+0000' flowId='assembly-id\t|r|n' name='/path/to|0x005Ctest-assembly.exe']", msg),
				msg => Assert.Equal("[Raw] => ##teamcity[flowFinished timestamp='2023-05-03T21:12:00.000+0000' flowId='assembly-id\t|r|n']", msg)
			);
		}

		[Fact]
		public static void FallsBackToAssemblyNameWhenPathIsNull()
		{
			var startingMessage = TestData.TestAssemblyStarting(assemblyUniqueID: "assembly-id\t\r\n", assemblyPath: null, assemblyName: "test[assembly].exe");
			var finishedMessage = TestData.TestAssemblyFinished(assemblyUniqueID: "assembly-id\t\r\n");
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(startingMessage);
			handler.OnMessage(finishedMessage);

			Assert.Collection(
				handler.Messages.Where(msg => msg.Contains("##teamcity")),
				msg => Assert.Equal("[Raw] => ##teamcity[flowStarted timestamp='2023-05-03T21:12:00.000+0000' flowId='assembly-id\t|r|n']", msg),
				msg => Assert.Equal("[Raw] => ##teamcity[testSuiteStarted timestamp='2023-05-03T21:12:00.000+0000' flowId='assembly-id\t|r|n' name='test|[assembly|].exe']", msg),
				msg => Assert.Equal("[Raw] => ##teamcity[testSuiteFinished timestamp='2023-05-03T21:12:00.000+0000' flowId='assembly-id\t|r|n' name='test|[assembly|].exe']", msg),
				msg => Assert.Equal("[Raw] => ##teamcity[flowFinished timestamp='2023-05-03T21:12:00.000+0000' flowId='assembly-id\t|r|n']", msg)
			);
		}

		[Fact]
		public static void UsesRootFlowIDFromTeamCityEnvironment()
		{
			var startingMessage = TestData.TestAssemblyStarting(assemblyUniqueID: "assembly-id\t\r\n", assemblyPath: "test-assembly.exe\t\r\n");
			var finishedMessage = TestData.TestAssemblyFinished(assemblyUniqueID: "assembly-id\t\r\n");
			var handler = TestableTeamCityReporterMessageHandler.Create("root-flow-id\t\r\n");

			handler.OnMessage(startingMessage);
			handler.OnMessage(finishedMessage);

			var msg = handler.Messages.Where(msg => msg.Contains("##teamcity")).First();
			Assert.Equal("[Raw] => ##teamcity[flowStarted timestamp='2023-05-03T21:12:00.000+0000' flowId='assembly-id\t|r|n' parent='root-flow-id\t|r|n']", msg);
		}
	}

	public class OnMessage_TestCollectionStarting_TestCollectionFinished
	{
		[Fact]
		public static void StartsAndEndsFlowAndSuite()
		{
			var startingMessage = TestData.TestCollectionStarting(testCollectionUniqueID: "test-collection-id\t\r\n", testCollectionDisplayName: "my-test-collection\t\r\n");
			var finishedMessage = TestData.TestCollectionFinished(testCollectionUniqueID: "test-collection-id\t\r\n");
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(startingMessage);
			handler.OnMessage(finishedMessage);

			Assert.Collection(
				handler.Messages.Where(msg => msg.Contains("##teamcity")),
				msg => Assert.Equal("[Raw] => ##teamcity[flowStarted timestamp='2023-05-03T21:12:00.000+0000' flowId='test-collection-id\t|r|n' parent='assembly-id']", msg),
				msg => Assert.Equal("[Raw] => ##teamcity[testSuiteStarted timestamp='2023-05-03T21:12:00.000+0000' flowId='test-collection-id\t|r|n' name='my-test-collection\t|r|n (test-collection-id\t|r|n)']", msg),
				msg => Assert.Equal("[Raw] => ##teamcity[testSuiteFinished timestamp='2023-05-03T21:12:00.000+0000' flowId='test-collection-id\t|r|n' name='my-test-collection\t|r|n (test-collection-id\t|r|n)']", msg),
				msg => Assert.Equal("[Raw] => ##teamcity[flowFinished timestamp='2023-05-03T21:12:00.000+0000' flowId='test-collection-id\t|r|n']", msg)
			);
		}
	}

	public class OnMessage_TestFailed
	{
		[Fact]
		public static void LogsTestFailed()
		{
			var startingMessage = TestData.TestStarting(testDisplayName: "This is my display name \t\r\n", testCollectionUniqueID: "test-collection-id\t\r\n");
			var failedMessage = TestData.TestFailed(
				exceptionParentIndices: new[] { -1 },
				exceptionTypes: new[] { "ExceptionType" },
				executionTime: 1.2345m,
				messages: new[] { "This is my message \t\r\n" },
				output: "This is\t\r\noutput",
				stackTraces: new[] { "Line 1\r\nLine 2\r\nLine 3" },
				testCollectionUniqueID: "test-collection-id\t\r\n"
			);
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(startingMessage);
			handler.OnMessage(failedMessage);

			Assert.Collection(
				handler.Messages.Where(msg => msg.Contains("##teamcity")),
				msg => { }, // testStarting
				msg => Assert.Equal("[Raw] => ##teamcity[testFailed timestamp='2023-05-03T21:12:00.000+0000' flowId='test-collection-id\t|r|n' name='This is my display name \t|r|n' details='ExceptionType : This is my message 	|r|n|r|nLine 1|r|nLine 2|r|nLine 3']", msg)
			);
		}
	}

	public class OnMessage_TestFinished
	{
		[Fact]
		public static void WithoutOutput()
		{
			var startingMessage = TestData.TestStarting(testDisplayName: "This is my display name \t\r\n", testCollectionUniqueID: "test-collection-id\t\r\n");
			var finishedMessage = TestData.TestFinished(executionTime: 1.2345m, testCollectionUniqueID: "test-collection-id\t\r\n");
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(startingMessage);
			handler.OnMessage(finishedMessage);

			var msg = handler.Messages.Last();
			Assert.Collection(
				handler.Messages.Where(msg => msg.Contains("##teamcity")),
				msg => { }, // testStarted
				msg => Assert.Equal("[Raw] => ##teamcity[testFinished timestamp='2023-05-03T21:12:00.000+0000' flowId='test-collection-id\t|r|n' name='This is my display name \t|r|n' duration='1234']", msg)
			);
		}

		[Fact]
		public static void WithOutput()
		{
			var startingMessage = TestData.TestStarting(testDisplayName: "This is my display name \t\r\n", testCollectionUniqueID: "test-collection-id\t\r\n");
			var finishedMessage = TestData.TestFinished(executionTime: 1.2345m, output: "This is\t\r\noutput", testCollectionUniqueID: "test-collection-id\t\r\n");
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(startingMessage);
			handler.OnMessage(finishedMessage);

			var msg = handler.Messages.Last();
			Assert.Collection(
				handler.Messages.Where(msg => msg.Contains("##teamcity")),
				msg => { }, // testStarted
				msg => Assert.Equal("[Raw] => ##teamcity[testStdOut timestamp='2023-05-03T21:12:00.000+0000' flowId='test-collection-id\t|r|n' name='This is my display name \t|r|n' out='This is	|r|noutput' tc:tags='tc:parseServiceMessagesInside']]", msg),
				msg => Assert.Equal("[Raw] => ##teamcity[testFinished timestamp='2023-05-03T21:12:00.000+0000' flowId='test-collection-id\t|r|n' name='This is my display name \t|r|n' duration='1234']", msg)
			);
		}
	}

	public class OnMessage_TestSkipped
	{
		[Fact]
		public static void LogsTestIgnored()
		{
			var startingMessage = TestData.TestStarting(testDisplayName: "This is my display name \t\r\n", testCollectionUniqueID: "test-collection-id\t\r\n");
			var skippedMessage = TestData.TestSkipped(reason: "This is my skip reason \t\r\n", testCollectionUniqueID: "test-collection-id\t\r\n");
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(startingMessage);
			handler.OnMessage(skippedMessage);

			Assert.Collection(
				handler.Messages.Where(msg => msg.Contains("##teamcity")),
				msg => { }, // testStarted
				msg => Assert.Equal("[Raw] => ##teamcity[testIgnored timestamp='2023-05-03T21:12:00.000+0000' flowId='test-collection-id\t|r|n' name='This is my display name \t|r|n' message='This is my skip reason \t|r|n']", msg)
			);
		}
	}

	public class OnMessage_TestStarting
	{
		[Fact]
		public static void LogsTestName()
		{
			var startingMessage = TestData.TestStarting(testDisplayName: "This is my display name \t\r\n", testCollectionUniqueID: "test-collection-id\t\r\n");
			var handler = TestableTeamCityReporterMessageHandler.Create();

			handler.OnMessage(startingMessage);

			var msg = Assert.Single(handler.Messages);
			Assert.Equal("[Raw] => ##teamcity[testStarted timestamp='2023-05-03T21:12:00.000+0000' flowId='test-collection-id\t|r|n' name='This is my display name \t|r|n']", msg);
		}
	}

	// Helpers

	class TestableTeamCityReporterMessageHandler : TeamCityReporterMessageHandler
	{
		DateTimeOffset now = new DateTimeOffset(2023, 5, 3, 21, 12, 0, TimeSpan.Zero);

		public IReadOnlyList<string> Messages;

		TestableTeamCityReporterMessageHandler(
			SpyRunnerLogger logger,
			string? rootFlowId) :
				base(logger, rootFlowId)
		{
			Messages = logger.Messages;
		}

		protected override DateTimeOffset UtcNow => now;

		public static TestableTeamCityReporterMessageHandler Create(string? rootFlowId = null)
		{
			return new TestableTeamCityReporterMessageHandler(new SpyRunnerLogger(), rootFlowId);
		}
	}
}

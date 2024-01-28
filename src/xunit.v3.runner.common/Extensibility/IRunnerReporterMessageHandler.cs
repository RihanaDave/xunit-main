using System;
using Xunit.v3;

namespace Xunit.Runner.Common;

/// <summary>
/// Indicates a message sink that's designed for use with runner reporters. In particular, this allows
/// runner reporters to implement <see cref="IDisposable"/> and forces runners to dispose of the message
/// handler that's returned to them.
/// </summary>
public interface IRunnerReporterMessageHandler : _IMessageSink, IAsyncDisposable
{ }

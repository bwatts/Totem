using System;

namespace Totem.Timeline.IntegrationTests.Hosting
{
  /// <summary>
  /// The time to wait before an expected event is considered missing.
  ///
  /// Implicitly converts from <see cref="System.TimeSpan"/> and <see cref="int"/> (milliseconds).
  ///
  /// Defaults to 500ms.
  /// </summary>
  public class ExpectTimeout
  {
    public ExpectTimeout(TimeSpan timeSpan)
    {
      Expect.That(timeSpan).IsGreaterThan(TimeSpan.Zero);

      TimeSpan = timeSpan;
    }

    public ExpectTimeout(int milliseconds) : this(TimeSpan.FromMilliseconds(milliseconds))
    {}

    public readonly TimeSpan TimeSpan;

    public const int DefaultMilliseconds = 1000;
    public static readonly TimeSpan DefaultTimeSpan = TimeSpan.FromMilliseconds(DefaultMilliseconds);
    public static readonly ExpectTimeout Default = new ExpectTimeout(DefaultTimeSpan);

    public static implicit operator ExpectTimeout(TimeSpan timeSpan) =>
      new ExpectTimeout(timeSpan);

    public static implicit operator ExpectTimeout(int milliseconds) =>
      new ExpectTimeout(milliseconds);
  }
}
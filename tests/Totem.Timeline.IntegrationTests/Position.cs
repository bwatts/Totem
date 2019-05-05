using System.Threading.Tasks;
using Xunit;

namespace Totem.Timeline.IntegrationTests
{
  /// <summary>
  /// Tests that the timeline position increases from 0
  /// </summary>
  public class Position : TestArea
  {
    [Fact]
    public async Task IncreasesFrom0()
    {
      var position0 = await Append(new Happened());
      var position1 = await Append(new Happened());
      var position2 = await Append(new Happened());

      Expect(position0).Is(new TimelinePosition(0));
      Expect(position1).Is(new TimelinePosition(1));
      Expect(position2).Is(new TimelinePosition(2));
    }

    class Happened : Event { }
  }
}
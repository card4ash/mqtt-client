using System.Threading;

namespace MqttClient
{
  public class ThreadSleepInfiniteLoopRunner : IInfineLoopRunner
  {
    private readonly int _sleepTime;

    public ThreadSleepInfiniteLoopRunner(int sleepTime)
    {
      _sleepTime = sleepTime;
    }

    public void RunInfiniteLoop()
    {
      while (true)
      {
        Thread.Sleep(_sleepTime);
      }
    }
  }
}

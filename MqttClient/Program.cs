using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClient
{
  class Program
  {
    static void Main(string[] args)
    {
      MqttClientWrapperAdapter.WrapperInstance.MakeConnection();
      var x=new ThreadSleepInfiniteLoopRunner(1000);
      x.RunInfiniteLoop();
    }
  }
}

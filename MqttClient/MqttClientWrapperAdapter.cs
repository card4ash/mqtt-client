using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using App.Logging;
using MQTT.Client;
using System;

namespace MqttClient
{
  public class MqttClientWrapperAdapter
  {
    private static MqttClientWrapper instance = null;
    private static object syncLock = new object();
    public static int Counter { get; set; }
    public static MqttClientWrapper WrapperInstance
    {
      get
      {
        lock (syncLock)
        {
          if (MqttClientWrapperAdapter.instance == null)
          {
            instance = new MqttClientWrapper();
            instance.NotifyMqttMsgPublishReceivedEvent += new MqttClientWrapper.NotifyMqttMsgPublishReceivedDelegate(PublishReceivedMessage_NotifyEvent);

            instance.NotifyMqttMsgPublishedEvent += new MqttClientWrapper.NotifyMqttMsgPublishedDelegate(PublishedMessage_NotifyEvent);

            instance.NotifyMqttMsgSubscribedEvent += new MqttClientWrapper.NotifyMqttMsgSubscribedDelegate(SubscribedMessage_NotifyEvent);
          }

          return instance;
        }
      }
    }

    static void PublishReceivedMessage_NotifyEvent(CustomEventArgs customEventArgs)
    {

      if (customEventArgs.ReceivedTopic.Contains("shhello") && Counter < 4)
      {
        LogMqttRequestMessages(customEventArgs);

        Console.WriteLine(customEventArgs.ReceivedMessage.ToString());
      }
      Counter = 0;
    }

    private static void LogMqttRequestMessages(CustomEventArgs customEventArgs)
    {
      string msg = customEventArgs.ReceivedMessage.ToString();
      Console.WriteLine(msg);
    }

    static void PublishedMessage_NotifyEvent(CustomEventArgs customEventArgs)
    {
      string msg = customEventArgs.ReceivedMessage;
    }
    static void SubscribedMessage_NotifyEvent(CustomEventArgs customEventArgs)
    {
      string msg = customEventArgs.ReceivedMessage;
    }
  }
}
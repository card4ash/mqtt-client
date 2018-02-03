using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Configuration;
using System.Collections.Generic;
using App.Logging;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using SmartHome.MQTT.Client;

namespace MQTT.Client
{
  public class MqttClientWrapper
  {

    #region delegate event

    #region MqttMsg-Publish-Received-Notification
    public delegate void NotifyMqttMsgPublishReceivedDelegate(CustomEventArgs customEventArgs);
    public event NotifyMqttMsgPublishReceivedDelegate NotifyMqttMsgPublishReceivedEvent;
    #endregion

    #region MqttMsg-Published-Notification
    public delegate void NotifyMqttMsgPublishedDelegate(CustomEventArgs customEventArgs);
    public event NotifyMqttMsgPublishedDelegate NotifyMqttMsgPublishedEvent;
    #endregion

    #region MqttMsg-Subscribed-Notification
    public delegate void NotifyMqttMsgSubscribedDelegate(CustomEventArgs customEventArgs);
    public event NotifyMqttMsgSubscribedDelegate NotifyMqttMsgSubscribedEvent;
    #endregion

    #endregion

    #region constructor
    public MqttClientWrapper()
    {
      _clientId = Guid.NewGuid().ToString();
    }
    public void MakeConnection()
    {
      Logger.Info("Make Connection Envoke");

      #region MyRegion

      try
      {
        if (AppMQTTClient == null || !AppMQTTClient.IsConnected)
        {
          Logger.Info("MQTT process to start connection.");

          if (BrokerAddress == "192.168.11.236")
          {
            LocalBrokerConnection(BrokerAddress);
          }

          else if (BrokerAddress == "192.168.11.189")
          {
            BrokerConnectionWithoutCertificateForCommand(BrokerAddress);
          }

          else if (BrokerAddress == "192.168.11.190")
          {
            Logger.Info("Try to connect with broker.");
            //BrokerConnectionWithoutCertificateForCommand(BrokerAddress);
            BrokerConnectionWithCertificateForWebBroker(BrokerAddress);
            Logger.Info("Web broker connection successfull.");
          }
          else if (BrokerAddress == "192.168.11.150")
          {
            BrokerConnectionWithoutCertificate(BrokerAddress);
          }
          else
          {
            BrokerConnectionWithCertificate(BrokerAddress);
          }

          DefinedMQTTCommunicationEvents();
          Logger.Info("MQTT successfully stablished connection.");
        }

      }
      catch (Exception ex)
      {
        Logger.Info(string.Format("Could not stablished connection to MQ broker: {0}", ex.Message));

        //don't leave the client connected
        if (AppMQTTClient != null && AppMQTTClient.IsConnected)
          try
          {
            AppMQTTClient.Disconnect();
          }
          catch
          {
            ////Logger.LogError(ex, string.Format("Could not disconnect to MQ broker: {1}", ex.Message));
          }
        //ideally through exception and call make connection on hanle it.
        MakeConnection();
        Logger.Info(string.Format("Try to connect with mqtt"));

      }
      #endregion

    }
    public void MakeDisConnect()
    {
      if (AppMQTTClient != null && AppMQTTClient.IsConnected)
        try
        {
          AppMQTTClient.Disconnect();
        }
        catch (Exception ex)
        {
          Logger.LogError(ex, string.Format("Could not disconnect to MQ broker: {0}", ex.Message));
        }
    }
    public bool client_RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
      return true;
      // logic for validation here
    }
    #endregion

    #region Properties
    private string _clientId;
    public string WillTopic { get; set; }
    readonly object locker = new object();
    public string BrokerAddress
    {
      get
      {
        if (ConfigurationManager.AppSettings["BrokerAddress"] == null)
        {
          return string.Empty;
        }
        return ConfigurationManager.AppSettings["BrokerAddress"].ToString();
      }

    }
    public int BrokerPort
    {
      get
      {
        if (ConfigurationManager.AppSettings["BrokerPort"] == null)
        {
          return 1883;
        }
        return Convert.ToInt32(ConfigurationManager.AppSettings["BrokerPort"]);
      }

    }
    public UInt16 BrokerKeepAlivePeriod
    {
      get
      {
        if (ConfigurationManager.AppSettings["BrokerKeepAlivePeriod"] == null)
        {
          return 3600;
        }
        return Convert.ToUInt16(ConfigurationManager.AppSettings["BrokerKeepAlivePeriod"]);
      }

    }
    public string ClientId
    {
      get
      {
        return _clientId;
        //return Guid.NewGuid().ToString();
        //if (ConfigurationManager.AppSettings["BrokerAccessClientId"] == null)
        //{
        //  return Guid.NewGuid().ToString();
        //}
        //return ConfigurationManager.AppSettings["BrokerAccessClientId"].ToString();
      }
    }
    public MqttClient AppMQTTClient { get; set; }
    public string ClientResponce { get; set; }
    #endregion

    #region Methods



    public string Publish(string messgeTopic, string publishMessage)
    {
      if (AppMQTTClient != null)
      {
        try
        {
          lock (locker)
          {
            ushort msgId = AppMQTTClient.Publish(messgeTopic, // topic
                              Encoding.UTF8.GetBytes(publishMessage), // message body
                              MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, // QoS level
                              false);
          }
        }
        catch (Exception ex)
        {
          //    log.Warn("Error while publishing: " + ex.Message, ex);
        }
      }
      return "Success";


    }

    public string Subscribe(string messgeTopic)
    {
      if (AppMQTTClient != null)
      {
        ushort msgId = AppMQTTClient.Subscribe(new string[] { messgeTopic },
             new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE }
             );
        //Logger.Info(string.Format("Subscription to topic {0}", messgeTopic));
      }
      return "Success";
    }

    /// <summary>
    /// Subscribe to a list of topics
    /// </summary>
    public void Subscribe(IEnumerable<string> messgeTopics)
    {
      foreach (var item in messgeTopics)
        Subscribe(item);
    }

    private void client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
    {
      Logger.Info("MQTT successfully MsgPublished.....messgae..." + e.IsPublished.ToString());
      NotifyMessage("MqttMsgPublished", e.IsPublished.ToString(), string.Empty);
      //Logger.Info(string.Format("Mqtt-Msg-Published to topic {0}", e.IsPublished.ToString()));
      ClientResponce = "Success";
    }



    public void client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
    {
      Logger.Info("MQTT successfully Subscribed.....messgae..." + e.MessageId.ToString());
      NotifyMessage("MqttMsgSubscribed", e.MessageId.ToString(), string.Empty);
      //Logger.Info(string.Format("Mqtt-Msg-Subscribed to topic {0}", e.MessageId.ToString()));

    }

    public void client_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
    {
      ClientResponce = "Success";
    }

    public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
      if (e.Topic.ToString().Contains("feedback"))
      {
        FeedbackPublishReceivedMessage(e);
      }
      else
      {
        NotifyMessage("MqttMsgPublishReceived", Encoding.UTF8.GetString(e.Message), e.Topic.ToString());
      }

      //Logger.Info(string.Format("Mqtt-Msg-Publish-Received to topic {0}", e.Topic.ToString()));
    }

    private void FeedbackPublishReceivedMessage(MqttMsgPublishEventArgs e)
    {
      List<string> feedback = new List<string>();
      foreach (var item in e.Message)
      {
        feedback.Add(item.ToString());
      }
      string joinedFeedback = string.Join(",", feedback);
      NotifyMessage("MqttMsgPublishReceived", joinedFeedback, e.Topic.ToString());
    }

    public void client_ConnectionClosed(object sender, EventArgs e)
    {

      Logger.Info("Connection Closed Envoke");

      if (!(sender as MqttClient).IsConnected || AppMQTTClient == null)
      {
        HandleReconnect();
      }
      Logger.Info("Try to connect with mqtt");
    }

    private async Task TryReconnectAsync(CancellationToken cancellationToken)
    {
      var connected = AppMQTTClient.IsConnected;
      while (!connected && !cancellationToken.IsCancellationRequested)
      {
        try
        {
          MakeConnection();
        }
        catch
        {
          Logger.Info("No connection to...");
        }
        connected = AppMQTTClient.IsConnected;
        await Task.Delay(10000, cancellationToken);
      }
    }


    void HandleReconnect()
    {
      MakeConnection();
    }




    

    #region Delegate and event implementation
    public void NotifyMessage(string NotifyType, string receivedMessage, string receivedTopic)
    {
      if (NotifyType == "MqttMsgPublishReceived")
      {
        InvokeEvents<NotifyMqttMsgPublishReceivedDelegate>(receivedMessage, receivedTopic, NotifyMqttMsgPublishReceivedEvent);
      }

      if (NotifyType == "MqttMsgPublished")
      {
        InvokeEvents<NotifyMqttMsgPublishedDelegate>(receivedMessage, receivedTopic, NotifyMqttMsgPublishedEvent);
      }

      if (NotifyType == "MqttMsgSubscribed")
      {
        InvokeEvents<NotifyMqttMsgSubscribedDelegate>(receivedMessage, receivedTopic, NotifyMqttMsgSubscribedEvent);

      }
    }

    private static void InvokeEvents<T>(string receivedMessage, string receivedTopic, T eventDelegate)
    {
      if (eventDelegate != null)
      {
        var customEventArgs = new CustomEventArgs(receivedMessage, receivedTopic);
        ((Delegate)(object)eventDelegate).DynamicInvoke(customEventArgs);
      }
    }
    #endregion

    #endregion

    #region MQTT connection events

    private void DefinedMQTTCommunicationEvents()
    {

      AppMQTTClient.MqttMsgPublished += client_MqttMsgPublished;//publish
      AppMQTTClient.MqttMsgSubscribed += client_MqttMsgSubscribed;//subscribe confirmation
      AppMQTTClient.MqttMsgUnsubscribed += client_MqttMsgUnsubscribed;
      AppMQTTClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;//received message.
      AppMQTTClient.ConnectionClosed += client_ConnectionClosed;

      ushort submsgId = AppMQTTClient.Subscribe(new string[] { "shhello", "sh/configuration", "sh/command", "sh/feedback/#" },
                        new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                                      MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

    }

    private void BrokerConnectionWithCertificate(string brokerAddress)
    {
      AppMQTTClient = new MqttClient(brokerAddress, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, true, new X509Certificate(Resource.ca), null, MqttSslProtocols.TLSv1_2, client_RemoteCertificateValidationCallback);
      AppMQTTClient.Connect(ClientId, "ash", "ash", false, BrokerKeepAlivePeriod);
    }

    private void BrokerConnectionWithCertificateForWebBroker(string brokerAddress)
    {
      try
      {
        //if (WaitUntilBrokerAlive(brokerAddress))
        //{
        AppMQTTClient = new MqttClient(brokerAddress, MqttSettings.MQTT_BROKER_DEFAULT_SSL_PORT, true, new X509Certificate(WebBrokerResouce.ca), null, MqttSslProtocols.TLSv1_2, client_RemoteCertificateValidationCallback);
        AppMQTTClient.Connect(ClientId, "ash", "ash", false, BrokerKeepAlivePeriod);
        //}
      }
      catch (Exception ex)
      {

        Logger.Error(string.Format("Error on BrokerConnectionWithCertificateForWebBroker {0} ", ex.Message.ToString()));
      }

    }

    private void BrokerConnectionWithoutCertificateForCommand(string brokerAddress)
    {
      AppMQTTClient = new MqttClient(brokerAddress, BrokerPort, false, null, null, MqttSslProtocols.None, null);
      AppMQTTClient.Connect(ClientId, "ash", "ash", false, BrokerKeepAlivePeriod);
      
    }

    private void BrokerConnectionWithoutCertificate(string brokerAddress)
    {
      AppMQTTClient = new MqttClient(brokerAddress, BrokerPort, false, null, null, MqttSslProtocols.None, null);
      MQTTConnectiobn();
    }

    private void LocalBrokerConnection(string brokerAddress)
    {
      AppMQTTClient = new MqttClient(brokerAddress);
      MQTTConnectiobn();
    }

    private void MQTTConnectiobn()
    {

      byte returnCode = AppMQTTClient.Connect(ClientId, "ash", "ash", false, BrokerKeepAlivePeriod);
      if (returnCode != 0)
      {
        MakeConnection();
      }
    }
    #endregion

    private bool IsNetworkAvailable()
    {
      bool isNetworkAlive = false;
      isNetworkAlive = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
      if (isNetworkAlive == false)
      {
        Logger.Info("Network is not available");
      }
      return isNetworkAlive;
    }
  }


  public class CustomEventArgs : EventArgs
  {
    public CustomEventArgs(string receivedMessage, string receivedTopic)
    {
      _receivedMessage = receivedMessage;
      _receivedTopic = receivedTopic;
    }
    private string _receivedMessage;

    public string ReceivedMessage
    {
      get { return _receivedMessage; }
      set { _receivedMessage = value; }
    }


    private string _receivedTopic;
    public string ReceivedTopic
    {
      get { return _receivedTopic; }
      set { _receivedTopic = value; }
    }
  }
}
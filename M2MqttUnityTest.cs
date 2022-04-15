/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json;

/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>
namespace M2MqttUnity.Examples
{
    /// <summary>
    /// Script for testing M2MQTT with a Unity UI
    /// </summary>
    public class M2MqttUnityTest : M2MqttUnityClient
    {
        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;
        [Header("User Interface")]
        public InputField consoleInputField;
        public Toggle encryptedToggle;
        public InputField addressInputField;
        public InputField portInputField;
        public Button connectButton;
        public Button disconnectButton;
        public Button testPublishButton;
        public Button clearButton;
        private Tween twenFade;
        private List<string> eventMessages = new List<string>();
        private bool updateUI = false;

        public Text error;

        [SerializeField]
        public InputField broker;
        [SerializeField]
        public InputField user;
        [SerializeField]
        public InputField pass;

        [SerializeField]
        private GameObject Btn_Quit;
        [SerializeField]
        private CanvasGroup _canvasLayer1;
        [SerializeField]
        private CanvasGroup _canvasLayer2;

        public List<string> topics = new List<string>();

        public SwitchButton btn1, btn2;
        public class Status_Data
        {
            public string temperature { get; set; }
            public string humidity { get; set; }
        }

        public class Status_Device
        {
            public string device { get; set; }
            public string status { get; set; }
        }

        public Text[] textDisplay = new Text[2];
        
        public void Fade(CanvasGroup _canvas, float endValue, float duration, TweenCallback onFinish)
        {
            if (twenFade != null)
            {
                twenFade.Kill(false);
            }

            twenFade = _canvas.DOFade(endValue, duration);
            twenFade.onComplete += onFinish;
        }

        public void FadeIn(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 1f, duration, () =>
            {
                _canvas.interactable = true;
                _canvas.blocksRaycasts = true;
            });
        }

        public void FadeOut(CanvasGroup _canvas, float duration)
        {
            Fade(_canvas, 0f, duration, () =>
            {
                _canvas.interactable = false;
                _canvas.blocksRaycasts = false;
            });
        }

        public void SwitchLayer()
        {
            StartCoroutine(_IESwitchLayer());
        }

        IEnumerator _IESwitchLayer()
        {
            if (_canvasLayer1.interactable == mqttClientConnected)
            {
                FadeOut(_canvasLayer1, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer2, 0.25f);
            }
            else
            {
                FadeOut(_canvasLayer2, 0.25f);
                yield return new WaitForSeconds(0.5f);
                FadeIn(_canvasLayer1, 0.25f);
            }
        }

        public void Setting()
        {
            // Debug.Log("Broker: " + broker.text);
            // Debug.Log("Username: " + user.text);
            // Debug.Log("Password: " + pass.text);
            this.brokerAddress = broker.text;
            this.mqttUserName = user.text;
            this.mqttPassword = pass.text;
            this.Connect();
        }

        // public void TestPublish()
        // {
        //     client.Publish("M2MQTT_Unity/test", System.Text.Encoding.UTF8.GetBytes("Test message"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        //     Debug.Log("Test message published");
        //     AddUiMessage("Test message published.");
        // }

        public void TestPublish() {
            //_controlFan_data = GetComponent<ChuongGaManager>().Update_ControlFan_Value(_controlFan_data);
            //string data = "{\"temperature\":75,\"humidity\":11}";
            //string data = "{\"device\":\"PUMP\",\"status\":\"OFF\"}";
            //client.Publish("/bkiot/1915976/pump", System.Text.Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("publish for pump");
        }

        public void LedPublish() {
            string status = btn1.switchState == true ? "OFF" : "ON";
            string data = "{\"device\":\"LED\",\"status\":\""+status+"\"}";
            client.Publish("/bkiot/1915976/led", System.Text.Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            btn1.setStatus(!btn1.switchState);
        }

        public void PumPublish() {     
            string status = btn2.switchState == true ? "OFF" : "ON";
            string data = "{\"device\":\"PUMP\",\"status\":\""+status+"\"}";
            client.Publish("/bkiot/1915976/pump", System.Text.Encoding.UTF8.GetBytes(data), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            btn2.setStatus(!btn2.switchState);
        }

        

        public void SetBrokerAddress(string brokerAddress)
        {
            //brokerAddress = this.broker.text;
            if (addressInputField && !updateUI)
            {
                this.brokerAddress = brokerAddress;
            }
        }

        public void SetBrokerPort(string brokerPort)
        {
            if (portInputField && !updateUI)
            {
                int.TryParse(brokerPort, out this.brokerPort);
            }
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }


        public void SetUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text = msg;
                updateUI = true;
            }
        }

        public void AddUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text += msg + "\n";
                updateUI = true;
            }
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            SetUiMessage("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            SetUiMessage("Connected to broker on " + brokerAddress + "\n");
            this.SwitchLayer();
            this.TestPublish();
        }

        // protected override void SubscribeTopics()
        // {
        //     client.Subscribe(new string[] { "M2MQTT_Unity/test" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        // }

        protected override void SubscribeTopics()
        {
            foreach (string topic in topics)
            {
                if (topic != "")
                {
                    client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                }
            }
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { "M2MQTT_Unity/test" });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            AddUiMessage("CONNECTION FAILED! " + errorMessage);
            error.text = "Please Login Again";
        }

        protected override void OnDisconnected()
        {
            AddUiMessage("Disconnected.");
        }

        protected override void OnConnectionLost()
        {
            AddUiMessage("CONNECTION LOST!");
        }

        private void UpdateUI()
        {
            if (client == null)
            {
                if (connectButton != null)
                {
                    connectButton.interactable = true;
                    disconnectButton.interactable = false;
                    testPublishButton.interactable = false;
                }
            }
            else
            {
                if (testPublishButton != null)
                {
                    testPublishButton.interactable = client.IsConnected;
                }
                if (disconnectButton != null)
                {
                    disconnectButton.interactable = client.IsConnected;
                }
                if (connectButton != null)
                {
                    connectButton.interactable = !client.IsConnected;
                }
            }
            if (addressInputField != null && connectButton != null)
            {
                addressInputField.interactable = connectButton.interactable;
                addressInputField.text = brokerAddress;
            }
            if (portInputField != null && connectButton != null)
            {
                portInputField.interactable = connectButton.interactable;
                portInputField.text = brokerPort.ToString();
            }
            if (encryptedToggle != null && connectButton != null)
            {
                encryptedToggle.interactable = connectButton.interactable;
                encryptedToggle.isOn = isEncrypted;
            }
            if (clearButton != null && connectButton != null)
            {
                clearButton.interactable = connectButton.interactable;
            }
            updateUI = false;
        }

        protected override void Start()
        {
            SetUiMessage("Ready.");
            updateUI = true;
            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            // Debug.Log("Received: " + msg);
            // StoreMessage(msg);
            if (topic == topics[0])
            {
                ProcessMessageStatus(msg);
            }
            if (topic == topics[1])
            {
                ProcessMessageLed(msg);
            }
            if (topic == topics[2])
            {
                ProcessMessagePump(msg);
            }
        }

        public void ProcessMessageStatus(string msg) {
            Status_Data _status_data = JsonConvert.DeserializeObject<Status_Data>(msg);
            textDisplay[0].text = (_status_data.temperature) + "°C";
            textDisplay[1].text = (_status_data.humidity) + "%";
        }

        public void ProcessMessageLed(string msg) {
            Status_Device _status_data = JsonConvert.DeserializeObject<Status_Device>(msg);
            //btn1.OnSwitchButtonClicked();
            if (_status_data.status=="OFF") {
                btn1.setStatus(false);
            }
            else {
                btn1.setStatus(true);
            }
        }

        public void ProcessMessagePump(string msg) {
            Status_Device _status_data = JsonConvert.DeserializeObject<Status_Device>(msg);
            //btn2.OnSwitchButtonClicked();
            if (_status_data.status=="OFF") {
                btn2.setStatus(false);
            }
            else {
                btn2.setStatus(true);
            }
        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            AddUiMessage("Received: " + msg);
        }

        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
            if (updateUI)
            {
                UpdateUI();
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }
    }
}

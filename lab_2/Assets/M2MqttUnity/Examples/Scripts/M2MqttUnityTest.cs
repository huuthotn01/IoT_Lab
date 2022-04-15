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
using System.Timers;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;

/// <summary>
/// Examples for the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// </summary>
namespace M2MqttUnity.Examples
{
    public class SensorData {
        public string temp {get; set;}
        public string humid {get; set;}
    }

    public class LEDState {
        public string device {get; set;}
        public string status {get; set;}
    }

    public class PumpState {
        public string device {get; set;}
        public string status {get; set;}
    }

    /// <summary>
    /// Script for testing M2MQTT with a Unity UI
    /// </summary>
    public class M2MqttUnityTest : M2MqttUnityClient
    {
        [Tooltip("Set this to true to perform a testing cycle automatically on startup")]
        public bool autoTest = false;
        [Header("User Interface")]
        public GameObject SignIn;
        public GameObject TestUI;
        public InputField signin_watch;
        public InputField dashboard_watch;
        public InputField consoleInputField;
        public InputField addressInputField;
        public InputField addressInputFieldUsername;
        public InputField addressInputFieldPass;
        public InputField portInputField;
        public Button connectButton;
        public string sensorTopic = "";
        public string msg_from_sensor = "";
        public string ledTopic = "";
        public string pumpTopic = "";
        public InputField InputFieldTemp;
        public InputField InputFieldHumid;
        public Image LED_img;
        public Image Pump_img;
        public Sprite img_off;
        public Sprite img_on;

        private List<string> eventMessages = new List<string>();
        private bool updateUI = false;

        // private System.Windows.Forms.Timer aTimer = new System.Windows.Forms.Timer();
        // private System.Timers.Timer aTimer = null;

        public void TestPublish()
        {
            SensorData sensor_info = new SensorData
            {
                temp = "10",
                humid = "59"
            };
            string sensor_info_json = JsonConvert.SerializeObject(sensor_info);
            client.Publish(sensorTopic, System.Text.Encoding.UTF8.GetBytes(sensor_info_json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Debug.Log("Test message published");
            AddUiMessage("Test message published.");
        }

        public void SetBrokerAddress(string brokerAddress)
        {
            if (addressInputField && !updateUI)
            {
                this.brokerAddress = brokerAddress;
            }
        }

        public void SetUsername(string username)
        {
            if (addressInputFieldUsername && !updateUI)
            {
                this.mqttUserName = username;
            }
        }

        public void SetPass(string pass)
        {
            if (addressInputFieldPass && !updateUI)
            {
                this.mqttPassword = pass;
            }
        }

        public void SetBrokerPort(string brokerPort)
        {
            if (portInputField && !updateUI)
            {
                int.TryParse(brokerPort, out this.brokerPort);
            }
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
                consoleInputField.text = msg + "\n";
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
            // SubscribeTopics();

            SignIn.SetActive(false);
            TestUI.SetActive(true);
            /*if (autoTest)
            {
                TestPublish();
            }*/
            // Update();
            //TestPublish();
        }

        protected override void SubscribeTopics()
        {
            if (sensorTopic != "")
            {
                client.Subscribe(new string[] { sensorTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
            Debug.Log("Test Sub Topic");
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { sensorTopic });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            AddUiMessage("CONNECTION FAILED! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            AddUiMessage("Disconnected.");
        }

        public void ChangeLedState()
        {
            if (LED_img.sprite == img_off)
            {
                LEDState led_info = new LEDState
                {
                    device = "LED",
                    status = "ON"
                };
                string led_info_json = JsonConvert.SerializeObject(led_info);
                client.Publish(ledTopic, System.Text.Encoding.UTF8.GetBytes(led_info_json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                LED_img.sprite = img_on;
            } else 
            {
                LEDState led_info = new LEDState
                {
                    device = "LED",
                    status = "OFF"
                };
                string led_info_json = JsonConvert.SerializeObject(led_info);
                client.Publish(ledTopic, System.Text.Encoding.UTF8.GetBytes(led_info_json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                LED_img.sprite = img_off;
            }
            // Debug.Log("LED clicked");
        }

        public void ChangePumpState()
        {
            if (Pump_img.sprite == img_off)
            {
                PumpState pump_info = new PumpState
                {
                    device = "PUMP",
                    status = "ON"
                };
                string pump_info_json = JsonConvert.SerializeObject(pump_info);
                client.Publish(pumpTopic, System.Text.Encoding.UTF8.GetBytes(pump_info_json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                Pump_img.sprite = img_on;
            } else 
            {
                PumpState pump_info = new PumpState
                {
                    device = "PUMP",
                    status = "OFF"
                };
                string pump_info_json = JsonConvert.SerializeObject(pump_info);
                client.Publish(pumpTopic, System.Text.Encoding.UTF8.GetBytes(pump_info_json), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                Pump_img.sprite = img_off;
            }
            // Debug.Log("Pump clicked");
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
                }
            }
            else
            {
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
            if (addressInputFieldUsername != null && connectButton != null)
            {
                addressInputFieldUsername.interactable = connectButton.interactable;
                addressInputFieldUsername.text = mqttUserName;
            }
            if (addressInputFieldPass != null && connectButton != null)
            {
                addressInputFieldPass.interactable = connectButton.interactable;
                addressInputFieldPass.text = mqttPassword;
            }
            if (portInputField != null && connectButton != null)
            {
                portInputField.interactable = connectButton.interactable;
                portInputField.text = brokerPort.ToString();
            }
            updateUI = false;
        }

        protected override void Start()
        {
            SignIn.SetActive(true);
            TestUI.SetActive(false);
            // time_watch.text = "Timer";
            SetUiMessage("Ready.");
            sensorTopic = "/bkiot/1915347/status";
            ledTopic = "/bkiot/1915347/led";
            pumpTopic = "/bkiot/1915347/pump";
            LED_img.sprite = img_off;
            Pump_img.sprite = img_off;
            //IntervalTimeWatch();
            updateUI = true;
            base.Start();
        }

        /*private void IntervalTimeWatch()
        {
            // System.Windows.Forms.Timer aTimer = new System.Windows.Forms.Timer();
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
            aTimer.SynchronizingObject = (System.ComponentModel.ISynchronizeInvoke)this;
            aTimer.AutoReset = true;
        }

        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            time_watch.text = e.SignalTime.ToString();
            Debug.Log(e.SignalTime.ToString());
        }*/

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            Debug.Log("Received: " + msg);
            StoreMessage(msg);
            SensorData sensor_data_json = JsonConvert.DeserializeObject<SensorData>(msg);
            if (InputFieldHumid != null && InputFieldTemp != null) {
                InputFieldTemp.text = sensor_data_json.temp + "°C";
                InputFieldHumid.text = sensor_data_json.humid + "%";
            }
            if (topic == sensorTopic)
            {
                if (autoTest)
                {
                    Debug.Log("Auto Test True");
                    autoTest = false;
                    Disconnect();
                }
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

        public void LogOut() 
        {
            Debug.Log("Log Out");
            SignIn.SetActive(true);
            TestUI.SetActive(false);
            Disconnect();
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
            // Debug.Log("Test Update");
            if (signin_watch != null) signin_watch.text = System.DateTime.Now.ToString("HH:mm:ss");
            if (dashboard_watch != null) dashboard_watch.text = System.DateTime.Now.ToString("HH:mm:ss");
        }

        private void OnDestroy()
        {
            //Disconnect();
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

print("IoT Lab1")

#import needed library
import paho.mqtt.client as mqttclient
import time
import json
import random

# using for get current latitude and longtitude
from locate import getLocateByWebScraping, getLocateByIP

# default
BROKER_ADDRESS = "demo.thingsboard.io"
# default MQTT port
PORT = 1883
THINGS_BOARD_ACCESS_TOKEN = "JRmz4cU7NuwNgu683cuW"

def subscribed(client, userdata, mid, granted_qos):
    print("Subscribed...")

def recv_message(client, userdata, message):
    print("Received: ", message.payload.decode("utf-8"))
    temp_data = {'value': True}
    try:
        jsonobj = json.loads(message.payload)
        if jsonobj['method'] == "setValue":
            temp_data['value'] = jsonobj['params']
            client.publish('v1/devices/me/attributes', json.dumps(temp_data), 1)
    except:
        pass

def connected(client, usedata, flags, rc):
    if rc == 0:
        print("Thingsboard connected successfully!!")
        client.subscribe("v1/devices/me/rpc/request/+")
    else:
        print("Connection is failed")

# login
client = mqttclient.Client("Gateway_Thingsboard")
client.username_pw_set(THINGS_BOARD_ACCESS_TOKEN)

# interrupt
client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
client.on_message = recv_message

# default values
temp, humi = 0,0
counter = 0
longitude = 107.70442
latitude = 16.5283791

def main():
    while True:
        # get latitude and longtitude
        locate = getLocateByIP()
        latitude, longitude = locate[0], locate[1]
        # data = getLocateByWebScraping()
        # latitude, longitude = data[0], data[1]

        temp, humi = random.randint(0, 100), random.randint(0, 100)
        collect_data = {'temperature': temp, 'humidity': humi, 'longitude': float(longitude), 'latitude': float(latitude)}
        client.publish('v1/devices/me/telemetry', json.dumps(collect_data), 1)
        time.sleep(10)

main()
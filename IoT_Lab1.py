print("IoT Lab1")
#####################################################
#                   import library                  #
#####################################################
import paho.mqtt.client as mqttclient
import time
import json
import random
import urllib.request

# default
BROKER_ADDRESS = "demo.thingsboard.io"
# default MQTT port
PORT = 1883
THINGS_BOARD_ACCESS_TOKEN = "ni8pcvHQm1pZErHyTJpp"

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

client = mqttclient.Client("Gateway_Thingsboard")
client.username_pw_set(THINGS_BOARD_ACCESS_TOKEN)

client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
client.on_message = recv_message

# default values
temp, humi = 0,0
counter = 0
longitude = 108.0528
latitude = 12.6674

# get longtirude, latitude by IP Adress
def getAddress():
    lat, lon = 0, 0 
    with urllib.request.urlopen("http://ip-api.com/json/") as url:
        s = url.read()
        data = s.decode('utf-8')
        lat = json.loads(data)['lat']
        lon = json.loads(data)['lon']
    return lat, lon
def main():
    while True:
        locate = getAddress()
        latitude, longitude = locate[0], locate[1]
        temp, humi = random.randint(0, 100), random.randint(0, 100)
        collect_data = {'temperature': temp, 'humidity': humi, 'longitude': longitude, 'latitude': latitude}
        client.publish('v1/devices/me/telemetry', json.dumps(collect_data), 1)
        time.sleep(10)

main()
import urllib.request
import json

def getAddress():
    lat, lon = 0, 0 
    with urllib.request.urlopen("http://ip-api.com/json/") as url:
        s = url.read()
        data = s.decode('utf-8')
        lat = json.loads(data)['lat']
        lon = json.loads(data)['lon']
    return lat, lon

print(getAddress())
##########################################################################################################
#                              USING IP TO GET LATITUDE AND LONGTITUDE                                   #
##########################################################################################################

# https://docs.python.org/3/library/urllib.request.html
import urllib.request
import json
# get longtirude, latitude by IP Adress
# use http://ip-api.com/json/ API to get location data
# https://stackoverflow.com/questions/24678308/how-to-find-location-with-ip-address-in-python
def getLocateByIP():
    lat, lon = 0, 0 
    with urllib.request.urlopen("http://ip-api.com/json/") as url:
        s = url.read()
        # decode bytes
        data = s.decode('utf-8')
        # assign latitude and longtitude values
        lat = json.loads(data)['lat']
        lon = json.loads(data)['lon']
    return lat, lon

##########################################################################################################
#                              USING 3RD TO GET LATITUDE AND LONGTITUDE                                  #
##########################################################################################################

# use https://www.where-am-i.co/track-my-location to find current locattion
# write function to get data from this website
# Requirement:
## 1. have Chrome browser
## 2. download chromedriver.exe (https://chromedriver.chromium.org/downloads) conform with your Chrome version
## 3. Run IoT_Lab1.py and enjoy :)

from selenium import webdriver
from selenium.webdriver.common.by import By

driver = webdriver.Chrome(executable_path='chromedriver.exe')
driver.get("https://www.where-am-i.co/track-my-location")

def getLocateByWebScraping():
    element = driver.find_element(By.XPATH, '/html/body/main/div/div[2]/div[2]/div/table/tbody/tr[2]/td/span')
    data = str(element.text)
    latlon = data.split(', ')
    return latlon
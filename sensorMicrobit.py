def on_received_value(name, value):
    if name == "LED":
        pins.digital_write_pin(DigitalPin.P3, value)
    elif name == "FAN":
        pins.digital_write_pin(DigitalPin.P4, value)
radio.on_received_value(on_received_value)

def Process():
    global counter
    counter += 1
    if counter >= 20:
        counter = 0
        NPNBitKit.dht11_read(DigitalPin.P0)
        radio.send_value("TEMP", NPNBitKit.dht11_temp())
        radio.send_value("HUMI", NPNBitKit.dht11_hum())
counter = 0
led.enable(False)
radio.set_group(178)
counter = 0
pins.digital_write_pin(DigitalPin.P3, 0)
pins.digital_write_pin(DigitalPin.P4, 0)

def on_forever():
    Process()
    basic.pause(100)
basic.forever(on_forever)

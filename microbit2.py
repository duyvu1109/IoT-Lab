import radio

def on_received_value(name, value):
    if name == "LED":
        if value == 0:
            pass
        else:
            pass
    elif name == "FAN":
        if value == 0:
            pass
        else:
            pass
radio.on_received_value(on_received_value)

radio.set_group(1)

def on_forever():
    radio.send_value("TEMP", input.temperature())
    basic.pause(5000)
    radio.send_value("LIGH", input.light_level())
    basic.pause(5000)
basic.forever(on_forever)

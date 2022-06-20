def on_data_received():
    global cmd
    cmd = serial.read_until(serial.delimiters(Delimiters.HASH))
    # basic.show_string(cmd)
    if cmd == "0":
        basic.show_leds("""
            . # # # .
                        # . . . #
                        # . . . #
                        # . . . #
                        . # # # .
        """)
        radio.send_value("LED", 0)
    elif cmd == "1":
        basic.show_leds("""
            . . # . .
                        . # # . .
                        . . # . .
                        . . # . .
                        . # # # .
        """)
        radio.send_value("LED", 1)
    elif cmd == "2":
        basic.show_icon(IconNames.SMALL_HEART)
        basic.show_leds("""
            . # # . .
                        # . . # .
                        . . # . .
                        . # . . .
                        # # # # .
        """)
        radio.send_value("FAN", 0)
    elif cmd == "3":
        basic.show_leds("""
            . # # # .
                        . . . # .
                        . # # # .
                        . . . # .
                        . # # # .
        """)
        radio.send_value("FAN", 1)
serial.on_data_received(serial.delimiters(Delimiters.HASH), on_data_received)

def on_received_value(name, value):
    if name == "TEMP":
        serial.write_string("!1:TEMP:" + ("" + str(value)) + "#")
    if name == "HUMI":
        serial.write_string("!1:LIGHT:" + ("" + str(value)) + "#")
radio.on_received_value(on_received_value)

cmd = ""
toggle = 0
radio.set_group(178)
count = 10

def on_forever():
    global count, toggle
    count += -1
    if count == 0:
        if toggle == 1:
            basic.show_icon(IconNames.SMALL_DIAMOND)
            toggle = 0
        else:
            basic.show_icon(IconNames.DIAMOND)
            toggle = 1
        count = 10
        basic.pause(100)
basic.forever(on_forever)
import serial, time     #need to install pyserial

ser = serial.Serial('COM12', 9600)   #put your COM port for the arduino
#while ser.read(5) != 'READY': pass
i = 0
while 1:
    try:
        ser.write('371449635398431=1812941116203$'.encode('utf-8'))   #put your track2 data before $
        print(ser.read(ser.inWaiting()), i)
        time.sleep(3)     #time delay bewteen each write data
    except Exception as e:
        print("error: ", e)
    i = i + 1
#ser.close()

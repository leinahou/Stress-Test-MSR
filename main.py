import serial, time

ser = serial.Serial('COM12', 9600)
#while ser.read(5)!= 'READY': pass
#ser.write('4012000033330026=20121011000012345678$')

while (1):
    ser.write('371449635398431=1812941116203$'.encode())
    print(ser.readline())
    time.sleep(3)

ser.close()

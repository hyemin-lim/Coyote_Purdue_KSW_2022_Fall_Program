import sounddevice as sd
import time
import serial
from datetime import datetime

# End Node Code (Microphone - Raspberry Pi - ESP32)

fs = 44100 #sampling rate 
second = 0.001 #frame interval 
i = 0

# port setting 
ser = serial.Serial('/dev/ttyUSB_DEV1', 115200, timeout=1)
ser.reset_output_buffer()

while(True):
    arr_high =[]
    record_voice = sd.rec( int(second*fs), samplerate=fs, channels=2 ) 
    sd.wait()
    
    for arr in record_voice:
        # decibel threshold
        if(arr[0]>0.057):
            output = datetime.now()
            output = output.strftime('%Y-%m-%d %H:%M:%S.%f')[14:26]
            print(output)
            output = output.encode('utf-8')
            ser.write(output)
            time.sleep(1)
            break



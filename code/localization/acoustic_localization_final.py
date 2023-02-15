import paho.mqtt.subscribe as subscribe
import json
# import websocket
from sympy import *
import math
from datetime import datetime
import base64

#websocket connection
'''
ws = websocket.WebSocket()
ws.connect("ws://192.168.2.222:3333")
'''

dictData = {}
output_x = 0.0
output_y = 0.0
v =  34.3 # velocity of the sound (cm/ms) 

# Get timestamps from TTN(The Things Network) server
# getting message data: sound id, time_obj
def get_coordinate(m):
    payload = m.payload
    content = json.loads(payload)

    # get coordinates
    if("uplink_message" in content):
        uplink_message = content["uplink_message"]
        
        if("frm_payload" in uplink_message):
            id = content["end_device_ids"]["device_id"]
            time = content["uplink_message"]["frm_payload"][0:16] #datetime now in str
            
            if id not in dictData:
                time_rm = base64.b64decode(time)
                time_rm = time_rm.decode('ascii')

                time_obj = datetime.strptime(time_rm,'%M:%S.%f')
                dictData[id]=time_obj
                
                return (id,time_obj)
            else:
                return 'already in'

# get area
def get_area(time0_obj,time1_obj,time2_obj):
    # parameter
    # time0_obj: timestamp of sensor0
    # time1_obj: timestamp of sensor1
    # time2_obj: timestamp of sensor2

    if (time0_obj<time1_obj and time1_obj<time2_obj):
        area = 1
    elif (time1_obj<time0_obj and time0_obj<time2_obj):
        area = 2
    elif (time1_obj<time2_obj and time2_obj<time0_obj):
        area = 3
    elif (time2_obj<time1_obj and time1_obj<time0_obj):
        area = 4
    elif (time2_obj<time0_obj and time0_obj<time1_obj):
        area = 5
    elif (time0_obj<time2_obj and time2_obj<time1_obj):
        area = 6
    else:
        area = 0
    return (area)

# acoustic localization algorithm 
def localization(r, m, n, td0, td1, td2, area, theta):
    # parameter: r, m, n, td0, td1, td2, area, theta (unit: cm, ms)
    # 2r: the distance between two sensors
    # (m,n): the center of the three sensors which unit is (latitude, longtitude)
    # td0, td1, td2: timestamp difference between three sensors
    # v: the velocity of the sound (34.3cm/ms)
    # coordinates of three sensors
    # => sensor0: (-r, r/sqrt(3)), sensor1: (r, r/sqrt(3)), sensor2: (0, -2r/sqrt(3))

    x = Symbol('x')
    y = Symbol('y')

    # HYPERBOLIC EQUATION
    # between sensor0, sensor1
    # [parallel translation] f(x,y)=0 -> f(x, y-r/sqrt(3))
    eq_0 = Eq((4*(x**2)/float((v*td0)**2)-4*((y-r/math.sqrt(3))**2)/float(4*(r**2)-((v*td0)**2))),1)

    # HYPERBOLIC EQUATION
    # between sensor1, sensor2
    # [Parallel Translation] g(x,y)=0 -> g(x-r/float(2), y+r/2*sqrt(3))
    # [Rotate Transformation] +60 degree (from origin, counterclockwise)
    eq_1 = Eq((4*(((x-r/float(2))/float(2)+math.sqrt(3)*(y+r/(2*math.sqrt(3)))/float(2))**2)/float((v*td1)**2)-4*((-(x-r/float(2))*math.sqrt(3)/float(2)+(y+r/(2*math.sqrt(3)))/float(2))**2)/float(4*(r**2)-((v*td1)**2))),1)

    # HYPERBOLIC EQUATION
    # between sensor2, sensor0
    # [Parallel Translation] h(x,y)=0 -> h(x+r/float(2), y+r/2*sqrt(3))
    # [Rotate Transformation] -60 degree (from origin, counterclockwise)
    eq_2 = Eq((4*(((x+r/float(2))/float(2)-math.sqrt(3)*(y+r/(2*math.sqrt(3)))/float(2))**2)/float((v*td2)**2)-4*((math.sqrt(3)*(x+r/float(2))/float(2)+(y+r/(2*math.sqrt(3)))/float(2))**2)/float(4*(r**2)-((v*td2)**2))),1)

    result_0 = solve([eq_0,eq_1])
    result_1 = solve([eq_1,eq_2])
    result_2 = solve([eq_0,eq_2])

    result = result_0+result_1+result_2

    val = []

    # fix complex numbers of results
    for r in result:
        a = r[x]
        b = r[y]
        a = str(a)
        b = str(b)
        if ' ' in a:
            a = a[0:a.index(' ')]
        if ' ' in b:
            b = b[0:b.index(' ')]
        val.append({'x':float(a),'y':float(b)})

    # To get the intersections which is in the area which is in the possible boundary
    result = []
    for dic in val:
        a = dic['x']
        b = dic['y']
        

        if a<0 and b>-a/math.sqrt(3) and b>a/math.sqrt(3): #area 1 : td0 < td1 < td2
            if (area==1):
                result.append({'x':a,'y':b})
        elif a>0 and b>-a/math.sqrt(3) and b>a/math.sqrt(3): #area 2 : td1 < td0 < td2
            if (area==2):
                result.append({'x':a,'y':b})
        elif a>0 and b>-a/math.sqrt(3) and b<a/math.sqrt(3): #area 3 : td1 < td2 < td0
            if (area==3):
                result.append({'x':a,'y':b})
        elif a>0 and b<-a/math.sqrt(3) and b<a/math.sqrt(3): #area 4 : td2 < td1 < td0
            if (area==4):
                result.append({'x':a,'y':b})
        elif a<0 and b<-a/math.sqrt(3) and b<a/math.sqrt(3): #area 5 : td2 < td0 < td1 
            if (area==5):
                result.append({'x':a,'y':b})
        elif a<0 and b<-a/math.sqrt(3) and b>a/math.sqrt(3): #area 6 : td0 < td2 < td1 
            if (area==6):
                result.append({'x':a,'y':b})
        else:
            print('NONE AREA')

    # Average coordinate of Intersections
    sum_x = 0.0
    sum_y = 0.0
    
    # Calculate sum_x and sum_y
    if len(result)!=0:
        for r in result:
            sum_x += r['x']
            sum_y += r['y']
        output_x = sum_x/float(len(result))
        output_y = sum_y/float(len(result))
    
        print('output_x(cm) : ',output_x)
        print('output_y(cm) : ',output_y)
        
        # Convert cm, ms unit into latitude and longtitude unit 
        result_x = output_x*math.cos(theta) - output_y*math.sin(theta)
        result_x = n+0.0000000902*result_x #Lng 
        result_y = output_x*math.sin(theta) + output_y*math.cos(theta)
        result_y = m+0.0000000902*result_y #Lat

        return (result_y, result_x, 1.0) #1.0 represents that it successfully get the coordinate result.
    
    # No the intersection result 
    else:
        print('================ No Intersection ================')
        return (0.0, 0.0, 0.0)


while True:
    # Paho MQTT subscribe
    sub = subscribe.simple(topics=['#'], keepalive=10 ,hostname="nam1.cloud.thethings.network", port=1883, auth={'username':"esp32-sound",'password':"NNSXS.7ZNIO2YWWQ4IOZYXW75QSFRFATNRUXARKVAOCLQ.GSQBD2I3S2AFTDHBUDYFXENGJJSA7DFEOPR4BN3JKG4DCLH25WLA"}, msg_count=3)
    for element in sub:
        print(get_coordinate(element))

    # if the datas successfully get from three acoustic sensors
    if "sound1" in dictData and "sound2" in dictData and "sound3" in dictData:
        print('========== Receive Done ==========')

        time0_obj = dictData.get('sound3') #sound3 indicates sensor0
        time1_obj = dictData.get('sound1') #sound1 indicates sensor1 
        time2_obj = dictData.get('sound2') #sound2 indicates sensor2

        td0 = abs(time1_obj-time0_obj)
        td1 = abs(time1_obj-time2_obj)
        td2 = abs(time2_obj-time0_obj)

        # time difference - multiple expirical value
        td0 = td0.total_seconds()  *100
        td1 = td1.total_seconds()  *100
        td2 = td2.total_seconds()  *100

        # if it get the right value of the right condition of hyperbolic equation
        if(td0<50 and td1<50 and td2<50):
            area = get_area(time0_obj,time1_obj,time2_obj) # get the area range 
            print('area is -> ',area)

            # input
            r = 152.4 # cm
            theta = 8 # Angle (radian) between origin coordinate plane and the experiment setting coordinate plane
            m = 40.42622175517373
            n = -86.90966878841449
            result = localization(r,m,n,td0,td1,td2,area,theta) #localization code returns (Lat, Lng)
            print(str(result[0])+","+str(result[1])) #Lat, Lng
            if(result[2]==1.0):
                print("success")    
                # sending websocket       
                #ws.send(str(result[0])+","+str(result[1])) # Sending (Lat, Lng) via web socket
    dictData.clear()
    sub.clear()

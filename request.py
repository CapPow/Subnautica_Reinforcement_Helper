import zmq
import time
import random

context = zmq.Context()
socket = context.socket(zmq.REQ)
socket.connect("tcp://localhost:12346")

TIMEOUT = 5000
time.sleep(3)

TestScore = True

if TestScore:
        for i in range(1000):
                time.sleep(0.1)
                socket.send_string("get_outputs")
                poller = zmq.Poller()
                poller.register(socket, zmq.POLLIN)
                evt = dict(poller.poll(TIMEOUT))
                if evt:
                        if evt.get(socket) == zmq.POLLIN:
                                response = socket.recv_string()
                                #score, pos = response.split("|")
                                print(response)

                else:
                        print("Connection failed, Attempting to wait")
                        break

else:
        for i in range(10):
                time.sleep(10)
                x = str(float(random.randrange(0, 800)))
                y = "10.00"
                z = str(float(random.randrange(-800, 800)))
                loc_string = f"warp|{x}, {y}, {z}"
                print(loc_string)
                socket.send_string(loc_string)
                poller = zmq.Poller()
                poller.register(socket, zmq.POLLIN)
                evt = dict(poller.poll(TIMEOUT))
                if evt:
                        if evt.get(socket) == zmq.POLLIN:
                                response = socket.recv_string()
                                #score, pos = response.split("|")
                                print(response)

                else:
                        print("Connection failed, Attempting to wait")
                        break


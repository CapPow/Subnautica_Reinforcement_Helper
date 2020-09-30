import zmq
import time
import random
import base64
import numpy as np
import io
from PIL import Image
context = zmq.Context()
socket = context.socket(zmq.REQ)
socket.connect("tcp://localhost:12346")

TIMEOUT = 5000
time.sleep(2)
for i in range(100):
        time.sleep(0.05)
        socket.send_string("get_outputs")
        poller = zmq.Poller()
        poller.register(socket, zmq.POLLIN)
        evt = dict(poller.poll(TIMEOUT))
        if evt:
                if evt.get(socket) == zmq.POLLIN:
                        response = socket.recv_string()
                        score, base64string = response.split("|")

                        jpg_img = base64.b64decode(base64string)
                        with open(f"{i}.jpg", "wb") as fh:
                                fh.write(jpg_img)
                        print(f"score was:{score}, fn:{i}.png")
                        if i == 0:
                                print(base64string)
        else:
                print("Connection failed")
                break

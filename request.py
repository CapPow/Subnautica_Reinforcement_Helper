import zmq
import time

"""
adapted from: https://github.com/sye8/Python-Unity-ZMQ
"""

context = zmq.Context()
requestSocket = context.socket(zmq.REQ)
requestSocket.connect("tcp://localhost:5558")

time.sleep(3)
while True:
    
    poller = zmq.Poller()
    poller.register(requestSocket, zmq.POLLIN)
    
    # Send On
    msg = b"get_outputs"
    requestSocket.send(msg)
    
    
    # Receive Acknowledgement
    ack = dict(poller.poll(2000))
    if ack:
        print("ack true")
        print(ack)
        print(dir(ack))
        if ack.get(requestSocket) == zmq.POLLIN:
            try:
                outputs = requestSocket.recv(zmq.NOBLOCK)

                score, base64string = outputs.split("|")
                jpg_img = base64.b64decode(base64string)
                
                with open(f"{img_name}.jpg", "wb") as fh:
                    fh.write(jpg_img)
                
                print(f"score was:{score}, fn:{img_name}.png")

            except zmq.Again as e:
                print("No response")
    time.sleep(3)

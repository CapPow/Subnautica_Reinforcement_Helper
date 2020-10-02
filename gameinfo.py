# -*- coding: utf-8 -*-
"""
Created on Wed Sep 30 22:19:49 2020

@author: qvd441
"""

import zmq
import random


class InfoSocket():
    """
    A pipeline for retrieving Subnautica game info
    
    """
    def __init__(self):
        context = zmq.Context()
        socket = context.socket(zmq.REQ)
        self.socket = socket
        self.socket.connect("tcp://localhost:12346")
        self.socket = socket
        self.TIMEOUT = 5000

    def await_command_reply(self):
        socket = self.socket
        poller = zmq.Poller()
        poller.register(socket, zmq.POLLIN)
        evt = dict(poller.poll(self.TIMEOUT))
        if evt:
            if evt.get(socket) == zmq.POLLIN:
                response = socket.recv_string()
                if not response:
                    raise IOError('Socket connection seems dead')
                elif response == "not a proper request":
                    raise IOError('Socket reported: "not a proper request"')
        return True

    def req_outputs(self):
        socket = self.socket
        socket.send_string("get_outputs")
        poller = zmq.Poller()
        poller.register(socket, zmq.POLLIN)
        evt = dict(poller.poll(self.TIMEOUT))
        if evt:
            if evt.get(socket) == zmq.POLLIN:
                response = socket.recv_string()
                score, x, y, z = response.split("|")
                score = float(score)
                x = round(float(x), 1)
                y = round(float(y), 1)
                z = round(float(z), 1)
                return score, x, y, z
        return False

    def reset_agent_view(self):
        """
        socket expects command formatted as "resetview"
        """
        socket = self.socket
        socket.send_string("resetview")
        self.await_command_reply()
    
    def warp_random(self, region_ident=False):
        """
        Either I mixed up the indices or subnautica's coordinate system is odd
        None-the-less, x = W/E, y = depth, z = N/S
        here is a useful map: https://cdn.mos.cms.futurecdn.net/ybU9mgJ67wiSqf7SGUuqi9-970-80.jpg
        
        region_ident, optional integer forcing the warp to be within a specific region
        """
        socket = self.socket
        
        # to avoid warping into solid structures:
        # choose from a group of predefined (relatively) safe regions
        if (not region_ident) or (region_ident>2):
            region_ident = random.randint(0, 2)
        # prefer the "safe" biome bounds
        if region_ident <= 1:
            x = random.randrange(-500, 350)
            z = random.randrange(-500, 500)
        elif region_ident == 2:
            x = random.randrange(-1500, 0)
            z = random.randrange(-600, 1500)

        x = str(float(x))
        y = "5.00"
        z = str(float(z))
        loc_string = f"warp|{x}, {y}, {z}"
        print(loc_string)
        socket.send_string(loc_string)
        self.await_command_reply()

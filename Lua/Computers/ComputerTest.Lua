ws = http.websocket("ws://127.0.0.1:8080/ws")

local readme = [[
My Turtle
=========
ID: Turtle_12
Fuel: Lava
Job: Mining
]]

ws.send(readme)
from socket import *
import webbrowser
import random

# from https://stackoverflow.com/questions/166506/finding-local-ip-addresses-using-pythons-stdlib
s = socket(AF_INET, SOCK_DGRAM)
s.connect(('8.8.8.8', 80))
IP = s.getsockname()[0]
s.close()

port = 8605
# start web server
serverSocket = socket(AF_INET, SOCK_STREAM)
serverSocket.bind((IP, port))
serverSocket.listen(1)

# open server inside browser
print('Server is now online at {0}:{1}'.format(IP, port))
webbrowser.open('http://{0}:{1}'.format(IP, port))

# infinite loop for server to keep running
while True:
    client, addr = serverSocket.accept()
    response = client.recv(1024)

    # header + data to be sent
    data = b'[]'
    header = b'HTTP/1.0 200 OK\n'
    content_type = b'Content-Type: text/html\n\n'

    # 30% chance when it will sends []
    num = random.randint(0, 10)
    if (num > 2):
      data = open('tempArenaInfo.json', 'rb').read()
      print('Empty data sent')

    # send eader
    client.send(header)
    client.send(content_type)
    # send requested file
    client.send(data)

    client.close()

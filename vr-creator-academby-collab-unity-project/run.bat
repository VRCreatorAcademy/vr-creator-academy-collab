@ECHO
ECHO Launching network test.
start "server" ".\Build\VR Creator Academy Collab.exe" -logfile log-server.txt -mlapi server 
timeout /t 2
start "client" ".\Build\VR Creator Academy Collab.exe" -logfile log-client.txt -mlapi client
timeout /t 2
start "host" ".\Build\VR Creator Academy Collab.exe" -logfile log-host.txt -mlapi host

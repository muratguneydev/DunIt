#!/bin/bash
set -e

# Start virtual display
export DISPLAY=:99
rm -f /tmp/.X99-lock
Xvfb "$DISPLAY" -screen 0 1280x720x24 &

# Wait for display to be ready
sleep 1

# Start VNC server (port 5900) and noVNC web proxy (port 7900)
x11vnc -display "$DISPLAY" -forever -nopw -quiet -rfbport 5900 &
/usr/share/novnc/utils/novnc_proxy --listen 7900 --vnc localhost:5900 &

# Give noVNC a moment to start, then prompt the user to connect
sleep 1
PAUSE=${PAUSE_BEFORE_TESTS:-5}
echo ""
echo "  Open http://localhost:7900/vnc.html in your browser to watch the tests."
echo "  Tests start in ${PAUSE} seconds..."
echo ""
sleep "$PAUSE"

# Run the tests
exec "$@"

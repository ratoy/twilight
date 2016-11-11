#!/bin/bash
#resolution
export DISPLAY=":0"
w=$(xrandr --current | grep '*' | uniq | awk '{print $1}' | cut -d 'x' -f1)
h=$(xrandr --current | grep '*' | uniq | awk '{print $1}' | cut -d 'x' -f2)
#mono
EXEFOLDER=/home/wuhao/work/own/twilight/twilight/bin/Debug
PNGFILE=/home/wuhao/wallpaper/screen.png
mono $EXEFOLDER/twilight.exe -d -s $w,$h -o $PNGFILE 
#set wallpaper
dconf write /org/mate/desktop/background/picture-filename "'$PNGFILE'"

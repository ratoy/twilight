#!/bin/bash
EXEFILE=../twilight/bin/Debug/twilight.exe
PNGFILE=/home/wuhao/wallpaper/screen.png
mono $EXEFILE -o $PNGFILE
dconf write /org/mate/desktop/background/picture-filename "'$PNGFILE'"

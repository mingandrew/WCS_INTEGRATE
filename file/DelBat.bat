@echo off

set srcDir="G:\kedaProject\WCS_INTEGRATE\file"

set daysAgo=90

echo Delete the Log 90 days ago...

forfiles /p %srcDir% /s /d -%daysAgo% /c "cmd /c del /f /s /q @path && rd /s /q @path"

echo Over.
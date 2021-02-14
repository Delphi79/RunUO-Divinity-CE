@echo off

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc /optimize /unsafe /t:exe /out:RunUO.exe /win32icon:Server\runuo.ico /d:NEWTIMERS /d:NEWPARENT /recurse:Server\\*.cs

:end
pause


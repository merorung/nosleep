@echo off
rem Builds with the C# compiler bundled with Windows. No install needed.
cd /d "%~dp0"
"%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /codepage:65001 /target:winexe /optimize+ /out:NoSleep.exe NoSleep.cs

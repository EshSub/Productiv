# PRODUCTIV 

<h3>Maximize window to a new virtual desktop implemented for windows</h3>

One of the most <b> underrated </b> productivity features available in Windows is the <b>virtual desktop system</b>. Due to its less user-friendliness, most people don't make the maximum use of virtual desktops.

Productiv is a tool for Windows machines that <b>enhances the usability of virtual desktops</b> in Windows by making windows behave similar to MacBooks when maximizing a window (<b>Maximizing a window to a new virtual desktop</b>) so that users can switch between Maximized windows using the four finger gesture similar to in a MacBook.

## Features

The following actions will be done automatically, when,
- Window maximized : sends the window to new virtual desktop.
<br> ![on maximized](https://github.com/EshSub/Productiv/blob/master/docs/gifs/windowMaximized.gif)
- Window minimized :  sends the window to main virtual desktop and remove the previous virtual desktop.
<br> ![on minimized](https://github.com/EshSub/Productiv/blob/master/docs/gifs/windowMinimzed.gif)
- App launched maximized : sends the window to new virtual desktop.
<br> ![launched minimized](https://github.com/EshSub/Productiv/blob/master/docs/gifs/launchedMaximized.gif)
- App launched minimized : if in a dedicated virtual desktop, launched app is sent to the main virtual desktop.
- Maximized app closed : remove the dedicated virtual desktop.
  <br> ![on close](https://github.com/EshSub/Productiv/blob/master/docs/gifs/windowClose.gif)
    
## Installation and usage

1. Download and install the setup from releases (tags).
2. Run the application `Productiv.exe` at `c:/Program Files (x86)/EshSub/Productiv/`.
3. Tick the `start on boot` to start app automatically on boot (optional).
4. Minimize the app to keep it running in background.

## Motivation

I used to use a MacBook and suddenly had to switch to Windows. After the switch, one of the main things I missed in Windows was the maximize windows to a new workspace (virtual desktop equivalent in MacOS). I searched the internet for a tool to get that feature to Windows but was unable to find one. Therefore I started making it myself. After a lot of research and development, I was finally able to create it. 

## Where is the source code?

I'm still cleaning, refactoring, and finalizing the code and will upload it ASAP. 

## Without these work this wouldn't have been possible

[CBT hooks tutorial](https://www.codeproject.com/Articles/18638/Using-Window-Messages-to-Implement-Global-System-H)

[VirtualDesktop by MScholtes](https://github.com/MScholtes/VirtualDesktop)

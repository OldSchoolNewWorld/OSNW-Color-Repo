# OSNW-Color-Dialog  

This is a custom WPF dialog to work with color spaces and color manipulations. 
There are two main groups of work screens. One group provides color selection 
in various color spaces. The other group performs manipulations of colors.  

xxxxxxxxxxxxxxxxxxxxxxxx  
The color spaces in use currently include:
* RGB - Red, Green, Blue.
* HTML - Hexadecimal representation of RGB.
* HSL - Hue, Saturation, Luminance.
* HSV - Hue, Saturation, Value.
* System.Windows.Media.Color - Describes a color in terms of alpha, red, green, 
and blue channels.
* System.Drawing.Color - Represents an ARGB (alpha, red, green, blue) color.
xxxxxxxxxxxxxxxxxxxxxxxx  

xxxxxxxxxxxxxxxxxxxxxxxx  
The color manipulations include:
* Shade - created by adding black pigment to any hue.
* Tint - created by adding white to any hue.
* Tone - created by adding gray to any hue.
* Blend - Combines two specified colors, in specified proportions.
xxxxxxxxxxxxxxxxxxxxxxxx  

xxxxxxxxxxxxxxxxxxxxxxxx  
Other utilities currently include:
* ContrastingBW - Determines Black or White as the best contrasting color for a
color.
* HueScaleEnum - ??????????????????
* GetHueFromPixel - ??????????????????
* PixelsToImageSource - ??????????????????
xxxxxxxxxxxxxxxxxxxxxxxx  

## Source

The project for the dialog is located at 
[OSNW-Color-Dialog](https://github.com/OldSchoolNewWorld/OSNW-Color-Repo/tree/master/OSNW-Color-Dialog), 
as part of 
[OSNW-Color-Repo](https://github.com/OldSchoolNewWorld/OSNW-Color-Repo). A 
demo/test project for the dialog is located at 
[OSNW-Color-Dialog-Demo](https://github.com/OldSchoolNewWorld/OSNW-Color-Repo/tree/master/OSNW-Color-Dialog/OSNW-Color-Dialog-Demo).

## Dependencies

There is a dependency on OSNW-Color-Utilities. The project for the referenced 
utilities is located at 
[OSNW-Color-Utilities](https://github.com/OldSchoolNewWorld/OSNW-Color-Repo/tree/master/OSNW-Color-Utilities), 
as part of the 
[OSNW-Color-Repo](https://github.com/OldSchoolNewWorld/OSNW-Color-Repo) 
repository. The assembly containing the utilities is 'OSNW.ColorUtilities.dll'. 
The namespace is 'OSNW.Graphics.ColorUtilities'.
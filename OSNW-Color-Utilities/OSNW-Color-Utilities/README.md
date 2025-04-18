# OSNW-Color-Utilities  

This is a collection of utilities to work with color spaces and color 
manipulations. There are two main groups of utilities. One group performs 
conversions between various color spaces. The other group performs 
manipulations of colors. A third group provides other special purpose 
utilities.

The color spaces in use currently include:
* RGB - Red, Green, Blue.
* HTML - Hexadecimal representation of RGB.
* HSL - Hue, Saturation, Luminance.
* HSV - Hue, Saturation, Value.
* System.Windows.Media.Color - Describes a color in terms of alpha, red, green, 
and blue channels.
* System.Drawing.Color - Represents an ARGB (alpha, red, green, blue) color.

The color manipulations include:
* Shade - created by adding black pigment to any hue.
* Tint - created by adding white to any hue.
* Tone - created by adding gray to any hue.
* Blend - Combines two specified colors, in specified proportions.

Other utilities currently include:
* ContrastingBW - Determines Black or White as the best contrasting color for a 
color.
* HueScaleEnum - Allows control over how hue angles are specified.
* GetHueFromPixel - Determine the angle from center for a pixel in a 
`System.Windows.Controls.Image`.
* PixelsToImageSource - Creates a source for a `System.Windows.Controls.Image` 
and fills it with a specified array of pixels.

## Source

The project for the utilities is located at 
[OSNW-Color-Utilities](https://github.com/OldSchoolNewWorld/OSNW-Color-Repo/tree/master/OSNW-Color-Utilities), 
as part of 
[OSNW-Color-Repo](https://github.com/OldSchoolNewWorld/OSNW-Color-Repo/tree/master). The 
assembly containing the utilities is 'OSNW.ColorUtilities.dll'. The namespace 
is 'OSNW.Graphics.ColorUtilities'. A unit test project for the utilities is located at 
[OSNW-Color-Utilities-Unit-Test](https://github.com/OldSchoolNewWorld/OSNW-Color-Repo/tree/master/OSNW-Color-Utilities/OSNW-Color-Utilities-Unit-Test).

## Dependencies

There are no dependencies for the utilities aside from .NET.
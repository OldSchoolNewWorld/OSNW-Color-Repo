Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports System.Text.RegularExpressions

' TODO:
' Where to get comparisons for tone: colors.dopely.top. Anything more on point???
' Done. Add unit-tests of bad inputs for the HTML conversions.
' Why does IntelliSense code completion cause exceptions, and hover hints fail
'   to show up on OSNW references?
'   A build creates the XML file correctly.
'   Does the use of <include file="CommentFile.xml" path="Docs/Members[@name='ColorForceEasy']/*"/>
'     cause the problem?
'     Commenting out those entries seems to cure it right away in Object Browser. XML shows up there.
'     Hovering did not cure until VS was restarted.
'     Code completion also started working.
'   Maybe because the sample used did not include "<?xml version="1.0" encoding="utf-8" ?>"?
'     This link shows an example that DOES have the header.
'       https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags#include
'   Not alone:
'     https://stackoverflow.com/questions/58505706/c-sharp-xml-documentation-include-tag-not-appearing-in-intellisense
'     https://stackoverflow.com/questions/28350340/how-do-i-show-content-from-an-included-xml-document-in-intellisense-tooltips-usi

'''' <summary>
'''' The <c>OSNW.Graphics.ColorUtilities</c> class provides utilities to work
'''' with color. It contains utilities for conversion between various color
'''' spaces used in software. It also contains routines to to work with shades,
'''' tints, and tones of colors.
'''' </summary>
'''' <remarks>
'''' <include file="CommentFile.xml"
'''' path="Docs/Members[@name='ColorForceEasy']/*"/>
'''' <include file="CommentFile.xml"
'''' path="Docs/Members[@name='WarnReverse']/*"/>
'''' xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
'''' xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
'''' xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
'''' </remarks>
''' <summary>
''' The <c>OSNW.Graphics.ColorUtils</c> class provides utilities to work with
''' color. It contains utilities for conversion between various color spaces
''' used in software. It also contains routines to to work with shades, tints,
''' and tones of colors.
''' </summary>
''' <remarks>
''' <para>
''' Rather than throwing an exception, or returning possibly-wild results, for
''' out-of-range input(s) the EasyXXXtoYYY converters return a recognizable set
''' of wrong converted values. The forced results are defined by
''' <see cref="FORCEDHSLH"/>, <see cref="FORCEDHSLS"/>,
''' <see cref="FORCEDHSLL"/>, <see cref="FORCEDHSVH"/>,
''' <see cref="FORCEDHSVS"/>, <see cref="FORCEDHSVV"/>,
''' <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
''' <see cref="FORCEDRGBB"/>.
''' </para>
''' <para>
''' Red, green, and blue values on displays and printers are represented as
''' byte values; these conversions use <c>Double</c>s. Therefore,
''' XXX =&gt; YYY =&gt; XXX may not result in exact before/after matches.
''' </para>
''' </remarks>
Public Class ColorUtilities

    ' Ref: System.Windows.Media Namespace
    ' https://learn.microsoft.com/en-us/dotnet/api/system.windows.media?view=windowsdesktop-8.0

    ' Reusables.
    Private Const ONETHIRD As System.Double = 1.0 / 3.0
    Private Const TWOTHIRDS As System.Double = 2.0 / 3.0
    Private Const TWOPI As System.Double = 2.0 * System.Math.PI
    Private Const HALFPI As System.Double = System.Math.PI / 2.0
    Private Const HALF3PI As System.Double = 3.0 * System.Math.PI / 2.0

    ''' <summary>
    ''' The forced HSL Hue return value for out-of-range inputs to EasyXXXtoHSL
    ''' converters.
    ''' </summary>
    Public Const FORCEDHSLH As System.Double = 0.0

    ''' <summary>
    ''' The forced HSL Saturation return value for out-of-range inputs to
    ''' EasyXXXtoHSL converters.
    ''' </summary>
    Public Const FORCEDHSLS As System.Double = 0.0

    ''' <summary>
    ''' The forced HSL Luminance return value for out-of-range inputs to
    ''' EasyXXXtoHSL converters.
    ''' </summary>
    Public Const FORCEDHSLL As System.Double = 0.5

    ''' <summary>
    ''' The forced HSV Hue return value for out-of-range inputs to EasyXXXtoHSV
    ''' converters.
    ''' </summary>
    Public Const FORCEDHSVH As System.Double = 0.0

    ''' <summary>
    ''' The forced HSV Saturation return value for out-of-range inputs to
    ''' EasyXXXtoHSV converters.
    ''' </summary>
    Public Const FORCEDHSVS As System.Double = 0.0

    ''' <summary>
    ''' The forced HSV Value return value for out-of-range inputs to
    ''' EasyXXXtoHSV converters.
    ''' </summary>
    Public Const FORCEDHSVV As System.Double = 0.5

    ''' <summary>
    ''' The forced RGB Red return value for out-of-range inputs to EasyXXXtoRGB
    ''' converters.
    ''' </summary>
    Public Const FORCEDRGBR As System.Double = 128.0

    ''' <summary>
    ''' The forced RGB Green return value for out-of-range inputs to
    ''' EasyXXXtoRGB converters.
    ''' </summary>
    Public Const FORCEDRGBG As System.Double = 128.0

    ''' <summary>
    ''' The forced RGB Blue return value for out-of-range inputs to EasyXXXtoRGB
    ''' converters.
    ''' </summary>
    Public Const FORCEDRGBB As System.Double = 128.0

    '''' <summary>
    '''' Specifies the relative proportion of Color1 vs. Color2 to be assumed
    '''' when a bad <c>color1Proportion</c> is provided to
    '''' <see cref="BlendColors"/>.
    '''' </summary>
    'Const FORCEDCOLOR1PROPORTION As System.Double = 60

    '''' <summary>
    '''' Specifies the relative proportion of Color1 vs. Color2 to be assumed
    '''' when a bad <c>color2Proportion</c> is provided to
    '''' <see cref="BlendColors"/>.
    '''' </summary>
    'Const FORCEDCOLOR2PROPORTION As System.Double = 40

    ''' <summary>
    ''' Specifies the value to set full opacity on a generated color.
    ''' </summary>
    Const FORCEOPACITY255 As System.Byte = 255

    ''' <summary>
    ''' Specifies the values used to indicate the scale range used for a Hue.
    ''' </summary>
    Public Enum HueScaleEnum

        ''' <summary>
        ''' Range 0.0 to almost (2 * PI).
        ''' </summary>
        Radians

        ''' <summary>
        ''' Range 0.0 to almost 360.0.
        ''' </summary>
        Degrees

        ''' <summary>
        ''' Range 0.0 to almost 1.0.
        ''' </summary>
        Fraction

        ''' <summary>
        ''' Range 0.0 to almost 240.0.
        ''' Used for some Microsoft applications.
        ''' Subsequent uses may require rounding to use all possible values.
        ''' </summary>
        Scale240 ' Some Microsoft applications.

        ''' <summary>
        ''' Range 0.0 to almost 255.0.
        ''' Used for some Microsoft applications.
        ''' Subsequent uses may require rounding to use all possible values.
        ''' </summary>
        Scale255 ' Some Microsoft applications.

    End Enum

#Region "Contrast"

    ''' <summary>
    ''' Calculates the 3-dimensional distance between two points in a 3D space.
    ''' </summary>
    ''' <param name="x1">Specifies the X coordinate of the first point.</param>
    ''' <param name="y1">Specifies the Y coordinate of the first point.</param>
    ''' <param name="z1">Specifies the Z coordinate of the first point.</param>
    ''' <param name="x2">Specifies the X coordinate of the second point.</param>
    ''' <param name="y2">Specifies the Y coordinate of the second point.</param>
    ''' <param name="z2">Specifies the Z coordinate of the second point.</param>
    ''' <returns>
    ''' Returns the 3D distance from the first point as specified by
    ''' <paramref name="x1"/>, <paramref name="y1"/>, and <paramref name="z1"/>
    ''' to the second point as specified by <paramref name="x2"/>,
    ''' <paramref name="y2"/>, and <paramref name="z2"/>.
    ''' </returns>
    Public Shared Function Hypotenuse3(ByVal x1 As System.Double,
        ByVal y1 As System.Double, ByVal z1 As System.Double,
        ByVal x2 As System.Double, ByVal y2 As System.Double,
        ByVal z2 As System.Double) As System.Double

        ' Ref: 3D Distance Formula
        ' https://www.cuemath.com/3d-distance-formula/
        Return System.Math.Sqrt((x2 - x1) ^ 2 + (y2 - y1) ^ 2 + (z2 - z1) ^ 2)
    End Function ' Hypotenuse3

    ''' <summary>
    ''' Calculates the 3-dimensional distance from the origin to a point in a 3D
    ''' space.
    ''' </summary>
    ''' <param name="x">Specifies the X coordinate of the point.</param>
    ''' <param name="y">Specifies the Y coordinate of the point.</param>
    ''' <param name="z">Specifies the Z coordinate of the point.</param>
    ''' <returns>
    ''' Returns the 3D distance from the origin (0, 0, 0) to the point specified
    ''' by <paramref name="x"/>, <paramref name="y"/>, and <paramref name="z"/>.
    ''' </returns>
    Public Shared Function Hypotenuse3(ByVal x As System.Double,
        ByVal y As System.Double, ByVal z As System.Double) As System.Double

        ' Ref: 3D Distance Formula
        ' https://www.cuemath.com/3d-distance-formula/
        Return System.Math.Sqrt(x ^ 2 + y ^ 2 + z ^ 2)
    End Function ' Hypotenuse3

    ''' <summary>
    ''' Selects either black or white for maximum contrast to, for example, a
    ''' background color.
    ''' </summary>
    ''' <param name="r">Specifies the red component of the reference
    ''' color.</param>
    ''' <param name="g">Specifies the green component of the reference
    ''' color.</param>
    ''' <param name="b">Specifies the blue component of the reference
    ''' color.</param>
    ''' <returns>
    ''' Either black or white as a <c>System.Windows.Media.Color</c>.
    ''' </returns>
    Public Shared Function ContrastingBW(ByVal r As System.Byte,
        ByVal g As System.Byte, ByVal b As System.Byte) _
        As System.Windows.Media.Color

        ' Ref: 3D Distance Formula
        ' https://www.cuemath.com/3d-distance-formula/

        ' No argument checking. Accept any valid System.Byte values.

        ' Hypotenuse3 is reproduced here to avoid excess subroutine calls when
        ' this method is called from a loop.
        Dim DistFromBlack As System.Double =
            System.Math.Sqrt(r ^ 2 + g ^ 2 + b ^ 2)
        Dim DistFromWhite As System.Double =
            System.Math.Sqrt((255 - r) ^ 2 + (255 - g) ^ 2 + (255 - b) ^ 2)

        Return If(DistFromWhite > DistFromBlack,
            System.Windows.Media.Colors.White,
            System.Windows.Media.Colors.Black)

    End Function ' ContrastingBW

    ''' <summary>
    ''' Selects either black or white for maximum contrast to, for example, a
    ''' background color.
    ''' </summary>
    ''' <param name="aColor">Specifies the reference color.</param>
    ''' <returns>
    ''' Either black or white as a <c>System.Drawing.Color</c>.
    ''' </returns>
    Public Shared Function ContrastingBW(ByVal aColor As System.Drawing.Color) _
        As System.Drawing.Color

        ' No argument check required. Accept any valid System.Drawing.Color.
        With aColor
            Dim ResultSMC As System.Windows.Media.Color =
                ContrastingBW(aColor.R, aColor.G, aColor.B)
            Return System.Drawing.Color.FromArgb(&HFF, ResultSMC.R, ResultSMC.G,
                                                 ResultSMC.B)
        End With
    End Function ' ContrastingBW

    ''' <summary>
    ''' Selects either black or white for maximum contrast to, for example, a
    ''' background color.
    ''' </summary>
    ''' <param name="aColor">Specifies the background color reference.</param>
    ''' <returns>
    ''' Either black or white as a <c>System.Windows.Media.Color</c>.
    ''' </returns>
    Public Shared Function ContrastingBW(
        ByVal aColor As System.Windows.Media.Color) _
        As System.Windows.Media.Color

        ' No argument check required. Accept any valid System.Windows.Media.Color.
        Return ContrastingBW(aColor.R, aColor.G, aColor.B)
    End Function ' ContrastingBW

#End Region ' "Contrast"

    ''' <summary>
    ''' Returns a hue angle for the specified pixel in an
    ''' <c>Image</c>.
    ''' </summary>
    ''' <param name="pixelX">Specifies the X value of the pixel.</param>
    ''' <param name="pixelY">Specifies the Y value of the pixel.</param>
    ''' <param name="imageWidth">Specifies the width of the image.</param>
    ''' <param name="imageHeight">Specifies the height of the image.</param>
    ''' <param name="scaleTo">Specifies the range to be used, from
    ''' <see cref="OSNW.Graphics.ColorUtilities.HueScaleEnum"/>.</param>
    ''' <returns>Returns a hue angle (0.0 to maximum value).</returns>
    ''' <remarks>The zero angle is at the top-center, with increasing values
    ''' moving clockwise toward, but not reaching, <paramref name="scaleTo"/>
    ''' at the top.</remarks>
    Public Shared Function GetHueFromPixel(ByVal pixelX As System.Int32,
        ByVal pixelY As System.Int32, ByVal imageWidth As System.Int32,
        ByVal imageHeight As System.Int32,
        ByVal scaleTo As OSNW.Graphics.ColorUtilities.HueScaleEnum) _
        As System.Double

        ' Locate the center.
        Dim CenterW As System.Double = (imageWidth - 1.0) / 2.0
        Dim CenterH As System.Double = (imageHeight - 1.0) / 2.0

        ' Calculate offsets from center in normal Cartesian form.
        Dim CartesianX As System.Double = pixelX - CenterW
        Dim CartesianY As System.Double = CenterH - pixelY

        ' Calculate the angle, in radians, relative to the X axis.
        ' Ref: Math.Atan(Double) Method
        ' https://learn.microsoft.com/en-us/dotnet/api/system.math.atan?view=net-9.0
        ' ATAN returns NaN if d equals NaN, -PI/2 rounded to double precision
        ' (-1.5707963267949) if d equals NegativeInfinity, or PI/2 rounded to
        ' double precision (1.5707963267949) if d equals PositiveInfinity.
        Dim TanAlpha As System.Double = CartesianY / CartesianX
        Dim AlphaRad As System.Double = System.Math.Atan(TanAlpha)

        ' Reorient the reference to top-center.
        AlphaRad = If(pixelX < CenterW,
            HALF3PI - AlphaRad,' Quadrant II, III - left.
            HALFPI - AlphaRad) ' Quadrant I, IV - right.

        ' Calculate the fraction of a circle, relative to clockwise rotation
        ' from the top-center.
        Dim AlphaFrac As System.Double = AlphaRad / TWOPI

        Dim ScaleMax As System.Double
        Select Case scaleTo
            Case OSNW.Graphics.ColorUtilities.HueScaleEnum.Radians
                ScaleMax = OSNW.Graphics.ColorUtilities.TWOPI
            Case OSNW.Graphics.ColorUtilities.HueScaleEnum.Fraction
                ScaleMax = 1.0
            Case OSNW.Graphics.ColorUtilities.HueScaleEnum.Scale240 ' Some MS apps.
                ScaleMax = 240.0
                ' That will likely scale to ALMOST 240. Visio HSL only allows
                ' 0-239 for H, but 0-240 for S, and L.
            Case OSNW.Graphics.ColorUtilities.HueScaleEnum.Scale255 ' Some MS apps.
                ScaleMax = 255.0
                ' That will likely scale to ALMOST 255, but Word actually allows
                ' 255 for H, S, and L. Rounding may have to be done to obtain
                ' 256 possible values of H.
            Case OSNW.Graphics.ColorUtilities.HueScaleEnum.Degrees
                ScaleMax = 360.0
            Case Else

                Dim ProcName As System.String =
                    New System.Diagnostics.StackFrame(0).GetMethod().Name
                Dim ErrStr As System.String =
                    $"Invalid 'scaleTo' value in '{ProcName}'."

                Dim NewEx As System.Exception =
                    New System.ApplicationException(ErrStr)

                ' Add arguments from this sub/function to identify
                ' which data set led to the exception.
                With NewEx.Data
                    .Add("pixelX", pixelX)
                    .Add("pixelY", pixelY)
                    .Add("imageWidth", imageWidth)
                    .Add("imageHeight", imageHeight)
                    .Add("scaleTo", scaleTo)
                End With

                Throw NewEx

        End Select

        Return AlphaFrac * ScaleMax

    End Function ' GetHueFromPixel

    ''' <summary>
    ''' Validates a string as being valid for use with
    ''' <see cref="OSNW.Graphics.ColorUtilities.HTMLtoRGB"/>, which also has
    ''' validity requirements for
    ''' <see cref="System.Convert.FromHexString(System.String)"/>.
    ''' </summary>
    ''' <param name="htmlString"></param>
    ''' <param name="requireHashtag"></param>
    ''' <param name="require6Hex"></param>
    ''' <param name="whyNot"></param>
    ''' <returns></returns>
    Public Shared Function IsValidHtmlString(
        ByVal htmlString As System.String,
        Optional ByVal requireHashtag As System.Boolean = False,
        Optional ByVal require6Hex As System.Boolean = False,
        Optional ByRef whyNot As System.String = "Is valid") As System.Boolean

        If htmlString Is Nothing Then
            whyNot = "No string"
            Return False ' Early exit.
        End If

        If requireHashtag AndAlso (Not htmlString(0) = "#"c) Then
            whyNot = "Missing leading hashtag"
            Return False ' Early exit.
        End If

        ' Hashtag was there, but strip it for internal needs.
        Dim HexPart As System.String = If(htmlString(0) = "#"c,
            htmlString.Remove(0, 1), htmlString)

        If Not ((HexPart.Length = 0) OrElse
            (HexPart.Length Mod 2 = 0)) Then

            whyNot = "Not zero or even"
            Return False ' Early exit.
        End If

        If require6Hex Then
            If Not HexPart.Length = 6 Then
                whyNot = "Not 6 hex characters"
                Return False ' Early exit.
            End If
        End If

        ' REF: Regex to check string contains only Hex characters
        ' https://stackoverflow.com/questions/5317320/regex-to-check-string-contains-only-hex-characters
        Dim IsHex As System.Boolean = Regex.IsMatch(HexPart, "[0-9A-Fa-f]+")
        If Not IsHex Then
            whyNot = "Invalid non-hex character"
            Return False ' Early exit.
        End If

        Return True

    End Function ' IsValidHtmlString

End Class ' ColorUtilities

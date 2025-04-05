Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Partial Public Class ColorUtilities

    ' This file contains routines to mix colors in various ways. References to
    ' shade, tint, and tone are as described at
    ' https://www.colorsexplained.com/shade-tint-tone/. The overloads of
    ' GetBlend(<params>) provide shared processes to make the required mixes to
    ' create a shade, tint, or tone.

#Region "Blend"

    ' No specific information was found for blending calculations, but the
    ' results of the implementations created here appear to be a good match for
    ' (50:50) results at
    ' REF: Color Blender Tool - Mix Colors - ColorHexa
    ' https://www.colorhexa.com/color-blend
    ' These are also used internally in the GetShade, GetTint, and GetTone
    ' implementations, which matched to web site results.

    ''' <summary>
    ''' Combines two specified RGB color combinations, in the specified
    ''' proportions. The resulting color is returned in the colorOutX values.
    ''' </summary>
    ''' <param name="color1R">Specifies the red component of the first color to
    ''' be blended.</param>
    ''' <param name="color1G">Specifies the green component of the first color
    ''' to be blended.</param>
    ''' <param name="color1b">Specifies the blue component of the first color to
    ''' be blended.</param>
    ''' <param name="color1Proportion">Specifies the relative amount of
    ''' the first color to be used.</param>
    ''' <param name="color2R">Specifies the red component of the second color to
    ''' be blended.</param>
    ''' <param name="color2G">Specifies the green component of the second color
    ''' to be blended.</param>
    ''' <param name="color2b">Specifies the blue component of the second color
    ''' to be blended.</param>
    ''' <param name="color2Proportion">Specifies the relative amount of
    ''' the second color to be used.</param>
    ''' <param name="colorOutR">Returns the red component of the resulting
    ''' shablended colorde.</param>
    ''' <param name="colorOutG">Returns the green component of the resulting
    ''' blended color.</param>
    ''' <param name="colorOutB">Returns the blue component of the resulting
    ''' blended color.</param>
    ''' <remarks>
    ''' The two specified RGB color combinations are mixed in the specified
    ''' proportions. Any valid proportion values can be used; 0.6 and 0.4 are,
    ''' effectively, the same as 6:4, 60:40, or even 3:2. The proportions do not
    ''' need to add up to 1, 100%, or any other value; for example, 19:13 is
    ''' valid.
    ''' <para>
    ''' <paramref name="color1Proportion"/> and
    ''' <paramref name="color2Proportion"/> should normally both be greater than
    ''' zero, but one zero is allowed and results in the other color being used
    ''' by itself. If both are zero, the default is returned,
    ''' using <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
    ''' <see cref="FORCEDRGBB"/>.
    ''' When one proportion is invalid, the default is returned.
    ''' </para>
    ''' <para>
    ''' The opacity of the result is always fully opaque.
    ''' </para>
    ''' </remarks>
    Public Shared Sub GetBlend(
        ByVal color1R As System.Double, ByVal color1G As System.Double,
        ByVal color1B As System.Double, ByVal color1Proportion As System.Double,
        ByVal color2R As System.Double, ByVal color2G As System.Double,
        ByVal color2B As System.Double, ByVal color2Proportion As System.Double,
        ByRef colorOutR As System.Double, ByRef colorOutG As System.Double,
        ByRef colorOutB As System.Double)

        ' Argument checking.
        ' No check for inputs with Nothing; they become zero.
        If color1R < 0.0 OrElse color1R > 255.0 OrElse
            color1G < 0.0 OrElse color1G > 255.0 OrElse
            color1B < 0.0 OrElse color1B > 255.0 OrElse
            color2R < 0.0 OrElse color2R > 255.0 OrElse
            color2G < 0.0 OrElse color2G > 255.0 OrElse
            color2B < 0.0 OrElse color2B > 255.0 OrElse
            color1Proportion < 0.0 OrElse color2Proportion < 0.0 Then

            ' Argument out of range. Force the result.
            colorOutR = FORCEDRGBR
            colorOutG = FORCEDRGBG
            colorOutB = FORCEDRGBB
            Return ' Early exit.
        End If

        Dim TotalOnce As System.Double =
            color1Proportion + color2Proportion
        If TotalOnce.Equals(0) Then
            ' Avoid division by zero. Force the result.
            colorOutR = FORCEDRGBR
            colorOutG = FORCEDRGBG
            colorOutB = FORCEDRGBB
            Return ' Early exit.
        End If

        ' Make the proportional mix.
        colorOutR = ((color1Proportion * color1R) +
            (color2Proportion * color2R)) / TotalOnce
        colorOutG = ((color1Proportion * color1G) +
            (color2Proportion * color2G)) / TotalOnce
        colorOutB = ((color1Proportion * color1B) +
            (color2Proportion * color2B)) / TotalOnce

    End Sub ' GetBlend

    ''' <summary>
    ''' Combines two specified RGB color combinations, in the specified
    ''' proportions. The resulting color is returned in the colorOutX values.
    ''' For use with RGB as Byte values.
    ''' </summary>
    ''' <param name="color1R">Specifies the red component of the first color
    ''' to be blended.</param>
    ''' <param name="color1G">Specifies the green component of the first color
    ''' to be blended.</param>
    ''' <param name="color1B">Specifies the blue component of the first color
    ''' to be blended.</param>
    ''' <param name="color1Proportion">Specifies the relative weight of the
    ''' first color to be blended.</param>
    ''' <param name="color2R">Specifies the red component of the second color
    ''' to be blended.</param>
    ''' <param name="color2G">Specifies the green component of the second color
    ''' to be blended.</param>
    ''' <param name="color2B">Specifies the blue component of the second color
    ''' to be blended.</param>
    ''' <param name="color2Proportion">Specifies the relative weight of the
    ''' second color to be blended.</param>
    ''' <param name="colorOutR">Returns the red component of the blended
    ''' color.</param>
    ''' <param name="colorOutG">Returns the green component of the blended
    ''' color.</param>
    ''' <param name="colorOutB">Returns the blue component of the blended
    ''' color.</param>
    ''' <remarks>
    ''' The two specified RGB color combinations are mixed in the specified
    ''' proportions. Any valid proportion values can be used; 0.6 and 0.4 are,
    ''' effectively, the same as 6:4, 60:40, or even 3:2. The proportions do not
    ''' need to add up to 1, 100%, or any other value; for example, 19:13 is
    ''' valid.
    ''' <para>
    ''' <paramref name="color1Proportion"/> and
    ''' <paramref name="color2Proportion"/> should normally both be greater than
    ''' zero, but one zero is allowed and results in the other color being used
    ''' by itself. If both are zero, the default is returned,
    ''' using <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
    ''' <see cref="FORCEDRGBB"/>.
    ''' When one proportion is invalid, the default is returned.
    ''' </para>
    ''' <para>
    ''' The opacity of the result is always fully opaque.
    ''' </para>
    ''' </remarks>
    Public Shared Sub GetBlend(
        ByVal color1R As System.Byte, ByVal color1G As System.Byte,
        ByVal color1B As System.Byte, ByVal color1Proportion As System.Double,
        ByVal color2R As System.Byte, ByVal color2G As System.Byte,
        ByVal color2B As System.Byte, ByVal color2Proportion As System.Double,
        ByRef colorOutR As System.Byte, ByRef colorOutG As System.Byte,
        ByRef colorOutB As System.Byte)

        ' Argument checking.
        ' No check for inputs with Nothing; they become zero.
        ' No range check for inputs. Any valid System.Byte is good.
        ' Let the subsequent call handle out-of-range values of color1Proportion
        ' and color1Proportion.

        ' Doubles used for the interim calculation.
        Dim TempR, TempG, TempB As System.Double

        ' Make the proportional mix.
        GetBlend(color1R, color1G, color1B, color1Proportion, color2R, color2G,
                 color2B, color2Proportion, TempR, TempG, TempB)

        ' CByte handles the rounding via "banker's rounding".
        colorOutR = CByte(TempR)
        colorOutG = CByte(TempG)
        colorOutB = CByte(TempB)

    End Sub ' GetBlend

    ''' <summary>
    ''' Returns a color that combines the colors specified by
    ''' <paramref name="color1"/> and <paramref name="color2"/>, in proportions
    ''' specified by <paramref name="color1Proportion"/> and
    ''' <paramref name="color2Proportion"/>.
    ''' </summary>
    ''' <param name="color1">Specifies one color to be combined with the other.
    ''' </param>
    ''' <param name="color1Proportion">Specifies the relative amount of
    ''' <paramref name="color1"/> to be used.</param>
    ''' <param name="color2">Specifies one color to be combined with the other.
    ''' </param>
    ''' <param name="color2Proportion">Specifies the relative amount of
    ''' <paramref name="color2"/> to be used.</param>
    ''' <returns>Returns a color that combines the colors specified by
    ''' <paramref name="color1"/> and <paramref name="color2"/>, based on
    ''' <paramref name="color1Proportion"/> and
    ''' <paramref name="color2Proportion"/>.</returns>
    ''' <remarks>
    ''' The two specified RGB color combinations are mixed in the specified
    ''' proportions. Any valid proportion values can be used; 0.6 and 0.4 are,
    ''' effectively, the same as 6:4, 60:40, or even 3:2. The proportions do not
    ''' need to add up to 1, 100%, or any other value; for example, 19:13 is
    ''' valid.
    ''' <para>
    ''' <paramref name="color1Proportion"/> and
    ''' <paramref name="color2Proportion"/> should normally both be greater than
    ''' zero, but one zero is allowed and results in the other color being used
    ''' by itself. If both are zero, the default is returned,
    ''' using <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
    ''' <see cref="FORCEDRGBB"/>.
    ''' When one proportion is invalid, the default is returned.
    ''' </para>
    ''' <para>
    ''' The opacity of the result is always fully opaque.
    ''' </para>
    ''' </remarks>
    Public Shared Function GetBlend(
        ByVal color1 As System.Windows.Media.Color,
        ByVal color1Proportion As System.Double,
        ByVal color2 As System.Windows.Media.Color,
        ByVal color2Proportion As System.Double) _
        As System.Windows.Media.Color

        ' Argument checking.
        ' No check for color inputs with Nothing; they become black.
        ' No range check for color inputs; any System.Windows.Media.Color is
        ' valid.
        ' No check for proportion inputs with Nothing; they become zero.
        ' Let the subsequent call handle out-of-range values of
        ' color1Proportion and color2Proportion.

        ' Doubles used for the interim calculation.
        Dim TempR As System.Double
        Dim TempG As System.Double
        Dim TempB As System.Double

        ' Make the proportional mix.
        GetBlend(CDbl(color1.R), CDbl(color1.G), CDbl(color1.B),
                 color1Proportion, CDbl(color2.R), CDbl(color2.G),
                 CDbl(color2.B), color2Proportion, TempR, TempG, TempB)

        Dim ColorOut As System.Windows.Media.Color
        With ColorOut
            ' CByte handles the rounding via "banker's rounding".
            .A = FORCEOPACITY255
            .R = CByte(TempR)
            .G = CByte(TempG)
            .B = CByte(TempB)
        End With
        Return ColorOut

    End Function ' GetBlend

#End Region ' "Blend"

#Region "Shade"

    ''' <summary>
    ''' Returns a shade of the color represented by <paramref name="colorInR"/>,
    ''' <paramref name="colorInG"/>, and <paramref name="colorInB"/>, based on
    ''' <paramref name="shadeFactor"/>.
    ''' </summary>
    ''' <param name="colorInR">Specifies the red component of the color to be
    ''' shaded.</param>
    ''' <param name="colorInG">Specifies the green component of the color to be
    ''' shaded.</param>
    ''' <param name="colorInB">Specifies the blue component of the color to be
    ''' shaded.</param>
    ''' <param name="shadeFactor">Specifies the degree (range 0.0 to 1.0) of
    ''' shading to be applied. Low values favor the base color; high values
    ''' favor black.</param>
    ''' <param name="colorOutR">Returns the red component of the resulting
    ''' shade.</param>
    ''' <param name="colorOutG">Returns the green component of the resulting
    ''' shade.</param>
    ''' <param name="colorOutB">Returns the blue component of the resulting
    ''' shade.</param>
    ''' <remarks>
    ''' According to color theory, shades are created by adding black pigment to
    ''' any hue (dominant color family).
    ''' <para>
    ''' Rather than throwing an exception, or returning possibly-wild results,
    ''' for out-of-range input(s) the color blending routines return a
    ''' recognizable set of wrong converted values. The forced results are
    ''' defined by <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
    ''' <see cref="FORCEDRGBB"/>.
    ''' </para>
    ''' The opacity of the result is always fully opaque.
    ''' </remarks>
    Public Shared Sub GetShade(
        ByVal colorInR As System.Double, ByVal colorInG As System.Double,
        ByVal colorInB As System.Double, ByVal shadeFactor As System.Double,
        ByRef colorOutR As System.Double, ByRef colorOutG As System.Double,
        ByRef colorOutB As System.Double)

        ' Ref: Shade, Tint, and Tone in Color Theory
        ' https://www.colorsexplained.com/shade-tint-tone/

        ' Color Theory 101: A Complete Color Guide
        ' https://www.colorsexplained.com/color-theory/

        ' Argument checking.
        ' No check for inputs with Nothing; they become zero.
        If colorInR < 0.0 OrElse colorInR > 255.0 OrElse
            colorInG < 0.0 OrElse colorInG > 255.0 OrElse
            colorInB < 0.0 OrElse colorInB > 255.0 OrElse
            shadeFactor < 0.0 Or shadeFactor > 1.0 Then

            ' Argument out of range. Force the result.
            colorOutR = FORCEDRGBR
            colorOutG = FORCEDRGBG
            colorOutB = FORCEDRGBB
            Return ' Early exit.
        End If

        ' Make the proportional mix.
        GetBlend(colorInR, colorInG, colorInB, 1.0 - shadeFactor, 0.0, 0.0, 0.0,
                 shadeFactor, colorOutR, colorOutG, colorOutB)

    End Sub ' GetShade

    ''' <summary>
    ''' Returns a shade of <paramref name="aColor"/>, based on
    ''' <paramref name="shadeFactor"/>.
    ''' </summary>
    ''' <param name="aColor">Specifies the base color to be shaded.</param>
    ''' <param name="shadeFactor">Specifies the degree (range 0.0 to 1.0) of
    ''' shading to be applied. Low values favor the base color; high values
    ''' favor black.</param>
    ''' <returns>Returns a shade of <paramref name="aColor"/>, based on
    ''' <paramref name="shadeFactor"/>.</returns>
    ''' <remarks>
    ''' According to color theory, shades are created by adding black pigment to
    ''' any hue (dominant color family).
    ''' <para>
    ''' Rather than throwing an exception, or returning possibly-wild results,
    ''' for out-of-range input(s) the color blending routines return a
    ''' recognizable set of wrong converted values. The forced results are
    ''' defined by <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
    ''' <see cref="FORCEDRGBB"/>.
    ''' xxxx
    ''' </para>
    ''' The opacity of the result is always fully opaque.
    ''' </remarks>
    Public Shared Function GetShade(ByVal aColor As System.Windows.Media.Color,
                                    ByVal shadeFactor As System.Double) _
        As System.Windows.Media.Color

        ' Ref: Shade, Tint, and Tone in Color Theory
        ' https://www.colorsexplained.com/shade-tint-tone/

        ' Color Theory 101: A Complete Color Guide
        ' https://www.colorsexplained.com/color-theory/

        ' No argument checking.
        ' Any System.Windows.Media.Color is valid.
        ' Range checking for shadeFactor is handled in the subsequent call.

        ' Make the proportional mix.
        Dim ColorOutR, ColorOutG, ColorOutB As System.Double
        GetShade(aColor.ScR * 255.0, aColor.ScG * 255.0, aColor.ScB * 255.0,
                 shadeFactor, ColorOutR, ColorOutG, ColorOutB)

        Dim ColorOut As System.Windows.Media.Color =
            System.Windows.Media.Color.FromArgb(
                FORCEOPACITY255, CByte(ColorOutR), CByte(ColorOutG),
                CByte(ColorOutB))

        Return ColorOut

    End Function ' GetShade

#End Region ' "Shade"

#Region "Tint"

    ''' <summary>
    ''' Returns a shade of the base color described by
    ''' <paramref name="colorInR"/>, <paramref name="colorInG"/>, and
    ''' <paramref name="colorInB"/>, based on <paramref name="tintFactor"/>.
    ''' </summary>
    ''' <param name="colorInR">Specifies the red component of the color to be
    ''' tinted.</param>
    ''' <param name="colorInG">Specifies the green component of the color to be
    ''' tinted.</param>
    ''' <param name="colorInB">Specifies the blue component of the color to be
    ''' tinted.</param>
    ''' <param name="tintFactor">Specifies the degree (range 0.0 to 1.0) of
    ''' tinting to be applied. Low values favor the base color; high values
    ''' favor white.
    ''' </param>
    ''' <param name="colorOutR">Returns the red component of the resulting
    ''' tint.</param>
    ''' <param name="colorOutG">Returns the green component of the resulting
    ''' tint.</param>
    ''' <param name="colorOutB">Returns the blue component of the resulting
    ''' tint.</param>
    ''' <remarks>
    ''' Tints are created by adding white to any hue, according to color theory.
    ''' This lightens and desaturates the hue, creating a subtler and lighter
    ''' color than the original hue.
    ''' <para>
    ''' Rather than throwing an exception, or returning possibly-wild results,
    ''' for out-of-range input(s) the color blending routines return a
    ''' recognizable set of wrong converted values. The forced results are
    ''' defined by <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
    ''' <see cref="FORCEDRGBB"/>.
    ''' </para>
    ''' The opacity of the result is always fully opaque.
    ''' </remarks>
    Public Shared Sub GetTint(
        ByVal colorInR As System.Double, ByVal colorInG As System.Double,
        ByVal colorInB As System.Double, ByVal tintFactor As System.Double,
        ByRef colorOutR As System.Double, ByRef colorOutG As System.Double,
        ByRef colorOutB As System.Double)

        ' Ref: Shade, Tint, and Tone in Color Theory
        ' https://www.colorsexplained.com/shade-tint-tone/

        ' Color Theory 101: A Complete Color Guide
        ' https://www.colorsexplained.com/color-theory/

        ' Argument checking.
        ' No check for inputs with Nothing; they become zero.
        If colorInR < 0.0 OrElse colorInR > 255.0 OrElse
            colorInG < 0.0 OrElse colorInG > 255.0 OrElse
            colorInB < 0.0 OrElse colorInB > 255.0 OrElse
            tintFactor < 0.0 Or tintFactor > 1.0 Then

            ' Argument out of range. Force the result.
            colorOutR = FORCEDRGBR
            colorOutG = FORCEDRGBG
            colorOutB = FORCEDRGBB
            Return ' Early exit.
        End If

        ' Make the proportional mix.
        GetBlend(colorInR, colorInG, colorInB, 1.0 - tintFactor, 255.0, 255.0,
                 255.0, tintFactor, colorOutR, colorOutG, colorOutB)

    End Sub ' GetTint

    ''' <summary>
    ''' Returns a tint of <paramref name="aColor"/>, based on
    ''' <paramref name="tintFactor"/>.
    ''' </summary>
    ''' <param name="aColor">Specifies the base color to be tinted.</param>
    ''' <param name="tintFactor">Specifies the degree (range 0.0 to 1.0) of
    ''' tinting to be applied. Low values favor the base color; high values
    ''' favor white.</param>
    ''' <returns>Returns a tint of <paramref name="aColor"/>, based on
    ''' <paramref name="tintFactor"/>.</returns>
    ''' <remarks>
    ''' Tints are created by adding white to any hue, according to color theory.
    ''' This lightens and desaturates the hue, creating a subtler and lighter
    ''' color than the original hue.
    ''' <para>
    ''' Rather than throwing an exception, or returning possibly-wild results,
    ''' for out-of-range input(s) the color blending routines return a
    ''' recognizable set of wrong converted values. The forced results are
    ''' defined by <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
    ''' <see cref="FORCEDRGBB"/>.
    ''' </para>
    ''' The opacity of the result is always fully opaque.
    ''' </remarks>
    Public Shared Function GetTint(ByVal aColor As System.Windows.Media.Color,
                                   ByVal tintFactor As System.Double) _
        As System.Windows.Media.Color

        ' Ref: Shade, Tint, and Tone in Color Theory
        ' https://www.colorsexplained.com/shade-tint-tone/

        ' Color Theory 101: A Complete Color Guide
        ' https://www.colorsexplained.com/color-theory/

        ' No argument checking.
        ' Any System.Windows.Media.Color is valid.
        ' Range checking for tintFactor is handled in the subsequent call.

        ' Make the proportional mix.
        Dim ColorOutR, ColorOutG, ColorOutB As System.Double
        GetTint(aColor.ScR * 255.0, aColor.ScG * 255.0, aColor.ScB * 255.0,
                tintFactor, ColorOutR, ColorOutG, ColorOutB)

        Dim ColorOut As System.Windows.Media.Color =
            System.Windows.Media.Color.FromArgb(
                FORCEOPACITY255, CByte(ColorOutR), CByte(ColorOutG),
                CByte(ColorOutB))
        Return ColorOut

    End Function ' GetTint

#End Region ' "Tint"

#Region "Tone"

    ''' <summary>
    ''' Returns a tone of the color represented by <paramref name="colorInR"/>,
    ''' <paramref name="colorInG"/>, and <paramref name="colorInB"/>, based on
    ''' <paramref name="grayLevel"/> and <paramref name="toneFactor"/>.
    ''' </summary>
    ''' <param name="colorInR">Specifies the red component of the color to be
    ''' toned.</param>
    ''' <param name="colorInG">Specifies the green component of the color to be
    ''' toned.</param>
    ''' <param name="colorInB">Specifies the blue component of the color to be
    ''' toned.</param>
    ''' <param name="grayLevel">Specifies the gray-scale (range 0.0 to 1.0)
    ''' value to be used to create the tone.</param>
    ''' <param name="toneFactor">Specifies the degree (range 0.0 to 1.0) of
    ''' toning to be applied. Low values favor the base color; high values
    ''' favor gray.
    ''' </param>
    ''' <param name="colorOutR">Returns the red component of the resulting
    ''' tone.</param>
    ''' <param name="colorOutG">Returns the green component of the resulting
    ''' tone.</param>
    ''' <param name="colorOutB">Returns the blue component of the resulting
    ''' tone.</param>
    ''' <remarks>
    ''' Tones are created by adding gray to any hue. The tone created depends on
    ''' the amount of black and white used in the gray and the amount of gray
    ''' added (keep in mind there are a lot of shades of gray).
    ''' <para>
    ''' Rather than throwing an exception, or returning possibly-wild results,
    ''' for out-of-range input(s) the color blending routines return a
    ''' recognizable set of wrong converted values. The forced results are
    ''' defined by <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
    ''' <see cref="FORCEDRGBB"/>.
    ''' </para>
    ''' The opacity of the result is always fully opaque.
    ''' </remarks>
    Public Shared Sub GetTone(
        ByVal colorInR As System.Double, ByVal colorInG As System.Double,
        ByVal colorInB As System.Double, ByVal grayLevel As System.Double,
        ByVal toneFactor As System.Double, ByRef colorOutR As System.Double,
        ByRef colorOutG As System.Double, ByRef colorOutB As System.Double)

        ' Ref: Shade, Tone, and Tone in Color Theory
        ' https://www.colorsexplained.com/shade-tone-tone/

        ' Color Theory 101: A Complete Color Guide
        ' https://www.colorsexplained.com/color-theory/

        ' Argument checking.
        ' No check for inputs with Nothing; they become zero.
        If colorInR < 0.0 OrElse colorInR > 255.0 OrElse
            colorInG < 0.0 OrElse colorInG > 255.0 OrElse
            colorInB < 0.0 OrElse colorInB > 255.0 OrElse
            grayLevel < 0.0 OrElse grayLevel > 255.0 OrElse
            toneFactor < 0.0 Or toneFactor > 1.0 Then

            ' Argument out of range. Force the result.
            colorOutR = FORCEDRGBR
            colorOutG = FORCEDRGBG
            colorOutB = FORCEDRGBB
            Return ' Early exit.
        End If

        ' Make the proportional mix.
        ' Yes, three grayLevel.
        GetBlend(colorInR, colorInG, colorInB, 1.0 - toneFactor, grayLevel,
                 grayLevel, grayLevel, toneFactor, colorOutR, colorOutG,
                 colorOutB)

    End Sub ' GetTone

    ''' <summary>
    ''' Returns a tone of <paramref name="aColor"/>, based on
    ''' <paramref name="grayLevel"/> and <paramref name="toneFactor"/>.
    ''' </summary>
    ''' <param name="aColor">Specifies the base color to be toned.</param>
    ''' <param name="grayLevel">Specifies the gray-scale (range 0.0 to 1.0)
    ''' value to be used to create the tone.</param>
    ''' <param name="toneFactor">Specifies the degree (range 0.0 to 1.0) of
    ''' toning to be applied. Low values favor the base color; high values
    ''' favor gray.</param>
    ''' <returns>Returns a tone of <paramref name="aColor"/>, based on
    ''' <paramref name="grayLevel"/> and <paramref name="toneFactor"/>.
    ''' </returns>
    ''' <remarks>
    ''' Tones are created by adding gray to any hue. The tone created depends on
    ''' the amount of black and white used in the gray and the amount of gray
    ''' added (keep in mind there are a lot of shades of gray).
    ''' <para>
    ''' Rather than throwing an exception, or returning possibly-wild results,
    ''' for out-of-range input(s) the color blending routines return a
    ''' recognizable set of wrong converted values. The forced results are
    ''' defined by <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
    ''' <see cref="FORCEDRGBB"/>.
    ''' </para>
    ''' The opacity of the result is always fully opaque.
    ''' </remarks>
    Public Shared Function GetTone(ByVal aColor As System.Windows.Media.Color,
                                   ByVal grayLevel As System.Double,
                                   ByVal toneFactor As System.Double) _
        As System.Windows.Media.Color

        ' Ref: Shade, Tone, and Tone in Color Theory
        ' https://www.colorsexplained.com/shade-tone-tone/

        ' Color Theory 101: A Complete Color Guide
        ' https://www.colorsexplained.com/color-theory/

        ' No argument checking.
        ' Any System.Windows.Media.Color is valid.
        ' Range checking for grayLevel and toneFactor is handled in the
        ' subsequent call.

        ' Make the proportional mix.
        Dim ColorOutR, ColorOutG, ColorOutB As System.Double
        GetTone(aColor.ScR * 255.0, aColor.ScG * 255.0, aColor.ScB * 255.0,
                grayLevel, toneFactor, ColorOutR, ColorOutG, ColorOutB)

        Dim ColorOut As System.Windows.Media.Color =
            System.Windows.Media.Color.FromArgb(
                FORCEOPACITY255, CByte(ColorOutR), CByte(ColorOutG),
                CByte(ColorOutB))
        Return ColorOut

    End Function ' GetTone

#End Region ' "Tone"

End Class ' ColorUtilities

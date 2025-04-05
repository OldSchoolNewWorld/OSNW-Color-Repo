Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

' This file contains implementations of conversions RGB to/from other color spaces.

Partial Public Class ColorUtilities

    ' This section contains implementations of routines related to HTML color
    ' descriptions.

    ''' <summary>
    ''' Converts RGB (0 - 255) color components to a HTML (#RRGGBB) string.
    ''' </summary>
    ''' <param name = "redIn" >
    ''' Represents the Red (0 - 255) input component.
    ''' </param>
    ''' <param name = "greenIn" >
    ''' Represents the Green (0 - 255) input component.
    ''' </param>
    ''' <param name = "blueIn" >
    ''' Represents the Blue (0 - 255) input component.
    ''' </param>
    ''' <param name = "htmlOut" >
    ''' Returns the HTML (#RRGGBB) string.
    ''' </param>
    Public Shared Sub RGBtoHTML(
        ByVal redIn As System.Byte, ByVal greenIn As System.Byte,
        ByVal blueIn As System.Byte, ByRef htmlOut As System.String)

        Dim ByteArray As System.Byte() = {redIn, greenIn, blueIn}
        htmlOut = $"#{System.Convert.ToHexString(ByteArray)}"
    End Sub ' RGBtoHTML

    ''' <summary>
    ''' Converts a HTML (#RRGGBB) string to RGB (0 - 255) color components.
    ''' </summary>
    ''' <param name="htmlIn">
    ''' Represents the HTML (#RRGGBB) string.
    ''' </param>
    ''' <param name="redOut">
    ''' Represents the Red (0 - 255) output component.
    ''' </param>
    ''' <param name="greenOut">
    ''' Represents the Green (0 - 255) output component.
    ''' </param>
    ''' <param name="blueOut">
    ''' Represents the Blue (0 - 255) output component.
    ''' </param>
    ''' <exception cref="System.ArgumentNullException"><paramref name="htmlIn"/>
    ''' is <c>Nothing/Null</c>.</exception>
    ''' <exception cref="System.FormatException">The length of
    ''' <paramref name="htmlIn"/>, excluding the optional hashtag, is not zero
    ''' or a multiple of 2.</exception>
    ''' <exception cref="System.FormatException">The format of
    ''' <paramref name="htmlIn"/> is invalid. <paramref name="htmlIn"/> contains
    ''' a non-hex character.</exception>
    ''' <remarks>
    ''' Exactly one, optional, leading hashtag ('#') is allowed in
    ''' <paramref name="htmlIn"/>. If <paramref name="htmlIn"/> contains only
    ''' the hashtag, black component values are returned. There must be an even
    ''' number of hexadecimal digits, after the optional hashtag.
    ''' </remarks>
    Public Shared Sub HTMLtoRGB(ByVal htmlIn As System.String,
        ByRef redOut As System.Byte, ByRef greenOut As System.Byte,
        ByRef blueOut As System.Byte)

        ' Argument checking.
        Dim HexPart As System.String
        If htmlIn Is Nothing Then
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            Dim NewEx As New System.ArgumentNullException(htmlIn,
                $"{NameOf(htmlIn)} is Nothing/Null in {CaughtBy}.")
            Throw NewEx
        Else
            ' Not nothing. Allow for one leading hashtag.
            If htmlIn(0) = "#"c Then
                ' Leading hashtag found.
                If htmlIn.Length = 0 Then
                    ' Nothing after the hashtag. Return black components.
                    redOut = 0
                    greenOut = 0
                    blueOut = 0
                    Return ' Early exit.
                Else
                    ' Strip the hashtag.
                    HexPart = htmlIn.Remove(0, 1)
                End If
            Else
                ' No hashtag.
                HexPart = htmlIn
            End If
        End If
        If HexPart.Length > 6 Then
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            Dim NewEx As New System.FormatException(
                "Length (excluding '#') &gt; 6 for " &
                    $"{NameOf(htmlIn)} in {CaughtBy}.")
            Throw NewEx
        End If

        ' Convert the hex portion.
        Dim AllBytes As System.Byte() = System.Convert.FromHexString(HexPart)
        redOut = If(AllBytes.Length > 2, AllBytes(AllBytes.Length - 3), CByte(0))
        greenOut = If(AllBytes.Length > 1, AllBytes(AllBytes.Length - 2), CByte(0))
        blueOut = If(AllBytes.Length > 0, AllBytes(AllBytes.Length - 1), CByte(0))

    End Sub ' HTMLtoRGB

End Class ' ColorUtilities

Partial Public Class ColorUtilities

    ' This file contains (mostly) direct Visual Basic implementations of
    ' routines obtained from the EasyRGB web site.
    ' The referenced routines are now gathered at
    ' http://www.EasyRGB.com/en/math.php.

    '''' <summary>
    '''' Converts RGB (0.0 - 255.0) color components to HSL (0.0 - 1.0) color
    '''' components.
    '''' </summary>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='RgbIn']/*"/>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='HslOut']/*"/>
    '''' <remarks>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='ColorForceEasy']/*"/>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='WarnReverse']/*"/>
    '''' </remarks>
    ''' <summary>
    ''' Converts RGB (0.0 - 255.0) color components to HSL (0.0 - 1.0) color
    ''' components.
    ''' </summary>
    ''' <param name = "redIn" >
    ''' Represents the Red (0.0 - 255.0) input component.
    ''' </param>
    ''' <param name = "greenIn" >
    ''' Represents the Green (0.0 - 255.0) input component.
    ''' </param>
    ''' <param name = "blueIn" >
    ''' Represents the Blue (0.0 - 255.0) input component.
    ''' </param>
    ''' <param name = "hueOut" >
    ''' Returns the Hue (0.0 - 1.0) output component.
    ''' </param>
    ''' <param name = "saturationOut" >
    ''' Returns the Saturation (0.0 - 1.0) output component.
    ''' </param>
    ''' <param name = "luminanceOut" >
    ''' Returns the Luminance (0.0 - 1.0) output component.
    ''' </param>
    ''' <remarks>
    ''' <para>
    ''' Rather than throwing an exception, or returning possibly-wild results,
    ''' for out-of-range input(s) the EasyXXXtoYYY converters return a
    ''' recognizable set of wrong converted values. The forced results are
    ''' defined by
    ''' <see cref="FORCEDHSLH"/>, <see cref="FORCEDHSLS"/>,
    ''' <see cref="FORCEDHSLL"/>, <see cref="FORCEDHSVH"/>,
    ''' <see cref="FORCEDHSVS"/>, <see cref="FORCEDHSVV"/>,
    ''' <see cref="FORCEDRGBR"/>, <see cref="FORCEDRGBG"/>, and
    ''' <see cref="FORCEDRGBB"/>.
    ''' </para>
    ''' <para>
    ''' Red, green, and blue values on displays and printers are represented as
    ''' byte values; this conversion uses <c>Double</c>s. Therefore,
    ''' XXX =&gt; YYY =&gt; XXX may not result in exact before/after matches.
    ''' </para>
    ''' </remarks>
    Public Shared Sub RGBtoHSL(ByVal redIn As System.Double,
        ByVal greenIn As System.Double, ByVal blueIn As System.Double,
        ByRef hueOut As System.Double, ByRef saturationOut As System.Double,
        ByRef luminanceOut As System.Double)

        ' RGB to HSL
        '
        ' ' R, G and B input range = 0 ÷ 255
        ' ' H, S and L output range = 0 ÷ 1.0
        ' 
        ' FracR = ( R / 255 )
        ' FracG = ( G / 255 )
        ' FracB = ( B / 255 )
        ' 
        ' MinRGB = min( FracR, FracG, FracB )    //Min. value of RGB
        ' MaxRGB = max( FracR, FracG, FracB )    //Max. value of RGB
        ' DiffMaxMin = MaxRGB - MinRGB             //Delta RGB value
        ' 
        ' L = ( MaxRGB + MinRGB )/ 2
        ' 
        ' if ( DiffMaxMin == 0 )                     //This is a gray, no chroma...
        ' {
        '     H = 0
        '     S = 0
        ' }
        ' else                                    //Chromatic data...
        ' {
        '    if ( L < 0.5 ) S = DiffMaxMin / ( MaxRGB + MinRGB )
        '    else           S = DiffMaxMin / ( 2 - MaxRGB - MinRGB )
        ' 
        '    DiffR = ( ( ( MaxRGB - FracR ) / 6 ) + ( DiffMaxMin / 2 ) ) / DiffMaxMin
        '    DiffG = ( ( ( MaxRGB - FracG ) / 6 ) + ( DiffMaxMin / 2 ) ) / DiffMaxMin
        '    DiffB = ( ( ( MaxRGB - FracB ) / 6 ) + ( DiffMaxMin / 2 ) ) / DiffMaxMin
        ' 
        '    if      ( FracR == MaxRGB ) H = DiffB - DiffG
        '    else if ( FracG == MaxRGB ) H = ( 1 / 3 ) + DiffR - DiffB
        '    else if ( FracB == MaxRGB ) H = ( 2 / 3 ) + DiffG - DiffR
        ' 
        '     if ( H < 0 ) H += 1
        '     if ( H > 1 ) H -= 1
        ' }

        ' Argument checking.
        ' No check for arguments with Nothing. They become zero.
        If redIn < 0.0 OrElse redIn > 255.0 OrElse greenIn < 0.0 OrElse
            greenIn > 255.0 OrElse blueIn < 0.0 OrElse blueIn > 255.0 Then

            ' Argument out of range. Force the result.
            hueOut = FORCEDHSLH
            saturationOut = FORCEDHSLS
            luminanceOut = FORCEDHSLL
            Return ' Early exit.
        End If

        Dim FracR As System.Double = redIn / 255.0
        Dim FracG As System.Double = greenIn / 255.0
        Dim FracB As System.Double = blueIn / 255.0

        ' Min. value of RGB.
        Dim MinRGB As System.Double =
            System.Double.Min(FracR, System.Double.Min(FracG, FracB))
        ' Max. value of RGB.
        Dim MaxRGB As System.Double =
            System.Double.Max(FracR, System.Double.Max(FracG, FracB))
        ' Delta RGB value.
        Dim DiffMaxMin As System.Double = MaxRGB - MinRGB

        luminanceOut = (MaxRGB + MinRGB) / 2.0

        If DiffMaxMin.Equals(0.0) Then
            ' Gray case - R=G=B.
            hueOut = 0.0
            saturationOut = 0.0
        Else
            ' Non-gray case.

            saturationOut = If(luminanceOut < 0.5,
                DiffMaxMin / (MaxRGB + MinRGB),
                DiffMaxMin / (2.0 - MaxRGB - MinRGB))

            Dim HalfDiffMaxOnce As System.Double = DiffMaxMin / 2.0
            Dim DiffR As System.Double =
                (((MaxRGB - FracR) / 6.0) + HalfDiffMaxOnce) / DiffMaxMin
            Dim DiffG As System.Double =
                (((MaxRGB - FracG) / 6.0) + HalfDiffMaxOnce) / DiffMaxMin
            Dim DiffB As System.Double =
                (((MaxRGB - FracB) / 6.0) + HalfDiffMaxOnce) / DiffMaxMin

            If FracR.Equals(MaxRGB) Then
                hueOut = DiffB - DiffG
            ElseIf FracG.Equals(MaxRGB) Then
                hueOut = ONETHIRD + DiffR - DiffB
            ElseIf FracB.Equals(MaxRGB) Then
                hueOut = TWOTHIRDS + DiffG - DiffR
            End If

            If hueOut < 0.0 Then
                hueOut += 1.0
            ElseIf hueOut > 1.0 Then
                hueOut -= 1.0
            End If

        End If

    End Sub ' RGBtoHSL

    '''' <summary>
    '''' Converts HSL (0.0 - 1.0) color components to RGB (0.0 - 255.0) color
    '''' components.
    '''' </summary>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='HslIn']/*"/>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='RgbOut']/*"/>
    '''' <remarks>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='ColorForceEasy']/*"/>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='WarnReverse']/*"/>
    '''' </remarks>
    ''' <summary>
    ''' Converts HSL (0.0 - 1.0) color components to RGB (0.0 - 255.0) color
    ''' components.
    ''' </summary>
    ''' <param name = "huein" >
    ''' Represents the Hue (0.0 - 1.0) input component.
    ''' </param>
    ''' <param name = "saturationin" >
    ''' Represents the Saturation (0.0 - 1.0) input component.
    ''' </param>
    ''' <param name = "luminancein" >
    ''' Represents the Luminance (0.0 - 1.0) input component.
    ''' </param>
    ''' <param name = "redout" >
    ''' Returns the Red (0.0 - 255.0) output component.
    ''' </param>
    ''' <param name = "greenout" >
    ''' Returns the Green (0.0 - 255.0) output component.
    ''' </param>
    ''' <param name = "blueout" >
    ''' Returns the Blue (0.0 - 255.0) output component.
    ''' </param>
    Public Shared Sub HSLtoRGB(ByVal hueIn As System.Double,
        ByVal saturationIn As System.Double, ByVal luminanceIn As System.Double,
        ByRef redOut As System.Double, ByRef greenOut As System.Double,
        ByRef blueOut As System.Double)

        ' HSL to RGB
        '
        ' //H, S and L input range = 0 ÷ 1.0
        ' //R, G and B output range = 0 ÷ 255
        ' 
        ' if ( S == 0 )
        ' {
        ' 
        '    R = L * 255
        '    G = L * 255
        '    B = L * 255
        ' }
        ' else
        ' {
        '    if ( L < 0.5 ) Var_2 = L * ( 1 + S )
        '    else           Var_2 = ( L + S ) - ( S * L )
        ' 
        '    Var_1 = 2 * L - Var_2
        ' 
        '    R = 255 * Hue_2_RGB( Var_1, Var_2, H + ( 1 / 3 ) )
        '    G = 255 * Hue_2_RGB( Var_1, Var_2, H )
        '    B = 255 * Hue_2_RGB( Var_1, Var_2, H - ( 1 / 3 ) )
        ' }
        ' 
        ' Hue_2_RGB( v1, v2, vH )             //Function Hue_2_RGB
        ' {
        '    if ( vH < 0 ) vH += 1
        '    if( vH > 1 ) vH -= 1
        '    if ( ( 6 * vH ) < 1 ) return ( v1 + ( v2 - v1 ) * 6 * vH )
        '    if ( ( 2 * vH ) < 1 ) return ( v2 )
        '    if ( ( 3 * vH ) < 2 ) return
        '        ( v1 + ( v2 - v1 ) * ( ( 2 / 3 ) - vH ) * 6 )
        '    return ( v1 )
        ' }

        ' Argument checking.
        ' No check for arguments with Nothing. They become zero.
        If hueIn < 0.0 OrElse hueIn > 1.0 OrElse saturationIn < 0.0 OrElse
            saturationIn > 1.0 OrElse luminanceIn < 0.0 OrElse
            luminanceIn > 1.0 Then

            ' Argument out of range. Force the result.
            redOut = FORCEDRGBR
            greenOut = FORCEDRGBG
            blueOut = FORCEDRGBB
            Return ' Early exit.
        End If

        If saturationIn.Equals(0.0) Then
            ' Gray case - R=G=B.
            Dim GrayOnce As System.Double = luminanceIn * 255.0
            redOut = GrayOnce
            greenOut = GrayOnce
            blueOut = GrayOnce
        Else
            ' Non-gray case.

            Dim Var_2 As System.Double = If(luminanceIn < 0.5,
                luminanceIn * (1.0 + saturationIn),
                luminanceIn + saturationIn - (saturationIn * luminanceIn))
            Dim Var_1 As System.Double = 2.0 * luminanceIn - Var_2

            redOut = 255.0 * Hue_2_RGB(Var_1, Var_2, hueIn + ONETHIRD)
            greenOut = 255.0 * Hue_2_RGB(Var_1, Var_2, hueIn)
            blueOut = 255.0 * Hue_2_RGB(Var_1, Var_2, hueIn - ONETHIRD)

        End If

    End Sub ' HSLtoRGB

    Private Shared Function Hue_2_RGB(ByVal v1 As System.Double,
        ByVal v2 As System.Double, ByVal vH As System.Double) As System.Double

        ' Hue_2_RGB( v1, v2, vH )             //Function Hue_2_RGB
        ' {
        '    if ( vH < 0 ) vH += 1
        '    if( vH > 1 ) vH -= 1
        '    if ( ( 6 * vH ) < 1 ) return ( v1 + ( v2 - v1 ) * 6 * vH )
        '    if ( ( 2 * vH ) < 1 ) return ( v2 )
        '    if ( ( 3 * vH ) < 2 ) return
        '        ( v1 + ( v2 - v1 ) * ( ( 2 / 3 ) - vH ) * 6 )
        '    return ( v1 )
        ' }

        If vH < 0.0 Then
            vH += 1.0
        ElseIf vH > 1.0 Then
            vH -= 1.0
        End If
        If (6.0 * vH) < 1.0 Then Return v1 + (v2 - v1) * 6.0 * vH
        If (2.0 * vH) < 1.0 Then Return v2
        If (3.0 * vH) < 2.0 Then Return v1 + (v2 - v1) * (TWOTHIRDS - vH) * 6.0
        Return v1

    End Function ' Hue_2_RGB

    '''' <summary>
    '''' Converts RGB (0.0 - 255.0) color components to HSV (0.0 - 1.0) color
    '''' components.
    '''' </summary>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='RgbIn']/*"/>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='HsvOut']/*"/>
    '''' <remarks>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='ColorForceEasy']/*"/>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='WarnReverse']/*"/>
    '''' </remarks>
    ''' <summary>
    ''' Converts RGB (0.0 - 255.0) color components to HSV (0.0 - 1.0) color
    ''' components.
    ''' </summary>
    ''' <param name = "redIn" >
    ''' Represents the Red (0.0 - 255.0) input component.
    ''' </param>
    ''' <param name = "greenIn" >
    ''' Represents the Green (0.0 - 255.0) input component.
    ''' </param>
    ''' <param name = "blueIn" >
    ''' Represents the Blue (0.0 - 255.0) input component.
    ''' </param>
    ''' <param name = "hueOut" >
    ''' Returns the Hue (0.0 - 1.0) output component.
    ''' </param>
    ''' <param name = "saturationOut" >
    ''' Returns the Saturation (0.0 - 1.0) output component.
    ''' </param>
    ''' <param name = "valueOut" >
    ''' Returns the valueOut (0.0 - 1.0) output component.
    ''' </param>
    Public Shared Sub RGBtoHSV(ByVal redIn As System.Double,
        ByVal greenIn As System.Double, ByVal blueIn As System.Double,
        ByRef hueOut As System.Double, ByRef saturationOut As System.Double,
        ByRef valueOut As System.Double)

        ' RGB to HSV
        '
        ' //R, G and B input range = 0 ÷ 255
        ' //H, S and V output range = 0 ÷ 1.0
        ' 
        ' FracR = ( R / 255 )
        ' FracG = ( G / 255 )
        ' FracB = ( B / 255 )
        ' 
        ' MinRGB = min( FracR, FracG, FracB )    //Min. value of RGB
        ' MaxRGB = max( FracR, FracG, FracB )    //Max. value of RGB
        ' DiffMaxMin = MaxRGB - MinRGB             //Delta RGB value
        ' 
        ' V = MaxRGB
        ' 
        ' if ( DiffMaxMin == 0 )                     //This is a gray, no chroma...
        ' {
        '     H = 0
        '     S = 0
        ' }
        ' else                                    //Chromatic data...
        ' {
        '    S = DiffMaxMin / MaxRGB
        ' 
        '    DiffR = ( ( ( MaxRGB - FracR ) / 6 ) + ( DiffMaxMin / 2 ) ) / DiffMaxMin
        '    DiffG = ( ( ( MaxRGB - FracG ) / 6 ) + ( DiffMaxMin / 2 ) ) / DiffMaxMin
        '    DiffB = ( ( ( MaxRGB - FracB ) / 6 ) + ( DiffMaxMin / 2 ) ) / DiffMaxMin
        ' 
        '    if      ( FracR == MaxRGB ) H = DiffB - DiffG
        '    else if ( FracG == MaxRGB ) H = ( 1 / 3 ) + DiffR - DiffB
        '    else if ( FracB == MaxRGB ) H = ( 2 / 3 ) + DiffG - DiffR
        ' 
        '     if ( H < 0 ) H += 1
        '     if ( H > 1 ) H -= 1
        ' }

        ' Argument checking.
        ' No check for arguments with Nothing. They become zero.
        If redIn < 0.0 OrElse redIn > 255.0 OrElse greenIn < 0.0 OrElse
            greenIn > 255.0 OrElse blueIn < 0.0 OrElse blueIn > 255.0 Then

            ' Argument out of range. Force the result.
            hueOut = FORCEDHSVH
            saturationOut = FORCEDHSVS
            valueOut = FORCEDHSVV
            Return ' Early exit.
        End If

        Dim FracR As System.Double = redIn / 255.0
        Dim FracG As System.Double = greenIn / 255.0
        Dim FracB As System.Double = blueIn / 255.0

        ' Min.value of RGB.
        Dim MinRGB As System.Double =
            System.Double.Min(FracR, System.Double.Min(FracG, FracB))
        ' Max.value of RGB.
        Dim MaxRGB As System.Double =
            System.Double.Max(FracR, System.Double.Max(FracG, FracB))
        ' Delta RGB value.
        Dim DiffMaxMin As System.Double = MaxRGB - MinRGB

        valueOut = MaxRGB

        If DiffMaxMin.Equals(0.0) Then
            ' Gray case - R=G=B.
            hueOut = 0.0
            saturationOut = 0.0
        Else
            ' Non-gray case.

            saturationOut = DiffMaxMin / MaxRGB

            Dim HalfDelMaxOnce As System.Double = (DiffMaxMin / 2.0)
            Dim DiffR As System.Double =
                (((MaxRGB - FracR) / 6.0) + HalfDelMaxOnce) / DiffMaxMin
            Dim DiffG As System.Double =
                (((MaxRGB - FracG) / 6.0) + HalfDelMaxOnce) / DiffMaxMin
            Dim DiffB As System.Double =
                (((MaxRGB - FracB) / 6.0) + HalfDelMaxOnce) / DiffMaxMin

            If FracR.Equals(MaxRGB) Then
                hueOut = DiffB - DiffG
            ElseIf FracG.Equals(MaxRGB) Then
                hueOut = ONETHIRD + DiffR - DiffB
            ElseIf FracB.Equals(MaxRGB) Then
                hueOut = TWOTHIRDS + DiffG - DiffR
            End If

            If hueOut < 0.0 Then
                hueOut += 1.0
            ElseIf hueOut > 1.0 Then
                hueOut -= 1.0
            End If

        End If

    End Sub ' RGBtoHSV

    '''' <summary>
    '''' Converts HSV (0.0 - 1.0) color components to RGB (0.0 - 255.0) color
    '''' components.
    '''' </summary>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='HsvIn']/*"/>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='RgbOut']/*"/>
    '''' <remarks>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='ColorForceEasy']/*"/>
    '''' <include file="CommentFile.xml"
    '''' path="Docs/Members[@name='WarnReverse']/*"/>
    '''' </remarks>
    ''' <summary>
    ''' Converts HSV (0.0 - 1.0) color components to RGB (0.0 - 255.0) color
    ''' components.
    ''' </summary>
    ''' <param name = "huein" >
    ''' Represents the Hue (0.0 - 1.0) input component.
    ''' </param>
    ''' <param name = "saturationin" >
    ''' Represents the Saturation (0.0 - 1.0) input component.
    ''' </param>
    ''' <param name = "valueIn" >
    ''' Represents the Value (0.0 - 1.0) input component.
    ''' </param>
    ''' <param name = "redout" >
    ''' Returns the Red (0.0 - 255.0) output component.
    ''' </param>
    ''' <param name = "greenout" >
    ''' Returns the Green (0.0 - 255.0) output component.
    ''' </param>
    ''' <param name = "blueout" >
    ''' Returns the Blue (0.0 - 255.0) output component.
    ''' </param>
    Public Shared Sub HSVtoRGB(ByVal hueIn As System.Double,
        ByVal saturationIn As System.Double, ByVal valueIn As System.Double,
        ByRef redOut As System.Double, ByRef greenOut As System.Double,
        ByRef blueOut As System.Double)

        ' HSV to RGB
        '
        ' //H, S and V input range = 0 ÷ 1.0
        ' //R, G and B output range = 0 ÷ 255
        ' 
        ' if ( S == 0 )
        ' {
        '    R = V * 255
        '    G = V * 255
        '    B = V * 255
        ' }
        ' else
        ' {
        '    VarHue6 = H * 6
        '    if ( VarHue6 == 6 ) VarHue6 = 0      //H must be < 1
        '    Var_I = int( VarHue6 )             //Or ... Var_I = floor( VarHue6 )
        '    Var_1 = V * ( 1 - S )
        '    Var_2 = V * ( 1 - S * ( VarHue6 - Var_I ) )
        '    Var_3 = V * ( 1 - S * ( 1 - ( VarHue6 - Var_I ) ) )
        ' 
        '    if      ( Var_I == 0 )
        '        { var_r = V     : var_g = Var_3 : var_b = Var_1 }
        '    else if ( Var_I == 1 )
        '        { var_r = Var_2 : var_g = V     : var_b = Var_1 }
        '    else if ( Var_I == 2 )
        '        { var_r = Var_1 : var_g = V     : var_b = Var_3 }
        '    else if ( Var_I == 3 )
        '        { var_r = Var_1 : var_g = Var_2 : var_b = V     }
        '    else if ( Var_I == 4 )
        '        { var_r = Var_3 : var_g = Var_1 : var_b = V     }
        '    else
        '        { var_r = V     : var_g = Var_1 : var_b = Var_2 }
        ' 
        '    R = var_r * 255
        '    G = var_g * 255
        '    B = var_b * 255
        ' }

        ' Argument checking.
        ' No check for arguments with Nothing. They become zero.
        If hueIn < 0.0 OrElse hueIn > 1.0 OrElse saturationIn < 0.0 OrElse
            saturationIn > 1.0 OrElse valueIn < 0.0 OrElse valueIn > 1.0 Then

            ' Argument out of range. Force the result.
            redOut = FORCEDRGBR
            greenOut = FORCEDRGBG
            blueOut = FORCEDRGBB
            Return ' Early exit.
        End If

        If saturationIn.Equals(0.0) Then
            Dim Once As System.Double = valueIn * 255.0
            redOut = Once
            greenOut = Once
            blueOut = Once
        Else

            ' H must be < 1.
            Dim VarHue6 As System.Double = If(hueIn = 1.0, 0.0, hueIn * 6.0)

            Dim Var_I As System.Double = Math.Floor(VarHue6)
            Dim Var_1 As System.Double = valueIn * (1.0 - saturationIn)
            Dim Var_2 As System.Double =
                valueIn * (1.0 - saturationIn * (VarHue6 - Var_I))
            Dim Var_3 As System.Double =
                valueIn * (1.0 - saturationIn * (1.0 - (VarHue6 - Var_I)))

            Dim FracR, FracG, FracB As System.Double
            If Var_I.Equals(0) Then
                FracR = valueIn : FracG = Var_3 : FracB = Var_1
            ElseIf Var_I.Equals(1) Then
                FracR = Var_2 : FracG = valueIn : FracB = Var_1
            ElseIf Var_I.Equals(2) Then
                FracR = Var_1 : FracG = valueIn : FracB = Var_3
            ElseIf Var_I.Equals(3) Then
                FracR = Var_1 : FracG = Var_2 : FracB = valueIn
            ElseIf Var_I.Equals(4) Then
                FracR = Var_3 : FracG = Var_1 : FracB = valueIn
            Else
                FracR = valueIn : FracG = Var_1 : FracB = Var_2
            End If

            redOut = FracR * 255.0
            greenOut = FracG * 255.0
            blueOut = FracB * 255.0

        End If

    End Sub ' HSVtoRGB

End Class ' ColorUtilities

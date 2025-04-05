Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Partial Friend Class ColorDlgWindow

#Region "Global"
    ' These routines are fairly generic or are not tied to a particular color
    ' space or type of mix.

    '''' <summary>
    '''' Creates a string in a standardized form.
    '''' </summary>
    Private Function G4ValueStr(ByVal componentValue As System.Double) _
        As System.String

        Return $"{componentValue:G4}"
    End Function

    '''' <summary>
    '''' Creates a string in a standardized form.
    '''' </summary>
    Private Function DoubleValueStr(ByVal componentName As System.String,
        ByVal componentValue As System.Double) As System.String

        Return $"{componentName}:{Me.G4ValueStr(componentValue)}"
    End Function

    '''' <summary>
    '''' Creates a string in a standardized form.
    '''' </summary>
    Private Function ByteValueStr(ByVal componentName As System.String,
        ByVal componentValue As System.Byte) As System.String

        Return $"{componentName}:{componentValue}"
    End Function

    Private Sub UpdatePreviewLabel()
        ' Update the preview.
        With Me.PreviewLabel
            .Background = Me.RgbWorkSolidBrush
            .Foreground = Me.RgbWorkContrastSolidBrush
            .Content = $"{Me.ByteValueStr(REDWORD, RgbWorkR)}" &
                       $" {Me.ByteValueStr(GREENWORD, RgbWorkG)}" &
                       $" {Me.ByteValueStr(BLUEWORD, RgbWorkB)}"
        End With
    End Sub

    '''' <summary>
    '''' Whole LastRgbChange on the predominant component.
    '''' </summary>
    Private Sub ResetRgbLastChange()
        With Me
            If .UnderlyingB > .UnderlyingR Then
                ' Not red; might be blue.
                .LastRgbChange = If(
                    .UnderlyingB > .UnderlyingG,
                    LastRgbChangeEnum.Blue,
                    LastRgbChangeEnum.Green)
            Else
                ' Not blue; might be red.
                .LastRgbChange = If(
                    .UnderlyingG > .UnderlyingR,
                    LastRgbChangeEnum.Green,
                    LastRgbChangeEnum.Red)
            End If
        End With
    End Sub ' ResetRgbLastChange

    ''' <summary>
    ''' Derives the basic reference values from the specified component values.
    ''' </summary>
    Private Sub DeriveFromRGB(ByVal r As System.Double,
        ByVal g As System.Double, ByVal b As System.Double)

        With Me

            ' Set the base components.
            .UnderlyingR = r
            .UnderlyingG = g
            .UnderlyingB = b

            ' Set the derived values.
            .RgbWorkR = CByte(.UnderlyingR)
            .RgbWorkG = CByte(.UnderlyingG)
            .RgbWorkB = CByte(.UnderlyingB)
            With .RgbWorkColor
                .A = &HFF
                .R = RgbWorkR
                .G = RgbWorkG
                .B = RgbWorkB
            End With
            .RgbWorkContrastColor =
                OSNW.Graphics.ColorUtilities.ContrastingBW(.RgbWorkColor)
            .RgbWorkSolidBrush =
                New System.Windows.Media.SolidColorBrush(.RgbWorkColor)
            .RgbWorkContrastSolidBrush =
                New System.Windows.Media.SolidColorBrush(.RgbWorkContrastColor)

            .UpdatePreviewLabel()

        End With
    End Sub ' DeriveFromRGB

    ''' <summary>
    ''' Distributes the impact of the associated change.
    ''' </summary>
    ''' <param name="baseR">Specifies the red component.</param>
    ''' <param name="baseG">Specifies the green component.</param>
    ''' <param name="baseB">Specifies the blue component.</param>
    ''' <param name="baseLastRgbChange">Specifies which color component was most
    ''' recently set. The value is chosen from <see cref="LastRgbChangeEnum"/>.
    ''' The default value is <c>LastRgbChangeEnum.Auto</c>, which selects the
    ''' most predominant component.</param>
    Private Sub UpdateBaseValuesFromRGB(ByVal baseR As System.Double,
        ByVal baseG As System.Double, ByVal baseB As System.Double,
        Optional ByVal baseLastRgbChange As LastRgbChangeEnum =
            LastRgbChangeEnum.Auto)

        ' Set base color components and derived values.
        With Me

            .DeriveFromRGB(baseR, baseG, baseB)
            OSNW.Graphics.ColorUtilities.RGBtoHSL(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HslWorkH, .HslWorkS, .HslWorkL)
            OSNW.Graphics.ColorUtilities.RGBtoHSV(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HsvWorkH, .HsvWorkS, .HsvWorkV)
            If baseLastRgbChange.Equals(LastRgbChangeEnum.Auto) Then
                ' Assume the most prominant.
                .ResetRgbLastChange()
            Else
                .LastRgbChange = baseLastRgbChange
            End If

        End With
    End Sub ' UpdateBaseValuesFromRGB

    '''' <summary>
    '''' Distributes the impact of the associated change.
    '''' </summary>
    '''' <param name="shadeFactor">Specifies the degree of shading to be applied.
    '''' The input range is 0 to 1000, which is scaled later to 0.000 to 1.000.
    '''' Low values favor the base color; high values favor black.</param>
    Private Sub UpdateBaseValuesFromShade(ByVal shadeFactor As System.Int32)
        ' Set base color components and derived values.
        With Me

            .ShadeWorkFactor = shadeFactor
            .ShadeDeriveBase(shadeFactor)

            OSNW.Graphics.ColorUtilities.RGBtoHSL(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HslWorkH, .HslWorkS, .HslWorkL)
            OSNW.Graphics.ColorUtilities.RGBtoHSV(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HsvWorkH, .HsvWorkS, .HsvWorkV)
            ' Assume the most prominant.
            .ResetRgbLastChange()

            .UpdatePreviewLabel()

        End With
    End Sub ' UpdateBaseValuesFromShade

    '''' <summary>
    '''' Distributes the impact of the associated change.
    '''' </summary>
    '''' <param name="tintFactor">Specifies the degree of tinting to be applied.
    '''' The input range is 0 to 1000, which is scaled later to 0.000 to 1.000.
    '''' Low values favor the base color; high values favor white.</param>
    Private Sub UpdateBaseValuesFromTint(ByVal tintFactor As System.Int32)
        ' Set base color components and derived values.
        With Me

            .TintWorkFactor = tintFactor
            .TintDeriveBase(tintFactor)

            OSNW.Graphics.ColorUtilities.RGBtoHSL(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HslWorkH, .HslWorkS, .HslWorkL)
            OSNW.Graphics.ColorUtilities.RGBtoHSV(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HsvWorkH, .HsvWorkS, .HsvWorkV)
            ' Assume the most prominant.
            .ResetRgbLastChange()

            .UpdatePreviewLabel()

        End With
    End Sub ' UpdateBaseValuesFromTint

    '''' <summary>
    '''' Distributes the impact of the associated change.
    '''' </summary>
    '''' <param name="grayVal">Specifies the gray (range 0 to 255) color to be
    '''' blended with the base color.</param>
    '''' <param name="toneFactor">Specifies the degree of toning to be applied.
    '''' The input range is 0 to 1000, which is scaled later to 0.000 to 1.000.
    '''' Low values favor the base color; high values favor gray.</param>
    '''' <remarks>
    '''' <paramref name="grayVal"/> is used to create a gray color where
    '''' R=grayVal, G=grayVal, B=grayVal.
    '''' </remarks>
    Private Sub UpdateBaseValuesFromTone(ByVal grayVal As System.Byte,
                                         ByVal toneFactor As System.Int32)
        ' Set base color components and derived values.
        With Me

            .ToneWorkFactor = toneFactor
            .ToneWorkGray = grayVal
            .ToneDeriveBase(grayVal, toneFactor)

            OSNW.Graphics.ColorUtilities.RGBtoHSL(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HslWorkH, .HslWorkS, .HslWorkL)
            OSNW.Graphics.ColorUtilities.RGBtoHSV(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HsvWorkH, .HsvWorkS, .HsvWorkV)
            ' Assume the most prominant.
            .ResetRgbLastChange()

            .UpdatePreviewLabel()

        End With
    End Sub ' UpdateBaseValuesFromTone

    '''' <summary>
    '''' Go up to the next lower small step.
    '''' </summary>
    '''' <param name="currVal">Specifies the starting value.</param>
    '''' <returns>The next lower step value.</returns>
    Private Function DownPoint001From(currVal As System.Int32) As System.Int32
        If currVal <= 1 Then
            Return 0
        ElseIf currVal >= 1000 Then
            Return 999
        Else
            Return currVal - 1
        End If
    End Function

    '''' <summary>
    '''' Go up to the next higher small step.
    '''' </summary>
    '''' <param name="currVal">Specifies the starting value.</param>
    '''' <returns>The next higher step value.</returns>
    Private Function UpPoint001From(currVal As System.Int32) As System.Int32
        If currVal >= 999 Then
            Return 1000
        ElseIf currVal <= 0 Then
            Return 1
        Else
            Return currVal + 1
        End If
    End Function

    '''' <summary>
    '''' Go down to the next lower big step.
    '''' </summary>
    '''' <param name="currVal">Specifies the starting value.</param>
    '''' <returns>The next lower step value.</returns>
    Private Function DownPoint01From(currVal As System.Int32) As System.Int32
        If currVal <= 10 Then
            Return 0
        ElseIf currVal > 990 Then
            Return 990
        Else
            Dim ScaleTo10 As System.Double = CDbl(currVal) / 10.0
            Dim IntPart As System.Int32 = CInt(System.Math.Floor(ScaleTo10))
            Dim FracPart As System.Double = ScaleTo10 - IntPart
            If FracPart > 0.0 Then
                ' Drop to the step.
                Return IntPart * 10
            Else
                Return (IntPart - 1) * 10
            End If
        End If
    End Function

    '''' <summary>
    '''' Go down to the next higher big step.
    '''' </summary>
    '''' <param name="currVal">Specifies the starting value.</param>
    '''' <returns>The next higher step value.</returns>
    Private Function UpPoint01From(currVal As System.Int32) As System.Int32
        If currVal >= 990 Then
            Return 1000
        ElseIf currVal <= 10 Then
            Return 10
        Else
            Dim ScaleTo10 As System.Double = CDbl(currVal) / 10.0
            Dim IntPart As System.Int32 = CInt(System.Math.Floor(ScaleTo10))
            Return (IntPart + 1) * 10
        End If
    End Function

    '''' <summary>
    '''' Go down to the next lower big step.
    '''' </summary>
    '''' <param name="currVal">Specifies the starting value.</param>
    '''' <returns>The next lower step value.</returns>
    Private Function Down17From(currVal As System.Byte) As System.Byte

        If currVal = &H0 Then
            Return &H0 ' Early exit.
        End If

        'Dim Whole As System.Byte = CByte(currVal \ 17)
        'Dim Remainder As System.Int32 = currVal Mod 17
        'If Remainder.Equals(0) Then
        '    Return CByte((Whole - 1) * 17)
        'Else
        '    Return CByte(Whole * 17)
        'End If

        Dim Whole As System.Byte = CByte(currVal \ 17)
        Dim Remainder As System.Int32 = currVal Mod 17
        Dim Base As System.Int32 =
            If(Remainder.Equals(0), Whole - 1, Whole)
        Return CByte(Base * 17)

    End Function

    '''' <summary>
    '''' Go down to the next higher big step.
    '''' </summary>
    '''' <param name="currVal">Specifies the starting value.</param>
    '''' <returns>The next higher step value.</returns>
    Private Function Up17From(currVal As System.Byte) As System.Byte

        If currVal = &HFF Then
            Return &HFF ' Early exit.
        End If

        Dim Whole As System.Byte = CByte(currVal \ 17)
        Return CByte((Whole + 1) * 17)

    End Function

#End Region ' "Global"

End Class ' ColorDlgWindow

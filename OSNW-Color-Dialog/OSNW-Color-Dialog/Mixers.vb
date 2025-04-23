Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports System.Windows.Input
Imports SHWV = OSNW.Dialog.ColorDlgWindow.SharedHostedWindowValues

Partial Friend Class ColorDlgWindow

#Region "Shade tab"

    Private Sub ShadeFillImage()

        ' Determine the pure hue to be used.
        Dim PureHue, DummyS, DummyV As System.Double
        OSNW.Graphics.ColorUtilities.RGBtoHSV(Me.ShadeStartR, Me.ShadeStartG,
            Me.ShadeStartB, PureHue, DummyS, DummyV)

        ' Determine the pure color to be used.
        Dim PureR, PureG, PureB As System.Double
        OSNW.Graphics.ColorUtilities.HSVtoRGB(
            PureHue, 1.0, 1.0, PureR, PureG, PureB)

        ' The shade factor for one column.
        Dim ColShadeFactor As System.Double

        ' Doubles for GetShade results.
        Dim RValD, GValD, BValD As System.Double

        ' These are Int32 to allow for left shift without exceptions.
        Dim RVal, GVal, BVal As System.Int32

        ' The constructed color for one column.
        Dim ConstructedColor As System.Int32

        ' Construct an array with colors for the individual pixels. Column is
        ' first here because of the way that the array offsets are processed in
        ' PixelsToImage.
        Dim PixelArray(CInt(Me.ShadeImage.Width - 1),
                       CInt(Me.ShadeImage.Height - 1)) As System.Int32
        Dim MaxCol As System.Int32 = PixelArray.GetUpperBound(0)
        Dim MaxRow As System.Int32 = PixelArray.GetUpperBound(1)

        ' Populate the array.
        For Col As Int32 = 0 To MaxCol

            ' Get the shade for this column.
            ColShadeFactor = Col / CDbl(MaxCol)
            OSNW.Graphics.ColorUtilities.GetShade(
                PureR, PureG, PureB, ColShadeFactor, RValD, GValD, BValD)

            RVal = CInt(RValD)
            GVal = CInt(GValD)
            BVal = CInt(BValD)
            ConstructedColor = HFF + (RVal << 16) + (GVal << 8) + BVal

            For Row As Int32 = 0 To MaxRow
                PixelArray(Col, Row) = ConstructedColor
            Next

        Next

        Me.ShadeImage.Source =
            OSNW.Graphics.ColorUtilities.PixelsToImageSource(PixelArray)

    End Sub ' ShadeFillImage

    '''' <summary>
    '''' Update the visual items unique to this tab; prepare for the next
    '''' adjustment.
    '''' </summary>
    Private Sub ShadeUpdateVisuals()
        With Me

            ' Update the displayed per-component values.
            .ShadeTweakRedLabel.Content =
                .DoubleValueStr(SHWV.REDWORD, .UnderlyingR)
            .ShadeTweakGreenLabel.Content =
                .DoubleValueStr(SHWV.GREENWORD, .UnderlyingG)
            .ShadeTweakBlueLabel.Content =
                .DoubleValueStr(SHWV.BLUEWORD, .UnderlyingB)
            .ShadeTweakHueLabel.Content =
                .DoubleValueStr(SHWV.HUEWORD, .ShadeStartH)
            .ShadeTweakFactorLabel.Content = .DoubleValueStr(
                "Factor", .ShadeWorkFactor / 1000.0)

            ' Limit visibility until a selection has been made.
            If .ShadeFactorClicked Then
                .ShadeTweakFactorLabel.Visibility =
                    System.Windows.Visibility.Visible
                .ShadeMM.IsEnabled = True
                .ShadeM.IsEnabled = True
                .ShadeP.IsEnabled = True
                .ShadePP.IsEnabled = True
            Else
                .ShadeTweakFactorLabel.Visibility =
                    System.Windows.Visibility.Collapsed
                .ShadeMM.IsEnabled = False
                .ShadeM.IsEnabled = False
                .ShadeP.IsEnabled = False
                .ShadePP.IsEnabled = False
            End If

            ' Update the image.
            Me.ShadeFillImage()

        End With
    End Sub ' ShadeUpdateVisuals

    '''' <summary>
    '''' Respond to  click in the image.
    '''' </summary>
    Private Sub ShadeProcessMouseClick(
        sender As Object, e As MouseButtonEventArgs)

        ' Ref: Type Conversion Functions (Visual Basic)
        ' https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/functions/type-conversion-functions?f1url=%3FappId%3DDev17IDEF1%26l%3DEN-US%26k%3Dk(vb.CInt)%3Bk(DevLang-VB)%26rd%3Dtrue#remarks
        ' 
        ' Fractional Parts. When you convert a nonintegral value to an integral
        ' type, the integer conversion functions (CByte, CInt, CLng, CSByte,
        ' CShort, CUInt, CULng, and CUShort) remove the fractional part and
        ' round the value to the closest integer.
        ' If the fractional part is exactly 0.5, the integer conversion
        ' functions round it to the nearest even integer. For example, 0.5
        ' rounds to 0, and 1.5 and 2.5 both round to 2. This is sometimes called
        ' banker's rounding, and its purpose is to compensate for a bias
        ' that could accumulate when adding many such numbers together.
        ' CInt and CLng differ from the Int and Fix functions, which truncate,
        ' rather than round, the fractional part of a number. Also, Fix and Int
        ' always return a value of the same data type as you pass in.

        ' Determine the mouse position within the image.
        ' Floor may actually be more precise than rounding. Nothing has been
        ' found yet to get the proper understanding of the Doubles in Point. See
        ' above regarding rounding.
        Dim PixelPoint As System.Windows.Point =
            e.GetPosition(CType(sender, System.Windows.IInputElement))
        Dim PixelX As System.Int32 = CInt(System.Math.Floor(PixelPoint.X))

        With Me

            Dim ClickedFactor As System.Double = PixelX / .ShadeImage.Width
            .UpdateBaseValuesFromShade(CInt(ClickedFactor * 1000.0))

            ' Update the displays.
            .ShadeFactorClicked = True
            .ShadeUpdateVisuals()

        End With

    End Sub ' ShadeProcessMouseClick

    Private Sub ShadeDeriveBase(ByVal shadeFactor As System.Int32)
        With Me

            ' Calculate the pure color components for the hue.
            Dim PureR, PureG, PureB As System.Double
            OSNW.Graphics.ColorUtilities.HSVtoRGB(
                .ShadeStartH, 1.0, 1.0, PureR, PureG, PureB)

            ' Calculate the shaded components.
            Dim ShadedR, ShadedG, ShadedB As System.Double
            OSNW.Graphics.ColorUtilities.GetShade(PureR, PureG, PureB,
                CDbl(shadeFactor) / 1000.0, ShadedR, ShadedG, ShadedB)

            ' Push updates.
            .UpdateBaseValuesFromRGB(ShadedR, ShadedG, ShadedB)

        End With
    End Sub ' ShadeDeriveBase

#End Region ' "Shade tab"

#Region "Tint tab"

    Private Sub TintFillImage()

        ' Determine the pure hue to be used.
        Dim PureHue, DummyS, DummyV As System.Double
        OSNW.Graphics.ColorUtilities.RGBtoHSV(Me.TintStartR, Me.TintStartG,
            Me.TintStartB, PureHue, DummyS, DummyV)

        ' Determine the pure color to be used.
        Dim PureR, PureG, PureB As System.Double
        OSNW.Graphics.ColorUtilities.HSVtoRGB(
            PureHue, 1.0, 1.0, PureR, PureG, PureB)

        ' The tint factor for one column.
        Dim ColTintFactor As System.Double

        ' Doubles for GetTint results.
        Dim RValD, GValD, BValD As System.Double

        ' These are Int32 to allow for left shift without exceptions.
        Dim RVal, GVal, BVal As System.Int32

        ' The constructed color for one column.
        Dim ConstructedColor As System.Int32

        ' Construct an array with colors for the individual pixels. Column is
        ' first here because of the way that the array offsets are processed in
        ' PixelsToImage.
        Dim PixelArray(CInt(Me.TintImage.Width) - 1,
                       CInt(Me.TintImage.Height) - 1) As System.Int32
        Dim MaxCol As System.Int32 = PixelArray.GetUpperBound(0)
        Dim MaxRow As System.Int32 = PixelArray.GetUpperBound(1)

        ' Populate the array.
        For Col As Int32 = 0 To MaxCol

            ' Get the tint for this column.
            ColTintFactor = Col / CDbl(MaxCol)
            OSNW.Graphics.ColorUtilities.GetTint(PureR, PureG, PureB, ColTintFactor,
                                             RValD, GValD, BValD)

            RVal = CInt(RValD)
            GVal = CInt(GValD)
            BVal = CInt(BValD)
            ConstructedColor = HFF + (RVal << 16) + (GVal << 8) + BVal

            For Row As Int32 = 0 To MaxRow
                PixelArray(Col, Row) = ConstructedColor
            Next

        Next

        Me.TintImage.Source =
            OSNW.Graphics.ColorUtilities.PixelsToImageSource(PixelArray)

    End Sub ' TintFillImage

    '''' <summary>
    '''' Update the visual items unique to this tab; prepare for the next
    '''' adjustment.
    '''' </summary>
    Private Sub TintUpdateVisuals()
        With Me

            ' Update the displayed per-component values.
            .TintTweakRedLabel.Content =
                .DoubleValueStr(SHWV.REDWORD, .UnderlyingR)
            .TintTweakGreenLabel.Content =
                .DoubleValueStr(SHWV.GREENWORD, .UnderlyingG)
            .TintTweakBlueLabel.Content =
                .DoubleValueStr(SHWV.BLUEWORD, .UnderlyingB)
            .TintTweakHueLabel.Content =
                .DoubleValueStr(SHWV.HUEWORD, .TintStartH)
            .TintTweakFactorLabel.Content = .DoubleValueStr(
                "Factor", .TintWorkFactor / 1000.0)

            ' Limit visibility until a selection has been made.
            If .TintFactorClicked Then
                .TintTweakFactorLabel.Visibility =
                    System.Windows.Visibility.Visible
                .TintMM.IsEnabled = True
                .TintM.IsEnabled = True
                .TintP.IsEnabled = True
                .TintPP.IsEnabled = True
            Else
                .TintTweakFactorLabel.Visibility =
                    System.Windows.Visibility.Collapsed
                .TintMM.IsEnabled = False
                .TintM.IsEnabled = False
                .TintP.IsEnabled = False
                .TintPP.IsEnabled = False
            End If

            ' Update the image.
            Me.TintFillImage()

        End With
    End Sub ' TintUpdateVisuals

    '''' <summary>
    '''' Respond to  click in the image.
    '''' </summary>
    Private Sub TintProcessMouseClick(
        sender As Object, e As MouseButtonEventArgs)

        ' Ref: Type Conversion Functions (Visual Basic)
        ' https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/functions/type-conversion-functions?f1url=%3FappId%3DDev17IDEF1%26l%3DEN-US%26k%3Dk(vb.CInt)%3Bk(DevLang-VB)%26rd%3Dtrue#remarks
        ' 
        ' Fractional Parts. When you convert a nonintegral value to an integral
        ' type, the integer conversion functions (CByte, CInt, CLng, CSByte,
        ' CShort, CUInt, CULng, and CUShort) remove the fractional part and
        ' round the value to the closest integer.
        ' If the fractional part is exactly 0.5, the integer conversion
        ' functions round it to the nearest even integer. For example, 0.5
        ' rounds to 0, and 1.5 and 2.5 both round to 2. This is sometimes called
        ' banker's rounding, and its purpose is to compensate for a bias
        ' that could accumulate when adding many such numbers together.
        ' CInt and CLng differ from the Int and Fix functions, which truncate,
        ' rather than round, the fractional part of a number. Also, Fix and Int
        ' always return a value of the same data type as you pass in.

        ' Determine the mouse position within the image.
        ' Floor may actually be more precise than rounding. Nothing has been
        ' found yet to get the proper understanding of the Doubles in Point. See
        ' above regarding rounding.
        Dim PixelPoint As System.Windows.Point =
            e.GetPosition(CType(sender, System.Windows.IInputElement))
        Dim PixelX As System.Int32 = CInt(System.Math.Floor(PixelPoint.X))

        With Me

            Dim ClickedFactor As System.Double = PixelX / .TintImage.Width
            .UpdateBaseValuesFromTint(CInt(ClickedFactor * 1000.0))

            ' Update the displays.
            .TintFactorClicked = True
            .TintUpdateVisuals()

        End With

    End Sub ' TintProcessMouseClick

    Private Sub TintDeriveBase(ByVal tintFactor As System.Int32)
        With Me

            ' Calculate the pure color components for the hue.
            Dim PureR, PureG, PureB As System.Double
            OSNW.Graphics.ColorUtilities.HSVtoRGB(
                .TintStartH, 1.0, 1.0, PureR, PureG, PureB)

            ' Calculate the tinted components.
            Dim TintedR, TintedG, TintedB As System.Double
            OSNW.Graphics.ColorUtilities.GetTint(PureR, PureG, PureB,
                CDbl(tintFactor) / 1000.0, TintedR, TintedG, TintedB)

            ' Push updates.
            .UpdateBaseValuesFromRGB(TintedR, TintedG, TintedB)

        End With
    End Sub ' TintDeriveBase

#End Region ' "Tint tab"

#Region "Tone tab"

    Private Sub ToneFillImage()

        ' Determine the pure hue to be used.
        Dim PureHue, DummyS, DummyV As System.Double
        OSNW.Graphics.ColorUtilities.RGBtoHSV(Me.ToneStartR, Me.ToneStartG,
            Me.ToneStartB, PureHue, DummyS, DummyV)

        ' Determine the pure color to be used.
        Dim PureR, PureG, PureB As System.Double
        OSNW.Graphics.ColorUtilities.HSVtoRGB(
            PureHue, 1.0, 1.0, PureR, PureG, PureB)

        ' The tone factor for a row.
        Dim ToneFactor As System.Double

        ' Doubles for GetTone results.
        Dim RValD, GValD, BValD As System.Double

        ' These are Int32 to allow for left shift without exceptions.
        Dim RVal, GVal, BVal As System.Int32

        ' The constructed color for one pixel.
        Dim ConstructedColor As System.Int32

        ' Construct an array for colors of the individual pixels.
        Dim PixelArray(CInt(Me.ToneImage.Width) - 1,
                           CInt(Me.ToneImage.Height) - 1) As System.Int32
        Dim MaxCol As System.Int32 = PixelArray.GetUpperBound(0)
        Dim MaxRow As System.Int32 = PixelArray.GetUpperBound(1)

        ' Populate the array.
        For Row As Int32 = 0 To MaxRow
            ToneFactor = 1.0 - (Row / MaxRow)
            For Col As Int32 = 0 To MaxCol

                OSNW.Graphics.ColorUtilities.GetTone(PureR, PureG, PureB, Col,
                    ToneFactor, RValD, GValD, BValD)

                RVal = CInt(RValD)
                GVal = CInt(GValD)
                BVal = CInt(BValD)
                ConstructedColor = HFF + (RVal << 16) + (GVal << 8) + BVal

                PixelArray(Col, Row) = ConstructedColor

            Next
        Next

        Me.ToneImage.Source =
            OSNW.Graphics.ColorUtilities.PixelsToImageSource(PixelArray)

    End Sub ' ToneFillImage

    '''' <summary>
    '''' Update the visual items unique to this tab; prepare for the next
    '''' adjustment.
    '''' </summary>
    Private Sub ToneUpdateVisuals()
        With Me

            ' Update the displayed per-component values for the base reference.
            .ToneBaseRedLabel.Content = .ByteValueStr(SHWV.REDWORD, .ToneStartR)
            .ToneBaseGreenLabel.Content =
                .ByteValueStr(SHWV.GREENWORD, .ToneStartG)
            .ToneBaseBlueLabel.Content =
                .ByteValueStr(SHWV.BLUEWORD, .ToneStartB)
            .ToneTweakHueLabel.Content =
                .DoubleValueStr(SHWV.HUEWORD, .ToneStartH)

            ' Limit visibility until a selection has been made.
            If .ToneValuesClicked Then
                .ToneTweakFactorLabel.Content =
                    .DoubleValueStr("Tone Factor", .ToneWorkFactor / 1000.0)
                .ToneTweakGrayLabel.Content = .ByteValueStr("Gray Level",
                                                            .ToneWorkGray)
                .ToneGrayMM.IsEnabled = True
                .ToneGrayM.IsEnabled = True
                .ToneGrayP.IsEnabled = True
                .ToneGrayPP.IsEnabled = True
                .ToneFactorMM.IsEnabled = True
                .ToneFactorM.IsEnabled = True
                .ToneFactorP.IsEnabled = True
                .ToneFactorPP.IsEnabled = True
            Else
                .ToneTweakFactorLabel.Content = "Tone Factor not set"
                .ToneTweakGrayLabel.Content = "Gray Level not set"
                .ToneGrayMM.IsEnabled = False
                .ToneGrayM.IsEnabled = False
                .ToneGrayP.IsEnabled = False
                .ToneGrayPP.IsEnabled = False
                .ToneFactorMM.IsEnabled = False
                .ToneFactorM.IsEnabled = False
                .ToneFactorP.IsEnabled = False
                .ToneFactorPP.IsEnabled = False
            End If

            ' Update the image.
            .ToneFillImage()

        End With
    End Sub ' ToneUpdateVisuals

    '''' <summary>
    '''' Respond to  click in the image.
    '''' </summary>
    Private Sub ToneProcessMouseClick(
        sender As Object, e As MouseButtonEventArgs)

        ' Ref: Type Conversion Functions (Visual Basic)
        ' https://learn.microsoft.com/en-us/dotnet/visual-basic/language-reference/functions/type-conversion-functions?f1url=%3FappId%3DDev17IDEF1%26l%3DEN-US%26k%3Dk(vb.CInt)%3Bk(DevLang-VB)%26rd%3Dtrue#remarks
        ' 
        ' Fractional Parts. When you convert a nonintegral value to an integral
        ' type, the integer conversion functions (CByte, CInt, CLng, CSByte,
        ' CShort, CUInt, CULng, and CUShort) remove the fractional part and
        ' round the value to the closest integer.
        ' If the fractional part is exactly 0.5, the integer conversion
        ' functions round it to the nearest even integer. For example, 0.5
        ' rounds to 0, and 1.5 and 2.5 both round to 2. This is sometimes called
        ' banker's rounding, and its purpose is to compensate for a bias
        ' that could accumulate when adding many such numbers together.
        ' CInt and CLng differ from the Int and Fix functions, which truncate,
        ' rather than round, the fractional part of a number. Also, Fix and Int
        ' always return a value of the same data type as you pass in.

        ' Determine the mouse position within the image.
        ' Floor may actually be more precise than rounding. Nothing has been
        ' found yet to get the proper understanding of the Doubles in Point. See
        ' above regarding rounding.
        Dim PixelPoint As System.Windows.Point =
            e.GetPosition(CType(sender, System.Windows.IInputElement))
        Dim PixelX As System.Int32 = CInt(System.Math.Floor(PixelPoint.X))
        Dim PixelY As System.Int32 = CInt(System.Math.Floor(PixelPoint.Y))

        With Me

            Dim ClickedFactor As System.Double = 1.0 - (PixelY / (.ToneImage.Height - 1.0))
            .UpdateBaseValuesFromTone(CByte(PixelX), CInt(ClickedFactor * 1000.0))

            ' Update the displays.
            .ToneValuesClicked = True
            .ToneUpdateVisuals()

        End With

    End Sub ' ToneProcessMouseClick

    Private Sub ToneDeriveBase(ByVal grayVal As System.Byte,
                               ByVal toneFactor As System.Int32)
        With Me

            ' Calculate the pure color components for the hue.
            Dim PureR, PureG, PureB As System.Double
            OSNW.Graphics.ColorUtilities.HSVtoRGB(
                .ToneStartH, 1.0, 1.0, PureR, PureG, PureB)

            ' Calculate the toned components.
            Dim TonedR, TonedG, TonedB As System.Double
            OSNW.Graphics.ColorUtilities.GetTone(PureR, PureG, PureB,
                grayVal, CDbl(toneFactor) / 1000.0, TonedR, TonedG, TonedB)

            ' Push updates.
            .UpdateBaseValuesFromRGB(TonedR, TonedG, TonedB)

        End With
    End Sub ' ToneDeriveBase

#End Region ' "Tone tab"

#Region "Blend tab"

    Private Sub BlendValidateRgb(
        ByVal oneTextBox As System.Windows.Controls.TextBox,
        ByRef byteVal As System.Byte, ByRef allValid As System.Boolean)

        ' Avoid processing for items not yet created.
        If IsNothing(oneTextBox) Then
            Exit Sub ' Early exit.
        End If

        If System.Byte.TryParse(oneTextBox.Text, byteVal) Then
            ' Any byte is valid.
            oneTextBox.Background = GoodBackgroundBrush
        Else
            oneTextBox.Background = BadBackgroundBrush
            allValid = False
        End If

    End Sub ' BlendValidateRgb

    Private Sub BlendValidateRatio(
        ByVal oneTextBox As System.Windows.Controls.TextBox,
        ByRef doubleVal As System.Double, ByRef allValid As System.Boolean)

        ' Avoid processing for items not yet created.
        If IsNothing(oneTextBox) Then
            Exit Sub ' Early exit.
        End If

        If Not System.Double.TryParse(oneTextBox.Text, doubleVal) Then
            oneTextBox.Background = BadBackgroundBrush
            allValid = False
        ElseIf doubleVal < 0.0 Then
            oneTextBox.Background = BadBackgroundBrush
            allValid = False
        Else
            oneTextBox.Background = GoodBackgroundBrush
        End If

    End Sub ' BlendValidateRatio

    Private Sub BlendSetRgbWorkColors(ByVal r As System.Double,
        ByVal g As System.Double, ByVal b As System.Double)

        With Me
            .UpdateBaseValuesFromRGB(r, g, b)
            OSNW.Graphics.ColorUtilities.RGBtoHSL(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HslWorkH, .HslWorkS, .HslWorkL)
            OSNW.Graphics.ColorUtilities.RGBtoHSV(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HsvWorkH, .HsvWorkS, .HsvWorkV)
        End With
    End Sub ' BlendSetRgbWorkColors

#End Region ' "Blend tab"

End Class ' ColorDlgWindow

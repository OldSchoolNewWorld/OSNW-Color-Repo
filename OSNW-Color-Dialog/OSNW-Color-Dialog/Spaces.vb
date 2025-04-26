Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports System.Windows.Input
Imports SHWV = OSNW.Dialog.ColorDlgWindow.SharedHostedWindowValues

Partial Friend Class ColorDlgWindow

    ' This file contains routines for the tabs related to color spaces - various
    ' ways to represent a color.

#Region "Convert tab"

    Private Sub DoConvertTextBoxByteTextChanged(
        ByVal sendingTextBox As System.Windows.Controls.TextBox,
        ByVal ByteVal As System.Byte)

        With Me

            Dim PushOnArrival As System.Boolean = .ConvertTabPushing
            .ConvertTabPushing = True
            Try
                If sendingTextBox.Equals(.ConvertRgbRedTextBox) Then
                    .ConvertSetWorkColorsFromRgb(ByteVal, .RgbWorkG, RgbWorkB)
                    .LastRgbChange = LastRgbChangeEnum.Red
                ElseIf sendingTextBox.Equals(.ConvertRgbGreenTextBox) Then
                    .ConvertSetWorkColorsFromRgb(.RgbWorkR, ByteVal, .RgbWorkB)
                    .LastRgbChange = LastRgbChangeEnum.Green
                ElseIf sendingTextBox.Equals(.ConvertRgbBlueTextBox) Then
                    .ConvertSetWorkColorsFromRgb(.RgbWorkR, .RgbWorkG, ByteVal)
                    .LastRgbChange = LastRgbChangeEnum.Blue
                End If
            Finally
                .ConvertTabPushing = PushOnArrival
            End Try

            .UpdatePreviewLabel()
            .ConvertUpdateVisuals(sendingTextBox)

        End With
    End Sub ' DoConvertTextBoxByteTextChanged

    Private Sub DoConvertTextBoxHtmlTextChanged(
        ByVal sendingTextBox As System.Windows.Controls.TextBox,
        ByVal htmlText As System.String)

        With Me

            Dim PushOnArrival As System.Boolean = .ConvertTabPushing
            .ConvertTabPushing = True
            Try
                Dim R, G, B As System.Byte
                OSNW.Graphics.ColorUtilities.HTMLtoRGB(htmlText, R, G, B)
                .ConvertSetWorkColorsFromRgb(R, G, B)
            Finally
                .ConvertTabPushing = PushOnArrival
            End Try

            .UpdatePreviewLabel()
            .ConvertUpdateVisuals(sendingTextBox)

        End With
    End Sub ' DoConvertTextBoxHtmlTextChanged

    Private Sub DoConvertTextBoxDoubleTextChanged(
        ByVal sendingTextBox As System.Windows.Controls.TextBox,
        ByVal doubleVal As System.Double)

        With Me

            Dim PushOnArrival As System.Boolean = .ConvertTabPushing
            .ConvertTabPushing = True
            Try
                ' Combine the changed value with two existing values.
                If sendingTextBox.Equals(.ConvertHueFractionTextBox) Then
                    OSNW.Graphics.ColorUtilities.HSVtoRGB(doubleVal, .HsvWorkS,
                        .HsvWorkV, .UnderlyingR, .UnderlyingG, .UnderlyingB)
                ElseIf sendingTextBox.Equals(.ConvertHueDegreesTextBox) Then
                    OSNW.Graphics.ColorUtilities.HSVtoRGB(doubleVal / 360.0, .HsvWorkS,
                        .HsvWorkV, .UnderlyingR, .UnderlyingG, .UnderlyingB)
                ElseIf sendingTextBox.Equals(.ConvertHslSaturationTextBox) Then
                    OSNW.Graphics.ColorUtilities.HSLtoRGB(.HslWorkH, doubleVal,
                        .HslWorkL, .UnderlyingR, .UnderlyingG, .UnderlyingB)
                ElseIf sendingTextBox.Equals(.ConvertHslLuminanceTextBox) Then
                    OSNW.Graphics.ColorUtilities.HSLtoRGB(.HslWorkH, .HslWorkS,
                        doubleVal, .UnderlyingR, .UnderlyingG, .UnderlyingB)
                ElseIf sendingTextBox.Equals(.ConvertHsvSaturationTextBox) Then
                    OSNW.Graphics.ColorUtilities.HSVtoRGB(.HsvWorkH, doubleVal,
                        .HsvWorkV, .UnderlyingR, .UnderlyingG, .UnderlyingB)
                ElseIf sendingTextBox.Equals(.ConvertHsvValueTextBox) Then
                    OSNW.Graphics.ColorUtilities.HSVtoRGB(.HsvWorkH, .HsvWorkS,
                        doubleVal, .UnderlyingR, .UnderlyingG, .UnderlyingB)
                End If
            Finally
                .ConvertTabPushing = PushOnArrival
            End Try

            .ConvertSetWorkColorsFromRgb(.UnderlyingR, .UnderlyingG,
                                         .UnderlyingB)
            .ResetRgbLastChange()
            .UpdatePreviewLabel()
            .ConvertUpdateVisuals(sendingTextBox)

        End With
    End Sub ' DoConvertTextBoxDoubleTextChanged

    '''' <summary>
    '''' Update the visual items unique to this tab; prepare for the next
    '''' adjustment.
    '''' </summary>
    Private Sub ConvertUpdateVisuals(
        Optional ByVal exceptFor As System.Windows.Controls.TextBox =
            Nothing)

        With Me
            Dim PushOnArrival As System.Boolean = .ConvertTabPushing
            .ConvertTabPushing = True
            Try

                If exceptFor IsNot .ConvertRgbRedTextBox Then
                    .ConvertRgbRedTextBox.Text = .RgbWorkR.ToString
                End If
                If exceptFor IsNot .ConvertRgbGreenTextBox Then
                    .ConvertRgbGreenTextBox.Text = .RgbWorkG.ToString
                End If
                If exceptFor IsNot .ConvertRgbBlueTextBox Then
                    .ConvertRgbBlueTextBox.Text = .RgbWorkB.ToString
                End If

                If exceptFor IsNot .ConvertHtmlTextBox Then
                    Dim HtmlText As System.String = System.String.Empty
                    OSNW.Graphics.ColorUtilities.RGBtoHTML(
                        .RgbWorkR, .RgbWorkG, .RgbWorkB, HtmlText)
                    .ConvertHtmlTextBox.Text = HtmlText
                End If

                If exceptFor IsNot .ConvertHueFractionTextBox Then
                    .ConvertHueFractionTextBox.Text = .G4ValueStr(.HslWorkH)
                End If
                If exceptFor IsNot .ConvertHueDegreesTextBox Then
                    .ConvertHueDegreesTextBox.Text = .G4ValueStr(.HsvWorkH * 360)
                End If

                If exceptFor IsNot .ConvertHslSaturationTextBox Then
                    .ConvertHslSaturationTextBox.Text = .G4ValueStr(.HslWorkS)
                End If
                If exceptFor IsNot .ConvertHslLuminanceTextBox Then
                    .ConvertHslLuminanceTextBox.Text = .G4ValueStr(.HslWorkL)
                End If

                If exceptFor IsNot .ConvertHsvSaturationTextBox Then
                    .ConvertHsvSaturationTextBox.Text = .G4ValueStr(.HsvWorkS)
                End If
                If exceptFor IsNot .ConvertHsvValueTextBox Then
                    .ConvertHsvValueTextBox.Text = .G4ValueStr(.HsvWorkV)
                End If

            Finally
                .ConvertTabPushing = PushOnArrival
            End Try
        End With
    End Sub ' ConvertUpdateVisuals

    Private Sub ConvertSetWorkColorsFromRgb(ByVal r As System.Double,
        ByVal g As System.Double, ByVal b As System.Double)

        With Me
            .DeriveFromRGB(r, g, b)
            OSNW.Graphics.ColorUtilities.RGBtoHSL(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HslWorkH, .HslWorkS, .HslWorkL)
            OSNW.Graphics.ColorUtilities.RGBtoHSV(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HsvWorkH, .HsvWorkS, .HsvWorkV)
        End With
    End Sub ' ConvertSetRgbWorkColors

#End Region ' "Convert tab"

#Region "Defined tab"

    Public Shared ReadOnly DefinedAllColors As NamedColorPair() = {
        New NamedColorPair("AliceBlue", System.Windows.Media.Colors.AliceBlue),
        New NamedColorPair("AntiqueWhite", System.Windows.Media.Colors.AntiqueWhite),
        New NamedColorPair("Aqua", System.Windows.Media.Colors.Aqua),
        New NamedColorPair("Aquamarine", System.Windows.Media.Colors.Aquamarine),
        New NamedColorPair("Azure", System.Windows.Media.Colors.Azure),
        New NamedColorPair("Beige", System.Windows.Media.Colors.Beige),
        New NamedColorPair("Bisque", System.Windows.Media.Colors.Bisque),
        New NamedColorPair("Black", System.Windows.Media.Colors.Black),
        New NamedColorPair("BlanchedAlmond", System.Windows.Media.Colors.BlanchedAlmond),
        New NamedColorPair("Blue", System.Windows.Media.Colors.Blue),
        New NamedColorPair("BlueViolet", System.Windows.Media.Colors.BlueViolet),
        New NamedColorPair("Brown", System.Windows.Media.Colors.Brown),
        New NamedColorPair("BurlyWood", System.Windows.Media.Colors.BurlyWood),
        New NamedColorPair("CadetBlue", System.Windows.Media.Colors.CadetBlue),
        New NamedColorPair("Chartreuse", System.Windows.Media.Colors.Chartreuse),
        New NamedColorPair("Chocolate", System.Windows.Media.Colors.Chocolate),
        New NamedColorPair("Coral", System.Windows.Media.Colors.Coral),
        New NamedColorPair("CornflowerBlue", System.Windows.Media.Colors.CornflowerBlue),
        New NamedColorPair("Cornsilk", System.Windows.Media.Colors.Cornsilk),
        New NamedColorPair("Crimson", System.Windows.Media.Colors.Crimson),
        New NamedColorPair("Cyan", System.Windows.Media.Colors.Cyan),
        New NamedColorPair("DarkBlue", System.Windows.Media.Colors.DarkBlue),
        New NamedColorPair("DarkCyan", System.Windows.Media.Colors.DarkCyan),
        New NamedColorPair("DarkGoldenrod", System.Windows.Media.Colors.DarkGoldenrod),
        New NamedColorPair("DarkGray", System.Windows.Media.Colors.DarkGray),
        New NamedColorPair("DarkGreen", System.Windows.Media.Colors.DarkGreen),
        New NamedColorPair("DarkKhaki", System.Windows.Media.Colors.DarkKhaki),
        New NamedColorPair("DarkMagenta", System.Windows.Media.Colors.DarkMagenta),
        New NamedColorPair("DarkOliveGreen", System.Windows.Media.Colors.DarkOliveGreen),
        New NamedColorPair("DarkOrange", System.Windows.Media.Colors.DarkOrange),
        New NamedColorPair("DarkOrchid", System.Windows.Media.Colors.DarkOrchid),
        New NamedColorPair("DarkRed", System.Windows.Media.Colors.DarkRed),
        New NamedColorPair("DarkSalmon", System.Windows.Media.Colors.DarkSalmon),
        New NamedColorPair("DarkSeaGreen", System.Windows.Media.Colors.DarkSeaGreen),
        New NamedColorPair("DarkSlateBlue", System.Windows.Media.Colors.DarkSlateBlue),
        New NamedColorPair("DarkSlateGray", System.Windows.Media.Colors.DarkSlateGray),
        New NamedColorPair("DarkTurquoise", System.Windows.Media.Colors.DarkTurquoise),
        New NamedColorPair("DarkViolet", System.Windows.Media.Colors.DarkViolet),
        New NamedColorPair("DeepPink", System.Windows.Media.Colors.DeepPink),
        New NamedColorPair("DeepSkyBlue", System.Windows.Media.Colors.DeepSkyBlue),
        New NamedColorPair("DimGray", System.Windows.Media.Colors.DimGray),
        New NamedColorPair("DodgerBlue", System.Windows.Media.Colors.DodgerBlue),
        New NamedColorPair("Firebrick", System.Windows.Media.Colors.Firebrick),
        New NamedColorPair("FloralWhite", System.Windows.Media.Colors.FloralWhite),
        New NamedColorPair("ForestGreen", System.Windows.Media.Colors.ForestGreen),
        New NamedColorPair("Fuchsia", System.Windows.Media.Colors.Fuchsia),
        New NamedColorPair("Gainsboro", System.Windows.Media.Colors.Gainsboro),
        New NamedColorPair("GhostWhite", System.Windows.Media.Colors.GhostWhite),
        New NamedColorPair("Gold", System.Windows.Media.Colors.Gold),
        New NamedColorPair("Goldenrod", System.Windows.Media.Colors.Goldenrod),
        New NamedColorPair("Gray", System.Windows.Media.Colors.Gray),
        New NamedColorPair("Green", System.Windows.Media.Colors.Green),
        New NamedColorPair("GreenYellow", System.Windows.Media.Colors.GreenYellow),
        New NamedColorPair("Honeydew", System.Windows.Media.Colors.Honeydew),
        New NamedColorPair("HotPink", System.Windows.Media.Colors.HotPink),
        New NamedColorPair("IndianRed", System.Windows.Media.Colors.IndianRed),
        New NamedColorPair("Indigo", System.Windows.Media.Colors.Indigo),
        New NamedColorPair("Ivory", System.Windows.Media.Colors.Ivory),
        New NamedColorPair("Khaki", System.Windows.Media.Colors.Khaki),
        New NamedColorPair("Lavender", System.Windows.Media.Colors.Lavender),
        New NamedColorPair("LavenderBlush", System.Windows.Media.Colors.LavenderBlush),
        New NamedColorPair("LawnGreen", System.Windows.Media.Colors.LawnGreen),
        New NamedColorPair("LemonChiffon", System.Windows.Media.Colors.LemonChiffon),
        New NamedColorPair("LightBlue", System.Windows.Media.Colors.LightBlue),
        New NamedColorPair("LightCoral", System.Windows.Media.Colors.LightCoral),
        New NamedColorPair("LightCyan", System.Windows.Media.Colors.LightCyan),
        New NamedColorPair("LightGoldenrodYellow", System.Windows.Media.Colors.LightGoldenrodYellow),
        New NamedColorPair("LightGray", System.Windows.Media.Colors.LightGray),
        New NamedColorPair("LightGreen", System.Windows.Media.Colors.LightGreen),
        New NamedColorPair("LightPink", System.Windows.Media.Colors.LightPink),
        New NamedColorPair("LightSalmon", System.Windows.Media.Colors.LightSalmon),
        New NamedColorPair("LightSeaGreen", System.Windows.Media.Colors.LightSeaGreen),
        New NamedColorPair("LightSkyBlue", System.Windows.Media.Colors.LightSkyBlue),
        New NamedColorPair("LightSlateGray", System.Windows.Media.Colors.LightSlateGray),
        New NamedColorPair("LightSteelBlue", System.Windows.Media.Colors.LightSteelBlue),
        New NamedColorPair("LightYellow", System.Windows.Media.Colors.LightYellow),
        New NamedColorPair("Lime", System.Windows.Media.Colors.Lime),
        New NamedColorPair("LimeGreen", System.Windows.Media.Colors.LimeGreen),
        New NamedColorPair("Linen", System.Windows.Media.Colors.Linen),
        New NamedColorPair("Magenta", System.Windows.Media.Colors.Magenta),
        New NamedColorPair("Maroon", System.Windows.Media.Colors.Maroon),
        New NamedColorPair("MediumAquamarine", System.Windows.Media.Colors.MediumAquamarine),
        New NamedColorPair("MediumBlue", System.Windows.Media.Colors.MediumBlue),
        New NamedColorPair("MediumOrchid", System.Windows.Media.Colors.MediumOrchid),
        New NamedColorPair("MediumPurple", System.Windows.Media.Colors.MediumPurple),
        New NamedColorPair("MediumSeaGreen", System.Windows.Media.Colors.MediumSeaGreen),
        New NamedColorPair("MediumSlateBlue", System.Windows.Media.Colors.MediumSlateBlue),
        New NamedColorPair("MediumSpringGreen", System.Windows.Media.Colors.MediumSpringGreen),
        New NamedColorPair("MediumTurquoise", System.Windows.Media.Colors.MediumTurquoise),
        New NamedColorPair("MediumVioletRed", System.Windows.Media.Colors.MediumVioletRed),
        New NamedColorPair("MidnightBlue", System.Windows.Media.Colors.MidnightBlue),
        New NamedColorPair("MintCream", System.Windows.Media.Colors.MintCream),
        New NamedColorPair("MistyRose", System.Windows.Media.Colors.MistyRose),
        New NamedColorPair("Moccasin", System.Windows.Media.Colors.Moccasin),
        New NamedColorPair("NavajoWhite", System.Windows.Media.Colors.NavajoWhite),
        New NamedColorPair("Navy", System.Windows.Media.Colors.Navy),
        New NamedColorPair("OldLace", System.Windows.Media.Colors.OldLace),
        New NamedColorPair("Olive", System.Windows.Media.Colors.Olive),
        New NamedColorPair("OliveDrab", System.Windows.Media.Colors.OliveDrab),
        New NamedColorPair("Orange", System.Windows.Media.Colors.Orange),
        New NamedColorPair("OrangeRed", System.Windows.Media.Colors.OrangeRed),
        New NamedColorPair("Orchid", System.Windows.Media.Colors.Orchid),
        New NamedColorPair("PaleGoldenrod", System.Windows.Media.Colors.PaleGoldenrod),
        New NamedColorPair("PaleGreen", System.Windows.Media.Colors.PaleGreen),
        New NamedColorPair("PaleTurquoise", System.Windows.Media.Colors.PaleTurquoise),
        New NamedColorPair("PaleVioletRed", System.Windows.Media.Colors.PaleVioletRed),
        New NamedColorPair("PapayaWhip", System.Windows.Media.Colors.PapayaWhip),
        New NamedColorPair("PeachPuff", System.Windows.Media.Colors.PeachPuff),
        New NamedColorPair("Peru", System.Windows.Media.Colors.Peru),
        New NamedColorPair("Pink", System.Windows.Media.Colors.Pink),
        New NamedColorPair("Plum", System.Windows.Media.Colors.Plum),
        New NamedColorPair("PowderBlue", System.Windows.Media.Colors.PowderBlue),
        New NamedColorPair("Purple", System.Windows.Media.Colors.Purple),
        New NamedColorPair("Red", System.Windows.Media.Colors.Red),
        New NamedColorPair("RosyBrown", System.Windows.Media.Colors.RosyBrown),
        New NamedColorPair("RoyalBlue", System.Windows.Media.Colors.RoyalBlue),
        New NamedColorPair("SaddleBrown", System.Windows.Media.Colors.SaddleBrown),
        New NamedColorPair("Salmon", System.Windows.Media.Colors.Salmon),
        New NamedColorPair("SandyBrown", System.Windows.Media.Colors.SandyBrown),
        New NamedColorPair("SeaGreen", System.Windows.Media.Colors.SeaGreen),
        New NamedColorPair("SeaShell", System.Windows.Media.Colors.SeaShell),
        New NamedColorPair("Sienna", System.Windows.Media.Colors.Sienna),
        New NamedColorPair("Silver", System.Windows.Media.Colors.Silver),
        New NamedColorPair("SkyBlue", System.Windows.Media.Colors.SkyBlue),
        New NamedColorPair("SlateBlue", System.Windows.Media.Colors.SlateBlue),
        New NamedColorPair("SlateGray", System.Windows.Media.Colors.SlateGray),
        New NamedColorPair("Snow", System.Windows.Media.Colors.Snow),
        New NamedColorPair("SpringGreen", System.Windows.Media.Colors.SpringGreen),
        New NamedColorPair("SteelBlue", System.Windows.Media.Colors.SteelBlue),
        New NamedColorPair("Tan", System.Windows.Media.Colors.Tan),
        New NamedColorPair("Teal", System.Windows.Media.Colors.Teal),
        New NamedColorPair("Thistle", System.Windows.Media.Colors.Thistle),
        New NamedColorPair("Tomato", System.Windows.Media.Colors.Tomato),
        New NamedColorPair("Transparent", System.Windows.Media.Colors.Transparent),
        New NamedColorPair("Turquoise", System.Windows.Media.Colors.Turquoise),
        New NamedColorPair("Violet", System.Windows.Media.Colors.Violet),
        New NamedColorPair("Wheat", System.Windows.Media.Colors.Wheat),
        New NamedColorPair("White", System.Windows.Media.Colors.White),
        New NamedColorPair("WhiteSmoke", System.Windows.Media.Colors.WhiteSmoke),
        New NamedColorPair("Yellow", System.Windows.Media.Colors.Yellow),
        New NamedColorPair("YellowGreen", System.Windows.Media.Colors.YellowGreen)
    }

    Private Sub DefinedInitComboBox()

        ' Remove the XAML items that were only for layout design.
        Me.DefinedComboBox.Items.Clear()

        Dim OptionPreviewColor As System.Windows.Media.SolidColorBrush
        Dim OptionTextColor As System.Windows.Media.SolidColorBrush

        For Each OneDefinedColor As NamedColorPair In DefinedAllColors

            ' Set up coloring for one option.
            OptionPreviewColor =
                New System.Windows.Media.SolidColorBrush(OneDefinedColor.Color)
            OptionTextColor = New System.Windows.Media.SolidColorBrush(
                OSNW.Graphics.ColorUtilities.ContrastingBW(OneDefinedColor.Color))

            ' Set up the label shown for this option.
            Dim NewLabel As New System.Windows.Controls.Label
            With NewLabel
                .Width = DEFINEDCOMBOBOXLABELWIDTH
                .Background = OptionPreviewColor
                .Foreground = OptionTextColor
                .Content = OneDefinedColor.Name
            End With

            ' Store the new option.
            Me.DefinedComboBox.Items.Add(NewLabel)

        Next

    End Sub ' DefinedInitComboBox

#End Region ' "Defined tab"

#Region "RGB tab"

    '''' <summary>
    '''' Update the visual items unique to this tab; prepare for the next
    '''' adjustment.
    '''' </summary>
    Private Sub RgbUpdateVisuals()

        With Me

            ' Update the labels around the image.
            Select Case Me.LastRgbChange
                Case LastRgbChangeEnum.Red
                    .RgbBaseLabel.Content =
                        $"{ByteValueStr(SHWV.REDWORD, .RgbWorkR)} Whole"
                    .RgbXLabel.Content = SHWV.GREENWORD
                    .RgbYLabel.Content = SHWV.BLUEWORD
                Case LastRgbChangeEnum.Green
                    .RgbBaseLabel.Content =
                        $"{ByteValueStr(SHWV.GREENWORD, .RgbWorkG)} Whole"
                    .RgbXLabel.Content = SHWV.REDWORD
                    .RgbYLabel.Content = SHWV.BLUEWORD
                Case Else
                    .RgbBaseLabel.Content =
                        $"{ByteValueStr(SHWV.BLUEWORD, .RgbWorkB)} Whole"
                    .RgbXLabel.Content = SHWV.REDWORD
                    .RgbYLabel.Content = SHWV.GREENWORD
            End Select

            ' Update the displayed per-component values.
            .TweakRedLabel.Content = .ByteValueStr(SHWV.REDWORD, .RgbWorkR)
            .TweakGreenLabel.Content = .ByteValueStr(SHWV.GREENWORD, .RgbWorkG)
            .TweakBlueLabel.Content = .ByteValueStr(SHWV.BLUEWORD, .RgbWorkB)

            ' Update the square.
            .RgbFillImage()

        End With

    End Sub ' RgbUpdateVisuals

    '''' <summary>
    '''' Respond to  click in the image.
    '''' </summary>
    Private Sub RgbProcessMouseClick(
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

            ' Determine which color that represents. Whole the image update on the
            ' predominant color that was clicked.
            Select Case .LastRgbChange
                Case LastRgbChangeEnum.Red
                    .UnderlyingG = CByte(PixelX)
                    .UnderlyingB = CByte(255 - PixelY)
                    .LastRgbChange = If(
                        .UnderlyingG > .UnderlyingB,
                        LastRgbChangeEnum.Green,
                        LastRgbChangeEnum.Blue)
                    Me.ResetRgbLastChange()
                Case LastRgbChangeEnum.Green
                    .UnderlyingR = CByte(PixelX)
                    .UnderlyingB = CByte(255 - PixelY)
                    .LastRgbChange = If(
                        .UnderlyingR > .UnderlyingB,
                        LastRgbChangeEnum.Red,
                        LastRgbChangeEnum.Blue)
                Case Else
                    .UnderlyingR = CByte(PixelX)
                    .UnderlyingG = CByte(255 - PixelY)
                    .LastRgbChange = If(
                        .UnderlyingR > .UnderlyingG,
                        LastRgbChangeEnum.Red,
                        LastRgbChangeEnum.Green)
            End Select

            .UpdateBaseValuesFromRGB(.UnderlyingR, .UnderlyingG, .UnderlyingB,
                                     .LastRgbChange)

            ' Update the displays.
            .RgbUpdateVisuals()

        End With

    End Sub ' RgbProcessMouseClick

    Private Sub RgbFillImage()

        ' These are Int32 to allow for left shift without exceptions.
        Dim RVal, GVal, BVal As System.Int32

        ' The constructed color for one pixel.
        Dim ConstructedColor As System.Int32

        ' Construct an array with colors for the individual pixels. Column is
        ' first here because of the way that the array offsets are processed in
        ' PixelsToImage.
        Dim PixelArray(CInt(Me.RgbImage.Width) - 1,
                       CInt(Me.RgbImage.Height) - 1) As System.Int32
        For Col As Int32 = 0 To PixelArray.GetUpperBound(0)
            For Row As Int32 = 0 To PixelArray.GetUpperBound(1)

                ' Update the working component values.
                Select Case Me.LastRgbChange
                    Case LastRgbChangeEnum.Red
                        RVal = Me.RgbWorkR
                        GVal = Col
                        BVal = 255 - Row
                    Case LastRgbChangeEnum.Green
                        RVal = Col
                        GVal = Me.RgbWorkG
                        BVal = 255 - Row
                    Case Else
                        RVal = Col
                        GVal = 255 - Row
                        BVal = Me.RgbWorkB
                End Select

                ConstructedColor = HFF + (RVal << 16) + (GVal << 8) + BVal
                PixelArray(Col, Row) = ConstructedColor

            Next
        Next

        Me.RgbImage.Source =
            OSNW.Graphics.ColorUtilities.PixelsToImageSource(PixelArray)

    End Sub ' RgbFillImage

#End Region ' "RGB tab"

#Region "HSx tabs"
    ' These are routines shared by HSL and HSV.

    ''' <summary>
    ''' Distributes the impact of the associated change.
    ''' </summary>
    ''' <param name="baseH">Specifies the hue component.</param>
    ''' <param name="baseS">Specifies the saturation component.</param>
    ''' <param name="baseL">Specifies the value component.</param>
    Private Sub UpdateBaseValuesFromHSL(ByVal baseH As System.Double,
        ByVal baseS As System.Double, ByVal baseL As System.Double)

        ' Set base color components and derived values.
        With Me

            OSNW.Graphics.ColorUtilities.HSLtoRGB(baseH, baseS, baseL,
                .UnderlyingR, .UnderlyingG, .UnderlyingB)

            .DeriveFromRGB(.UnderlyingR, .UnderlyingG, .UnderlyingB)
            .ResetRgbLastChange()
            OSNW.Graphics.ColorUtilities.RGBtoHSV(.UnderlyingR, .UnderlyingG,
                .UnderlyingB, .HsvWorkH, .HsvWorkS, .HsvWorkV)

        End With
    End Sub ' UpdateBaseValuesFromHSL

    ''' <summary>
    ''' Distributes the impact of the associated change.
    ''' </summary>
    ''' <param name="baseH">Specifies the hue component.</param>
    ''' <param name="baseS">Specifies the saturation component.</param>
    ''' <param name="baseV">Specifies the value component.</param>
    Private Sub UpdateBaseValuesFromHSV(ByVal baseH As System.Double,
        ByVal baseS As System.Double, ByVal baseV As System.Double)

        ' Set base color components and derived values.
        With Me

            OSNW.Graphics.ColorUtilities.HSVtoRGB(baseH, baseS, baseV,
                .UnderlyingR, .UnderlyingG, .UnderlyingB)

            .DeriveFromRGB(.UnderlyingR, .UnderlyingG, .UnderlyingB)
            .ResetRgbLastChange()

        End With
    End Sub ' UpdateBaseValuesFromHSV

    ''' <summary>
    ''' Identifies the mouse location within an Image and changes the cursor if
    ''' the mouse in in the desired circle.
    ''' </summary>
    Private Sub HsxSelectHueImageProcessMouseMove(
        sendingImage As System.Windows.Controls.Image, e As MouseEventArgs)

        ' FrameworkElement.Width/Height. The width/height of the element, in
        ' device-independent units (1/96th inch per unit). The default value is
        ' NaN. This value must be equal to or greater than 0.0.
        Dim ImageWidth As System.Double = sendingImage.Width
        Dim ImageHeight As System.Double = sendingImage.Height

        ' Calculate the center.
        Dim CenterW As System.Double = ImageWidth / 2.0
        Dim CenterH As System.Double = ImageHeight / 2.0

        ' Locate the mouse.
        Dim MousePoint As System.Windows.Point = e.GetPosition(sendingImage)
        Dim MouseX As System.Double = MousePoint.X
        Dim MouseY As System.Double = MousePoint.Y

        ' Calculate the mouse distance from the center point.
        ' Hypotenuse3 is reproduced here to avoid excess subroutine calls when
        ' this method is called from a loop.
        Dim MouseDeltaX As System.Double = MouseX - CenterW
        Dim MouseDeltaY As System.Double = MouseY - CenterH
        Dim MouseRadius As System.Double =
            System.Math.Sqrt(MouseDeltaX ^ 2 + MouseDeltaY ^ 2)

        ' Whole the radius on the smaller dimension of the rectangle.
        Dim ImageDiameter As System.Double =
            System.Math.Min(ImageWidth, ImageHeight)
        Dim ImageRadius As System.Double = ImageDiameter / 2.0

        ' Only react if the mouse was inside the circular area.
        If MouseRadius <= ImageRadius Then
            ' Set the cursor.
            sendingImage.Cursor = System.Windows.Input.Cursors.Cross
        Else
            ' Restore the cursor.
            If sendingImage.Equals(Me.HslSelectHueImage) Then
                Me.HslSelectHueImage.Cursor = Me.OriginalHslHueCursor
            Else
                Me.HsvSelectHueImage.Cursor = Me.OriginalHsvHueCursor
            End If
        End If

    End Sub ' HsxSelectHueImageProcessMouseMove

    '''' <summary>
    '''' Respond to a click in the specified (sendingImage) Hue image.
    '''' </summary>
    Private Sub HsxHueProcessMouseClick(
        sendingImage As System.Windows.Controls.Image,
        e As MouseButtonEventArgs)

        ' FrameworkElement.Width/Height. The width/height of the element, in
        ' device-independent units (1/96th inch per unit). The default value is
        ' NaN. This value must be equal to or greater than 0.0.
        Dim ImageWidth As System.Double = sendingImage.Width
        Dim ImageHeight As System.Double = sendingImage.Height

        '' FrameworkElement.ActualWidth/ActualHeight. Gets the rendered
        '' width/height of this element.
        '' The element's width/height, as a value in device-independent
        '' units (1/96th inch per unit).
        'Dim ImageActualWidth As System.Double = sendingHueImage.ActualWidth
        'Dim ImageActualHeight As System.Double = sendingHueImage.ActualHeight

        ' Calculate the center.
        Dim CenterW As System.Double = ImageWidth / 2.0
        Dim CenterH As System.Double = ImageHeight / 2.0

        ' Locate the mouse.
        Dim MousePoint As System.Windows.Point = e.GetPosition(sendingImage)
        Dim MouseX As System.Double = MousePoint.X
        Dim MouseY As System.Double = MousePoint.Y

        ' Calculate the mouse distance from the center point.
        ' Hypotenuse3 is reproduced here to avoid excess subroutine calls when
        ' this method is called from a loop.
        Dim MouseDeltaX As System.Double = MouseX - CenterW
        Dim MouseDeltaY As System.Double = MouseY - CenterH
        Dim MouseRadius As System.Double =
            System.Math.Sqrt(MouseDeltaX ^ 2 + MouseDeltaY ^ 2)

        ' Whole the radius on the smaller dimension of the rectangle.
        Dim ImageDiameter As System.Double =
            System.Math.Min(ImageWidth, ImageHeight)
        Dim ImageRadius As System.Double = ImageDiameter / 2.0

        ' Only react if the mouse was inside the circular area.
        If MouseRadius <= ImageRadius Then

            ' Determine the mouse pixel-position within the image.
            Dim MouseCol As System.Byte = CByte(System.Math.Floor(MouseX))
            Dim MouseRow As System.Byte = CByte(System.Math.Floor(MouseY))

            ' Determine which hue that pixel location represents. Calculate the
            ' hue as a fraction of a circle rotation.
            Dim SameWorkH As System.Double =
                OSNW.Graphics.ColorUtilities.GetHueFromPixel(MouseCol, MouseRow,
                    CInt(sendingImage.Width), CInt(sendingImage.Height),
                    OSNW.Graphics.ColorUtilities.HueScaleEnum.Fraction)
            If sendingImage.Equals(Me.HslSelectHueImage) Then
                HslWorkH = SameWorkH
            Else
                HsvWorkH = SameWorkH
            End If

        End If

        ' Create a color reflecting the selected component values.
        With Me
            If sendingImage.Equals(Me.HslSelectHueImage) Then
                .UpdateBaseValuesFromHSL(.HslWorkH, .HslWorkS, .HslWorkL)
            Else
                .UpdateBaseValuesFromHSV(.HsvWorkH, .HsvWorkS, .HsvWorkV)
            End If
        End With

        If sendingImage.Equals(Me.HslSelectHueImage) Then
            Me.HslUpdateVisuals()
        Else
            Me.HsvUpdateVisuals()
            'Else
            '
            '
            ' Do nothing?
            '
            '
        End If

    End Sub ' HsxHueProcessMouseClick

    '''' <summary>
    '''' Respond to a click in the specified (sendingImage) saturation vs.
    '''' luminance/value image.
    '''' </summary>
    '''' <param name="sendingImage">xxxxxxxxxxxxxxxxxxxxxxx</param>
    '''' <param name="e">xxxxxxxxxxxxxxxxxxxxxxx</param>
    Private Sub HsxSatProcessMouseClick(
        sendingImage As System.Windows.Controls.Image,
        e As MouseButtonEventArgs)

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

        ' FrameworkElement.Width/Height. The width/height of the element, in
        ' device-independent units (1/96th inch per unit). The default value is
        ' NaN. This value must be equal to or greater than 0.0.
        Dim ImageWidth As System.Double = sendingImage.Width
        Dim ImageHeight As System.Double = sendingImage.Height

        ' Determine the mouse position within the image.
        Dim MousePoint As System.Windows.Point = e.GetPosition(sendingImage)
        Dim MouseX As System.Double = MousePoint.X
        Dim MouseY As System.Double = MousePoint.Y

        ' Identify that as one pixel in an array of pixels.
        Dim PixelX As System.Byte = CByte(System.Math.Floor(MouseX))
        Dim PixelY As System.Byte = CByte(System.Math.Floor(MouseY))

        With Me

            ' Determine which components that represents.
            If sendingImage.Equals(Me.HslSelectSatLumImage) Then
                .HslWorkS = PixelX / ImageWidth
                .HslWorkL = (255.0 - PixelY) / ImageWidth
            Else
                .HsvWorkS = PixelX / ImageHeight
                .HsvWorkV = (255.0 - PixelY) / ImageHeight
            End If

            ' Create a color reflecting the selected component values.
            If sendingImage.Equals(Me.HslSelectSatLumImage) Then
                .UpdateBaseValuesFromHSL(.HslWorkH, .HslWorkS, .HslWorkL)
            Else
                .UpdateBaseValuesFromHSV(.HsvWorkH, .HsvWorkS, .HsvWorkV)
            End If

            ' Update the visual items unique to the associated tab; prepare for
            ' the next adjustment.
            If sendingImage.Equals(Me.HslSelectSatLumImage) Then
                .HslUpdateVisuals()
            Else
                .HsvUpdateVisuals()
            End If

        End With

    End Sub ' HsxSatProcessMouseClick

    Private Sub HsxFillImages(
        ByRef SelectHueImage As System.Windows.Controls.Image,
        ByRef SelectSatImage As System.Windows.Controls.Image)

        ' FrameworkElement.Width/Height. The width/height of the element, in
        ' device-independent units (1/96th inch per unit). The default value is
        ' NaN. This value must be equal to or greater than 0.0.
        Dim ImageWidth As System.Double = SelectHueImage.Width
        Dim ImageHeight As System.Double = SelectHueImage.Height

        ' Find the center.
        Dim CenterW As System.Double = ImageWidth / 2.0
        Dim CenterH As System.Double = ImageHeight / 2.0

        ' Whole the radius on the smaller dimension of the rectangle.
        ' Should this be based on .RenderSize vs .Width/.Height ????????
        Dim ImageDiameter As System.Double =
            System.Math.Min(ImageWidth, ImageHeight)
        Dim ImageRadius As System.Double = ImageDiameter / 2.0

        ' The constructed color for one pixel.
        Dim PixelHue As System.Double
        Dim PixelR, PixelG, PixelB As System.Double
        Dim PixelColor As System.Int32

        ' Update the hue selection image.
        ' Construct an array with colors for the individual pixels. Column is
        ' first here because of the way that the array offsets are processed in
        ' PixelsToImage.
        ' Should this be based on .RenderSize vs .Width/.Height ??????????
        Dim PixelArray(CInt(SelectHueImage.Width) - 1,
                       CInt(SelectHueImage.Height) - 1) As System.Int32
        For Col As System.Int32 = 0 To PixelArray.GetUpperBound(0)
            For Row As System.Int32 = 0 To PixelArray.GetUpperBound(1)

                ' Calculate the distance from the center point.
                ' Hypotenuse3 is reproduced here to avoid excess subroutine
                ' calls when this method is called from a loop.
                Dim PixelOffsetX As System.Double = Col - CenterW
                Dim PixelOffsetY As System.Double = Row - CenterH
                Dim PixelOffset As System.Double =
                    System.Math.Sqrt(PixelOffsetX ^ 2 + PixelOffsetY ^ 2)

                If PixelOffset <= ImageRadius Then
                    ' Construct and apply a color for one pixel.
                    PixelHue = OSNW.Graphics.ColorUtilities.GetHueFromPixel(Col,
                        Row, CInt(ImageWidth), CInt(ImageHeight),
                        OSNW.Graphics.ColorUtilities.HueScaleEnum.Fraction)
                    If SelectHueImage.Equals(Me.HslSelectHueImage) Then
                        OSNW.Graphics.ColorUtilities.HSLtoRGB(PixelHue,
                            Me.HslWorkS, Me.HslWorkL, PixelR, PixelG, PixelB)
                    Else
                        OSNW.Graphics.ColorUtilities.HSVtoRGB(PixelHue,
                            Me.HsvWorkS, Me.HsvWorkV, PixelR, PixelG, PixelB)
                    End If
                    PixelColor = HFF + (CInt(PixelR) << 16) +
                        (CInt(PixelG) << 8) + CInt(PixelB)
                    PixelArray(Col, Row) = PixelColor
                Else
                    ' Make the pixel transparent.
                    PixelColor = &H0
                    PixelArray(Col, Row) = PixelColor
                End If

            Next
        Next
        SelectHueImage.Source =
            OSNW.Graphics.ColorUtilities.PixelsToImageSource(PixelArray)

        ' Update the sat/lum or sat/val selection image.
        ' Construct an array with colors for the individual pixels. Column is
        ' first here because of the way that the array offsets are processed in
        ' PixelsToImage.
        ReDim PixelArray(CInt(SelectSatImage.Width) - 1,
                         CInt(SelectSatImage.Height) - 1)
        For Col As System.Int32 = 0 To PixelArray.GetUpperBound(0)
            For Row As System.Int32 = 0 To PixelArray.GetUpperBound(1)
                ' Construct and apply a color for one pixel.
                If SelectSatImage.Equals(Me.HslSelectSatLumImage) Then
                    OSNW.Graphics.ColorUtilities.HSLtoRGB(HslWorkH, Col / 255.0,
                        (255.0 - Row) / 255.0, PixelR, PixelG, PixelB)
                Else
                    OSNW.Graphics.ColorUtilities.HSVtoRGB(HsvWorkH, Col / 255.0,
                        (255.0 - Row) / 255.0, PixelR, PixelG, PixelB)
                End If
                PixelColor = HFF + (CInt(PixelR) << 16) +
                    (CInt(PixelG) << 8) + CInt(PixelB)
                PixelArray(Col, Row) = PixelColor
            Next
        Next

        If SelectSatImage.Equals(Me.HslSelectSatLumImage) Then
            Me.HslSelectSatLumImage.Source =
                OSNW.Graphics.ColorUtilities.PixelsToImageSource(PixelArray)
        Else
            Me.HsvSelectSatValImage.Source =
                OSNW.Graphics.ColorUtilities.PixelsToImageSource(PixelArray)
        End If

    End Sub ' HsxFillImages

#End Region ' "HSx tabs"

#Region "HSL tab"

    '''' <summary>
    '''' Update the visual items unique to this tab; prepare for the next
    '''' adjustment.
    '''' </summary>
    Private Sub HslUpdateVisuals()

        ' Update the displayed per-component values.
        With Me
            .HslTweakHueDegreesLabel.Content =
                .DoubleValueStr("Hue degrees", .HslWorkH * 360.0)
            .HslTweakHueFractionLabel.Content =
                .DoubleValueStr("Hue fraction", .HslWorkH)
            .HslTweakSaturationLabel.Content =
                .DoubleValueStr("Saturation", .HslWorkS)
            .HslTweakLuminanceLabel.Content =
                .DoubleValueStr("Luminance", .HslWorkL)
        End With

        Me.HsxFillImages(HslSelectHueImage, HslSelectSatLumImage)

    End Sub ' HslUpdateVisuals

#End Region ' "HSL tab"

#Region "HSV tab"

    '''' <summary>
    '''' Update the visual items unique to this tab; prepare for the next
    '''' adjustment.
    '''' </summary>
    Private Sub HsvUpdateVisuals()

        ' Update the displayed per-component values.
        With Me
            .HsvTweakHueDegreesLabel.Content =
                .DoubleValueStr("Hue degrees", HsvWorkH * 360.0)
            .HsvTweakHueFractionLabel.Content =
                .DoubleValueStr("Hue fraction", HsvWorkH)
            .HsvTweakSaturationLabel.Content =
                .DoubleValueStr("Saturation", HsvWorkS)
            .HsvTweakValueLabel.Content =
                .DoubleValueStr("Value", HsvWorkV)
        End With

        ' Update the square.
        Me.HsxFillImages(HsvSelectHueImage, HsvSelectSatValImage)

    End Sub ' HsvUpdateVisuals

#End Region ' "HSV tab"

End Class ' ColorDlgWindow

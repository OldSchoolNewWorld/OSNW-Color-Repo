Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports System.Windows
Imports Xunit

Namespace ColorUtilUnitTests

    Public Class HtmlUnitTests

        ' RGBtoHTML has bytes for parameters and any byte value is valid, so no
        ' bad-argument tests are needed.
        ' Test values: https://htmlcolorcodes.com/color-names/

        <Theory>
        <InlineData(205, 92, 92, "#CD5C5C")>
        <InlineData(255, 192, 203, "#FFC0CB")>
        <InlineData(255, 239, 213, "#FFEFD5")>
        <InlineData(65, 105, 225, "#4169E1")>
        <InlineData(107, 142, 35, "#6B8E23")>
        Sub RGBtoHTML_GoodInput_Succeeds(ByVal red As System.Byte, ByVal green As System.Byte,
            ByVal blue As System.Byte, ByVal expected As System.String)

            ' Test values: https://htmlcolorcodes.com/color-names/

            Dim ByteArray As System.Byte() = {red, green, blue}
            Dim Actual As System.String = System.String.Empty

            OSNW.Graphics.ColorUtilities.RGBtoHTML(red, green, blue, Actual)

            Assert.Equal(expected, Actual)

        End Sub

        <Theory>
        <InlineData("#A1B2C3D4", 161, 178, 195)> ' Too long.
        <InlineData("#A1B2C", 161, 178, 195)> ' Length is not zero or a multiple of 2
        <InlineData("#A1Q2C3", 177, 168, 88)> ' Contains a non-hex character.
        Sub HTMLtoRGB_BadInput_Fails(
            ByVal htmlIn As System.String, ByVal expectR As System.Byte,
            ByVal expectG As System.Byte, ByVal expectB As System.Byte)

            Dim ThrownEx As System.Exception

            ThrownEx = Assert.Throws(Of FormatException)(
                Sub()
                    OSNW.Graphics.ColorUtilities.HTMLtoRGB(
                        htmlIn, expectR, expectG, expectB)
                End Sub)

            Assert.True(TypeOf ThrownEx Is FormatException)

        End Sub

        '<Theory>

        <Theory>
        <InlineData("#FFC0CB", 255, 192, 203)> ' htmlcolorcodes
        <InlineData("#FFEFD5", 255, 239, 213)> ' htmlcolorcodes
        <InlineData("#4169E1", 65, 105, 225)> ' htmlcolorcodes
        <InlineData("#6B8E23", 107, 142, 35)> ' htmlcolorcodes
        <InlineData("#CD5C5C", 205, 92, 92)> ' htmlcolorcodes
        <InlineData("#5C5C", 0, 92, 92)> ' Short hex part.
        <InlineData("#5C", 0, 0, 92)> ' Short hex part.
        <InlineData("#", 0, 0, 0)> ' Short hex part.
        <InlineData("CD5C5C", 205, 92, 92)> ' No hashtag.
        <InlineData("#cd5C5C", 205, 92, 92)> ' Mixed case.
        Sub HTMLtoRGB_GoodInput_Succeeds(
            ByVal htmlIn As System.String, ByVal expectR As System.Byte,
            ByVal expectG As System.Byte, ByVal expectB As System.Byte)

            ' Test values: https://htmlcolorcodes.com/color-names/

            Dim ReturnR, ReturnG, ReturnB As System.Byte

            OSNW.Graphics.ColorUtilities.HTMLtoRGB(
                htmlIn, ReturnR, ReturnG, ReturnB)

            Assert.Equal(expectR, ReturnR)
            Assert.Equal(expectG, ReturnG)
            Assert.Equal(expectB, ReturnB)

        End Sub

    End Class ' HtmlUnitTests

    Public Class EasyUnitTests

        <Theory>
        <InlineData(-0.01, 0.0, 0.0)>
        <InlineData(0.0, -0.01, 0.0)>
        <InlineData(0.0, 0.0, -0.01)>
        <InlineData(255.01, 0.0, 0.0)>
        <InlineData(0.0, 255.01, 0.0)>
        <InlineData(0.0, 0.0, 255.01)>
        Sub EasyRGBtoHSL_BadInput_ForcesResults(ByVal redIn As System.Double,
            ByVal greenIn As System.Double, ByVal blueIn As System.Double)

            Const SMALLDIFF As System.Double = 0.001
            Dim Hue As System.Double
            Dim Saturation As System.Double
            Dim Luminance As System.Double

            OSNW.Graphics.ColorUtilities.RGBtoHSL(
                redIn, greenIn, blueIn, Hue, Saturation, Luminance)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDHSLH, Hue, SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDHSLS, Saturation, SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDHSLL, Luminance, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(64.0, 64.0, 64.0, 0.0, 0.0, 64.0 / 255.0)>    ' Try gray.
        <InlineData(128.0, 128.0, 128.0, 0.0, 0.0, 128.0 / 255.0)>
        <InlineData(192.0, 192.0, 192.0, 0.0, 0.0, 192.0 / 255.0)>
        <InlineData(106.0, 90.0, 205.0, 248.34782575163814 / 360.0,
                    0.53488371609480978, 0.57843137693171409)>    ' Try colored.
        <InlineData(183.0, 156.0, 20.0, 50.0 / 360.0, 0.8, 0.4)>
        <InlineData(182.0, 219.0, 162.0, 100.0 / 360.0, 0.45, 0.75)>
        <InlineData(Nothing, 255.0, 255.0, 180.0 / 360.0, 1.0, 0.5)>
        <InlineData(255.0, Nothing, 255.0, 300.0 / 360.0, 1.0, 0.5)>
        <InlineData(255.0, 255.0, Nothing, 60.0 / 360.0, 1.0, 0.5)>
        Public Sub EasyRGBtoHSL_GoodInput_Succeeds(ByVal redIn As System.Double,
            ByVal greenIn As System.Double, ByVal blueIn As System.Double,
            ByRef expectHue As System.Double,
            ByRef expectSaturation As System.Double,
            ByRef expectLuminance As System.Double)

            Const SMALLDIFF As System.Double = 0.01

            Dim HueOut As System.Double
            Dim SaturationOut As System.Double
            Dim LuminanceOut As System.Double

            OSNW.Graphics.ColorUtilities.RGBtoHSL(redIn, greenIn, blueIn,
                HueOut, SaturationOut, LuminanceOut)

            Assert.Equal(expectHue, HueOut, SMALLDIFF)
            Assert.Equal(expectSaturation, SaturationOut, SMALLDIFF)
            Assert.Equal(expectLuminance, LuminanceOut, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(-0.01, 0.0, 0.0)>
        <InlineData(0.0, -0.01, 0.0)>
        <InlineData(0.0, 0.0, -0.01)>
        <InlineData(1.01, 0.0, 0.0)>
        <InlineData(0.0, 1.01, 0.0)>
        <InlineData(0.0, 0.0, 1.01)>
        Sub EasyHSLtoRGB_BadInput_ForcesResults(ByVal hueIn As System.Double,
            ByVal saturationIn As System.Double,
            ByVal luminanceIn As System.Double)

            Const SMALLDIFF As System.Double = 0.001
            Dim Red As System.Double
            Dim Green As System.Double
            Dim Blue As System.Double

            OSNW.Graphics.ColorUtilities.HSLtoRGB(
                hueIn, saturationIn, luminanceIn, Red, Green, Blue)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, Red, SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBG, Green, SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBB, Blue, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(0.0, 0.0, 64.0 / 255.0, 64.0, 64.0, 64.0)>    ' Try gray.
        <InlineData(0.0, 0.0, 128.0 / 255.0, 128.0, 128.0, 128.0)>
        <InlineData(0.0, 0.0, 192.0 / 255.0, 192.0, 192.0, 192.0)>
        <InlineData(248.34782575163814 / 360.0, 0.53488371609480978,
                    0.57843137693171409, 106.0, 90.0, 205.0)>     ' Try colored.
        <InlineData(50.0 / 360.0, 0.8, 0.4, 183.0, 156.0, 20.0)>
        <InlineData(100.0 / 360.0, 0.45, 0.75, 182.0, 219.0, 162.0)>
        <InlineData(Nothing, 0.5, 0.5, 191.0, 64.0, 64.0)>
        <InlineData(0.5, Nothing, 0.5, 128.0, 128.0, 128.0)>
        <InlineData(0.5, 0.5, Nothing, 0.0, 0.0, 0.0)>
        Sub EasyHSLtoRGB_GoodInput_Succeeds(ByVal hueIn As System.Double,
            ByVal saturationIn As System.Double,
            ByVal luminanceIn As System.Double,
            ByRef expectRed As System.Double,
            ByRef expectGreen As System.Double,
            ByRef expectBlue As System.Double)

            Const SMALLDIFF As System.Double = 1.0
            Dim Red As System.Double
            Dim Green As System.Double
            Dim Blue As System.Double

            OSNW.Graphics.ColorUtilities.HSLtoRGB(
                hueIn, saturationIn, luminanceIn, Red, Green, Blue)

            Assert.Equal(expectRed, Red, SMALLDIFF)
            Assert.Equal(expectGreen, Green, SMALLDIFF)
            Assert.Equal(expectBlue, Blue, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(-0.01, 0.0, 0.0)>
        <InlineData(0.0, -0.01, 0.0)>
        <InlineData(0.0, 0.0, -0.01)>
        <InlineData(255.01, 0.0, 0.0)>
        <InlineData(0.0, 255.01, 0.0)>
        <InlineData(0.0, 0.0, 255.01)>
        Sub EasyRGBtoHSV_BadInput_ForcesResults(ByVal redIn As System.Double,
            ByVal greenIn As System.Double, ByVal blueIn As System.Double)

            Const SMALLDIFF As System.Double = 0.001
            Dim Hue As System.Double
            Dim Saturation As System.Double
            Dim Value As System.Double

            OSNW.Graphics.ColorUtilities.RGBtoHSV(
                redIn, greenIn, blueIn, Hue, Saturation, Value)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDHSVH, Hue, SMALLDIFF)
            Assert.Equal(
                OSNW.Graphics.ColorUtilities.FORCEDHSVS, Saturation, SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDHSVV, Value, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(64.0, 64.0, 64.0, 0.0, 0.0, 0.25)>            ' Try gray.
        <InlineData(128.0, 128.0, 128.0, 0.0, 0.0, 0.5)>
        <InlineData(192.0, 192.0, 192.0, 0.0, 0.0, 0.75)>
        <InlineData(106.0, 90.0, 205.0, 248.34782575163814 / 360.0,
                    0.56097560975609762, 0.803921568627451)>      ' Try colored.
        <InlineData(183.0, 156.0, 20.0, 50.0 / 360.0, 0.89, 0.72)>
        <InlineData(182.0, 219.0, 162.0, 99.0 / 360.0, 0.26, 0.86)>
        <InlineData(Nothing, 255.0, 255.0, 180.0 / 360.0, 1.0, 1.0)>
        <InlineData(255.0, Nothing, 255.0, 300.0 / 360.0, 1.0, 1.0)>
        <InlineData(255.0, 255.0, Nothing, 60.0 / 360.0, 1.0, 1.0)>
        Sub EasyRGBtoHSV_GoodInput_Succeeds(ByVal redIn As System.Double,
            ByVal greenIn As System.Double, ByVal blueIn As System.Double,
            ByRef ExpectHue As System.Double,
            ByRef ExpectSaturation As System.Double,
            ByRef ExpectValue As System.Double)

            Const SMALLDIFF As System.Double = 0.01
            Dim Hue As System.Double
            Dim Saturation As System.Double
            Dim Value As System.Double

            OSNW.Graphics.ColorUtilities.RGBtoHSV(redIn, greenIn, blueIn,
                                                  Hue, Saturation, Value)

            Assert.Equal(ExpectHue, Hue, SMALLDIFF)
            Assert.Equal(ExpectSaturation, Saturation, SMALLDIFF)
            Assert.Equal(ExpectValue, Value, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(-0.01, 0.0, 0.0)>
        <InlineData(0.0, -0.01, 0.0)>
        <InlineData(0.0, 0.0, -0.01)>
        <InlineData(1.01, 0.0, 0.0)>
        <InlineData(0.0, 1.01, 0.0)>
        <InlineData(0.0, 0.0, 1.01)>
        Sub EasyHSVtoRGB_BadInput_ForcesResults(ByVal hueIn As System.Double,
            ByVal saturationIn As System.Double, ByVal valueIn As System.Double)

            Const SMALLDIFF As System.Double = 0.001
            Dim Red As System.Double
            Dim Green As System.Double
            Dim Blue As System.Double

            OSNW.Graphics.ColorUtilities.HSVtoRGB(
                hueIn, saturationIn, valueIn, Red, Green, Blue)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, Red, SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBG, Green, SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBB, Blue, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(0.0, 0.0, 0.25, 64.0, 64.0, 64.0)>            ' Try gray.
        <InlineData(0.0, 0.0, 0.5, 128.0, 128.0, 128.0)>
        <InlineData(0.0, 0.0, 0.75, 192.0, 192.0, 192.0)>
        <InlineData(248.34782575163814 / 360.0, 0.56097560975609762,
                    0.803921568627451, 106.0, 90.0, 205.0)>       ' Try colored.
        <InlineData(50.0 / 360.0, 0.89, 0.72, 183.0, 156.0, 20.0)>
        <InlineData(99.0 / 360.0, 0.26, 0.86, 182.0, 219.0, 162.0)>
        <InlineData(Nothing, 0.5, 0.5, 128.0, 64.0, 64.0)>
        <InlineData(0.5, Nothing, 0.5, 128.0, 128.0, 128.0)>
        <InlineData(0.5, 0.5, Nothing, 0.0, 0.0, 0.0)>
        Sub EasyHSVtoRGB_GoodInput_Succeeds(ByVal hueIn As System.Double,
            ByVal saturationIn As System.Double, ByVal valueIn As System.Double,
            ByRef ExpectRed As System.Double, ByRef ExpectGreen As System.Double,
            ByRef ExpectBlue As System.Double)

            Const SMALLDIFF As System.Double = 1.0
            Dim Red As System.Double
            Dim Green As System.Double
            Dim Blue As System.Double

            OSNW.Graphics.ColorUtilities.HSVtoRGB(hueIn, saturationIn, valueIn,
                                                  Red, Green, Blue)

            Assert.Equal(ExpectRed, Red, SMALLDIFF)
            Assert.Equal(ExpectGreen, Green, SMALLDIFF)
            Assert.Equal(ExpectBlue, Blue, SMALLDIFF)

        End Sub

    End Class ' EasyUnitTests

End Namespace ' ColorUtilUnitTests

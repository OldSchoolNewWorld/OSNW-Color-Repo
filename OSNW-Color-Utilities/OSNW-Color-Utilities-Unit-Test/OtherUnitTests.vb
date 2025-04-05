Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports OSNW
Imports Xunit

Namespace ColorUtilUnitTests

    ' NOTE: OSNW-Color-Utilities-Unit-Test.vbproj needs the UseWPF option to use the
    ' System.Windows.Media Namespace.
    '   <PropertyGroup>
    '     <UseWPF>true</UseWPF>
    '   </PropertyGroup>

    Public Class OtherUnitTests

        <Theory>
        <InlineData(1.0, 2.0, 3.0, 5.0)>
        <InlineData(2.0, 3.0, 1.0, 5.0)>
        <InlineData(3.0, 1.0, 2.0, 5.0)>
        Sub Hypotenuse3_OneNearbyPoint_InRange(
            ByVal x As System.Double, ByVal y As System.Double,
            ByVal z As System.Double, ByVal maxDiff As System.Double)

            Dim Distance As System.Double =
                OSNW.Graphics.ColorUtilities.Hypotenuse3(x, y, z)

            Assert.True(Distance <= maxDiff)

        End Sub

        <Theory>
        <InlineData(10.0, 11.0, 12.0, 5.0)>
        <InlineData(11.0, 12.0, 10.0, 5.0)>
        <InlineData(12.0, 10.0, 11.0, 5.0)>
        Sub Hypotenuse3_OneDistantPoint_ExceedsRange(
            ByVal x As System.Double, ByVal y As System.Double,
            ByVal z As System.Double, ByVal maxDiff As System.Double)

            Dim Distance As System.Double =
                OSNW.Graphics.ColorUtilities.Hypotenuse3(x, y, z)

            Assert.False(Distance <= maxDiff)

        End Sub

        <Theory>
        <InlineData(1.0, 2.0, 3.0, 1.5, 2.5, 3.5, 5.0)>
        <InlineData(2.0, 3.0, 1.0, 2.0, 3.0, 1.0, 5.0)>
        <InlineData(3.0, 1.0, 2.0, 3.0, 1.0, 2.0, 5.0)>
        Sub Hypotenuse3_TwoNearbyPoints_InRange(
            ByVal x1 As System.Double, ByVal y1 As System.Double,
            ByVal z1 As System.Double, ByVal x2 As System.Double,
            ByVal y2 As System.Double, ByVal z2 As System.Double,
            ByVal maxDiff As System.Double)

            Dim Distance As System.Double =
                OSNW.Graphics.ColorUtilities.Hypotenuse3(x1, y1, z1, x2, y2, z2)

            Assert.True(Distance <= maxDiff)

        End Sub

        <Theory>
        <InlineData(10.0, 11.0, 12.0, 15.0, 16.0, 17.0, 5.0)>
        <InlineData(11.0, 12.0, 10.0, 16.0, 17.0, 15.0, 5.0)>
        <InlineData(12.0, 10.0, 11.0, 17.0, 15.0, 16.0, 5.0)>
        Sub Hypotenuse3_TwoDistantPoints_ExceedRange(
            ByVal x1 As System.Double, ByVal y1 As System.Double,
            ByVal z1 As System.Double, ByVal x2 As System.Double,
            ByVal y2 As System.Double, ByVal z2 As System.Double,
            ByVal maxDiff As System.Double)

            Dim Distance As System.Double =
                OSNW.Graphics.ColorUtilities.Hypotenuse3(x1, y1, z1, x2, y2, z2)

            Assert.False(Distance <= maxDiff)

        End Sub

#Region "Blend"

        <Theory>
        <InlineData(-0.01, 192.0, 192.0, 1.0, 192.0, 192.0, 192.0, 1.0)> ' Too low.
        <InlineData(192.0, -0.01, 192.0, 1.0, 192.0, 192.0, 192.0, 1.0)>
        <InlineData(192.0, 192.0, -0.01, 1.0, 192.0, 192.0, 192.0, 1.0)>
        <InlineData(192.0, 192.0, 192.0, 1.0, -0.01, 192.0, 192.0, 1.0)>
        <InlineData(192.0, 192.0, 192.0, 1.0, 192.0, -0.01, 192.0, 1.0)>
        <InlineData(192.0, 192.0, 192.0, 1.0, 192.0, 192.0, -0.01, 1.0)>
        <InlineData(192.0, 192.0, 192.0, -0.01, 192.0, 192.0, 192.0, 1.0)>
        <InlineData(192.0, 192.0, 192.0, 1.0, 192.0, 192.0, 192.0, -0.01)>
        <InlineData(255.01, 192.0, 192.0, 1.0, 192.0, 192.0, 192.0, 1.0)> ' Too high.
        <InlineData(192.0, 255.01, 192.0, 1.0, 192.0, 192.0, 192.0, 1.0)>
        <InlineData(192.0, 192.0, 255.01, 1.0, 192.0, 192.0, 192.0, 1.0)>
        <InlineData(192.0, 192.0, 192.0, 1.0, 255.01, 192.0, 192.0, 1.0)>
        <InlineData(192.0, 192.0, 192.0, 1.0, 192.0, 255.01, 192.0, 1.0)>
        <InlineData(192.0, 192.0, 192.0, 1.0, 192.0, 192.0, 255.01, 1.0)>
        Sub GetBlend_BadInput_ForcesResults(
            color1R As System.Double, color1G As System.Double,
            color1B As System.Double, color2R As System.Double,
            color2G As System.Double, color2B As System.Double,
            color1Proportion As System.Double,
            color2Proportion As System.Double)

            Const SMALLDIFF As System.Double = 0.1
            Dim ResultR, ResultG, ResultB As System.Double

            OSNW.Graphics.ColorUtilities.GetBlend(color1R, color1G, color1B,
                color2R, color2G, color2B, color1Proportion,
                color2Proportion, ResultR, ResultG, ResultB)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultR,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBG, ResultG,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBB, ResultB,
                         SMALLDIFF)

        End Sub

        ' The math for this seems to work ok, but the outcomes do not match the
        ' classic idea that red plus green yields yellow. The result is muted.
        ' The zeroes get averaged in.
        ' Compare results at https://www.colorhexa.com/color-blend.
        <Theory>
        <InlineData(255.0, 0.0, 0.0, 1.0, 0.0, 255.0, 0.0, 1.0, 127.5, 127.5, 0.0)> ' match
        <InlineData(255.0, 0.0, 0.0, 1.0, 0.0, 0.0, 255.0, 1.0, 127.5, 0.0, 127.5)> ' match
        <InlineData(0.0, 255.0, 0.0, 1.0, 0.0, 0.0, 255.0, 1.0, 0.0, 127.5, 127.5)> ' match
        <InlineData(192.0, 64.0, 64.0, 1.0, 64.0, 192.0, 64.0, 1.0, 128.0, 128.0, 64.0)> ' match
        <InlineData(192.0, 64.0, 64.0, 1.0, 64.0, 64.0, 192.0, 1.0, 128.0, 64.0, 128.0)>
        <InlineData(64.0, 192.0, 64.0, 1.0, 64.0, 64.0, 192.0, 1.0, 64.0, 128.0, 128.0)>
        <InlineData(255.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 127.5, 0.0, 0.0)> ' match
        <InlineData(0.0, 255.0, 0.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 127.5, 0.0)>
        <InlineData(0.0, 0.0, 255.0, 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 127.5)>
        <InlineData(255.0, 255.0, 255.0, 1.0, 0.0, 0.0, 0.0, 1.0, 127.5, 127.5, 127.5)> ' match
        Sub GetBlend_GoodInput_Succeeds(
            color1R As System.Double, color1G As System.Double,
            color1B As System.Double, color1Proportion As System.Double,
            color2R As System.Double, color2G As System.Double,
            color2B As System.Double, color2Proportion As System.Double,
            expectedR As System.Double, expectedG As System.Double,
            expectedB As System.Double)

            Const SMALLDIFF As System.Double = 0.1
            Dim ResultR, ResultG, ResultB As System.Double

            OSNW.Graphics.ColorUtilities.GetBlend(color1R, color1G, color1B,
                color1Proportion, color2R, color2G, color2B, color2Proportion,
                ResultR, ResultG, ResultB)

            Assert.Equal(expectedR, ResultR, SMALLDIFF)
            Assert.Equal(expectedG, ResultG, SMALLDIFF)
            Assert.Equal(expectedB, ResultB, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(192, 192, 192, -0.1, 192, 192, 192, 1)> ' Too low.
        <InlineData(192, 192, 192, 1, 192, 192, 192, -0.1)> ' Too low.
        <InlineData(192, 192, 192, 0.0, 192, 192, 192, 0.0)> ' Both zero.
        Sub GetBlendBytes_BadInput_ForcesResults(
            color1R As System.Byte, color1G As System.Byte,
            color1B As System.Byte, color1Proportion As System.Double,
            color2R As System.Byte, color2G As System.Byte,
            color2B As System.Byte, color2Proportion As System.Double)

            Dim ResultR, ResultG, ResultB As System.Byte

            OSNW.Graphics.ColorUtilities.GetBlend(color1R, color1G, color1B,
                color1Proportion, color2R, color2G, color2B, color2Proportion,
                ResultR, ResultG, ResultB)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultR)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBG, ResultG)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBB, ResultB)

        End Sub

        ' The math for this seems to work ok, but the outcomes do not match the
        ' classic idea that red plus green yields yellow. The result is muted.
        ' The zeroes get averaged in.
        <Theory>
        <InlineData(255, 0, 0, 1, 0, 255, 0, 1, 128, 128, 0)>
        <InlineData(255, 0, 0, 1, 0, 0, 255, 1, 128, 0, 128)>
        <InlineData(0, 255, 0, 1, 0, 0, 255, 1, 0, 128, 128)>
        <InlineData(192, 64, 64, 1, 64, 192, 64, 1, 128, 128, 64)>
        <InlineData(192, 64, 64, 1, 64, 64, 192, 1, 128, 64, 128)>
        <InlineData(64, 192, 64, 1, 64, 64, 192, 1, 64, 128, 128)>
        <InlineData(255, 0, 0, 1, 0, 0, 0, 1, 128, 0, 0)>
        <InlineData(0, 255, 0, 1, 0, 0, 0, 1, 0, 128, 0)>
        <InlineData(0, 0, 255, 1, 0, 0, 0, 1, 0, 0, 128)>
        <InlineData(255, 255, 255, 1, 0, 0, 0, 1, 128, 128, 128)>
        Sub GetBlendBytes_GoodInput_Succeeds(
            color1R As System.Byte, color1G As System.Byte,
            color1B As System.Byte, color1Proportion As System.Double,
            color2R As System.Byte, color2G As System.Byte,
            color2B As System.Byte, color2Proportion As System.Double,
            expectedR As System.Byte, expectedG As System.Byte,
            expectedB As System.Byte)

            Dim ResultR, ResultG, ResultB As System.Byte

            OSNW.Graphics.ColorUtilities.GetBlend(color1R, color1G, color1B,
                color1Proportion, color2R, color2G, color2B, color2Proportion,
                ResultR, ResultG, ResultB)

            Assert.Equal(expectedR, ResultR)
            Assert.Equal(expectedG, ResultG)
            Assert.Equal(expectedB, ResultB)

        End Sub

        <Theory>
        <InlineData(192, 192, 192, -0.01, 192, 192, 192, 1.0)> ' Too low.
        <InlineData(192, 192, 192, 1.0, 192, 192, 192, -0.01)>
        <InlineData(192, 192, 192, 0.0, 192, 192, 192, 0.0)> ' Both zero.
        Sub GetBlendColor_BadInput_ForcesResults(color1R As System.Byte,
            color1G As System.Byte, color1B As System.Byte,
            color1Proportion As System.Double, color2R As System.Byte,
            color2G As System.Byte, color2B As System.Byte,
            color2Proportion As System.Double)

            Dim Color1 As System.Windows.Media.Color =
                System.Windows.Media.Color.FromRgb(color1R, color1G, color1B)
            Dim Color2 As System.Windows.Media.Color =
                System.Windows.Media.Color.FromRgb(color2R, color2G, color2B)

            Dim ResultC As System.Windows.Media.Color =
                OSNW.Graphics.ColorUtilities.GetBlend(Color1, color1Proportion,
                                                  Color2, color2Proportion)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultC.R)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBG, ResultC.G)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBB, ResultC.B)

        End Sub

        <Theory>
        <InlineData(255, 0, 0, 1.0, 0, 255, 0, 1.0, 128, 128, 0)>
        <InlineData(255, 0, 0, 1.0, 0, 0, 255, 1.0, 128, 0, 128)>
        <InlineData(0, 255, 0, 1.0, 0, 0, 255, 1.0, 0, 128, 128)>
        <InlineData(192, 64, 64, 1.0, 64, 192, 64, 1.0, 128, 128, 64)>
        <InlineData(192, 64, 64, 1.0, 64, 64, 192, 1.0, 128, 64, 128)>
        <InlineData(64, 192, 64, 1.0, 64, 64, 192, 1.0, 64, 128, 128)>
        <InlineData(255, 0, 0, 1.0, 0, 0, 0, 1.0, 128, 0, 0)>
        <InlineData(0, 255, 0, 1.0, 0, 0, 0, 1.0, 0, 128, 0)>
        <InlineData(0, 0, 255, 1.0, 0, 0, 0, 1.0, 0, 0, 128)>
        <InlineData(255, 255, 255, 1.0, 0, 0, 0, 1.0, 128, 128, 128)>
        Sub GetBlendColor_GoodInput_Succeeds(color1R As System.Byte,
            color1G As System.Byte, color1B As System.Byte,
            color1Proportion As System.Double, color2R As System.Byte,
            color2G As System.Byte, color2B As System.Byte,
            color2Proportion As System.Double, expectedR As System.Double,
            expectedG As System.Double, expectedB As System.Double)

            Dim Color1 As System.Windows.Media.Color =
                System.Windows.Media.Color.FromRgb(color1R, color1G, color1B)
            Dim Color2 As System.Windows.Media.Color =
                System.Windows.Media.Color.FromRgb(color2R, color2G, color2B)

            Dim ResultC As System.Windows.Media.Color =
                OSNW.Graphics.ColorUtilities.GetBlend(Color1, color1Proportion,
                                                  Color2, color2Proportion)

            Assert.Equal(expectedR, ResultC.R)
            Assert.Equal(expectedG, ResultC.G)
            Assert.Equal(expectedB, ResultC.B)

        End Sub

#End Region ' "Blend"

        <Theory>
        <InlineData(-0.01, 192, 192, 0.5)>
        <InlineData(192, -0.01, 192, 0.5)>
        <InlineData(192, 192, -0.01, 0.5)>
        <InlineData(255.01, 192, 192, 0.5)>
        <InlineData(192, 255.01, 192, 0.5)>
        <InlineData(192, 192, 255.01, 0.5)>
        <InlineData(192, 192, 192, -0.01)>
        <InlineData(192, 192, 192, 1.01)>
        Sub GetShadeRGB_BadInput_ForcesResults(
            ByVal colorInR As System.Double, ByVal colorInG As System.Double,
            ByVal colorInB As System.Double, ByVal shadeFactor As System.Double)

            Const SMALLDIFF As System.Double = 0.1
            Dim ResultR As System.Double
            Dim ResultG As System.Double
            Dim ResultB As System.Double

            OSNW.Graphics.ColorUtilities.GetShade(colorInR, colorInG, colorInB,
                shadeFactor, ResultR, ResultG, ResultB)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultR,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultG,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultB,
                         SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(255.0, 0.0, 0.0, 0.1, 229.5, 0.0, 0.0, 0.1)> ' Float.
        <InlineData(255.0, 0.0, 0.0, 0.1, 230.0, 0.0, 0.0, 0.5)> ' maketintsandshades.
        <InlineData(0.0, 255.0, 0.0, 0.1, 0.0, 229.5, 0.0, 0.1)>
        <InlineData(0.0, 0.0, 255.0, 0.1, 0.0, 0.0, 229.5, 0.1)>
        <InlineData(255.0, 0.0, 0.0, 0.5, 127.5, 0.0, 0.0, 0.1)> ' Float.
        <InlineData(255.0, 0.0, 0.0, 0.5, 128.0, 0.0, 0.0, 0.5)> ' maketintsandshades.
        <InlineData(0.0, 255.0, 0.0, 0.5, 0.0, 127.5, 0.0, 0.1)>
        <InlineData(0.0, 0.0, 255.0, 0.5, 0.0, 0.0, 127.5, 0.1)>
        <InlineData(255.0, 0.0, 0.0, 0.9, 25.5, 0.0, 0.0, 0.1)> ' Float.
        <InlineData(255.0, 0.0, 0.0, 0.9, 25.0, 0.0, 0.0, 0.5)> ' maketintsandshades.
        <InlineData(0.0, 255.0, 0.0, 0.9, 0.0, 25.5, 0.0, 0.1)>
        <InlineData(0.0, 0.0, 255.0, 0.9, 0.0, 0.0, 25.5, 0.1)>
        Sub GetShadeRGB_GoodInput_Succeeds(
            ByVal colorInR As System.Double, ByVal colorInG As System.Double,
            ByVal colorInB As System.Double, ByVal shadeFactor As System.Double,
            ByRef expectR As System.Double, ByRef expectG As System.Double,
            ByRef expectB As System.Double, ByVal allowance As System.Double)

            Dim ResultR, ResultG, ResultB As System.Double

            OSNW.Graphics.ColorUtilities.GetShade(colorInR, colorInG, colorInB,
                shadeFactor, ResultR, ResultG, ResultB)

            Assert.Equal(expectR, ResultR, allowance)
            Assert.Equal(expectG, ResultG, allowance)
            Assert.Equal(expectB, ResultB, allowance)

        End Sub

        <Theory>
        <InlineData(192, 192, 192, -0.01)>
        <InlineData(192, 192, 192, 1.01)>
        Sub GetShadeColors_BadInput_ForcesResults(
            ByVal colorInR As System.Byte, ByVal colorInG As System.Byte,
            ByVal colorInB As System.Byte, ByVal shadeFactor As System.Double)

            Const SMALLDIFF As System.Double = 0.1
            Dim ColorIn As System.Windows.Media.Color =
                System.Windows.Media.Color.FromRgb(colorInR, colorInG, colorInB)
            Dim ColorOut As System.Windows.Media.Color =
                OSNW.Graphics.ColorUtilities.GetShade(ColorIn, shadeFactor)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ColorOut.R,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ColorOut.G,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ColorOut.B,
                         SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(255.0, 0.0, 0.0, 0.1, 229.5, 0.0, 0.0)> ' Float.
        <InlineData(255.0, 0.0, 0.0, 0.1, 229.5, 0.0, 0.0)> ' maketintsandshades.
        <InlineData(0.0, 255.0, 0.0, 0.1, 0.0, 229.5, 0.0)>
        <InlineData(0.0, 0.0, 255.0, 0.1, 0.0, 0.0, 229.5)>
        <InlineData(255.0, 0.0, 0.0, 0.5, 127.5, 0.0, 0.0)> ' Float.
        <InlineData(255.0, 0.0, 0.0, 0.5, 128.0, 0.0, 0.0)> ' maketintsandshades.
        <InlineData(0.0, 255.0, 0.0, 0.5, 0.0, 127.5, 0.0)>
        <InlineData(0.0, 0.0, 255.0, 0.5, 0.0, 0.0, 127.5)>
        <InlineData(255.0, 0.0, 0.0, 0.9, 25.5, 0.0, 0.0)> ' Float.
        <InlineData(255.0, 0.0, 0.0, 0.9, 25.0, 0.0, 0.0)> ' maketintsandshades.
        <InlineData(0.0, 255.0, 0.0, 0.9, 0.0, 25.5, 0.0)>
        <InlineData(0.0, 0.0, 255.0, 0.9, 0.0, 0.0, 25.5)>
        Sub GetShadeColors_GoodInput_Succeeds(
            ByVal colorInR As System.Byte, ByVal colorInG As System.Byte,
            ByVal colorInB As System.Byte, ByVal shadeFactor As System.Double,
            ByRef expectR As System.Double, ByRef expectG As System.Double,
            ByRef expectB As System.Double)

            Const SMALLDIFF As System.Double = 1.0
            Dim ColorIn As System.Windows.Media.Color =
                System.Windows.Media.Color.FromRgb(colorInR, colorInG, colorInB)
            Dim ColorOut As System.Windows.Media.Color

            ColorOut = OSNW.Graphics.ColorUtilities.GetShade(ColorIn, shadeFactor)

            Assert.Equal(expectR, ColorOut.R, SMALLDIFF)
            Assert.Equal(expectG, ColorOut.G, SMALLDIFF)
            Assert.Equal(expectB, ColorOut.B, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(192, 192, 192, -0.01)>
        <InlineData(192, 192, 192, 1.01)>
        Sub GetTintRGB_BadInput_ForcesResults(
            ByVal colorInR As System.Double, ByVal colorInG As System.Double,
            ByVal colorInB As System.Double, ByVal tintFactor As System.Double)

            Const SMALLDIFF As System.Double = 0.1
            Dim ResultR, ResultG, ResultB As System.Double

            OSNW.Graphics.ColorUtilities.GetTint(colorInR, colorInG, colorInB,
                tintFactor, ResultR, ResultG, ResultB)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultR,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultG,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultB,
                         SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(255.0, 0.0, 0.0, 0.1, 255.0, 25.5, 25.5, 0.01)> ' Float.
        <InlineData(255.0, 0.0, 0.0, 0.1, 255.0, 26.0, 26.0, 0.5)> ' maketintsandshades.
        <InlineData(0.0, 255.0, 0.0, 0.1, 25.5, 255.0, 25.5, 0.01)>
        <InlineData(0.0, 0.0, 255.0, 0.1, 25.5, 25.5, 255.0, 0.01)>
        <InlineData(255.0, 0.0, 0.0, 0.5, 255.0, 127.5, 127.5, 0.01)> ' Float.
        <InlineData(255.0, 0.0, 0.0, 0.5, 255.0, 128.0, 128.0, 0.5)> ' maketintsandshades.
        <InlineData(0.0, 255.0, 0.0, 0.5, 127.5, 255.0, 127.5, 0.01)>
        <InlineData(0.0, 0.0, 255.0, 0.5, 127.5, 127.5, 255.0, 0.01)>
        <InlineData(255.0, 0.0, 0.0, 0.9, 255.0, 229.5, 229.5, 0.01)> ' Float.
        <InlineData(255.0, 0.0, 0.0, 0.9, 255.0, 230.0, 230.0, 0.5)> ' maketintsandshades.
        <InlineData(0.0, 255.0, 0.0, 0.9, 229.5, 255.0, 229.5, 0.01)>
        <InlineData(0.0, 0.0, 255.0, 0.9, 229.5, 229.5, 255.0, 0.01)>
        Sub GetTintRGB_GoodInput_Succeeds(
            ByVal colorInR As System.Double, ByVal colorInG As System.Double,
            ByVal colorInB As System.Double, ByVal tintFactor As System.Double,
            ByRef expectR As System.Double, ByRef expectG As System.Double,
            ByRef expectB As System.Double, ByVal allowance As System.Double)

            ' Test values: https://maketintsandshades.com/#ff0000,008088

            Dim ResultR, ResultG, ResultB As System.Double

            OSNW.Graphics.ColorUtilities.GetTint(colorInR, colorInG, colorInB,
                tintFactor, ResultR, ResultG, ResultB)

            Assert.Equal(expectR, ResultR, allowance)
            Assert.Equal(expectG, ResultG, allowance)
            Assert.Equal(expectB, ResultB, allowance)

        End Sub

        <Theory>
        <InlineData(192, 192, 192, -0.01)>
        <InlineData(192, 192, 192, 1.01)>
        Sub GetTintColors_BadInput_ForcesResults(
            ByVal colorInR As System.Byte, ByVal colorInG As System.Byte,
            ByVal colorInB As System.Byte, ByVal tintFactor As System.Double)

            Const SMALLDIFF As System.Double = 0.1
            Dim ColorIn As System.Windows.Media.Color =
                System.Windows.Media.Color.FromRgb(colorInR, colorInG, colorInB)
            Dim ResultC As System.Windows.Media.Color =
                OSNW.Graphics.ColorUtilities.GetTint(ColorIn, tintFactor)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultC.R,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultC.G,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultC.B,
                         SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(255, 0, 0, 0.1, 255.0, 25.5, 25.5)> ' Float.
        <InlineData(255, 0, 0, 0.1, 255.0, 25.5, 25.5)> ' maketintsandshades.
        <InlineData(0, 255, 0, 0.1, 25.5, 255.0, 25.5)>
        <InlineData(0, 0, 255, 0.1, 25.5, 25.5, 255.0)>
        <InlineData(255, 0, 0, 0.5, 255.0, 127.5, 127.5)> ' Float.
        <InlineData(255, 0, 0, 0.5, 255.0, 127.5, 127.5)> ' maketintsandshades.
        <InlineData(0, 255, 0, 0.5, 127.5, 255.0, 127.5)>
        <InlineData(0, 0, 255, 0.5, 127.5, 127.5, 255.0)>
        <InlineData(255, 0, 0, 0.9, 255.0, 229.5, 229.5)> ' Float.
        <InlineData(255, 0, 0, 0.9, 255.0, 229.5, 229.5)> ' maketintsandshades.
        <InlineData(0, 255, 0, 0.9, 229.5, 255.0, 229.5)>
        <InlineData(0, 0, 255, 0.9, 229.5, 229.5, 255.0)>
        Sub GetTintColors_GoodInput_Succeeds(
            ByVal colorInR As System.Byte, ByVal colorInG As System.Byte,
            ByVal colorInB As System.Byte, ByVal tintFactor As System.Double,
            ByRef expectR As System.Double, ByRef expectG As System.Double,
            ByRef expectB As System.Double)

            Const SMALLDIFF As System.Double = 1.0
            Dim ColorIn As System.Windows.Media.Color =
                System.Windows.Media.Color.FromRgb(colorInR, colorInG, colorInB)
            Dim ColorOut As System.Windows.Media.Color

            ColorOut = OSNW.Graphics.ColorUtilities.GetTint(ColorIn, tintFactor)

            Assert.Equal(expectR, ColorOut.R, SMALLDIFF)
            Assert.Equal(expectG, ColorOut.G, SMALLDIFF)
            Assert.Equal(expectB, ColorOut.B, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(64, 128, 192, -0.01, 0.5)>
        <InlineData(64, 128, 192, 0.5, -0.01)>
        <InlineData(64, 128, 192, 255.01, 0.5)>
        <InlineData(64, 128, 192, 0.5, 1.01)>
        Sub GetToneRGB_BadInput_ForcesResults(
            ByVal colorInR As System.Double, ByVal colorInG As System.Double,
            ByVal colorInB As System.Double, ByVal grayLevel As System.Double,
            ByVal toneFactor As System.Double)

            Const SMALLDIFF As System.Double = 0.1
            Dim ResultR, ResultG, ResultB As System.Double

            OSNW.Graphics.ColorUtilities.GetTone(colorInR, colorInG, colorInB,
                grayLevel, toneFactor, ResultR, ResultG, ResultB)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultR,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultG,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ResultB,
                         SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(255.0, 0.0, 0.0, 127.5, 0.1, 242.0, 13.0, 13.0, 1.0)> ' dopely 50% gray 10% tone
        <InlineData(255.0, 0.0, 0.0, 127.5, 0.5, 192.0, 64.0, 64.0, 1.0)> ' dopely 50% gray 50% tone
        <InlineData(255.0, 0.0, 0.0, 127.5, 0.9, 141.0, 115.0, 115.0, 1.0)> ' dopely 50% gray 90% tone
        <InlineData(255.0, 0.0, 0.0, 25.5, 0.5, 140.25, 12.75, 12.75, 0.1)>
        <InlineData(0.0, 255.0, 0.0, 25.5, 0.5, 12.75, 140.25, 12.75, 0.1)>
        <InlineData(0.0, 0.0, 255.0, 25.5, 0.5, 12.75, 12.75, 140.25, 0.1)>
        <InlineData(255.0, 0.0, 0.0, 127.5, 0.5, 191.25, 63.75, 63.75, 0.1)>
        <InlineData(0.0, 255.0, 0.0, 127.5, 0.5, 63.75, 191.25, 63.75, 0.1)>
        <InlineData(0.0, 0.0, 255.0, 127.5, 0.5, 63.75, 63.75, 191.25, 0.1)>
        <InlineData(255.0, 0.0, 0.0, 229.5, 0.5, 242.25, 114.75, 114.75, 0.1)>
        <InlineData(0.0, 255.0, 0.0, 229.5, 0.5, 114.75, 242.25, 114.75, 0.1)>
        <InlineData(0.0, 0.0, 255.0, 229.5, 0.5, 114.75, 114.75, 242.25, 0.1)>
        Sub GetToneRGB_GoodInput_Succeeds(
            ByVal colorInR As System.Double, ByVal colorInG As System.Double,
            ByVal colorInB As System.Double, ByVal grayLevel As System.Double,
            ByVal toneFactor As System.Double, ByRef expectR As System.Double,
            ByRef expectG As System.Double, ByRef expectB As System.Double,
            ByVal allowance As System.Double)

            Dim ResultR, ResultG, ResultB As System.Double

            OSNW.Graphics.ColorUtilities.GetTone(colorInR, colorInG, colorInB,
                grayLevel, toneFactor, ResultR, ResultG, ResultB)

            Assert.Equal(expectR, ResultR, allowance)
            Assert.Equal(expectG, ResultG, allowance)
            Assert.Equal(expectB, ResultB, allowance)

        End Sub

        <Theory>
        <InlineData(64, 128, 192, -0.01, 0.5)>
        <InlineData(64, 128, 192, 0.5, -0.01)>
        <InlineData(64, 128, 192, 255.01, 0.5)>
        <InlineData(64, 128, 192, 0.5, 1.01)>
        Sub GetToneColors_BadInput_ForcesResults(
            ByVal colorInR As System.Byte, ByVal colorInG As System.Byte,
            ByVal colorInB As System.Byte, ByVal grayLevel As System.Double,
            ByVal toneFactor As System.Double)

            Const SMALLDIFF As System.Double = 0.1
            Dim ColorIn As System.Windows.Media.Color =
                System.Windows.Media.Color.FromRgb(colorInR, colorInG, colorInB)

            Dim ColorOut As System.Windows.Media.Color =
                OSNW.Graphics.ColorUtilities.GetTone(ColorIn, grayLevel, toneFactor)

            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ColorOut.R,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ColorOut.G,
                         SMALLDIFF)
            Assert.Equal(OSNW.Graphics.ColorUtilities.FORCEDRGBR, ColorOut.B,
                         SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(255, 0, 0, 25.5, 0.5, 140.25, 12.75, 12.75)> ' 10% gray 50% tone
        <InlineData(0, 255, 0, 25.5, 0.5, 12.75, 140.25, 12.75)>
        <InlineData(0, 0, 255, 25.5, 0.5, 12.75, 12.75, 140.25)>
        <InlineData(255, 0, 0, 127.5, 0.1, 242.0, 13.0, 13.0)> ' dopely 50% gray 10% tone
        <InlineData(255, 0, 0, 127.5, 0.5, 192.0, 64.0, 64.0)> ' dopely 50% gray 50% tone
        <InlineData(255, 0, 0, 127.5, 0.9, 141.0, 115.0, 115.0)> ' dopely 50% gray 90% tone
        <InlineData(255, 0, 0, 127.5, 0.5, 191.25, 63.75, 63.75)> ' 50% gray 50% tone
        <InlineData(0, 255, 0, 127.5, 0.5, 63.75, 191.25, 63.75)>
        <InlineData(0, 0, 255, 127.5, 0.5, 63.75, 63.75, 191.25)>
        <InlineData(255, 0, 0, 229.5, 0.5, 242.25, 114.75, 114.75)> ' 90% gray 50% tone
        <InlineData(0, 255, 0, 229.5, 0.5, 114.75, 242.25, 114.75)>
        <InlineData(0, 0, 255, 229.5, 0.5, 114.75, 114.75, 242.25)>
        Sub GetToneColors_GoodInput_Succeeds(
            ByVal colorInR As System.Byte, ByVal colorInG As System.Byte,
            ByVal colorInB As System.Byte, ByVal grayLevel As System.Double,
            ByVal toneFactor As System.Double, ByRef expectR As System.Double,
            ByRef expectG As System.Double, ByRef expectB As System.Double)
            '            xxx change inputs to bytes?

            ' Test values: https://colors.dopely.top/color-toner/ff0000-tones-11
            ' That appears to show 11 tones, from 0%-100%, in 10% steps. If so, the 100% tone
            ' result looks like medium gray, with no choice of which gray is used.

            Const SMALLDIFF As System.Double = 1.0

            Dim ColorIn As System.Windows.Media.Color =
                System.Windows.Media.Color.FromRgb(colorInR, colorInG, colorInB)
            Dim ColorOut As System.Windows.Media.Color

            ColorOut = OSNW.Graphics.ColorUtilities.GetTone(ColorIn, grayLevel,
                                                        toneFactor)

            Assert.Equal(expectR, ColorOut.R, SMALLDIFF)
            Assert.Equal(expectG, ColorOut.G, SMALLDIFF)
            Assert.Equal(expectB, ColorOut.B, SMALLDIFF)

        End Sub

        <Theory>
        <InlineData(8, 0, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Fraction, 1 / 8)>
        <InlineData(0, 0, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Fraction, 7 / 8)>
        <InlineData(0, 8, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Fraction, 5 / 8)>
        <InlineData(8, 8, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Fraction, 3 / 8)>
        <InlineData(8, 0, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Radians, System.Math.PI / 4.0)>
        <InlineData(0, 0, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Radians, System.Math.PI * 7 / 4.0)>
        <InlineData(0, 8, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Radians, System.Math.PI * 5 / 4.0)>
        <InlineData(8, 8, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Radians, System.Math.PI * 3 / 4.0)>
        <InlineData(8, 0, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Degrees, 45)>
        <InlineData(0, 0, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Degrees, 315)>
        <InlineData(0, 8, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Degrees, 225)>
        <InlineData(8, 8, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Degrees, 135)>
        <InlineData(8, 0, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Scale255, 255 / 8)>
        <InlineData(0, 0, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Scale255, 255 * 7 / 8)>
        <InlineData(0, 8, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Scale255, 255 * 5 / 8)>
        <InlineData(8, 8, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Scale255, 255 * 3 / 8)>
        <InlineData(8, 0, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Scale240, 240 / 8)>
        <InlineData(0, 0, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Scale240, 240 * 7 / 8)>
        <InlineData(0, 8, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Scale240, 240 * 5 / 8)>
        <InlineData(8, 8, 9, 9, OSNW.Graphics.ColorUtilities.HueScaleEnum.Scale240, 240 * 3 / 8)>
        Sub GetHueFromPixel_GoodInput_Succeeds(ByVal pixelX As System.Int32,
            ByVal pixelY As System.Int32, ByVal imageWidth As System.Int32,
            ByVal imageHeight As System.Int32,
            ByVal scaleTo As OSNW.Graphics.ColorUtilities.HueScaleEnum,
            ByVal expectResult As System.Double)

            Const SMALLDIFF As System.Double = 0.01

            Dim ActualResult As System.Double =
                OSNW.Graphics.ColorUtilities.GetHueFromPixel(pixelX, pixelY,
                    imageWidth, imageHeight, scaleTo)

            Assert.Equal(expectResult, ActualResult, SMALLDIFF)

        End Sub

    End Class ' OtherUnitTests

End Namespace ' ColorUtilUnitTests

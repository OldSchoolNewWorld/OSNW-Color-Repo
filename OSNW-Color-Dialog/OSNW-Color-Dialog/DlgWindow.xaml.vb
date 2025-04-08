Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

' NOTE: <UseWPF>true</UseWPF> may need to be added to the dialogs'
' <projectname>.vbproj file.
'   https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props-desktop
'   Maybe just include PresentationFramework.dll? Or System.Windows?

' For Dialog.ico, set "Build Action" to "Resource" and "Copy to Output
' Directory" to "Do not copy".

' Trying to put XML comments here results in "BC42314 XML comment cannot be
'   applied more than once on a partial class. XML comments for this class will
'   be ignored."
'
'''' <summary>
'''' Represents a model for the window displayed by a <see cref="ColorDialog"/>.
'''' </summary>
'''' <remarks>
'''' A <see cref="ColorDialog"/> creates a layer of abstraction between its
'''' underlying <c>ColorDlgWindow</c> and the consuming assembly.
'''' <c>ColorDlgWindow</c> is designated as <c>Friend</c> and its XAML
'''' contains <c>x:ClassModifier="Friend"</c>; it is only directly available to
'''' the associated <see cref="ColorDialog"/>. Public members of
'''' <see cref="System.Windows.Window"/> are not reachable by the consuming
'''' assembly unless exposed by the <see cref="ColorDialog"/>.
'''' </remarks>
Friend Class ColorDlgWindow

    ' These links are from looking into being able to have the dialog window not
    ' be accessible outside of the DLL.

    ' REF: How do I mark a control as 'Private' in WPF?
    ' https://stackoverflow.com/questions/29525968/how-do-i-mark-a-control-as-private-in-wpf

    ' REF: In WPF, how do I make my controls inside a usercontrol private?
    ' https://www.ansaurus.com/question/300255-in-wpf-how-do-i-make-my-controls-inside-a-usercontrol-private

    ' REF: x:FieldModifier Directive
    ' https://learn.microsoft.com/en-us/dotnet/desktop/xaml-services/xfieldmodifier-directive

    ' REF: x:ClassModifier Directive
    ' https://learn.microsoft.com/en-us/dotnet/desktop/xaml-services/xclassmodifier-directive
    ' For Microsoft Visual Basic .NET, the string to pass to designate TypeAttributes.NotPublic is Friend.
    ' That is done in ColorDlgWindow.xaml.

    ' A signal to distinguish between aborts and acceptance, at closure.
    Private ClosingViaOk As System.Boolean

    ' A signal to prevent recursive responses.
    Private SettingSliders As System.Boolean

#Region "Localized Constants"

    ' Frequent reuse and shorthand.
    Private Const HFF As System.Int32 = &HFF << 24

    ' Reduce screen resize.
    Private Const DEFINEDCOMBOBOXLABELWIDTH As System.Double = 150.0

#End Region ' Localized Constants

#Region "Localized Types"

    Public Class SharedColorDlgValues

        ' These are public members of the Friend ColorDlgWindow class. As such,
        ' they are reachable by the dialog host, particulary during initialization.

        ' Initialization constants.

        ' Window items.
        Public Const DEFAULTRESIZEMODE As System.Windows.ResizeMode =
            System.Windows.ResizeMode.CanResize
        Public Const DEFAULTDIALOGTITLE As System.String = "Color Workspace"
        Public Const DEFAULTSHOWINTASKBAR As System.Boolean = False
        Public Const DEFAULTWINDOWSTARTUPLOCATION As _
            System.Windows.WindowStartupLocation =
            System.Windows.WindowStartupLocation.Manual ' Matches default.

        ' Background color components for bad entries in text boxes.
        Public Const BADTEXTR As System.Byte = 238
        Public Const BADTEXTG As System.Byte = 170
        Public Const BADTEXTB As System.Byte = 170

        ' Color dialog items.
        Public Const DEFAULTSHOWCONVERTTAB As System.Boolean = True
        Public Const DEFAULTSHOWDEFINEDTAB As System.Boolean = True
        Public Const DEFAULTSHOWRGBTTAB As System.Boolean = True
        Public Const DEFAULTSHOWHSLTAB As System.Boolean = True
        Public Const DEFAULTSHOWHSVTAB As System.Boolean = True
        Public Const DEFAULTSHOWSHADETAB As System.Boolean = True
        Public Const DEFAULTSHOWTINTTAB As System.Boolean = True
        Public Const DEFAULTSHOWTONETAB As System.Boolean = True
        Public Const DEFAULTSHOWBLENDTAB As System.Boolean = True

        ' Tone values.
        Public Const TONEINITIALFACTOR As System.Int32 = 0
        Public Const TONEINITIALGRAY As System.Byte = 192

        ' Blend values.
        Public Const BLENDINITIALRGBRATIO1 As System.Double = 100.0
        Public Const BLENDINITIALRGBRATIO2 As System.Double = 0.0

        ' Consistency strings.
        Public Const REDWORD As System.String = "Red"
        Public Const GREENWORD As System.String = "Green"
        Public Const BLUEWORD As System.String = "Blue"
        Public Const HUEWORD As System.String = "Hue"

    End Class ' SharedColorDlgValues

    ' Values for LastRgbChange.
    Private Enum LastRgbChangeEnum
        Auto ' Causes a calculated choice.
        Red
        Green
        Blue
    End Enum

    ''' <summary>
    ''' Packages a <see cref="System.Windows.Media.Color"/> with a name.
    ''' </summary>
    Public Class NamedColorPair

        Public Sub New(aName As System.String,
                       aColor As System.Windows.Media.Color)
            Me.Name = aName
            Me.Color = aColor
        End Sub

        Public ReadOnly Name As System.String
        Public ReadOnly Color As System.Windows.Media.Color

    End Class ' NamedColorPair

#End Region ' Localized Types

#Region "Localized Variables"

    ' Early exits are used throughout this appplication to reduce excessive
    '   indentation of If statements.

    ' Use to avoid actions before objects are ready.
    ' Not being used at this time.
    '    Private WindowInitialized As System.Boolean = False
    ' Replaced by reference to Me.IsLoaded.
    '    Private WindowLoaded As System.Boolean = False

    ' Track whether the last change was red, green, or blue.
    Private LastRgbChange As LastRgbChangeEnum

    ' Track whether shade/tint factor has been selected at least once.
    Private ShadeFactorClicked As System.Boolean
    Private TintFactorClicked As System.Boolean
    Private ToneValuesClicked As System.Boolean

    ' Representations of the color being modified.

    ' The high precision RGB values used by conversions.
    Private UnderlyingR As System.Double
    Private UnderlyingG As System.Double
    Private UnderlyingB As System.Double

    ' Less precise representations that wind up actually being used for display.
    Private RgbWorkR As System.Byte
    Private RgbWorkG As System.Byte
    Private RgbWorkB As System.Byte
    Private RgbWorkColor As System.Windows.Media.Color
    Private RgbWorkSolidBrush As System.Windows.Media.SolidColorBrush
    Private RgbWorkContrastColor As System.Windows.Media.Color
    Private RgbWorkContrastSolidBrush As System.Windows.Media.SolidColorBrush

    ' Used for an undo feature.
    Private RememberR As System.Byte
    Private RememberG As System.Byte
    Private RememberB As System.Byte

    ' Flag for when changes are being pushed. Used to suppress reactions.
    ' Set to True initially until everything is set.
    Private ConvertTabPushing As System.Boolean = True

    ' Coloring for good/bad text in the Convert and Blend tabs.
    Private GoodBackgroundBrush As System.Windows.Media.Brush
    Private BadBackgroundBrush As System.Windows.Media.Brush

    ' HSL equivalent of the working color.
    Private HslWorkH As System.Double
    Private HslWorkS As System.Double
    Private HslWorkL As System.Double

    ' HSV equivalent of the working color.
    Private HsvWorkH As System.Double
    Private HsvWorkS As System.Double
    Private HsvWorkV As System.Double

    ' Values for the shade operations.
    Private ShadeStartR As System.Byte
    Private ShadeStartG As System.Byte
    Private ShadeStartB As System.Byte
    Private ShadeStartH As System.Double
    Private ShadeWorkFactor As System.Int32

    ' Values for the tint operations.
    Private TintStartR As System.Byte
    Private TintStartG As System.Byte
    Private TintStartB As System.Byte
    Private TintStartH As System.Double
    Private TintWorkFactor As System.Int32

    ' Values for the tone operations.
    Private ToneStartR As System.Byte
    Private ToneStartG As System.Byte
    Private ToneStartB As System.Byte
    Private ToneStartH As System.Double
    Private ToneWorkFactor As System.Int32
    Private ToneWorkGray As System.Byte

    ' Record some original cursors for restoration.
    Private OriginalHslHueCursor As System.Windows.Input.Cursor
    Private OriginalHsvHueCursor As System.Windows.Input.Cursor

    ' SomeTab.GotFocus sometimes gets triggered when already on the same tab.
    ' Use this to recognize when GotFocus occurs on the current tab.
    Private LastFocusTab As System.Windows.Controls.TabItem

#End Region ' "Localized Variables"

#Region "Properties"

    ''' <summary>
    ''' Represents the red component passed to and from the dialog window.
    ''' </summary>
    Public Property Red As System.Byte

    ''' <summary>
    ''' Represents the green component passed to and from the dialog window.
    ''' </summary>
    Public Property Green As System.Byte

    ''' <summary>
    ''' Represents the blue component passed to and from the dialog window.
    ''' </summary>
    Public Property Blue As System.Byte

    ''' <summary>
    ''' Specifies whether to show the 'Convert' tab when the dialog starts.
    ''' </summary>
    ''' <remarks>This is only checked at startup. It is not passed to change an
    ''' open dialog.</remarks>
    Public Property ShowConvertTab As System.Boolean

    ''' <summary>
    ''' Specifies whether to show the 'Defined' tab when the dialog starts.
    ''' </summary>
    ''' <remarks>This is only checked at startup. It is not passed to change an
    ''' open dialog.</remarks>
    Public Property ShowDefinedTab As System.Boolean

    ''' <summary>
    ''' Specifies whether to show the 'RGB' tab when the dialog starts.
    ''' </summary>
    ''' <remarks>This is only checked at startup. It is not passed to change an
    ''' open dialog.</remarks>
    Public Property ShowRgbTab As System.Boolean

    ''' <summary>
    ''' Specifies whether to show the 'HSL' tab when the dialog starts.
    ''' </summary>
    ''' <remarks>This is only checked at startup. It is not passed to change an
    ''' open dialog.</remarks>
    Public Property ShowHslTab As System.Boolean

    ''' <summary>
    ''' Specifies whether to show the 'HSV' tab when the dialog starts.
    ''' </summary>
    ''' <remarks>This is only checked at startup. It is not passed to change an
    ''' open dialog.</remarks>
    Public Property ShowHsvTab As System.Boolean

    ''' <summary>
    ''' Specifies whether to show the 'Shade' tab when the dialog starts.
    ''' </summary>
    ''' <remarks>This is only checked at startup. It is not passed to change an
    ''' open dialog.</remarks>
    Public Property ShowShadeTab As System.Boolean

    ''' <summary>
    ''' Specifies whether to show the 'Tinr' tab when the dialog starts.
    ''' </summary>
    ''' <remarks>This is only checked at startup. It is not passed to change an
    ''' open dialog.</remarks>
    Public Property ShowTintTab As System.Boolean

    ''' <summary>
    ''' Specifies whether to show the 'Tone' tab when the dialog starts.
    ''' </summary>
    ''' <remarks>This is only checked at startup. It is not passed to change an
    ''' open dialog.</remarks>
    Public Property ShowToneTab As System.Boolean

    ''' <summary>
    ''' Specifies whether to show the 'Blend' tab when the dialog starts.
    ''' </summary>
    ''' <remarks>This is only checked at startup. It is not passed to change an
    ''' open dialog.</remarks>
    Public Property ShowBlendTab As System.Boolean

#End Region ' "Properties"

#Region "Internal Utilities"
    ' DEV: These utilities are not intended as part of the model. Any dialog may
    ' need to perform operations unique to itself.

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
    Private Shared Function ContrastingBW(ByVal r As System.Byte,
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

#End Region ' "Internal Utilities"

#Region "Dialog Model Utilities"
    ' These utilities are intended as part of the model, but the implementation
    ' may vary or they may be omitted in individual cases.

    ''' <summary>
    ''' Update visual items that reflect the impact of state changes.
    ''' </summary>
    Private Sub UpdateVisuals()

        ' DEV: The entries below are speficic the the sample dialog window.
        'Dim BackgroundColor As System.Windows.Media.Color =
        '    System.Windows.Media.Color.FromRgb(Me.Red, Me.Green, Me.Blue)
        'Me.ColorTextBox.Background =
        '    New System.Windows.Media.SolidColorBrush(BackgroundColor)
        'Dim ForegroundColor As System.Windows.Media.Color =
        '    ContrastingBW(Me.Red, Me.Green, Me.Blue)
        'Me.ColorTextBox.Foreground =
        '    New System.Windows.Media.SolidColorBrush(ForegroundColor)
        'Me.ColorTextBox.Text = $"R:{Me.Red} G:{Me.Green} B:{Me.Blue}"

        ''''''''Throw New System.NotImplementedException(
        ''''''''    $"Thrown by {System.Reflection.MethodBase.GetCurrentMethod}")

    End Sub ' UpdateVisuals

#End Region ' "Dialog Model Utilities"

End Class ' ColorDlgWindow

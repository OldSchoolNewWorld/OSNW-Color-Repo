Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Class MainWindow

    Const DEFAULTRED As System.String = "64"
    Const DEFAULTGREEN As System.String = "128"
    Const DEFAULTBLUE As System.String = "192"

    ' Components for bad entries in text boxes.
    Private Const BADTEXTR As System.Byte = 238
    Private Const BADTEXTG As System.Byte = 170
    Private Const BADTEXTB As System.Byte = 170

    Private ValidConvertRgbRedTextBox As System.Boolean = False
    Private ValidConvertRgbGreenTextBox As System.Boolean = False
    Private ValidConvertRgbBlueTextBox As System.Boolean = False
    Private ValidConvertHtmlTextBox As System.Boolean = False

    ' Flag for when changes are being pushed. Used to suppress reactions.
    ' Set to True initially until everything is set.
    Private HtmlOrRgbPushing As System.Boolean = True

    ' Coloring for good/bad text.
    Private GoodBackgroundBrush As System.Windows.Media.Brush
    Private BadBackgroundBrush As System.Windows.Media.Brush

    Dim Red As System.Int32
    Dim Green As System.Int32
    Dim Blue As System.Int32

#Region "Event Utilities"

    Private Sub LoadToolTips()
        With Me

            Const ENTER255 As System.String = "Enter the component value (0-255)"
            Const ENTERHTML As System.String = "Enter the HTML #RRGGBB code"
            Const DEMOSELECT As System.String = "Click to select a new color"
            Const DEMOEXIT As System.String = "Click to exit the demo application"

            .ConvertRgbRedTextBox.ToolTip = ENTER255
            .ConvertRgbGreenTextBox.ToolTip = ENTER255
            .ConvertRgbBlueTextBox.ToolTip = ENTER255
            .ConvertHtmlTextBox.ToolTip = ENTERHTML
            .SelectButton.ToolTip = DEMOSELECT
            .ExitButton.ToolTip = DEMOEXIT

        End With
    End Sub ' LoadToolTips

#End Region ' "Event Utilities"

#Region "Model Events"

    '''' <summary>
    '''' Occurs when this <c>Window</c> is initialized. Backing fields and local
    '''' variables can usually be set after arriving here. See
    '''' <see cref="System.Windows.FrameworkElement.Initialized"/>.
    '''' </summary>
    'Private Sub Window_Initialized(sender As Object, e As EventArgs) _
    '    Handles Me.Initialized

    '    Me.ClosingViaOk = False
    'End Sub ' Window_Initialized

    ''' <summary>
    ''' Occurs when the <c>Window</c> is laid out, rendered, and ready for
    ''' interaction. Sometimes updates have to wait until arriving here. See
    ''' <see cref="System.Windows.FrameworkElement.Loaded"/>.
    ''' </summary>
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) _
        Handles Me.Loaded

        '        Me.DoWindow_Loaded(sender, e)

        ' Create an original background color. Establish the bad text color.
        Me.GoodBackgroundBrush = Me.ConvertRgbRedTextBox.Background
        Dim BadBackgroundColor As New System.Windows.Media.Color With {
                .A = &HFF, .R = BADTEXTR, .G = BADTEXTG, .B = BADTEXTB}
        Me.BadBackgroundBrush =
                New System.Windows.Media.SolidColorBrush(BadBackgroundColor)

        Me.LoadToolTips()

        ' Prevent unwanted reactions.
        Me.HtmlOrRgbPushing = True
        Try

            ' Initialize the RGB component text.
            Me.ConvertRgbRedTextBox.Text = DEFAULTRED
            Me.ConvertRgbGreenTextBox.Text = DEFAULTGREEN
            Me.ConvertRgbBlueTextBox.Text = DEFAULTBLUE

            ' Initialize the HTML text.
            Dim HtmlStr As System.String = System.String.Empty
            OSNW.Graphics.ColorUtilities.RGBtoHTML(
            System.Convert.ToByte(ConvertRgbRedTextBox.Text),
            System.Convert.ToByte(ConvertRgbGreenTextBox.Text),
            System.Convert.ToByte(ConvertRgbBlueTextBox.Text), HtmlStr)
            Me.ConvertHtmlTextBox.Text = HtmlStr

        Finally
            ' Restore reactions.
            Me.HtmlOrRgbPushing = False
        End Try

    End Sub ' Window_Loaded

    Private Sub ExitButton_Click(sender As Object, e As RoutedEventArgs) _
        Handles ExitButton.Click

        Me.Close()
    End Sub ' ExitButton_Click

#End Region ' "Model Events"

#Region "Localized Events"

    Private Function AllInputsValid() As System.Boolean
        Return ValidConvertHtmlTextBox AndAlso ValidConvertRgbRedTextBox AndAlso
            ValidConvertRgbGreenTextBox AndAlso ValidConvertRgbBlueTextBox
    End Function ' AllInputsValid

    Private Sub ConvertRgbTextBox_TextChanged(sender As Object,
                                              e As RoutedEventArgs) _
        Handles ConvertRgbRedTextBox.TextChanged,
            ConvertRgbGreenTextBox.TextChanged,
            ConvertRgbBlueTextBox.TextChanged

        ' Validate new text and obtain byte value.
        Dim SendingTextBox As System.Windows.Controls.TextBox =
            CType(sender, System.Windows.Controls.TextBox)
        Dim ByteVal As System.Byte
        If Not System.Byte.TryParse(SendingTextBox.Text, ByteVal) Then

            ' Show that one as invalid.
            SendingTextBox.Background = BadBackgroundBrush

            ' Keep track of outstanding bad text boxes.
            Select Case SendingTextBox.Name
                Case ConvertRgbRedTextBox.Name
                    ValidConvertRgbRedTextBox = False
                Case ConvertRgbGreenTextBox.Name
                    ValidConvertRgbGreenTextBox = False
                Case ConvertRgbBlueTextBox.Name
                    ValidConvertRgbBlueTextBox = False
                Case Else
                    Dim CaughtBy As System.Reflection.MethodBase =
                        System.Reflection.MethodBase.GetCurrentMethod()
                    Throw New System.ApplicationException(
                        $"Unexpected {NameOf(SendingTextBox)} in {CaughtBy}")
            End Select

            Exit Sub ' Early exit.

        Else

            ' Show that one as valid.
            SendingTextBox.Background = GoodBackgroundBrush

            ' Keep track of outstanding bad text.
            Select Case SendingTextBox.Name
                Case ConvertRgbRedTextBox.Name
                    ValidConvertRgbRedTextBox = True
                Case ConvertRgbGreenTextBox.Name
                    ValidConvertRgbGreenTextBox = True
                Case ConvertRgbBlueTextBox.Name
                    ValidConvertRgbBlueTextBox = True
                Case Else
                    Dim CaughtBy As System.Reflection.MethodBase =
                        System.Reflection.MethodBase.GetCurrentMethod()
                    Throw New System.ApplicationException(
                        $"Unexpected {NameOf(SendingTextBox)} in {CaughtBy}")

            End Select

        End If

        ' This appears AFTER the validation so that pushes will still set the
        ' good/bad color.
        If Me.HtmlOrRgbPushing Then
            ' Do not process further.
            Exit Sub ' Early exit.
        End If

        If Me.AllInputsValid() Then
            ' Process the new value.
            With Me
                Dim PushOnArrival As System.Boolean = .HtmlOrRgbPushing
                .HtmlOrRgbPushing = True
                Try
                    Dim HtmlStr As System.String = System.String.Empty
                    OSNW.Graphics.ColorUtilities.RGBtoHTML(
                        System.Convert.ToByte(ConvertRgbRedTextBox.Text),
                        System.Convert.ToByte(ConvertRgbGreenTextBox.Text),
                        System.Convert.ToByte(ConvertRgbBlueTextBox.Text), HtmlStr)
                    .ConvertHtmlTextBox.Text = HtmlStr
                Finally
                    .HtmlOrRgbPushing = PushOnArrival
                End Try
            End With
        End If

    End Sub ' ConvertRgbTextBox_TextChanged

    Private Sub ConvertHtmlTextBox_TextChanged(sender As Object,
                                               e As RoutedEventArgs) _
        Handles ConvertHtmlTextBox.TextChanged

        ' Validate new text and obtain the text.
        Dim HtmlText As System.String = ConvertHtmlTextBox.Text
        If Not OSNW.Graphics.ColorUtilities.IsValidHtmlString(HtmlText) Then

            ' Show that one as invalid.
            ConvertHtmlTextBox.Background = BadBackgroundBrush

            ' Keep track of outstanding bad text.
            ValidConvertHtmlTextBox = False

            Exit Sub ' Early exit.

        Else
            ' New text is valid.
            ConvertHtmlTextBox.Background = GoodBackgroundBrush
            ValidConvertHtmlTextBox = True
        End If

        ' This appears AFTER the validation so that pushes will still set the
        ' good/bad color.
        If Me.HtmlOrRgbPushing Then
            ' Do not process further.
            Exit Sub ' Early exit.
        End If

        If AllInputsValid() Then
            ' Process the new value.
            With Me

                Dim PushOnArrival As System.Boolean = Me.HtmlOrRgbPushing
                Me.HtmlOrRgbPushing = True
                Try
                    Dim ByteR As System.Byte = CByte(Me.Red)
                    Dim ByteG As System.Byte = CByte(Me.Green)
                    Dim ByteB As System.Byte = CByte(Me.Blue)
                    OSNW.Graphics.ColorUtilities.HTMLtoRGB(
                        HtmlText, ByteR, ByteG, ByteB)
                    Me.ConvertRgbRedTextBox.Text = ByteR.ToString
                    Me.ConvertRgbGreenTextBox.Text = ByteG.ToString
                    Me.ConvertRgbBlueTextBox.Text = ByteB.ToString
                Finally
                    .HtmlOrRgbPushing = PushOnArrival
                End Try

            End With
        End If

    End Sub ' ConvertHtmlTextBox_TextChanged

    Private Sub SelectButton_Click(sender As Object, e As RoutedEventArgs) _
        Handles SelectButton.Click

        Try

            ' Extract the values entered.
            Dim Red As System.Byte =
                System.Byte.Parse(Me.ConvertRgbRedTextBox.Text)
            Dim Green As System.Byte =
                System.Byte.Parse(Me.ConvertRgbGreenTextBox.Text)
            Dim Blue As System.Byte =
                System.Byte.Parse(Me.ConvertRgbBlueTextBox.Text)

            ' Set up and show dialog.
            Dim Dlg As New OSNW.Dialog.ColorDialog With {
                .Owner = Me, .WindowStartupLocation = WindowStartupLocation.CenterOwner,
                .Red = Red, .Green = Green, .Blue = Blue}
            Dlg.ShowDialog()

            If Dlg.DialogResult Then

                Dim HtmlStr As System.String = System.String.Empty
                OSNW.Graphics.ColorUtilities.RGBtoHTML(
                    System.Convert.ToByte(Dlg.Red),
                    System.Convert.ToByte(Dlg.Green),
                    System.Convert.ToByte(Dlg.Blue), HtmlStr)

                ' Update text boxes.
                Me.ConvertRgbRedTextBox.Text = Dlg.Red.ToString()
                Me.ConvertRgbGreenTextBox.Text = Dlg.Green.ToString()
                Me.ConvertRgbBlueTextBox.Text = Dlg.Blue.ToString()
                Me.ConvertHtmlTextBox.Text = HtmlStr

            Else
                ' ????????
            End If

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            Me.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e)
        End Try

    End Sub ' SelectButton_Click

#End Region ' "Localized Events"

End Class ' MainWindow

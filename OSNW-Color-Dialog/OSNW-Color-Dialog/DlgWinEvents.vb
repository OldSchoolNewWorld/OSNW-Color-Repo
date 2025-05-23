﻿Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports SHWV = OSNW.Dialog.ColorDlgWindow.SharedHostedWindowValues
Imports OEH = OSNW.Dialog.ColorDlgWindow.OSNWExceptionHandler

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
Partial Friend Class ColorDlgWindow

#Region "Dialog Model Event Utilities"

    ' Maybe this would be activated to warn agoinst the loss of a selection that has been made.
    ''' <summary>
    ''' Evaluate whether there is any reason to consider aborting closure via
    ''' <c>CancelButton</c>, etc.
    ''' </summary>
    ''' <returns><c>True</c> if closure via <c>CancelButton</c>, etc. should be
    ''' reconsidered; otherwise, <c>False</c>.</returns>
    Private Function WarnClose() As System.Boolean
        ' DEV: Add code here to determine if some risky condition exists when
        ' faced with a closure. If so, display a message to decide how to
        ' proceed. This can be left as is and returning False. It can also be
        ' deleted, or commented out, to avoid the useless call.
        Return False
    End Function

#End Region ' "Dialog Model Event Utilities"

#Region "Dialog Model Events"

    ''' <summary>
    ''' Initializes the control data. Occurs when this <c>Window</c> is
    ''' initialized. Backing fields and local variables can usually be set after
    ''' arriving here. See 
    ''' <see cref="System.Windows.FrameworkElement.Initialized"/>.
    ''' </summary>
    Private Sub Window_Initialized(sender As Object, e As EventArgs) _
        Handles Me.Initialized

        ' No argument checking.

        Try
            Me.Do_Window_Initialized(sender, e)
            Me.ClosingViaOk = False
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try

    End Sub ' Window_Initialized

    ''' <summary>
    ''' Initializes the control data. after having been loaded.
    ''' Occurs when the <c>Window</c> is laid out, rendered, and ready for
    ''' interaction. Sometimes updates have to wait until arriving here. See
    ''' <see cref="System.Windows.FrameworkElement.Loaded"/>.
    ''' </summary>
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) _
        Handles Me.Loaded

        ' No argument checking.

        Try
            Me.Do_Window_Loaded(sender, e)
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try

    End Sub ' Window_Loaded

    ''' <summary>
    ''' Occurs directly after <see cref="System.Windows.Window.Close"/> is
    ''' called, and can be handled to cancel window closure. See
    ''' <see cref="System.Windows.Window.Closing"/>.
    ''' </summary>
    ''' <remarks>
    ''' This gets hit for <c>CancelButton</c>, Escape, ALT+F4,
    ''' System menu | Close, and the window's red X. It also gets hit whenever
    ''' <c>DialogResult</c> is set. It also gets hit for <c>OkButton</c>, if
    ''' only because it sets <c>DialogResult</c>.
    ''' </remarks>
    Private Sub Window_Closing(sender As Object,
        e As ComponentModel.CancelEventArgs) _
        Handles Me.Closing

        ' In general, do not interfere when OkButton was used.
        If Me.ClosingViaOk Then
            Exit Sub ' Early exit.
        End If

        ' Falling through to here allows the closure to continue.

    End Sub ' Window_Closing

    ''' <summary>
    ''' Fill in any updates to the passed data then close the window.
    ''' </summary>
    Private Sub OkButton_Click(sender As Object, e As Windows.RoutedEventArgs) _
        Handles OkButton.Click

        ' No argument checking.

        Try

            ' Set any return values.
            Me.Red = CByte(Me.UnderlyingR)
            Me.Green = CByte(Me.UnderlyingG)
            Me.Blue = CByte(Me.UnderlyingB)

            ' Get ready to shut down.
            Me.ClosingViaOk = True
            Me.DialogResult = True

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try

    End Sub ' OkButton_Click

#End Region ' "Dialog Model Events"

#Region "Localized Events"

#Region "Convert Tab Events"

    Private Sub ConvertTabItem_GotFocus(sender As Object, e As RoutedEventArgs) _
        Handles ConvertTabItem.GotFocus

        Try

            ' Only respond on a new arrival here.
            If (Me.LastFocusTab IsNot Nothing) AndAlso
            Me.LastFocusTab.Equals(sender) Then
                Exit Sub ' Early exit.
            End If
            Me.LastFocusTab =
                DirectCast(sender, System.Windows.Controls.TabItem)

            Me.ConvertRgbRedTextBox.Focus()
            Me.ConvertUpdateVisuals()

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' ConvertTabItem_GotFocus

    Private Sub ConvertTextBox_ByteTextChanged(sender As Object,
                                               e As TextChangedEventArgs) _
        Handles ConvertRgbRedTextBox.TextChanged,
          ConvertRgbGreenTextBox.TextChanged,
          ConvertRgbBlueTextBox.TextChanged

        Try

            ' Get the new value.
            Dim SendingTextBox As System.Windows.Controls.TextBox =
            CType(sender, System.Windows.Controls.TextBox)
            Dim ByteVal As System.Byte
            If Not System.Byte.TryParse(SendingTextBox.Text, ByteVal) Then
                SendingTextBox.Background = BadBackgroundBrush
                Exit Sub ' Early exit.
            End If

            ' Any byte is valid.
            SendingTextBox.Background = GoodBackgroundBrush

            ' This appears AFTER the range checks so that pushes will still set
            ' the good/bad color.
            If Me.ConvertTabPushing Then
                ' Do not process further.
                Exit Sub ' Early exit.
            End If

            ' Process the new value.
            Me.DoConvertTextBoxByteTextChanged(SendingTextBox, ByteVal)

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' ConvertTextBox_ByteTextChanged

    Private Sub ConvertTextBox_HtmlTextChanged(sender As Object,
                                               e As TextChangedEventArgs) _
        Handles ConvertHtmlTextBox.TextChanged

        Try

            ' Get the new value.
            Dim SendingTextBox As System.Windows.Controls.TextBox =
            ConvertHtmlTextBox
            Dim HtmlText As System.String = SendingTextBox.Text
            If OSNW.Graphics.ColorUtilities.IsValidHtmlString(HtmlText) Then
                SendingTextBox.Background = GoodBackgroundBrush
            Else
                SendingTextBox.Background = BadBackgroundBrush
                Exit Sub ' Early exit.
            End If

            ' This appears AFTER the validation so that pushes will still set
            ' the ' good/bad color.
            If Me.ConvertTabPushing Then
                ' Do not process further.
                Exit Sub ' Early exit.
            End If

            ' Process the new value.
            Me.DoConvertTextBoxHtmlTextChanged(SendingTextBox, HtmlText)

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' ConvertTextBox_HtmlTextChanged

    Private Sub ConvertTextBox_DoubleTextChanged(sender As Object,
                                                 e As TextChangedEventArgs) _
       Handles ConvertHueFractionTextBox.TextChanged,
           ConvertHueDegreesTextBox.TextChanged,
           ConvertHslSaturationTextBox.TextChanged,
           ConvertHslLuminanceTextBox.TextChanged,
           ConvertHsvSaturationTextBox.TextChanged,
           ConvertHsvValueTextBox.TextChanged

        Try

            ' Get the new value.
            Dim SendingTextBox As System.Windows.Controls.TextBox =
            CType(sender, System.Windows.Controls.TextBox)
            Dim DoubleVal As System.Double
            If Not System.Double.TryParse(SendingTextBox.Text, DoubleVal) Then
                SendingTextBox.Background = BadBackgroundBrush
                Exit Sub ' Early exit.
            End If

            ' Getting this far indicates a good double. Check range.
            If SendingTextBox.Equals(Me.ConvertHueFractionTextBox) Then
                ' Prevent reaching full scale.
                If DoubleVal < 0.0 OrElse DoubleVal >= 1.0 Then
                    SendingTextBox.Background = BadBackgroundBrush
                    Exit Sub ' Early exit.
                End If
            ElseIf SendingTextBox.Equals(Me.ConvertHueDegreesTextBox) Then
                ' Prevent reaching full scale.
                If DoubleVal < 0.0 OrElse DoubleVal >= 360.0 Then
                    SendingTextBox.Background = BadBackgroundBrush
                    Exit Sub ' Early exit.
                End If
            Else
                ' Allow reaching full scale.
                If DoubleVal < 0.0 OrElse DoubleVal > 1.0 Then
                    SendingTextBox.Background = BadBackgroundBrush
                    Exit Sub ' Early exit.
                End If
            End If

            ' Getting this far indicates in-range. This appears AFTER the range
            ' checks so that pushes will still set the good/bad color.
            SendingTextBox.Background = GoodBackgroundBrush
            If Me.ConvertTabPushing Then
                ' Do not process further.
                Exit Sub ' Early exit.
            End If

            ' Process the new value.
            Me.DoConvertTextBoxDoubleTextChanged(SendingTextBox, DoubleVal)

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' ConvertTextBox_DoubleTextChanged

#End Region ' "Convert Tab Events"

#Region "Defined Tab Events"

    Private Sub DefinedTabItem_GotFocus(sender As Object, e As RoutedEventArgs) _
        Handles DefinedTabItem.GotFocus

        Try

            ' Only respond on a new arrival here.
            If (Me.LastFocusTab IsNot Nothing) AndAlso
            Me.LastFocusTab.Equals(sender) Then
                Exit Sub ' Early exit.
            End If
            Me.LastFocusTab = DirectCast(sender,
                System.Windows.Controls.TabItem)

            Me.DefinedComboBox.SelectedItem = Nothing

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub DefinedComboBox_SelectionChanged(
        sender As Object, e As SelectionChangedEventArgs) _
        Handles DefinedComboBox.SelectionChanged

        Try
            If Me.DefinedComboBox.SelectedValue IsNot Nothing Then

                ' Identify the Label with the selected color.
                Dim SelectedLabel As System.Windows.Controls.Label =
                CType(Me.DefinedComboBox.SelectedValue,
                System.Windows.Controls.Label)

                ' Extract and store the selected color, allowing the default
                ' LastRgbChangeEnum assignment.
                Dim SelectedBrush As System.Windows.Media.Brush =
                SelectedLabel.Background
                Dim SelectedSolidBrush As System.Windows.Media.SolidColorBrush =
                CType(SelectedLabel.Background,
                      System.Windows.Media.SolidColorBrush)
                Dim SelectedColor As System.Windows.Media.Color =
                SelectedSolidBrush.Color
                Me.UpdateBaseValuesFromRGB(SelectedColor.R, SelectedColor.G,
                                       SelectedColor.B)

            End If
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' DefinedComboBox_SelectionChanged

#End Region ' "Defined Tab Events"

#Region "RGB Tab Events"

    Private Sub RgbTabItem_GotFocus(sender As Object, e As RoutedEventArgs) _
        Handles RgbTabItem.GotFocus

        Try

            ' Only respond on a new arrival here.
            If (Me.LastFocusTab IsNot Nothing) AndAlso
            Me.LastFocusTab.Equals(sender) Then
                Exit Sub ' Early exit.
            End If
            Me.LastFocusTab = DirectCast(sender,
                System.Windows.Controls.TabItem)

            Me.RgbUpdateVisuals()

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' RgbTabItem_GotFocus

    Private Sub RgbImage_MouseLeftButtonUp(
        sender As Object, e As MouseButtonEventArgs) _
        Handles RgbImage.MouseLeftButtonUp

        Try
            Me.RgbProcessMouseClick(sender, e)
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' RgbImage_MouseLeftButtonUp

    Private Sub RedMM_Click(sender As Object, e As RoutedEventArgs) _
        Handles RedMM.Click

        Try
            Me.UpdateBaseValuesFromRGB(Me.Down17From(Me.RgbWorkR), Me.RgbWorkG,
                                       Me.RgbWorkB, LastRgbChangeEnum.Red)
            Me.RgbUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub RedM_Click(sender As Object, e As RoutedEventArgs) _
        Handles RedM.Click

        Try
            If Me.RgbWorkR >= &H0 Then
                Me.UpdateBaseValuesFromRGB(CByte(Me.RgbWorkR - 1), Me.RgbWorkG,
                                           Me.RgbWorkB, LastRgbChangeEnum.Red)
                Me.RgbUpdateVisuals()
            End If
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub RedP_Click(sender As Object, e As RoutedEventArgs) _
        Handles RedP.Click

        Try
            If Me.RgbWorkR < &HFF Then
                Me.UpdateBaseValuesFromRGB(CByte(Me.RgbWorkR + 1), Me.RgbWorkG,
                                           Me.RgbWorkB, LastRgbChangeEnum.Red)
                Me.RgbUpdateVisuals()
            End If
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub RedPP_Click(sender As Object, e As RoutedEventArgs) _
        Handles RedPP.Click

        Try
            Me.UpdateBaseValuesFromRGB(Me.Up17From(Me.RgbWorkR), Me.RgbWorkG,
                                       Me.RgbWorkB, LastRgbChangeEnum.Red)
            Me.RgbUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub GreenMM_Click(sender As Object, e As RoutedEventArgs) _
        Handles GreenMM.Click

        Try
            Me.UpdateBaseValuesFromRGB(Me.RgbWorkR, Me.Down17From(Me.RgbWorkG),
                                       Me.RgbWorkB, LastRgbChangeEnum.Green)
            Me.RgbUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub GreenM_Click(sender As Object, e As RoutedEventArgs) _
        Handles GreenM.Click

        Try
            If Me.RgbWorkG > 0 Then
                Me.UpdateBaseValuesFromRGB(Me.RgbWorkR, CByte(Me.RgbWorkG - 1),
                                           Me.RgbWorkB, LastRgbChangeEnum.Green)
                Me.RgbUpdateVisuals()
            End If
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub GreenP_Click(sender As Object, e As RoutedEventArgs) _
        Handles GreenP.Click

        Try
            If Me.RgbWorkG < &HFF Then
                Me.UpdateBaseValuesFromRGB(Me.RgbWorkR, CByte(Me.RgbWorkG + 1),
                                           Me.RgbWorkB, LastRgbChangeEnum.Green)
                Me.RgbUpdateVisuals()
            End If
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub GreenPP_Click(sender As Object, e As RoutedEventArgs) _
        Handles GreenPP.Click

        Try
            Me.UpdateBaseValuesFromRGB(Me.RgbWorkR, Me.Up17From(Me.RgbWorkG),
                                       Me.RgbWorkB, LastRgbChangeEnum.Green)
            Me.RgbUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub BlueMM_Click(sender As Object, e As RoutedEventArgs) _
        Handles BlueMM.Click

        Try
            Me.UpdateBaseValuesFromRGB(Me.RgbWorkR, Me.RgbWorkG,
                Me.Down17From(Me.RgbWorkB), LastRgbChangeEnum.Blue)
            Me.RgbUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub BlueM_Click(sender As Object, e As RoutedEventArgs) _
        Handles BlueM.Click

        Try
            If Me.RgbWorkB >= &H0 Then
                Me.UpdateBaseValuesFromRGB(Me.RgbWorkR, Me.RgbWorkG,
                    CByte(Me.RgbWorkB - 1), LastRgbChangeEnum.Blue)
                Me.RgbUpdateVisuals()
            End If
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub BlueP_Click(sender As Object, e As RoutedEventArgs) _
        Handles BlueP.Click

        Try
            If Me.RgbWorkB < &HFF Then
                Me.UpdateBaseValuesFromRGB(Me.RgbWorkR, Me.RgbWorkG,
                    CByte(Me.RgbWorkB + 1), LastRgbChangeEnum.Blue)
                Me.RgbUpdateVisuals()
            End If
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub BluePP_Click(sender As Object, e As RoutedEventArgs) _
        Handles BluePP.Click

        Try
            Me.UpdateBaseValuesFromRGB(Me.RgbWorkR, Me.RgbWorkG,
                Me.Up17From(Me.RgbWorkB), LastRgbChangeEnum.Blue)
            Me.RgbUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

#End Region ' "RGB Tab Events"

#Region "HSx Tab Events"

    Private Sub HsxTabItem_GotFocus(sender As Object, e As RoutedEventArgs) _
        Handles HslTabItem.GotFocus, HsvTabItem.GotFocus

        Try

            ' Only respond on a new arrival here.
            If (Me.LastFocusTab IsNot Nothing) AndAlso
            Me.LastFocusTab.Equals(sender) Then
                Exit Sub ' Early exit.
            End If
            Me.LastFocusTab =
                DirectCast(sender, System.Windows.Controls.TabItem)

            Dim SendingTabItem As System.Windows.Controls.TabItem =
            DirectCast(sender, System.Windows.Controls.TabItem)
            If SendingTabItem.Equals(Me.HslTabItem) Then
                Me.HslUpdateVisuals()
            Else
                Me.HsvUpdateVisuals()
            End If

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub HsxSelectHueImage_MouseMove(sender As Object, e As MouseEventArgs) _
        Handles HslSelectHueImage.MouseMove, HsvSelectHueImage.MouseMove

        Try
            Dim SendingImage As System.Windows.Controls.Image =
                DirectCast(sender, System.Windows.Controls.Image)
            Me.HsxSelectHueImageProcessMouseMove(SendingImage, e)
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' HsxSelectHueImage_MouseMove

    Private Sub HsxSelectHueImage_MouseLeftButtonUp(
        sender As Object, e As MouseButtonEventArgs) _
        Handles HslSelectHueImage.MouseLeftButtonUp,
          HsvSelectHueImage.MouseLeftButtonUp

        Try
            Dim SendingImage As System.Windows.Controls.Image =
                DirectCast(sender, System.Windows.Controls.Image)
            Me.HsxHueProcessMouseClick(SendingImage, e)
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' HsxSelectHueImage_MouseLeftButtonUp

    '''' <summary>
    '''' Covers the Sat/Lum and Sat/Val parts.
    '''' </summary>
    '''' <param name="sender"></param>
    '''' <param name="e"></param>
    Private Sub HsxSelectSatImage_MouseLeftButtonUp(
        sender As Object, e As MouseButtonEventArgs) _
        Handles HslSelectSatLumImage.MouseLeftButtonUp,
            HsvSelectSatValImage.MouseLeftButtonUp

        Try
            Dim SendingImage As System.Windows.Controls.Image =
                DirectCast(sender, System.Windows.Controls.Image)
            Me.HsxSatProcessMouseClick(SendingImage, e)
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' HslSelectSatLumImage_MouseLeftButtonUp

#End Region ' "HSx Tab Events"

#Region "Shade Tab Events"

    Private Sub ShadeTabItem_GotFocus(sender As Object, e As RoutedEventArgs) _
        Handles ShadeTabItem.GotFocus

        Try

            ' Only respond on a new arrival at this tab.
            If (Me.LastFocusTab IsNot Nothing) AndAlso
                Me.LastFocusTab.Equals(sender) Then

                ' Not a new arrival.
                Exit Sub ' Early exit.
            End If
            Me.LastFocusTab =
                DirectCast(sender, System.Windows.Controls.TabItem)

            ' This is a new visit to this tab. Take note of starting conditions.
            With Me

                ' Take note of entry conditions.
                .ShadeStartR = RgbWorkR
                .ShadeStartG = RgbWorkG
                .ShadeStartB = RgbWorkB
                .ShadeStartH = .HsvWorkH
                .ShadeWorkFactor = 0

                ' Hide the display until a factor has been clicked.
                .ShadeFactorClicked = False

                .ShadeUpdateVisuals()

            End With

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' ShadeTabItem_GotFocus

    Private Sub ShadeImage_MouseLeftButtonUp(
        sender As Object, e As MouseButtonEventArgs) _
        Handles ShadeImage.MouseLeftButtonUp

        Try
            Me.ShadeProcessMouseClick(sender, e)
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' ShadeImage_MouseLeftButtonUp

    Private Sub ShadeMM_Click(sender As Object, e As RoutedEventArgs) _
        Handles ShadeMM.Click

        Try
            Me.UpdateBaseValuesFromShade(
                Me.DownPoint01From(Me.ShadeWorkFactor))
            Me.ShadeUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub ShadeM_Click(sender As Object, e As RoutedEventArgs) _
        Handles ShadeM.Click

        Try
            Me.UpdateBaseValuesFromShade(
                Me.DownPoint001From(Me.ShadeWorkFactor))
            Me.ShadeUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub ShadeP_Click(sender As Object, e As RoutedEventArgs) _
        Handles ShadeP.Click

        Try
            Me.UpdateBaseValuesFromShade(Me.UpPoint001From(Me.ShadeWorkFactor))
            Me.ShadeUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub ShadePP_Click(sender As Object, e As RoutedEventArgs) _
        Handles ShadePP.Click

        Try
            Me.UpdateBaseValuesFromShade(Me.UpPoint01From(Me.ShadeWorkFactor))
            Me.ShadeUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

#End Region ' "Shade Tab Events"

#Region "Tint Tab Events"

    Private Sub TintTabItem_GotFocus(sender As Object, e As RoutedEventArgs) _
        Handles TintTabItem.GotFocus

        Try

            ' Only respond on a new arrival at this tab.
            If (Me.LastFocusTab IsNot Nothing) AndAlso
            Me.LastFocusTab.Equals(sender) Then

                ' Not a new arrival.
                Exit Sub ' Early exit.
            End If
            Me.LastFocusTab =
                DirectCast(sender, System.Windows.Controls.TabItem)

            ' This is a new visit to this tab. Take note of starting conditions.
            With Me

                ' Take note of entry conditions.
                .TintStartR = RgbWorkR
                .TintStartG = RgbWorkG
                .TintStartB = RgbWorkB
                .TintStartH = .HsvWorkH
                .TintWorkFactor = 0

                ' Hide the display until a factor has been clicked.
                .TintFactorClicked = False

                .TintUpdateVisuals()

            End With

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' TintTabItem_GotFocus

    Private Sub TintImage_MouseLeftButtonUp(
        sender As Object, e As MouseButtonEventArgs) _
        Handles TintImage.MouseLeftButtonUp

        Try
            Me.TintProcessMouseClick(sender, e)
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' TintImage_MouseLeftButtonUp

    Private Sub TintMM_Click(sender As Object, e As RoutedEventArgs) _
        Handles TintMM.Click

        Try
            Me.UpdateBaseValuesFromTint(Me.DownPoint01From(Me.TintWorkFactor))
            Me.TintUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub TintM_Click(sender As Object, e As RoutedEventArgs) _
        Handles TintM.Click

        Try
            Me.UpdateBaseValuesFromTint(Me.DownPoint001From(Me.TintWorkFactor))
            Me.TintUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub TintP_Click(sender As Object, e As RoutedEventArgs) _
        Handles TintP.Click

        Try
            Me.UpdateBaseValuesFromTint(Me.UpPoint001From(Me.TintWorkFactor))
            Me.TintUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub TintPP_Click(sender As Object, e As RoutedEventArgs) _
        Handles TintPP.Click

        Try
            Me.UpdateBaseValuesFromTint(Me.UpPoint01From(Me.TintWorkFactor))
            Me.TintUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

#End Region ' "Tint Tab Events"

#Region "Tone Tab Events"

    Private Sub ToneTabItem_GotFocus(sender As Object, e As RoutedEventArgs) _
        Handles ToneTabItem.GotFocus

        Try

            ' Only respond on a new arrival at this tab.
            If (Me.LastFocusTab IsNot Nothing) AndAlso
            Me.LastFocusTab.Equals(sender) Then

                ' Not a new arrival.
                Exit Sub ' Early exit.
            End If
            Me.LastFocusTab =
                DirectCast(sender, System.Windows.Controls.TabItem)

            ' This is a new visit to this tab. Take note of starting conditions.
            With Me

                ' Take note of entry conditions.
                .ToneStartR = .RgbWorkR
                .ToneStartG = .RgbWorkG
                .ToneStartB = .RgbWorkB
                .ToneStartH = .HsvWorkH
                .ToneWorkFactor = SHWV.TONEINITIALFACTOR
                .ToneWorkGray = SHWV.TONEINITIALGRAY

                ' Hide the displays until values have been clicked.
                .ToneValuesClicked = False

                .ToneUpdateVisuals()

            End With

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' ToneTabItem_GotFocus

    Private Sub ToneImage_MouseLeftButtonUp(
        sender As Object, e As MouseButtonEventArgs) _
        Handles ToneImage.MouseLeftButtonUp

        Try
            Me.ToneProcessMouseClick(sender, e)
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' ToneImage_MouseLeftButtonUp

    Private Sub ToneFactorMM_Click(sender As Object, e As RoutedEventArgs) _
        Handles ToneFactorMM.Click

        Try
            Me.UpdateBaseValuesFromTone(Me.ToneWorkGray,
                                        Me.DownPoint01From(Me.ToneWorkFactor))
            Me.ToneUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub ToneFactorM_Click(sender As Object, e As RoutedEventArgs) _
        Handles ToneFactorM.Click

        Try
            Me.UpdateBaseValuesFromTone(Me.ToneWorkGray,
                                        Me.DownPoint001From(Me.ToneWorkFactor))
            Me.ToneUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub ToneFactorP_Click(sender As Object, e As RoutedEventArgs) _
        Handles ToneFactorP.Click

        Try
            Me.UpdateBaseValuesFromTone(Me.ToneWorkGray,
                                        Me.UpPoint001From(Me.ToneWorkFactor))
            Me.ToneUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub ToneFactorPP_Click(sender As Object, e As RoutedEventArgs) _
        Handles ToneFactorPP.Click

        Try
            Me.UpdateBaseValuesFromTone(Me.ToneWorkGray,
                                        Me.UpPoint01From(Me.ToneWorkFactor))
            Me.ToneUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub ToneGrayMM_Click(sender As Object, e As RoutedEventArgs) _
        Handles ToneGrayMM.Click

        Try
            If Me.ToneWorkGray >= &H0 Then
                Me.UpdateBaseValuesFromTone(Me.Down17From(Me.ToneWorkGray),
                                            Me.ToneWorkFactor)
                Me.ToneUpdateVisuals()
            End If
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub ToneGrayM_Click(sender As Object, e As RoutedEventArgs) _
        Handles ToneGrayM.Click

        Try
            If Me.ToneWorkGray >= &H0 Then
                Me.UpdateBaseValuesFromTone(CByte(Me.ToneWorkGray - 1),
                                            Me.ToneWorkFactor)
                Me.ToneUpdateVisuals()
            End If
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub ToneGrayP_Click(sender As Object, e As RoutedEventArgs) _
        Handles ToneGrayP.Click

        Try
            If Me.ToneWorkGray < &HFF Then
                Me.UpdateBaseValuesFromTone(CByte(Me.ToneWorkGray + 1),
                                            Me.ToneWorkFactor)
                Me.ToneUpdateVisuals()
            End If
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

    Private Sub ToneGrayPP_Click(sender As Object, e As RoutedEventArgs) _
        Handles ToneGrayPP.Click

        Try
            Me.UpdateBaseValuesFromTone(Me.Up17From(Me.ToneWorkGray),
                                        Me.ToneWorkFactor)
            Me.ToneUpdateVisuals()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub

#End Region ' "Tone Tab Events"

#Region "Blend Tab Events"

    Private Sub BlendTabItem_GotFocus(sender As Object, e As RoutedEventArgs) _
        Handles BlendTabItem.GotFocus

        Try

            ' Only respond on a new arrival here.
            If (Me.LastFocusTab IsNot Nothing) AndAlso
            Me.LastFocusTab.Equals(sender) Then
                Exit Sub ' Early exit.
            End If
            Me.LastFocusTab =
                DirectCast(sender, System.Windows.Controls.TabItem)

            Me.BlendRgb1RedTextBox.Focus()
            With Me

                .BlendRgb1RedTextBox.Text = RgbWorkR.ToString
                .BlendRgb1GreenTextBox.Text = RgbWorkG.ToString
                .BlendRgb1BlueTextBox.Text = RgbWorkB.ToString
                .BlendRgb1RatioTextBox.Text =
                SHWV.BLENDINITIALRGBRATIO1.ToString

                .BlendRgb2RedTextBox.Text = RgbWorkR.ToString
                .BlendRgb2GreenTextBox.Text = RgbWorkG.ToString
                .BlendRgb2BlueTextBox.Text = RgbWorkB.ToString
                .BlendRgb2RatioTextBox.Text =
                SHWV.BLENDINITIALRGBRATIO2.ToString

            End With

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' BlendTabItem_GotFocus

    Private Sub BlendColorInfoTextBox_TextChanged(
        sender As Object, e As TextChangedEventArgs) _
        Handles BlendRgb1RedTextBox.TextChanged,
            BlendRgb1GreenTextBox.TextChanged,
            BlendRgb1BlueTextBox.TextChanged,
            BlendRgb1RatioTextBox.TextChanged,
            BlendRgb2RedTextBox.TextChanged,
            BlendRgb2GreenTextBox.TextChanged,
            BlendRgb2BlueTextBox.TextChanged,
            BlendRgb2RatioTextBox.TextChanged

        Try

            ' Avoid processing before all text boxes have been created in
            ' initialization.
            'If Not Me.WindowLoaded Then
            '    Exit Sub ' Early exit
            'End If
            If Not Me.IsLoaded Then
                Exit Sub ' Early exit
            End If

            Dim R1, G1, B1, R2, G2, B2 As System.Byte
            Dim Ratio1, Ratio2 As System.Double

            Dim AllValid As System.Boolean = True ' For now.
            With Me

                .BlendValidateRgb(BlendRgb1RedTextBox, R1, AllValid)
                .BlendValidateRgb(BlendRgb1GreenTextBox, G1, AllValid)
                .BlendValidateRgb(BlendRgb1BlueTextBox, B1, AllValid)
                .BlendValidateRatio(BlendRgb1RatioTextBox, Ratio1, AllValid)

                .BlendValidateRgb(BlendRgb2RedTextBox, R2, AllValid)
                .BlendValidateRgb(BlendRgb2GreenTextBox, G2, AllValid)
                .BlendValidateRgb(BlendRgb2BlueTextBox, B2, AllValid)
                .BlendValidateRatio(BlendRgb2RatioTextBox, Ratio2, AllValid)

            End With
            If Not AllValid Then
                ' Do not process further.
                Exit Sub ' Early exit.
            End If

            ' This appears AFTER the validation so that pushes will still set the
            ' good/bad color.
            If Me.ConvertTabPushing Then
                ' Do not process further.
                Exit Sub ' Early exit.
            End If

            ' Process the new values.
            With Me

                Dim NewR, NewG, NewB As System.Byte
                OSNW.Graphics.ColorUtilities.GetBlend(
                R1, G1, B1, Ratio1, R2, G2, B2, Ratio2, NewR, NewG, NewB)
                .BlendSetRgbWorkColors(NewR, NewG, NewB)

                .UpdatePreviewLabel()

            End With

        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' BlendColorInfoTextBox_TextChanged

#End Region ' "Blend Tab Events"

#Region "Bottom Buttons Events"

    Private Sub RememberButton_Click(sender As Object, e As RoutedEventArgs) _
        Handles RememberButton.Click

        Try

            With Me

                ' Save the basic components.
                .RememberR = .RgbWorkR
                .RememberG = .RgbWorkG
                .RememberB = .RgbWorkB

                ' Color, then activate, the restore button.
                With .RestoreButton
                    .Background = Me.RgbWorkSolidBrush
                    .Foreground = Me.RgbWorkContrastSolidBrush
                    .IsEnabled = True
                End With

            End With
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' RememberButton_Click

    Private Sub RestoreButton_Click(sender As Object, e As RoutedEventArgs) _
        Handles RestoreButton.Click

        Try
            Me.DoRestoreButtonClick()
        Catch CaughtEx As System.Exception
            ' Report the unexpected exception.
            Dim CaughtBy As System.Reflection.MethodBase =
                System.Reflection.MethodBase.GetCurrentMethod()
            OEH.ShowExceptionMessageBox(CaughtBy, CaughtEx, sender, e, Me)
        End Try
    End Sub ' RestoreButton_Click

#End Region ' "Bottom Buttons Events"

#End Region ' "Localized Events"

End Class ' ColorDlgWindow

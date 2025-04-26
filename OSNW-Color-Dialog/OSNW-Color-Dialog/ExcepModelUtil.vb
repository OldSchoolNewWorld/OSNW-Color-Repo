Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Imports System.Reflection

''' <summary>
''' Represents a <c>Class</c> that supplies functionality to respond to
''' <see cref="System.Exception"/>s.
''' </summary>
''' <remarks>
''' <c>ExceptionHandler</c> is designated as <c>Friend</c>; it is only directly
''' available to the associated <see cref="ColorDlgWindow"/> and <see cref="ColorDialog"/>.
''' </remarks>
Friend Class ExceptionHandler
    ' DEV: These routines are intended as part of the model.

#Region "Basic Sub/Function Model"
    ' DEV: This entire region can be copied into a new project as a supply of
    ' code that can be copied within that project. The copy can be reworked as
    ' chosen, or used to create specialized versions, then used during
    ' development. Once no longer needed, the entire region can be deleted.

    ' Private Sub SomeSub()
    '     ' DEV: The outer protective wrapper goes here. It ensures that
    '     ' unhandled exceptions, either local or in a subsequent call, are
    '     ' captured.
    '     Try
    '         ' DEV: The major intended operation goes here.
    ' 
    '         ' DEV: Expected-safe operations go here.
    '         ' Argument checking.
    '         ' Setups.
    ' 
    '         ' DEV: An inner protective wrapper goes here. Exceptions can be
    '         ' captured and dealt with where there is a better indication of
    '         ' where things went wrong. Multiple risky operations can be
    '         ' wrapped separately to further limit the scope of where a problem
    '         ' occured or to have different reactions in place.
    '         Try
    '             ' DEV: A risky operation goes here.
    ' 
    '         Catch CaughtEx As System.Exception
    '             ' BC30030 - Try must have at least one 'Catch' or a 'Finally'.
    ' 
    '             ' Respond to an anticipated exception.
    '             Dim CaughtBy As System.Reflection.MethodBase =
    '                 System.Reflection.MethodBase.GetCurrentMethod
    '             ExceptionHandler.ShowExceptionMessageBox(CaughtBy, CaughtEx)
    ' 
    '             ' Optional rethrow of the caught exception.
    '             'Throw
    ' 
    '         Finally
    '             ' BC30030 - Try must have at least one 'Catch' or a 'Finally'.
    '
    '             ' DEV: Do any clean-ups or back-outs here.
    '
    '         End Try
    ' 
    '         ' DEV: Expected-safe operations go here.
    ' 
    '     Catch CaughtEx As System.Exception
    '         ' Report the unexpected exception.
    '         Dim CaughtBy As System.Reflection.MethodBase =
    '             System.Reflection.MethodBase.GetCurrentMethod()
    '         ExceptionHandler.ShowExceptionMessageBox(CaughtBy, CaughtEx)
    '     End Try
    ' End Sub

#End Region ' "Basic Sub/Function Model"

#Region "Extractable Data"
    ' DEV: This entire region can be copied into a new project as a supply of
    ' code that can be copied into event- and exception-handlers. None of these
    ' routines are intended to be called directly. They are used to separate the
    ' variable assignments based on where they get their data.
    ' The left-side variable assignments can be left as shown, using the
    ' assigned variable in subsequent code or the right-side of the assignment
    ' can be pasted directly into the new code. When no longer needed, the
    ' entire region can be deleted.

    ''' <summary>
    ''' Gathers examples of extractable information in System.Exception in one
    ''' place.
    ''' </summary>
    ''' <param name="caughtEx"></param>
    ''' <remarks>
    ''' DEV: This is only intended as a supply of copyable code that will create
    ''' a local variable containing one property of <paramref name="caughtEx"/>.
    ''' That variable can then be referenced by local code or inspected while
    ''' debugging.
    ''' </remarks>
    Private Shared Sub ExtractException(ByVal caughtEx As System.Exception)

        ' The following are examples of information in System.Exception that can
        ' be examined to determine the cause of an exception.

        ' Gets a collection of key/value pairs that provide additional
        ' user-defined information about the exception. Data can be updated when
        ' an exception occurs. Parameters that were passed to a routine can be
        ' added to help identify edge cases, etc. that can cause a problem.
        Dim CaughtExData As System.Collections.IDictionary = caughtEx.Data

        ' Recursive - another exception. Returns the Exception that is the root
        ' cause of one or more subsequent exceptions.
        Dim CaughtExBaseException As System.Exception =
            caughtEx.GetBaseException

        ' The type of exception. Gets the runtime type of the current instance.
        Dim CaughtExType As Type = caughtEx.GetType

        ' Gets the name of the type of exception.
        Dim CaughtExTypeName As System.String = CaughtExType.Name

        ' Gets the full name of the type of exception.
        Dim CaughtExTypeFullname As System.String = CaughtExType.FullName

        ' Recursive - another exception. Gets the Exception instance that
        ' caused the current exception.
        Dim CaughtExInnerException As System.Exception = caughtEx.InnerException

        ' Gets a message that describes the current exception.
        Dim CaughtExMessage As System.String = caughtEx.Message

        ' Gets the name of the application or the object that causes the error.
        Dim CaughtExSource As System.String = caughtEx.Source

        ' Gets a string representation of the immediate frames on the call
        ' stack.
        Dim CaughtExStackTrace As System.String = caughtEx.StackTrace

        ' Gets the method that throws the current exception.
        Dim CaughtExTargetSite As System.Reflection.MethodBase =
            caughtEx.TargetSite

        ' Creates and returns a string representation of the current exception.
        Dim CaughtExToString As System.String =
            caughtEx.ToString 'System.Exception: Forced exception

    End Sub ' ExtractException

    ''' <summary>
    ''' Gathers examples of extractable information in System.EventArgs in one
    ''' place.
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' DEV: This is only intended as a supply of copyable code that will create
    ''' a local variable containing one property of <paramref name="e"/>. That
    ''' variable can then be referenced by local code or inspected while
    ''' debugging.
    ''' <para>
    ''' <see cref="System.EventArgs"/> is a base class for other event types.
    ''' <see cref="System.EventArgs"/> has more than 30 derived types that may
    ''' provide additional specific information of interest.
    ''' </para>
    ''' </remarks>
    Private Shared Sub ExtractEventArgs(ByVal e As System.EventArgs)

        ' The following are examples of information in System.EventArgs that can
        ' be examined to determine the conditions that caused an exception.

        ' Gets the Type of the current instance.
        Dim EType As Type = e.GetType

        ' Gets the name of the current member.
        Dim ETypeName As System.String = EType.Name

        ' Gets the fully qualified name of the type, including its namespace but
        ' not its assembly.
        Dim ETypeFullName As System.String = EType.FullName ' System.EventArgs

        ' Returns a string that represents the current object.
        Dim ETypeToString As System.String = e.ToString

    End Sub ' ExtractEventArgs

    ''' <summary>
    ''' Gathers examples of extractable information in
    ''' System.Windows.RoutedEventArgs in one place.
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' DEV: This is only intended as a supply of copyable code that will create
    ''' a local variable containing one property of <paramref name="e"/>. That
    ''' variable can then be referenced by local code or inspected while
    ''' debugging.
    ''' <para>
    ''' A <see cref="System.Windows.RoutedEventArgs"/> contains information that
    ''' a <see cref="System.EventArgs"/> does not have.
    ''' <see cref="System.Windows.RoutedEventArgs"/> has many derived types that
    ''' may provide additional specific information of interest.
    ''' </para>
    ''' </remarks>
    Private Shared Sub ExtractRoutedEventArgs(
        ByVal e As System.Windows.RoutedEventArgs)

        ' The following are examples of unique information in
        ' System.Windows.RoutedEventArgs that can be examined to determine the
        ' conditions that caused an exception.

        ' Gets or sets a value that indicates the present state of the event
        ' handling for a routed event as it travels the route.
        Dim EHandled As System.Boolean = e.Handled

        ' Gets the original reporting source as determined by pure hit testing,
        ' before any possible Source adjustment by a parent class.
        Dim EOriginalSource As Object = e.OriginalSource

        ' Gets or sets the RoutedEvent associated with this RoutedEventArgs
        ' instance.
        Dim ERoutedEvent As System.Windows.RoutedEvent = e.RoutedEvent

        ' Gets or sets a reference to the object that raised the event.
        Dim ESource As Object = e.Source

    End Sub ' ExtractRoutedEventArgs

    ''' <summary>
    ''' Gathers examples of extractable information in
    ''' System.ComponentModel.CancelEventArgs in one place.
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' DEV: This is only intended as a supply of copyable code that will create
    ''' a local variable containing one property of <paramref name="e"/>. That
    ''' variable can then be referenced by local code or inspected while
    ''' debugging.
    ''' <para>
    ''' A <see cref="System.ComponentModel.CancelEventArgs"/> contains
    ''' information that a <see cref="System.EventArgs"/> does not have.
    ''' <see cref="System.ComponentModel.CancelEventArgs"/> has many derived
    ''' types that may provide additional specific information of interest.
    ''' </para>
    ''' </remarks>
    Private Shared Sub ExtractCancelEventArgs(
        ByVal e As System.ComponentModel.CancelEventArgs)

        ' Unique information in System.ComponentModel.CancelEventArgs that can
        ' be examined to determine the cause of an exception and where it occurred.

        ' Gets or sets a value indicating whether the event should be canceled.
        Dim ECancel As System.Boolean = e.Cancel

    End Sub ' ExtractCancelEventArgs

#End Region ' "Extractable Data"

#Region "Message Box Utilities"
    ' These are utilities used by various versions of
    ' ShowExceptionMessageBox(<params>).

    ''' <summary>
    ''' Extracts the <c>Data</c> entries from the specified <c>Exception</c>.
    ''' </summary>
    ''' <param name="caughtEx">Specifies the <c>Exception</c> from which to
    ''' extract the <c>Data</c> pairs.</param>
    ''' <returns>A string containing the data.</returns>
    ''' <remarks>
    ''' DEV: This can be modified to report the data in any chosen form, or
    ''' multiple versions can be created to align with the type(s) of the data
    ''' provided.
    ''' </remarks>
    Private Shared Function GetDataMessage(ByVal caughtEx As System.Exception) _
        As System.String

        Dim IsFirstDataPair As System.Boolean = True
        Dim SBuilder As New System.Text.StringBuilder
        For Each OneDictionaryEntry As _
            System.Collections.DictionaryEntry In caughtEx.Data

            If Not IsFirstDataPair Then
                SBuilder.Append("; ")
            End If
            IsFirstDataPair = False

            SBuilder.Append($"{OneDictionaryEntry.Key}")
            SBuilder.Append($"={OneDictionaryEntry.Value}")

        Next
        Return SBuilder.ToString
    End Function ' GetDataMessage

    ''' <summary>
    ''' Reports an invalid call to one of the
    ''' <c>ShowExceptionMessageBox(&lt;varies&gt;)</c> implementations.
    ''' </summary>
    ''' <param name="paramName">Specifies the name of the parameter that was
    ''' invalid.</param>
    ''' <param name="reason">Specifies the reason for the rejection.</param>
    ''' <param name="owner">Optional. Specifies the Window that should own the
    ''' MessageBox containing the exception details. When
    ''' <paramref name="owner"/> is not specified, the MessageBox is owned by
    ''' the window that is currently active.</param>
    ''' <remarks>This is for invalid calls to
    ''' <c>ShowExceptionMessageBox(&lt;varies&gt;)</c>, not for generic invalid
    ''' procedure calls.</remarks>
    Private Shared Sub ShowExceptionArgNotice(ByVal paramName As System.String,
        ByVal reason As System.String,
        ByVal Optional owner As System.Windows.Window = Nothing)

        Dim CaptionStr As System.String = "Invalid ShowExceptionMessageBox"
        Dim IntroDetails As System.String = System.String.Concat(
            "An invalid exception notice was requested.",
            $" There is a problem with '{paramName}'.")

        ' Construct and show the notice.
        Dim ShownDetail As System.String = System.String.Concat(IntroDetails,
            System.Environment.NewLine, System.Environment.NewLine, reason)
        If owner Is Nothing Then
            ' Show without owner.
            ' REF: https://learn.microsoft.com/en-us/dotnet/api/system.windows.messagebox.show?view=windowsdesktop-9.0#remarks
            ' Use an overload of the Show method, which enables you to specify
            ' an owner window. Otherwise, the message box is owned by the window
            ' that is currently active.
            System.Windows.MessageBox.Show(ShownDetail, CaptionStr,
                                           System.Windows.MessageBoxButton.OK,
                                           System.Windows.MessageBoxImage.Error)
        Else
            ' Use Me.Owner.
            System.Windows.MessageBox.Show(owner, ShownDetail, CaptionStr,
                                           System.Windows.MessageBoxButton.OK,
                                           System.Windows.MessageBoxImage.Error)
        End If

    End Sub ' ShowExceptionArgNotice

    ''' <summary>
    ''' Provides a consistent generic appearance for messages.
    ''' </summary>
    ''' <param name="captionStr">Specifies the caption to show on the 
    ''' <c>MessageBox</c>.</param>
    ''' <param name="introDetails">Specifies a summary of the exception.</param>
    ''' <param name="techDetails">Specifies detailed information about the 
    ''' exception.</param>
    ''' <param name="owner">Optional. Specifies the Window that should own the
    ''' MessageBox containing the exception details. When
    ''' <paramref name="owner"/> is not specified, the MessageBox is owned by
    ''' the window that is currently active.</param>
    ''' <remarks>
    ''' This is mainly intended for exceptions caught in the outer layer of the
    ''' model. It provides high-level detail regarding where to look for
    ''' problems. It can also be used by the inner layer if specific information
    ''' is provided in <paramref name="techDetails"/>.
    ''' </remarks>
    Private Shared Sub ShowExceptionNotice(ByVal captionStr As System.String,
        ByVal introDetails As System.String, ByVal techDetails As System.String,
        ByVal Optional owner As System.Windows.Window = Nothing)

        ' Construct and show the notice.
        Dim ShownDetail As System.String = System.String.Concat(introDetails,
            System.Environment.NewLine, System.Environment.NewLine, techDetails)
        If owner Is Nothing Then
            ' Show without owner.
            ' REF: https://learn.microsoft.com/en-us/dotnet/api/system.windows.messagebox.show?view=windowsdesktop-9.0#remarks
            ' Use an overload of the Show method, which enables you to specify
            ' an owner window. Otherwise, the message box is owned by the window
            ' that is currently active.
            System.Windows.MessageBox.Show(ShownDetail, captionStr,
                                           System.Windows.MessageBoxButton.OK,
                                           System.Windows.MessageBoxImage.Error)
        Else
            ' Use Me.Owner.
            System.Windows.MessageBox.Show(owner, ShownDetail, captionStr,
                                           System.Windows.MessageBoxButton.OK,
                                           System.Windows.MessageBoxImage.Error)
        End If
    End Sub ' ShowExceptionNotice

#End Region ' "Message Box Utilities"

#Region "Exception Message Box"

    ''' <summary>
    ''' Shows details about an <c>Exception</c> that was caught.
    ''' </summary>
    ''' <param name="caughtBy">Specifies the process in which an exception was
    ''' caught.</param>
    ''' <param name="caughtEx">Provides the exception that was caught.</param>
    ''' <param name="owner">Optional. Specifies the Window that should own the
    ''' MessageBox containing the exception details. When
    ''' <paramref name="owner"/> is not specified, the MessageBox is owned by
    ''' the window that is currently active.</param>
    ''' <remarks>
    ''' This is a generic version of
    ''' <c>ShowExceptionMessageBox(&lt;params&gt;)</c>. Other overloaded
    ''' versions take additional parameters that can provide more information
    ''' about the conditions that led to the exception.
    ''' </remarks>
    Public Shared Sub ShowExceptionMessageBox(
        ByVal caughtBy As System.Reflection.MethodBase,
        ByVal caughtEx As System.Exception,
        ByVal Optional owner As System.Windows.Window = Nothing)

        ' Argument checking.
        If caughtEx Is Nothing Then
            ShowExceptionArgNotice(NameOf(caughtEx),
                $"'{NameOf(caughtEx)}' cannot be 'Nothing'/'Null'.", owner)
            Exit Sub ' Early exit.
        End If
        Dim CaughtByName As System.String = If(caughtBy Is Nothing,
            $"Unspecified '{NameOf(caughtBy)}'", caughtBy.Name)

        ' Gather information of interest.
        Dim CaughtExMessage As System.String = caughtEx.Message
        Dim CaughtExBaseException As System.Exception =
            caughtEx.GetBaseException
        Dim IntroDetails As System.String =
            $"'{CaughtByName}' failed with message '{CaughtExMessage}'."
        Dim TechDetails As System.String =
            $"The initial cause is {CaughtExBaseException}."

        ' Construct and show the notice.
        Dim Caption As System.String = "Process Failure"
        ShowExceptionNotice(Caption, IntroDetails, TechDetails, owner)

    End Sub ' ShowExceptionMessageBox

    ''' <summary>
    ''' Shows details about an <c>Exception</c> that was caught, when conditions
    ''' data is reported.
    ''' </summary>
    ''' <param name="caughtBy">Specifies the process in which an exception was
    ''' caught.</param>
    ''' <param name="caughtEx">Provides the exception that was caught.</param>
    ''' <param name="dataMessage"></param>Provides a message about conditions
    ''' where the exception occurred.
    ''' <param name="owner">Optional. Specifies the Window that should own the
    ''' MessageBox containing the exception details. When
    ''' <paramref name="owner"/> is not specified, the MessageBox is owned by
    ''' the window that is currently active.</param>
    ''' <remarks>
    ''' See
    ''' <see cref="ShowExceptionMessageBox(System.Reflection.MethodBase,
    ''' System.Exception, System.Windows.Window)"/> for information common
    ''' across exception types.
    ''' </remarks>
    Public Shared Sub ShowExceptionMessageBox(
        ByVal caughtBy As System.Reflection.MethodBase,
        ByVal caughtEx As System.Exception, ByVal dataMessage As System.String,
        ByVal Optional owner As System.Windows.Window = Nothing)

        ' Argument checking.
        If caughtEx Is Nothing Then
            ShowExceptionArgNotice(NameOf(caughtEx),
                $"'{NameOf(caughtEx)}' cannot be 'Nothing'/'Null'.", owner)
            Exit Sub ' Early exit.
        End If
        Dim CaughtByName As System.String = If(caughtBy Is Nothing,
            $"Unspecified '{NameOf(caughtBy)}'", caughtBy.Name)

        ' Gather information of interest.
        Dim CaughtExBaseException As System.Exception =
            caughtEx.GetBaseException
        Dim IntroDetails As System.String =
            $"'{CaughtByName}' failed with message" &
            $" '{caughtEx.Message}'" &
            $" and data:{dataMessage}."
        Dim TechDetails As System.String =
            $"The initial cause is {CaughtExBaseException}."

        ' Construct and show the notice.
        Dim Caption As System.String = "Process Failure"
        ShowExceptionNotice(Caption, IntroDetails, TechDetails, owner)

    End Sub ' ShowExceptionMessageBox

    ''' <summary>
    ''' Shows details about an <c>Exception</c> that was caught by an event
    ''' associated with an <c>Object</c>.
    ''' </summary>
    ''' <param name="caughtBy">Specifies the process in which an exception was
    ''' caught.</param>
    ''' <param name="caughtEx">Provides the exception that was caught.</param>
    ''' <param name="sender">Specifies the <c>Object</c> that sent the
    ''' event.</param>
    ''' <param name="e">Specifies arguments that are associated with the
    ''' event.</param>
    ''' <param name="owner">Optional. Specifies the Window that should own the
    ''' MessageBox containing the exception details. When
    ''' <paramref name="owner"/> is not specified, the MessageBox is owned by
    ''' the window that is currently active.</param>
    ''' <remarks>
    ''' <para>
    ''' This is a generic version of ShowExceptionMessageBox(&lt;params&gt;)
    ''' that handles exceptions associated with an event. Other overloaded
    ''' versions take more specific parameters that can provide more information
    ''' about the conditions that led to the exception.
    ''' </para>
    ''' <para>
    ''' <see cref="System.EventArgs"/> is a base class for other event types.
    ''' <see cref="System.EventArgs"/> has more than 30 derived types that may
    ''' provide even more specific information.
    ''' </para>
    ''' </remarks>
    Public Shared Sub ShowExceptionMessageBox(
        ByVal caughtBy As System.Reflection.MethodBase,
        ByVal caughtEx As System.Exception,
        ByVal sender As Object, ByVal e As System.EventArgs,
        ByVal Optional owner As System.Windows.Window = Nothing)

        ' Argument checking.
        If caughtEx Is Nothing Then
            ShowExceptionArgNotice(NameOf(caughtEx),
                $"'{NameOf(caughtEx)}' cannot be 'Nothing'/'Null'.", owner)
            Exit Sub ' Early exit.
        End If
        Dim CaughtByName As System.String = If(caughtBy Is Nothing,
            $"Unspecified '{NameOf(caughtBy)}'", caughtBy.Name)
        Dim SenderName As System.String = If(sender Is Nothing,
            $"Unspecified '{NameOf(sender)}'", sender.ToString)
        Dim EText As System.String = If(e Is Nothing,
            "Unspecified EventArgs", e.ToString)

        ' Gather information of interest.
        Dim CaughtExBaseException As System.Exception =
            caughtEx.GetBaseException
        Dim IntroDetails As System.String =
            $"An exception was caught in '{CaughtByName}'" &
            $", with sender='{sender}'" & $" and message '{caughtEx.Message}'."
        Dim TechDetails As System.String =
            $"The initial cause is {CaughtExBaseException}."

        ' Construct and show the notice.
        Dim Caption As System.String = "Initialization Event Failure"
        ShowExceptionNotice(Caption, IntroDetails, TechDetails, owner)

    End Sub ' ShowExceptionMessageBox

    ''' <summary>
    ''' Shows details about an <c>Exception</c> that was caught by an event, for
    ''' an <c>Object</c>, that provides a System.Windows.RoutedEventArgs.
    ''' </summary>
    ''' <param name="caughtBy">Specifies the process in which an exception was
    ''' caught.</param>
    ''' <param name="caughtEx">Provides the exception that was caught.</param>
    ''' <param name="sender">Specifies the <c>Object</c> that sent the
    ''' event.</param>
    ''' <param name="e">Specifies arguments that are associated with the
    ''' event.</param>
    ''' <param name="owner">Optional. Specifies the Window that should own the
    ''' MessageBox containing the exception details. When
    ''' <paramref name="owner"/> is not specified, the MessageBox is owned by
    ''' the window that is currently active.</param>
    ''' <remarks>
    ''' A <see cref="System.Windows.RoutedEventArgs"/> contains information that
    ''' a <see cref="System.EventArgs"/> does not have.
    ''' <see cref="System.Windows.RoutedEventArgs"/> has many derived types.
    ''' </remarks>
    Public Shared Sub ShowExceptionMessageBox(
        ByVal caughtBy As System.Reflection.MethodBase,
        ByVal caughtEx As System.Exception, ByVal sender As Object,
        ByVal e As System.Windows.RoutedEventArgs,
        ByVal Optional owner As System.Windows.Window = Nothing)

        ' Argument checking.
        If caughtEx Is Nothing Then
            ShowExceptionArgNotice(NameOf(caughtEx),
                $"'{NameOf(caughtEx)}' cannot be 'Nothing'/'Null'.", owner)
            Exit Sub ' Early exit.
        End If
        Dim CaughtByName As System.String = If(caughtBy Is Nothing,
            $"Unspecified '{NameOf(caughtBy)}'", caughtBy.Name)
        Dim SenderName As System.String = If(sender Is Nothing,
            $"Unspecified '{NameOf(sender)}'", sender.ToString)
        Dim EText As System.String = If(e Is Nothing,
            "Unspecified RoutedEventArgs", e.RoutedEvent.ToString)

        ' Gather information of interest.
        Dim CaughtExBaseException As System.Exception =
            caughtEx.GetBaseException
        Dim IntroDetails As System.String =
            $"An exception was caught in '{CaughtByName}'" &
            $", with sender='{sender}'" & $" and message '{caughtEx.Message}'."
        Dim TechDetails As System.String =
            $"The initial cause is {CaughtExBaseException}."

        ' Construct and show the notice.
        Dim Caption As System.String = "Routed Event Failure"
        ShowExceptionNotice(Caption, IntroDetails, TechDetails, owner)

    End Sub ' ShowExceptionMessageBox

    ''' <summary>
    ''' Shows details about an <c>Exception</c> that was caught by an event, for
    ''' an <c>Object</c>, that provides a System.ComponentModel.CancelEventArgs.
    ''' </summary>
    ''' <param name="caughtBy">Specifies the process in which an exception was
    ''' caught.</param>
    ''' <param name="caughtEx">Provides the exception that was caught.</param>
    ''' <param name="sender">Specifies the <c>Object</c> that sent the
    ''' event.</param>
    ''' <param name="e">Specifies arguments that are associated with the
    ''' event.</param>
    ''' <param name="owner">Optional. Specifies the Window that should own the
    ''' MessageBox containing the exception details. When
    ''' <paramref name="owner"/> is not specified, the MessageBox is owned by
    ''' the window that is currently active.</param>
    ''' <remarks>
    ''' A <see cref="System.ComponentModel.CancelEventArgs"/> contains
    ''' information that a <see cref="System.EventArgs"/> does not have.
    ''' </remarks>
    Public Shared Sub ShowExceptionMessageBox(
        ByVal caughtBy As System.Reflection.MethodBase,
        ByVal caughtEx As System.Exception, ByVal sender As Object,
        ByVal e As System.ComponentModel.CancelEventArgs,
        ByVal Optional owner As System.Windows.Window = Nothing)

        ' Argument checking.
        If caughtEx Is Nothing Then
            ShowExceptionArgNotice(NameOf(caughtEx),
                $"'{NameOf(caughtEx)}' cannot be 'Nothing'/'Null'.", owner)
            Exit Sub ' Early exit.
        End If
        Dim CaughtByName As System.String = If(caughtBy Is Nothing,
            $"Unspecified '{NameOf(caughtBy)}'", caughtBy.Name)
        Dim SenderName As System.String = If(sender Is Nothing,
            $"Unspecified '{NameOf(sender)}'", sender.ToString)
        Dim EText As System.String = If(e Is Nothing,
            "Unspecified CancelEventArgs", e.ToString)

        ' Gather information of interest.
        Dim CaughtExBaseException As System.Exception =
            caughtEx.GetBaseException
        Dim IntroDetails As System.String =
            $"An exception was caught in '{CaughtByName}'" &
            $", with sender='{sender}'" & $" and message '{caughtEx.Message}'."
        Dim TechDetails As System.String =
            $"The initial cause is {CaughtExBaseException}."

        ' Construct and show the notice.
        Dim Caption As System.String = "Cancel Event Failure"

        ShowExceptionNotice(Caption, IntroDetails, TechDetails, owner)

    End Sub ' ShowExceptionMessageBox

#End Region ' "Exception Message Box"

End Class ' ExceptionHandler
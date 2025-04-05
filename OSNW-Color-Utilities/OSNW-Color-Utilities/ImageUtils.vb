Option Explicit On
Option Strict On
Option Compare Binary
Option Infer Off

Partial Public Class ColorUtilities

    ' This file contains routines to work with a System.Windows.Controls.Image.
    ' It should eventually be moved into a different namespace arrangement where
    ' color features and image features are both subs of an overall graphics
    ' namespace.

    ''' <summary>
    ''' Creates a source for a <c>System.Windows.Controls.Image</c> and fills
    ''' it with a specified array of pixels.
    ''' </summary>
    ''' <param name="pixels">Specifies the array of pixels.</param>
    ''' <returns>Returns a <c>System.Windows.Media.Imaging.BitmapSource</c>
    ''' based on the pixels specified by <paramref name="pixels"/>.</returns>
    Public Shared Function PixelsToImageSource(pixels As System.Int32(,)) As _
        System.Windows.Media.Imaging.BitmapSource

        ' Ref: WPF: Looking for a fast method to display a rgb pixel array
        ' (byte) to canvas to create an image
        ' https://stackoverflow.com/questions/46718174/wpf-looking-for-a-fast-method-to-display-a-rgb-pixel-array-byte-to-canvas-to

        Dim PixelWidth As System.Int32 = pixels.GetUpperBound(0) + 1
        Dim PixelHeight As System.Int32 = pixels.GetUpperBound(1) + 1

        Dim WritableImg As New System.Windows.Media.Imaging.WriteableBitmap(
            PixelWidth, PixelHeight, 96, 96,
            System.Windows.Media.PixelFormats.Bgra32, Nothing)

        ' Lock the buffer.
        WritableImg.Lock()
        Try

            Dim BackBufferBasePtr As System.IntPtr = WritableImg.BackBuffer
            Dim BackBufferPtr As System.IntPtr
            Dim BackBufferStrideOnce As System.Int32 =
                WritableImg.BackBufferStride
            For Row As System.Int32 = 0 To PixelWidth - 1
                For Col As System.Int32 = 0 To PixelHeight - 1
                    ' The buffer is a monodimensionnal array.
                    BackBufferPtr = BackBufferBasePtr +
                        (Col * BackBufferStrideOnce) + (Row * 4)
                    System.Runtime.InteropServices.Marshal.WriteInt32(
                        BackBufferPtr, pixels(Row, Col))
                Next
            Next

            ' Specify the area to update.
            WritableImg.AddDirtyRect(
                New System.Windows.Int32Rect(0, 0, PixelWidth, PixelHeight))

        Finally
            ' Release the buffer.
            WritableImg.Unlock()
        End Try

        Return WritableImg

    End Function ' PixelsToImage

End Class ' ColorUtilities

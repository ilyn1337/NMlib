Imports NMlibVB
Public Class Form1
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        NMlib64VB.NeutronMemoryLibrary.IslemEkle("İŞLEM İSMİ")
        If NMlib64VB.NeutronMemoryLibrary.ProcessRunning = True Then
            label1.Text = "İşlem çalışıyor."
        Else
            label1.Text = "İşlem çalışmıyor."
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles checkBox1.CheckedChanged
        NMlib64VB.NeutronMemoryLibrary.OpcodeKullan(&H18B59E, "90 90 90 90")
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Timer1.Start()
    End Sub

    Private Sub TrackBar1_Scroll_1(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        NMlib64VB.NeutronMemoryLibrary.PointerKullan(&H3B4FA0, &HB10, &H180, &H219, 1)
    End Sub
End Class

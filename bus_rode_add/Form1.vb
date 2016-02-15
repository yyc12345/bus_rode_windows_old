Public Class Form1
    Public t_a As New Threading.Thread(AddressOf add_res)

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        t_a.Start()
    End Sub

    Public Sub add_res()
        Threading.Thread.Sleep(5000)
        If Command() = "" Then
            GoTo aaa
        End If
        Dim leibie As Integer = Int(Command)
        Select Case leibie
            Case 0
                '安装主资源
                System.IO.File.Delete(Application.StartupPath + "\library\bus.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\have_bus.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\readme.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\short_line.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\short_stop.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\station.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\stop.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\subway.txt")
                bus_rode_compression.bus_rode_compression.DeCompress(Application.StartupPath + "\temp.brs", Application.StartupPath + "\library\")
                System.IO.File.Delete(Application.StartupPath + "\temp.brs")

            Case 1
                '安装图片

                If System.IO.File.Exists(Application.StartupPath + "\temp.jpg") = True Then
                    System.IO.File.Delete(Application.StartupPath + "\background.jpg")
                    System.IO.File.Move(Application.StartupPath + "\temp.jpg", Application.StartupPath + "\background.jpg")
                    System.IO.File.Delete(Application.StartupPath + "\temp.jpg")
                End If

            Case 2
                '安装dll

                If System.IO.File.Exists(Application.StartupPath + "\temp.dll") = True Then
                    System.IO.File.Delete(Application.StartupPath + "\bus_rode_mod.dll")
                    System.IO.File.Move(Application.StartupPath + "\temp.dll", Application.StartupPath + "\bus_rode_mod.dll")
                    System.IO.File.Delete(Application.StartupPath + "\temp.dll")
                End If

            Case 3
                '删除dll

                System.IO.File.Delete(Application.StartupPath + "\bus_rode_mod.dll")

            Case 4
                '删除资源

                System.IO.File.Delete(Application.StartupPath + "\library\bus.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\have_bus.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\readme.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\short_line.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\short_stop.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\station.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\stop.txt")
                System.IO.File.Delete(Application.StartupPath + "\library\subway.txt")

        End Select
        Shell("bus_rode.exe")
aaa:

        Application.Exit()
    End Sub
End Class

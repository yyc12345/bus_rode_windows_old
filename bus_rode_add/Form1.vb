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

        '分离版本号和指令
        '版本号
        Dim bus_rode_version As Long = Int(Command) / 10
        '指令
        Dim leibie As Long = CType(Int(Command), Long) - bus_rode_version * 10

        Select Case leibie
            Case 0
                '安装主资源
                '在解压里面已设置相关函数，无需处理
                'System.IO.File.Delete(Application.StartupPath + "\library\bus.txt")
                'System.IO.File.Delete(Application.StartupPath + "\library\have_bus.txt")
                'System.IO.File.Delete(Application.StartupPath + "\library\readme.txt")
                'System.IO.File.Delete(Application.StartupPath + "\library\short_line.txt")
                'System.IO.File.Delete(Application.StartupPath + "\library\short_stop.txt")
                'System.IO.File.Delete(Application.StartupPath + "\library\station.txt")
                'System.IO.File.Delete(Application.StartupPath + "\library\stop.txt")
                'System.IO.File.Delete(Application.StartupPath + "\library\subway.txt")
                Dim result As Boolean = bus_rode_compression.bus_rode_compression.Decompress(Application.StartupPath + "\temp.brs", Application.StartupPath + "\library\", bus_rode_version)
                If result = False Then
                    MsgBox("在解包的过程中出现一些错误，可能是由于包不是一个可用的包或者是包文件已损坏。您现在使用的资源文件已经被清空，请尝试从设置中再次安装包。", 16, "解包错误")
                End If
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

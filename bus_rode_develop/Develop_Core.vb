Module Develop_Core

    ''' <summary>
    ''' 工作模式 0=没有打开文件 1=已经打开文件
    ''' </summary>
    Public work_mode As Integer = 0
    ''' <summary>
    ''' 编译状态 0=没有编译 1=已经编译
    ''' </summary>
    Public build_mode As Integer = 0

    ''' <summary>
    ''' 指定项目的路径,带\符号
    ''' </summary>
    Public project_path As String = ""

    ''' <summary>
    ''' 预设9个
    ''' </summary>
    Public desktop(9) As String

    '==================================================内存中的数据备份
    ''' <summary>
    ''' 内存中的数据备份-总站(站台序列，0=名称 1-4各个叙述)
    ''' </summary>
    Public back_station(500, 4) As String
    ''' <summary>
    ''' 地铁(地铁序列，0=出口名 1=出口文字按回车分割)
    ''' </summary>
    Public back_subway(1000, 1) As String
    ''' <summary>
    ''' 描述出口（序号，0=名字 1=叙述）
    ''' </summary>
    Public back_exit(20, 1) As String
    ''' <summary>
    ''' 上一个选择的地铁出口序号
    ''' </summary>
    Public before_subway_stop As Integer = -1


#Region "预设"

    ''' <summary>
    ''' 读取预设
    ''' </summary>
    Public Sub read_desktop()
        Dim file As New System.IO.StreamReader(Environment.CurrentDirectory & "\desktop.txt", System.Text.Encoding.UTF8)

        'line里的5个设置
        desktop(0) = file.ReadLine
        desktop(1) = file.ReadLine
        desktop(2) = file.ReadLine
        desktop(3) = file.ReadLine
        desktop(4) = file.ReadLine

        'stop
        desktop(5) = file.ReadLine
        desktop(6) = file.ReadLine
        desktop(7) = file.ReadLine
        desktop(8) = file.ReadLine

        'setting
        If file.ReadLine = "0" Then
            '开启
            setting_save = True
        Else
            '关闭
            setting_save = False
        End If
        If file.ReadLine = "0" Then
            '开启
            setting_time = True
        Else
            '关闭
            setting_time = False
        End If

        file.Dispose()
    End Sub

    ''' <summary>
    ''' 保存预设
    ''' </summary>
    Public Sub save_desktop()
        Dim file As New System.IO.StreamWriter(Environment.CurrentDirectory & "\desktop.txt", False, System.Text.Encoding.UTF8)

        file.WriteLine(desktop(0))
        file.WriteLine(desktop(1))
        file.WriteLine(desktop(2))
        file.WriteLine(desktop(3))
        file.WriteLine(desktop(4))
        file.WriteLine(desktop(5))
        file.WriteLine(desktop(6))
        file.WriteLine(desktop(7))
        file.WriteLine(desktop(8))
        If setting_save = True Then
            file.WriteLine("0")
        Else
            file.WriteLine("1")
        End If
        If setting_time = True Then
            file.WriteLine("0")
        Else
            file.WriteLine("1")
        End If

        file.Dispose()
    End Sub


#End Region

    ''' <summary>
    ''' 清除数据
    ''' </summary>
    Public Sub clear_data()
        For a = 0 To 1000
            For b = 0 To 1
                back_subway(a, b) = ""
            Next
        Next

        For c = 0 To 500
            For d = 0 To 4
                back_station(c, d) = ""
            Next
        Next

        For f = 0 To 20
            For g = 0 To 1
                back_exit(f, g) = ""
            Next
        Next

        before_subway_stop = -1
    End Sub

    ''' <summary>
    ''' 检查工程是否编译
    ''' </summary>
    ''' <returns></returns>
    Public Function check_build()
        Dim result As Boolean = True

        If Not (System.IO.File.Exists(project_path + "subway.txt")) Then
            result = False
        End If
        If Not (System.IO.File.Exists(project_path + "stop.txt")) Then
            result = False
        End If
        If Not (System.IO.File.Exists(project_path + "station.txt")) Then
            result = False
        End If

        Return result
    End Function


End Module

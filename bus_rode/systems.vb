Module systems

    '常量
    Public Const app_version As String = "9.0.0.0"
    Public Const app_build As String = "build 9000"
    Public Const app_name As String = "bus_rode Helium"
    Public Const app_update_date As String = "9:00 2016/2/7"
    Public Const app_build_number As Integer = 9000

    Public Const app_use_baidu_ak As String = "GyEb8Gs7DYaCuiKEsb5GIs9N"


    '************************************************************************文件读取部分************************************************************

    ''' <summary>
    ''' [系统]初始化函数
    ''' </summary>
    Public Sub start()
        '初始化
        For test1 = 0 To 1000
            have_bus(test1) = ""
        Next

        For test2 = 0 To 200
            bus_stop_up(test2) = ""
            bus_stop_down(test2) = ""
        Next

        For test3 = 0 To 5
            bus_msg(test3) = ""
        Next

        For test4 = 0 To 2
            For test5 = 0 To 200
                mid_bus(test4, test5).number = 0
                mid_bus(test4, test5).message_1 = ""
                mid_bus(test4, test5).message_2 = ""
                mid_bus(test4, test5).message_3 = ""
            Next
        Next

        For test6 = 0 To 10000
            bus_stop_stop(test6) = ""

        Next

    End Sub

    ''' <summary>
    ''' [系统]加载核心资源文件
    ''' </summary>
    Public Sub add_system()
        '加载核心文件
        '拥有车辆
        Dim file As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\have_bus.txt", System.Text.Encoding.UTF8)
        Dim word As String
        Dim number As Integer = 0

        '设置ui
        Dim dd As New ui_depend_line_list
        ui_connet_core_form_line_list.Clear()

        Do
            word = file.ReadLine
            If word = "END" Then
                Exit Do
            Else
                have_bus(number) = word
                '设置ui
                dd.ui_line_name = word
                ui_connet_core_form_line_list.Add(dd)
                dd = New ui_depend_line_list

                number = number + 1
            End If
        Loop
        file.Dispose()

        '设置初始站台,以第一个线路初始化
        bus = have_bus(0)
        read_bus()
        read_mid_bus_word = ""
        read_mid_bus()
        look_bus_date()


        '站台
        Dim file_4 As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\stop.txt", System.Text.Encoding.UTF8)

        word = ""
        number = 0

        '设置ui
        Dim ff As New ui_depend_stop_list
        ui_connet_core_form_stop_list.Clear()

        Do
            word = file_4.ReadLine
            If word = "" Then
                Exit Do
            End If

            bus_stop_stop(number) = word
            '设置ui
            ff.ui_stop_name = word
            ui_connet_core_form_stop_list.Add(ff)
            ff = New ui_depend_stop_list

            file_4.ReadLine()
            file_4.ReadLine()
            number = number + 1
        Loop
        file_4.Dispose()

        '初始化站台
        refsh_stop()

        Dim file_7 As New System.IO.StreamReader(Environment.CurrentDirectory + "\desktop.txt", System.Text.Encoding.UTF8)

        If file_7.ReadLine = "0" Then
            get_bus_addr = True
            '已经启动无需更改
        Else
            get_bus_addr = False
            '在外部实现
        End If

        '仅仅加载实时资源的属性

        file_7.Dispose()

        Dim file_6 As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\readme.txt", System.Text.Encoding.UTF8)
        set_address = file_6.ReadLine
        word = file_6.ReadLine
        Dim word_arr() = word.Split("-")
        Try
            set_address_part = word_arr(2)
        Catch ex As Exception
            Environment.Exit(8)
        End Try

        file_6.Dispose()
    End Sub

    ''' <summary>
    ''' [系统]加载设置
    ''' </summary>
    Public Sub add_setting()

        '用户设置
        Dim file_7 As New System.IO.StreamReader(Environment.CurrentDirectory + "\desktop.txt", System.Text.Encoding.UTF8)

        '不加载这个，相关加载写在add_system里，加载核心内容
        file_7.ReadLine()

        If file_7.ReadLine = "0" Then
            talk_man = True
        Else
            talk_man = False
        End If

        If file_7.ReadLine = "0" Then
            use_new_dialogs = True
        Else
            use_new_dialogs = False
        End If

        '加载颜色写在内部
        Dim new_color_r As Integer = Int(file_7.ReadLine)
        Dim new_color_g As Integer = Int(file_7.ReadLine)
        Dim new_color_b As Integer = Int(file_7.ReadLine)

        form_color = Color.FromRgb(new_color_r, new_color_g, new_color_b)

        '自动加载背景函数写在内部

        file_7.Dispose()
    End Sub

    '********************************************************************************************************************************
    ''' <summary>
    ''' [系统]向消息列表添加一个消息
    ''' </summary>
    ''' <param name="title">消息标题</param>
    ''' <param name="word">消息内容</param>
    ''' <remarks></remarks>
    Public Sub message_ex(ByVal title As String, ByVal word As String, ByRef win As Window)

        If use_new_dialogs = False Then
            '旧式的
            '计算当前时间
            Dim time_word As String = ""
            Dim clock_hour As Integer = Hour(Now)
            Dim clock_min As Integer = Minute(Now)
            If clock_hour <= 12 Then
                If clock_min < 10 Then
                    time_word = clock_hour & ":0" & clock_min & " AM"
                Else
                    time_word = clock_hour & ":" & clock_min & " AM"
                End If
            Else
                If clock_min < 10 Then
                    time_word = clock_hour & ":0" & clock_min & " PM"
                Else
                    time_word = clock_hour & ":" & clock_min & " PM"
                End If
            End If

            '添加ui
            Dim a As New ui_depend_message_list
            a.ui_msg_title = title
            a.ui_msg_text = word
            a.ui_msg_date = time_word

            ui_connet_core_form_message_list.Add(a)

            '响铃
            System.Media.SystemSounds.Beep.Play()
        Else
            '新式的
            window_dialogs_show(title, word, 1, False, "确认", "", win)
        End If


    End Sub

    '********************************************************************************************************************************
    ''' <summary>
    ''' [系统]检查一串字符串是不是全是数字
    ''' </summary>
    ''' <param name="word">要检查的字符串</param>
    ''' <returns></returns>
    Public Function check_is_number(ByVal word As String)

        Dim yes As Boolean = True

        If word <> "" Then
            For a = 1 To word.Length
                Select Case Mid(word, a, 1)
                    Case "0"
                    Case "1"
                    Case "2"
                    Case "3"
                    Case "4"
                    Case "5"
                    Case "6"
                    Case "7"
                    Case "8"
                    Case "9"
                    Case Else
                        yes = False
                        Exit For
                End Select

            Next
        Else
            yes = False
        End If

        Return yes
    End Function

    ''' <summary>
    ''' [系统]返回比指定数临近的两个最小的数，返回他们的差值，若没有，返回-1
    ''' </summary>
    ''' <param name="number_group">要检索的数组，用,分割</param>
    ''' <param name="check_number">寻找的数</param>
    ''' <param name="first_nearly">第一小数的差值</param>
    ''' <param name="secound_nearly">第二小数的差值</param>
    Public Sub return_nearly_number(ByVal number_group As String, ByVal check_number As String, ByRef first_nearly As String, ByRef secound_nearly As String)
        Dim number_group_sp() As String = number_group.Split(",")
        Dim n_group As New ArrayList

        Dim max_item As Integer = number_group_sp.Count - 1
        For a = 0 To max_item
            n_group.Add(CType(number_group_sp(a), Integer))
        Next

        If n_group.Count < 2 Then
            '不需要排序
        Else
            For i = 1 To max_item
                For j = i To 1 Step -1
                    If CType(n_group.Item(j), Integer) < CType(n_group.Item(j - 1), Integer) Then
                        Dim temp As Integer = CType(n_group.Item(j), Integer)
                        n_group.Item(j) = CType(n_group.Item(j - 1), Integer)
                        n_group.Item(j - 1) = temp
                    Else
                        Exit For
                    End If
                Next
            Next
        End If

        '=============================获取最小值
        Dim min_list As Integer = -1
        For a = 0 To max_item
            If CType(n_group.Item(a), Integer) >= CType(check_number, Integer) Then
                min_list = a - 1
                Exit For
            End If
            If a = max_item And min_list = -1 Then
                '全部都是在下面的，写最高值
                min_list = max_item
            End If
        Next

        If min_list = -1 Then
            '没有最小值
            first_nearly = "-1"
            secound_nearly = "-1"
        Else
            If min_list = 0 Then
                '只有一个最小值
                first_nearly = Math.Abs(CType(n_group.Item(0), Integer) - CType(check_number, Integer))
                secound_nearly = "-1"
            Else
                '都有
                first_nearly = Math.Abs(CType(n_group.Item(min_list), Integer) - CType(check_number, Integer))
                secound_nearly = Math.Abs(CType(n_group.Item(min_list - 1), Integer) - CType(check_number, Integer))
            End If
        End If


    End Sub


End Module

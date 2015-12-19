Module systems

    '常量
    Public Const app_version As String = "8.0.0.0"
    Public Const app_build As String = "build 8000"
    Public Const app_name As String = "bus_rode Hydrogen"
    Public Const app_update_date As String = "18:09 2015/11/7"
    Public Const app_build_number As Integer = 8000

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

        '不加载这个
        file_7.ReadLine()

        If file_7.ReadLine = "0" Then
            talk_man = True
        Else
            talk_man = False
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
    Public Sub message_ex(ByVal title As String, ByVal word As String)
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

    End Sub

    '********************************************************************************************************************************
    ''' <summary>
    ''' [系统]设置为true可以强制获取一次在线资源
    ''' </summary>
    Public connect_dll_get_resources_urgent As Boolean = False
    ''' <summary>
    ''' [系统]设置为true可使往复调用插件获取实时资源线程始终阻塞，优先级高于urgent
    ''' </summary>
    Public connect_dll_get_resources_always_stop As Boolean = False
    ''' <summary>
    ''' [系统]线程往复调用插件获取实时资源
    ''' </summary>
    Public Sub connect_dll_get_resources()

        '检测插件
        Try
            Dim ass1 As System.Reflection.Assembly = System.Reflection.Assembly.LoadFile(System.Environment.CurrentDirectory & "\bus_rode_mod.dll")
            Dim tp1 As Type = ass1.GetType("bus_rode_dll.main_dll", True)

            If tp1 <> Nothing Then
                Dim instance_check As Object = Activator.CreateInstance(tp1)

                Dim prop_1_check As Reflection.FieldInfo = tp1.GetField("DllDependBusRodeVersion")
                Dim prop_2_check As Reflection.FieldInfo = tp1.GetField("DllRegoin")
                Dim prop_3_check As Reflection.FieldInfo = tp1.GetField("DllUseBusLineName")
                Dim prop_4_check As Reflection.FieldInfo = tp1.GetField("DllGetTick")

                '获取主函数
                Dim out_check As Reflection.MethodInfo = tp1.GetMethod("GetResources", Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

            End If

            GC.Collect()
        Catch ex As Exception
            MsgBox("无法读取插件的相关信息，这可能是由于插件损坏或者插件不完整没有通过CHMOSGroup的认证", 16, "错误")
            Exit Sub
        End Try

        '**********************************************开始执行
        Dim ass As System.Reflection.Assembly = System.Reflection.Assembly.LoadFile(System.Environment.CurrentDirectory & "\bus_rode_mod.dll")
        Dim tp As Type = ass.GetType("bus_rode_dll.main_dll", True)

        Dim instance As Object = Activator.CreateInstance(tp)

        Dim prop_1 As Reflection.FieldInfo = tp.GetField("DllDependBusRodeVersion")
        Dim linshi As Integer = 0
        linshi = CType(prop_1.GetValue(instance), Integer)
        If linshi <> app_build_number Then
            '版本不和
            MsgBox("插件所需要的bus_rode版本和当前使用的bus_rode版本号不同，无法加载，请选择一个合适的插件", 16, "插件加载错误")
            Exit Sub
        End If

        Dim prop_2 As Reflection.FieldInfo = tp.GetField("DllRegoin")
        Dim linshi2 As String = ""
        linshi2 = CType(prop_2.GetValue(instance), String)
        If linshi2 <> set_address Then
            '地区不和
            MsgBox("插件所应用的地区与当前加载资源所表示的地区不同，无法加载，请选择一个合适的插件", 16, "插件加载错误")
            Exit Sub
        End If

        '设置信息
        Dim prop_3 As Reflection.FieldInfo = tp.GetField("DllUseBusLineName")
        prop_3.SetValue(instance, bus)

        '获取以500毫秒为基础的循环次数
        Dim prop_4 As Reflection.FieldInfo = tp.GetField("DllGetTick")
        Dim get_tick As Integer = 0
        get_tick = CType(prop_4.GetValue(instance), Integer)
        Dim round_number As Integer = Int(get_tick / 500) + 1

        '获取主函数
        Dim out As Reflection.MethodInfo = tp.GetMethod("GetResources", Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

        '获取循环
        '先获取一次
        Dim result As Object
        result = out.Invoke(instance, Nothing)
        read_mid_bus_word = result.ToString

        '循环
        Do
            '等待刷新
            For a = 0 To round_number
                '判断紧急情况
                If connect_dll_get_resources_urgent = True Then
                    connect_dll_get_resources_urgent = False
                    Exit For
                Else
                    System.Threading.Thread.Sleep(500)
                End If
            Next

            '刷新
            prop_3.SetValue(instance, bus)

            If connect_dll_get_resources_always_stop = False Then
                result = out.Invoke(instance, Nothing)
                read_mid_bus_word = result.ToString

            End If

        Loop

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


End Module

Module mod_loader

    ''' <summary>
    ''' 线路界面用的资源获取要用的类
    ''' </summary>
    Public connect_dll_screen_line_use_object As Object = Nothing
    ''' <summary>
    ''' 线路界面用的资源获取要用的类中的DllUseBusLineName字段
    ''' </summary>
    Public connect_dll_screen_line_use_object_prop_bus_name As Reflection.FieldInfo = Nothing
    ''' <summary>
    ''' 线路界面用的资源获取要用的类中单位循环时间
    ''' </summary>
    Public connect_dll_screen_line_use_object_round_number As Integer = 5000
    ''' <summary>
    ''' 线路界面用的资源获取要用的类是否可用
    ''' </summary>
    Public connect_dll_screen_line_use_object_can_use As Boolean = False
    '======================================================================
    ''' <summary>
    ''' 站台界面用的资源获取要用的类
    ''' </summary>
    Public connect_dll_screen_stop_use_object As Object = Nothing
    ''' <summary>
    ''' 站台界面用的资源获取要用的类中的DllUseBusLineName字段
    ''' </summary>
    Public connect_dll_screen_stop_use_object_prop_bus_name As Reflection.FieldInfo = Nothing
    ''' <summary>
    ''' 站台界面用的资源获取要用的类中单位循环时间
    ''' </summary>
    Public connect_dll_screen_stop_use_object_round_number As Integer = 5000
    ''' <summary>
    ''' 站台界面用的资源获取要用的类是否可用
    ''' </summary>
    Public connect_dll_screen_stop_use_object_can_use As Boolean = False

    '********************************************************************************************************************************
    ''' <summary>
    ''' [系统][MOD]设置为true可以强制获取一次在线资源
    ''' </summary>
    Public connect_dll_get_resources_urgent As Boolean = False
    ''' <summary>
    ''' [系统][MOD]设置为true可使往复调用插件获取实时资源线程始终阻塞，优先级高于urgent
    ''' </summary>
    Public connect_dll_get_resources_always_stop As Boolean = False

    ''' <summary>
    ''' [系统][MOD]线程往复调用插件获取实时资源
    ''' </summary>
    Public Sub connect_dll_screen_line_get_resources()

        '==========执行判断===========
        If connect_dll_screen_line_use_object_can_use = False Then Exit Sub

        '**********************************************开始执行
        '获取反射
        Dim ass As System.Reflection.Assembly = System.Reflection.Assembly.LoadFile(System.Environment.CurrentDirectory & "\bus_rode_mod.dll")
        Dim tp As Type = ass.GetType("bus_rode_dll.main_dll", True)

        '获取主函数
        Dim out As Reflection.MethodInfo = tp.GetMethod("GetResources", Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

        '获取循环
        Dim result As Object

        '循环
        Do
            '等待刷新
            For a = 0 To connect_dll_screen_line_use_object_round_number
                '判断紧急情况
                If connect_dll_get_resources_urgent = True Then
                    connect_dll_get_resources_urgent = False
                    Exit For
                Else
                    System.Threading.Thread.Sleep(500)
                End If
            Next

            '刷新
            '仅在线路界面中刷新，减少消耗
            If screens = 1 Then

                connect_dll_screen_line_use_object_prop_bus_name.SetValue(connect_dll_screen_line_use_object, bus)

                If connect_dll_get_resources_always_stop = False Then
                    '尝试获取
                    Try
                        result = out.Invoke(connect_dll_screen_line_use_object, Nothing)
                        read_mid_bus_word = result.ToString
                        read_mid_bus_word_last_update = DateTime.Now.TimeOfDay.ToString
                    Catch ex As Exception
                        '获取失败不停止获取，但返回空
                        read_mid_bus_word = ""
                    End Try

                End If
            End If

        Loop

    End Sub

    ''' <summary>
    ''' [系统][MOD]线程往复调用插件获取真实站台
    ''' </summary>
    Public Sub connect_dll_screen_stop_get_resources()

        '==========执行判断===========
        If connect_dll_screen_stop_use_object_can_use = False Then Exit Sub

        '**********************************************开始执行
        '获取反射
        Dim ass As System.Reflection.Assembly = System.Reflection.Assembly.LoadFile(System.Environment.CurrentDirectory & "\bus_rode_mod.dll")
        Dim tp As Type = ass.GetType("bus_rode_dll.main_dll", True)

        '获取主函数
        Dim out As Reflection.MethodInfo = tp.GetMethod("GetResources", Reflection.BindingFlags.Public Or Reflection.BindingFlags.Instance)

        '获取循环
        Dim result As Object
        Dim line_group As ArrayList = New ArrayList

        '循环
        Do
            '等待刷新
            For a = 0 To connect_dll_screen_stop_use_object_round_number
                '判断紧急情况
                If connect_dll_get_resources_urgent = True Then
                    connect_dll_get_resources_urgent = False
                    Exit For
                Else
                    System.Threading.Thread.Sleep(500)
                End If
            Next

            '刷新
            '仅在站台界面中刷新，减少消耗
            If screens = 2 And connect_dll_get_resources_always_stop = False Then
                '先读取所有车辆
                For a = 0 To realistic_stop_list - 1
                    line_group.Add(realistic_stop(a, 0))
                Next

                '循环获取
                For a = 0 To realistic_stop_list - 1
                    connect_dll_screen_stop_use_object_prop_bus_name.SetValue(connect_dll_screen_stop_use_object, line_group.Item(a).ToString)

                    '尝试获取
                    Try
                        result = out.Invoke(connect_dll_screen_stop_use_object, Nothing)
                        line_group.Item(a) = result.ToString
                    Catch ex As Exception
                        '获取失败不停止获取，但返回空
                        line_group.Item(a) = ""
                    End Try
                Next

                Dim up_line_word As String = ""
                Dim down_line_word As String = ""
                For a = 0 To realistic_stop_list - 1
                    '没有内容
                    If line_group.Item(a).ToString = "" Then
                        realistic_stop(a, 1) = "-1"
                        realistic_stop(a, 2) = "-1"
                        realistic_stop(a, 5) = "-1"
                        realistic_stop(a, 6) = "-1"
                        '开始新循环
                        GoTo begin_new_circle
                    End If

                    '拆分
                    Dim word_sp() As String = line_group.Item(a).ToString.Split("@")
                    For b = 0 To word_sp.Count - 1
                        Dim bus_sp() As String = word_sp(b).Split("#")
                        If bus_sp(1) = "0" Then
                            '上行
                            If up_line_word = "" Then
                                up_line_word = bus_sp(2)
                            Else
                                up_line_word = up_line_word & "," & bus_sp(2)
                            End If
                        Else
                            '下行
                            If down_line_word = "" Then
                                down_line_word = bus_sp(2)
                            Else
                                down_line_word = down_line_word & "," & bus_sp(2)
                            End If
                        End If
                    Next

                    '尝试返回
                    If up_line_word = "" Or realistic_stop(a, 4) = "-1" Then
                        realistic_stop(a, 1) = "-1"
                        realistic_stop(a, 2) = "-1"
                    Else
                        return_nearly_number(up_line_word, realistic_stop(a, 4), realistic_stop(a, 1), realistic_stop(a, 2))
                    End If
                    If down_line_word = "" Or realistic_stop(a, 8) = "-1" Then
                        realistic_stop(a, 1) = "-1"
                        realistic_stop(a, 2) = "-1"
                    Else
                        return_nearly_number(down_line_word, realistic_stop(a, 8), realistic_stop(a, 5), realistic_stop(a, 6))
                    End If

begin_new_circle:
                    '还原
                    up_line_word = ""
                    down_line_word = ""

                Next



            End If

            line_group.Clear()

        Loop

    End Sub

    ''' <summary>
    ''' [系统][MOD]mod获取初始化
    ''' </summary>
    Public Sub connect_dll_init()

        '检测插件基本内容
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
            MsgBox(read_resources_describe_into_memory("lang_code_MainWindow_contorl_mod_error_1") & read_resources_describe_into_memory("lang_code_MainWindow_contorl_mod_error_2") & 
			read_resources_describe_into_memory("lang_code_MainWindow_contorl_mod_error_3"), 16, read_resources_describe_into_memory("lang_code_MainWindow_error_title"))
            Exit Sub
        End Try

        '**********************************************高级处理
        '获取反射
        Dim ass As System.Reflection.Assembly = System.Reflection.Assembly.LoadFile(System.Environment.CurrentDirectory & "\bus_rode_mod.dll")
        Dim tp As Type = ass.GetType("bus_rode_dll.main_dll", True)

        Dim instance As Object = Activator.CreateInstance(tp)

        '=====================================更详细的判断=========================================
        Dim prop_1 As Reflection.FieldInfo = tp.GetField("DllDependBusRodeVersion")
        Dim linshi As Integer = 0
        linshi = CType(prop_1.GetValue(instance), Integer)
        If linshi <> app_build_number Then
            '版本不和
            MsgBox(read_resources_describe_into_memory("lang_code_mod_loader_check_version_error"), 16, read_resources_describe_into_memory("lang_code_MainWindow_error_title"))
            Exit Sub
        End If

        Dim prop_2 As Reflection.FieldInfo = tp.GetField("DllRegoin")
        Dim linshi2 As String = ""
        linshi2 = CType(prop_2.GetValue(instance), String)
        If linshi2 <> set_address Then
            '地区不和
            MsgBox(read_resources_describe_into_memory("lang_code_mod_loader_check_regoin_error"), 16, read_resources_describe_into_memory("lang_code_MainWindow_error_title"))
            Exit Sub
        End If

        '设置信息，不需要删了
        'Dim prop_3 As Reflection.FieldInfo = tp.GetField("DllUseBusLineName")
        'prop_3.SetValue(instance, bus)

        '获取以500毫秒为基础的循环次数
        Dim prop_4 As Reflection.FieldInfo = tp.GetField("DllGetTick")
        Dim get_tick As Integer = 0
        get_tick = CType(prop_4.GetValue(instance), Integer)
        Dim round_number As Integer = Int(get_tick / 500) + 1

        '======================================可用，写入相关内容=================================================
        '创建类，写入可用
        connect_dll_screen_line_use_object = Activator.CreateInstance(tp)
        connect_dll_screen_stop_use_object = Activator.CreateInstance(tp)
        '读取字段
        connect_dll_screen_line_use_object_prop_bus_name = tp.GetField("DllUseBusLineName")
        connect_dll_screen_stop_use_object_prop_bus_name = tp.GetField("DllUseBusLineName")
        '写入可用
        connect_dll_screen_line_use_object_can_use = True
        connect_dll_screen_stop_use_object_can_use = True
        '写入时间
        connect_dll_screen_line_use_object_round_number = round_number
        connect_dll_screen_stop_use_object_round_number = round_number

    End Sub


End Module

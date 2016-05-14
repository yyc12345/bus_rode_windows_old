Module screen_setting

    ''' <summary>
    ''' [内核]是否跟踪
    ''' </summary>
    ''' <remarks></remarks>
    Public get_bus_addr As Boolean = False

    ''' <summary>
    ''' [内核]是否讲述人
    ''' </summary>
    ''' <remarks></remarks>
    Public talk_man As Boolean = False

    ''' <summary>
    ''' [内核]是否新对话框
    ''' </summary>
    Public use_new_dialogs As Boolean = False

    ''' <summary>
    ''' 指示设置界面组1是否展开
    ''' </summary>
    Public setting_group_1 As Boolean = False
    ''' <summary>
    ''' 指示设置界面组1是否展开
    ''' </summary>
    Public setting_group_2 As Boolean = False
    ''' <summary>
    ''' 指示设置界面组1是否展开
    ''' </summary>
    Public setting_group_3 As Boolean = False

    ''' <summary>
    ''' 界面的语言
    ''' </summary>
    Public interface_language As String = "en-US"

    ''' <summary>
    ''' 是否自动翻译
    ''' </summary>
    Public auto_translate As Boolean = False

    ''' <summary>
    ''' 窗口的颜色,不包含透明度
    ''' </summary>
    ''' <remarks></remarks>
    Public form_color As Color = Color.FromRgb(0, 0, 255)

    ''' <summary>
    ''' 资源所属地区-仅城市
    ''' </summary>
    Public set_address_part As String = ""
    ''' <summary>
    ''' 资源所属地区
    ''' </summary>
    Public set_address As String = ""


    ''' <summary>
    ''' [内核]保存用户设置
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub save_user_contorl()

        Dim file As New System.IO.StreamWriter(Environment.CurrentDirectory + "\desktop.txt", False, System.Text.Encoding.UTF8)

        If get_bus_addr = True Then
            file.WriteLine("0")
        Else
            file.WriteLine("1")
        End If

        If talk_man = True Then
            file.WriteLine("0")
        Else
            file.WriteLine("1")
        End If

        If use_new_dialogs = True Then
            file.WriteLine("0")
        Else
            file.WriteLine("1")
        End If

        If auto_translate = True Then
            file.WriteLine("0")
        Else
            file.WriteLine("1")
        End If

        file.WriteLine(form_color.R)
        file.WriteLine(form_color.G)
        file.WriteLine(form_color.B)

        file.WriteLine(interface_language)

        file.Dispose()

    End Sub


    ''' <summary>
    ''' [内核]讲述人函数执行线程
    ''' </summary>
    Public talk_man_main_td As New System.Threading.Thread(AddressOf talk_man_main)

    ''' <summary>
    ''' [内核]允许讲述开始设置的标识符
    ''' </summary>
    Public run_talk_flag As Boolean = False

    ''' <summary>
    ''' [内核]讲述人主函数
    ''' </summary>
    Public Sub talk_man_main()

        '无限循环
		'TODO:讲述人的问题
        Do
            Do
                If run_talk_flag = True Then
                    Exit Do
                Else
                    System.Threading.Thread.Sleep(1000)
                End If
            Loop

            '开始讲述
            '先检测消息列表是否存在，优先
            If message_show = True Then
                If ui_connet_core_form_message_list.Count = 0 Then
                    talk_man_file("消息中心没有消息")
                Else
                    For a = 0 To ui_connet_core_form_message_list.Count - 1
                        talk_man_file("第" & a + 1 & "条消息,")
                        talk_man_file(ui_connet_core_form_message_list.Item(a).ui_msg_title & ",")
                        talk_man_file(ui_connet_core_form_message_list.Item(a).ui_msg_text & ",")
                        talk_man_file("该消息于" & ui_connet_core_form_message_list.Item(a).ui_msg_date & "发出,")
                    Next
                End If
            Else
                Select Case screens
                    Case 0
                        talk_man_file("欢迎使用bus rode，您可以在这里查看线路，车站，进行相关设置，查看有关这个程序的相关信息的操作")
                    Case 1
                        talk_man_file(bus + ",")

                        If bus_or_subway = 0 Then
                            '公交
                            talk_man_file("类别,公交，")
                        Else
                            '地铁
                            talk_man_file("类别,地铁，")
                        End If
                        talk_man_file(bus_msg(0) + "," + bus_msg(1) + "," + bus_msg(2) + "," + bus_msg(3) + "," + bus_msg(4) + "," + bus_run + "，")

                        talk_man_file("当前车次等待的难度，" & bus_wait_hard)

                        talk_man_file("当前线路的上行站台,")
                        For a = 0 To 200
                            If bus_stop_up(a) = "" Then
                                Exit For
                            End If

                            talk_man_file(bus_stop_up(a) & ",")
                        Next
                        talk_man_file("当前线路的下行站台,")
                        For a = 0 To 200
                            If bus_stop_down(a) = "" Then
                                Exit For
                            End If

                            talk_man_file(bus_stop_down(a) & ",")
                        Next

                        If bus_or_subway = 1 Then
                            If up_or_down_line = True Then
                                talk_man_file("当前线路选定的的站台," & bus_stop_up(select_stop_point) & "是地铁站点，所以，这个站台拥有以下出口")
                            Else
                                talk_man_file("当前线路选定的的站台," & bus_stop_down(select_stop_point) & "是地铁站点，所以，这个站台拥有以下出口")
                            End If

                            For i = 0 To subway_have_exit_list - 1
                                '排除错误
                                If subway_have_exit_list = 0 Then
                                    Exit For
                                End If
                                talk_man_file(subway_have_exit(i, 0) + "," + subway_have_exit(i, 1))
                            Next

                        End If

                    Case 2

                        talk_man_file(Replace(cross_stop, vbCrLf, ","))

                        talk_man_file("当前站点附近站点有，")
                        For a = 0 To bus_stop_line_list - 1
                            talk_man_file(bus_stop_line(a, 1) & "从" & bus_stop_line(a, 0) & "开到此站")
                        Next
                    Case 3
                        talk_man_file("共有常规设置，实时设置，资源设置，个性化设置，这4种设置,")
                        '===================================================
                        talk_man_file("常规设置下共有3个设置项目，“)
                        talk_man_file("设置项1，讲述人，帮助视力不佳的人朗读屏幕上的内容（在任意界面按下 F1 键以朗读），")
                        If talk_man = True Then
                            talk_man_file("当前讲述人选项是开启的")
                        Else
                            talk_man_file("当前讲述人选项是关闭的")
                        End If
                        talk_man_file("设置项2，新对话框，打开以使用新式对话框而不是旧式对话框，")
                        If use_new_dialogs = True Then
                            talk_man_file("当前新对话框选项是开启的")
                        Else
                            talk_man_file("当前新对话框选项是关闭的")
                        End If
                        talk_man_file("设置项3，重新启动应用程序，重启解决一些偶然性问题，")
                        '===================================================
                        talk_man_file("实时设置下共有4个设置项目，")
                        talk_man_file("设置项1，车辆跟踪，对处在运营状态的车次到达站次显示在线路图上，")
                        If get_bus_addr = True Then
                            talk_man_file("当前车辆跟踪选项是开启的")
                        Else
                            talk_man_file("当前车辆跟踪选项是关闭的")
                        End If
                        talk_man_file("设置项2，车辆跟踪插件，车辆跟踪插件可以方便地安全地获取到你所在城市的实时公交信息并显示在bus_rode上，")
                        talk_man_file("设置项3，资源信息，删除当前加载的插件，")
                        talk_man_file("设置项4，插件信息，显示当前车辆跟踪插件的信息，")
                        '===================================================
                        talk_man_file("资源设置下共有3个设置项目，")
                        talk_man_file("设置项1，替换资源，替换加载的资源，便于切换到其他城市进行指引，")
                        talk_man_file("设置项2，删除加载的资源，删除当前加载的资源，")
                        talk_man_file("设置项3，资源信息，显示当前资源的信息，")
                        '===================================================
                        talk_man_file("个性化设置下共有2个设置项目，")
                        talk_man_file("设置项1，替换背景，选用一张新的背景图片作为背景，")
                        talk_man_file("设置项2，更改配色，输入一组新的颜色值来替换现有颜色，")

                    Case 4
                        Dim describe As String = "Programmer : Wiliam Tad，" &
"Insider : Nothing，" &
"Previous Insider : Tianyue Sun，" &
"Return : Xianlei Bian，Yi Gao，Zechen Li，Junzhe Jiang，" &
"Version : " & app_version & " " & app_build & "," &
"Last update date : " & app_update_date & "," &
"User Environment," &
".NET Framework Version : " & Environment.Version.ToString & "," &
"Sign up user : " & Environment.UserName.ToString & "," &
"OS Version : " & Environment.OSVersion.ToString & "," &
"CHMOSGroup Copyright 2012-2016"

                        talk_man_file(describe)

                End Select
            End If

            '设置回原样
            run_talk_flag = False
        Loop
    End Sub



    ''' <summary>
    ''' [内核]讲述人主执行函数_ex
    ''' </summary>
    ''' <param name="word"></param>
    Public Sub talk_man_file(ByVal word As String)

        CreateObject("SAPI.SpVoice").Speak(word)

    End Sub

End Module

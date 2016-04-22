Module screen_line

    ''' <summary>
    ''' [内核][screen_line]当前选定的公交车
    ''' </summary>
    ''' <remarks></remarks>
    Public bus As String = "0"
    ''' <summary>
    ''' [内核][screen_line]受支持的公交车列表
    ''' </summary>
    ''' <remarks></remarks>
    Public have_bus(1000) As String

    ''' <summary>
    ''' [内核][screen_line]公交车站点存储-上行
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_stop_up(200) As String
    ''' <summary>
    ''' [内核][screen_line]公交车站点存储-下行
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_stop_down(200) As String
    ''' <summary>
    ''' [内核][screen_line]站台区间车辆信息(站点内车辆信息，0填写上行的数字，1填写下行的数字,对应站点序号)
    ''' </summary>
    ''' <remarks></remarks>
    Public mid_bus(2, 200) As obj_mid_bus
    ''' <summary>
    ''' [内核][screen_line]公交车属性
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_msg(5) As String
    ''' <summary>
    ''' [内核][screen_line]当前选中的线路站台
    ''' </summary>
    ''' <remarks></remarks>
    Public select_stop_point As Integer = 0
    ''' <summary>
    ''' [内核][screen_line]当前选中的是上行还是下行线路，True表示上行，False表示下行
    ''' </summary>
    Public up_or_down_line As Boolean = True

    ''' <summary>
    ''' [内核][screen_line]公交车首班车时间，小时
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_date_start_hours As Integer = 0
    ''' <summary>
    ''' [内核][screen_line]公交车首班车时间，分钟
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_date_start_min As Integer = 0
    ''' <summary>
    ''' [内核][screen_line]公交车末班时间，小时
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_date_end_hours As Integer = 0
    ''' <summary>
    ''' [内核][screen_line]公交车末班时间，分钟
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_date_end_min As Integer = 0
    ''' <summary>
    ''' [内核][screen_line]系统时间，小时
    ''' </summary>
    ''' <remarks></remarks>
    Public system_date_hours As Integer = 0
    ''' <summary>
    ''' [内核][screen_line]系统时间，分钟
    ''' </summary>
    ''' <remarks></remarks>
    Public system_date_min As Integer = 0
    ''' <summary>
    ''' [内核][screen_line]运营状态
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_run As String = "未知"
    ''' <summary>
    ''' [内核][screen_line]当前公交等待难易度
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_wait_hard As String = "适中"

    ''' <summary>
    ''' [内核][screen_line]指示当前选中的车次是公交还是地铁0=公交 1=地铁
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_or_subway As Integer = 0

    ''' <summary>
    ''' [内核][screen_line]read_mid_bus所需要检测并分析的String表达式
    ''' </summary>
    Public read_mid_bus_word As String = ""
    ''' <summary>
    ''' [内核][screen_line]read_mid_bus_word最后更新时间
    ''' </summary>
    Public read_mid_bus_word_last_update As String = "无"

    ''' <summary>
    ''' [内核][screen_line]界面上行线路走向的描述
    ''' </summary>
    Public bus_up_line_describe As String = ""
    ''' <summary>
    ''' [内核][screen_line]界面下行线路走向的描述
    ''' </summary>
    Public bus_down_line_describe As String = ""

    '***********************************************************结构************************************************************************************
    ''' <summary>
    ''' [内核][screen_line]mid_bus所用的结构
    ''' </summary>
    Public Structure obj_mid_bus
        ''' <summary>
        ''' 车辆数
        ''' </summary>
        Public number As Short
        ''' <summary>
        ''' 附加信息1
        ''' </summary>
        Public message_1 As String
        ''' <summary>
        ''' 附加信息2
        ''' </summary>
        Public message_2 As String
        ''' <summary>
        ''' 附加信息3
        ''' </summary>
        Public message_3 As String

    End Structure


    '***********************************************************函数************************************************************************************

    ''' <summary>
    ''' [内核][screen_line]读取车辆站台
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub read_bus()

        Dim file As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\bus.txt", System.Text.Encoding.UTF8)

        Dim word As String = ""
        Do
            word = file.ReadLine
            If word = bus Then

                '读取车辆信息
                bus_msg(0) = file.ReadLine
                bus_msg(1) = file.ReadLine
                bus_msg(2) = file.ReadLine
                bus_msg(3) = file.ReadLine
                bus_msg(4) = file.ReadLine

                '读取车辆属性
                bus_or_subway = Int(file.ReadLine)
                '读取等待难易程度
                Select Case file.ReadLine
                    Case "0"
                        bus_wait_hard = "极难等"
                    Case "1"
                        bus_wait_hard = "难等"
                    Case "2"
                        bus_wait_hard = "适中"
                    Case "3"
                        bus_wait_hard = "易等"
                    Case "4"
                        bus_wait_hard = "极易等"
                    Case Else
                        bus_wait_hard = "适中"
                End Select

                '读取运营时间
                bus_date_start_hours = Int(file.ReadLine)
                bus_date_start_min = Int(file.ReadLine)
                bus_date_end_hours = Int(file.ReadLine)
                bus_date_end_min = Int(file.ReadLine)

                '站点
                Dim flag As Integer = 0

                Do
                    word = file.ReadLine
                    If word = "ENDLINE" Then
                        Exit Do
                    End If
                    bus_stop_up(flag) = word

                    flag = flag + 1
                Loop

                flag = 0

                Do
                    word = file.ReadLine
                    If word = "END" Then
                        Exit Do
                    End If
                    bus_stop_down(flag) = word

                    flag = flag + 1
                Loop


                Exit Do
            End If
        Loop
        file.Dispose()

        '获取起点-重点描述字段
        bus_up_line_describe = bus_stop_up(0)
        For a = 0 To 200
            If bus_stop_up(a) = "" Then
                bus_up_line_describe = bus_up_line_describe & "-" & bus_stop_up(a - 1)
                Exit For
            End If
        Next

        If bus_stop_down(0) = "" Then
            '只有单行线路
            bus_down_line_describe = ""
        Else
            bus_down_line_describe = bus_stop_down(0)
            For a = 0 To 200
                If bus_stop_down(a) = "" Then
                    bus_down_line_describe = bus_down_line_describe & "-" & bus_stop_down(a - 1)
                    Exit For
                End If
            Next
        End If



        '设置地铁相关
        If bus_or_subway = 0 Then
            ui_connet_core_form_line_subway_list.Clear()
        Else
            re_subway_stop()
        End If

    End Sub

    ''' <summary>
    ''' [内核][screen_line]清除车辆站台和车辆信息记录
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub clear()
        For test2 = 0 To 200
            bus_stop_up(test2) = ""
            bus_stop_down(test2) = ""
        Next

        For test4 = 0 To 5
            bus_msg(test4) = ""
        Next

        bus_up_line_describe = ""
        bus_down_line_describe = ""

    End Sub

    ''' <summary>
    ''' [内核][screen_line]读取车辆站台区间车辆数
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub read_mid_bus()
        For b = 0 To 2
            For c = 0 To 200
                mid_bus(b, c).number = 0
                mid_bus(b, c).message_1 = ""
                mid_bus(b, c).message_2 = ""
                mid_bus(b, c).message_3 = ""
            Next
        Next

        '分析字符串
        If read_mid_bus_word <> "" Then
            '防止出错
            Try

                Dim word_arr() As String = read_mid_bus_word.Split("@")
                If word_arr.Count <> 0 Then
                    For a = 0 To word_arr.Count - 1
                        '0=路数 1=上下行 2=站点名，或passstation 3=消息 4=消息 5=消息
                        Dim word_arr_arr() As String = word_arr(a).Split("#")

                        '是当前选中车的信息
                        If word_arr_arr(0) = bus Then

                            '判断上下行，0=上行，1=下行
                            If word_arr_arr(1) = "0" Then

                                '判断pass还是站名
                                If check_is_number(word_arr_arr(2)) = True Then
                                    'pass,直接写入
                                    If Int(word_arr_arr(2)) <= 200 Then
                                        mid_bus(0, Int(word_arr_arr(2))).number = mid_bus(0, Int(word_arr_arr(2))).number + 1
                                        mid_bus(0, Int(word_arr_arr(2))).message_1 = mid_bus(0, Int(word_arr_arr(2))).message_1 & word_arr_arr(3)
                                        mid_bus(0, Int(word_arr_arr(2))).message_2 = mid_bus(0, Int(word_arr_arr(2))).message_2 & word_arr_arr(4)
                                        mid_bus(0, Int(word_arr_arr(2))).message_3 = mid_bus(0, Int(word_arr_arr(2))).message_3 & word_arr_arr(5)
                                    End If

                                Else
                                    '站点
                                    For d = 0 To 200
                                        If word_arr_arr(2) = bus_stop_up(d) Then
                                            mid_bus(0, d).number = mid_bus(0, d).number + 1
                                            mid_bus(0, d).message_1 = mid_bus(0, d).message_1 & word_arr_arr(3)
                                            mid_bus(0, d).message_2 = mid_bus(0, d).message_2 & word_arr_arr(4)
                                            mid_bus(0, d).message_3 = mid_bus(0, d).message_3 & word_arr_arr(5)
                                            Exit For
                                        End If
                                    Next

                                End If

                            Else

                                '判断pass还是站名
                                If check_is_number(word_arr_arr(2)) = True Then
                                    'pass,直接写入
                                    If Int(word_arr_arr(2)) <= 200 Then
                                        mid_bus(1, Int(word_arr_arr(2))).number = mid_bus(1, Int(word_arr_arr(2))).number + 1
                                        mid_bus(1, Int(word_arr_arr(2))).message_1 = mid_bus(1, Int(word_arr_arr(2))).message_1 & word_arr_arr(3)
                                        mid_bus(1, Int(word_arr_arr(2))).message_2 = mid_bus(1, Int(word_arr_arr(2))).message_2 & word_arr_arr(4)
                                        mid_bus(1, Int(word_arr_arr(2))).message_3 = mid_bus(1, Int(word_arr_arr(2))).message_3 & word_arr_arr(5)
                                    End If

                                Else
                                    '站点
                                    For d = 0 To 200
                                        If word_arr_arr(2) = bus_stop_up(d) Then
                                            mid_bus(1, d).number = mid_bus(1, d).number + 1
                                            mid_bus(1, d).message_1 = mid_bus(1, d).message_1 & word_arr_arr(3)
                                            mid_bus(1, d).message_2 = mid_bus(1, d).message_2 & word_arr_arr(4)
                                            mid_bus(1, d).message_3 = mid_bus(1, d).message_3 & word_arr_arr(5)
                                            Exit For
                                        End If
                                    Next

                                End If

                            End If
                        End If


                    Next
                End If

            Catch ex As Exception

                For b = 0 To 2
                    For c = 0 To 200
                        mid_bus(b, c).number = 0
                        mid_bus(b, c).message_1 = ""
                        mid_bus(b, c).message_2 = ""
                        mid_bus(b, c).message_3 = ""
                    Next
                Next

            End Try

        End If

        '全部写入ui
        Dim ui_connect As New ui_depend_line_stop_list
        ui_connet_core_form_line_have_sotp_list_up.Clear()

        For d = 0 To 200
            If bus_stop_up(d) = "" Then
                Exit For
            End If

            '连接ui
            ui_connect.ui_stop_have_bus_number = mid_bus(0, d).number
            ui_connect.ui_stop_have_bus_message = mid_bus(0, d).message_1 & vbCrLf & mid_bus(0, d).message_2 & vbCrLf & mid_bus(0, d).message_3
            Select Case mid_bus(0, d).number
                Case 0
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 255, 255, 255))
                Case 1
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 163, 163, 255))
                Case 2
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 92, 92, 255))
                Case 3
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 41, 41, 255))
                Case 4
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 10, 10, 255))
                Case Else
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 0, 0, 255))
            End Select
            ui_connect.ui_stop_name = bus_stop_up(d)
            ui_connet_core_form_line_have_sotp_list_up.Add(ui_connect)
            ui_connect = New ui_depend_line_stop_list

        Next

        ui_connect = New ui_depend_line_stop_list
        ui_connet_core_form_line_have_sotp_list_down.Clear()

        For d = 0 To 200
            If bus_stop_down(d) = "" Then
                Exit For
            End If

            '连接ui
            ui_connect.ui_stop_have_bus_number = mid_bus(1, d).number
            ui_connect.ui_stop_have_bus_message = mid_bus(1, d).message_1 & vbCrLf & mid_bus(1, d).message_2 & vbCrLf & mid_bus(1, d).message_3
            Select Case mid_bus(1, d).number
                Case 0
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 255, 255, 255))
                Case 1
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 163, 163, 255))
                Case 2
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 92, 92, 255))
                Case 3
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 41, 41, 255))
                Case 4
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 10, 10, 255))
                Case Else
                    ui_connect.ui_stop_have_bus_number_color = New SolidColorBrush(Color.FromArgb(255, 0, 0, 255))
            End Select
            ui_connect.ui_stop_name = bus_stop_down(d)
            ui_connet_core_form_line_have_sotp_list_down.Add(ui_connect)
            ui_connect = New ui_depend_line_stop_list

        Next


    End Sub

    ''' <summary>
    ''' [内核][screen_line]检测公交车运营时间
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub look_bus_date()
        system_date_hours = Hour(Now)
        system_date_min = Minute(Now)

        '检测这是夜班车还是白班车(即是否跨过午夜12点)
        If (bus_date_end_hours < bus_date_start_hours) Or (bus_date_end_hours = bus_date_start_hours And bus_date_end_min < bus_date_start_min) Then
            '夜班
            If bus_date_end_hours > system_date_hours Or (bus_date_end_hours = system_date_hours And bus_date_end_min >= system_date_min) Or
                bus_date_start_hours < system_date_hours Or (bus_date_start_hours = system_date_hours And bus_date_start_min <= system_date_min) Then
                bus_run = "车次在正常情况处于运营状态，可以乘坐。"
            Else
                bus_run = "有坐不到该车次的风险，该车次此时此刻已经停止发车！"
            End If

        Else
            '白班
            '检查是否到达首班车
            If (bus_date_end_hours > system_date_hours Or (bus_date_end_hours = system_date_hours And bus_date_end_min >= system_date_min)) And
                (bus_date_start_hours < system_date_hours Or (bus_date_start_hours = system_date_hours And bus_date_start_min <= system_date_min)) Then
                bus_run = "车次在正常情况处于运营状态，可以乘坐。"
            Else
                bus_run = "有坐不到该车次的风险，该车次此时此刻已经停止发车！"
            End If
        End If



    End Sub

    '**********************************************************服务函数********************************************************
    ''' <summary>
    ''' [内核][screen_line]检查当前加载的线路含有车站中是否有指定车站
    ''' </summary>
    ''' <param name="key">要搜索的站台名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function check_bus_stop_have(ByVal key As String)
        Dim yes As Boolean = False

        For test1 = 0 To 200
            If bus_stop_up(test1) = key Then
                yes = True
                Exit For
            End If
            If bus_stop_down(test1) = key Then
                yes = True
                Exit For
            End If
        Next

        Return yes
    End Function

    ''' <summary>
    ''' [内核][screen_line]返回当前加载的线路含有车站中指定车站的序数，从0起始，没有返回-1
    ''' </summary>
    ''' <param name="key">要搜索的站台名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function return_bus_stop_list(ByVal key As String)
        Dim yes As Integer = -1

        If up_or_down_line = True Then
            For test1 = 0 To 200
                If bus_stop_up(test1) = key Then
                    yes = test1
                    Exit For
                End If
            Next
        Else
            For test1 = 0 To 200
                If bus_stop_down(test1) = key Then
                    yes = test1
                    Exit For
                End If
            Next
        End If

        Return yes
    End Function

    ''' <summary>
    ''' [内核][screen_line]返回线路中指定线路的序数，从0起始，没有返回-1
    ''' </summary>
    ''' <param name="key">要搜索的线路名称</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function return_bus_list(ByVal key As String)
        Dim yes As Integer = -1

        For test1 = 0 To 1000
            If have_bus(test1) = key Then
                yes = test1
                Exit For
            End If
        Next

        Return yes
    End Function

    '**********************************************************地铁函数********************************************************
    '
    ''' <summary>
    ''' [内核][screen_line]当前选中的地铁站台的出口信息(出口编号,数据指针)数据指针0=出口名 1=出口通往哪里
    ''' </summary>
    ''' <remarks></remarks>
    Public subway_have_exit(10, 2) As String
    ''' <summary>
    ''' [内核][screen_line]地铁站台的出口数量
    ''' </summary>
    ''' <remarks></remarks>
    Public subway_have_exit_list As Integer = 0

    ''' <summary>
    ''' [内核][screen_line]刷新地铁站出口
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub re_subway_stop()
        Dim file As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\subway.txt", System.Text.Encoding.UTF8)
        Dim word As String = ""

        subway_have_exit_list = 0

        Do
            word = file.ReadLine

            If word = "" Then
                Exit Do
            End If

            If up_or_down_line = True Then
                If word = bus_stop_up(select_stop_point) Then
                    Do
                        word = file.ReadLine
                        If word = "END" Then
                            Exit Do
                        End If
                        subway_have_exit(subway_have_exit_list, 0) = word
                        subway_have_exit(subway_have_exit_list, 1) = file.ReadLine

                        subway_have_exit_list = subway_have_exit_list + 1
                    Loop

                    Exit Do
                Else
                    '越过
                    Do
                        If file.ReadLine = "END" Then
                            Exit Do
                        End If
                    Loop
                End If
            Else
                If word = bus_stop_down(select_stop_point) Then
                    Do
                        word = file.ReadLine
                        If word = "END" Then
                            Exit Do
                        End If
                        subway_have_exit(subway_have_exit_list, 0) = word
                        subway_have_exit(subway_have_exit_list, 1) = file.ReadLine

                        subway_have_exit_list = subway_have_exit_list + 1
                    Loop

                    Exit Do
                Else
                    '越过
                    Do
                        If file.ReadLine = "END" Then
                            Exit Do
                        End If
                    Loop
                End If
            End If

        Loop

        file.Dispose()

        '更新ui
        Dim d As New ui_depend_line_subway_list
        ui_connet_core_form_line_subway_list.Clear()
        For f = 0 To subway_have_exit_list - 1
            d.ui_exit_name = subway_have_exit(f, 0)
            d.ui_go_to = subway_have_exit(f, 1)

            ui_connet_core_form_line_subway_list.Add(d)
            d = New ui_depend_line_subway_list
        Next

    End Sub

End Module

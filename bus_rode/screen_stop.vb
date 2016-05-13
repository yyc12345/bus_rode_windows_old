Module screen_stop

    ''' <summary>
    ''' [内核][screen_stop]当前站台是否是总站
    ''' </summary>
    ''' <remarks></remarks>
    Public how_station As Boolean = False

    ''' <summary>
    ''' [内核][screen_stop]站台列表
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_stop_stop(10000) As String
    ''' <summary>
    ''' [内核][screen_stop]当前选定站台位于列表中的序号
    ''' </summary>
    ''' <remarks></remarks>
    Public now_stop As Integer = 0
    ''' <summary>
    ''' [内核][screen_stop]当前站台的描述
    ''' </summary>
    ''' <remarks></remarks>
    Public cross_stop As String = ""

    '==============================已过时，现在只显示一边站台=================================
    ''' <summary>
    ''' [内核][screen_stop]站台中显示交汇的数组-----第一个数字指示是第几个线路，第二个数字0=站台名 1=经过的车辆的组
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_stop_line(50, 2) As String
    ''' <summary>
    ''' [内核][screen_stop]站台中显示交汇的数组-----之前的站台所包含的项的数目
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_stop_line_list As Integer = 0

    '==================================真实站台内容====================================================
    ''' <summary>
    ''' [内核][screen_stop]站台中真实站台内容（序号，0=线路名 1=上行最近车差距 2=上行第二近车差距 3=上行方向 4=上行的当前选中站台的所在序列数 5=下行最近车差距 6=下行第二近车差距 7=下行方向 8=下行的当前选中站台所在序列数）差距为空或为-1，没有
    ''' </summary>
    Public realistic_stop(50, 8) As String
    ''' <summary>
    ''' [内核][screen_stop]站台中真实站台内容的个数
    ''' </summary>
    Public realistic_stop_list As Integer = 0

    '******************************************************最短路径**************************************************
    ''' <summary>
    ''' [内核][screen_stop]最短路径起始站点
    ''' </summary>
    ''' <remarks></remarks>
    Public stamp_start_stop As String = ""
    ''' <summary>
    ''' [内核][screen_stop]最短路径终到站点
    ''' </summary>
    ''' <remarks></remarks>
    Public stamp_end_stop As String = ""

    ''' <summary>
    ''' [内核][screen_stop]最短路径刚刚计算完成时--之前选择的起点
    ''' </summary>
    ''' <remarks></remarks>
    Public before_stamp_start_stop As String = ""
    ''' <summary>
    ''' [内核][screen_stop]最短路径刚刚计算完成时--之前选择的终点
    ''' </summary>
    ''' <remarks></remarks>
    Public before_stamp_end_stop As String = ""

    ''' <summary>
    ''' [内核][screen_stop]最短路径上一次执行执行的是联网的还是本地的true=联网 false=本地
    ''' </summary>
    Public short_rode_calc_mode As Boolean = Nothing

    ''' <summary>
    ''' [内核][screen_stop]最短路径步骤数字
    ''' </summary>
    ''' <remarks></remarks>
    Public step_number As Integer = 0
    ''' <summary>
    ''' [内核][screen_stop]最短路径存放如何行驶的数组，(步骤数，0=stop 1=line 2=toword)
    ''' </summary>
    ''' <remarks></remarks>
    Public toword(20, 3) As String
    ''' <summary>
    ''' [内核][screen_stop]最短路径是否使用本地解析模式
    ''' </summary>
    Public toword_local As Boolean = True

    ''' <summary>
    ''' [内核][screen_stop]最短路径呈现页面是否可以翻页
    ''' </summary>
    ''' <remarks></remarks>
    Public short_line_can_page As Boolean = True

    ''' <summary>
    ''' [内核][screen_stop]指示该页面是不是从线路界面跳转过来的，如果是，返回线路界面而不是主界面
    ''' </summary>
    Public jump_to_stop_from_line As Boolean = False

    ''' <summary>
    ''' [内核][screen_stop]最短路径解析end_result的函数，放入toword
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub word_to_gui()
        '归0
        step_number = 0
        For a = 0 To 20
            For b = 0 To 3
                toword(a, b) = ""
            Next
        Next

        If end_result(0).Name <> "" Then
            toword(0, 0) = stamp_start_stop
            For b = 0 To 16
                toword(b, 1) = end_result(b).Name
                toword(b, 2) = end_result(b).Toword
                If b = 16 Then
                    Exit For
                Else
                    If end_result(b + 1).Name = "" Then
                        Exit For
                    Else
                        toword(b + 1, 0) = end_result(b).TransferStop
                    End If
                End If
            Next
        End If

    End Sub


    '******************************************************函数********************************************************************************************
    ''' <summary>
    ''' [内核][screen_stop]以当前选定车站刷新界面数据
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub refsh_stop()
        Dim file As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\stop.txt", System.Text.Encoding.UTF8)

        '清空
        bus_stop_line_list = 0
        For a = 0 To 50
            For b = 0 To 2
                bus_stop_line(a, b) = ""
            Next
        Next

        Dim word As String = ""
        Do
            word = file.ReadLine
            If word = bus_stop_stop(now_stop) Then
                Dim linshi As String = file.ReadLine()
                cross_stop = read_resources_describe_into_memory("lang_code_stop_now_stop") + bus_stop_stop(now_stop) + vbCrLf + read_resources_describe_into_memory("lang_code_stop_cross_line") + linshi

                '输出下端交汇列表
                get_bus_stop_line(linshi)
                '读取真实站台
                get_realistic_stop(linshi)
                Exit Do
            Else
                file.ReadLine()
                file.ReadLine()
            End If

        Loop

        file.Dispose()

        '读总站信息
        Dim file_2 As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\station.txt", System.Text.Encoding.UTF8)

        word = ""
        how_station = False

        Do
            word = file_2.ReadLine
            If word = "" Then
                Exit Do
            End If
            If word = bus_stop_stop(now_stop) Then
                how_station = True
                cross_stop = cross_stop + vbCrLf + read_resources_describe_into_memory("lang_code_stop_station_describe") + file_2.ReadLine + vbCrLf + file_2.ReadLine + vbCrLf + file_2.ReadLine + vbCrLf + file_2.ReadLine + vbCrLf + file_2.ReadLine

                Exit Do
            End If

            file_2.ReadLine()
            file_2.ReadLine()
            file_2.ReadLine()
            file_2.ReadLine()
            file_2.ReadLine()
            file_2.ReadLine()
        Loop

        file_2.Dispose()

        '输出ui
        ui_connet_core_form_stop_cross_line_list.Clear()
        'describe由外部代码实现
        Dim d As New ui_depend_stop_cross_line_list
        For f = 0 To bus_stop_line_list - 1
            d.ui_stop_name = bus_stop_line(f, 0)
            d.ui_cross_line = bus_stop_line(f, 1)
            ui_connet_core_form_stop_cross_line_list.Add(d)
            d = New ui_depend_stop_cross_line_list
        Next

    End Sub

    ''' <summary>
    ''' [内核][screen_stop]检测站台列表里是否含有指定站台
    ''' </summary>
    ''' <param name="key">要搜索的站台名</param>
    ''' <returns>是否包含要搜索的站台名</returns>
    ''' <remarks></remarks>
    Public Function check_stop_list(ByVal key As String)
        Dim yes As Boolean = False

        For test1 = 0 To 10000
            If bus_stop_stop(test1) = "" Then
                Exit For
            End If
            If bus_stop_stop(test1) = key Then
                yes = True
                Exit For
            End If
        Next

        Return yes
    End Function

    ''' <summary>
    ''' [内核][screen_stop]返回搜索到的站台位于列表的位置，从0起始，搜索不到返回-1
    ''' </summary>
    ''' <param name="key">要搜索的站台名</param>
    ''' <returns>列表序数</returns>
    ''' <remarks></remarks>
    Public Function return_check_stop_list_nopage(ByVal key As String)
        Dim yes As Integer = -1

        For test1 = 0 To 10000
            If bus_stop_stop(test1) = key Then
                yes = test1
                Exit For
            End If
        Next

        Return yes
    End Function

    ''' <summary>
    ''' [内核][screen_stop]获取站台交汇情况的主函数
    ''' </summary>
    ''' <param name="word">输入经过中心站台的车辆字符串---按照 车辆组 字符串模式编码，即按空格分隔</param>
    ''' <remarks></remarks>
    Public Sub get_bus_stop_line(ByVal word As String)

        Dim word_arr() As String = word.Split(" ")

        For a = 0 To word_arr.Count - 1
            If word_arr(a) <> "" Then get_bus_stop_line_ex(word_arr(a))
        Next

    End Sub

    ''' <summary>
    ''' [内核][screen_stop]获取站台交汇情况的处理函数，不可直接调用，只能供get_bus_stop_line调用
    ''' </summary>
    ''' <param name="bus_name">输入经过中心站台的某一个车辆名</param>
    ''' <remarks></remarks>
    Public Sub get_bus_stop_line_ex(ByVal bus_name As String)
        Dim file As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\bus.txt", System.Text.Encoding.UTF8)

        Dim before_word As String = ""
        Dim word As String = ""
        Dim last_word As String = ""

        '是否重检查下行
        Dim check_again As Boolean = True
        Dim check_word As String = "ENDLINE"
        Dim again_before_word As String = ""
        Dim again_last_word As String = ""
        Dim linshi1 As String = ""


        Do
            If file.ReadLine = bus_name Then

                '跳过无用的值
                file.ReadLine()
                file.ReadLine()
                file.ReadLine()
                file.ReadLine()
                file.ReadLine()
                file.ReadLine()
                file.ReadLine()
                file.ReadLine()
                file.ReadLine()
                file.ReadLine()
                file.ReadLine()

                '站点
again:
                Do
                    word = file.ReadLine
                    If word = check_word Then
                        '防止出错
                        If check_again = False Then
                            Environment.Exit(7)
                        Else
                            linshi1 = word
                            Exit Do
                        End If

                    End If
                    If word = bus_stop_stop(now_stop) Then
                        '开始写入,获取下一个站点
                        last_word = file.ReadLine
                        linshi1 = last_word
                        If last_word = check_word Then
                            last_word = ""
                        End If

                        If check_again = False And last_word = again_before_word And before_word = again_last_word Then
                            GoTo end_fx
                        End If

                        '开始写入***********************************************
                        If before_word = "" Then

                            '没有上一站，写下站
                            If check_item_stop(last_word) <> -1 Then
                                '列表存在这个站台，写数据

                                If check_item_stop_have_bus(bus_stop_line(check_item_stop(last_word), 1), bus_name) = False Then
                                    '在找到的项目中没有这个车次，写入，找到了不写
                                    bus_stop_line(check_item_stop(last_word), 1) &= bus_name + " "
                                End If
                            Else
                                '列表不存在，写站台，写数据
                                bus_stop_line(bus_stop_line_list, 0) = last_word
                                bus_stop_line(bus_stop_line_list, 1) = bus_name + " "
                                bus_stop_line_list += 1
                            End If

                        Else
                            If last_word = "" Then

                                '没有下一站，写下站
                                If check_item_stop(before_word) <> -1 Then
                                    '列表存在这个站台，写数据

                                    If check_item_stop_have_bus(bus_stop_line(check_item_stop(before_word), 1), bus_name) = False Then
                                        '在找到的项目中没有这个车次，写入，找到了不写
                                        bus_stop_line(check_item_stop(before_word), 1) &= bus_name + " "
                                    End If
                                Else
                                    '列表不存在，写站台，写数据
                                    bus_stop_line(bus_stop_line_list, 0) = before_word
                                    bus_stop_line(bus_stop_line_list, 1) = bus_name + " "
                                    bus_stop_line_list += 1
                                End If

                            Else
                                '上下站都有，都写

                                '===============上站
                                If check_item_stop(before_word) <> -1 Then
                                    '列表存在这个站台，写数据

                                    If check_item_stop_have_bus(bus_stop_line(check_item_stop(before_word), 1), bus_name) = False Then
                                        '在找到的项目中没有这个车次，写入，找到了不写
                                        bus_stop_line(check_item_stop(before_word), 1) &= bus_name + " "
                                    End If
                                Else
                                    '列表不存在，写站台，写数据
                                    bus_stop_line(bus_stop_line_list, 0) = before_word
                                    bus_stop_line(bus_stop_line_list, 1) = bus_name + " "
                                    bus_stop_line_list += 1
                                End If

                                '===============下站
                                If check_item_stop(last_word) <> -1 Then
                                    '列表存在这个站台，写数据

                                    If check_item_stop_have_bus(bus_stop_line(check_item_stop(last_word), 1), bus_name) = False Then
                                        '在找到的项目中没有这个车次，写入，找到了不写
                                        bus_stop_line(check_item_stop(last_word), 1) &= bus_name + " "
                                    End If
                                Else
                                    '列表不存在，写站台，写数据
                                    bus_stop_line(bus_stop_line_list, 0) = last_word
                                    bus_stop_line(bus_stop_line_list, 1) = bus_name + " "
                                    bus_stop_line_list += 1
                                End If


                            End If
                        End If
                        '结束写入*************************************************
                        check_again = False
                        '退出
                        Exit Do
                    End If
                    before_word = word
                Loop

                '检查要不要再来一下
                If check_again = True Then
                    check_again = False
                    check_word = "END"
                    again_before_word = before_word
                    again_last_word = last_word
                    last_word = ""
                    before_word = ""
                    If linshi1 <> "ENDLINE" Then
                        Do
                            word = file.ReadLine
                            If word = "ENDLINE" Then
                                Exit Do
                            End If
                        Loop
                    End If

                    GoTo again
                End If


                Exit Do
            End If
        Loop
end_fx:

        file.Dispose()
    End Sub

    '===================================================================================
    ''' <summary>
    ''' 获取真实站台内容
    ''' </summary>
    ''' <param name="cross_line">经过的车次</param>
    Public Sub get_realistic_stop(ByVal cross_line As String)

        '清空
        If realistic_stop_list <> 0 Then
            For a = 0 To realistic_stop_list - 1
                For b = 0 To 8
                    realistic_stop(a, b) = ""
                Next
            Next
            realistic_stop_list = 0
        End If


        '读入
        Dim word_arr() As String = cross_line.Split(" ")

        For a = 0 To word_arr.Count - 1
            If word_arr(a) <> "" Then
                If check_realistic_stop_have_item(word_arr(a)) = False Then
                    realistic_stop(realistic_stop_list, 0) = word_arr(a)
                    '获取有关数据
                    Dim str As String = get_start_and_end_stop_in_line(word_arr(a), bus_stop_stop(now_stop))
                    If str <> "" Then
                        Dim str_sp() As String = str.Split(",")
                        realistic_stop(realistic_stop_list, 3) = str_sp(0)
                        realistic_stop(realistic_stop_list, 7) = str_sp(1)
                        realistic_stop(realistic_stop_list, 4) = str_sp(2)
                        realistic_stop(realistic_stop_list, 8) = str_sp(3)
                    Else
                        realistic_stop(realistic_stop_list, 3) = read_resources_describe_into_memory("lang_global_nothing")
                        realistic_stop(realistic_stop_list, 7) = read_resources_describe_into_memory("lang_global_nothing")
                        realistic_stop(realistic_stop_list, 4) = "-1"
                        realistic_stop(realistic_stop_list, 8) = "-1"
                    End If

                    realistic_stop_list += 1
                End If
            End If
        Next

        '先写入内容
        ui_connet_core_form_stop_realistic_stop_list.Clear()
        Dim linshi As ui_depend_stop_realistic_stop_list = New ui_depend_stop_realistic_stop_list
        For a = 0 To realistic_stop_list - 1
            linshi.ui_line_name = realistic_stop(a, 0)
            linshi.ui_up_line_toward = realistic_stop(a, 3)
            linshi.ui_up_line_describe_1 = read_resources_describe_into_memory("lang_code_MainWindow_get_realistic_stop_timer_function_first_nothing")
            linshi.ui_up_line_describe_2 = read_resources_describe_into_memory("lang_code_MainWindow_get_realistic_stop_timer_function_secound_nothing")
            linshi.ui_down_line_toward = realistic_stop(a, 7)
            linshi.ui_down_line_describe_1 = read_resources_describe_into_memory("lang_code_MainWindow_get_realistic_stop_timer_function_first_nothing")
            linshi.ui_down_line_describe_2 = read_resources_describe_into_memory("lang_code_MainWindow_get_realistic_stop_timer_function_secound_nothing")

            linshi.ui_up_bk_color = New SolidColorBrush(Color.FromArgb(255, 255, 255, 255))
            linshi.ui_down_bk_color = New SolidColorBrush(Color.FromArgb(255, 255, 255, 255))

            ui_connet_core_form_stop_realistic_stop_list.Add(linshi)
            linshi = New ui_depend_stop_realistic_stop_list
        Next

        ''启动获取task，可取消版
        'If get_bus_addr = False Then Exit Sub

    End Sub

    ''' <summary>
    ''' 检测realistic_stop里是否含有某些项
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    Public Function check_realistic_stop_have_item(ByVal name As String) As Boolean
        For a = 0 To 50
            If realistic_stop(a, 0) = "" Then Exit For
            If realistic_stop(a, 0) = name Then Return True
        Next
        Return False
    End Function

    '===================================================================================
    ''' <summary>
    ''' [内核][screen_stop]检索在指定交汇，中是否有指定项，有返回项的索引，否则返回-1，仅供get_bus_stop_line_ex调用
    ''' </summary>
    ''' <param name="name">要检索的之前站台的名称</param>
    ''' <returns>返回项的索引</returns>
    ''' <remarks></remarks>
    Public Function check_item_stop(ByVal name As String)
        Dim yes As Integer = -1

        For a = 0 To bus_stop_line_list - 1
            If bus_stop_line(a, 0) = name Then
                '在列表中有
                yes = a
            End If
        Next

        Return yes
    End Function

    ''' <summary>
    ''' [内核][screen_stop]检索指定前后区块中某个站台内是否含有指定的车辆名，仅供get_bus_stop_line_ex调用
    ''' </summary>
    ''' <param name="name">要查找的字符</param>
    ''' <param name="bus_name">查找的车辆名</param>
    ''' <returns></returns>
    Public Function check_item_stop_have_bus(ByVal name As String, ByVal bus_name As String)
        Dim yes As Boolean = False
        Dim arr() As String = name.Split(" ")

        For b = 0 To arr.Count - 2
            If arr(b) = "" Then
                Exit For
            End If

            If arr(b) = bus_name Then
                yes = True
                Exit For
            End If
        Next

        Return yes
    End Function

End Module

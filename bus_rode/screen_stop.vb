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

    '
    ''' <summary>
    ''' [内核][screen_stop]站台中显示交汇的数组-----之前的站台-第一个数字指示是第几个线路，第二个数字0=站台名 1=经过的车辆的组
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_stop_line_before(10, 2) As String
    ''' <summary>
    ''' [内核][screen_stop]站台中显示交汇的数组-----之前的站台所包含的项的数目
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_stop_line_before_list As Integer = 0
    '
    ''' <summary>
    ''' [内核][screen_stop]站台中显示交汇的数组-----之后的站台-第一个数字指示是第几个线路，第二个数字0=站台名 1=经过的车辆的组
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_stop_line_last(10, 2) As String
    ''' <summary>
    ''' [内核][screen_stop]站台中显示交汇的数组-----之后的站台所包含的项的数目
    ''' </summary>
    ''' <remarks></remarks>
    Public bus_stop_line_last_list As Integer = 0

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
        bus_stop_line_before_list = 0
        bus_stop_line_last_list = 0
        For a = 0 To 10
            For b = 0 To 2
                bus_stop_line_before(a, b) = ""
                bus_stop_line_last(a, b) = ""
            Next
        Next

        Dim word As String = ""
        Do
            word = file.ReadLine
            If word = bus_stop_stop(now_stop) Then
                Dim linshi As String = file.ReadLine()
                cross_stop = "当前站台：" + bus_stop_stop(now_stop) + vbCrLf + "经过车次：" + linshi

                '输出下端交汇列表
                get_bus_stop_line(linshi)
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
                cross_stop = cross_stop + vbCrLf + "以当前站为起点终点的" + file_2.ReadLine + vbCrLf + file_2.ReadLine + vbCrLf + file_2.ReadLine + vbCrLf + file_2.ReadLine + vbCrLf + file_2.ReadLine

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
        ui_connet_core_form_stop_left_cross_line_list.Clear()
        ui_connet_core_form_stop_right_cross_line_list.Clear()
        'describe由外部代码实现
        Dim d As New ui_depend_stop_cross_line_list
        For f = 0 To bus_stop_line_before_list - 1
            d.ui_stop_name = bus_stop_line_before(f, 0)
            d.ui_cross_line = bus_stop_line_before(f, 1)
            ui_connet_core_form_stop_left_cross_line_list.Add(d)
            d = New ui_depend_stop_cross_line_list
        Next
        For g = 0 To bus_stop_line_last_list - 1
            d.ui_stop_name = bus_stop_line_last(g, 0)
            d.ui_cross_line = bus_stop_line_last(g, 1)
            ui_connet_core_form_stop_right_cross_line_list.Add(d)
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
    ''' <param name="word">输入经过中心站台的车辆字符串---按照 车辆组 字符串模式编码</param>
    ''' <remarks></remarks>
    Public Sub get_bus_stop_line(ByVal word As String)
        Dim list As Integer = 1

        Do
            If Mid(word, list, 1) = "" Then
                Exit Do
            End If
            If Mid(word, list, 1) = " " Then
                get_bus_stop_line_ex(Mid(word, 1, list - 1))
                word = Mid(word, list + 1)
                list = 1
            Else
                list = list + 1
            End If
        Loop

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
                            '没有上一站
                            If check_item_before_stop(last_word) <> -1 Then
                                If check_item_stop_have_bus(bus_stop_line_before(check_item_before_stop(last_word), 1), bus_name) = False Then
                                    '在找到的项目中没有这个车次，写入，找到了不写
                                    bus_stop_line_before(check_item_before_stop(last_word), 1) = bus_stop_line_before(check_item_before_stop(last_word), 1) + bus_name + " "
                                End If
                            Else
                                If check_item_last_stop(last_word) <> -1 Then
                                    If check_item_stop_have_bus(bus_stop_line_last(check_item_last_stop(last_word), 1), bus_name) = False Then
                                        '在找到的项目中没有这个车次，写入，找到了不写
                                        bus_stop_line_last(check_item_last_stop(last_word), 1) = bus_stop_line_last(check_item_last_stop(last_word), 1) + bus_name + " "
                                    End If

                                Else
                                    If bus_stop_line_last_list >= bus_stop_line_before_list Then
                                        bus_stop_line_before(bus_stop_line_before_list, 0) = last_word
                                        bus_stop_line_before(bus_stop_line_before_list, 1) = bus_stop_line_before(bus_stop_line_before_list, 1) + bus_name + " "
                                        bus_stop_line_before_list = bus_stop_line_before_list + 1
                                    Else
                                        bus_stop_line_last(bus_stop_line_last_list, 0) = last_word
                                        bus_stop_line_last(bus_stop_line_last_list, 1) = bus_stop_line_last(bus_stop_line_last_list, 1) + bus_name + " "
                                        bus_stop_line_last_list = bus_stop_line_last_list + 1
                                    End If

                                End If
                            End If
                        Else
                            If last_word = "" Then
                                '没有下一站
                                If check_item_before_stop(before_word) <> -1 Then
                                    If check_item_stop_have_bus(bus_stop_line_before(check_item_before_stop(before_word), 1), bus_name) = False Then
                                        '在找到的项目中没有这个车次，写入，找到了不写
                                        bus_stop_line_before(check_item_before_stop(before_word), 1) = bus_stop_line_before(check_item_before_stop(before_word), 1) + bus_name + " "
                                    End If
                                Else
                                    If check_item_last_stop(before_word) <> -1 Then
                                        If check_item_stop_have_bus(bus_stop_line_last(check_item_last_stop(before_word), 1), bus_name) = False Then
                                            '在找到的项目中没有这个车次，写入，找到了不写
                                            bus_stop_line_last(check_item_last_stop(before_word), 1) = bus_stop_line_last(check_item_last_stop(before_word), 1) + bus_name + " "
                                        End If
                                    Else
                                        If bus_stop_line_last_list >= bus_stop_line_before_list Then
                                            bus_stop_line_before(bus_stop_line_before_list, 0) = before_word
                                            bus_stop_line_before(bus_stop_line_before_list, 1) = bus_stop_line_before(bus_stop_line_before_list, 1) + bus_name + " "
                                            bus_stop_line_before_list = bus_stop_line_before_list + 1
                                        Else
                                            bus_stop_line_last(bus_stop_line_last_list, 0) = before_word
                                            bus_stop_line_last(bus_stop_line_last_list, 1) = bus_stop_line_last(bus_stop_line_last_list, 1) + bus_name + " "
                                            bus_stop_line_last_list = bus_stop_line_last_list + 1
                                        End If

                                    End If
                                End If
                            Else
                                '上下站都有
                                If check_item_before_stop(before_word) <> -1 Then
                                    '前在左？
                                    If check_item_stop_have_bus(bus_stop_line_before(check_item_before_stop(before_word), 1), bus_name) = False Then
                                        '在找到的项目中没有这个车次，写入，找到了不写
                                        bus_stop_line_before(check_item_before_stop(before_word), 1) = bus_stop_line_before(check_item_before_stop(before_word), 1) + bus_name + " "
                                    End If
                                    If check_item_last_stop(last_word) <> -1 Then
                                        If check_item_stop_have_bus(bus_stop_line_last(check_item_last_stop(last_word), 1), bus_name) = False Then
                                            '在找到的项目中没有这个车次，写入，找到了不写
                                            bus_stop_line_last(check_item_last_stop(last_word), 1) = bus_stop_line_last(check_item_last_stop(last_word), 1) + bus_name + " "
                                        End If
                                    Else
                                        bus_stop_line_last(bus_stop_line_last_list, 0) = last_word
                                        bus_stop_line_last(bus_stop_line_last_list, 1) = bus_stop_line_last(bus_stop_line_last_list, 1) + bus_name + " "
                                        bus_stop_line_last_list = bus_stop_line_last_list + 1
                                    End If
                                Else
                                    If check_item_last_stop(before_word) <> -1 Then
                                        '前在右
                                        If check_item_stop_have_bus(bus_stop_line_last(check_item_last_stop(before_word), 1), bus_name) = False Then
                                            '在找到的项目中没有这个车次，写入，找到了不写
                                            bus_stop_line_last(check_item_last_stop(before_word), 1) = bus_stop_line_last(check_item_last_stop(before_word), 1) + bus_name + " "
                                        End If
                                        If check_item_before_stop(last_word) <> -1 Then
                                            If check_item_stop_have_bus(bus_stop_line_before(check_item_before_stop(last_word), 1), bus_name) = False Then
                                                '在找到的项目中没有这个车次，写入，找到了不写
                                                bus_stop_line_before(check_item_before_stop(last_word), 1) = bus_stop_line_before(check_item_before_stop(last_word), 1) + bus_name + " "
                                            End If
                                        Else
                                            bus_stop_line_before(bus_stop_line_before_list, 0) = last_word
                                            bus_stop_line_before(bus_stop_line_before_list, 1) = bus_stop_line_before(bus_stop_line_before_list, 1) + bus_name + " "
                                            bus_stop_line_before_list = bus_stop_line_before_list + 1
                                        End If
                                    Else
                                        If check_item_before_stop(last_word) <> -1 Then
                                            '后在左？
                                            If check_item_stop_have_bus(bus_stop_line_before(check_item_before_stop(last_word), 1), bus_name) = False Then
                                                '在找到的项目中没有这个车次，写入，找到了不写
                                                bus_stop_line_before(check_item_before_stop(last_word), 1) = bus_stop_line_before(check_item_before_stop(last_word), 1) + bus_name + " "
                                            End If
                                            If check_item_last_stop(before_word) <> -1 Then
                                                If check_item_stop_have_bus(bus_stop_line_last(check_item_last_stop(before_word), 1), bus_name) = False Then
                                                    '在找到的项目中没有这个车次，写入，找到了不写
                                                    bus_stop_line_last(check_item_last_stop(before_word), 1) = bus_stop_line_last(check_item_last_stop(before_word), 1) + bus_name + " "
                                                End If
                                            Else
                                                bus_stop_line_last(bus_stop_line_last_list, 0) = before_word
                                                bus_stop_line_last(bus_stop_line_last_list, 1) = bus_stop_line_last(bus_stop_line_last_list, 1) + bus_name + " "
                                                bus_stop_line_last_list = bus_stop_line_last_list + 1
                                            End If
                                        Else
                                            If check_item_last_stop(last_word) <> -1 Then
                                                '后在右？
                                                If check_item_stop_have_bus(bus_stop_line_last(check_item_last_stop(last_word), 1), bus_name) = False Then
                                                    '在找到的项目中没有这个车次，写入，找到了不写
                                                    bus_stop_line_last(check_item_last_stop(last_word), 1) = bus_stop_line_last(check_item_last_stop(last_word), 1) + bus_name + " "
                                                End If
                                                If check_item_before_stop(before_word) <> -1 Then
                                                    If check_item_stop_have_bus(bus_stop_line_before(check_item_before_stop(before_word), 1), bus_name) = False Then
                                                        '在找到的项目中没有这个车次，写入，找到了不写
                                                        bus_stop_line_before(check_item_before_stop(before_word), 1) = bus_stop_line_before(check_item_before_stop(before_word), 1) + bus_name + " "
                                                    End If
                                                Else
                                                    bus_stop_line_before(bus_stop_line_before_list, 0) = before_word
                                                    bus_stop_line_before(bus_stop_line_before_list, 1) = bus_stop_line_before(bus_stop_line_before_list, 1) + bus_name + " "
                                                    bus_stop_line_before_list = bus_stop_line_before_list + 1
                                                End If
                                            Else
                                                '老老实实写前在左，后在右
                                                bus_stop_line_before(bus_stop_line_before_list, 0) = before_word
                                                bus_stop_line_before(bus_stop_line_before_list, 1) = bus_stop_line_before(bus_stop_line_before_list, 1) + bus_name + " "
                                                bus_stop_line_before_list = bus_stop_line_before_list + 1

                                                bus_stop_line_last(bus_stop_line_last_list, 0) = last_word
                                                bus_stop_line_last(bus_stop_line_last_list, 1) = bus_stop_line_last(bus_stop_line_last_list, 1) + bus_name + " "
                                                bus_stop_line_last_list = bus_stop_line_last_list + 1
                                            End If
                                        End If
                                    End If
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


    ''' <summary>
    ''' [内核][screen_stop]检索在指定交汇--左侧，中是否有指定项，有返回项的索引，否则返回-1，仅供get_bus_stop_line_ex调用
    ''' </summary>
    ''' <param name="name">要检索的之前站台的名称</param>
    ''' <returns>返回项的索引</returns>
    ''' <remarks></remarks>
    Public Function check_item_before_stop(ByVal name As String)
        Dim yes As Integer = -1

        For a = 0 To bus_stop_line_before_list - 1
            If bus_stop_line_before(a, 0) = name Then
                '在列表中有
                yes = a
            End If
        Next

        Return yes
    End Function

    ''' <summary>
    ''' [内核][screen_stop]检索在指定交汇--右侧，中是否有指定项，有返回项的索引，否则返回-1，仅供get_bus_stop_line_ex调用
    ''' </summary>
    ''' <param name="name">要检索的之后站台的名称</param>
    ''' <returns>返回项的索引</returns>
    ''' <remarks></remarks>
    Public Function check_item_last_stop(ByVal name As String)
        Dim yes As Integer = -1

        For a = 0 To bus_stop_line_last_list - 1
            If bus_stop_line_last(a, 0) = name Then
                '在列表中有
                yes = a
            End If
        Next

        Return yes
    End Function

    ''' <summary>
    ''' 检索指定前后区块中某个站台内是否含有指定的车辆名，仅供get_bus_stop_line_ex调用
    ''' </summary>
    ''' <param name="name">查找的字符</param>
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

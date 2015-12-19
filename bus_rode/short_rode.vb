Module short_rode
    '计算的线程
    Public main_calc_td As New System.Threading.Thread(AddressOf main_calc)
    '允许计算开始设置的标识符
    Public run_calc_flag As Boolean = False

    '整个计算交会车次的2个三维数组 访问模式(组序号，每个车次的序号)
    Public go_bus_name_list(7, 100) As obj_short_rode_line
    Public end_bus_name_list(7, 100) As obj_short_rode_line
    'Public go_bus_name_list_number As Integer = 0
    'Public end_bus_name_list_number As Integer = 0
    Public group_list As Short = 0

    '输出结果
    Public result(16) As obj_short_rode_line
    '最终结果
    Public end_result(16) As obj_end_result

    '函数计算的结果存放地
    '来自both_have_bus_name的结果，相同车的名字：车名+空格+   车名+空格
    '来自two_jiaohui_bus_name的结果，格式：车名1+空格+车名2+空格+   车名1+空格+车名2+空格
    Public function_calc As String = ""


    '标识符
    '标示是否进行接下来的计算
    Public can_calc As Boolean = True
    '表示function_calc采用的是哪种结构0=both_have_bus_name 1=two_jiaohui_bus_name
    Public style_function_calc As Short = 0


    '来自首末班车时间的返回值
    Public function_result(4) As Short

    Public Structure obj_short_rode_line
        ''' <summary>
        ''' 线路名
        ''' </summary>
        Public Name As String
        ''' <summary>
        ''' 依赖的上级车辆，若为顶级车辆则留空
        ''' </summary>
        Public Depend As String
        ''' <summary>
        ''' 等待难度（0最难，4最容易）
        ''' </summary>
        Public Difficult As Short
        ''' <summary>
        ''' 运营开始时间小时数
        ''' </summary>
        Public RunTimeStartHour As Short
        ''' <summary>
        ''' 运营开始时间分钟数
        ''' </summary>
        Public RunTimeStartMinute As Short
        ''' <summary>
        ''' 运营结束时间小时数
        ''' </summary>
        Public RunTimeEndHour As Short
        ''' <summary>
        ''' 运营结束时间分钟数
        ''' </summary>
        Public RunTimeEndMinute As Short
        ''' <summary>
        ''' 换乘的距离
        ''' </summary>
        Public Far As Short
    End Structure

    Public Structure obj_end_result
        ''' <summary>
        ''' 线路名称
        ''' </summary>
        Public Name As String
        ''' <summary>
        ''' 换乘方向
        ''' </summary>
        Public Toword As String
        ''' <summary>
        ''' 坐到哪站换乘
        ''' </summary>
        Public TransferStop As String
    End Structure


    '核心计算函数
    Public Sub main_calc()
        Do
            Do
                If run_calc_flag = True Then
                    run_calc_flag = False
                    Exit Do
                Else
                    System.Threading.Thread.Sleep(1000)
                End If
            Loop

            '实际运行代码
            '清空
            For a = 0 To 16
                result(a).Name = ""
                end_result(a).Name = ""
            Next
            function_calc = ""
            can_calc = True
            style_function_calc = 0
            group_list = 0

            For q = 0 To 7
                For r = 0 To 100
                    go_bus_name_list(q, r).Name = ""
                    end_bus_name_list(q, r).Name = ""
                Next
            Next

            '检查是否有所需资源，没有就编译
            If System.IO.File.Exists(Environment.CurrentDirectory + "\library\short_stop.txt") And System.IO.File.Exists(Environment.CurrentDirectory + "\library\short_line.txt") Then
                '已编译
            Else
                '没有编译，立即编译
                build_file()
                '暂停3S，减少长时间使用
                System.Threading.Thread.Sleep(3000)
            End If
            MsgBox("aaa")

            '*********************************************************************************************************************
            '辨别线路交会
            '写入始末站台
            Dim linshi1 As String = ""
            Dim linshi2 As String = ""
            linshi1 = search_stop_have_bus(stamp_start_stop)
            linshi2 = search_stop_have_bus(stamp_end_stop)
            Dim linshi3 As Integer = 1
            Dim linshi_arr_1() As String = linshi1.Split(" ")
            Dim linshi_arr_2() As String = linshi2.Split(" ")

            '写入go_bus_name_list
            For a = 0 To linshi_arr_1.Count - 2
                '写入车次
                '检查是否有
                For b = 0 To 100
                    If go_bus_name_list(0, b).Name = "" Then
                        '写入
                        go_bus_name_list(0, b).Name = linshi_arr_1(a)
                        '获取完全信息
                        get_bus_msg(go_bus_name_list(0, b).Name, go_bus_name_list(0, b).Difficult, go_bus_name_list(0, b).RunTimeStartHour,
                                    go_bus_name_list(0, b).RunTimeStartMinute, go_bus_name_list(0, b).RunTimeEndHour, go_bus_name_list(0, b).RunTimeEndMinute)
                        Exit For
                    End If
                    If go_bus_name_list(0, b).Name = linshi_arr_1(a) Then
                        '有了，不写入
                        Exit For
                    End If
                Next
            Next


            '写入end_bus_name_list
            For a = 0 To linshi_arr_2.Count - 2
                '写入车次
                '检查是否有
                For b = 0 To 100
                    If end_bus_name_list(0, b).Name = "" Then
                        '写入
                        end_bus_name_list(0, b).Name = linshi_arr_2(a)
                        '获取完全信息
                        get_bus_msg(end_bus_name_list(0, b).Name, end_bus_name_list(0, b).Difficult, end_bus_name_list(0, b).RunTimeStartHour,
                                    end_bus_name_list(0, b).RunTimeStartMinute, end_bus_name_list(0, b).RunTimeEndHour, end_bus_name_list(0, b).RunTimeEndMinute)
                        Exit For
                    End If
                    If end_bus_name_list(0, b).Name = linshi_arr_2(a) Then
                        '有了，不写入
                        Exit For
                    End If
                Next
            Next


            'do循环开始一遍一遍寻找
            Do
                If go_bus_name_list(group_list, 0).Name = "" Or end_bus_name_list(group_list, 0).Name = "" Then
                    '没有车次能连接这两个站，拒绝操作

                    can_calc = False
                    Exit Do
                End If

                '检索两个站台拥有的数组有无同样的车次
                If both_have_bus_name() = True Then

                    style_function_calc = 0
                    Exit Do
                End If

                '检索两个站台拥有的数组有无能交汇的车次
                If two_jiaohui_bus_name() = True Then

                    style_function_calc = 1
                    Exit Do
                End If



                '结束检测*******************最后处理结束
                If group_list = 7 Then
                    '没有车次能连接这两个站(超过换乘最大次数)，拒绝操作

                    can_calc = False
                    Exit Do
                End If


                '处理，引出下一级车辆列表
                creat_next_arr()

                '自增
                group_list += 1
            Loop

            MsgBox("bbb")

            If can_calc = True Then
                '*********************************************************************************************************************
                '向上索引并填充result
                search_and_fill_result()

                MsgBox("ccc")

                '*********************************************************************************************************************
                '计算每条线路之间应该怎么换乘
                fenxi_result()

                MsgBox("ddd")
                For a = 0 To 16
                    If end_result(a).Name = "" Then
                        Exit For
                    End If

                    debug_tostring(end_result(a), "")
                Next

                '转换结果并呈现
                word_to_gui()

            End If
            '立即回收内存
            GC.Collect()

            '结束工作，向主线程发信号
            short_line_can_page = False

        Loop
    End Sub

    Public Sub debug_tostring(ByVal test As obj_end_result, ByVal after_word As String)
        Dim yes As String

        yes = test.Name & vbCrLf & test.Toword & vbCrLf & test.TransferStop & vbCrLf & vbCrLf
        yes = after_word & vbCrLf & vbCrLf & yes

        MsgBox(yes)
    End Sub

    ''' <summary>
    ''' []获取车辆有关信息的函数
    ''' </summary>
    Public Sub get_bus_msg(ByVal in_name As String, ByRef out_difficult As Short, ByRef out_run_time_start_hour As Short,
                           ByRef out_run_time_start_minute As Short, out_run_time_end_hour As Short, out_run_time_end_minute As Short)

        Dim file_line As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\bus.txt", System.Text.Encoding.UTF8)
        Dim word As String = ""

        Do
            word = file_line.ReadLine
            If word = "ENDDATE" Then
                Exit Do
            End If

            If word = in_name Then
                '跳过无用的值
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()

                '输出
                out_difficult = CType(file_line.ReadLine(), Short)

                out_run_time_start_hour = CType(file_line.ReadLine(), Short)
                out_run_time_start_minute = CType(file_line.ReadLine(), Short)
                out_run_time_end_hour = CType(file_line.ReadLine(), Short)
                out_run_time_end_minute = CType(file_line.ReadLine(), Short)

                Exit Do

            End If
            '跳过无用的值
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()

            '刷完站台
            Do
                word = file_line.ReadLine

                If word = "END" Then
                    Exit Do
                End If

            Loop

        Loop
        file_line.Dispose()

    End Sub

    '*********************************************************************************************************************
    '编译文件函数
    Public Sub build_file()
        'short_stop的文件
        Dim file_short_stop As New System.IO.StreamWriter(Environment.CurrentDirectory + "\library\short_stop.txt", False, System.Text.Encoding.UTF8)
        '读取stop的文件
        Dim file_stop As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\stop.txt", System.Text.Encoding.UTF8)

        Dim word As String = ""
        Dim list As Integer = 1

        Do
            word = file_stop.ReadLine
            If word = "" Then
                Exit Do
            End If

            '保存站台名
            file_short_stop.WriteLine(word)
            '分行写入经过车次
            word = file_stop.ReadLine
            list = 1
            Do
                If Mid(word, List, 1) = "" Then
                    Exit Do
                End If
                If Mid(word, list, 1) = " " Then
                    '写入车次
                    file_short_stop.WriteLine(Mid(word, 1, list - 1))

                    word = Mid(word, list + 1)
                    list = 1
                Else
                    list = list + 1
                End If
            Loop
            '写入end
            file_short_stop.WriteLine("END")

            '读空值
            file_stop.ReadLine()

        Loop

        '释放资源
        file_short_stop.Dispose()
        file_stop.Dispose()

        '编译short_line
        Dim file_short_line As New System.IO.StreamWriter(Environment.CurrentDirectory + "\library\short_line.txt", False, System.Text.Encoding.UTF8)
        Dim file_line As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\bus.txt", System.Text.Encoding.UTF8)

        '用于存放当前线路能换乘的车次
        Dim this_bus_can_tf(100) As String
        Dim this_bus_can_tf_list As Integer = 0

        '记录当前的线路名
        Dim this_bus_name As String = ""

        word = ""

        Do
            word = file_line.ReadLine
            If word = "ENDDATE" Then
                Exit Do
            End If

            '跳过无用的值
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()

            '清空
            For a = 0 To 100
                this_bus_can_tf(a) = ""
            Next
            this_bus_can_tf_list = 0

            '写入线路名
            this_bus_name = word
            file_short_line.WriteLine(word)

            '寻找站台开始
            Do
                word = file_line.ReadLine

                If word = "ENDLINE" Then
                    Exit Do
                End If
                '获取经过车次
                word = search_stop_have_bus(word)


                Do
                    If Mid(word, list, 1) = "" Then
                        Exit Do
                    End If
                    If Mid(word, list, 1) = " " Then
                        '写入车次
                        If check_this_bus_can_tf(this_bus_can_tf, Mid(word, 1, list - 1)) = False And Mid(word, 1, list - 1) <> this_bus_name Then
                            this_bus_can_tf(this_bus_can_tf_list) = Mid(word, 1, list - 1)
                            file_short_line.WriteLine(Mid(word, 1, list - 1))
                            this_bus_can_tf_list += 1
                        End If

                        word = Mid(word, list + 1)
                        list = 1
                    Else
                        list = list + 1
                    End If
                Loop

            Loop
            '下行
            Do
                word = file_line.ReadLine

                If word = "END" Then
                    Exit Do
                End If
                '获取经过车次
                word = search_stop_have_bus(word)


                Do
                    If Mid(word, list, 1) = "" Then
                        Exit Do
                    End If
                    If Mid(word, list, 1) = " " Then
                        '写入车次
                        If check_this_bus_can_tf(this_bus_can_tf, Mid(word, 1, list - 1)) = False And Mid(word, 1, list - 1) <> this_bus_name Then
                            this_bus_can_tf(this_bus_can_tf_list) = Mid(word, 1, list - 1)
                            file_short_line.WriteLine(Mid(word, 1, list - 1))
                            this_bus_can_tf_list += 1
                        End If

                        word = Mid(word, list + 1)
                        list = 1
                    Else
                        list = list + 1
                    End If
                Loop

            Loop

            '寻找完毕，写入END
            file_short_line.WriteLine("END")

        Loop

        '完毕，写入ENDDATE
        file_short_line.WriteLine("ENDDATE")

        file_short_line.Dispose()
        file_line.Dispose()

    End Sub


    '寻找某一站可以换的车次
    Public Function search_stop_have_bus(ByVal stop_name As String)
        Dim bus_name As String = ""

        Dim file_stop As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\stop.txt", System.Text.Encoding.UTF8)

        Dim word As String = ""

        Do
            word = file_stop.ReadLine
            If word = "" Then
                Exit Do
            End If

            If word = stop_name Then
                '返回
                bus_name = file_stop.ReadLine
                Exit Do
            Else
                '空值
                file_stop.ReadLine()
                file_stop.ReadLine()
            End If

        Loop

        file_stop.Dispose()

        Return bus_name
    End Function

    Public Function check_this_bus_can_tf(ByVal arr() As String, ByVal search_name As String)
        Dim yes As Boolean = False

        For a = 0 To 100
            If arr(a) = "" Then
                Exit For
            End If
            If arr(a) = search_name Then
                yes = True
                Exit For
            End If
        Next

        Return yes
    End Function
    '*********************************************************************************************************************

    '检查两个数组内有无同样的车次,返回为boolean，结果存储在function_calc中
    Public Function both_have_bus_name()
        Dim yes As Boolean = False

        For a = 0 To 100
            If go_bus_name_list(group_list, a).Name = "" Then
                '没有了，跳出for
                Exit For
            End If
            For b = 0 To 100
                If end_bus_name_list(group_list, b).Name = "" Then
                    '没有了，跳出for
                    Exit For
                End If

                If go_bus_name_list(group_list, a).Name = end_bus_name_list(group_list, b).Name Then
                    '若发现相等的车次，写入发现的记号，记录在案，但不退出
                    yes = True
                    function_calc = function_calc + go_bus_name_list(group_list, a).Name + " "
                End If

            Next
        Next

        Return yes
    End Function

    '检查两个数组内有无能够交汇的2个车次,返回为boolean，结果存储在function_calc中
    Public Function two_jiaohui_bus_name()
        Dim yes As Boolean = False

        For a = 0 To 100
            If go_bus_name_list(group_list, a).Name = "" Then
                '没有了，跳出for
                Exit For
            End If

            '检索
            If search_bus_jiaohui_another_bus(go_bus_name_list(group_list, a).Name) = True Then
                '成功匹配，记录在案，但不退出
                yes = True
            End If
        Next

        Return yes
    End Function
    '*********************************************************************************************************************
    '生成下一级车辆列表
    Public Sub creat_next_arr()

        Dim list_2 As Integer = 0
        Dim word As String = ""
        Dim list As Integer = 1

        'go的总循环
        Do
            '若没有了，结束
            If go_bus_name_list(group_list, list_2).Name = "" Then
                Exit Do
            End If

            '获取线路
            word = search_bus_have_bus(go_bus_name_list(group_list, list_2).Name)
            Dim word_arr() As String = word.Split(" ")

            For a = 0 To word_arr.Count - 2
                '写入车次
                '检查是否有
                For b = 0 To 100
                    If go_bus_name_list(group_list + 1, b).Name = "" Then
                        '写入
                        go_bus_name_list(group_list + 1, b).Name = word_arr(a)
                        '写入从属
                        go_bus_name_list(group_list + 1, b).Depend = go_bus_name_list(group_list + 1, b).Depend + go_bus_name_list(group_list, list_2).Name + " "
                        Exit For
                    End If
                    If go_bus_name_list(group_list + 1, b).Name = word_arr(a) Then
                        '有了，写入从属关系表
                        go_bus_name_list(group_list + 1, b).Depend = go_bus_name_list(group_list + 1, b).Depend + go_bus_name_list(group_list, list_2).Name + " "
                        Exit For
                    End If
                Next
            Next

            '自增
            list_2 += 1

        Loop

        '归0
        list_2 = 0
        word = ""
        list = 1

        'end的总循环
        Do
            '若没有了，结束
            If end_bus_name_list(group_list, list_2).Name = "" Then
                Exit Do
            End If

            '获取线路
            word = search_bus_have_bus(end_bus_name_list(group_list, list_2).Name)
            Dim word_arr_2() As String = word.Split(" ")

            For b = 0 To word_arr_2.Count - 2
                '写入车次
                '检查是否有
                For a = 0 To 100
                    If end_bus_name_list(group_list + 1, a).Name = "" Then
                        '写入
                        end_bus_name_list(group_list + 1, a).Name = word_arr_2(b)
                        '写入从属
                        end_bus_name_list(group_list + 1, a).Depend = end_bus_name_list(group_list + 1, a).Depend + end_bus_name_list(group_list, list_2).Name + " "
                        Exit For
                    End If
                    If end_bus_name_list(group_list + 1, a).Name = word_arr_2(b) Then
                        '有了，写入从属关系表
                        end_bus_name_list(group_list + 1, a).Depend = end_bus_name_list(group_list + 1, a).Depend + end_bus_name_list(group_list, list_2).Name + " "
                        Exit For
                    End If
                Next
            Next

            '自增
            list_2 += 1

        Loop

        '完成

    End Sub

    '寻找某车次可以换的车次
    Public Function search_bus_have_bus(ByVal bus_name As String)
        Dim line_name As String = ""

        Dim file_line As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\short_line.txt", System.Text.Encoding.UTF8)

        Dim word As String = ""

        Do
            word = file_line.ReadLine
            If word = "ENDDATE" Then
                Exit Do
            End If

            If word = bus_name Then
                '返回
                Do
                    word = file_line.ReadLine
                    If word = "END" Then
                        Exit Do
                    End If

                    line_name = line_name + word + " "
                Loop
                Exit Do
            Else
                '空值
                Do
                    word = file_line.ReadLine
                    If word = "END" Then
                        Exit Do
                    End If
                Loop
            End If

        Loop

        file_line.Dispose()

        Return line_name
    End Function

    '*********************************************************************************************************************
    '寻找某车次能否与另组车次中的几辆交汇，返回boolean，结果记在function_calc
    Public Function search_bus_jiaohui_another_bus(ByVal bus_name As String)
        Dim yes As Boolean = False

        Dim file_line As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\short_line.txt", System.Text.Encoding.UTF8)


        Dim word As String = ""

        Do
            word = file_line.ReadLine
            If word = "ENDDATE" Then
                Exit Do
            End If

            If word = bus_name Then
                '返回
                Do
                    word = file_line.ReadLine
                    If word = "END" Then
                        Exit Do
                    End If

                    '开始查验
                    For b = 0 To 100
                        If end_bus_name_list(group_list, b).Name = "" Then
                            '没有了，跳出for
                            Exit For
                        End If

                        If word = end_bus_name_list(group_list, b).Name Then
                            '若发现相等的车次，写入发现的记号，记录在案，但不退出
                            yes = True
                            function_calc = function_calc + bus_name + " " + end_bus_name_list(group_list, b).Name + " "
                        End If

                    Next

                Loop
                Exit Do
            Else
                '空值
                Do
                    word = file_line.ReadLine
                    If word = "END" Then
                        Exit Do
                    End If
                Loop
            End If

        Loop

        file_line.Dispose()

        Return yes
    End Function

    '*********************************************************************************************************************
    '向上索引并填充result
    Public Sub search_and_fill_result()
        If style_function_calc = 0 Then
            'both mode
            '从function中选择一个好车次
            Dim mid_bus_name As obj_short_rode_line
            mid_bus_name = select_good_line_both(function_calc)

            '表示当前中央站台在写入result时所在的索引
            Dim mid_bus_list As Short = group_list
            '表示当前写入的偏移量
            Dim save_list As Short = 1

            '先写入中央站台
            result(mid_bus_list) = mid_bus_name
            '***************************************************

            '临时存储选中车次的变量
            Dim linshi_save As obj_short_rode_line


            '从go数组向上按照选出的好车次逐层索引，写入数据
            '获取需要获取depend的bus的索引
            Dim list As Short = return_bus_name_as_arr_go(group_list, mid_bus_name.Name)
            If group_list <> 0 Then
                For a = group_list - 1 To 0 Step -1
                    '获取需要获取depend的bus的索引
                    linshi_save = select_good_line_both(go_bus_name_list(a + 1, list).Depend)
                    '写入
                    result(mid_bus_list - save_list) = linshi_save
                    list = return_bus_name_as_arr_go(a, linshi_save.Name)
                    '加偏移量
                    save_list += 1
                Next
            End If

            '***************************************************
            '从end数组向上按照选出的好车次逐层索引，写入数据
            save_list = 1
            '获取需要获取depend的bus的索引
            list = return_bus_name_as_arr_end(group_list, mid_bus_name.Name)

            If group_list <> 0 Then
                For a = group_list - 1 To 0 Step -1
                    '获取需要获取depend的bus的索引
                    linshi_save = select_good_line_both(end_bus_name_list(a + 1, list).Depend)
                    '写入
                    result(mid_bus_list + save_list) = linshi_save
                    list = return_bus_name_as_arr_end(a, linshi_save.Name)
                    '加偏移量
                    save_list += 1
                Next
            End If

        Else
            'two mode
            '从function中选择一个好车次（同时2个）
            Dim mid_bus_name_1 As obj_short_rode_line
            Dim mid_bus_name_2 As obj_short_rode_line
            select_good_line_two(function_calc,mid_bus_name_1,mid_bus_name_2)

            '表示当前中央站台在写入result时所在的索引
            Dim mid_bus_list As Short = group_list
            '表示当前写入的偏移量
            Dim save_list As Short = 1

            '先写入中央站台
            result(mid_bus_list) = mid_bus_name_1
            result(mid_bus_list + 1) = mid_bus_name_2
            '***************************************************

            '临时存储选中车次的变量
            Dim linshi_save As obj_short_rode_line


            '从go数组向上按照选出的好车次逐层索引，写入数据
            '获取需要获取depend的bus的索引
            Dim list As Short = return_bus_name_as_arr_go(group_list, mid_bus_name_1.Name)
            If group_list <> 0 Then
                For a = group_list - 1 To 0 Step -1
                    '获取需要获取depend的bus的索引
                    linshi_save = select_good_line_both(go_bus_name_list(a + 1, list).Depend)
                    '写入
                    result(mid_bus_list - save_list) = linshi_save
                    list = return_bus_name_as_arr_go(a, linshi_save.Name)
                    '加偏移量
                    save_list += 1
                Next
            End If


            '***************************************************
            '从end数组向上按照选出的好车次逐层索引，写入数据
            '修复中央站台有2个的相关问题
            mid_bus_list += 1
            save_list = 1
            '获取需要获取depend的bus的索引
            list = return_bus_name_as_arr_end(group_list, mid_bus_name_2.Name)

            If group_list <> 0 Then
                For a = group_list - 1 To 0 Step -1
                    '获取需要获取depend的bus的索引
                    linshi_save = select_good_line_both(end_bus_name_list(a + 1, list).Depend)
                    '写入
                    result(mid_bus_list + save_list) = linshi_save
                    list = return_bus_name_as_arr_end(a, linshi_save.Name)
                    '加偏移量
                    save_list += 1
                Next
            End If

        End If
    End Sub

    '从文字列表中选择一个合适的线路并返回mode:both
    Public Function select_good_line_both(ByVal word As String) As obj_short_rode_line
        Dim select_line As obj_short_rode_line
        select_line.Name = ""

        '存放地点，(序列，0=name 1=等待难易度)
        Dim line_name(100) As obj_short_rode_line
        Dim line_name_list As Integer = 0
        Dim word_arr() As String = word.Split(" ")
        For a = 0 To word_arr.Count - 2
            '写入车次
            line_name(line_name_list).Name = word_arr(a)
            '获取完全信息
            get_bus_msg(line_name(line_name_list).Name, line_name(line_name_list).Difficult, line_name(line_name_list).RunTimeStartHour,
                                    line_name(line_name_list).RunTimeStartMinute, line_name(line_name_list).RunTimeEndHour, line_name(line_name_list).RunTimeEndMinute)

            line_name_list += 1
        Next


        '检索等待难易度
        Dim easy_gread As Integer = 4
        Do
            For a = 0 To 100
                If line_name(a).Name = "" Then
                    Exit For
                End If
                If line_name(a).Difficult = easy_gread Then
                    '检查这是白班还是夜班
                    If (line_name(a).RunTimeEndHour < line_name(a).RunTimeStartHour) Or (line_name(a).RunTimeEndHour = line_name(a).RunTimeStartHour And line_name(a).RunTimeEndMinute < line_name(a).RunTimeStartMinute) Then
                        '夜班
                        If (Hour(Now) > line_name(a).RunTimeStartHour Or (Hour(Now) = line_name(a).RunTimeStartHour And Minute(Now) >= line_name(a).RunTimeStartMinute)) Or
                        ((Hour(Now) < line_name(a).RunTimeEndHour) Or (Hour(Now) = line_name(a).RunTimeEndHour And Minute(Now) < line_name(a).RunTimeEndMinute)) Then
                            '正在运营
                            select_line = line_name(a)
                        End If
                    Else
                        '白班
                        If (Hour(Now) > line_name(a).RunTimeStartHour Or (Hour(Now) = line_name(a).RunTimeStartHour And Minute(Now) >= line_name(a).RunTimeStartMinute)) And
                        ((Hour(Now) < line_name(a).RunTimeEndHour) Or (Hour(Now) = line_name(a).RunTimeEndHour And Minute(Now) < line_name(a).RunTimeEndMinute)) Then
                            '正在运营
                            select_line = line_name(a)
                        End If
                    End If
                    Exit For
                End If


            Next

            If select_line.Name = "" Then
                If easy_gread = 0 Then
                    select_line = line_name(0)
                    Exit Do
                Else
                    easy_gread -= 1
                End If
            Else
                Exit Do
            End If
        Loop


        Return select_line
    End Function

    '从文字列表中选择一个合适的线路并返回mode:two
    Public Sub select_good_line_two(ByVal in_word As String, ByRef out_select_bus_1 As obj_short_rode_line, ByRef out_select_bus_2 As obj_short_rode_line)
        Dim ok As Boolean = False

        '存放地点，(序列，0=车1 1=车2)
        Dim line_name(100, 2) As obj_short_rode_line
        Dim line_name_list As Integer = 0
        Dim line_name_arr() = in_word.Split(" ")

        '写入模式，true表示写入第一个车 flase表示写入第二个车
        Dim write_mode As Boolean = True
        For a = 0 To line_name_arr.Count - 2
            If write_mode = True Then
                line_name(line_name_list, 0).Name = line_name_arr(a)
                '写入全部信息
                get_bus_msg(line_name(line_name_list, 0).Name, line_name(line_name_list, 0).Difficult, line_name(line_name_list, 0).RunTimeStartHour,
                                    line_name(line_name_list, 0).RunTimeStartMinute, line_name(line_name_list, 0).RunTimeEndHour, line_name(line_name_list, 0).RunTimeEndMinute)

            Else
                line_name(line_name_list, 1).Name = line_name_arr(a)
                '写入全部信息
                get_bus_msg(line_name(line_name_list, 1).Name, line_name(line_name_list, 1).Difficult, line_name(line_name_list, 1).RunTimeStartHour,
                                    line_name(line_name_list, 1).RunTimeStartMinute, line_name(line_name_list, 1).RunTimeEndHour, line_name(line_name_list, 1).RunTimeEndMinute)
                '增加序数
                line_name_list += 1
            End If
            '反向
            write_mode = Not (write_mode)
        Next


        '检索等待难易度
        Dim easy_gread As Integer = 4
        Do
            For a = 0 To 100
                If line_name(a, 0).Name = "" Then
                    Exit For
                End If
                If line_name(a, 0).Difficult >= easy_gread And line_name(a, 1).Difficult >= easy_gread Then
                    '检测运营时间

                    If (line_name(a, 0).RunTimeEndHour < line_name(a, 0).RunTimeStartHour) Or
                        (line_name(a, 0).RunTimeEndHour = line_name(a, 0).RunTimeStartHour And line_name(a, 0).RunTimeEndMinute < line_name(a, 0).RunTimeStartMinute) Then

                        '************************
                        '夜班
                        If (Hour(Now) > line_name(a, 0).RunTimeStartHour Or (Hour(Now) = line_name(a, 0).RunTimeStartHour And Minute(Now) >= line_name(a, 0).RunTimeStartMinute)) Or
                            ((Hour(Now) < line_name(a, 0).RunTimeEndHour) Or (Hour(Now) = line_name(a, 0).RunTimeEndHour And Minute(Now) < line_name(a, 0).RunTimeEndMinute)) Then
                            '第一辆车正在运营

                            '************************
                            If (line_name(a, 1).RunTimeEndHour < line_name(a, 1).RunTimeStartHour) Or
                                (line_name(a, 1).RunTimeEndHour = line_name(a, 1).RunTimeStartHour And line_name(a, 1).RunTimeEndMinute < line_name(a, 1).RunTimeStartMinute) Then
                                '夜班
                                If (Hour(Now) > line_name(a, 1).RunTimeStartHour Or (Hour(Now) = line_name(a, 1).RunTimeStartHour And Minute(Now) >= line_name(a, 1).RunTimeStartMinute)) Or
                                    ((Hour(Now) < line_name(a, 1).RunTimeEndHour) Or (Hour(Now) = line_name(a, 1).RunTimeEndHour And Minute(Now) < line_name(a, 1).RunTimeEndMinute)) Then
                                    '第二辆车正在运营
                                    out_select_bus_1 = line_name(a, 0)
                                    out_select_bus_2 = line_name(a, 1)

                                    '发送信号
                                    ok = True
                                End If
                            Else
                                '白班
                                '************************
                                If (Hour(Now) > line_name(a, 1).RunTimeStartHour Or (Hour(Now) = line_name(a, 1).RunTimeStartHour And Minute(Now) >= line_name(a, 1).RunTimeStartMinute)) And
                                    ((Hour(Now) < line_name(a, 1).RunTimeEndHour) Or (Hour(Now) = line_name(a, 1).RunTimeEndHour And Minute(Now) < line_name(a, 1).RunTimeEndMinute)) Then
                                    '第二辆车正在运营
                                    out_select_bus_1 = line_name(a, 0)
                                    out_select_bus_2 = line_name(a, 1)

                                    '发送信号
                                    ok = True
                                End If
                            End If
                        End If
                    Else
                        '白班
                        '************************
                        If (Hour(Now) > line_name(a, 0).RunTimeStartHour Or (Hour(Now) = line_name(a, 0).RunTimeStartHour And Minute(Now) >= line_name(a, 0).RunTimeStartMinute)) And
                            ((Hour(Now) < line_name(a, 0).RunTimeEndHour) Or (Hour(Now) = line_name(a, 0).RunTimeEndHour And Minute(Now) < line_name(a, 0).RunTimeEndMinute)) Then
                            '第一辆车正在运营

                            '************************
                            If (line_name(a, 1).RunTimeEndHour < line_name(a, 1).RunTimeStartHour) Or
                                (line_name(a, 1).RunTimeEndHour = line_name(a, 1).RunTimeStartHour And line_name(a, 1).RunTimeEndMinute < line_name(a, 1).RunTimeStartMinute) Then
                                '夜班
                                If (Hour(Now) > line_name(a, 1).RunTimeStartHour Or (Hour(Now) = line_name(a, 1).RunTimeStartHour And Minute(Now) >= line_name(a, 1).RunTimeStartMinute)) Or
                                    ((Hour(Now) < line_name(a, 1).RunTimeEndHour) Or (Hour(Now) = line_name(a, 1).RunTimeEndHour And Minute(Now) < line_name(a, 1).RunTimeEndMinute)) Then
                                    '第二辆车正在运营
                                    out_select_bus_1 = line_name(a, 0)
                                    out_select_bus_2 = line_name(a, 1)

                                    '发送信号
                                    ok = True
                                End If
                            Else
                                '白班
                                '************************
                                If (Hour(Now) > line_name(a, 1).RunTimeStartHour Or (Hour(Now) = line_name(a, 1).RunTimeStartHour And Minute(Now) >= line_name(a, 1).RunTimeStartMinute)) And
                                    ((Hour(Now) < line_name(a, 1).RunTimeEndHour) Or (Hour(Now) = line_name(a, 1).RunTimeEndHour And Minute(Now) < line_name(a, 1).RunTimeEndMinute)) Then
                                    '第二辆车正在运营
                                    out_select_bus_1 = line_name(a, 0)
                                    out_select_bus_2 = line_name(a, 1)

                                    '发送信号
                                    ok = True
                                End If
                            End If
                        End If
                    End If

                    Exit For
                End If
            Next

            If ok = False Then
                If easy_gread = 0 Then
                    out_select_bus_1 = line_name(0, 0)
                    out_select_bus_2 = line_name(0, 1)

                    Exit Do
                Else
                    easy_gread -= 1
                End If
            Else
                Exit Do
            End If
        Loop

    End Sub

    '返回某个线路在数组中的第二位置的索引，给出group_list,as go
    Public Function return_bus_name_as_arr_go(ByVal group_number As String, ByVal search_name As String)
        Dim list As Short = 0

        For a = 0 To 100
            If go_bus_name_list(group_number, a).Name = "" Then
                Exit For
            End If
            If go_bus_name_list(group_number, a).Name = search_name Then
                list = a
                Exit For
            End If
        Next

        Return list
    End Function
    '返回某个线路在数组中的第二位置的索引，给出group_list,as end
    Public Function return_bus_name_as_arr_end(ByVal group_number As String, ByVal search_name As String)
        Dim list As Integer = 0

        For a = 0 To 100
            If end_bus_name_list(group_number, a).Name = "" Then
                Exit For
            End If
            If end_bus_name_list(group_number, a).Name = search_name Then
                list = a
                Exit For
            End If
        Next

        Return list
    End Function

    '检索某辆车等待的难易度
    Public Function check_bus_wait_hard(ByVal bus_name As String)
        Dim gread As Integer = 0
        Dim file_line As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\bus.txt", System.Text.Encoding.UTF8)
        Dim word As String = ""

        Do
            word = file_line.ReadLine
            If word = "ENDDATE" Then
                Exit Do
            End If

            If word = bus_name Then
                '跳过无用的值
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                gread = Int(file_line.ReadLine)
                Exit Do

            End If
            '跳过无用的值
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()

            '刷完站台
            Do
                word = file_line.ReadLine

                If word = "END" Then
                    Exit Do
                End If

            Loop

        Loop
        file_line.Dispose()

        Return gread
    End Function

    '检索某辆车首末班车时间,返回字符串形式(00:00-00:00),数值存储在function_1-4中 1=start_hour 2=start_min 3=end_hour 4=end_min
    Public Function check_bus_on_time(ByVal bus_name As String)
        Dim time As String = ""
        Dim file_line As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\bus.txt", System.Text.Encoding.UTF8)
        Dim word As String = ""

        Do
            word = file_line.ReadLine
            If word = "ENDDATE" Then
                Exit Do
            End If

            If word = bus_name Then
                '跳过无用的值
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()

                function_result(1) = Int(file_line.ReadLine)
                function_result(2) = Int(file_line.ReadLine)
                function_result(3) = Int(file_line.ReadLine)
                function_result(4) = Int(file_line.ReadLine)

                time = function_result(1) & ":" & function_result(2) & "-" & function_result(3) & ":" & function_result(4)

                Exit Do

            End If
            '跳过无用的值
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()
            file_line.ReadLine()

            '刷完站台
            Do
                word = file_line.ReadLine

                If word = "END" Then
                    Exit Do
                End If

            Loop

        Loop
        file_line.Dispose()

        Return time
    End Function

    '*********************************************************************************************************************
    '分析result,找出如何换乘这些车
    Public Sub fenxi_result()
        Dim before_stop As String = stamp_start_stop

        For a = 0 To 16
            If result(a).Name = "" Then
                Exit For
            End If

            '写入名称
            end_result(a).Name = result(a).Name

            '写入交汇
            If a = 16 Or result(a + 1).Name = "" Then
                '没有下一项了
                end_result(a).TransferStop = stamp_end_stop
                end_result(a).Toword = search_good_tf_stop_b(before_stop, result(a).Name, stamp_end_stop)
            Else
                '正常，写入数据
                end_result(a).TransferStop = search_good_tf_stop_a(before_stop, result(a).Name, result(a + 1).Name)
                end_result(a).Toword = function_calc

                before_stop = end_result(a).TransferStop
            End If
        Next

    End Sub

    '给起始站，线路，换乘线路，寻找最好的交汇站点,在function_calc里写入换乘方向
    Public Function search_good_tf_stop_a(ByVal start_stop As String, ByVal start_line As String, ByVal end_line As String)
        Dim end_stop As String = ""

        Dim file_line As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\bus.txt", System.Text.Encoding.UTF8)
        Dim word As String = ""

        Dim before_stop As String = ""
        Dim before_stop_list As Integer = 0
        Dim last_stop As String = ""
        Dim last_stop_list As Integer = 0
        Dim this_stop_list As Integer = 0

        Dim zhantai_list As Integer = 0

        Do
            word = file_line.ReadLine
            If word = "ENDDATE" Then
                Exit Do
            End If

            If word = start_line Then
                '开始读取
                '跳过无用的值
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()

                '开始读取
                '之前的站台
                Do
                    word = file_line.ReadLine
                    If word = start_stop Then
                        '到达中央站台，跳出
                        Exit Do
                    End If
                    If stop_have_line(word, end_line) = True Then
                        '该站台可以换
                        before_stop = word
                        before_stop_list = zhantai_list
                    End If
                    '加循环
                    zhantai_list += 1
                Loop

                '写本站的信息
                this_stop_list = zhantai_list
                zhantai_list += 1
                '之后的站台
                Do
                    word = file_line.ReadLine
                    If word = "ENDLINE" Then
                        '结束，跳出
                        Exit Do
                    End If
                    If stop_have_line(word, end_line) = True Then
                        '该站台可以换,写完立马跳出
                        last_stop = word
                        last_stop_list = zhantai_list
                        Exit Do
                    End If
                    '加循环
                    zhantai_list += 1
                Loop

                '检索是否找到了必要的站台，没找到就在下行线路里找
                If before_stop = "" And last_stop = "" Then
                    this_stop_list = 0
                    before_stop = ""
                    before_stop_list = 0
                    last_stop = ""
                    last_stop_list = 0

                    '之前的站台
                    Do
                        word = file_line.ReadLine
                        If word = start_stop Then
                            '到达中央站台，跳出
                            Exit Do
                        End If
                        If stop_have_line(word, end_line) = True Then
                            '该站台可以换
                            before_stop = word
                            before_stop_list = zhantai_list
                        End If
                        '加循环
                        zhantai_list += 1
                    Loop

                    '写本站的信息
                    this_stop_list = zhantai_list
                    zhantai_list += 1
                    '之后的站台
                    Do
                        word = file_line.ReadLine
                        If word = "END" Then
                            '结束,error
                            Environment.Exit(7)
                        End If
                        If stop_have_line(word, end_line) = True Then
                            '该站台可以换,写完立马跳出
                            last_stop = word
                            last_stop_list = zhantai_list
                            Exit Do
                        End If
                        '加循环
                        zhantai_list += 1
                    Loop
                End If


                '分析
                If before_stop = "" Then
                        '没有前，取后
                        end_stop = last_stop
                        function_calc = search_bus_start_end_stop(1, start_line)
                    Else
                        If last_stop = "" Then
                            '没有后，取前
                            end_stop = before_stop
                            function_calc = search_bus_start_end_stop(0, start_line)
                        Else
                            '两者都有，比绝对值
                            If this_stop_list - before_stop_list >= last_stop_list - this_stop_list Then
                                '后站更近
                                end_stop = last_stop
                                function_calc = search_bus_start_end_stop(1, start_line)
                            Else
                                '前站更近
                                end_stop = before_stop
                                function_calc = search_bus_start_end_stop(0, start_line)
                            End If


                        End If
                    End If


                    '读完跳出
                    Exit Do
                Else
                    '不是的
                    '跳过无用的值
                    file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                '刷完站台
                Do
                    word = file_line.ReadLine

                    If word = "END" Then
                        Exit Do
                    End If

                Loop

            End If

        Loop

        file_line.Dispose()

        Return end_stop
    End Function
    '给起始站，线路，终到站，寻找方向
    Public Function search_good_tf_stop_b(ByVal start_stop As String, ByVal start_line As String, ByVal end_stop As String)
        Dim yes As String = ""

        Dim file_line As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\bus.txt", System.Text.Encoding.UTF8)
        Dim word As String = ""

        '指示是否需要在下行线路中重新读取
        Dim need_re_read As Boolean = False

        Do
            word = file_line.ReadLine
            If word = "ENDDATE" Then
                Exit Do
            End If

            If word = start_line Then
                '开始读取
                '跳过无用的值
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()

                '开始读取
                Do
                    word = file_line.ReadLine
                    If word = "ENDLINE" Then
                        Exit Do
                        need_re_read = True
                    End If

                    If word = start_stop Then
                        '先找到始发站，方向向终点站
                        yes = search_bus_start_end_stop(1, start_line)
                        Exit Do
                    End If
                    If word = end_stop Then
                        '先找到结束站，方向向起点站
                        yes = search_bus_start_end_stop(0, start_line)
                        Exit Do
                    End If

                Loop

                '检测是否需要重读
                If need_re_read = True Then
                    Do
                        word = file_line.ReadLine
                        If word = "END" Then
                            Environment.Exit(7)
                        End If

                        If word = start_stop Then
                            '先找到始发站，方向向终点站
                            yes = search_bus_start_end_stop(1, start_line)
                            Exit Do
                        End If
                        If word = end_stop Then
                            '先找到结束站，方向向起点站
                            yes = search_bus_start_end_stop(0, start_line)
                            Exit Do
                        End If

                    Loop
                End If

                '读完跳出
                Exit Do
            Else
                '不是的
                '跳过无用的值
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                '刷完站台
                Do
                    word = file_line.ReadLine

                    If word = "END" Then
                        Exit Do
                    End If

                Loop
            End If
        Loop

        file_line.Dispose()

        Return yes
    End Function

    '检索始发站或终到站，给予0搜始发，给予1搜终到
    Public Function search_bus_start_end_stop(ByVal mode As Integer, ByVal bus_name As String)
        Dim yes As String = ""

        Dim file_line As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\bus.txt", System.Text.Encoding.UTF8)
        Dim word As String = ""
        Dim before_word As String = ""

        Do
            word = file_line.ReadLine
            If word = "ENDDATE" Then
                Exit Do
            End If

            If word = bus_name Then
                '跳过无用的值
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()

                '索引
                Do
                    word = file_line.ReadLine
                    If word = "ENDLINE" Then
                        '结束
                        If mode = 1 Then
                            yes = before_word
                        End If
                        Exit Do
                    End If

                    If mode = 0 Then
                        yes = word
                        Exit Do
                    End If

                    before_word = word
                Loop

                '退出
                Exit Do

            Else
                '不是的
                '跳过无用的值
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                file_line.ReadLine()
                '刷完站台
                Do
                    word = file_line.ReadLine

                    If word = "END" Then
                        Exit Do
                    End If

                Loop
            End If
        Loop

        Return yes
    End Function

    '检测某站台里是否有某车次
    Public Function stop_have_line(ByVal stop_name As String, ByVal line_name As String)
        Dim yes As Boolean = False

        Dim file_stop As New System.IO.StreamReader(Environment.CurrentDirectory + "\library\short_stop.txt", System.Text.Encoding.UTF8)
        Dim word As String = ""

        Do
            word = file_stop.ReadLine
            If word = "" Then
                Exit Do
            End If

            '开始读取
            If word = stop_name Then
                Do
                    word = file_stop.ReadLine
                    If word = "END" Then
                        Exit Do
                    End If

                    '找到，退出
                    If word = line_name Then
                        yes = True
                        Exit Do
                    End If
                Loop

                '退出
                Exit Do
            Else
                '掠过
                Do
                    word = file_stop.ReadLine
                    If word = "END" Then
                        Exit Do
                    End If
                Loop
            End If

        Loop

        file_stop.Dispose()

        Return yes
    End Function
End Module


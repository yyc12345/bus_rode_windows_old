'引用
Imports System.Xml
Imports System.Net

Module xml_read

    ''' <summary>
    ''' [内核][xml_read]是否需要重新读取xml
    ''' </summary>
    Public re_read_xml As Boolean = False

    ''' <summary>
    ''' [内核][xml_read]读取xml的主函数
    ''' </summary>
    Public Sub read_xml_main()


        '读取xml文档
        Dim xml_root As New XmlDocument
        xml_root.Load(Environment.CurrentDirectory + "\get_short_rode.xml")

        '确认是否可读
        Dim can_read_1 As String = xml_root.SelectSingleNode("/DirectionBusTransResponse/status").InnerText
        Dim can_read_2 As String = xml_root.SelectSingleNode("/DirectionBusTransResponse/type").InnerText

        '不可以
        If can_read_1 = "0" Then
            If can_read_2 = "1" Then
                '模糊查询
                re_read_xml = True
                read_xml_vague(xml_root)

            Else
                '清晰查询
                re_read_xml = False
                read_xml_clear(xml_root)

            End If
        Else
            '调用浏览器
            System.Diagnostics.Process.Start("http://api.map.baidu.com/direction?origin=" & stamp_start_stop &
                                                        "&origin_region=" & set_address_part & "&destination_region=" & set_address_part & "&destination=" &
                                                         stamp_end_stop & "&mode=transit&region=" & set_address_part & "&output=html&scr=bus_rode")

            '强制退出
            re_read_xml = False
        End If


        '是否需要重读
        If re_read_xml = True Then
            '重读
            re_read_xml = False
            read_xml_main()
        End If


    End Sub

    ''' <summary>
    ''' [内核][xml_read]读取清晰的xml最短路径文件
    ''' </summary>
    ''' <param name="xml_file"></param>
    Public Sub read_xml_clear(ByRef xml_file As XmlDocument)

        '继续读
        Dim scheme_list As XmlNodeList = xml_file.SelectNodes("/DirectionBusTransResponse/result/scheme")
        Dim info_list As XmlNodeList


        '给对话框选定
        Dim aaa As New ui_depend_window_select_item_list
        ui_connect_window_select_item_list.Clear()
        ui_connect_window_select_item_list_title = read_resources_describe_into_memory("lang_code_xml_read_select_solution")

        '输入内容
        For a = 0 To scheme_list.Count - 1

		'TODO
            aaa.pro_title = "方案" & (a + 1)
            aaa.pro_text = "路程：" & scheme_list(a).SelectSingleNode("distance").InnerText & "米 需要时间：" & scheme_list(a).SelectSingleNode("duration").InnerText & "秒"
            aaa.pro_fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))

            ui_connect_window_select_item_list.Add(aaa)
            aaa = New ui_depend_window_select_item_list
        Next

        '显示对话框

        Dim linshi = New Window_select_item
        ui_connect_window_select_item_list_select_index = -1
        linshi.Owner = Application.Current.MainWindow()
        linshi.ShowDialog()

        '执行
        If ui_connect_window_select_item_list_select_index <> -1 Then

            info_list = scheme_list(ui_connect_window_select_item_list_select_index).SelectNodes("steps/info")

            For a = 0 To info_list.Count - 1
                If a = 21 Then
                    Exit For
                End If

                '检测是不是公交
                If info_list(a).SelectSingleNode("type").InnerText = "3" Then
                    '是的
                    Dim word As String = info_list(a).SelectSingleNode("stepInstruction").InnerText
                    word = Replace(word, "<font color=" & Chr(34) & "#313233" & Chr(34) & ">", "")
                    word = Replace(word, "<font color=" & Chr(34) & "#7a7c80" & Chr(34) & ">", "")
                    word = Replace(word, "</font>", "")

                    toword(a, 0) = word & vbCrLf & vbCrLf & "有关 " & info_list(a).SelectSingleNode("vehicle/name").InnerText &
                " 运营时间的信息：" & info_list(a).SelectSingleNode("vehicle/start_time").InnerText & "-" & info_list(a).SelectSingleNode("vehicle/end_time").InnerText
                Else
                    toword(a, 0) = info_list(a).SelectSingleNode("stepInstruction").InnerText
                End If
            Next

        Else
            '返回空值，读取第一项
            info_list = scheme_list(0).SelectNodes("steps/info")

            For a = 0 To info_list.Count - 1
                If a = 21 Then
                    Exit For
                End If

                '检测是不是公交
                If info_list(a).SelectSingleNode("type").InnerText = "3" Then
                    '是的
                    Dim word As String = info_list(a).SelectSingleNode("stepInstruction").InnerText
                    word = Replace(word, "<font color=" & Chr(34) & "#313233" & Chr(34) & ">", "")
                    word = Replace(word, "</font>", "")

                    toword(a, 0) = word & vbCrLf & vbCrLf & "有关 " & info_list(a).SelectSingleNode("vehicle/name").InnerText &
                    " 运营时间的信息：" & info_list(a).SelectSingleNode("vehicle/start_time").InnerText & "-" & info_list(a).SelectSingleNode("vehicle/end_time").InnerText
                Else
                    toword(a, 0) = info_list(a).SelectSingleNode("stepInstruction").InnerText
                End If
            Next

        End If

    End Sub

    ''' <summary>
    ''' [内核][xml_read]读取xml最短路径模糊查询的结果，让用户选择一个地点
    ''' </summary>
    ''' <param name="xml_file"></param>
    Public Sub read_xml_vague(ByRef xml_file As XmlDocument)

        '继续读
        Dim origin_list As XmlNodeList = xml_file.SelectNodes("/DirectionBusTransResponse/result/origin")
        Dim destination_list As XmlNodeList = xml_file.SelectNodes("/DirectionBusTransResponse/result/destination")
        Dim select_origin As String = ""
        Dim select_destination As String = ""

        '需要确认起点
        If origin_list.Count <> 0 Then
            '给对话框选定
            Dim aaa As New ui_depend_window_select_item_list
            ui_connect_window_select_item_list.Clear()
            ui_connect_window_select_item_list_title = read_resources_describe_into_memory("lang_code_xml_read_select_start")
            Dim content_list As XmlNodeList = origin_list(0).SelectNodes("content")

            '输入内容
            For a = 0 To content_list.Count - 1

                aaa.pro_title = content_list(a).SelectSingleNode("name").InnerText
                aaa.pro_text = "经度坐标：" & content_list(a).SelectSingleNode("location/lng").InnerText & " 纬度坐标：" & content_list(a).SelectSingleNode("location/lat").InnerText
                aaa.pro_fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))

                ui_connect_window_select_item_list.Add(aaa)
                aaa = New ui_depend_window_select_item_list
            Next

            '显示对话框

            Dim linshi = New Window_select_item
            ui_connect_window_select_item_list_select_index = -1
            linshi.Owner = Application.Current.MainWindow()
            linshi.ShowDialog()

            '执行
            If ui_connect_window_select_item_list_select_index <> -1 Then

                select_origin = content_list(ui_connect_window_select_item_list_select_index).SelectSingleNode("location/lat").InnerText & "," &
                                content_list(ui_connect_window_select_item_list_select_index).SelectSingleNode("location/lng").InnerText
            Else
                '返回空值，读取第一项

                select_origin = content_list(0).SelectSingleNode("location/lat").InnerText & "," & content_list(0).SelectSingleNode("location/lng").InnerText
            End If
        Else
            '无需更改
            select_origin = xml_file.SelectSingleNode("/DirectionBusTransResponse/result/originInfo/wd").InnerText
        End If

        '需要确认终点****************************************************
        If destination_list.Count <> 0 Then
            '给对话框选定
            Dim bbb As New ui_depend_window_select_item_list
            ui_connect_window_select_item_list.Clear()
            ui_connect_window_select_item_list_title = read_resources_describe_into_memory("lang_code_xml_read_select_end")
            Dim content_list As XmlNodeList = destination_list(0).SelectNodes("content")

            '输入内容
            For a = 0 To content_list.Count - 1

                bbb.pro_title = content_list(a).SelectSingleNode("name").InnerText
                bbb.pro_text = "经度坐标：" & content_list(a).SelectSingleNode("location/lng").InnerText & " 纬度坐标：" & content_list(a).SelectSingleNode("location/lat").InnerText
                bbb.pro_fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))

                ui_connect_window_select_item_list.Add(bbb)
                bbb = New ui_depend_window_select_item_list
            Next

            '显示对话框

            Dim linshi = New Window_select_item
            ui_connect_window_select_item_list_select_index = -1
            linshi.Owner = Application.Current.MainWindow()
            linshi.ShowDialog()

            '执行
            If ui_connect_window_select_item_list_select_index <> -1 Then

                select_destination = content_list(ui_connect_window_select_item_list_select_index).SelectSingleNode("location/lat").InnerText & "," &
                                content_list(ui_connect_window_select_item_list_select_index).SelectSingleNode("location/lng").InnerText
            Else
                '返回空值，读取第一项

                select_destination = content_list(0).SelectSingleNode("location/lat").InnerText & "," & content_list(0).SelectSingleNode("location/lng").InnerText
            End If
        Else
            '无需更改
            select_destination = xml_file.SelectSingleNode("/DirectionBusTransResponse/result/destinationInfo/wd").InnerText
        End If


        '重新发起一次读取*****************************************
        '可以执行，先下载文件
        Try
            Dim tf_client As WebClient = New WebClient()
            Dim tf_word As String = System.Text.Encoding.UTF8.GetString(tf_client.DownloadData("http://api.map.baidu.com/direction/v1?origin=" & select_origin & "&destination=" &
                                                                                                  select_destination & "&mode=transit&region=" & set_address_part & "&output=xml&ak=" & app_use_baidu_ak))

            Dim file As New System.IO.StreamWriter(Environment.CurrentDirectory + "\get_short_rode.xml", False, System.Text.Encoding.UTF8)
            file.Write(tf_word)
            file.Dispose()
        Catch ex As Exception
            '发生错误，调用浏览器
            System.Diagnostics.Process.Start("http://api.map.baidu.com/direction?origin=" & select_origin &
                                                        "&origin_region=" & set_address_part & "&destination_region=" & set_address_part & "&destination=" &
                                                         select_destination & "&mode=transit&region=" & set_address_part & "&output=html&scr=bus_rode")

            '强制不读
            re_read_xml = False

        End Try

    End Sub


End Module

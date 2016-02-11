Public Class MainWindow

#Region "文件框"

    ''' <summary>
    ''' 打开工程的打开框
    ''' </summary>
    Public open_brsp_file As New Microsoft.Win32.OpenFileDialog

    ''' <summary>
    ''' 打开txt的打开框
    ''' </summary>
    Public open_txt_file As New Microsoft.Win32.OpenFileDialog

#End Region

#Region "app级事件"

    ''' <summary>
    ''' 应用初始化
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub app_init(sender As Object, e As RoutedEventArgs)

        '初始操作
        read_desktop()
        re_work_area()

        open_brsp_file.Filter = "bus_rode资源开发工程文件|*.brsp"
        open_txt_file.Filter = "文本文档|*.txt"

    End Sub


#End Region

#Region "菜单"

    ''' <summary>
    ''' 创建工程
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_creat_project(sender As Object, e As RoutedEventArgs) Handles ui_menu_form_project_form_new.Click

        Dim linshi As New Window_new_project
        linshi.Owner = Me
        linshi.ShowDialog()

        If window_communication.window_new_project_ok_for_create = True Then
            '确认创建

            create_project()

        End If

    End Sub

    ''' <summary>
    ''' 打开工程
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_open_project(sender As Object, e As RoutedEventArgs) Handles ui_menu_form_project_form_open.Click

        open_project()

    End Sub

    ''' <summary>
    ''' 保存工程
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_save_project(sender As Object, e As RoutedEventArgs) Handles ui_menu_form_project_form_save.Click

        save_project()

    End Sub

    ''' <summary>
    ''' 关闭工程
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_close_project(sender As Object, e As RoutedEventArgs) Handles ui_menu_form_project_form_close.Click

        window_dialogs_show("关闭", "确认关闭工程吗？", 1, 2, False, "确定", "取消", Me)
        If window_dialogs_select_btn = 0 Then

            '先保存
            save_project()

            work_mode = 0
            build_mode = 0
            project_path = ""
            re_work_area()

            '清空station subway相关数据
            clear_data()

            Me.Title = "bus_rode资源开发器"


            '清空控件
            ui_form_line_form_line_list.Items.Clear()
            ui_form_station_form_station_list.Items.Clear()
            ui_form_subway_form_subway_list.Items.Clear()
            ui_form_subway_form_exit_list.Items.Clear()
            '清空输入区
            clear_line()

        End If

    End Sub

    ''' <summary>
    ''' 退出
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_exit_project(sender As Object, e As RoutedEventArgs) Handles ui_menu_form_project_form_exit.Click

        window_dialogs_show("退出", "确认退出吗？", 1, 2, False, "确定", "取消", Me)
        If window_dialogs_select_btn = 0 Then

            If work_mode = 1 Then
                '打开了工程，先要保存文件

                save_project()

            End If

            Environment.Exit(1)
        End If

    End Sub

    '=======================================================

    ''' <summary>
    ''' 预设
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_setup(sender As Object, e As RoutedEventArgs) Handles ui_menu_form_setup_form_setup.Click

        Dim linshi As New Window_before_set
        linshi.Owner = Me
        linshi.ShowDialog()

    End Sub

    '=======================================================

    ''' <summary>
    ''' 编译
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_build(sender As Object, e As RoutedEventArgs) Handles ui_menu_form_build_form_build.Click

        build_project()

    End Sub

    ''' <summary>
    ''' 打包
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_packups(sender As Object, e As RoutedEventArgs) Handles ui_menu_form_build_form_packups.Click

        packups_project()

    End Sub

    '=======================================================

    ''' <summary>
    ''' 关于
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_about(sender As Object, e As RoutedEventArgs) Handles ui_menu_form_about.Click

        window_dialogs_show("关于", "bus_rode资源开发器，适用于开发bus_rode 8000，内部版本：" & systems.app_build, 0, 1, False, "确定", "", Me)

    End Sub


#End Region

#Region "线路"

    ''' <summary>
    ''' 线路-添加线路
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_add(sender As Object, e As RoutedEventArgs)
        add_line()
    End Sub
    ''' <summary>
    ''' 线路-删除线路
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_del(sender As Object, e As RoutedEventArgs)
        If ui_form_line_form_line_list.SelectedIndex <> -1 Then
            del_line()
        End If

    End Sub
    ''' <summary>
    ''' 线路-重命名线路
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_rename(sender As Object, e As RoutedEventArgs)
        If ui_form_line_form_line_list.SelectedIndex <> -1 Then
            rename_line()
        End If

    End Sub
    ''' <summary>
    ''' 线路-搜索线路
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_search(sender As Object, e As RoutedEventArgs)
        '搜索
        If ui_form_line_form_search.Text <> "" Then
            If ui_form_line_form_line_list.Items.Contains(ui_form_line_form_search.Text) = True Then
                ui_form_line_form_line_list.SelectedIndex = ui_form_line_form_line_list.Items.IndexOf(ui_form_line_form_search.Text)
            Else
                window_dialogs_show("错误", "无法找到这个线路名", 2, 1, False, "确定", "", Me)
            End If
        End If
    End Sub

    '========================================
    ''' <summary>
    ''' 线路-选择新线路
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_init_line(sender As Object, e As SelectionChangedEventArgs) Handles ui_form_line_form_line_list.SelectionChanged

        If ui_form_line_form_line_list.SelectedIndex <> -1 Then
            read_line(ui_form_line_form_line_list.Items.Item(ui_form_line_form_line_list.SelectedIndex))
        End If

    End Sub

    '========================================
    ''' <summary>
    ''' 线路-保存线路
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_save(sender As Object, e As RoutedEventArgs)

        If ui_form_line_form_line_list.SelectedIndex <> -1 Then
            save_line()
        End If

    End Sub

    '========================================
    ''' <summary>
    ''' 线路-标准描述
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_form_normal_describe_normal(sender As Object, e As RoutedEventArgs)
        '预设

        ui_form_line_form_describe_form_describe_1.Text = desktop(0)
        ui_form_line_form_describe_form_describe_2.Text = desktop(1)
        ui_form_line_form_describe_form_describe_3.Text = desktop(2)
        ui_form_line_form_describe_form_describe_4.Text = desktop(3)
        ui_form_line_form_describe_form_describe_5.Text = desktop(4)

    End Sub

    '========================================
    ''' <summary>
    ''' 线路-导入txt
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_form_stop_enter_txt(sender As Object, e As RoutedEventArgs)

        open_txt_file.ShowDialog()
        If open_txt_file.FileName <> "" Then
            Dim file As New System.IO.StreamReader(open_txt_file.FileName, System.Text.Encoding.Default)
            If ui_form_line_form_stop_form_up_line_tab.IsSelected = True Then
                '上行
                ui_form_line_form_stop_form_up_line_word.Text = ui_form_line_form_stop_form_up_line_word.Text + file.ReadToEnd
            Else
                '下行
                ui_form_line_form_stop_form_down_line_word.Text = ui_form_line_form_stop_form_down_line_word.Text + file.ReadToEnd
            End If

            file.Dispose()
        End If

    End Sub
    ''' <summary>
    ''' 线路-清空
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_form_stop_clear(sender As Object, e As RoutedEventArgs)

        window_dialogs_show("清空", "确认清空?", 1, 2, False, "确定", "取消", Me)
        If window_dialogs_select_btn = 0 Then
            If ui_form_line_form_stop_form_up_line_tab.IsSelected = True Then
                '上行
                ui_form_line_form_stop_form_up_line_word.Text = ""
            Else
                '下行
                ui_form_line_form_stop_form_down_line_word.Text = ""
            End If
        End If

    End Sub
    ''' <summary>
    ''' 线路-反转
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_form_stop_rewrite_stop(sender As Object, e As RoutedEventArgs)

        If ui_form_line_form_stop_form_up_line_tab.IsSelected = True Then
            '上行
            If ui_form_line_form_stop_form_up_line_word.Text = "" Then
                window_dialogs_show("错误", "没有内容可反转", 2, 1, False, "确定", "", Me)
                Exit Sub
            End If
        Else
            '下行
            If ui_form_line_form_stop_form_down_line_word.Text = "" Then
                window_dialogs_show("错误", "没有内容可反转", 2, 1, False, "确定", "", Me)
                Exit Sub
            End If
        End If

        Dim word As String = ""
        If ui_form_line_form_stop_form_up_line_tab.IsSelected = True Then
            '上行
            word = ui_form_line_form_stop_form_up_line_word.Text
        Else
            '下行
            word = ui_form_line_form_stop_form_down_line_word.Text
        End If

        Dim word_arr() As String = word.Split(vbCrLf)
        Dim return_word As String = ""

        For a = word_arr.Count - 1 To 0 Step -1

            '清除
            word_arr(a) = Replace(word_arr(a), vbLf, "")

            '写入
            If return_word = "" Then
                return_word = word_arr(a)
            Else
                return_word = return_word & vbCrLf & word_arr(a)
            End If
        Next

        If ui_form_line_form_stop_form_up_line_tab.IsSelected = True Then
            '上行
            ui_form_line_form_stop_form_up_line_word.Text = return_word
        Else
            '下行
            ui_form_line_form_stop_form_down_line_word.Text = return_word
        End If


    End Sub
    ''' <summary>
    ''' 线路-替换为回车
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_form_stop_replace_to_enter(sender As Object, e As RoutedEventArgs)

        If ui_form_line_form_stop_form_replace_form_up_line.IsChecked = True Then
            ui_form_line_form_stop_form_up_line_word.Text = Replace(ui_form_line_form_stop_form_up_line_word.Text, ui_form_line_form_stop_form_replace_form_word.Text, vbCrLf)
        End If
        If ui_form_line_form_stop_form_replace_form_down_line.IsChecked = True Then
            ui_form_line_form_stop_form_down_line_word.Text = Replace(ui_form_line_form_stop_form_down_line_word.Text, ui_form_line_form_stop_form_replace_form_word.Text, vbCrLf)
        End If

    End Sub
    ''' <summary>
    ''' 线路-替换为其他文字
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_line_form_stop_replace_to_word(sender As Object, e As RoutedEventArgs)

        window_dialogs_show("替换字符", "输入要替换为的字符：", 0, 2, True, "确定", "取消", Me)
        If window_dialogs_select_btn = 0 Then
            Dim a = window_dialogs_input_text
            window_dialogs_show("替换字符", "是否将 " & ui_form_line_form_stop_form_replace_form_word.Text & " 替换为 " & a, 1, 2, False, "确定", "取消", Me)
            If window_dialogs_select_btn = 0 Then
                If ui_form_line_form_stop_form_replace_form_up_line.IsChecked = True Then
                    ui_form_line_form_stop_form_up_line_word.Text = Replace(ui_form_line_form_stop_form_up_line_word.Text, ui_form_line_form_stop_form_replace_form_word.Text, a)
                End If
                If ui_form_line_form_stop_form_replace_form_down_line.IsChecked = True Then
                    ui_form_line_form_stop_form_down_line_word.Text = Replace(ui_form_line_form_stop_form_down_line_word.Text, ui_form_line_form_stop_form_replace_form_word.Text, a)
                End If

            End If
        End If

    End Sub

#End Region

#Region "总站"
    ''' <summary>
    ''' 总站-搜索
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_station_search(sender As Object, e As RoutedEventArgs)
        '搜索
        If ui_form_station_form_search.Text <> "" Then
            If ui_form_station_form_station_list.Items.Contains(ui_form_station_form_search.Text) = True Then
                ui_form_station_form_station_list.SelectedIndex = ui_form_station_form_station_list.Items.IndexOf(ui_form_station_form_search)
            Else
                window_dialogs_show("错误", "找不到这个车站名", 2, 1, False, "确定", "", Me)
            End If
        End If
    End Sub
    ''' <summary>
    ''' 总站-选择总站
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_station_select_station(sender As Object, e As SelectionChangedEventArgs) Handles ui_form_station_form_station_list.SelectionChanged

        If ui_form_station_form_station_list.SelectedIndex <> -1 Then
            '读取总站
            ui_form_station_form_describe_form_describe_1.Text = back_station(ui_form_station_form_station_list.SelectedIndex, 1)
            ui_form_station_form_describe_form_describe_2.Text = back_station(ui_form_station_form_station_list.SelectedIndex, 2)
            ui_form_station_form_describe_form_describe_3.Text = back_station(ui_form_station_form_station_list.SelectedIndex, 3)
            ui_form_station_form_describe_form_describe_4.Text = back_station(ui_form_station_form_station_list.SelectedIndex, 4)

        End If

    End Sub
    ''' <summary>
    ''' 总站-保存数据
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_station_save(sender As Object, e As RoutedEventArgs)

        If ui_form_station_form_station_list.SelectedIndex <> -1 Then
            '保存总站
            back_station(ui_form_station_form_station_list.SelectedIndex, 1) = ui_form_station_form_describe_form_describe_1.Text
            back_station(ui_form_station_form_station_list.SelectedIndex, 2) = ui_form_station_form_describe_form_describe_2.Text
            back_station(ui_form_station_form_station_list.SelectedIndex, 3) = ui_form_station_form_describe_form_describe_3.Text
            back_station(ui_form_station_form_station_list.SelectedIndex, 4) = ui_form_station_form_describe_form_describe_4.Text
        End If

    End Sub
    ''' <summary>
    ''' 总站-常规模板
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_station_normal(sender As Object, e As RoutedEventArgs)
        ui_form_station_form_describe_form_describe_1.Text = desktop(5)
        ui_form_station_form_describe_form_describe_2.Text = desktop(6)
        ui_form_station_form_describe_form_describe_3.Text = desktop(7)
        ui_form_station_form_describe_form_describe_4.Text = desktop(8)
    End Sub

#End Region

#Region "地铁"
    ''' <summary>
    ''' 地铁-搜索
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_subway_search(sender As Object, e As RoutedEventArgs)
        '搜索
        If ui_form_subway_form_search.Text <> "" Then
            If ui_form_subway_form_subway_list.Items.Contains(ui_form_subway_form_search.Text) = True Then
                ui_form_subway_form_subway_list.SelectedIndex = ui_form_subway_form_subway_list.Items.IndexOf(ui_form_subway_form_search.Text)
            Else
                window_dialogs_show("错误", "找不到这个地铁站点名", 2, 1, False, "确定", "", Me)
            End If
        End If
    End Sub
    ''' <summary>
    ''' 地铁-保存
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_subway_save(sender As Object, e As RoutedEventArgs)

        If ui_form_subway_form_subway_list.SelectedIndex <> -1 Then

            '先检测
            For a = 0 To 20
                If back_exit(a, 0) = "" Then
                    Exit For
                Else
                    If back_exit(a, 1) = "" Then
                        '不合格
                        window_dialogs_show("错误", "出口描述不得为空！无法保存。请检查所有出口描述保证其描述不为空", 2, 1, False, "确定", "", Me)
                        Exit Sub
                    End If
                End If
            Next


            '保存地铁信息
            Dim word As String = ""

            For a = 0 To 20
                If back_exit(a, 0) = "" Then
                    Exit For
                Else
                    If word <> "" Then
                        word = word + vbCrLf
                    End If
                    word = word + back_exit(a, 0) + vbCrLf + back_exit(a, 1)
                End If
            Next

            back_subway(ui_form_subway_form_subway_list.SelectedIndex, 1) = word

        End If

    End Sub
    ''' <summary>
    ''' 地铁-选择新地铁
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_subway_select_subway(sender As Object, e As SelectionChangedEventArgs) Handles ui_form_subway_form_subway_list.SelectionChanged

        If ui_form_subway_form_subway_list.SelectedIndex <> -1 Then
            '先保存原本打开的地铁

            '保存地铁信息
            If before_subway_stop <> -1 Then

                Dim word As String = ""

                For a = 0 To 20
                    If back_exit(a, 0) = "" Then
                        Exit For
                    Else
                        If word <> "" Then
                            word = word + vbCrLf
                        End If
                        word = word + back_exit(a, 0) + vbCrLf + back_exit(a, 1)
                    End If
                Next

                back_subway(before_subway_stop, 1) = word

            End If

            before_subway_stop = ui_form_subway_form_subway_list.SelectedIndex
            '在执行打开新的

            Dim word2 As String = back_subway(ui_form_subway_form_subway_list.SelectedIndex, 1)
            Dim word2_arr() As String = word2.Split(vbCrLf)

            '清除
            For a = 0 To word2_arr.Count - 1
                word2_arr(a) = Replace(word2_arr(a), vbLf, "")
            Next
            ui_form_subway_form_exit_list.Items.Clear()
            ui_form_subway_form_exit_form_exit_word.Text = ""
            For f = 0 To 20
                For g = 0 To 1
                    back_exit(f, g) = ""
                Next
            Next

            '写入
            For a = 0 To word2_arr.Count - 2 Step 2
                back_exit(a / 2, 0) = word2_arr(a)
                ui_form_subway_form_exit_list.Items.Add(word2_arr(a))
                back_exit(a / 2, 1) = word2_arr(a + 1)
            Next

            '选择
            ui_form_subway_form_exit_list.SelectedIndex = 0
        End If


    End Sub

    '====================================================
    ''' <summary>
    ''' 地铁-出口-添加
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_subway_form_exit_add(sender As Object, e As RoutedEventArgs)

        Dim word As String = ""
        For a = 0 To 20
            If back_exit(a, 0) = "" Then

                '=================================
                window_dialogs_show("添加出口", "输入出口名:", 0, 2, True, "确定", "取消", Me)
                If window_dialogs_select_btn = 0 Then

                    word = window_dialogs_input_text
                    If ui_form_subway_form_exit_list.Items.Contains(word) = True Then
                        window_dialogs_show("错误", "这个出口名已经存在了", 2, 1, False, "确定", "", Me)
                    Else
                        If word <> "" Then
                            back_exit(a, 0) = word
                            ui_form_subway_form_exit_list.Items.Add(word)
                            ui_form_subway_form_exit_list.SelectedIndex = ui_form_subway_form_exit_list.Items.IndexOf(word)
                        Else
                            window_dialogs_show("错误", "出口名不得为空", 2, 1, False, "确定", "", Me)
                        End If

                    End If

                End If
                '===================================
                Exit For

            End If
            If a = 20 Then
                window_dialogs_show("错误", "无法添加出口了，因为最大出口量20个已经满了", 2, 1, False, "确定", "", Me)
            End If
        Next

    End Sub
    ''' <summary>
    ''' 地铁-出口-删除
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_subway_form_exit_del(sender As Object, e As RoutedEventArgs)

        If ui_form_subway_form_exit_list.Items.Count = 1 Then
            window_dialogs_show("错误", "不能删除该项，您必须保证有一个出口存在于该列表", 2, 1, False, "确定", "", Me)
        Else
            For a = 0 To 20
                If back_exit(a, 0) = ui_form_subway_form_exit_list.Items.Item(ui_form_subway_form_exit_list.SelectedIndex) Then

                    For b = a To 20
                        If b = 20 Then
                            back_exit(b, 0) = ""
                            back_exit(b, 1) = ""
                        Else
                            back_exit(b, 0) = back_exit(b + 1, 0)
                            back_exit(b, 1) = back_exit(b + 1, 1)
                        End If
                    Next
                    Exit For

                End If
            Next

            ui_form_subway_form_exit_list.Items.RemoveAt(ui_form_subway_form_exit_list.SelectedIndex)
            ui_form_subway_form_exit_list.SelectedIndex = 0

        End If

    End Sub
    ''' <summary>
    ''' 地铁-出口-重命名
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_subway_form_exit_rename(sender As Object, e As RoutedEventArgs)

        window_dialogs_show("重命名出口", "输入新的出口名:", 0, 2, True, "确定", "取消", Me)
        If window_dialogs_select_btn = 0 Then

            Dim word As String = window_dialogs_input_text

            If ui_form_subway_form_exit_list.Items.Contains(word) = True Then
                window_dialogs_show("错误", "该出口名已存在", 2, 1, False, "确定", "", Me)
            Else
                If word <> "" Then

                    '寻找旧的
                    Dim a As Integer = ui_form_subway_form_exit_list.SelectedIndex
                    back_exit(a, 0) = word
                    ui_form_subway_form_exit_list.Items.RemoveAt(a)
                    ui_form_subway_form_exit_list.Items.Insert(a, word)

                Else
                    window_dialogs_show("错误", "出口名不得为空", 2, 1, False, "确定", "", Me)
                End If

            End If

        End If


    End Sub
    ''' <summary>
    ''' 地铁-出口-选择出口
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_subway_form_exit_select_exit(sender As Object, e As SelectionChangedEventArgs) Handles ui_form_subway_form_exit_list.SelectionChanged

        If ui_form_subway_form_exit_list.SelectedIndex <> -1 Then
            back_exit(ui_form_subway_form_exit_list.SelectedIndex, 1) = ui_form_subway_form_exit_form_exit_word.Text
        End If

    End Sub



#End Region

#Region "杂项"

    ''' <summary>
    ''' readme保存
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub event_form_sundry_form_readme_save(sender As Object, e As RoutedEventArgs)
        save_readme()
    End Sub

#End Region


#Region "代理函数"

    ''' <summary>
    ''' 以当前工作模式刷新工作区
    ''' </summary>
    Public Sub re_work_area()

        If work_mode = 0 Then
            ui_menu_form_project_form_new.IsEnabled = True
            ui_menu_form_project_form_open.IsEnabled = True

            ui_work_area.IsEnabled = False
            ui_work_area.Opacity = 0
            ui_bk_word.Opacity = 1

            ui_menu_form_build.IsEnabled = False
            ui_menu_form_project_form_save.IsEnabled = False
            ui_menu_form_project_form_close.IsEnabled = False
        Else
            ui_menu_form_project_form_new.IsEnabled = False
            ui_menu_form_project_form_open.IsEnabled = False

            ui_work_area.IsEnabled = True
            ui_work_area.Opacity = 1
            ui_bk_word.Opacity = 0

            ui_menu_form_build.IsEnabled = True
            ui_menu_form_project_form_save.IsEnabled = True
            ui_menu_form_project_form_close.IsEnabled = True
            If build_mode = 0 Then
                ui_menu_form_build_form_packups.IsEnabled = False

                ui_form_station.IsEnabled = False
                ui_form_subway.IsEnabled = False
            Else
                ui_menu_form_build_form_packups.IsEnabled = True

                ui_form_station.IsEnabled = True
                ui_form_subway.IsEnabled = True
            End If
        End If

    End Sub


#End Region

#Region "读取线路和附属资源"

    ''' <summary>
    ''' 读取线路主文档
    ''' </summary>
    Public Sub read_line_name()
        Dim file As New System.IO.StreamReader(project_path + "have_bus.txt", System.Text.Encoding.UTF8)

        Dim word As String = ""
        Do
            word = file.ReadLine
            If word = "END" Then
                Exit Do
            End If

            ui_form_line_form_line_list.Items.Add(word)

        Loop

        file.Dispose()

    End Sub

    ''' <summary>
    ''' 读取线路数据
    ''' </summary>
    ''' <param name="name"></param>
    Public Sub read_line(ByVal name As String)

        Dim file As New System.IO.StreamReader(project_path + "bus.txt", System.Text.Encoding.UTF8)

        Dim word As String = ""
        Do
            word = file.ReadLine
            If word = "ENDDATE" Then
                window_dialogs_show("灾难性错误", "灾难性错误，程序即将关闭", 2, 1, False, "确定", "", Me)
                Environment.Exit(100)
            End If

            If word = name Then
                '找到
                '读取数据
                ui_form_line_form_basic_pro_form_line_name.Text = word

                ui_form_line_form_describe_form_describe_1.Text = file.ReadLine
                ui_form_line_form_describe_form_describe_2.Text = file.ReadLine
                ui_form_line_form_describe_form_describe_3.Text = file.ReadLine
                ui_form_line_form_describe_form_describe_4.Text = file.ReadLine
                ui_form_line_form_describe_form_describe_5.Text = file.ReadLine


                If Int(file.ReadLine) = 0 Then
                    ui_form_line_form_basic_pro_form_genre.SelectedIndex = 0
                Else
                    ui_form_line_form_basic_pro_form_genre.SelectedIndex = 1
                End If

                ui_form_line_form_basic_pro_form_easy.SelectedIndex = Int(file.ReadLine)


                ui_form_line_form_basic_pro_form_first_bus_hour.SelectedIndex = Int(file.ReadLine)
                ui_form_line_form_basic_pro_form_first_bus_minute.SelectedIndex = Int(file.ReadLine)
                ui_form_line_form_basic_pro_form_last_bus_hour.SelectedIndex = Int(file.ReadLine) - 12
                ui_form_line_form_basic_pro_form_last_bus_minute.SelectedIndex = Int(file.ReadLine)

                Dim all_text As String = ""
                Dim all_text_2 As String = ""

                Do
                    word = file.ReadLine
                    If word = "ENDLINE" Then
                        Exit Do
                    Else
                        If all_text <> "" Then
                            all_text = all_text + vbCrLf
                        End If
                    End If

                    all_text = all_text + word

                Loop

                Do
                    word = file.ReadLine
                    If word = "END" Then
                        Exit Do
                    Else
                        If all_text_2 <> "" Then
                            all_text_2 = all_text_2 + vbCrLf
                        End If
                    End If

                    all_text_2 = all_text_2 + word

                Loop

                ui_form_line_form_stop_form_up_line_word.Text = all_text
                ui_form_line_form_stop_form_down_line_word.Text = all_text_2

                '退出
                Exit Do
            End If
        Loop

        file.Dispose()

    End Sub

    ''' <summary>
    ''' 读取总站、地铁核心数据
    ''' </summary>
    Public Sub read_more_date()
        Dim list As Integer = 0
        Dim word As String = ""

        '=======================================station
        Dim file1 As New System.IO.StreamReader(project_path + "station.txt", System.Text.Encoding.UTF8)

        Do
            word = file1.ReadLine
            If word = "" Then
                Exit Do
            End If

            back_station(list, 0) = word
            '对控件输出
            ui_form_station_form_station_list.Items.Add(word)

            'jump
            file1.ReadLine()

            back_station(list, 1) = file1.ReadLine
            back_station(list, 2) = file1.ReadLine
            back_station(list, 3) = file1.ReadLine
            back_station(list, 4) = file1.ReadLine

            'jump
            file1.ReadLine()

            list += 1
        Loop

        file1.Dispose()

        list = 0
        word = ""
        '============================================================subway
        Dim file2 As New System.IO.StreamReader(project_path + "subway.txt", System.Text.Encoding.UTF8)

        Do
            word = file2.ReadLine
            If word = "" Then
                If ui_form_subway_form_subway_list.Items.Count = 0 Then
                    '封堵没有地铁的情况
                    ui_form_subway.IsEnabled = False
                End If
                Exit Do
            End If

            back_subway(list, 0) = word
            '对控件输出
            ui_form_subway_form_subway_list.Items.Add(word)
            Do
                word = file2.ReadLine
                If word = "END" Then
                    Exit Do
                Else
                    If back_subway(list, 1) <> "" Then
                        back_subway(list, 1) = back_subway(list, 1) + vbCrLf
                    End If
                End If

                back_subway(list, 1) = back_subway(list, 1) + word

            Loop

            list += 1
        Loop

        file2.Dispose()

        '设置起始项
        ui_form_station_form_station_list.SelectedIndex = 0
        If ui_form_subway_form_subway_list.Items.Count <> 0 Then
            ui_form_subway_form_subway_list.SelectedIndex = 0
        End If

    End Sub







    '读取杂项
    Public Sub read_another()
        If System.IO.File.Exists(project_path + "readme.txt") = True Then
            Dim file As New System.IO.StreamReader(project_path + "readme.txt", System.Text.Encoding.UTF8)
            Dim word As String = file.ReadLine

            Dim word_arr() As String = word.Split("-")

            If word_arr.Count = 3 Then
                ui_form_sundry_form_readme_form_english_path_form_country.Text = word_arr(0)
                ui_form_sundry_form_readme_form_english_path_form_state.Text = word_arr(1)
                ui_form_sundry_form_readme_form_english_path_form_city.Text = word_arr(2)
            End If

            word = file.ReadLine
            Dim word_arr_2() As String = word.Split("-")

            If word_arr_2.Count = 3 Then
                ui_form_sundry_form_readme_form_local_path_form_country.Text = word_arr_2(0)
                ui_form_sundry_form_readme_form_local_path_form_state.Text = word_arr_2(1)
                ui_form_sundry_form_readme_form_local_path_form_city.Text = word_arr_2(2)
            End If

            ui_form_sundry_form_readme_form_developer.Text = file.ReadLine
            ui_form_sundry_form_readme_form_date.Text = file.ReadLine

            file.Dispose()
        End If

    End Sub


#End Region

#Region "工程相关"

    ''' <summary>
    ''' 创建工程
    ''' </summary>
    Public Sub create_project()

        '创建工程文件
        Dim file As New System.IO.StreamWriter(window_new_project_project_path & window_new_project_project_name + ".brsp", False, System.Text.Encoding.UTF8)
        '写入创建的版本
        file.WriteLine(systems.app_build)
        '写入项目名
        file.WriteLine(window_new_project_project_name)
        '写入工程路径
        file.WriteLine(window_new_project_project_path)
        file.Dispose()

        '创建资源文件
        file = New System.IO.StreamWriter(window_new_project_project_path & "readme.txt", False, System.Text.Encoding.UTF8)
        file.WriteLine("")
        file.Dispose()

        If window_new_project_use_old_file = True And System.IO.File.Exists(window_new_project_bus_path) = True And
            System.IO.File.Exists(window_new_project_have_bus_path) = True Then
            '有预设
            System.IO.File.Delete(window_new_project_project_path & "bus.txt")
            System.IO.File.Copy(window_new_project_bus_path, window_new_project_project_path & "bus.txt")

            System.IO.File.Delete(window_new_project_project_path & "have_bus.txt")
            System.IO.File.Copy(window_new_project_have_bus_path, window_new_project_project_path & "have_bus.txt")
        Else
            '没有预设
            file = New System.IO.StreamWriter(window_new_project_project_path & "bus.txt", False, System.Text.Encoding.UTF8)
            file.WriteLine("ENDDATE")
            file.Dispose()

            file = New System.IO.StreamWriter(window_new_project_project_path & "have_bus.txt", False, System.Text.Encoding.UTF8)
            file.WriteLine("END")
            file.Dispose()
        End If

        '预设
        project_path = window_new_project_project_path
        work_mode = 1
        build_mode = 0
        re_work_area()
        If window_new_project_use_old_file = True And System.IO.File.Exists(window_new_project_bus_path) = True And
            System.IO.File.Exists(window_new_project_have_bus_path) = True Then

            '读取数据
            read_line_name()
            read_another()
        End If

        '设置控件
        Me.Title = Me.Title + " --- " + window_new_project_project_name

    End Sub

    ''' <summary>
    ''' 打开工程
    ''' </summary>
    Public Sub open_project()

        open_brsp_file.ShowDialog()
        If open_brsp_file.FileName <> "" Then
            '设置标题
            Dim file As New System.IO.StreamReader(open_brsp_file.FileName, System.Text.Encoding.UTF8)
            Dim linshi As String = file.ReadLine
            If linshi = systems.app_build Then

                Me.Title = Me.Title + " --- " + file.ReadLine
                project_path = file.ReadLine

                work_mode = 1
                If check_build() = True Then
                    build_mode = 1
                    re_work_area()
                    read_more_date()
                Else
                    re_work_area()
                End If

                '读取数据
                read_line_name()
                read_another()

            Else
                '版本不对

                window_dialogs_show("无法打开该工程", "该工程所使用的bus_rode开发工具版本号与您现在使用的版本号不同，打开该工程需要的版本号：" & linshi, 2, 1, False, "确定", "", Me)

            End If
            file.Dispose()

        End If

    End Sub

    ''' <summary>
    ''' 保存工程
    ''' </summary>
    Public Sub save_project()
        '

        If build_mode = 1 Then
            '写station
            Dim file As New System.IO.StreamWriter(project_path + "station.txt", False, System.Text.Encoding.UTF8)

            For a = 0 To 500
                If back_station(a, 0) = "" Then
                    Exit For
                End If

                file.WriteLine(back_station(a, 0))
                file.WriteLine(back_station(a, 1))
                file.WriteLine(back_station(a, 2))
                file.WriteLine(back_station(a, 3))
                file.WriteLine(back_station(a, 4))
                file.WriteLine("END")
            Next

            file.Dispose()

            'subway
            Dim file2 As New System.IO.StreamWriter(project_path + "subway.txt", False, System.Text.Encoding.UTF8)

            If ui_form_subway_form_subway_list.Items.Count = 0 Then
                '取消写入
            Else
                For b = 0 To 1000
                    If back_station(b, 0) = "" Then
                        Exit For
                    End If

                    file2.WriteLine(back_subway(b, 0))
                    file2.WriteLine(back_subway(b, 1))
                    file2.WriteLine("END")
                Next
            End If

            file2.Dispose()
        End If

        '写have_bus
        Dim file_2 As New System.IO.StreamWriter(project_path + "have_bus.txt", False, System.Text.Encoding.UTF8)

        Dim word As String = ""
        Dim list As Integer = 0
        Do
            If ui_form_line_form_line_list.Items.Count = list Then
                file_2.WriteLine("END")
                Exit Do
            End If
            word = ui_form_line_form_line_list.Items.Item(list)
            file_2.WriteLine(word)

            list += 1
        Loop

        file_2.Dispose()

    End Sub

    ''' <summary>
    ''' 编译工程
    ''' </summary>
    Public Sub build_project()

        If ui_form_line_form_line_list.Items.Count = 0 Then
            '错误

            window_dialogs_show("无法编译", "无法编译该工程，因为不存在任何需要编译的内容", 2, 1, False, "确定", "", Me)

        Else
            '写have_bus
            Dim file_2 As New System.IO.StreamWriter(project_path + "have_bus.txt", False, System.Text.Encoding.UTF8)

            Dim word As String = ""
            Dim list As Integer = 0
            Do
                If ui_form_line_form_line_list.Items.Count = list Then
                    file_2.WriteLine("END")
                    Exit Do
                End If
                word = ui_form_line_form_line_list.Items.Item(list)
                file_2.WriteLine(word)

                list += 1
            Loop

            file_2.Dispose()

            '移动到目录下编译
            System.IO.File.Delete(Environment.CurrentDirectory & "\bus.txt")
            System.IO.File.Copy(project_path + "bus.txt", Environment.CurrentDirectory & "\bus.txt")

            System.IO.File.Delete(Environment.CurrentDirectory & "\stop.txt")
            System.IO.File.Delete(Environment.CurrentDirectory & "\station.txt")
            System.IO.File.Delete(Environment.CurrentDirectory & "\subway.txt")

            '检测现有的station
            If System.IO.File.Exists(project_path + "station.txt") Then
                System.IO.File.Copy(project_path + "station.txt", Environment.CurrentDirectory & "\station.txt")
            End If
            '检测现有的subway
            If System.IO.File.Exists(project_path + "subway.txt") Then
                System.IO.File.Copy(project_path + "subway.txt", Environment.CurrentDirectory & "\subway.txt")
            End If

            '编译
            bus_rode_develop_tools.bus_rode_build_bus_to_station.start()
            bus_rode_develop_tools.bus_rode_build_bus_to_stop.start()
            bus_rode_develop_tools.bus_rode_build_bus_to_subway.start()

            '移动
            System.IO.File.Delete(project_path + "subway.txt")
            System.IO.File.Copy(Environment.CurrentDirectory & "\subway.txt", project_path + "subway.txt")
            System.IO.File.Delete(project_path + "station.txt")
            System.IO.File.Copy(Environment.CurrentDirectory & "\station.txt", project_path + "station.txt")
            System.IO.File.Delete(project_path + "stop.txt")
            System.IO.File.Copy(Environment.CurrentDirectory & "\stop.txt", project_path + "stop.txt")

            window_dialogs_show("编译完毕", "成功编译该工程", 0, 1, False, "确定", "", Me)

            build_mode = 1
            re_work_area()
            ui_form_station_form_station_list.Items.Clear()
            ui_form_subway_form_subway_list.Items.Clear()
            ui_form_subway_form_exit_list.Items.Clear()
            read_more_date()
        End If

    End Sub

    ''' <summary>
    ''' 打包工程
    ''' </summary>
    Public Sub packups_project()

        '重新生成一遍，然后打包
        window_dialogs_show("打包", "我们将先再次编译一遍该工程，然后再打包", 0, 1, False, "确定", "", Me)

        build_project()

        System.IO.File.Delete(project_path + "release.brs")
        bus_rode_compression.bus_rode_compression.Compress(Mid(project_path, 1, project_path.Length - 1), project_path + "release.brs")
        window_dialogs_show("打包", "打包完毕", 0, 1, False, "确定", "", Me)

    End Sub

#End Region

#Region "线路、总站、地铁写入函数"

    ''' <summary>
    ''' 添加线路
    ''' </summary>
    Public Sub add_line()

        window_dialogs_show("添加线路", "输入线路名称:", 0, 2, True, "确定", "取消", Me)

        If window_dialogs_select_btn = 1 Then
            Exit Sub
        End If

        Dim word As String = window_dialogs_input_text
        If ui_form_line_form_line_list.Items.Contains(word) = True Or word = "" Then
            window_dialogs_show("错误", "名称为空或者已存在该线路", 2, 1, False, "确定", "", Me)
        Else
            ui_form_line_form_line_list.Items.Add(word)

            Dim file_new As New System.IO.StreamWriter(project_path + "bus.txt.new", False, System.Text.Encoding.UTF8)
            Dim file As New System.IO.StreamReader(project_path + "bus.txt", System.Text.Encoding.UTF8)

            '写入头部
            file_new.WriteLine(word)
            file_new.WriteLine("")
            file_new.WriteLine("")
            file_new.WriteLine("")
            file_new.WriteLine("")
            file_new.WriteLine("")

            file_new.WriteLine("0")
            file_new.WriteLine("2")

            file_new.WriteLine("0")
            file_new.WriteLine("0")
            file_new.WriteLine("13")
            file_new.WriteLine("0")

            file_new.WriteLine("ENDLINE")
            file_new.WriteLine("END")

            '复制文件
            Dim wordd As String = ""
            Do
                wordd = file.ReadLine
                If wordd = "ENDDATE" Then
                    file_new.WriteLine("ENDDATE")
                    Exit Do
                End If

                '写入
                file_new.WriteLine(wordd)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)

                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)

                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                Do
                    wordd = file.ReadLine
                    If wordd = "END" Then
                        file_new.WriteLine("END")
                        Exit Do
                    End If

                    file_new.WriteLine(wordd)
                Loop
            Loop


            file.Dispose()
            file_new.Dispose()

            System.IO.File.Delete(project_path + "bus.txt")
            System.IO.File.Move(project_path + "bus.txt.new", project_path + "bus.txt")
        End If

    End Sub

    ''' <summary>
    ''' 删除线路
    ''' </summary>
    Public Sub del_line()

        Dim word As String = ui_form_line_form_line_list.Items.Item(ui_form_line_form_line_list.SelectedIndex)
        If ui_form_line_form_line_list.Items.Contains(word) = True Then
            ui_form_line_form_line_list.Items.Remove(word)

            Dim file_new As New System.IO.StreamWriter(project_path + "bus.txt.new", False, System.Text.Encoding.UTF8)
            Dim file As New System.IO.StreamReader(project_path + "bus.txt", System.Text.Encoding.UTF8)

            '复制文件
            Dim wordd As String = ""
            Do
                wordd = file.ReadLine
                If wordd = "ENDDATE" Then
                    file_new.WriteLine("ENDDATE")
                    Exit Do
                End If

                '检测到要删除的项目
                If wordd = word Then
                    '读空
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
                    Do
                        wordd = file.ReadLine
                        If wordd = "END" Then
                            Exit Do
                        End If
                    Loop
                Else
                    '不是的->写入
                    file_new.WriteLine(wordd)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)

                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)

                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    Do
                        wordd = file.ReadLine
                        If wordd = "END" Then
                            file_new.WriteLine("END")
                            Exit Do
                        End If

                        file_new.WriteLine(wordd)
                    Loop
                End If


            Loop


            file.Dispose()
            file_new.Dispose()

            System.IO.File.Delete(project_path + "bus.txt")
            System.IO.File.Move(project_path + "bus.txt.new", project_path + "bus.txt")

            '设置控件
            clear_line()

            ui_form_line_form_line_list.SelectedIndex = -1

        Else
            window_dialogs_show("应用错误", "无法在列表中找到要删除的线路", 2, 1, False, "确定", "", Me)
        End If

    End Sub

    ''' <summary>
    ''' 重命名线路
    ''' </summary>
    Public Sub rename_line()

        window_dialogs_show("重命名线路", "输入新名称:", 0, 2, True, "确定", "取消", Me)
        If window_dialogs_select_btn = 1 Then
            Exit Sub
        End If

        Dim word As String = window_dialogs_input_text
        If ui_form_line_form_line_list.Items.Contains(word) = False And word <> "" And
            ui_form_line_form_line_list.Items.Item(ui_form_line_form_line_list.SelectedIndex) <> "" Then


            Dim file_new As New System.IO.StreamWriter(project_path + "bus.txt.new", False, System.Text.Encoding.UTF8)
            Dim file As New System.IO.StreamReader(project_path + "bus.txt", System.Text.Encoding.UTF8)

            '写入头部
            file_new.WriteLine(word)

            file_new.WriteLine(ui_form_line_form_describe_form_describe_1.Text)
            file_new.WriteLine(ui_form_line_form_describe_form_describe_2.Text)
            file_new.WriteLine(ui_form_line_form_describe_form_describe_3.Text)
            file_new.WriteLine(ui_form_line_form_describe_form_describe_4.Text)
            file_new.WriteLine(ui_form_line_form_describe_form_describe_5.Text)

            file_new.WriteLine(ui_form_line_form_basic_pro_form_genre.SelectedIndex)
            file_new.WriteLine(ui_form_line_form_basic_pro_form_easy.SelectedIndex)

            file_new.WriteLine(ui_form_line_form_basic_pro_form_first_bus_hour.SelectedIndex)
            file_new.WriteLine(ui_form_line_form_basic_pro_form_first_bus_minute.SelectedIndex)
            file_new.WriteLine(ui_form_line_form_basic_pro_form_last_bus_hour.SelectedIndex + 12)
            file_new.WriteLine(ui_form_line_form_basic_pro_form_last_bus_minute.SelectedIndex)

            file_new.WriteLine(ui_form_line_form_stop_form_up_line_word)
            '不换行
            file_new.WriteLine("ENDLINE")

            If ui_form_line_form_stop_form_down_line_word.Text <> "" Then
                file_new.WriteLine(ui_form_line_form_stop_form_down_line_word.Text)

            End If
            '不换行
            file_new.WriteLine("END")

            '复制文件
            Dim wordd As String = ""
            Do
                wordd = file.ReadLine
                If wordd = "ENDDATE" Then
                    file_new.WriteLine("ENDDATE")
                    Exit Do
                End If

                '检测到要删除的项目
                If wordd = ui_form_line_form_line_list.Items.Item(ui_form_line_form_line_list.SelectedIndex) Then
                    '读空
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
                    Do
                        wordd = file.ReadLine
                        If wordd = "END" Then
                            Exit Do
                        End If
                    Loop
                Else
                    '不是的->写入
                    file_new.WriteLine(wordd)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)

                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)

                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    file_new.WriteLine(file.ReadLine)
                    Do
                        wordd = file.ReadLine
                        If wordd = "END" Then
                            file_new.WriteLine("END")
                            Exit Do
                        End If

                        file_new.WriteLine(wordd)
                    Loop
                End If
            Loop


            file.Dispose()
            file_new.Dispose()

            System.IO.File.Delete(project_path + "bus.txt")
            System.IO.File.Move(project_path + "bus.txt.new", project_path + "bus.txt")


            ui_form_line_form_line_list.Items.RemoveAt(ui_form_line_form_line_list.SelectedIndex)
            ui_form_line_form_line_list.Items.Add(word)
            ui_form_line_form_line_list.SelectedIndex = ui_form_line_form_line_list.Items.IndexOf(word)
        Else
            window_dialogs_show("错误", "列表含有该名称或者该名称为空", 2, 1, False, "确定", "", Me)
        End If

    End Sub

    ''' <summary>
    ''' 清空线路界面右侧面板数据
    ''' </summary>
    Public Sub clear_line()

        ui_form_line_form_basic_pro_form_line_name.Text = ""
        ui_form_line_form_basic_pro_form_genre.SelectedIndex = 0
        ui_form_line_form_basic_pro_form_easy.SelectedIndex = 2
        ui_form_line_form_basic_pro_form_first_bus_hour.SelectedIndex = 0
        ui_form_line_form_basic_pro_form_first_bus_minute.SelectedIndex = 0
        ui_form_line_form_basic_pro_form_last_bus_hour.SelectedIndex = 0
        ui_form_line_form_basic_pro_form_last_bus_minute.SelectedIndex = 0

        ui_form_line_form_describe_form_describe_1.Text = ""
        ui_form_line_form_describe_form_describe_2.Text = ""
        ui_form_line_form_describe_form_describe_3.Text = ""
        ui_form_line_form_describe_form_describe_4.Text = ""
        ui_form_line_form_describe_form_describe_5.Text = ""

        ui_form_line_form_stop_form_up_line_word.Text = ""
        ui_form_line_form_stop_form_down_line_word.Text = ""

    End Sub

    ''' <summary>
    ''' 保存线路
    ''' </summary>
    Public Sub save_line()

        Dim file_new As New System.IO.StreamWriter(project_path + "bus.txt.new", False, System.Text.Encoding.UTF8)
        Dim file As New System.IO.StreamReader(project_path + "bus.txt", System.Text.Encoding.UTF8)

        '写入头部
        file_new.WriteLine(ui_form_line_form_basic_pro_form_line_name.Text)

        file_new.WriteLine(ui_form_line_form_describe_form_describe_1.Text)
        file_new.WriteLine(ui_form_line_form_describe_form_describe_2.Text)
        file_new.WriteLine(ui_form_line_form_describe_form_describe_3.Text)
        file_new.WriteLine(ui_form_line_form_describe_form_describe_4.Text)
        file_new.WriteLine(ui_form_line_form_describe_form_describe_5.Text)

        file_new.WriteLine(ui_form_line_form_basic_pro_form_genre.SelectedIndex)
        file_new.WriteLine(ui_form_line_form_basic_pro_form_easy.SelectedIndex)

        file_new.WriteLine(ui_form_line_form_basic_pro_form_first_bus_hour.SelectedIndex)
        file_new.WriteLine(ui_form_line_form_basic_pro_form_first_bus_minute.SelectedIndex)
        file_new.WriteLine(ui_form_line_form_basic_pro_form_last_bus_hour.SelectedIndex + 12)
        file_new.WriteLine(ui_form_line_form_basic_pro_form_last_bus_minute.SelectedIndex)

        file_new.WriteLine(ui_form_line_form_stop_form_up_line_word)
        '不换行
        file_new.WriteLine("ENDLINE")

        If ui_form_line_form_stop_form_down_line_word.Text <> "" Then
            file_new.WriteLine(ui_form_line_form_stop_form_down_line_word.Text)

        End If
        '不换行
        file_new.WriteLine("END")

        '复制文件
        Dim wordd As String = ""
        Do
            wordd = file.ReadLine
            If wordd = "ENDDATE" Then
                file_new.WriteLine("ENDDATE")
                Exit Do
            End If

            '检测到要删除的项目
            If wordd = ui_form_line_form_basic_pro_form_line_name.Text Then
                '读空
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
                Do
                    wordd = file.ReadLine
                    If wordd = "END" Then
                        Exit Do
                    End If
                Loop
            Else
                '不是的->写入
                file_new.WriteLine(wordd)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)

                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)

                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                Do
                    wordd = file.ReadLine
                    If wordd = "END" Then
                        file_new.WriteLine("END")
                        Exit Do
                    End If

                    file_new.WriteLine(wordd)
                Loop
            End If
        Loop


        file.Dispose()
        file_new.Dispose()

        System.IO.File.Delete(project_path + "bus.txt")
        System.IO.File.Move(project_path + "bus.txt.new", project_path + "bus.txt")


    End Sub


#End Region

#Region "附属资源写入"

    ''' <summary>
    ''' 保存readme.txt
    ''' </summary>
    Public Sub save_readme()

        Dim file As New System.IO.StreamWriter(project_path + "readme.txt", False, System.Text.Encoding.UTF8)
        file.WriteLine(ui_form_sundry_form_readme_form_english_path_form_country.Text & "-" & ui_form_sundry_form_readme_form_english_path_form_state.Text & "-" & ui_form_sundry_form_readme_form_english_path_form_city.Text)
        file.WriteLine(ui_form_sundry_form_readme_form_local_path_form_country.Text & "-" & ui_form_sundry_form_readme_form_local_path_form_state.Text & "-" & ui_form_sundry_form_readme_form_local_path_form_city.Text)
        file.WriteLine(ui_form_sundry_form_readme_form_developer.Text)
        file.WriteLine(ui_form_sundry_form_readme_form_date.Text)
        file.Dispose()

    End Sub

#End Region



End Class

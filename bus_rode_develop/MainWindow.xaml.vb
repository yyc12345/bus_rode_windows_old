Public Class MainWindow

#Region "文件框"

    ''' <summary>
    ''' 打开工程的打开框
    ''' </summary>
    Public open_brsp_file As New Microsoft.Win32.OpenFileDialog

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


#End Region

#Region "线路"

#End Region

#Region "总站"

#End Region

#Region "地铁"

#End Region

#Region "杂项"

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

                'Form1.TabPage3.Parent = Nothing
                'Form1.TabPage4.Parent = Nothing
            Else
                ui_menu_form_build_form_packups.IsEnabled = True

                'Form1.TabPage3.Parent = Form1.TabControl1
                'Form1.TabPage4.Parent = Form1.TabControl1
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
                MsgBox("灾难性错误", 16)
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

                window_dialogs_show("无法打开该工程", "该工程所使用的bus_rode开发工具版本号与您现在使用的版本号不同，打开该工程需要的版本号：" & linshi， 2， 1， False, "确定", "", Me)

            End If
            file.Dispose()

        End If

    End Sub


#End Region



End Class

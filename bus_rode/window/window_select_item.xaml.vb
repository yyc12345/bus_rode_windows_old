Public Class Window_select_item

    ''' <summary>
    ''' 窗口加载
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub window_select_item_load(sender As Object, e As RoutedEventArgs)
        '对列表的模板进行设置************************************************************************************************
        If ui_connect_window_select_item_list_is_single = True Then
            ui_window_select_item_list.ItemTemplate = CType(Application.Current.Resources("ui_temp_window_select_item_list_single"), DataTemplate)
        Else
            ui_window_select_item_list.ItemTemplate = CType(Application.Current.Resources("ui_temp_window_select_item_list_multiple"), DataTemplate)
        End If

        ui_window_select_item_list.ItemsSource = ui_connect_window_select_item_list
        ui_title.Text = ui_connect_window_select_item_list_title

    End Sub

    ''' <summary>
    ''' 选择的项变化
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub item_change(sender As Object, e As SelectionChangedEventArgs) Handles ui_window_select_item_list.SelectionChanged

        If ui_window_select_item_list.SelectedIndex = -1 Then Exit Sub

        '记住选中的数字
        Dim remember_select_index As Integer = ui_window_select_item_list.SelectedIndex

        If ui_connect_window_select_item_list_is_single = True Then

            '单选处理

            '处理数据
            Dim title As String = ""
                Dim word As String = ""
                Dim linshi As ui_depend_window_select_item_list = New ui_depend_window_select_item_list
                ui_window_select_item_list.ItemsSource = Nothing
                For a = 0 To ui_connect_window_select_item_list.Count - 1

                    '当前是选中项且选中项不是选中状态
                    If a = remember_select_index And ui_connect_window_select_item_list.Item(a).pro_is_select = False Then
                        '替换为选中

                        linshi = New ui_depend_window_select_item_list

                        title = ui_connect_window_select_item_list.Item(a).pro_title
                        word = ui_connect_window_select_item_list.Item(a).pro_text
                        ui_connect_window_select_item_list.RemoveAt(a)
                        linshi.pro_title = title
                        linshi.pro_text = word

                        '选中，填充画刷
                        linshi.pro_fill = New SolidColorBrush(Color.FromArgb(255, 30, 144, 255))
                        linshi.pro_is_select = True
                        linshi.pro_opacity = 1

                        ui_connect_window_select_item_list.Insert(a, linshi)

                    Else
                        '当前不是选中项但是是选中状态
                        If ui_connect_window_select_item_list.Item(a).pro_is_select = True Then
                            '替换为未选

                            linshi = New ui_depend_window_select_item_list

                            title = ui_connect_window_select_item_list.Item(a).pro_title
                            word = ui_connect_window_select_item_list.Item(a).pro_text
                            ui_connect_window_select_item_list.RemoveAt(a)
                            linshi.pro_title = title
                            linshi.pro_text = word


                            '未选中，填充画刷
                            linshi.pro_fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                            linshi.pro_is_select = False
                            linshi.pro_opacity = 0

                            ui_connect_window_select_item_list.Insert(a, linshi)

                        End If

                    End If

                Next

            ui_window_select_item_list.ItemsSource = ui_connect_window_select_item_list

        Else

            '多选处理

            '处理数据
            Dim title As String = ""
                Dim word As String = ""
                Dim linshi As ui_depend_window_select_item_list = New ui_depend_window_select_item_list
                ui_window_select_item_list.ItemsSource = Nothing
                For a = 0 To ui_connect_window_select_item_list.Count - 1
                    If a = remember_select_index Then
                        '选中，修改之

                        linshi = New ui_depend_window_select_item_list

                        title = ui_connect_window_select_item_list.Item(a).pro_title
                    word = ui_connect_window_select_item_list.Item(a).pro_text
                    linshi.pro_title = title
                        linshi.pro_text = word

                    If ui_connect_window_select_item_list.Item(a).pro_is_select = True Then
                        '修改为未选中
                        linshi.pro_fill = New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
                        linshi.pro_is_select = False
                        linshi.pro_opacity = 0
                    Else
                        '修改为选中
                        linshi.pro_fill = New SolidColorBrush(Color.FromArgb(255, 30, 144, 255))
                        linshi.pro_is_select = True
                        linshi.pro_opacity = 1
                    End If

                    ui_connect_window_select_item_list.RemoveAt(a)
                    ui_connect_window_select_item_list.Insert(a, linshi)
                    End If
                Next

            ui_window_select_item_list.ItemsSource = ui_connect_window_select_item_list


        End If


    End Sub

    ''' <summary>
    ''' 窗口移动
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub window_move(sender As Object, e As MouseButtonEventArgs)

        ReleaseCapture()
        SendMessage(New Interop.WindowInteropHelper(window_select_item_name).Handle, &HA1, 2, 0)

    End Sub

    ''' <summary>
    ''' 确认
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param> 
    Private Sub mouse_enter_ok(sender As Object, e As RoutedEventArgs) Handles ui_btn.Click

        'If ui_window_select_item_list.SelectedIndex <> -1 Then

        If ui_connect_window_select_item_list_is_single = True Then
            'single
            ui_connect_window_select_item_list_select_index = -1
            ui_connect_windows_select_item_list_select_index_group = ""

            For a = 0 To ui_connect_window_select_item_list.Count - 1
                If ui_connect_window_select_item_list.Item(a).pro_is_select = True Then
                    ui_connect_window_select_item_list_select_index = a
                    Exit For
                End If
            Next

        Else
            'multiple
            ui_connect_window_select_item_list_select_index = 0
            ui_connect_windows_select_item_list_select_index_group = ""

            For a = 0 To ui_connect_window_select_item_list.Count - 1
                If ui_connect_window_select_item_list.Item(a).pro_is_select = True Then

                    ui_connect_window_select_item_list_select_index += 1
                    If ui_connect_windows_select_item_list_select_index_group = "" Then
                        ui_connect_windows_select_item_list_select_index_group = a
                    Else
                        ui_connect_windows_select_item_list_select_index_group &= "," & a
                    End If

                End If
            Next

            If ui_connect_windows_select_item_list_select_index_group = "" Then ui_connect_window_select_item_list_select_index = -1

        End If

        Me.Close()
        'Else
        '    '没有选中任何项
        '    window_dialogs_show(read_resources_describe_into_memory("lang_global_error"), read_resources_describe_into_memory("lang_code_window_select_item_nothing"), 1,
        '                        False, read_resources_describe_into_memory("lang_global_ok"), "")
        'End If

    End Sub

End Class

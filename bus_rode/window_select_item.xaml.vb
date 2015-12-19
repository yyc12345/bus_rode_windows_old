Public Class Window_select_item
    ''' <summary>
    ''' 窗口加载
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub window_select_item_load(sender As Object, e As RoutedEventArgs)
        '对列表的模板进行设置************************************************************************************************
        ui_window_select_item_list.ItemTemplate = CType(Application.Current.Resources("ui_temp_window_select_item_list"), DataTemplate)
        ui_window_select_item_list.ItemsSource = ui_connect_window_select_item_list
        Me.Title = ui_connect_window_select_item_list_title

    End Sub

    ''' <summary>
    ''' 选择的项变化
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub item_change(sender As Object, e As SelectionChangedEventArgs) Handles ui_window_select_item_list.SelectionChanged
        is_selected_item_title.text = ui_connect_window_select_item_list.Item(ui_window_select_item_list.SelectedIndex).pro_title
        is_selected_item_text.Text = ui_connect_window_select_item_list.Item(ui_window_select_item_list.SelectedIndex).pro_text
    End Sub

    ''' <summary>
    ''' 确认
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub mouse_enter_ok(sender As Object, e As MouseButtonEventArgs)
        If ui_window_select_item_list.SelectedIndex <> -1 Then
            ui_connect_window_select_item_list_select_index = ui_window_select_item_list.SelectedIndex
            Me.Close()
        Else
            MsgBox("你没有选择任何项", 16, "错误")
        End If

    End Sub
End Class

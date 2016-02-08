Public Class Window_new_project

#Region "文件选择框"

    ''' <summary>
    ''' 保存项目的保存框
    ''' </summary>
    Public save_peoject_file As New Microsoft.Win32.SaveFileDialog


    ''' <summary>
    ''' 打开bus.txt的打开框
    ''' </summary>
    Public open_bus_file As New Microsoft.Win32.OpenFileDialog


    ''' <summary>
    ''' 打开have_bus.txt的打开框
    ''' </summary>
    Public open_have_bus_file As New Microsoft.Win32.OpenFileDialog

#End Region


    ''' <summary>
    ''' 创建工程
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub creat_project(sender As Object, e As RoutedEventArgs)

        '检测
        If ui_project_name.Text <> "" And ui_project_path.Text <> "" Then
            If ui_use_old_file.IsChecked = True Then
                If ui_bus_path.Text <> "" And ui_have_bus_path.Text <> "" Then

                Else
                    '错误
                    window_dialogs_show("错误", "您必须填写已有的bus.txt和have_bus.txt地址"， 2， 1， False, "确定", "", Me)

                    Exit Sub
                End If
            Else

            End If
        Else
            '错误
            window_dialogs_show("错误", "您必须填写工程名和工程保存地址"， 2， 1， False, "确定", "", Me)

            Exit Sub
        End If

        '确认创建
        window_new_project_ok_for_create = True
        window_new_project_project_name = ui_project_name.Text
        window_new_project_project_path = ui_project_path.Text
        window_new_project_use_old_file = ui_use_old_file.IsChecked
        window_new_project_bus_path = ui_bus_path.Text
        window_new_project_have_bus_path = ui_have_bus_path.Text

        Me.Close()

    End Sub

    ''' <summary>
    ''' 窗口开启
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub window_open(sender As Object, e As RoutedEventArgs)

        window_new_project_clear()

        save_peoject_file.Filter = "工程文件|Project.brsp"
        open_bus_file.Filter = "bus.txt文件|bus.txt"
        open_have_bus_file.Filter = "have_bus.txt文件|have_bus.txt"

    End Sub


#Region "选择路径"

    ''' <summary>
    ''' 选择工程路径
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub select_project_path(sender As Object, e As RoutedEventArgs)

        save_peoject_file.ShowDialog()
        If save_peoject_file.FileName <> "" Then
            '截取字符串
            Dim word_arr() As String = save_peoject_file.FileName.Split("\")
            ui_project_path.Text = Replace(save_peoject_file.FileName, word_arr(word_arr.Count - 1), "")
        End If

    End Sub
    ''' <summary>
    ''' 选择bus.txt路径
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub select_bus_path(sender As Object, e As RoutedEventArgs)

        open_bus_file.ShowDialog()
        If open_bus_file.FileName <> "" Then
            ui_bus_path.Text = open_bus_file.FileName
        End If

    End Sub
    ''' <summary>
    ''' 选择have_bus.txt路径
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub select_have_bus_path(sender As Object, e As RoutedEventArgs)

        open_have_bus_file.ShowDialog()
        If open_have_bus_file.FileName <> "" Then
            ui_have_bus_path.Text = open_have_bus_file.FileName
        End If

    End Sub

#End Region

End Class

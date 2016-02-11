Public Class Window_before_set

    ''' <summary>
    ''' 窗口打开
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub window_open(sender As Object, e As RoutedEventArgs)

        read_desktop()

        ui_bus_describe_1.Text = desktop(0)
        ui_bus_describe_2.Text = desktop(1)
        ui_bus_describe_3.Text = desktop(2)
        ui_bus_describe_4.Text = desktop(3)
        ui_bus_describe_5.Text = desktop(4)

        ui_station_describe_1.Text = desktop(5)
        ui_station_describe_2.Text = desktop(6)
        ui_station_describe_3.Text = desktop(7)
        ui_station_describe_4.Text = desktop(8)

    End Sub

    ''' <summary>
    ''' 保存预设
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub window_close(sender As Object, e As RoutedEventArgs)

        desktop(0) = ui_bus_describe_1.Text
        desktop(1) = ui_bus_describe_2.Text
        desktop(2) = ui_bus_describe_3.Text
        desktop(3) = ui_bus_describe_4.Text
        desktop(4) = ui_bus_describe_5.Text

        desktop(5) = ui_station_describe_1.Text
        desktop(6) = ui_station_describe_2.Text
        desktop(7) = ui_station_describe_3.Text
        desktop(8) = ui_station_describe_4.Text

        save_desktop()

        Me.Close()

    End Sub

End Class

'引用
Imports System.Xml
Imports System.Net

Public Class MainWindow
#Region "***************************************************************定义区*******************************************************************************"

    ''' <summary>
    ''' [系统][ui]向指定窗口的消息循环队列中发送消息
    ''' </summary>
    ''' <param name="hwnd">指定窗口的句柄</param>
    ''' <param name="wMsg">发送内容</param>
    ''' <param name="wParam">附加1</param>
    ''' <param name="lParam">附加2</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Declare Function SendMessage Lib "user32" Alias "SendMessageA" (ByVal hwnd As Integer, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer

    ''' <summary>
    ''' [系统][ui]取消鼠标按下状态
    ''' </summary>
    ''' <remarks></remarks>
    Private Declare Sub ReleaseCapture Lib "User32" ()

    ''' <summary>
    ''' [ui]动画所用的动画模板
    ''' </summary>
    ''' <remarks></remarks>
    Public storyboard_action As New System.Windows.Media.Animation.Storyboard

    ''' <summary>
    ''' [内核]说明程序是否以无资源状态启动
    ''' </summary>
    ''' <remarks></remarks>
    Public no_resource As Boolean = False

    ''' <summary>
    ''' [系统]标题栏时间的计时器
    ''' </summary>
    ''' <remarks></remarks>
    Public date_timer As New Windows.Threading.DispatcherTimer

    ''' <summary>
    ''' [系统]获取实时资源的计时器
    ''' </summary>
    ''' <remarks></remarks>
    Public get_resource_timer As New Windows.Threading.DispatcherTimer

    ''' <summary>
    ''' 打开资源文件的打开框
    ''' </summary>
    Public open_resources_file As New Microsoft.Win32.OpenFileDialog

    ''' <summary>
    ''' 打开背景文件的打开框
    ''' </summary>
    Public open_background_file As New Microsoft.Win32.OpenFileDialog

    ''' <summary>
    ''' 打开插件文件的打开框
    ''' </summary>
    Public open_mod_file As New Microsoft.Win32.OpenFileDialog

    ''' <summary>
    ''' 托盘图标
    ''' </summary>
    Public app_desktop_icon = New System.Windows.Forms.NotifyIcon

    ''' <summary>
    ''' 获取实时资源的线程
    ''' </summary>
    Public connect_dll_get_resources_td As New System.Threading.Thread(AddressOf connect_dll_get_resources)

#End Region

    '***************************************************************函数区*******************************************************************************
#Region "应用程序级函数"

    ''' <summary>
    ''' [系统][ui]退出程序的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub window_exit(sender As Object, e As MouseButtonEventArgs)
        Environment.Exit(0)
    End Sub

    ''' <summary>
    ''' [系统][ui]最大化/还原程序窗口的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub window_max(sender As Object, e As MouseButtonEventArgs) Handles ui_title_max_btn.MouseDown
        If Me.WindowState = Windows.WindowState.Maximized Then
            Me.WindowState = Windows.WindowState.Normal
        Else
            Me.WindowState = Windows.WindowState.Maximized
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]把程序最小化到任务栏的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub window_behind(sender As Object, e As MouseButtonEventArgs)
        Me.WindowState = Windows.WindowState.Minimized
    End Sub

    ''' <summary>
    ''' [系统][ui]移动程序窗口的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub window_move(sender As Object, e As MouseButtonEventArgs)
        ReleaseCapture()
        SendMessage(New Interop.WindowInteropHelper(ui_bus_rode_main_window).Handle, &HA1, 2, 0)
    End Sub

    ''' <summary>
    ''' [系统][ui]程序关闭的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub app_shutdown(sender As Object, e As EventArgs) Handles ui_bus_rode_main_window.Closed
        '结束所有线程
        '退出
        Environment.Exit(0)

    End Sub

    ''' <summary>
    ''' 窗体变化大小，更改窗体中元素的位置
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub form_size_change(sender As Object, e As SizeChangedEventArgs) Handles ui_bus_rode_main_window.SizeChanged
        '写入新值
        window_x = e.NewSize.Width
        window_y = e.NewSize.Height

        '停止播放
        storyboard_action.Stop()

        re_window()

        '检测窗口是否最大化的状态
        If ui_bus_rode_main_window.WindowState = WindowState.Maximized Then
            Dim a As New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/photo/min.png")))
            ui_title_max_btn.Background = a
        Else
            Dim a As New ImageBrush(New BitmapImage(New Uri("pack://application:,,,/photo/max.png")))
            ui_title_max_btn.Background = a
        End If


    End Sub

#End Region

#Region "计时器"
    ''' <summary>
    ''' [系统]标题栏时间的计时器刷新函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub date_timer_function(ByVal sender As Object, ByVal e As EventArgs)
        If Minute(Now) < 10 Then
            ui_title_time.Text = Hour(Now()) & ":0" & Minute(Now())
        Else
            ui_title_time.Text = Hour(Now()) & ":" & Minute(Now())
        End If

    End Sub

    ''' <summary>
    ''' [系统]获取实时资源的计时器刷新函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Public Sub get_resource_timer_function(ByVal sender As Object, ByVal e As EventArgs)

        '刷新车辆状态

        ui_form_line_have_stop_list.ItemsSource = Nothing
        ui_form_line_form_re_mid_bus_width.Width = GridLength.Auto

        read_mid_bus()

        ui_form_line_form_re_mid_bus_width.Width = New GridLength(0)
        If up_or_down_line = True Then
            ui_connet_core_form_line_have_sotp_list_show = ui_connet_core_form_line_have_sotp_list_up
        Else
            ui_connet_core_form_line_have_sotp_list_show = ui_connet_core_form_line_have_sotp_list_down
        End If
        ui_form_line_have_stop_list.ItemsSource = ui_connet_core_form_line_have_sotp_list_show

    End Sub


#End Region

    ''' <summary>
    ''' [系统]应用初始化的处理
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub app_start(sender As Object, e As RoutedEventArgs) Handles ui_bus_rode_main_window.Loaded
        '***********************************************************正式启动*********************************************************************************

        '检测环境
        '定义变量确定检测结果
        Dim linshi1 As Boolean = False
        Dim linshi2 As Boolean = False
        Dim linshi3 As Boolean = False
        If Int(System.Environment.OSVersion.Version.Major) >= 6 Then
            If Int(System.Environment.OSVersion.Version.Minor) >= 1 Then

            Else
                linshi1 = True
            End If
        Else
            linshi1 = True
        End If

        ''反360模块
        'For Each process As Process In process.GetProcesses()
        '    If process.ProcessName = "360sd" Then
        '        linshi2 = True
        '    End If
        '    If process.ProcessName = "360tray" Then
        '        linshi3 = True
        '    End If
        'Next

        '最终判断
        If (linshi1 = False) And (linshi2 = False) And (linshi3 = False) Then

        Else
            '错误，退出
            MsgBox("很抱歉,bus_rode无法运行在当前环境下,这可能由于下列情况:" + vbCrLf +
                    "1.操作系统版本过低,我们需要Windows 7(NT 6.1)及以上操作系统" + vbCrLf +
                    "您可以访问 https://insider.windows.com/ 来获取最新的操作系统,这将保障程序正常的运行" + vbCrLf +
                    "2.检测到非法进程,对于此种情况,我们建议您卸载可能导致程序无法启动的应用程序" + vbCrLf + vbCrLf +
                    "程序即将退出,我们深感抱歉." + vbCrLf +
                    "CHMOSGroup Programmer", 48, "拒绝启动")
            Environment.Exit(3)
        End If

        '初始化托盘
        app_desktop_icon.Visible = True
        app_desktop_icon.Icon = New System.Drawing.Icon("bus_rode_icon.dat")
        app_desktop_icon.Text = app_name

        '按操作系统修改ui
        If Environment.OSVersion.Version.Major > 6 Or (Environment.OSVersion.Version.Major = 6 And Environment.OSVersion.Version.Minor >= 2) Then
            'win8以上
            ui_title_msg_column_1.Width = New GridLength(0)
            ui_title_msg_column_2.Width = New GridLength(0)
            ui_title_msg_column_3.Width = New GridLength(0)
        End If

        '检测文件
        Dim linshi_word As String = ""
        If bus_rode_check.bus_rode_check.check_file(Environment.CurrentDirectory + "\", linshi_word) = False Then
            MsgBox(linshi_word, 16, "缺少启动文件")
            Environment.Exit(4)
        End If

        If bus_rode_check.bus_rode_check.have_file(Environment.CurrentDirectory + "\library\") = 4 Then
            message_ex_ex("错误", "没有找到资源文件，您应该先安装资源文件！你可以在程序的设置中安装资源")
            ui_form_message_clear.Opacity = 1
            ui_form_message_up_line.Opacity = 1
            ui_form_message_no_msg_title.Opacity = 0
            ui_title_msg_count.Text = Int(ui_title_msg_count.Text) + 1

            no_resource = True
        End If

        '确定程序能否更好地工作
        If System.Environment.Is64BitProcess = False And System.Environment.Is64BitOperatingSystem = True Then
            message_ex_ex("程序可以更好地工作", "你的操作系统是64Bit,你可以使用64Bit的bus_rode")
            ui_form_message_clear.Opacity = 1
            ui_form_message_up_line.Opacity = 1
            ui_form_message_no_msg_title.Opacity = 0
            ui_window_title.Text = ui_window_title.Text + "(32Bit)"
        End If

        '接受命令行
        If Command() <> "" Then
            Dim font_list As Integer = 1
            Do
                If Mid(Command, font_list, 1) = "" Then
                    Exit Do
                End If
                Try
                    If Mid(Command, font_list, 1) = " " Then
                        ui_bus_rode_main_window.Width = Int(Mid(Command, 1, font_list - 1))
                        ui_bus_rode_main_window.Height = Int(Mid(Command, font_list + 1))

                        Exit Do
                    End If
                Catch ex As Exception
                    '命令行不合要求,直接取消
                    Exit Do
                End Try

                font_list += 1
            Loop
        End If

        '尝试加载背景
        If System.IO.File.Exists(Environment.CurrentDirectory + "\background.jpg") Then
            '有背景
            Dim bk As New ImageBrush(New BitmapImage(New Uri(Environment.CurrentDirectory + "\background.jpg")))
            ui_bus_rode_main_window.Background = bk
        End If


        '刷新时间，启动时间计时器
        If Minute(Now) < 10 Then
            ui_title_time.Text = Hour(Now()) & ":0" & Minute(Now())
        Else
            ui_title_time.Text = Hour(Now()) & ":" & Minute(Now())
        End If

        date_timer = New Windows.Threading.DispatcherTimer
        date_timer.Interval = TimeSpan.FromSeconds(5)
        AddHandler date_timer.Tick, AddressOf date_timer_function
        date_timer.Start()

        If no_resource = False Then
            '启动获取资源的计时器
            get_resource_timer = New Windows.Threading.DispatcherTimer
            get_resource_timer.Interval = TimeSpan.FromSeconds(10)
            AddHandler get_resource_timer.Tick, AddressOf get_resource_timer_function
            get_resource_timer.Start()


            '启动最短路径线程
            main_calc_td.Start()
            '启动讲述人线程
            talk_man_main_td.Start()

            '加载文件
            start()
            ui_form_line_have_stop_list.ItemsSource = Nothing
            ui_form_line_subway_list.ItemsSource = Nothing
            add_system()
            '设置默认上行
            ui_connet_core_form_line_have_sotp_list_show = ui_connet_core_form_line_have_sotp_list_up
            ui_form_line_have_stop_list.ItemsSource = ui_connet_core_form_line_have_sotp_list_show
            ui_form_line_subway_list.ItemsSource = ui_connet_core_form_line_subway_list

            '修改计时器状态
            '设置获取实时资源的线程状态
            If System.IO.File.Exists(Environment.CurrentDirectory + "\bus_rode_mod.dll") = True Then
                connect_dll_get_resources_td.Start()
            End If
            If get_bus_addr = False Then
                get_resource_timer.Stop()
                connect_dll_get_resources_always_stop = True
            End If

            '设置文本
            If bus_or_subway = 0 Then
                '公交
                ui_form_line_describe.Text = "当前车次：" + bus + vbCrLf + "类别：公交" + vbCrLf + bus_msg(0) + vbCrLf + bus_msg(1) + vbCrLf + bus_msg(2) + vbCrLf + bus_msg(3) + vbCrLf + bus_msg(4) + vbCrLf + bus_run + vbCrLf + "等待该车的难度:" + bus_wait_hard
            Else
                '地铁
                ui_form_line_describe.Text = "当前车次：" + bus + vbCrLf + "类别：地铁" + vbCrLf + bus_msg(0) + vbCrLf + bus_msg(1) + vbCrLf + bus_msg(2) + vbCrLf + bus_msg(3) + vbCrLf + bus_msg(4) + vbCrLf + bus_run + vbCrLf + "等待该车的难度:" + bus_wait_hard
            End If

            ui_form_stop_describe.Text = cross_stop
            ui_form_stop_middle_stop_name.Text = bus_stop_stop(now_stop)

            '关联控件

            '初始化line
            ui_form_line_list.ItemsSource = ui_connet_core_form_line_list
            ui_form_line_stop_search_list.ItemsSource = ui_connet_core_form_line_search_stop_list

            '初始化stop
            If ui_connet_core_form_stop_list.Count > 500 Then
                '太多了不显示，若显示会炸内存
                ui_form_stop_list.ItemsSource = Nothing
                ui_form_stop_contorl_full_list.Opacity = 1
            Else
                ui_form_stop_list.ItemsSource = ui_connet_core_form_stop_list
            End If

            ui_form_stop_left_cross_line_list.ItemsSource = ui_connet_core_form_stop_left_cross_line_list
            ui_form_stop_right_cross_line_list.ItemsSource = ui_connet_core_form_stop_right_cross_line_list

        End If

        '加载无关紧要的设置
        add_setting()
        '设置颜色 1-3透明df=223 4透明8f=143 5-8透明6f=111
        ui_color_1.Color = Color.FromArgb(223, form_color.R, form_color.G, form_color.B)
        ui_color_2.Color = Color.FromArgb(223, form_color.R, form_color.G, form_color.B)
        ui_color_3.Color = Color.FromArgb(223, form_color.R, form_color.G, form_color.B)

        ui_color_4.Color = Color.FromArgb(143, form_color.R, form_color.G, form_color.B)

        ui_color_5.Color = Color.FromArgb(111, form_color.R, form_color.G, form_color.B)
        ui_color_6.Color = Color.FromArgb(111, form_color.R, form_color.G, form_color.B)
        ui_color_7.Color = Color.FromArgb(111, form_color.R, form_color.G, form_color.B)
        ui_color_8.Color = Color.FromArgb(111, form_color.R, form_color.G, form_color.B)

        ui_form_line_select_up_line.Color = Color.FromArgb(143, form_color.R, form_color.G, form_color.B)
        ui_form_line_select_down_line.Color = Color.FromArgb(0, 0, 0, 0)

        If get_bus_addr = True Then
            ui_form_contorl_check_background_color_1.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
            ui_form_contorl_check_background_color_2.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
            ui_form_contorl_check_background_color_3.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
        End If
        If talk_man = True Then
            ui_form_contorl_check_background_color_2_1.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
            ui_form_contorl_check_background_color_2_2.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
            ui_form_contorl_check_background_color_2_3.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
        End If

        ui_form_contorl_r_value.Text = form_color.R
        ui_form_contorl_g_value.Text = form_color.G
        ui_form_contorl_b_value.Text = form_color.B


        '设置界面文本
        Dim describe As String = ""
        describe = "Programmer : Wiliam Tad" & vbCrLf &
"Insider : Nothing" & vbCrLf &
"Previous Insider : Tianyue Sun" & vbCrLf &
"Return : Xianlei Bian，Yi Gao，Zechen Li，Junzhe Jiang" & vbCrLf &
"Version : " & app_version & " " & app_build & vbCrLf &
"Last update date : " & app_update_date & vbCrLf &
"" & vbCrLf &
"User Environment" & vbCrLf &
".NET Framework Version : " & Environment.Version.ToString & vbCrLf &
"Sign up user : " & Environment.UserName.ToString & vbCrLf &
"OS Version : " & Environment.OSVersion.ToString & vbCrLf &
"" & vbCrLf &
"CHMOSGroup Copyright 2012-2016" & vbCrLf &
"" & vbCrLf &
"感谢您使用bus_rode，bus_rode是一个开源软件，使用MIT开源协议，在下面会有开源地址" & vbCrLf &
"您可以加入这个工程，帮助我们变得更好，也可以在下面这个地址反馈错我，我们将会将其" & vbCrLf &
"标记并在将来消除这个问题"
        ui_form_about_describe.Text = describe

        describe = ""
        describe = "CHMOSGroup 机密" & vbCrLf &
            "CHMOSGroup Copyright 2012-2016" & vbCrLf &
            "该版本不应当向外界传播或发布，仅供内部测试，调试使用。将这个版本的程序的源码，界面设计及其他类型的截图，程序副本未经授权向外界传播，公布，发行将会在查明发布者后将其从CHMOSGroup内部强制离职并列入信任黑名单。具体内容参见《CHMOSGroup 协议》" & vbCrLf &
            app_name & " " & app_build
        ui_form_start_secreat_text.Text = describe

        ui_window_title.Text = app_name
        Me.Title = app_name

        '测试调试范围


        '关联控件
        '初始化消息列表
        ui_form_message_list.ItemsSource = ui_connet_core_form_message_list

        '初始化打开文件窗口
        open_resources_file.Filter = app_name & "资源文件|*.brs"
        open_background_file.Filter = "JPG/JPEG图像文件|*.jpg"
        open_mod_file.Filter = "bus_rode插件|*.dll"

    End Sub

    ''' <summary>
    ''' 发送消息的附属函数
    ''' </summary>
    ''' <param name="title"></param>
    ''' <param name="text"></param>
    Public Sub message_ex_ex(ByVal title As String, ByVal text As String)
        If Environment.OSVersion.Version.Major > 6 Or (Environment.OSVersion.Version.Major = 6 And Environment.OSVersion.Version.Minor >= 2) Then
            'win8以上
            app_desktop_icon.ShowBalloonTip(5000, title, text, System.Windows.Forms.ToolTipIcon.Info)
        Else
            ui_form_message_list.ItemsSource = Nothing
            message_ex(title, text)
            ui_form_message_clear.Opacity = 1
            ui_form_message_up_line.Opacity = 1
            ui_form_message_no_msg_title.Opacity = 0
            ui_title_msg_count.Text = Int(ui_title_msg_count.Text) + 1
            ui_form_message_list.ItemsSource = ui_connet_core_form_message_list
        End If
    End Sub

    ''' <summary>
    ''' [ui]显示,隐藏消息的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_message(sender As Object, e As MouseButtonEventArgs) Handles ui_title_msg_count.MouseDown
        If Environment.OSVersion.Version.Major > 6 Or (Environment.OSVersion.Version.Major = 6 And Environment.OSVersion.Version.Minor >= 2) Then
            'win8以上
        Else
            Dim stb_1
            storyboard_action.Stop()
            storyboard_action.Children.Clear()

            If message_show = True Then
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.15)))
            Else
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.15)))
            End If

            Animation.Storyboard.SetTarget(stb_1, ui_form_message)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))
            Dim linshi = New Animation.ExponentialEase()
            linshi.EasingMode = Animation.EasingMode.EaseOut
            stb_1.EasingFunction = linshi

            storyboard_action.Children.Add(stb_1)
            storyboard_action.Begin()

            message_show = Not (message_show)

        End If


    End Sub

#Region "开始到别的面板的移动"

    ''' <summary>
    ''' [ui]从开始界面前往线路的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_goto_line(sender As Object, e As MouseButtonEventArgs) Handles ui_form_start_btn_1_goto.MouseDown

        If no_resource = False Then
            Dim stb_1
            storyboard_action.Stop()
            storyboard_action.Children.Clear()
            '切入新窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.3)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_line)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

            storyboard_action.Children.Add(stb_1)
            '移动旧窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.3)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_start)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

            storyboard_action.Children.Add(stb_1)

            storyboard_action.Begin()

            '设置界面
            screens = 1
            '刷新
            re_window()
        Else
            message_ex_ex("资源", "当前没有安装资源！")
        End If



    End Sub

    ''' <summary>
    ''' [ui]从开始界面前往车站的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_goto_stop(sender As Object, e As MouseButtonEventArgs) Handles ui_form_start_btn_2_goto.MouseDown

        If no_resource = False Then
            Dim stb_1
            storyboard_action.Stop()
            storyboard_action.Children.Clear()
            '切入新窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.3)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_stop)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

            storyboard_action.Children.Add(stb_1)
            '移动旧窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.3)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_start)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

            storyboard_action.Children.Add(stb_1)

            storyboard_action.Begin()

            '设置界面
            screens = 2
            '刷新
            re_window()
        Else
            message_ex_ex("资源", "当前没有安装资源！")
        End If

    End Sub

    ''' <summary>
    ''' [ui]从开始界面前往设置的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_goto_contorl(sender As Object, e As MouseButtonEventArgs) Handles ui_form_start_btn_3_goto.MouseDown
        Dim stb_1
        storyboard_action.Stop()
        storyboard_action.Children.Clear()
        '切入新窗口
        stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.3)))

        Animation.Storyboard.SetTarget(stb_1, ui_form_contorl)
        Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

        storyboard_action.Children.Add(stb_1)
        '移动旧窗口
        stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.3)))

        Animation.Storyboard.SetTarget(stb_1, ui_form_start)
        Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

        storyboard_action.Children.Add(stb_1)

        storyboard_action.Begin()

        '设置界面
        screens = 3
        '刷新
        re_window()
    End Sub

    ''' <summary>
    ''' [ui]从开始界面前往关于的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_goto_about(sender As Object, e As MouseButtonEventArgs) Handles ui_form_start_btn_4_goto.MouseDown
        Dim stb_1
        storyboard_action.Stop()
        storyboard_action.Children.Clear()
        '切入新窗口
        stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.3)))

        Animation.Storyboard.SetTarget(stb_1, ui_form_about)
        Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

        storyboard_action.Children.Add(stb_1)
        '移动旧窗口
        stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.3)))

        Animation.Storyboard.SetTarget(stb_1, ui_form_start)
        Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

        storyboard_action.Children.Add(stb_1)

        storyboard_action.Begin()

        '设置界面
        screens = 4
        '刷新
        re_window()
    End Sub

#End Region

    ''' <summary>
    ''' [系统][ui]刷新窗口的函数
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub re_window()
        '更改大小

        '主面板
        Select Case screens
            Case 0
                Canvas.SetLeft(ui_form_start, 0)
                ui_form_start.Width = window_x
                ui_form_start.Height = window_y - 30

                Canvas.SetLeft(ui_form_line, -window_x)
                ui_form_line.Width = window_x
                ui_form_line.Height = window_y - 30

                Canvas.SetLeft(ui_form_stop, -window_x)
                ui_form_stop.Width = window_x
                ui_form_stop.Height = window_y - 30

                Canvas.SetLeft(ui_form_contorl, -window_x)
                ui_form_contorl.Width = window_x
                ui_form_contorl.Height = window_y - 30

                Canvas.SetLeft(ui_form_about, -window_x)
                ui_form_about.Width = window_x
                ui_form_about.Height = window_y - 30

                ui_title_back_btn_color.Color = Color.FromArgb(180, 143, 143, 143)
                ui_title_menu_btn_color.Color = Color.FromArgb(180, 143, 143, 143)
            Case 1
                Canvas.SetLeft(ui_form_start, -window_x)
                ui_form_start.Width = window_x
                ui_form_start.Height = window_y - 30

                Canvas.SetLeft(ui_form_line, 0)
                ui_form_line.Width = window_x
                ui_form_line.Height = window_y - 30

                Canvas.SetLeft(ui_form_stop, -window_x)
                ui_form_stop.Width = window_x
                ui_form_stop.Height = window_y - 30

                Canvas.SetLeft(ui_form_contorl, -window_x)
                ui_form_contorl.Width = window_x
                ui_form_contorl.Height = window_y - 30

                Canvas.SetLeft(ui_form_about, -window_x)
                ui_form_about.Width = window_x
                ui_form_about.Height = window_y - 30

                ui_title_back_btn_color.Color = Color.FromArgb(180, 255, 0, 0)
                ui_title_menu_btn_color.Color = Color.FromArgb(0, 0, 0, 0)
            Case 2
                Canvas.SetLeft(ui_form_start, -window_x)
                ui_form_start.Width = window_x
                ui_form_start.Height = window_y - 30

                Canvas.SetLeft(ui_form_line, -window_x)
                ui_form_line.Width = window_x
                ui_form_line.Height = window_y - 30

                Canvas.SetLeft(ui_form_stop, 0)
                ui_form_stop.Width = window_x
                ui_form_stop.Height = window_y - 30

                Canvas.SetLeft(ui_form_contorl, -window_x)
                ui_form_contorl.Width = window_x
                ui_form_contorl.Height = window_y - 30

                Canvas.SetLeft(ui_form_about, -window_x)
                ui_form_about.Width = window_x
                ui_form_about.Height = window_y - 30

                ui_title_back_btn_color.Color = Color.FromArgb(180, 255, 0, 0)
                ui_title_menu_btn_color.Color = Color.FromArgb(0, 0, 0, 0)
            Case 3
                Canvas.SetLeft(ui_form_start, -window_x)
                ui_form_start.Width = window_x
                ui_form_start.Height = window_y - 30

                Canvas.SetLeft(ui_form_line, -window_x)
                ui_form_line.Width = window_x
                ui_form_line.Height = window_y - 30

                Canvas.SetLeft(ui_form_stop, -window_x)
                ui_form_stop.Width = window_x
                ui_form_stop.Height = window_y - 30

                Canvas.SetLeft(ui_form_contorl, 0)
                ui_form_contorl.Width = window_x
                ui_form_contorl.Height = window_y - 30

                Canvas.SetLeft(ui_form_about, -window_x)
                ui_form_about.Width = window_x
                ui_form_about.Height = window_y - 30

                ui_title_back_btn_color.Color = Color.FromArgb(180, 255, 0, 0)
                ui_title_menu_btn_color.Color = Color.FromArgb(180, 143, 143, 143)
            Case 4
                Canvas.SetLeft(ui_form_start, -window_x)
                ui_form_start.Width = window_x
                ui_form_start.Height = window_y - 30

                Canvas.SetLeft(ui_form_line, -window_x)
                ui_form_line.Width = window_x
                ui_form_line.Height = window_y - 30

                Canvas.SetLeft(ui_form_stop, -window_x)
                ui_form_stop.Width = window_x
                ui_form_stop.Height = window_y - 30

                Canvas.SetLeft(ui_form_contorl, -window_x)
                ui_form_contorl.Width = window_x
                ui_form_contorl.Height = window_y - 30

                Canvas.SetLeft(ui_form_about, 0)
                ui_form_about.Width = window_x
                ui_form_about.Height = window_y - 30

                ui_title_back_btn_color.Color = Color.FromArgb(180, 255, 0, 0)
                ui_title_menu_btn_color.Color = Color.FromArgb(180, 143, 143, 143)
            Case Else
                window_dialogs_show("灾难性错误", "bus_rode的核心变量值被篡改，程序将在点击确定后立即关闭！", 2, 1, False, "确定", "", Application.Current.MainWindow)
                Environment.Exit(1)
        End Select

        '设置面版
        If line_contorl = True Then
            Canvas.SetLeft(ui_form_line_contorl, 0)
            ui_form_line_contorl.Height = window_y - 30
        Else
            Canvas.SetLeft(ui_form_line_contorl, -300)
            ui_form_line_contorl.Height = window_y - 30
        End If

        ui_form_line_contorl_left_form.Height = window_y - 30 - 72
        ui_form_line_contorl_right_form.Height = window_y - 30 - 72

        If stop_contorl = True Then
            Canvas.SetLeft(ui_form_stop_contorl, 0)
            ui_form_stop_contorl.Height = window_y - 30
        Else
            Canvas.SetLeft(ui_form_stop_contorl, -300)
            ui_form_stop_contorl.Height = window_y - 30
        End If

        ui_form_stop_contorl_left_form.Height = window_y - 30 - 72
        ui_form_stop_contorl_right_form.Height = window_y - 30 - 72


        If line_contorl_page = True Then
            ui_form_line_contorl_left_grid_color.Color = Color.FromArgb(255, 255, 255, 255)
            ui_form_line_contorl_right_grid_color.Color = Color.FromArgb(0, 0, 0, 0)

            Canvas.SetLeft(ui_form_line_contorl_left_form, 0)
            Canvas.SetLeft(ui_form_line_contorl_right_form, -280)
        Else
            ui_form_line_contorl_right_grid_color.Color = Color.FromArgb(255, 255, 255, 255)
            ui_form_line_contorl_left_grid_color.Color = Color.FromArgb(0, 0, 0, 0)

            Canvas.SetLeft(ui_form_line_contorl_right_form, 0)
            Canvas.SetLeft(ui_form_line_contorl_left_form, -280)
        End If
        If stop_contorl_page = True Then
            ui_form_stop_contorl_left_grid_color.Color = Color.FromArgb(255, 255, 255, 255)
            ui_form_stop_contorl_right_grid_color.Color = Color.FromArgb(0, 0, 0, 0)

            Canvas.SetLeft(ui_form_stop_contorl_left_form, 0)
            Canvas.SetLeft(ui_form_stop_contorl_right_form, -280)
        Else
            ui_form_stop_contorl_right_grid_color.Color = Color.FromArgb(255, 255, 255, 255)
            ui_form_stop_contorl_left_grid_color.Color = Color.FromArgb(0, 0, 0, 0)

            Canvas.SetLeft(ui_form_stop_contorl_right_form, 0)
            Canvas.SetLeft(ui_form_stop_contorl_left_form, -280)
        End If





        '消息面版
        If message_show = True Then
            ui_form_message.Height = window_y - 30
            Canvas.SetLeft(ui_form_message, window_x - 300)
        Else
            ui_form_message.Height = window_y - 30
            Canvas.SetLeft(ui_form_message, window_x + 300)
        End If


        '控制界面的开关
        If get_bus_addr = True Then
            ui_form_contorl_check_background_color_1.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
            ui_form_contorl_check_background_color_2.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
            ui_form_contorl_check_background_color_3.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
            Canvas.SetLeft(ui_form_contorl_check_btn, 70)
        Else
            ui_form_contorl_check_background_color_1.Color = Color.FromArgb(255, 143, 143, 143)
            ui_form_contorl_check_background_color_2.Color = Color.FromArgb(255, 143, 143, 143)
            ui_form_contorl_check_background_color_3.Color = Color.FromArgb(255, 143, 143, 143)
            Canvas.SetLeft(ui_form_contorl_check_btn, 0)
        End If
        If talk_man = True Then
            ui_form_contorl_check_background_color_2_1.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
            ui_form_contorl_check_background_color_2_2.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
            ui_form_contorl_check_background_color_2_3.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
            Canvas.SetLeft(ui_form_contorl_check_btn_2, 70)
        Else
            ui_form_contorl_check_background_color_2_1.Color = Color.FromArgb(255, 143, 143, 143)
            ui_form_contorl_check_background_color_2_2.Color = Color.FromArgb(255, 143, 143, 143)
            ui_form_contorl_check_background_color_2_3.Color = Color.FromArgb(255, 143, 143, 143)
            Canvas.SetLeft(ui_form_contorl_check_btn_2, 0)
        End If
    End Sub


#Region "按钮"
    ''' <summary>
    ''' [ui]返回按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_back_page(sender As Object, e As MouseButtonEventArgs) Handles ui_title_back_btn.MouseDown
        Dim stb_1
        storyboard_action.Stop()
        storyboard_action.Children.Clear()

        Select Case screens
            Case 0

            Case 1
                '切入新窗口
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.3)))

                Animation.Storyboard.SetTarget(stb_1, ui_form_start)
                Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                storyboard_action.Children.Add(stb_1)
                '移动旧窗口
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.3)))

                Animation.Storyboard.SetTarget(stb_1, ui_form_line)
                Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                storyboard_action.Children.Add(stb_1)

                If line_contorl = True Then
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.15)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_line_contorl)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))
                    Dim linshi = New Animation.ExponentialEase()
                    linshi.EasingMode = Animation.EasingMode.EaseOut
                    stb_1.EasingFunction = linshi

                    storyboard_action.Children.Add(stb_1)
                End If

                '设置界面
                screens = 0
                If line_contorl = True Then
                    line_contorl = False
                End If

            Case 2

                If jump_to_stop_from_line = True Then
                    '退回线路界面

                    '切入新窗口
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.3)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_line)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                    storyboard_action.Children.Add(stb_1)
                    '移动旧窗口
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.3)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_stop)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                    storyboard_action.Children.Add(stb_1)

                    '修改回来
                    jump_to_stop_from_line = False
                    '设置界面
                    screens = 1

                Else
                    '退回主界面

                    '切入新窗口
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.3)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_start)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                    storyboard_action.Children.Add(stb_1)
                    '移动旧窗口
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.3)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_stop)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                    storyboard_action.Children.Add(stb_1)

                    '设置界面
                    screens = 0
                End If

                If stop_contorl = True Then
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.15)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_stop_contorl)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))
                    Dim linshi = New Animation.ExponentialEase()
                    linshi.EasingMode = Animation.EasingMode.EaseOut
                    stb_1.EasingFunction = linshi

                    storyboard_action.Children.Add(stb_1)
                End If

                '设置界面
                If stop_contorl = True Then
                    stop_contorl = False
                End If
            Case 3
                '切入新窗口
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.3)))

                Animation.Storyboard.SetTarget(stb_1, ui_form_start)
                Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                storyboard_action.Children.Add(stb_1)
                '移动旧窗口
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.3)))

                Animation.Storyboard.SetTarget(stb_1, ui_form_contorl)
                Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                storyboard_action.Children.Add(stb_1)

                '设置界面
                screens = 0

            Case 4
                '切入新窗口
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.3)))

                Animation.Storyboard.SetTarget(stb_1, ui_form_start)
                Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                storyboard_action.Children.Add(stb_1)
                '移动旧窗口
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.3)))

                Animation.Storyboard.SetTarget(stb_1, ui_form_about)
                Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                storyboard_action.Children.Add(stb_1)

                '设置界面
                screens = 0

            Case Else
                window_dialogs_show("灾难性错误", "bus_rode的核心变量值被篡改，程序将在点击确定后立即关闭！", 2, 1, False, "确定", "", Application.Current.MainWindow)
                Environment.Exit(1)
        End Select


        storyboard_action.Begin()

        '刷新
        re_window()
    End Sub

    ''' <summary>
    ''' [ui]菜单按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_menu(sender As Object, e As MouseButtonEventArgs) Handles ui_title_menu_btn.MouseDown
        Dim stb_1
        storyboard_action.Stop()
        storyboard_action.Children.Clear()

        Select Case screens
            Case 0

            Case 1

                If line_contorl = True Then
                    '收回
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.15)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_line_contorl)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))
                    Dim linshi = New Animation.ExponentialEase()
                    linshi.EasingMode = Animation.EasingMode.EaseOut
                    stb_1.EasingFunction = linshi

                    storyboard_action.Children.Add(stb_1)
                Else
                    '展开
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.15)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_line_contorl)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))
                    Dim linshi = New Animation.ExponentialEase()
                    linshi.EasingMode = Animation.EasingMode.EaseOut
                    stb_1.EasingFunction = linshi

                    storyboard_action.Children.Add(stb_1)
                End If

                '设置界面
                line_contorl = Not (line_contorl)

            Case 2

                If stop_contorl = True Then
                    '收回
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.15)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_stop_contorl)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))
                    Dim linshi = New Animation.ExponentialEase()
                    linshi.EasingMode = Animation.EasingMode.EaseOut
                    stb_1.EasingFunction = linshi

                    storyboard_action.Children.Add(stb_1)
                Else
                    '展开
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.15)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_stop_contorl)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))
                    Dim linshi = New Animation.ExponentialEase()
                    linshi.EasingMode = Animation.EasingMode.EaseOut
                    stb_1.EasingFunction = linshi

                    storyboard_action.Children.Add(stb_1)
                End If

                '设置界面
                stop_contorl = Not (stop_contorl)
            Case 3

            Case 4

            Case Else
                window_dialogs_show("灾难性错误", "bus_rode的核心变量值被篡改， 程序将在点击确定后立即关闭！", 2, 1, False, "确定", "", Application.Current.MainWindow)

                Environment.Exit(1)
        End Select


        storyboard_action.Begin()

        '刷新
        re_window()
    End Sub

    ''' <summary>
    ''' [ui]线路控制界面中左边按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_line_contorl_left_btn(sender As Object, e As MouseButtonEventArgs)
        If line_contorl_page = True Then
            '就是这面，不切换
        Else
            Dim stb_1
            storyboard_action.Stop()
            storyboard_action.Children.Clear()
            '切入新窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(-280, 0, New Duration(TimeSpan.FromSeconds(0.15)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_line_contorl_left_form)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))
            Dim linshi = New Animation.ExponentialEase()
            linshi.EasingMode = Animation.EasingMode.EaseOut
            stb_1.EasingFunction = linshi

            storyboard_action.Children.Add(stb_1)
            '移动旧窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, -280, New Duration(TimeSpan.FromSeconds(0.15)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_line_contorl_right_form)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))
            Dim linshi_2 = New Animation.ExponentialEase()
            linshi_2.EasingMode = Animation.EasingMode.EaseOut
            stb_1.EasingFunction = linshi_2

            storyboard_action.Children.Add(stb_1)

            storyboard_action.Begin()

            '设置界面
            line_contorl_page = True

            '刷新
            re_window()
        End If
    End Sub

    ''' <summary>
    ''' [ui]线路控制界面中右边按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_line_contorl_right_btn(sender As Object, e As MouseButtonEventArgs)
        If line_contorl_page = True Then

            Dim stb_1
            storyboard_action.Stop()
            storyboard_action.Children.Clear()
            '切入新窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(-280, 0, New Duration(TimeSpan.FromSeconds(0.15)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_line_contorl_right_form)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))
            Dim linshi = New Animation.ExponentialEase()
            linshi.EasingMode = Animation.EasingMode.EaseOut
            stb_1.EasingFunction = linshi

            storyboard_action.Children.Add(stb_1)
            '移动旧窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, -280, New Duration(TimeSpan.FromSeconds(0.15)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_line_contorl_left_form)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))
            Dim linshi_2 = New Animation.ExponentialEase()
            linshi_2.EasingMode = Animation.EasingMode.EaseOut
            stb_1.EasingFunction = linshi_2

            storyboard_action.Children.Add(stb_1)

            storyboard_action.Begin()

            '设置界面
            line_contorl_page = False

            '刷新
            re_window()
        Else
            '就是这面，不切换
        End If
    End Sub

    ''' <summary>
    ''' [ui]车站控制界面中左边按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_stop_contorl_left_btn(sender As Object, e As MouseButtonEventArgs)
        If stop_contorl_page = True Then
            '就是这面，不切换
        Else
            Dim stb_1
            storyboard_action.Stop()
            storyboard_action.Children.Clear()
            '切入新窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(-280, 0, New Duration(TimeSpan.FromSeconds(0.15)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_stop_contorl_left_form)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))
            Dim linshi = New Animation.ExponentialEase()
            linshi.EasingMode = Animation.EasingMode.EaseOut
            stb_1.EasingFunction = linshi

            storyboard_action.Children.Add(stb_1)
            '移动旧窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, -280, New Duration(TimeSpan.FromSeconds(0.15)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_stop_contorl_right_form)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))
            Dim linshi_2 = New Animation.ExponentialEase()
            linshi_2.EasingMode = Animation.EasingMode.EaseOut
            stb_1.EasingFunction = linshi_2

            storyboard_action.Children.Add(stb_1)

            storyboard_action.Begin()

            '设置界面
            stop_contorl_page = True

            '刷新
            re_window()
        End If
    End Sub

    ''' <summary>
    ''' [ui]车站控制界面中右边按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_stop_contorl_right_btn(sender As Object, e As MouseButtonEventArgs)
        If stop_contorl_page = True Then

            Dim stb_1
            storyboard_action.Stop()
            storyboard_action.Children.Clear()
            '切入新窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(-280, 0, New Duration(TimeSpan.FromSeconds(0.15)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_stop_contorl_right_form)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))
            Dim linshi = New Animation.ExponentialEase()
            linshi.EasingMode = Animation.EasingMode.EaseOut
            stb_1.EasingFunction = linshi

            storyboard_action.Children.Add(stb_1)
            '移动旧窗口
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, -280, New Duration(TimeSpan.FromSeconds(0.15)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_stop_contorl_left_form)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))
            Dim linshi_2 = New Animation.ExponentialEase()
            linshi_2.EasingMode = Animation.EasingMode.EaseOut
            stb_1.EasingFunction = linshi_2

            storyboard_action.Children.Add(stb_1)

            storyboard_action.Begin()

            '设置界面
            stop_contorl_page = False

            '刷新
            re_window()
        Else
            '就是这面，不切换
        End If
    End Sub

    ''' <summary>
    ''' [ui]设置界面中开关滑块
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_function_contorl_check_btn(sender As Object, e As MouseButtonEventArgs) Handles ui_form_contorl_check_btn.MouseDown
        If no_resource = False Then
            Dim stb_1
            storyboard_action.Stop()
            storyboard_action.Children.Clear()

            If get_bus_addr = True Then
                '关闭
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(70, 0, New Duration(TimeSpan.FromSeconds(0.1)))

                Animation.Storyboard.SetTarget(stb_1, ui_form_contorl_check_btn)
                Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))
                Dim linshi = New Animation.ExponentialEase()
                linshi.EasingMode = Animation.EasingMode.EaseOut
                stb_1.EasingFunction = linshi

                storyboard_action.Children.Add(stb_1)
            Else
                '打开
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 70, New Duration(TimeSpan.FromSeconds(0.1)))

                Animation.Storyboard.SetTarget(stb_1, ui_form_contorl_check_btn)
                Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))
                Dim linshi = New Animation.ExponentialEase()
                linshi.EasingMode = Animation.EasingMode.EaseOut
                stb_1.EasingFunction = linshi

                storyboard_action.Children.Add(stb_1)
            End If

            storyboard_action.Begin()

            '设置界面
            get_bus_addr = Not (get_bus_addr)
            If get_bus_addr = True Then
                If System.IO.File.Exists(Environment.CurrentDirectory + "\bus_rode_mod.dll") = True Then
                    '文件存在
                    get_resource_timer.Start()
                    connect_dll_get_resources_always_stop = False
                Else
                    message_ex("未找到模块", "未找到指定模块，可能是未安装模块")
                End If
            Else
                get_resource_timer.Stop()
                connect_dll_get_resources_always_stop = True
            End If
            '清空
            read_mid_bus_word = ""

            '保存设置
            save_user_contorl()

            '刷新
            re_window()
        Else
            message_ex_ex("资源", "当前没有安装资源！")
        End If

    End Sub


    ''' <summary>
    ''' [系统][ui]线路界面选择上行线路的按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_line_select_up_line_fx(sender As Object, e As MouseButtonEventArgs)
        If up_or_down_line = True Then
            '是的，不切换
        Else
            ui_form_line_select_up_line.Color = Color.FromArgb(143, form_color.R, form_color.G, form_color.B)
            ui_form_line_select_down_line.Color = Color.FromArgb(0, 0, 0, 0)
            ui_form_line_have_stop_list.ItemsSource = Nothing
            ui_connet_core_form_line_have_sotp_list_show = ui_connet_core_form_line_have_sotp_list_up
            ui_form_line_have_stop_list.ItemsSource = ui_connet_core_form_line_have_sotp_list_show
            up_or_down_line = True
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]线路界面选择下行线路的按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_line_select_down_line_fx(sender As Object, e As MouseButtonEventArgs)
        If up_or_down_line = False Then
            '是的，不切换
        Else
            ui_form_line_select_up_line.Color = Color.FromArgb(0, 0, 0, 0)
            ui_form_line_select_down_line.Color = Color.FromArgb(143, form_color.R, form_color.G, form_color.B)
            ui_form_line_have_stop_list.ItemsSource = Nothing
            ui_connet_core_form_line_have_sotp_list_show = ui_connet_core_form_line_have_sotp_list_down
            ui_form_line_have_stop_list.ItemsSource = ui_connet_core_form_line_have_sotp_list_show
            up_or_down_line = False
        End If
    End Sub

#End Region

    ''' <summary>
    ''' [系统][ui]车辆列表选择新项的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_form_line_list_function(sender As Object, e As SelectionChangedEventArgs) Handles ui_form_line_list.SelectionChanged
        If ui_form_line_list.SelectedIndex <> -1 And ui_form_line_contorl_left_form_textbox.Text = "" Then
            '消除线路内搜索站台的数据
            ui_form_line_contorl_right_form_textbox.Text = ""

            ui_form_line_have_stop_list.ItemsSource = Nothing
            ui_form_line_subway_list.ItemsSource = Nothing

            clear()
            up_or_down_line = True
            bus = have_bus(ui_form_line_list.SelectedIndex)
            read_bus()
            read_mid_bus_word = ""
            read_mid_bus()
            look_bus_date()

            ui_form_line_select_up_line.Color = Color.FromArgb(143, form_color.R, form_color.G, form_color.B)
            ui_form_line_select_down_line.Color = Color.FromArgb(0, 0, 0, 0)

            '设置文本
            If bus_or_subway = 0 Then
                '公交
                ui_form_line_describe.Text = "当前车次：" + bus + vbCrLf + "类别：公交" + vbCrLf + bus_msg(0) + vbCrLf + bus_msg(1) + vbCrLf + bus_msg(2) + vbCrLf + bus_msg(3) + vbCrLf + bus_msg(4) + vbCrLf + bus_run + vbCrLf + "等待该车的难度:" + bus_wait_hard
            Else
                '地铁
                ui_form_line_describe.Text = "当前车次：" + bus + vbCrLf + "类别：地铁" + vbCrLf + bus_msg(0) + vbCrLf + bus_msg(1) + vbCrLf + bus_msg(2) + vbCrLf + bus_msg(3) + vbCrLf + bus_msg(4) + vbCrLf + bus_run + vbCrLf + "等待该车的难度:" + bus_wait_hard
            End If

            select_stop_point = 0

            '设置上行线路
            ui_connet_core_form_line_have_sotp_list_show = ui_connet_core_form_line_have_sotp_list_up
            ui_form_line_have_stop_list.ItemsSource = ui_connet_core_form_line_have_sotp_list_show
            ui_form_line_subway_list.ItemsSource = ui_connet_core_form_line_subway_list

        Else
            If ui_form_line_list.SelectedIndex <> -1 Then
                '正在自动填充
                Dim list_name As String = ui_connet_core_form_line_list_for_search.Item(ui_form_line_list.SelectedIndex).ui_line_name
                Dim list As Integer = return_bus_list(list_name)

                If list <> -1 Then
                    '找到项目，切换
                    ui_form_line_have_stop_list.ItemsSource = Nothing
                    ui_form_line_subway_list.ItemsSource = Nothing

                    '消除线路内搜索站台的数据
                    ui_form_line_contorl_right_form_textbox.Text = ""

                    clear()
                    up_or_down_line = True
                    bus = have_bus(list)
                    read_bus()
                    read_mid_bus_word = ""
                    read_mid_bus()
                    look_bus_date()

                    ui_form_line_select_up_line.Color = Color.FromArgb(143, form_color.R, form_color.G, form_color.B)
                    ui_form_line_select_down_line.Color = Color.FromArgb(0, 0, 0, 0)

                    '设置文本
                    If bus_or_subway = 0 Then
                        '公交
                        ui_form_line_describe.Text = "当前车次：" + bus + vbCrLf + "类别：公交" + vbCrLf + bus_msg(0) + vbCrLf + bus_msg(1) + vbCrLf + bus_msg(2) + vbCrLf + bus_msg(3) + vbCrLf + bus_msg(4) + vbCrLf + bus_run + vbCrLf + "等待该车的难度:" + bus_wait_hard
                    Else
                        '地铁
                        ui_form_line_describe.Text = "当前车次：" + bus + vbCrLf + "类别：地铁" + vbCrLf + bus_msg(0) + vbCrLf + bus_msg(1) + vbCrLf + bus_msg(2) + vbCrLf + bus_msg(3) + vbCrLf + bus_msg(4) + vbCrLf + bus_run + vbCrLf + "等待该车的难度:" + bus_wait_hard
                    End If

                    select_stop_point = 0

                    '设置上行线路
                    ui_connet_core_form_line_have_sotp_list_show = ui_connet_core_form_line_have_sotp_list_up
                    ui_form_line_have_stop_list.ItemsSource = ui_connet_core_form_line_have_sotp_list_show
                    ui_form_line_subway_list.ItemsSource = ui_connet_core_form_line_subway_list


                    '还原自动填充
                    ui_form_line_contorl_left_form_textbox.Text = ""
                Else
                    '出现错误
                    Environment.Exit(5)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]站台列表选择新项的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_form_stop_list_function(sender As Object, e As SelectionChangedEventArgs) Handles ui_form_stop_list.SelectionChanged
        If ui_form_stop_list.SelectedIndex <> -1 And ui_form_stop_contorl_left_form_textbox.Text = "" Then
            ui_form_stop_left_cross_line_list.ItemsSource = Nothing
            ui_form_stop_right_cross_line_list.ItemsSource = Nothing

            now_stop = ui_form_stop_list.SelectedIndex
            refsh_stop()

            '设置文本
            ui_form_stop_describe.Text = cross_stop
            ui_form_stop_middle_stop_name.Text = bus_stop_stop(now_stop)

            ui_form_stop_left_cross_line_list.ItemsSource = ui_connet_core_form_stop_left_cross_line_list
            ui_form_stop_right_cross_line_list.ItemsSource = ui_connet_core_form_stop_right_cross_line_list

        Else
            If ui_form_stop_list.SelectedIndex <> -1 Then
                '正在自动填充
                Dim list_name As String = ui_connet_core_form_stop_list_for_search.Item(ui_form_stop_list.SelectedIndex).ui_stop_name
                Dim list As Integer = return_check_stop_list_nopage(list_name)

                If list <> -1 Then
                    '找到项目，切换
                    ui_form_stop_left_cross_line_list.ItemsSource = Nothing
                    ui_form_stop_right_cross_line_list.ItemsSource = Nothing

                    now_stop = list
                    refsh_stop()

                    '设置文本
                    ui_form_stop_describe.Text = cross_stop
                    ui_form_stop_middle_stop_name.Text = bus_stop_stop(now_stop)

                    ui_form_stop_left_cross_line_list.ItemsSource = ui_connet_core_form_stop_left_cross_line_list
                    ui_form_stop_right_cross_line_list.ItemsSource = ui_connet_core_form_stop_right_cross_line_list


                    '还原自动填充
                    ui_form_stop_contorl_left_form_textbox.Text = ""
                Else
                    '出现错误
                    Environment.Exit(6)
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]线路左侧面板的搜索按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_line_contorl_left_function(sender As Object, e As MouseButtonEventArgs)
        If ui_form_line_contorl_left_form_textbox.Text <> "" Then
            Dim a As Integer = return_bus_list(ui_form_line_contorl_left_form_textbox.Text)
            If a <> -1 Then
                ui_form_line_list.SelectedIndex = a
                ui_form_line_contorl_left_form_textbox.Text = ""
            Else
                message_ex_ex("搜索", "没有找到车次：" + ui_form_line_contorl_left_form_textbox.Text)
            End If
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]线路右侧面板的搜索按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_form_line_contorl_right_function(sender As Object, e As MouseButtonEventArgs)
        If ui_form_line_contorl_right_form_textbox.Text <> "" Then

            Dim a As Integer = return_bus_stop_list(ui_form_line_contorl_right_form_textbox.Text)
            If a <> -1 Then
                select_stop_point = a
                ui_form_line_contorl_left_form_textbox.Text = ""
                ui_form_line_have_stop_list.SelectedIndex = a

            Else
                message_ex_ex("搜索", "没有在该线路中找到站台：" + ui_form_line_contorl_right_form_textbox.Text)
            End If

        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]线路拥有站台列表-上行选择新项的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_form_line_have_stop_list_up_function(sender As Object, e As SelectionChangedEventArgs) Handles ui_form_line_have_stop_list.SelectionChanged
        If ui_form_line_have_stop_list.SelectedIndex <> -1 Then
            select_stop_point = ui_form_line_have_stop_list.SelectedIndex
            If bus_or_subway = 1 Then
                ui_form_line_subway_list.ItemsSource = Nothing

                re_subway_stop()

                ui_form_line_subway_list.ItemsSource = ui_connet_core_form_line_subway_list
            End If
        End If

    End Sub

    ''' <summary>
    ''' [系统][ui]站台左侧面板搜索按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_stop_contorl_left_function(sender As Object, e As MouseButtonEventArgs)
        If ui_form_stop_contorl_left_form_textbox.Text <> "" Then
            Dim a As Integer = return_check_stop_list_nopage(ui_form_stop_contorl_left_form_textbox.Text)
            If a <> -1 Then
                ui_form_stop_list.SelectedIndex = a
                ui_form_stop_contorl_left_form_textbox.Text = ""
            Else
                message_ex_ex("搜索", "没有找到车站：" + ui_form_stop_contorl_left_form_textbox.Text)
            End If
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]设置界面获取资源信息
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_contorl_list_3_function(sender As Object, e As MouseButtonEventArgs)
        If no_resource = False Then
            window_dialogs_show("资源属性/自述", System.IO.File.ReadAllText(Environment.CurrentDirectory + "\library\readme.txt"), 0, 1, False, "确定", "", Application.Current.MainWindow)
        Else
            message_ex_ex("资源", "当前没有安装资源！")
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]设置界面重启
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_contorl_list_5_function(sender As Object, e As MouseButtonEventArgs)
        Process.Start(Environment.CurrentDirectory + "\file.bat")
        Environment.Exit(3)
    End Sub

    ''' <summary>
    ''' [系统][ui]消息界面清空消息
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_form_message_function(sender As Object, e As MouseButtonEventArgs)
        ui_form_message_list.ItemsSource = Nothing
        ui_connet_core_form_message_list.Clear()
        ui_title_msg_count.Text = 0
        ui_form_message_clear.Opacity = 0
        ui_form_message_up_line.Opacity = 0
        ui_form_message_no_msg_title.Opacity = 1
        ui_form_message_list.ItemsSource = ui_connet_core_form_message_list
    End Sub

    ''' <summary>
    ''' [系统][ui]设置面板更换颜色选项
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_contorl_list_6_function(sender As Object, e As MouseButtonEventArgs)

        Dim new_color_r As Integer = 0
        Dim new_color_g As Integer = 0
        Dim new_color_b As Integer = 0
        Try
            new_color_r = Int(ui_form_contorl_r_value.Text)
            new_color_g = Int(ui_form_contorl_g_value.Text)
            new_color_b = Int(ui_form_contorl_b_value.Text)

            If 0 <= new_color_r And new_color_r <= 255 Then
                If 0 <= new_color_g And new_color_g <= 255 Then
                    If 0 <= new_color_b And new_color_b <= 255 Then
                        '符合条件

                        form_color = Color.FromRgb(new_color_r, new_color_g, new_color_b)

                        '设置颜色 1-3透明df=223 4透明8f=143 5-8透明6f=111
                        ui_color_1.Color = Color.FromArgb(223, form_color.R, form_color.G, form_color.B)
                        ui_color_2.Color = Color.FromArgb(223, form_color.R, form_color.G, form_color.B)
                        ui_color_3.Color = Color.FromArgb(223, form_color.R, form_color.G, form_color.B)

                        ui_color_4.Color = Color.FromArgb(143, form_color.R, form_color.G, form_color.B)

                        ui_color_5.Color = Color.FromArgb(111, form_color.R, form_color.G, form_color.B)
                        ui_color_6.Color = Color.FromArgb(111, form_color.R, form_color.G, form_color.B)
                        ui_color_7.Color = Color.FromArgb(111, form_color.R, form_color.G, form_color.B)
                        ui_color_8.Color = Color.FromArgb(111, form_color.R, form_color.G, form_color.B)

                        If up_or_down_line = True Then
                            '上行
                            ui_form_line_select_up_line.Color = Color.FromArgb(143, form_color.R, form_color.G, form_color.B)
                            ui_form_line_select_down_line.Color = Color.FromArgb(0, 0, 0, 0)
                        Else
                            '下行
                            ui_form_line_select_up_line.Color = Color.FromArgb(0, 0, 0, 0)
                            ui_form_line_select_down_line.Color = Color.FromArgb(143, form_color.R, form_color.G, form_color.B)
                        End If

                        If get_bus_addr = True Then
                            ui_form_contorl_check_background_color_1.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
                            ui_form_contorl_check_background_color_2.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
                            ui_form_contorl_check_background_color_3.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
                        End If
                        If talk_man = True Then
                            ui_form_contorl_check_background_color_2_1.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
                            ui_form_contorl_check_background_color_2_2.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
                            ui_form_contorl_check_background_color_2_3.Color = Color.FromArgb(255, form_color.R, form_color.G, form_color.B)
                        End If

                        '保存设置
                        save_user_contorl()
                    Else
                        window_dialogs_show("数值", "数值非法", 2, 1, False, "确定", "", Application.Current.MainWindow)
                        ui_form_contorl_r_value.Text = form_color.R
                        ui_form_contorl_g_value.Text = form_color.G
                        ui_form_contorl_b_value.Text = form_color.B
                    End If
                Else
                    window_dialogs_show("数值", "数值非法", 2, 1, False, "确定", "", Application.Current.MainWindow)
                    ui_form_contorl_r_value.Text = form_color.R
                    ui_form_contorl_g_value.Text = form_color.G
                    ui_form_contorl_b_value.Text = form_color.B
                End If
            Else
                window_dialogs_show("数值", "数值非法", 2, 1, False, "确定", "", Application.Current.MainWindow)
                ui_form_contorl_r_value.Text = form_color.R
                ui_form_contorl_g_value.Text = form_color.G
                ui_form_contorl_b_value.Text = form_color.B
            End If
        Catch ex As Exception
            '输入文本类型错误
            window_dialogs_show("数值", "数值非法", 2, 1, False, "确定", "", Application.Current.MainWindow)
            ui_form_contorl_r_value.Text = form_color.R
            ui_form_contorl_g_value.Text = form_color.G
            ui_form_contorl_b_value.Text = form_color.B
        End Try
    End Sub

    ''' <summary>
    ''' [系统][ui]设置面板颜色textbox数值更改
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_form_contorl_value_change(sender As Object, e As TextChangedEventArgs) Handles ui_form_contorl_r_value.TextChanged, ui_form_contorl_g_value.TextChanged, ui_form_contorl_b_value.TextChanged
        Dim new_color_r As Integer = 0
        Dim new_color_g As Integer = 0
        Dim new_color_b As Integer = 0
        Try
            new_color_r = Int(ui_form_contorl_r_value.Text)
            new_color_g = Int(ui_form_contorl_g_value.Text)
            new_color_b = Int(ui_form_contorl_b_value.Text)

            If 0 <= new_color_r And new_color_r <= 255 Then
                If 0 <= new_color_g And new_color_g <= 255 Then
                    If 0 <= new_color_b And new_color_b <= 255 Then
                        '符合条件
                    Else
                        'MsgBox("数值非法")
                    End If
                Else
                    'MsgBox("数值非法")
                End If
            Else
                'MsgBox("数值非法")
            End If
        Catch ex As Exception
            '输入文本类型错误
            'MsgBox("数值非法")
        End Try
    End Sub

    ''' <summary>
    ''' [系统][ui]车站界面执行最短路径的按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_form_stop_short_line_search_function(sender As Object, e As MouseButtonEventArgs)
        '开始计算最短路径按钮

        before_stamp_start_stop = stamp_start_stop
        before_stamp_end_stop = stamp_end_stop

        stamp_start_stop = ui_form_stop_contorl_right_form_textbox_1.Text
        stamp_end_stop = ui_form_stop_contorl_right_form_textbox_2.Text

        If ui_form_stop_short_line_check() = True Then

            'ui设置
            short_line_can_page = True
            ui_form_stop_contorl_step_describe.Text = "正在计算换乘路线，请稍候。如果是第一次使用这个功能，可能需要更多时间。"

            toword_local = True


            '运行计算线程
            run_calc_flag = True

            '检测screens变化，并帮助执行刷新
            Do
                If short_line_can_page = False Then
                    '结束工作开始设置
                    step_number = 0
                    If toword(0, 0) = "" Then
                        '没有可以到的线路
                        short_line_can_page = True

                        ui_form_stop_contorl_step_describe.Text = "由 " & stamp_start_stop & " 至 " & stamp_end_stop & " 的换乘不能进行" & vbCrLf &
                            "因为换乘线路过长，或者是根本无法换乘，对此，我们感到十分抱歉"

                        ui_form_stop_contorl_left_page.Opacity = 0
                        ui_form_stop_contorl_right_page.Opacity = 0

                        ui_form_stop_contorl_page.Text = "步骤"
                    Else
                        ui_form_stop_contorl_page.Text = "步骤1"

                        ui_form_stop_contorl_step_describe.Text = "在 " & toword(step_number, 0) & " 乘坐 " & toword(step_number, 1) & " (" & toword(step_number, 2) & " 方向)" & vbCrLf & "在 "

                        If toword(step_number + 1, 0) = "" Then
                            ui_form_stop_contorl_step_describe.Text = ui_form_stop_contorl_step_describe.Text & stamp_end_stop & " 下车结束换乘，祝您旅途愉快，再见"
                        Else
                            ui_form_stop_contorl_step_describe.Text = ui_form_stop_contorl_step_describe.Text & toword(step_number + 1, 0) & " 下车，准备下一个换乘"
                        End If
                        ui_form_stop_contorl_step_describe.Text = ui_form_stop_contorl_step_describe.Text & vbCrLf & vbCrLf & "有关 " & toword(step_number, 1) & " 运营时间的信息 " & check_bus_on_time(toword(step_number, 1))

                        ui_form_stop_contorl_left_page.Opacity = 0
                        If toword(1, 0) = "" Then
                            '没有下一页
                            ui_form_stop_contorl_right_page.Opacity = 0
                        Else
                            ui_form_stop_contorl_right_page.Opacity = 1
                        End If
                    End If

                    Exit Do
                Else
                    System.Threading.Thread.Sleep(1000)
                End If
            Loop
        End If

    End Sub

    ''' <summary>
    ''' [系统][ui]检测最短路径是否可以执行的函数
    ''' </summary>
    ''' <returns></returns>
    Public Function ui_form_stop_short_line_check()
        Dim yes As Boolean = False

        If stamp_start_stop <> "" And stamp_end_stop <> "" Then
            If stamp_start_stop = stamp_end_stop Then
                message_ex_ex("换乘规划站点错误", "起点和终点不能相同！")
            Else
                If check_stop_list(stamp_start_stop) = False Or check_stop_list(stamp_end_stop) = False Then
                    message_ex_ex("换乘规划站点错误", "没有找到指定车站，请确认车站存在！")
                Else
                    If stamp_start_stop = before_stamp_start_stop And stamp_end_stop = before_stamp_end_stop Then
                        '选择项相同,直接保持不变
                        message_ex_ex("换乘规划", "结果已经计算完成，无需再次计算！")
                    Else
                        yes = True
                    End If
                End If
            End If
        Else
            message_ex_ex("换乘规划站点错误", "起点和终点不能为空！")
        End If

        Return yes
    End Function

    ''' <summary>
    ''' [系统][ui]最短路径向下翻页
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_form_stop_short_line_down(sender As Object, e As MouseButtonEventArgs) Handles ui_form_stop_contorl_right_page.MouseDown
        '下一页按钮
        If step_number <> 20 Then
            If toword(step_number + 1, 0) = "" Then
                '拒绝
            Else
                step_number += 1
                ui_form_stop_contorl_page.Text = "步骤" & (step_number + 1)

                If toword_local = True Then


                    ui_form_stop_contorl_step_describe.Text = "在 " & toword(step_number, 0) & " 乘坐 " & toword(step_number, 1) & " (" & toword(step_number, 2) & " 方向)" & vbCrLf & "在 "

                    If toword(step_number + 1, 0) = "" Then
                        ui_form_stop_contorl_step_describe.Text = ui_form_stop_contorl_step_describe.Text & stamp_end_stop & " 下车结束换乘，祝您旅途愉快，再见"
                    Else
                        ui_form_stop_contorl_step_describe.Text = ui_form_stop_contorl_step_describe.Text & toword(step_number + 1, 0) & " 下车，准备下一个换乘"
                    End If
                    ui_form_stop_contorl_step_describe.Text = ui_form_stop_contorl_step_describe.Text & vbCrLf & vbCrLf & "有关 " & toword(step_number, 1) & " 运营时间的信息 " & check_bus_on_time(toword(step_number, 1))

                Else
                    ui_form_stop_contorl_step_describe.Text = toword(step_number, 0)
                End If

                If step_number = 0 Then
                    '没有上一页
                    ui_form_stop_contorl_left_page.Opacity = 0
                Else
                    ui_form_stop_contorl_left_page.Opacity = 1
                End If
                If toword(step_number + 1, 0) = "" Then
                    '没有下一页
                    ui_form_stop_contorl_right_page.Opacity = 0
                Else
                    ui_form_stop_contorl_right_page.Opacity = 1
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]最短路径向上翻页
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ui_form_stop_short_line_up(sender As Object, e As MouseButtonEventArgs) Handles ui_form_stop_contorl_left_page.MouseDown
        '上一页按钮
        If step_number <> 0 Then
            step_number -= 1

            ui_form_stop_contorl_page.Text = "步骤" & (step_number + 1)

            If toword_local = True Then
                ui_form_stop_contorl_step_describe.Text = "在 " & toword(step_number, 0) & " 乘坐 " & toword(step_number, 1) & " (" & toword(step_number, 2) & " 方向)" & vbCrLf & "在 "

                If toword(step_number + 1, 0) = "" Then
                    ui_form_stop_contorl_step_describe.Text = ui_form_stop_contorl_step_describe.Text & stamp_end_stop & " 下车结束换乘，祝您旅途愉快，再见"
                Else
                    ui_form_stop_contorl_step_describe.Text = ui_form_stop_contorl_step_describe.Text & toword(step_number + 1, 0) & " 下车，准备下一个换乘"
                End If
                ui_form_stop_contorl_step_describe.Text = ui_form_stop_contorl_step_describe.Text & vbCrLf & vbCrLf & "有关 " & toword(step_number, 1) & " 运营时间的信息 " & check_bus_on_time(toword(step_number, 1))

            Else
                ui_form_stop_contorl_step_describe.Text = toword(step_number, 0)
            End If


            If step_number = 0 Then
                '没有上一页
                ui_form_stop_contorl_left_page.Opacity = 0
            Else
                ui_form_stop_contorl_left_page.Opacity = 1
            End If
            If toword(step_number + 1, 0) = "" Then
                '没有下一页
                ui_form_stop_contorl_right_page.Opacity = 0
            Else
                ui_form_stop_contorl_right_page.Opacity = 1
            End If
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]线路列表搜索字符更改
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_line_contorl_left_form_textbox_change(sender As Object, e As TextChangedEventArgs) Handles ui_form_line_contorl_left_form_textbox.TextChanged
        If ui_form_line_contorl_left_form_textbox.Text = "" Then
            '强制复原
            ui_form_line_list.ItemsSource = ui_connet_core_form_line_list
        Else
            '搜索并设置
            ui_form_line_list.ItemsSource = Nothing
            Dim a As New ui_depend_line_list
            ui_connet_core_form_line_list_for_search.Clear()

            For test1 = 0 To 1000
                If have_bus(test1) = "" Then
                    Exit For
                End If

                If InStr(have_bus(test1), ui_form_line_contorl_left_form_textbox.Text) = 0 Then
                    '没找到 
                    '什么都不做
                Else
                    '找到了
                    a.ui_line_name = have_bus(test1)
                    ui_connet_core_form_line_list_for_search.Add(a)
                    a = New ui_depend_line_list
                End If
            Next

            ui_form_line_list.ItemsSource = ui_connet_core_form_line_list_for_search

        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]站台列表搜索字符更改
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_stop_contorl_left_form_textbox_change(sender As Object, e As TextChangedEventArgs) Handles ui_form_stop_contorl_left_form_textbox.TextChanged
        If ui_form_stop_contorl_left_form_textbox.Text = "" Then
            '强制复原
            If ui_connet_core_form_stop_list.Count > 500 Then
                '太多了不显示，若显示会炸内存
                ui_form_stop_list.ItemsSource = Nothing
                ui_form_stop_contorl_full_list.Opacity = 1
            Else
                ui_form_stop_list.ItemsSource = ui_connet_core_form_stop_list
            End If
        Else
            '搜索并设置
            ui_form_stop_contorl_full_list.Opacity = 0
            ui_form_stop_list.ItemsSource = Nothing
            Dim a As New ui_depend_stop_list
            ui_connet_core_form_stop_list_for_search.Clear()

            For test1 = 0 To 10000
                If bus_stop_stop(test1) = "" Then
                    Exit For
                End If

                If InStr(bus_stop_stop(test1), ui_form_stop_contorl_left_form_textbox.Text) = 0 Then
                    '没找到 
                    '什么都不做
                Else
                    '找到了
                    a.ui_stop_name = bus_stop_stop(test1)
                    ui_connet_core_form_stop_list_for_search.Add(a)
                    a = New ui_depend_stop_list
                End If
            Next

            ui_form_stop_list.ItemsSource = ui_connet_core_form_stop_list_for_search
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]线路列表搜索含有站台字符更改
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_line_contorl_right_form_textbox_change(sender As Object, e As TextChangedEventArgs) Handles ui_form_line_contorl_right_form_textbox.TextChanged
        If ui_form_line_contorl_right_form_textbox.Text = "" Then
            '强制复原
            ui_form_line_stop_search_list.ItemsSource = Nothing
            ui_connet_core_form_line_search_stop_list.Clear()
            ui_form_line_stop_search_list.ItemsSource = ui_connet_core_form_line_search_stop_list
        Else
            ui_form_line_stop_search_list.ItemsSource = Nothing
            Dim a As New ui_depend_line_search_stop_list
            ui_connet_core_form_line_search_stop_list.Clear()

            For test1 = 0 To 200
                If bus_stop_up(test1) = "" Then
                    Exit For
                End If

                If InStr(bus_stop_up(test1), ui_form_line_contorl_right_form_textbox.Text) = 0 Then
                    '没找到 
                    '什么都不做
                Else
                    '找到了
                    a.ui_line_of_stop_name = bus_stop_up(test1)
                    a.ui_line_of_stop_name_on_where = "上行线路"
                    ui_connet_core_form_line_search_stop_list.Add(a)
                    a = New ui_depend_line_search_stop_list
                End If
            Next

            a = New ui_depend_line_search_stop_list

            For test1 = 0 To 200
                If bus_stop_down(test1) = "" Then
                    Exit For
                End If

                If InStr(bus_stop_down(test1), ui_form_line_contorl_right_form_textbox.Text) = 0 Then
                    '没找到 
                    '什么都不做
                Else
                    '找到了
                    a.ui_line_of_stop_name = bus_stop_down(test1)
                    a.ui_line_of_stop_name_on_where = "下行线路"
                    ui_connet_core_form_line_search_stop_list.Add(a)
                    a = New ui_depend_line_search_stop_list
                End If
            Next

            ui_form_line_stop_search_list.ItemsSource = ui_connet_core_form_line_search_stop_list
        End If

    End Sub

    ''' <summary>
    ''' [系统][ui]设置界面替换背景
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_contorl_list_4_function(sender As Object, e As MouseButtonEventArgs)
        If System.IO.File.Exists(ui_form_contorl_background_path.Text) = True Then
            System.IO.File.Delete(Environment.CurrentDirectory + "\temp.jpg")
            System.IO.File.Copy(ui_form_contorl_background_path.Text, Environment.CurrentDirectory + "\temp.jpg")
            Process.Start(Environment.CurrentDirectory + "\bus_rode_add.exe", "1")
            Environment.Exit(3)
        Else
            message_ex_ex("替换背景", "没有找到背景文件：" + ui_form_contorl_background_path.Text)
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]设置界面替换资源
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_contorl_list_2_function(sender As Object, e As MouseButtonEventArgs)
        If System.IO.File.Exists(ui_form_contorl_resource_path.Text) = True Then
            System.IO.File.Delete(Environment.CurrentDirectory + "\temp.brs")
            System.IO.File.Copy(ui_form_contorl_resource_path.Text, Environment.CurrentDirectory + "\temp.brs")
            Process.Start(Environment.CurrentDirectory + "\bus_rode_add.exe", "0")
            Environment.Exit(3)
        Else
            message_ex_ex("替换资源", "没有找到资源文件：" + ui_form_contorl_resource_path.Text)
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]打开 打开资源文件 对话框函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_contorl_throw_rs_file(sender As Object, e As MouseButtonEventArgs)
        open_resources_file.ShowDialog()
        If open_resources_file.FileName <> "" Then
            ui_form_contorl_resource_path.Text = open_resources_file.FileName
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]打开 打开背景图片 对话框函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_contorl_throw_bk_file(sender As Object, e As MouseButtonEventArgs)
        open_background_file.ShowDialog()
        If open_background_file.FileName <> "" Then
            ui_form_contorl_background_path.Text = open_background_file.FileName
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]站台设置界面添加起始站台的车次
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_stop_contorl_add_start_stop_fx(sender As Object, e As MouseButtonEventArgs) Handles ui_form_stop_contorl_add_start_stop.MouseDown
        ui_form_stop_contorl_right_form_textbox_1.Text = bus_stop_stop(now_stop)
    End Sub

    ''' <summary>
    ''' [系统][ui]站台设置界面添加终到站台的车次
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_stop_contorl_add_end_stop_fx(sender As Object, e As MouseButtonEventArgs) Handles ui_form_stop_contorl_add_end_stop.MouseDown
        ui_form_stop_contorl_right_form_textbox_2.Text = bus_stop_stop(now_stop)
    End Sub

    ''' <summary>
    ''' [系统][ui]设置面板讲述人按钮移动
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_function_contorl_check_btn_2(sender As Object, e As MouseButtonEventArgs) Handles ui_form_contorl_check_btn_2.MouseDown
        Dim stb_1
        storyboard_action.Stop()
        storyboard_action.Children.Clear()

        If talk_man = True Then
            '关闭
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(70, 0, New Duration(TimeSpan.FromSeconds(0.1)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_contorl_check_btn_2)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))

            storyboard_action.Children.Add(stb_1)
        Else
            '打开
            stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 70, New Duration(TimeSpan.FromSeconds(0.1)))

            Animation.Storyboard.SetTarget(stb_1, ui_form_contorl_check_btn_2)
            Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("(Canvas.Left)"))

            storyboard_action.Children.Add(stb_1)
        End If

        storyboard_action.Begin()

        '设置界面
        talk_man = Not (talk_man)

        '保存设置
        save_user_contorl()

        '刷新
        re_window()
    End Sub

    ''' <summary>
    ''' [系统][ui]设置界面选择插件路径
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_contorl_throw_mod_file(sender As Object, e As MouseButtonEventArgs)
        open_mod_file.ShowDialog()
        If open_mod_file.FileName <> "" Then
            ui_form_contorl_mod_path.Text = open_mod_file.FileName
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]设置界面应用插件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_contorl_list_7_function(sender As Object, e As MouseButtonEventArgs)
        If System.IO.File.Exists(ui_form_contorl_mod_path.Text) = True Then
            System.IO.File.Delete(Environment.CurrentDirectory + "\temp.dll")
            System.IO.File.Copy(ui_form_contorl_mod_path.Text, Environment.CurrentDirectory + "\temp.dll")
            Process.Start(Environment.CurrentDirectory + "\bus_rode_add.exe", "2")
            Environment.Exit(3)
        Else
            message_ex_ex("替换资源", "没有找到车辆跟踪插件文件：" + ui_form_contorl_mod_path.Text)
        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]设置界面插件信息
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_contorl_list_8_function(sender As Object, e As MouseButtonEventArgs)

        Try
            Dim ass As System.Reflection.Assembly = System.Reflection.Assembly.LoadFile(System.Environment.CurrentDirectory & "\bus_rode_mod.dll")
            Dim tp As Type = ass.GetType("bus_rode_dll.main_dll", True)
            Dim out_word As String = ""

            If tp <> Nothing Then
                Dim instance As Object = Activator.CreateInstance(tp)

                Dim prop_1 As Reflection.FieldInfo = tp.GetField("DllDependBusRodeVersion")
                out_word = "插件依赖的bus_rode版本号：" & CType(prop_1.GetValue(instance), String) & vbCrLf
                Dim prop_2 As Reflection.FieldInfo = tp.GetField("DllWriter")
                out_word = out_word & "插件作者：" & CType(prop_2.GetValue(instance), String) & vbCrLf
                Dim prop_3 As Reflection.FieldInfo = tp.GetField("DllRegoin")
                out_word = out_word & "插件适用的城市：" & CType(prop_3.GetValue(instance), String) & vbCrLf
                Dim prop_4 As Reflection.FieldInfo = tp.GetField("DllVersion")
                out_word = out_word & "插件版本号：" & CType(prop_4.GetValue(instance), String)

                window_dialogs_show("插件信息", "out_word", 0, 1, False, "确定", "", Application.Current.MainWindow)
            End If
        Catch ex As Exception
            window_dialogs_show("错误", "无法读取插件的相关信息，这可能是由于：" & vbCrLf &
                   "1、没有安装插件" & vbCrLf &
                   "2、插件损坏或者插件不完整没有通过CHMOSGroup的认证", 2, 1, False, "确定", "", Application.Current.MainWindow)
        End Try

    End Sub

    ''' <summary>
    ''' [系统][ui]窗体按下键盘的相关处理
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub app_keyboard(sender As Object, e As KeyEventArgs) Handles ui_bus_rode_main_window.KeyDown
        '启动讲述人
        If e.Key = Key.F1 And talk_man = True Then
            If run_talk_flag = True Then
                '不予执行
                message_ex_ex("讲述人", "讲述人正在朗读，请等待朗诵完毕再按下F1" + ui_form_contorl_resource_path.Text)
            Else
                run_talk_flag = True
            End If


        End If
    End Sub

    ''' <summary>
    ''' [系统][ui]联网搜索路径
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_stop_short_line_search_web_function(sender As Object, e As MouseButtonEventArgs)

        before_stamp_start_stop = stamp_start_stop
        before_stamp_end_stop = stamp_end_stop

        stamp_start_stop = ui_form_stop_contorl_right_form_textbox_1.Text
        stamp_end_stop = ui_form_stop_contorl_right_form_textbox_2.Text

        If My.Computer.Network.IsAvailable = False Then
            message_ex_ex("没有网络", "联网规划需要有效的网络连接才能进行，没有网络时请使用本地规划")
        Else

            If ui_form_stop_short_line_check() = True Then


                '可以执行，先下载文件
                Try
                    Dim tf_client As WebClient = New WebClient()
                    Dim tf_word As String = System.Text.Encoding.UTF8.GetString(tf_client.DownloadData("http://api.map.baidu.com/direction/v1?origin=" & stamp_start_stop & "&destination=" &
                                                                                                  stamp_end_stop & "&mode=transit&region=" & set_address_part & "&output=xml&ak=" & app_use_baidu_ak))

                    Dim file As New System.IO.StreamWriter(Environment.CurrentDirectory + "\get_short_rode.xml", False, System.Text.Encoding.UTF8)
                    file.Write(tf_word)
                    file.Dispose()
                Catch ex As Exception
                    '发生错误，调用浏览器
                    System.Diagnostics.Process.Start("http://api.map.baidu.com/direction?origin=" & stamp_start_stop &
                                                        "&origin_region=" & set_address_part & "&destination_region=" & set_address_part & "&destination=" &
                                                         stamp_end_stop & "&mode=transit&region=" & set_address_part & "&output=html&scr=bus_rode")

                    '强制退出
                    Exit Sub
                End Try

                '核心读取
                read_xml_main()

                '确认是否设置ui
                If toword(0, 0) <> "" Then
                    '设置ui
                    ui_form_stop_contorl_page.Text = "步骤1"

                    ui_form_stop_contorl_step_describe.Text = toword(0, 0)

                    ui_form_stop_contorl_left_page.Opacity = 0
                    If toword(1, 0) = "" Then
                        '没有下一页
                        ui_form_stop_contorl_right_page.Opacity = 0
                    Else
                        ui_form_stop_contorl_right_page.Opacity = 1
                    End If

                End If

                toword_local = False

            End If

        End If

    End Sub


#Region "列表鼠标进入/离开的滚动条有无"

    ''' <summary>
    ''' [系统][ui]线路-当前线路站台列表移入鼠标显示滚动条
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_line_have_stop_list_mouse_enter(sender As Object, e As MouseEventArgs) Handles ui_form_line_have_stop_list.MouseEnter
        ScrollViewer.SetHorizontalScrollBarVisibility(ui_form_line_have_stop_list, ScrollBarVisibility.Auto)
    End Sub

    ''' <summary>
    ''' [系统][ui]线路-当前线路站台列表移出鼠标不显示滚动条
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_line_have_stop_mouse_leave(sender As Object, e As MouseEventArgs) Handles ui_form_line_have_stop_list.MouseLeave
        ScrollViewer.SetHorizontalScrollBarVisibility(ui_form_line_have_stop_list, ScrollBarVisibility.Hidden)
    End Sub

    '**********************************************

    ''' <summary>
    ''' [系统][ui]线路-当前线路地铁出口列表移入鼠标显示滚动条
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_line_subway_list_mouse_enter(sender As Object, e As MouseEventArgs) Handles ui_form_line_subway_list.MouseEnter
        ScrollViewer.SetHorizontalScrollBarVisibility(ui_form_line_subway_list, ScrollBarVisibility.Auto)
    End Sub

    ''' <summary>
    ''' [系统][ui]线路-当前线路地铁出口列表移出鼠标不显示滚动条
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_line_subway_list_mouse_leave(sender As Object, e As MouseEventArgs) Handles ui_form_line_subway_list.MouseLeave
        ScrollViewer.SetHorizontalScrollBarVisibility(ui_form_line_subway_list, ScrollBarVisibility.Hidden)
    End Sub

    '**********************************************

    ''' <summary>
    ''' [系统][ui]站台-左侧车次列表移入鼠标显示滚动条
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_stop_left_cross_line_list_mouse_enter(sender As Object, e As MouseEventArgs) Handles ui_form_stop_left_cross_line_list.MouseEnter
        ScrollViewer.SetVerticalScrollBarVisibility(ui_form_stop_left_cross_line_list, ScrollBarVisibility.Auto)
    End Sub

    ''' <summary>
    ''' [系统][ui]站台-左侧车次列表移出鼠标不显示滚动条
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_stop_left_cross_line_list_mouse_leave(sender As Object, e As MouseEventArgs) Handles ui_form_stop_left_cross_line_list.MouseLeave
        ScrollViewer.SetVerticalScrollBarVisibility(ui_form_stop_left_cross_line_list, ScrollBarVisibility.Hidden)
    End Sub

    '**********************************************

    ''' <summary>
    ''' [系统][ui]站台-右侧车次列表移入鼠标显示滚动条
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_stop_right_cross_line_list_mouse_enter(sender As Object, e As MouseEventArgs) Handles ui_form_stop_right_cross_line_list.MouseEnter
        ScrollViewer.SetVerticalScrollBarVisibility(ui_form_stop_right_cross_line_list, ScrollBarVisibility.Auto)
    End Sub

    ''' <summary>
    ''' [系统][ui]站台-右侧车次列表移出鼠标不显示滚动条
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_stop_right_cross_line_list_mouse_leave(sender As Object, e As MouseEventArgs) Handles ui_form_stop_right_cross_line_list.MouseLeave
        ScrollViewer.SetVerticalScrollBarVisibility(ui_form_stop_right_cross_line_list, ScrollBarVisibility.Hidden)
    End Sub


#End Region


    ''' <summary>
    ''' [系统][ui]线路-经过站台列表跳转到站台界面的函数
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_line_have_stop_list_jump_to_stop(sender As Object, e As MouseButtonEventArgs) Handles ui_form_line_have_stop_list.MouseDoubleClick

        If ui_form_line_have_stop_list.SelectedIndex <> -1 Then

            '寻找当前站点
            Dim word As String = ""
            If up_or_down_line = True Then
                '上行
                word = ui_connet_core_form_line_have_sotp_list_up.Item(ui_form_line_have_stop_list.SelectedIndex).ui_stop_name
            Else
                '下行
                word = ui_connet_core_form_line_have_sotp_list_down.Item(ui_form_line_have_stop_list.SelectedIndex).ui_stop_name
            End If

            '确认索引
            Dim linshi_1 As Integer = return_check_stop_list_nopage(word)
            If linshi_1 <> -1 Then
                '清除旧的
                ui_form_stop_left_cross_line_list.ItemsSource = Nothing
                ui_form_stop_right_cross_line_list.ItemsSource = Nothing

                now_stop = linshi_1
                refsh_stop()
                jump_to_stop_from_line = True

                '设置文本
                ui_form_stop_describe.Text = cross_stop
                ui_form_stop_middle_stop_name.Text = bus_stop_stop(now_stop)

                ui_form_stop_left_cross_line_list.ItemsSource = ui_connet_core_form_stop_left_cross_line_list
                ui_form_stop_right_cross_line_list.ItemsSource = ui_connet_core_form_stop_right_cross_line_list

                '**************************动画跳转
                Dim stb_1
                storyboard_action.Stop()
                storyboard_action.Children.Clear()

                '切入新窗口
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(0, 1, New Duration(TimeSpan.FromSeconds(0.3)))

                Animation.Storyboard.SetTarget(stb_1, ui_form_stop)
                Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                storyboard_action.Children.Add(stb_1)
                '移动旧窗口
                stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.3)))

                Animation.Storyboard.SetTarget(stb_1, ui_form_line)
                Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))

                storyboard_action.Children.Add(stb_1)

                '控制面板
                If line_contorl = True Then
                    stb_1 = New System.Windows.Media.Animation.DoubleAnimation(1, 0, New Duration(TimeSpan.FromSeconds(0.15)))

                    Animation.Storyboard.SetTarget(stb_1, ui_form_line_contorl)
                    Animation.Storyboard.SetTargetProperty(stb_1, New PropertyPath("Opacity"))
                    Dim linshi = New Animation.ExponentialEase()
                    linshi.EasingMode = Animation.EasingMode.EaseOut
                    stb_1.EasingFunction = linshi

                    storyboard_action.Children.Add(stb_1)
                End If

                '设置界面
                screens = 2
                line_contorl = False

                '****************************开始跳转
                storyboard_action.Begin()

                '刷新
                re_window()

            Else
                '发生错误，退出
                Environment.Exit(9)
            End If


        End If

    End Sub

    ''' <summary>
    ''' [系统][ui]关于界面前往github的按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ui_form_about_github(sender As Object, e As MouseButtonEventArgs)
        System.Diagnostics.Process.Start("https://github.com/yyc12345/bus_rode")
    End Sub
End Class

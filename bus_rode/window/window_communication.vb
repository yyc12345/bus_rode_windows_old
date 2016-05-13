Module window_communication

    '主窗口和附属窗口之间的信息交流

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
    Public Declare Sub ReleaseCapture Lib "User32" ()

#Region "*************************window_dialogs*********************************"

    ''' <summary>
    ''' window_dialogs窗口的标题
    ''' </summary>
    Public window_dialogs_title As String = ""
    ''' <summary>
    ''' window_dialogs窗口的具体内容
    ''' </summary>
    Public window_dialogs_message As String = ""
    ''' <summary>
    ''' window_dialogs窗口左侧按钮文本
    ''' </summary>
    Public window_dialogs_left_btn_text As String = ""
    ''' <summary>
    ''' window_dialogs窗口右侧按钮文本
    ''' </summary>
    Public window_dialogs_right_btn_text As String = ""
    ''' <summary>
    ''' window_dialogs窗口选择的按钮，0表示左侧，1表示右侧
    ''' </summary>
    Public window_dialogs_select_btn As Byte = 0
    ''' <summary>
    ''' window_dialogs窗口显示几个按钮，允许数值1-2，写1只显示原左侧按钮
    ''' </summary>
    Public window_dialogs_btn_count As Byte = 1

    ''' <summary>
    ''' window_dialogs窗口是否需要接受输入的数据
    ''' </summary>
    Public window_dialogs_show_inputbox As Boolean = False
    ''' <summary>
    ''' window_dialogs窗口输入的文本，如果有的话
    ''' </summary>
    Public window_dialogs_input_text As String = ""


    ''' <summary>
    ''' 清理window_dialogs的各项内容
    ''' </summary>
    Public Sub window_dialogs_clear()
        window_dialogs_title = ""
        window_dialogs_message = ""
        window_dialogs_left_btn_text = ""
        window_dialogs_right_btn_text = ""
        window_dialogs_input_text = ""
    End Sub

    ''' <summary>
    ''' 显示一个window_dialogs对话框
    ''' </summary>
    ''' <param name="title">标题</param>
    ''' <param name="message">消息</param>
    ''' <param name="btn_count">窗口显示几个按钮，允许数值1-2，写1只显示原左侧按钮</param>
    ''' <param name="show_input">窗口是否需要接受输入的数据</param>
    ''' <param name="left_btn_text">窗口左侧按钮文本</param>
    ''' <param name="right_btn_text">窗口右侧按钮文本</param>
    Public Sub window_dialogs_show(ByVal title As String, ByVal message As String, ByVal btn_count As Byte,
                                   ByVal show_input As Boolean, ByVal left_btn_text As String, ByVal right_btn_text As String)

        '<param name="owner">传递的主窗体</param>
        'Public Sub window_dialogs_show(ByVal title As String, ByVal message As String, ByVal msg_state As Byte, ByVal btn_count As Byte,
        '                       ByVal show_input As Boolean, ByVal left_btn_text As String, ByVal right_btn_text As String, ByRef owner As Window)

        window_dialogs_clear()

        window_dialogs_title = title
        window_dialogs_message = message
        window_dialogs_btn_count = btn_count
        window_dialogs_show_inputbox = show_input
        window_dialogs_left_btn_text = left_btn_text
        window_dialogs_right_btn_text = right_btn_text

        Dim linshi As New Window_dialogs
        Try
            linshi.Owner = Application.Current.MainWindow
        Catch ex As Exception
            linshi.Owner = Nothing
        End Try

        linshi.ShowDialog()


    End Sub

#End Region

End Module

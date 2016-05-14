
Module screen

    ''' <summary>
    ''' 窗口的宽
    ''' </summary>
    ''' <remarks></remarks>
    Public window_x As Integer = 800
    ''' <summary>
    ''' 窗口的高
    ''' </summary>
    ''' <remarks></remarks>
    Public window_y As Integer = 600

    ''' <summary>
    ''' 指示当前窗口正在处于哪一个窗口状态之中
    ''' 
    ''' 0=开始界面 1=线路界面 2=车站界面 3=设置界面 4=关于界面
    ''' </summary>
    ''' <remarks></remarks>
    Public screens As Integer = 0
    ''' <summary>
    ''' 指示线路控制界面是否显示
    ''' </summary>
    ''' <remarks></remarks>
    Public line_contorl As Boolean = False
    ''' <summary>
    ''' 指示线路控制界面显示的面版 true=page left   false=page right
    ''' </summary>
    ''' <remarks></remarks>
    Public line_contorl_page As Boolean = True
    ''' <summary>
    ''' 指示车站控制界面是否显示
    ''' </summary>
    ''' <remarks></remarks>
    Public stop_contorl As Boolean = False
    ''' <summary>
    ''' 指示站台控制界面显示的面版 true=page left   false=page right
    ''' </summary>
    ''' <remarks></remarks>
    Public stop_contorl_page As Boolean = True
    ''' <summary>
    ''' 指示消息界面是否显示
    ''' </summary>
    ''' <remarks></remarks>
    Public message_show As Boolean = False


    '************************************************************************************************
    Public ui_connet_core_form_start_ring_list As New List(Of ui_depend_start_ring_list)
    ''' <summary>
    ''' 线路界面中线路所拥有站台的列表-上行中项的集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_line_have_sotp_list_up As New List(Of ui_depend_line_stop_list)
    ''' <summary>
    ''' 线路界面中线路所拥有站台的列表-下行中项的集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_line_have_sotp_list_down As New List(Of ui_depend_line_stop_list)
    ''' <summary>
    ''' 线路界面中线路所拥有站台的列表-用于展示的集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_line_have_sotp_list_show As New List(Of ui_depend_line_stop_list)
    ''' <summary>
    ''' 线路界面中地铁线路出口列表中项的集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_line_subway_list As New List(Of ui_depend_line_subway_list)
    ''' <summary>
    ''' 线路界面中线路列表中项的集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_line_list As New List(Of ui_depend_line_list)
    ''' <summary>
    ''' 线路界面中线路列表中项的集合_用于搜索
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_line_list_for_search As New List(Of ui_depend_line_list)
    ''' <summary>
    ''' 线路界面中搜索站台列表中项的集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_line_search_stop_list As New List(Of ui_depend_line_search_stop_list)

    '************************************************************************************************
    ''' <summary>
    ''' 车站界面中附近站台经过车次列表中项的集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_stop_cross_line_list As New List(Of ui_depend_stop_cross_line_list)
    ''' <summary>
    ''' 车站界面中真实站台列表中项的集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_stop_realistic_stop_list As New List(Of ui_depend_stop_realistic_stop_list)
    ''' <summary>
    ''' 车站界面中车站列表中项的集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_stop_list As New List(Of ui_depend_stop_list)
    ''' <summary>
    ''' 车站界面中车站列表中项的集合_用于搜索
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_stop_list_for_search As New List(Of ui_depend_stop_list)


    '************************************************************************************************
    ''' <summary>
    ''' 消息界面中消息列表中项的集合
    ''' </summary>
    ''' <remarks></remarks>
    Public ui_connet_core_form_message_list As New List(Of ui_depend_message_list)

End Module

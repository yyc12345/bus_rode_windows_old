Public Class bus_rode_check

    ''' <summary>
    ''' 检查资源是否合理
    ''' </summary>
    ''' <param name="library_path">检查资源的启动路径的绝对路径，包含 \ </param>
    ''' <param name="describe">需要输出的错误描述文本</param>
    ''' <returns></returns>
    Public Shared Function check_file(ByVal library_path As String, ByRef describe As String) As Boolean
        Dim end_result As Boolean = True
        Dim return_word As String = ""

        If System.IO.File.Exists(library_path + "file.bat") = False Then
            end_result = False
            describe = describe + "file.bat" + vbCrLf
        End If
        If System.IO.File.Exists(library_path + "desktop.txt") = False Then
            end_result = False
            describe = describe + "desktop.txt" + vbCrLf
        End If
        If System.IO.File.Exists(library_path + "bus_rode_compression.dll") = False Then
            end_result = False
            describe = describe + "bus_rode_compression.dll---严重错误" + vbCrLf
        End If
        If System.IO.File.Exists(library_path + "bus_rode_add.exe") = False Then
            end_result = False
            describe = describe + "bus_rode_add.exe---严重错误" + vbCrLf
        End If

        describe = return_word + "缺失以上文件，导致程序无法启动，请检查相关文件是否完整，如若不确定，请重新安装"
        Return end_result
    End Function

    ''' <summary>
    ''' 检查是否存在资源文件。返回0，资源存在，有最短路径文件；返回1，资源存在，没有line最短路径文件；返回2，资源存在，没有stop最短路径文件；返回3，资源存在，没有line和stop最短路径文件；返回4，资源不存在或部分不存在
    ''' </summary>
    ''' <param name="library_path">检查资源的library绝对路径，包含 \ </param>
    ''' <returns></returns>
    Public Shared Function have_file(ByVal library_path As String) As Integer
        Dim return_value As Integer = 0

        If System.IO.File.Exists(library_path + "bus.txt") = False Then
            return_value = 4
            GoTo return_area
        End If
        If System.IO.File.Exists(library_path + "have_bus.txt") = False Then
            return_value = 4
            GoTo return_area
        End If
        If System.IO.File.Exists(library_path + "readme.txt") = False Then
            return_value = 4
            GoTo return_area
        End If
        If System.IO.File.Exists(library_path + "station.txt") = False Then
            return_value = 4
            GoTo return_area
        End If
        If System.IO.File.Exists(library_path + "stop.txt") = False Then
            return_value = 4
            GoTo return_area
        End If
        If System.IO.File.Exists(library_path + "subway.txt") = False Then
            return_value = 4
            GoTo return_area
        End If

        '检查最短路径
        If System.IO.File.Exists(library_path + "short_line.txt") = False Then
            return_value = 1
        End If
        If System.IO.File.Exists(library_path + "short_stop.txt") = False Then

            If return_value = 1 Then
                '都不存在
                return_value = 3
            Else
                '仅stop不存在
                return_value = 2
            End If

        End If

return_area:
        Return return_value
    End Function


End Class

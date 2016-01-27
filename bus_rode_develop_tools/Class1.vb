Public Class bus_rode_build_bus_to_station
    Public Shared bus_stop(1000) As String
    Public Shared bus_line(1000) As String

    '旧文件
    Public Shared bus_stop_old(1000) As String
    Public Shared bus_line_old(1000, 4) As String

    '是否抄写旧的文件
    Public Shared have_old_station As Boolean = False
    '旧文件指针
    Public Shared old_station_list As Integer = 0

    Public Shared Sub start()
        If System.IO.File.Exists(System.Environment.CurrentDirectory + "\bus.txt") Then
            Dim file As New System.IO.StreamReader(System.Environment.CurrentDirectory + "\bus.txt", System.Text.Encoding.UTF8)
            Dim key As New System.IO.StreamWriter(System.Environment.CurrentDirectory + "\station.txt.new", False, System.Text.Encoding.UTF8)

            Dim list As Integer = 0
            Dim line As String = ""
            Dim word As String = ""
            Dim word_last As String = ""
            Dim word_frist As String = ""

            Dim after_line_start As String = ""
            Dim after_line_end As String = ""
            Dim last_line_start As String = ""
            Dim last_line_end As String = ""

            For test1 = 0 To 1000
                bus_stop(test1) = ""
                bus_line(test1) = ""
                bus_stop_old(test1) = ""
                For test2 = 0 To 4
                    bus_line_old(test1, test2) = ""
                Next
            Next

            '检测是否存在旧的文件,并读取
            If System.IO.File.Exists(System.Environment.CurrentDirectory + "\station.txt") Then
                have_old_station = True
            Else
                have_old_station = False
                Dim file_test As New System.IO.StreamWriter(System.Environment.CurrentDirectory + "\station.txt", False, System.Text.Encoding.UTF8)
                file_test.WriteLine("")
                file_test.Dispose()
            End If
            Dim file_old As New System.IO.StreamReader(System.Environment.CurrentDirectory + "\station.txt", System.Text.Encoding.UTF8)

            Do
                word = file_old.ReadLine
                If word = "" Then
                    Exit Do
                Else
                    bus_stop_old(list) = word
                    '放过经过车次
                    file_old.ReadLine()
                    bus_line_old(list, 0) = file_old.ReadLine
                    bus_line_old(list, 1) = file_old.ReadLine
                    bus_line_old(list, 2) = file_old.ReadLine
                    bus_line_old(list, 3) = file_old.ReadLine
                    '放过end
                    file_old.ReadLine()

                    list = list + 1
                End If
            Loop
            file_old.Dispose()



            list = 0
            line = ""
            word = ""
            word_last = ""
            word_frist = ""

            Do

                word = file.ReadLine
                If word = "ENDDATE" Then
                    Exit Do

                Else
                    line = word
                    '读空值
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

                    word_frist = file.ReadLine
                    Do
                        word = file.ReadLine
                        If word = "ENDLINE" Then
                            Exit Do
                        End If
                        word_last = word
                    Loop
                    after_line_start = word_frist
                    after_line_end = word_last

                    word_frist = file.ReadLine
                    If word_frist <> "END" Then
                        Do
                            word = file.ReadLine
                            If word = "END" Then
                                Exit Do
                            End If
                            word_last = word
                        Loop
                        last_line_start = word_frist
                        last_line_end = word_last
                    End If

                    Dim aaa As ArrayList = New ArrayList
                    aaa.Add(after_line_start)
                    If aaa.Contains(after_line_end) = False Then
                        aaa.Add(after_line_end)
                    End If
                    If aaa.Contains(last_line_start) = False And last_line_start <> "" Then
                        aaa.Add(last_line_start)
                    End If
                    If aaa.Contains(last_line_end) = False And last_line_end <> "" Then
                        aaa.Add(last_line_end)
                    End If

                    For a = 0 To aaa.Count - 1
                        If check(CType(aaa.Item(a), String)) Then
                            bus_line(check_number(CType(aaa.Item(a), String))) = bus_line(check_number(CType(aaa.Item(a), String))) + line + " "
                        Else
                            bus_stop(list) = CType(aaa.Item(a), String)
                            bus_line(list) = bus_line(list) + line + " "
                            list = list + 1
                        End If
                    Next

                End If
            Loop

            Dim ok As Integer = 0

            Do
                If bus_stop(ok) = "" Then
                    Exit Do
                End If

                key.WriteLine(bus_stop(ok))
                key.WriteLine("车次:" + bus_line(ok))
                '分辨是否使用旧文件
                If have_old_station = True Then
                    If get_old_station(bus_stop(ok)) Then
                        key.WriteLine(bus_line_old(old_station_list, 0))
                        key.WriteLine(bus_line_old(old_station_list, 1))
                        key.WriteLine(bus_line_old(old_station_list, 2))
                        key.WriteLine(bus_line_old(old_station_list, 3))
                    Else
                        key.WriteLine("调度员:无")
                        key.WriteLine("地址:无")
                        key.WriteLine("类别:临时停放")
                        key.WriteLine("描述:无")
                    End If
                Else
                    key.WriteLine("调度员:无")
                    key.WriteLine("地址:无")
                    key.WriteLine("类别:临时停放")
                    key.WriteLine("描述:无")
                End If
                key.WriteLine("END")
                ok = ok + 1
            Loop


            file.Dispose()
            key.Dispose()

            System.IO.File.Delete(System.Environment.CurrentDirectory + "\station.txt")
            System.IO.File.Move(System.Environment.CurrentDirectory + "\station.txt.new", System.Environment.CurrentDirectory + "\station.txt")

        Else
            MsgBox("can't find file!", 16)

        End If

    End Sub

    Public Shared Function check(ByVal name As String)
        Dim yes As Boolean = False
        For test1 = 0 To 1000
            If bus_stop(test1) = "" Then
                Exit For
            Else
                If bus_stop(test1) = name Then
                    yes = True
                End If
            End If
        Next

        Return yes
    End Function
    Public Shared Function check_number(ByVal name As String)
        Dim yes As Integer = 0
        For test1 = 0 To 1000
            If bus_stop(test1) = "" Then
                Exit For
            Else
                If bus_stop(test1) = name Then
                    yes = test1
                End If
            End If
        Next

        Return yes
    End Function

    Public Shared Function get_old_station(ByVal name As String)
        Dim yes As Boolean = False

        For test1 = 0 To 100
            If bus_stop_old(test1) = "" Then
                Exit For
            Else
                If bus_stop_old(test1) = name Then
                    yes = True
                    old_station_list = test1
                End If
            End If
        Next

        Return yes
    End Function

End Class

Public Class bus_rode_build_bus_to_stop

    Public Shared bus_stop(10000) As String
    Public Shared bus_line(10000) As String

    Public Shared Sub start()

        If System.IO.File.Exists(System.Environment.CurrentDirectory + "\bus.txt") Then
            Dim file As New System.IO.StreamReader(System.Environment.CurrentDirectory + "\bus.txt", System.Text.Encoding.UTF8)
            Dim key As New System.IO.StreamWriter(System.Environment.CurrentDirectory + "\stop.txt", False, System.Text.Encoding.UTF8)

            For test1 = 0 To 10000
                bus_stop(test1) = ""
                bus_line(test1) = ""
            Next

            Dim list As Integer = 0
            Dim line As String = ""
            Dim word As String = ""

            Do

                word = file.ReadLine
                If word = "ENDDATE" Then
                    Exit Do

                Else
                    line = word
                    '读空值
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
                        word = file.ReadLine
                        If word = "ENDLINE" Then
                            Exit Do
                        End If

                        If check(word) Then
                            If check_word_have_bus(bus_line(check_number(word)), line) = False Then
                                bus_line(check_number(word)) = bus_line(check_number(word)) + line + " "
                            End If

                        Else
                            bus_stop(list) = word
                            bus_line(list) = bus_line(list) + line + " "
                            list = list + 1
                        End If
                    Loop

                    Do
                        word = file.ReadLine
                        If word = "END" Then
                            Exit Do
                        End If

                        If check(word) Then
                            If check_word_have_bus(bus_line(check_number(word)), line) = False Then
                                bus_line(check_number(word)) = bus_line(check_number(word)) + line + " "
                            End If

                        Else
                            bus_stop(list) = word
                            bus_line(list) = bus_line(list) + line + " "
                            list = list + 1
                        End If
                    Loop

                End If
            Loop

            Dim ok As Integer = 0

            Do
                If bus_stop(ok) = "" Then
                    Exit Do
                End If
                key.WriteLine(bus_stop(ok))
                key.WriteLine(bus_line(ok))
                key.WriteLine("END")
                ok = ok + 1
            Loop


            file.Dispose()
            key.Dispose()

        Else
            MsgBox("can't find file!", 16)

        End If

    End Sub

    Public Shared Function check(ByVal name As String)
        Dim yes As Boolean = False
        For test1 = 0 To 10000
            If bus_stop(test1) = "" Then
                Exit For
            Else
                If bus_stop(test1) = name Then
                    yes = True
                End If
            End If
        Next

        Return yes
    End Function
    Public Shared Function check_number(ByVal name As String)
        Dim yes As Integer = 0
        For test1 = 0 To 10000
            If bus_stop(test1) = "" Then
                Exit For
            Else
                If bus_stop(test1) = name Then
                    yes = test1
                End If
            End If
        Next

        Return yes
    End Function

    Public Shared Function check_word_have_bus(ByVal word_arr_word As String, ByVal word As String)
        Dim yes As Boolean = False

        If word_arr_word <> "" Then
            Dim word_arr() As String = word_arr_word.Split(" ")

            For a = 0 To word_arr.Count - 2
                If word_arr(a) = word Then
                    '有了
                    yes = True
                    Exit For
                End If
            Next
        Else

        End If

        Return yes
    End Function

End Class

Public Class bus_rode_build_bus_to_subway
    Public Shared bus_stop(1000) As String
    Public Shared remember As Boolean = False

    Public Shared Sub start()

        If System.IO.File.Exists(System.Environment.CurrentDirectory + "\bus.txt") Then

            '检测记录文档
            If System.IO.File.Exists(System.Environment.CurrentDirectory + "\subway.txt") = True Then
                remember = True
                System.IO.File.Copy(System.Environment.CurrentDirectory + "\subway.txt", System.Environment.CurrentDirectory + "\subway.txt.old")
            End If


            Dim file As New System.IO.StreamReader(System.Environment.CurrentDirectory + "\bus.txt", System.Text.Encoding.UTF8)
            Dim key As New System.IO.StreamWriter(System.Environment.CurrentDirectory + "\subway.txt", False, System.Text.Encoding.UTF8)

            For test1 = 0 To 1000
                bus_stop(test1) = ""
            Next


            Dim list As Integer = 0
            Dim word As String = ""
            Dim linshi As String = ""

            Do

                word = file.ReadLine
                If word = "ENDDATE" Then
                    Exit Do

                Else
                    '读空值
                    file.ReadLine()
                    file.ReadLine()
                    file.ReadLine()
                    file.ReadLine()
                    file.ReadLine()

                    '检测属性
                    word = file.ReadLine
                    If word = "0" Then
                        '不是的，循环到这个车次结束
                        file.ReadLine()
                        file.ReadLine()
                        file.ReadLine()
                        file.ReadLine()
                        file.ReadLine()

                        Do
                            word = file.ReadLine
                            If word = "END" Then
                                Exit Do
                            End If
                        Loop

                    Else
                        '读空
                        file.ReadLine()
                        file.ReadLine()
                        file.ReadLine()
                        file.ReadLine()
                        file.ReadLine()

                        '取值
                        Do
                            word = file.ReadLine
                            If word = "ENDLINE" Then
                                Exit Do
                            End If

                            If check(word) = True Then
                                '有了，不写
                            Else
                                '没有，写上
                                bus_stop(check_list()) = word
                                '检测记录
                            End If
                        Loop

                        Do
                            word = file.ReadLine
                            If word = "END" Then
                                Exit Do
                            End If

                            If check(word) = True Then
                                '有了，不写
                            Else
                                '没有，写上
                                bus_stop(check_list()) = word
                                '检测记录
                            End If
                        Loop

                    End If


                End If
            Loop

            '写文档

            Do
                If bus_stop(list) = "" Then
                    Exit Do
                End If

                '写主体
                key.WriteLine(bus_stop(list))
                '写记录
                linshi = get_remember(bus_stop(list))
                If linshi <> "" Then
                    key.WriteLine(linshi)
                Else
                    key.WriteLine("示例出口")
                    key.WriteLine("通向遥远的地方")
                End If
                key.WriteLine("END")

                list += 1
            Loop

            file.Dispose()
            key.Dispose()

            System.IO.File.Delete(System.Environment.CurrentDirectory + "\subway.txt.old")

        Else
            MsgBox("can't find file!", 16)

        End If

    End Sub

    Public Shared Function check(ByVal name As String)
        Dim yes As Boolean = False
        For test1 = 0 To 1000
            If bus_stop(test1) = "" Then
                Exit For
            Else
                If bus_stop(test1) = name Then
                    yes = True
                End If
            End If
        Next

        Return yes
    End Function
    Public Shared Function check_list()
        Dim yes As Integer = 0
        For test1 = 0 To 1000
            If bus_stop(test1) = "" Then
                yes = test1
                Exit For
            End If
        Next

        Return yes
    End Function

    Public Shared Function get_remember(ByVal name As String)
        Dim yes As String = ""

        If remember = True Then
            Dim old_file As New System.IO.StreamReader(System.Environment.CurrentDirectory + "\subway.txt.old", System.Text.Encoding.UTF8)
            Dim word As String = ""
            Do
                word = old_file.ReadLine
                If word = "" Then
                    '没了
                    Exit Do
                End If

                If word = name Then
                    '是要找的信息
                    Do
                        word = old_file.ReadLine
                        If word = "END" Then
                            Exit Do
                        Else
                            If yes <> "" Then
                                yes = yes + vbCrLf
                            End If
                        End If

                        yes = yes + word

                    Loop
                Else
                    '不是要找的信息,循环到结尾
                    Do
                        word = old_file.ReadLine
                        If word = "END" Then
                            Exit Do
                        End If
                    Loop
                End If

            Loop


            old_file.Dispose()
        End If

        Return yes
    End Function
End Class



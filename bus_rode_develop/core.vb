Module core

    '工作模式 0=没有打开文件 1=已经打开文件
    Public work_mode As Integer = 0
    '编译状态 0=没有编译 1=已经编译
    Public build_mode As Integer = 0

    '指定项目的路径,带\符号
    Public project_path As String = ""

    '预设
    Public desktop(9) As String

    '内存中的数据备份
    '站台(站台序列，0=名称 1=有无图片 => 0=无 1=有)
    Public back_stop(1000, 1) As String
    '总站(站台序列，0=名称 1-4各个叙述)
    Public back_station(500, 4) As String
    '地铁(地铁序列，出口文字)
    Public back_subway(1000, 1) As String

    '描述出口
    Public back_exit(20, 1) As String

    '以当前工作模式和编译模式刷新工作区
    Public Sub re_work_are()

        If work_mode = 0 Then
            Form1.新建工程ToolStripMenuItem.Enabled = True
            Form1.打开工程ToolStripMenuItem.Enabled = True

            Form1.TabControl1.Visible = False
            Form1.编译ToolStripMenuItem.Enabled = False
            Form1.保存工程ToolStripMenuItem.Enabled = False
            Form1.关闭工程ToolStripMenuItem.Enabled = False
        Else
            Form1.新建工程ToolStripMenuItem.Enabled = False
            Form1.打开工程ToolStripMenuItem.Enabled = False

            Form1.TabControl1.Visible = True
            Form1.编译ToolStripMenuItem.Enabled = True
            Form1.保存工程ToolStripMenuItem.Enabled = True
            Form1.关闭工程ToolStripMenuItem.Enabled = True
            If build_mode = 0 Then
                Form1.打包资源ToolStripMenuItem.Enabled = False

                Form1.TabPage3.Parent = Nothing
                Form1.TabPage4.Parent = Nothing
            Else
                Form1.打包资源ToolStripMenuItem.Enabled = True

                Form1.TabPage3.Parent = Form1.TabControl1
                Form1.TabPage4.Parent = Form1.TabControl1
            End If
        End If

    End Sub

    '检查是否编译
    Public Function check_build()
        Dim result As Boolean = True

        If Not (System.IO.File.Exists(project_path + "subway.txt")) Then
            result = False
        End If
        If Not (System.IO.File.Exists(project_path + "stop.txt")) Then
            result = False
        End If
        If Not (System.IO.File.Exists(project_path + "station.txt")) Then
            result = False
        End If

        Return result
    End Function

    '保存线路数据
    Public Sub save_line(ByVal file_name As String)
        Dim file_new As New System.IO.StreamWriter(project_path + "bus.txt.new", False, System.Text.Encoding.UTF8)
        Dim file As New System.IO.StreamReader(project_path + "bus.txt", System.Text.Encoding.UTF8)

        '写入头部
        file_new.WriteLine(Form1.TextBox2.Text)

        file_new.WriteLine(Form1.TextBox3.Text)
        file_new.WriteLine(Form1.TextBox4.Text)
        file_new.WriteLine(Form1.TextBox5.Text)
        file_new.WriteLine(Form1.TextBox6.Text)
        file_new.WriteLine(Form1.TextBox7.Text)

        file_new.WriteLine(Form1.ComboBox1.SelectedIndex)
        file_new.WriteLine(Form1.ComboBox2.SelectedIndex)

        file_new.WriteLine(Form1.NumericUpDown1.Value.ToString)
        file_new.WriteLine(Form1.NumericUpDown2.Value.ToString)
        file_new.WriteLine(Form1.NumericUpDown4.Value.ToString)
        file_new.WriteLine(Form1.NumericUpDown3.Value.ToString)

        file_new.WriteLine(Form1.TextBox20.Text)

        '不换行
        file_new.WriteLine("ENDLINE")

        If Form1.TextBox10.Text <> "" Then
            file_new.WriteLine(Form1.TextBox10.Text)

        End If
        '不换行
        file_new.WriteLine("END")

        '复制文件
        Dim wordd As String = ""
        Do
            wordd = file.ReadLine
            If wordd = "ENDDATE" Then
                file_new.WriteLine("ENDDATE")
                Exit Do
            End If

            '检测到要删除的项目
            If wordd = Form1.TextBox2.Text Then
                '读空
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
                    wordd = file.ReadLine
                    If wordd = "END" Then
                        Exit Do
                    End If
                Loop
            Else
                '不是的->写入
                file_new.WriteLine(wordd)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)

                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)

                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                file_new.WriteLine(file.ReadLine)
                Do
                    wordd = file.ReadLine
                    If wordd = "END" Then
                        file_new.WriteLine("END")
                        Exit Do
                    End If

                    file_new.WriteLine(wordd)
                Loop
            End If
        Loop


        file.Dispose()
        file_new.Dispose()

        System.IO.File.Delete(project_path + "bus.txt")
        System.IO.File.Move(project_path + "bus.txt.new", project_path + "bus.txt")


    End Sub

    '读取线路主文档
    Public Sub read_line_name()
        Dim file As New System.IO.StreamReader(project_path + "have_bus.txt", System.Text.Encoding.UTF8)

        Dim word As String = ""
        Do
            word = file.ReadLine
            If word = "END" Then
                Exit Do
            End If

            Form1.ListBox1.Items.Add(word)

        Loop

        file.Dispose()
    End Sub

    '读取线路数据
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
                Form1.TextBox2.Text = word

                Form1.TextBox3.Text = file.ReadLine
                Form1.TextBox4.Text = file.ReadLine
                Form1.TextBox5.Text = file.ReadLine
                Form1.TextBox6.Text = file.ReadLine
                Form1.TextBox7.Text = file.ReadLine


                If Int(file.ReadLine) = 0 Then
                    Form1.ComboBox1.SelectedIndex = 0
                Else
                    Form1.ComboBox1.SelectedIndex = 1
                End If

                Form1.ComboBox2.SelectedIndex = Int(file.ReadLine)


                Form1.NumericUpDown1.Value = Int(file.ReadLine)
                Form1.NumericUpDown2.Value = Int(file.ReadLine)
                Form1.NumericUpDown4.Value = Int(file.ReadLine)
                Form1.NumericUpDown3.Value = Int(file.ReadLine)

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

                Form1.TextBox20.Text = all_text
                Form1.TextBox10.Text = all_text_2

                '退出
                Exit Do
            End If
        Loop

        file.Dispose()

    End Sub

    '读取其他
    Public Sub read_another()
        If System.IO.File.Exists(project_path + "readme.txt") = True Then
            Dim file As New System.IO.StreamReader(project_path + "readme.txt", System.Text.Encoding.UTF8)
            Dim word As String = file.ReadLine

            Dim word_arr() As String = word.Split("-")

            If word_arr.Count = 3 Then
                Form1.TextBox21.Text = word_arr(0)
                Form1.TextBox22.Text = word_arr(1)
                Form1.TextBox23.Text = word_arr(2)
            End If

            Form1.TextBox19.Text = file.ReadLine
            Form1.TextBox24.Text = file.ReadLine

            file.Dispose()
        End If
        If System.IO.File.Exists(project_path + "download.txt") = True Then
            Dim file_1 As New System.IO.StreamReader(project_path + "download.txt", System.Text.Encoding.UTF8)
            Form1.TextBox17.Text = file_1.ReadLine
            Form1.TextBox18.Text = file_1.ReadLine
            file_1.Dispose()
        End If

    End Sub

    '读取预设
    Public Sub read_desktop()
        Dim file As New System.IO.StreamReader(Application.StartupPath + "\desktop.txt", System.Text.Encoding.UTF8)

        'line里的5个设置
        desktop(0) = file.ReadLine
        desktop(1) = file.ReadLine
        desktop(2) = file.ReadLine
        desktop(3) = file.ReadLine
        desktop(4) = file.ReadLine

        'stop
        desktop(5) = file.ReadLine
        desktop(6) = file.ReadLine
        desktop(7) = file.ReadLine
        desktop(8) = file.ReadLine

        file.Dispose()
    End Sub
    Public Sub save_desktop()
        Dim file As New System.IO.StreamWriter(Application.StartupPath + "\desktop.txt", False, System.Text.Encoding.UTF8)

        file.WriteLine(desktop(0))
        file.WriteLine(desktop(1))
        file.WriteLine(desktop(2))
        file.WriteLine(desktop(3))
        file.WriteLine(desktop(4))
        file.WriteLine(desktop(5))
        file.WriteLine(desktop(6))
        file.WriteLine(desktop(7))
        file.WriteLine(desktop(8))

        file.Dispose()
    End Sub

    '读取除线路外的所有核心数据
    Public Sub read_more_date()
        Dim list As Integer = 0
        Dim word As String = ""

        'station
        Dim file1 As New System.IO.StreamReader(project_path + "station.txt", System.Text.Encoding.UTF8)

        Do
            word = file1.ReadLine
            If word = "" Then
                Exit Do
            End If

            back_station(list, 0) = word
            '对控件输出
            Form1.ListBox4.Items.Add(word)

            'jump
            file1.ReadLine()

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
        'subway
        Dim file2 As New System.IO.StreamReader(project_path + "subway.txt", System.Text.Encoding.UTF8)

        Do
            word = file2.ReadLine
            If word = "" Then
                If Form1.ListBox5.Items.Count = 0 Then
                    '封堵没有地铁的情况
                    Form1.TabPage4.Parent = Nothing
                End If
                Exit Do
            End If

            back_subway(list, 0) = word
            '对控件输出
            Form1.ListBox5.Items.Add(word)
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
        Form1.ListBox4.Text = Form1.ListBox4.Items.Item(0)
        If Form1.ListBox5.Items.Count <> 0 Then
            Form1.ListBox5.Text = Form1.ListBox5.Items.Item(0)
        End If

    End Sub

    Public Sub clear_data()
        For a = 0 To 1000
            For b = 0 To 1
                back_stop(a, b) = ""
            Next
        Next
        For a = 0 To 1000
            For b = 0 To 1
                back_subway(a, b) = ""
            Next
        Next

        For c = 0 To 500
            For d = 0 To 4
                back_station(c, d) = ""
            Next
        Next

        For f = 0 To 20
            For g = 0 To 1
                back_exit(f, g) = ""
            Next
        Next
    End Sub

End Module

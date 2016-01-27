Public Class Form1
    Private Sub 新建工程ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 新建工程ToolStripMenuItem.Click
        new_project.Show()
    End Sub

    Private Sub 设置相关预设ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 设置相关预设ToolStripMenuItem.Click
        before_set.Show()
    End Sub

    Private Sub 打开工程ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 打开工程ToolStripMenuItem.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            '设置标题
            Dim file As New System.IO.StreamReader(OpenFileDialog1.FileName, System.Text.Encoding.UTF8)
            Me.Text = Me.Text + " --- " + file.ReadLine
            project_path = file.ReadLine
            file.Dispose()

            work_mode = 1
            If check_build() = True Then
                build_mode = 1
                re_work_are()
                read_more_date()
            Else
                re_work_are()
            End If

            '读取数据
            read_line_name()
            read_another()

            '设置控件
            Button5.Enabled = False

        End If
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If OpenFileDialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim file As New System.IO.StreamReader(OpenFileDialog2.FileName, System.Text.Encoding.Default)
            TextBox20.Text = TextBox20.Text + file.ReadToEnd
            file.Dispose()
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '载入数据
        re_work_are()
        read_desktop()
    End Sub

    Private Sub 保存工程ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 保存工程ToolStripMenuItem.Click
        '

        If build_mode = 1 Then
            '写station
            Dim file As New System.IO.StreamWriter(project_path + "station.txt", False, System.Text.Encoding.UTF8)

            For a = 0 To 500
                If back_station(a, 0) = "" Then
                    Exit For
                End If

                file.WriteLine(back_station(a, 0))
                file.WriteLine(back_station(a, 1))
                file.WriteLine(back_station(a, 2))
                file.WriteLine(back_station(a, 3))
                file.WriteLine(back_station(a, 4))
                file.WriteLine("END")
            Next

            file.Dispose()

            'subway
            Dim file2 As New System.IO.StreamWriter(project_path + "subway.txt", False, System.Text.Encoding.UTF8)

            If ListBox5.Items.Count = 0 Then
                '取消写入
            Else
                For b = 0 To 1000
                    If back_station(b, 0) = "" Then
                        Exit For
                    End If

                    file2.WriteLine(back_subway(b, 0))
                    file2.WriteLine(back_subway(b, 1))
                    file2.WriteLine("END")
                Next
            End If

            file2.Dispose()
        End If

        '写have_bus
        Dim file_2 As New System.IO.StreamWriter(project_path + "have_bus.txt", False, System.Text.Encoding.UTF8)

        Dim word As String = ""
        Dim list As Integer = 0
        Do
            If ListBox1.Items.Count = list Then
                file_2.WriteLine("END")
                Exit Do
            End If
            word = ListBox1.Items.Item(list)
            file_2.WriteLine(word)

            list += 1
        Loop

        file_2.Dispose()

    End Sub

    Private Sub 关闭工程ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 关闭工程ToolStripMenuItem.Click
        If MsgBox("确认关闭吗？所有没保存的工作都将丢失！", 32 + 1, "关闭") = 1 Then
            work_mode = 0
            build_mode = 0
            project_path = ""
            re_work_are()

            '清空station subway相关数据
            clear_data()

            Me.Text = "bus_rode Resource Studio"


            '清空控件
            ListBox1.Items.Clear()
            ListBox4.Items.Clear()
            ListBox5.Items.Clear()
            ListBox6.Items.Clear()
        End If
    End Sub

    Private Sub 退出ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 退出ToolStripMenuItem.Click
        If MsgBox("确认退出吗？这将不会保存任何未保存的数据", 32 + 1, "退出") = 1 Then
            Application.Exit()
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        ListBox1.Sorted = CheckBox1.Checked
    End Sub

    Private Sub 生成全部ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 生成全部ToolStripMenuItem.Click
        If ListBox1.Items.Count = 0 Then
        Else
            '写have_bus
            Dim file_2 As New System.IO.StreamWriter(project_path + "have_bus.txt", False, System.Text.Encoding.UTF8)

            Dim word As String = ""
            Dim list As Integer = 0
            Do
                If ListBox1.Items.Count = list Then
                    file_2.WriteLine("END")
                    Exit Do
                End If
                word = ListBox1.Items.Item(list)
                file_2.WriteLine(word)

                list += 1
            Loop

            file_2.Dispose()

            '移动到目录下编译
            System.IO.File.Delete(Application.StartupPath + "\bus.txt")
            System.IO.File.Copy(project_path + "bus.txt", Application.StartupPath + "\bus.txt")

            System.IO.File.Delete(Application.StartupPath + "\stop.txt")
            System.IO.File.Delete(Application.StartupPath + "\station.txt")
            System.IO.File.Delete(Application.StartupPath + "\subway.txt")

            '检测现有的station
            If System.IO.File.Exists(project_path + "station.txt") Then
                System.IO.File.Delete(Application.StartupPath + "\station.txt")
                System.IO.File.Copy(project_path + "station.txt", Application.StartupPath + "\station.txt")
            End If
            '检测现有的subway
            If System.IO.File.Exists(project_path + "subway.txt") Then
                System.IO.File.Delete(Application.StartupPath + "\subway.txt")
                System.IO.File.Copy(project_path + "subway.txt", Application.StartupPath + "\subway.txt")
            End If

            '编译
            bus_rode_develop_tools.bus_rode_build_bus_to_station.start()
            bus_rode_develop_tools.bus_rode_build_bus_to_stop.start()
            bus_rode_develop_tools.bus_rode_build_bus_to_subway.start()

            '移动
            System.IO.File.Delete(project_path + "subway.txt")
            System.IO.File.Copy(Application.StartupPath + "\subway.txt", project_path + "subway.txt")
            System.IO.File.Delete(project_path + "station.txt")
            System.IO.File.Copy(Application.StartupPath + "\station.txt", project_path + "station.txt")
            System.IO.File.Delete(project_path + "stop.txt")
            System.IO.File.Copy(Application.StartupPath + "\stop.txt", project_path + "stop.txt")

            Dim file_test As New System.IO.StreamWriter(project_path + "station_have.txt", False, System.Text.Encoding.UTF8)
            file_test.WriteLine("END")
            file_test.Dispose()
            file_test = New System.IO.StreamWriter(project_path + "bus_bus.txt", False, System.Text.Encoding.UTF8)
            file_test.WriteLine("END")
            file_test.Dispose()

            MsgBox("build finish", 64)

            build_mode = 1
            re_work_are()
            ListBox4.Items.Clear()
            ListBox5.Items.Clear()
            ListBox6.Items.Clear()
            read_more_date()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        '添加
        Dim word As String = InputBox("输入名称:")
        If ListBox1.Items.Contains(word) = True Or word = "" Then
            MsgBox("name is empty or the list have this name!")
        Else
            ListBox1.Items.Add(word)

            Dim file_new As New System.IO.StreamWriter(project_path + "bus.txt.new", False, System.Text.Encoding.UTF8)
            Dim file As New System.IO.StreamReader(project_path + "bus.txt", System.Text.Encoding.UTF8)

            '写入头部
            file_new.WriteLine(word)
            file_new.WriteLine("")
            file_new.WriteLine("")
            file_new.WriteLine("")
            file_new.WriteLine("")
            file_new.WriteLine("")

            file_new.WriteLine("0")
            file_new.WriteLine("2")

            file_new.WriteLine("0")
            file_new.WriteLine("0")
            file_new.WriteLine("13")
            file_new.WriteLine("0")

            file_new.WriteLine("ENDLINE")
            file_new.WriteLine("END")

            '复制文件
            Dim wordd As String = ""
            Do
                wordd = file.ReadLine
                If wordd = "ENDDATE" Then
                    file_new.WriteLine("ENDDATE")
                    Exit Do
                End If

                '写入
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
            Loop


            file.Dispose()
            file_new.Dispose()

            System.IO.File.Delete(project_path + "bus.txt")
            System.IO.File.Move(project_path + "bus.txt.new", project_path + "bus.txt")
        End If

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        '删除
        Dim word As String = ListBox1.SelectedItem
        If ListBox1.Items.Contains(word) = True Then
            ListBox1.Items.Remove(word)

            Dim file_new As New System.IO.StreamWriter(project_path + "bus.txt.new", False, System.Text.Encoding.UTF8)
            Dim file As New System.IO.StreamReader(project_path + "bus.txt", System.Text.Encoding.UTF8)

            '复制文件
            Dim wordd As String = ""
            Do
                wordd = file.ReadLine
                If wordd = "ENDDATE" Then
                    file_new.WriteLine("ENDDATE")
                    Exit Do
                End If

                '检测到要删除的项目
                If wordd = word Then
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

            '设置控件

            Button5.Enabled = False

            '没有数据
            TextBox2.Text = ""

            TextBox3.Text = ""
            TextBox4.Text = ""
            TextBox5.Text = ""
            TextBox6.Text = ""
            TextBox7.Text = ""

            ComboBox1.SelectedIndex = 0
            ComboBox2.SelectedIndex = 2

            NumericUpDown1.Value = 0
            NumericUpDown2.Value = 0
            NumericUpDown4.Value = 12
            NumericUpDown3.Value = 0

            TextBox20.Text = ""

        Else
            MsgBox("app error,can't find this name in this list!")
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '搜索
        If TextBox1.Text <> "" Then
            If ListBox1.Items.Contains(TextBox1.Text) = True Then
                ListBox1.Text = TextBox1.Text
            Else
                MsgBox("Can't find this bus name!")
            End If
        End If
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        '更改选中车次
        If ListBox1.Text <> "" Then
            read_line(ListBox1.Text)
        End If

        If Button5.Enabled = False Then
            Button5.Enabled = True
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        save_line(project_path & ListBox1.Text & ".txt")
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Dim word As String = InputBox("输入新名称:")
        If ListBox1.Items.Contains(word) = False And word <> "" And ListBox1.Text <> "" Then


            Dim file_new As New System.IO.StreamWriter(project_path + "bus.txt.new", False, System.Text.Encoding.UTF8)
            Dim file As New System.IO.StreamReader(project_path + "bus.txt", System.Text.Encoding.UTF8)

            '写入头部
            file_new.WriteLine(word)

            file_new.WriteLine(TextBox3.Text)
            file_new.WriteLine(TextBox4.Text)
            file_new.WriteLine(TextBox5.Text)
            file_new.WriteLine(TextBox6.Text)
            file_new.WriteLine(TextBox7.Text)

            file_new.WriteLine(ComboBox1.SelectedIndex)
            file_new.WriteLine(ComboBox2.SelectedIndex)

            file_new.WriteLine(NumericUpDown1.Value.ToString)
            file_new.WriteLine(NumericUpDown2.Value.ToString)
            file_new.WriteLine(NumericUpDown4.Value.ToString)
            file_new.WriteLine(NumericUpDown3.Value.ToString)

            file_new.WriteLine(TextBox20.Text)
            '不换行
            file_new.WriteLine("ENDLINE")

            If TextBox10.Text <> "" Then
                file_new.WriteLine(TextBox10.Text)

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
                If wordd = ListBox1.Text Then
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


            ListBox1.Items.Remove(ListBox1.Text)
            ListBox1.Items.Add(word)
            TextBox2.Text = word
        Else
            MsgBox("List have this name or name is empty!")
        End If
    End Sub

    Private Sub Button22_Click(sender As Object, e As EventArgs) Handles Button22.Click
        '写入杂项
        Dim file As New System.IO.StreamWriter(project_path + "readme.txt", False, System.Text.Encoding.UTF8)
        file.WriteLine(TextBox21.Text & "-" & TextBox22.Text & "-" & TextBox23.Text)
        file.WriteLine(TextBox19.Text)
        file.WriteLine(TextBox24.Text)
        file.Dispose()
        Dim file_1 As New System.IO.StreamWriter(project_path + "download.txt", False, System.Text.Encoding.UTF8)
        file_1.WriteLine(TextBox17.Text)
        file_1.Write(TextBox18.Text)
        file_1.Dispose()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        '预设

        TextBox3.Text = desktop(0)
        TextBox4.Text = desktop(1)
        TextBox5.Text = desktop(2)
        TextBox6.Text = desktop(3)
        TextBox7.Text = desktop(4)
    End Sub

    Private Sub ListBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox5.SelectedIndexChanged
        If ListBox5.Items.Count <> 0 Then

            For a = 0 To 20
                For b = 0 To 1
                    back_exit(a, b) = ""
                Next
            Next
            ListBox6.Items.Clear()

            Dim word As String = ""
            Dim list1 As Integer = 0

            Dim file2 As New System.IO.StreamReader(project_path + "subway.txt", System.Text.Encoding.UTF8)

            Do
                word = file2.ReadLine
                If word = "" Then
                    Exit Do
                End If

                If word = ListBox5.Text Then

                    Do
                        word = file2.ReadLine
                        If word = "END" Then
                            Exit Do
                        End If

                        back_exit(list1, 0) = word
                        '对控件输出
                        ListBox6.Items.Add(word)

                        back_exit(list1, 1) = file2.ReadLine

                        list1 += 1

                    Loop
                Else
                    '读到结束
                    Do
                        word = file2.ReadLine
                        If word = "END" Then
                            Exit Do
                        End If

                        'jump
                        file2.ReadLine()

                    Loop
                End If

            Loop

            file2.Dispose()

            ListBox6.Text = ListBox6.Items.Item(0)

        End If
    End Sub

    Private Sub ListBox6_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox6.SelectedIndexChanged
        If ListBox6.SelectedIndex >= 0 Then
            TextBox16.Text = back_exit(ListBox6.SelectedIndex, 1)
        End If

    End Sub

    Private Sub Button21_Click(sender As Object, e As EventArgs) Handles Button21.Click
        back_exit(ListBox6.SelectedIndex, 1) = TextBox16.Text
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        Dim word As String = ""
        For a = 0 To 20
            If back_exit(a, 0) = "" Then
                word = InputBox("输入出口名:")
                If ListBox6.Items.Contains(word) = True And word = "" Then
                    MsgBox("This word have added in this list!", 16)
                Else
                    If word <> "" Then
                        back_exit(a, 0) = word
                        ListBox6.Items.Add(word)
                        ListBox6.Text = word
                    Else
                        MsgBox("this word is empty!", 16)
                    End If

                End If
                Exit For
            End If
            If a = 20 Then
                MsgBox("cna't add this word, because list is full!", 16)
            End If
        Next
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        If ListBox6.Items.Count = 1 Then
            MsgBox("you can't delete, you must keep once item in this list!")
        Else
            For a = 0 To 20
                If back_exit(a, 0) = ListBox6.Text Then

                    For b = a To 20
                        If b = 20 Then
                            back_exit(b, 0) = ""
                            back_exit(b, 1) = ""
                        Else
                            back_exit(b, 0) = back_exit(b + 1, 0)
                            back_exit(b, 1) = back_exit(b + 1, 1)
                        End If
                    Next
                    Exit For

                End If
            Next

            ListBox6.Items.Remove(ListBox6.Text)
            ListBox6.Text = ListBox6.Items.Item(0)

        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        '保存地铁信息
        Dim word As String = ""

        For a = 0 To 20
            If back_exit(a, 0) = "" Then
                Exit For
            Else
                If word <> "" Then
                    word = word + vbCrLf
                End If
                word = word + back_exit(a, 0) + vbCrLf + back_exit(a, 1)
            End If
        Next

        back_subway(ListBox5.SelectedIndex, 1) = word

        '存储到文件
        Dim file2 As New System.IO.StreamWriter(project_path + "subway.txt", False, System.Text.Encoding.UTF8)

        If ListBox5.Items.Count = 0 Then
            '取消写入
        Else
            For b = 0 To 1000
                If back_station(b, 0) = "" Then
                    Exit For
                End If

                file2.WriteLine(back_subway(b, 0))
                file2.WriteLine(back_subway(b, 1))
                file2.WriteLine("END")
            Next

        End If

        file2.Dispose()

    End Sub

    Private Sub ListBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox4.SelectedIndexChanged
        '读取总站
        TextBox14.Text = back_station(ListBox4.SelectedIndex, 1)
        TextBox13.Text = back_station(ListBox4.SelectedIndex, 2)
        TextBox12.Text = back_station(ListBox4.SelectedIndex, 3)
        TextBox11.Text = back_station(ListBox4.SelectedIndex, 4)
    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        '保存总站
        back_station(ListBox4.SelectedIndex, 1) = TextBox14.Text
        back_station(ListBox4.SelectedIndex, 2) = TextBox13.Text
        back_station(ListBox4.SelectedIndex, 3) = TextBox12.Text
        back_station(ListBox4.SelectedIndex, 4) = TextBox11.Text
    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        '搜索
        If TextBox9.Text <> "" Then
            If ListBox4.Items.Contains(TextBox9.Text) = True Then
                ListBox4.Text = TextBox9.Text
            Else
                MsgBox("Can't find this station name!")
            End If
        End If
    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        '搜索
        If TextBox15.Text <> "" Then
            If ListBox5.Items.Contains(TextBox15.Text) = True Then
                ListBox5.Text = TextBox15.Text
            Else
                MsgBox("Can't find this subway name!")
            End If
        End If
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click

        TextBox14.Text = desktop(5)
        TextBox13.Text = desktop(6)
        TextBox12.Text = desktop(7)
        TextBox11.Text = desktop(8)
    End Sub

    Private Sub 打包资源ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 打包资源ToolStripMenuItem.Click
        MsgBox("请确保在打包前已经点击了立即生成，否则资源可能会不配套")
        System.IO.File.Delete(project_path + "release.brs")
        bus_rode_compression.bus_rode_compression.Compress(Mid(project_path, 1, project_path.Length - 1), project_path + "release.brs")
        MsgBox("打包完成")
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        '替换为回车
        If TextBox8.Text <> "" Then
            If CheckBox2.Checked = True Then
                TextBox20.Text = Replace(TextBox20.Text, TextBox8.Text, vbCrLf)
            End If
            If CheckBox3.Checked = True Then
                TextBox10.Text = Replace(TextBox10.Text, TextBox8.Text, vbCrLf)
            End If
        End If
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        Dim a = InputBox("输入要替换为的字符：")
        If MsgBox("是否将 " & TextBox8.Text & " 替换为 " & a, MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
            TextBox20.Text = Replace(TextBox20.Text, TextBox8.Text, a)
        End If

    End Sub

    Private Sub 关于ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 关于ToolStripMenuItem.Click
        MsgBox("bus_rode_develope version 7000 适用于开发bus_rode 7版本的资源文件", 64, "bus_rode_develop")
    End Sub
End Class

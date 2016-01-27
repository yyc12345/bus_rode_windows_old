Public Class new_project

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            TextBox2.Text = FolderBrowserDialog1.SelectedPath

            If Mid(TextBox2.Text, TextBox2.Text.Length, 1) <> "\" Then
                TextBox2.Text = TextBox2.Text & "\"
            End If

        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            TextBox3.Text = OpenFileDialog1.FileName
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If TextBox1.Text <> "" And TextBox2.Text <> "" Then

            '创建工程文件
            Dim file As New System.IO.StreamWriter(TextBox2.Text + TextBox1.Text + ".brsp", False, System.Text.Encoding.UTF8)
            '写入项目名
            file.WriteLine(TextBox1.Text)
            '写入工程路径
            file.WriteLine(TextBox2.Text)
            file.Dispose()

            '创建资源文件
            file = New System.IO.StreamWriter(TextBox2.Text + "readme.txt", False, System.Text.Encoding.UTF8)
            file.WriteLine("")
            file.Dispose()

            If CheckBox1.Checked = True And System.IO.File.Exists(TextBox3.Text) = True And System.IO.File.Exists(TextBox4.Text) = True Then
                '有预设
                System.IO.File.Delete(TextBox2.Text + "bus.txt")
                System.IO.File.Copy(TextBox3.Text, TextBox2.Text + "bus.txt")

                System.IO.File.Delete(TextBox2.Text + "have_bus.txt")
                System.IO.File.Copy(TextBox4.Text, TextBox2.Text + "have_bus.txt")
            Else
                '没有预设
                file = New System.IO.StreamWriter(TextBox2.Text + "bus.txt", False, System.Text.Encoding.UTF8)
                file.WriteLine("ENDDATE")
                file.Dispose()

                file = New System.IO.StreamWriter(TextBox2.Text + "have_bus.txt", False, System.Text.Encoding.UTF8)
                file.WriteLine("END")
                file.Dispose()
            End If

            file = New System.IO.StreamWriter(TextBox2.Text + "download.txt", False, System.Text.Encoding.UTF8)
            file.WriteLine("")
            file.WriteLine("")
            file.Dispose()


            '预设
            project_path = TextBox2.Text
            work_mode = 1
            build_mode = 0
            re_work_are()
            If CheckBox1.Checked = True And System.IO.File.Exists(TextBox3.Text) = True And System.IO.File.Exists(TextBox4.Text) = True Then
                '读取数据
                read_line_name()
                read_another()
            End If

            '设置控件
            Form1.Text = Form1.Text + " --- " + TextBox1.Text
            Form1.Button5.Enabled = False

            Me.Hide()


        Else
                MsgBox("没有指定工程名或目录", 16, "错误")
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = False Then
            Button2.Enabled = False
            Label4.Enabled = False
            Button4.Enabled = False
            Label5.Enabled = False
        Else
            Button2.Enabled = True
            Label4.Enabled = True
            Button4.Enabled = True
            Label5.Enabled = True
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If OpenFileDialog2.ShowDialog = Windows.Forms.DialogResult.OK Then
            TextBox4.Text = OpenFileDialog2.FileName
        End If
    End Sub
End Class
Imports System.IO
Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If TextBox1.Text <> "" And TextBox2.Text <> "" And TextBox5.Text <> "" Then
            If RadioButton1.Checked = True Then
                '压缩
                Dim result As Boolean = bus_rode_compression.bus_rode_compression.Compress(TextBox2.Text, TextBox1.Text, CType(TextBox5.Text, Int64))
                If result = False Then
                    MsgBox("compress error", 16, "error")
                Else
                    MsgBox("compress ok", 64, "finish")
                End If
            Else
                '解压缩
                Dim result As Boolean = bus_rode_compression.bus_rode_compression.Decompress(TextBox2.Text, TextBox1.Text, CType(TextBox5.Text, Int64))
                If result = False Then
                    MsgBox("decompress error", 16, "error")
                Else
                    MsgBox("decompress ok", 64, "finish")
                End If
            End If
        Else
            MsgBox("don't have folder or file or version", 16, "error")
        End If

    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click

        If TextBox2.Text <> "" And File.Exists(TextBox2.Text) = True Then
            '开始读取
            Dim into As FileStream = New FileStream(TextBox2.Text, FileMode.Open, FileAccess.Read, FileShare.Read)
            Dim br As BinaryReader = New BinaryReader(into, System.Text.Encoding.UTF8)

            '======================先读文件头，判断
            Dim file_head As String = New String(br.ReadChars(4))
            If file_head <> "BRSP" Then
                '不合格，不行，退出
                MsgBox("unknow file!!!!", 16, "error")
                br.Dispose()
                into.Dispose()
                Exit Sub
            End If

            MsgBox(br.ReadInt64(), 64, "Touch Version")

            br.Dispose()
            into.Dispose()

        Else
            MsgBox("don't have file", 16, "error")
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            Dim word As String = FolderBrowserDialog1.SelectedPath
            If RadioButton1.Checked = True Then
                '压缩
                If Mid(word, word.Length, 1) = "\" Then
                    word = Mid(word, 1, word.Length - 1)
                End If
            Else
                If Mid(word, word.Length, 1) <> "\" Then
                    word = word & "\"
                End If

            End If

            TextBox1.Text = word
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If RadioButton1.Checked = True Then
            '压缩，用保存
            If SaveFileDialog1.ShowDialog = DialogResult.OK Then
                TextBox2.Text = SaveFileDialog1.FileName
            End If
        Else
            If OpenFileDialog1.ShowDialog = DialogResult.OK Then
                TextBox2.Text = OpenFileDialog1.FileName
            End If
        End If
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If TextBox1.Text <> "" Then
            Dim word As String = TextBox1.Text
            If RadioButton1.Checked = True Then
                '压缩
                If Mid(word, word.Length, 1) = "\" Then
                    word = Mid(word, 1, word.Length - 1)
                End If
            Else
                If Mid(word, word.Length, 1) <> "\" Then
                    word = word & "\"
                End If

            End If

            TextBox1.Text = word
        End If
        If TextBox3.Text <> "" Then
            Dim word_2 As String = TextBox3.Text
            If RadioButton1.Checked = True Then
                '压缩
                If Mid(word_2, word_2.Length, 1) = "\" Then
                    word_2 = Mid(word_2, 1, word_2.Length - 1)
                End If
            Else
                If Mid(word_2, word_2.Length, 1) <> "\" Then
                    word_2 = word_2 & "\"
                End If

            End If

            TextBox3.Text = word_2
        End If

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            Dim word As String = FolderBrowserDialog1.SelectedPath
            If RadioButton1.Checked = True Then
                '压缩
                If Mid(word, word.Length, 1) = "\" Then
                    word = Mid(word, 1, word.Length - 1)
                End If
            Else
                If Mid(word, word.Length, 1) <> "\" Then
                    word = word & "\"
                End If

            End If

            TextBox3.Text = word
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If RadioButton1.Checked = True Then
            '压缩，用保存
            If SaveFileDialog1.ShowDialog = DialogResult.OK Then
                TextBox4.Text = SaveFileDialog1.FileName
            End If
        Else
            If OpenFileDialog1.ShowDialog = DialogResult.OK Then
                TextBox4.Text = OpenFileDialog1.FileName
            End If
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If TextBox3.Text <> "" And TextBox4.Text <> "" Then
            If RadioButton1.Checked = True Then
                '压缩
                Dim aaa As New ICSharpCode.SharpZipLib.Zip.FastZip
                aaa.CreateZip(TextBox4.Text, TextBox3.Text, False, Nothing)
            Else
                '解压缩
                Dim aaa As New ICSharpCode.SharpZipLib.Zip.FastZip
                aaa.ExtractZip(TextBox4.Text, TextBox3.Text, Nothing)
            End If
        Else
            MsgBox("don't have folder or file", 16, "error")
        End If
    End Sub

End Class

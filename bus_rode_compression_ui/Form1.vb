Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        If TextBox1.Text <> "" And TextBox2.Text <> "" Then
            If RadioButton1.Checked = True Then
                '压缩
                bus_rode_compression.bus_rode_compression.Compress(TextBox1.Text, TextBox2.Text)
            Else
                '解压缩
                bus_rode_compression.bus_rode_compression.DeCompress(TextBox2.Text, TextBox1.Text)
            End If
        Else
            MsgBox("don't have folder or file", 16, "error")
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

Public Class before_set

    Private Sub before_set_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        read_desktop()
        TextBox3.Text = desktop(0)
        TextBox4.Text = desktop(1)
        TextBox5.Text = desktop(2)
        TextBox6.Text = desktop(3)
        TextBox7.Text = desktop(4)

        TextBox9.Text = desktop(5)
        TextBox8.Text = desktop(6)
        TextBox2.Text = desktop(7)
        TextBox1.Text = desktop(8)

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        desktop(0) = TextBox3.Text
        desktop(1) = TextBox4.Text
        desktop(2) = TextBox5.Text
        desktop(3) = TextBox6.Text
        desktop(4) = TextBox7.Text

        desktop(5) = TextBox9.Text
        desktop(6) = TextBox8.Text
        desktop(7) = TextBox2.Text
        desktop(8) = TextBox1.Text

        save_desktop()
    End Sub
End Class
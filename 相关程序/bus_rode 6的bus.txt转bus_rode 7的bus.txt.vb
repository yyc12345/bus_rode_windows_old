Module Module1

    Public bus_stop(200) As String
    Public bus_stop_list As Integer = 0

    Sub Main()

        If System.IO.File.Exists(Environment.CurrentDirectory + "\bus.txt") Then
            System.IO.File.Move(Environment.CurrentDirectory + "\bus.txt", Environment.CurrentDirectory + "\bus.txt.old")

            Dim file_old As New System.IO.StreamReader(Environment.CurrentDirectory + "\bus.txt.old", System.Text.Encoding.UTF8)
            Dim file As New System.IO.StreamWriter(Environment.CurrentDirectory + "\bus.txt", False, System.Text.Encoding.UTF8)

            Dim word As String = ""
            Do
                word = file_old.ReadLine
                If word = "ENDDATE" Then
                    file.WriteLine("ENDDATE")
                    Exit Do
                End If

                file.WriteLine(word)
                file.WriteLine(file_old.ReadLine)
                file.WriteLine(file_old.ReadLine)
                file.WriteLine(file_old.ReadLine)
                file.WriteLine(file_old.ReadLine)
                file.WriteLine(file_old.ReadLine)
                file.WriteLine(file_old.ReadLine)
                file.WriteLine(file_old.ReadLine)
                file.WriteLine(file_old.ReadLine)
                file.WriteLine(file_old.ReadLine)
                file.WriteLine(file_old.ReadLine)
                file.WriteLine(file_old.ReadLine)

                Do
                    word = file_old.ReadLine
                    If word = "END" Then
                        file.WriteLine("ENDLINE")
                        Exit Do
                    End If

                    bus_stop(bus_stop_list) = word
                    bus_stop_list += 1
                    file.WriteLine(word)
                Loop

                For a = (bus_stop_list - 1) To 0 Step -1
                    file.WriteLine(bus_stop(a))
                Next

                file.WriteLine("END")

                bus_stop_list = 0
            Loop






            file_old.Dispose()
            file.Dispose()

            System.IO.File.Delete(Environment.CurrentDirectory + "\bus.txt.old")

            MsgBox("finish!")

        Else
            MsgBox("can't find file!", 16)
        End If

    End Sub

End Module

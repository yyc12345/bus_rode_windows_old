Imports System.IO
Public Class bus_rode_compression

    Public Const ResourcesFiles As String = "bus.txt,have_bus.txt,readme.txt,station.txt,stop.txt,subway.txt"

    ''' <summary>
    ''' 压缩文件，返回true表示成功，false表示失败
    ''' </summary>
    ''' <param name="out_file_path">输出文件的绝对目录</param>
    ''' <param name="file_folder">输入文件所在的文件夹，需要加\</param>
    ''' <param name="file_version">需要输入的build版本号</param>
    ''' <returns></returns>
    Public Shared Function Compress(ByVal out_file_path As String, ByVal file_folder As String, ByVal file_version As Int64) As Boolean

        If CheckFolder(file_folder) = False Then Return False
        If File.Exists(out_file_path) = True Then File.Delete(out_file_path)

        '开始保存
        Dim out As FileStream = New FileStream(out_file_path, FileMode.Create, FileAccess.Write, FileShare.Write)
        Dim br As BinaryReader
        Dim bw As BinaryWriter = New BinaryWriter(out, System.Text.Encoding.UTF8)
        Dim file_arr() As String = ResourcesFiles.Split(",")
        Dim into As FileStream

        '计量数据数字的变量
        'char以1000为基数读取，读不完就写
        Dim char_count As Int64 = 0
        Dim file_count_list As New ArrayList

        '先乱写文件头
        bw.Write("BRSP".ToCharArray)
        bw.Write(CType(0, Int64))
        For a = 0 To file_arr.Count - 1
            bw.Write(CType(0, Int64))
        Next

        '读取文件写入
        For a = 0 To file_arr.Count - 1
            '打开文件
            into = New FileStream(file_folder & file_arr(a), FileMode.Open, FileAccess.Read, FileShare.Read)
            br = New BinaryReader(into, System.Text.Encoding.UTF8)

            '读取循环
            Do
                '先读取那么多
                Dim cache_char() As Char = br.ReadChars(1000)

                '如果为0，没有内容，不结束读取
                If cache_char.Count = 0 Then Exit Do

                '读取数量不足，说明也到文件尾了
                If cache_char.Count < 1000 Then
                    char_count += cache_char.Count
                    bw.Write(cache_char)
                    Exit Do
                Else
                    '继续读取
                    char_count += 1000
                    bw.Write(cache_char)
                End If

            Loop


            '善后处理
            br.Dispose()
            into.Dispose()

            file_count_list.Add(char_count)
            char_count = 0
        Next

        '==============================结束各个变量，准备覆写
        bw.Dispose()
        out.Dispose()

        '覆写
        Dim out_process As FileStream = New FileStream(out_file_path, FileMode.Open, FileAccess.Write, FileShare.Write)
        Dim bw_process As BinaryWriter = New BinaryWriter(out_process, System.Text.Encoding.UTF8)

        '写文件头
        bw_process.Write("BRSP".ToCharArray)
        bw_process.Write(file_version)
        For a = 0 To file_arr.Count - 1
            bw_process.Write(CType(file_count_list.Item(a), Int64))
        Next

        bw_process.Dispose()
        out_process.Dispose()

        Return True
    End Function

    ''' <summary>
    ''' 解压缩文件，返回true表示成功，false表示失败
    ''' </summary>
    ''' <param name="file_path">包文件的绝对目录</param>
    ''' <param name="out_file_folder">输出文件所在的文件夹，需要加\</param>
    ''' <param name="file_version">需要验证的build版本号</param>
    ''' <returns></returns>
    Public Shared Function Decompress(ByVal file_path As String, ByVal out_file_folder As String, ByVal file_version As Int64) As Boolean

        '检测
        If File.Exists(file_path) = False Then Return False

        '开始读取
        Dim into As FileStream = New FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.Read)
        Dim br As BinaryReader = New BinaryReader(into, System.Text.Encoding.UTF8)
        Dim out As FileStream
        Dim bw As BinaryWriter
        Dim file_arr() As String = ResourcesFiles.Split(",")

        '计量数据数字的变量
        'char以1000为基数写入
        Dim file_count_list As New ArrayList

        '======================先读文件头，判断
        Dim file_head As String = New String(br.ReadChars(4))
        If file_head <> "BRSP" Then
            '不合格，不行，退出
            br.Dispose()
            into.Dispose()
            Return False
        End If
        If br.ReadInt64() <> file_version Then
            '不合格，不行，退出
            br.Dispose()
            into.Dispose()
            Return False
        End If

        '============确认无误后可以写入===============
        SetFolder(out_file_folder)

        '读入区块
        For a = 0 To file_arr.Count - 1
            file_count_list.Add(br.ReadInt64())
        Next

        '读数据
        For a = 0 To file_arr.Count - 1

            '如果没有可以读的，取消之
            If CType(file_count_list.Item(a), Int64) = 0 Then GoTo exit_for

            '打开文件
            out = New FileStream(out_file_folder & file_arr(a), FileMode.Create, FileAccess.Write, FileShare.Write)
            bw = New BinaryWriter(out, System.Text.Encoding.UTF8)

            '=================================计算读取次数
            '需要的for循环次数
            Dim for_count As Integer = Int(CType(file_count_list.Item(a), Int64) / 1000)
            '余下的多余的次数
            Dim lost_count As Integer = CType(file_count_list.Item(a), Int64) Mod 1000

            '====================================读取循环
            '没有达到1000，直接在余下处理里处理
            If for_count = 0 Then GoTo lost_count_process
            For b = 0 To for_count - 1
                '读取与写入
                bw.Write(br.ReadChars(1000))
            Next

lost_count_process:

            '其余读取
            '是否需要读取
            If lost_count = 0 Then
                '不需要
            Else
                '需要
                bw.Write(br.ReadChars(lost_count))
            End If

            '===================善后处理
            bw.Dispose()
            out.Dispose()

exit_for:
        Next

        '善后处理
        br.Dispose()
        into.Dispose()

        Return True
    End Function

    ''' <summary>
    ''' 设置输出的目录
    ''' </summary>
    ''' <param name="folder_path">文件夹，需要加\</param>
    Public Shared Sub SetFolder(ByVal folder_path As String)

        Dim file_arr() As String = ResourcesFiles.Split(",")
        For a = 0 To file_arr.Count - 1
            File.Delete(folder_path & file_arr(a))
        Next

    End Sub

    ''' <summary>
    ''' 检测文件夹下是否存在需要打包的文件
    ''' </summary>
    ''' <param name="folder_path">文件夹，需要加\</param>
    ''' <returns></returns>
    Public Shared Function CheckFolder(ByVal folder_path As String) As Boolean

        Dim file_arr() As String = ResourcesFiles.Split(",")
        For a = 0 To file_arr.Count - 1
            If File.Exists(folder_path & file_arr(a)) = False Then Return False
        Next

        Return True

    End Function

End Class




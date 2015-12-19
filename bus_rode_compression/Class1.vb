Imports System
Imports System.Text
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Collections
Imports System.IO
Imports System.IO.Compression
Public Class bus_rode_compression
    ''' <summary>
    ''' 对目标文件夹进行压缩，将压缩结果保存为指定文件
    ''' </summary>
    ''' <param name="dirPath">目标文件夹</param>
    ''' <param name="fileName">压缩文件</param>
    ''' <remarks></remarks>
    Public Shared Sub Compress(ByVal dirPath As String, ByVal fileName As String)

        Dim list = New ArrayList()
        For Each f As String In Directory.GetFiles(dirPath)

            Dim destBuffer As Byte() = File.ReadAllBytes(f)
            Dim sfi As SerializeFileInfo = New SerializeFileInfo(f, destBuffer)
            list.Add(sfi)
        Next
        Dim Formatter As IFormatter = New BinaryFormatter()
        Using s As Stream = New MemoryStream()
            Formatter.Serialize(s, list)
            s.Position = 0
            CreateCompressFile(s, fileName)
        End Using
    End Sub

    ''' <summary>
    ''' 对目标压缩文件解压缩，将内容解压缩到指定文件夹
    ''' </summary>
    ''' <param name="fileName">压缩文件</param>
    ''' <param name="dirPath">解压缩目录</param>
    ''' <remarks></remarks>
    Public Shared Sub DeCompress(ByVal fileName As String, ByVal dirPath As String)
        Using source As Stream = File.OpenRead(fileName)

            Using destination As Stream = New MemoryStream()

                Using input As GZipStream = New GZipStream(source, CompressionMode.Decompress, True)

                    Dim bytes(4096) As Byte
                    Dim n As Integer
                    n = input.Read(bytes, 0, bytes.Length)
                    While n <> 0
                        destination.Write(bytes, 0, n)
                        n = input.Read(bytes, 0, bytes.Length)
                    End While
                End Using
                destination.Flush()
                destination.Position = 0
                DeSerializeFiles(destination, dirPath)
            End Using
        End Using
    End Sub

    Private Shared Sub DeSerializeFiles(ByVal s As Stream, ByVal dirPath As String)
        Dim b As BinaryFormatter = New BinaryFormatter()
        Dim list As ArrayList = CType(b.Deserialize(s), ArrayList)

        For Each f As SerializeFileInfo In list
            Dim newName As String = dirPath + Path.GetFileName(f.fileName)
            Using fs As FileStream = New FileStream(newName, FileMode.Create, FileAccess.Write)
                fs.Write(f.fileBuffer, 0, f.fileBuffer.Length)
                fs.Close()
            End Using
        Next
    End Sub

    Private Shared Sub CreateCompressFile(ByVal source As Stream, ByVal destinationName As String)

        Using destination As Stream = New FileStream(destinationName, FileMode.Create, FileAccess.Write)
            Using output As GZipStream = New GZipStream(destination, CompressionMode.Compress)
                Dim bytes(4096) As Byte
                Dim n As Integer
                n = source.Read(bytes, 0, bytes.Length)
                While n <> 0
                    output.Write(bytes, 0, n)
                    n = source.Read(bytes, 0, bytes.Length)
                End While
                output.Close()
            End Using
        End Using

    End Sub

    <Serializable()>
    Class SerializeFileInfo

        Public Sub New(ByVal name As String, ByVal buffer As Byte())
            fileName = name
            fileBuffer = buffer
        End Sub
        Public Property fileName As String
        Public Property fileBuffer As Byte()

    End Class
End Class


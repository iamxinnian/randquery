Public Class LZerOne_toHmacString
    Dim key(15) As Byte
    Dim comparer_one As String
    Dim comparer_zero As String
    Dim clzerone As New leftzeronecode.CodeLeftZeroOne
    Dim myEncoder As New System.Text.UTF8Encoding
    'Dim aciiEncoder As New System.Text.ASCIIEncoding
    Public Sub set_parameter(ByVal keystr As String, ByVal data_num As String)
        Dim n As Integer = Int(Len(keystr) / 2)
        For i = 1 To n
            key(i - 1) = CByte("&h" & Mid(keystr, 2 * i - 1, 2))
        Next
        clzerone.digit_n = Integer.Parse(data_num)
    End Sub
    Public Sub calc_zerone_hmacstring(ByVal num_str As String)
        Dim myhmacmac As New System.Security.Cryptography.HMACMD5(key)
        comparer_one = ""
        comparer_zero = ""
        clzerone.num = num_str
        clzerone.DoLZeroOneCode()

        '处理左向1编码
        For i = 0 To 63 Step 1
            If clzerone.LOne(i) = "" Then
                Exit For
            Else
                Dim bytd() As Byte = myhmacmac.ComputeHash(myEncoder.GetBytes(clzerone.LOne(i)))
                If i = 0 Then
                    comparer_one = Convert.ToBase64String(bytd)
                Else
                    comparer_one = comparer_one + "," + Convert.ToBase64String(bytd)
                End If

            End If
        Next

        '处理左向0编码
        For i = 0 To 63 Step 1
            If clzerone.LZero(i) = "" Then
                Exit For
            Else
                Dim bytd() As Byte = myhmacmac.ComputeHash(myEncoder.GetBytes(clzerone.LZero(i)))
                If i = 0 Then
                    comparer_zero = Convert.ToBase64String(bytd)
                Else
                    comparer_zero = comparer_zero + "," + Convert.ToBase64String(bytd)
                End If

            End If
        Next

    End Sub
    Public Function get_comparer_one() As String
        Return comparer_one
    End Function
    Public Function get_comparer_zero() As String
        Return comparer_zero
    End Function
End Class

Public Class aes_decripter_strn
    Dim key(15) As Byte
    Public nstr As String()
    Public str_min_max(2) As String
    Public clearcode As String
    Dim decripter As System.Security.Cryptography.Aes = System.Security.Cryptography.Aes.Create("AES")
    Dim cripter As Security.Cryptography.ICryptoTransform
    Public Sub setkey(ByVal mykey As String)
        Dim n As Integer = Int(Len(mykey) / 2)
        For i = 1 To n Step 1
            key(i - 1) = CByte("&h" & Mid(mykey, 2 * i - 1, 2))
        Next
        decripter.BlockSize = 128
        decripter.Key = key
        decripter.Mode = System.Security.Cryptography.CipherMode.ECB
        decripter.Padding = System.Security.Cryptography.PaddingMode.Zeros
    End Sub
    Public Function do_decripter(ByVal cipher As String) As String
        clearcode = ""
        cripter = decripter.CreateDecryptor()
        Try
            Dim inBuff As Byte() = Convert.FromBase64String(cipher)
            clearcode = System.Text.ASCIIEncoding.ASCII.GetString(cripter.TransformFinalBlock(inBuff, 0, inBuff.Length))
        Catch ex As Exception
            'MessageBox.Show(ex.Message.ToString + ":-1", "提示：")
            Return "-1"
        End Try
        Return "0"
    End Function
    Public Function do_encripter(ByVal input_str As String) As String
        Dim mycripter As String = ""
        cripter = decripter.CreateEncryptor()
        Try
            mycripter = Convert.ToBase64String(cripter.TransformFinalBlock(System.Text.Encoding.ASCII.GetBytes(input_str), 0, input_str.Length))
        Catch ex As Exception
            MessageBox.Show(ex.Message.ToString + ":-2", "提示：")
        End Try
        Return mycripter
    End Function
    Public Sub split_tonstr()
        nstr = {""}
        str_min_max = {"", ""}
        nstr = clearcode.Split(",")
        str_min_max(0) = nstr(0)
        str_min_max(1) = nstr(0)
        For i = 1 To nstr.Length - 1 Step 1
            If CInt(nstr(i)) < CInt(str_min_max(0)) Then
                str_min_max(0) = nstr(i)
            End If

            If CInt(nstr(i)) > CInt(str_min_max(1)) Then
                str_min_max(1) = nstr(i)
            End If
        Next
    End Sub
End Class

Public Class TimeCheck
    Public currtime As DateTime
    Public endtime As DateTime
End Class
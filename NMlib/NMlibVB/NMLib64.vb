Imports System
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Text

'SADECE 64 BIT İŞLETİM SİSTEMİNİ DESTEKLER!
'Bu kütüphaneyi kullandığınız için teşekkürler. :)
'https://www.turkhackteam.org/members/832873.html
'- ilyn

Namespace NMlib64VB
    Public Module NeutronMemoryLibrary

        Public ProcessRunning As Boolean = False

        <DllImport("kernel32.dll", EntryPoint:="WriteProcessMemory")>
        Private Function WriteMemory64(ByVal hProcess As Integer, ByVal lpBaseAddress As Long, ByVal lpBuffer As Byte(), ByVal dwSize As Integer, ByRef IpNumberOfBytesWritten As Integer) As Boolean
        End Function
        <DllImport("user32.dll")>
        Private Function GetWindowText(ByVal hWnd As IntPtr, ByVal text As StringBuilder, ByVal count As Integer) As Integer
        End Function
        <DllImport("user32.dll")>
        Private Function GetForegroundWindow() As IntPtr
        End Function
        <DllImport("kernel32.dll")>
        Function OpenProcess(ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Integer) As IntPtr
        End Function
        <DllImport("kernel32.dll", SetLastError:=True, EntryPoint:="ReadProcessMemory")>
        Private Function ReadMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr,
        <Out> ByVal lpBuffer As Byte(), ByVal dwSize As Integer, <Out> ByRef lpNumberOfBytesRead As IntPtr) As Boolean
        End Function
        Private Handle As IntPtr = IntPtr.Zero
        Private Base As Long = 0
        Private hasBase As Boolean = False

        Private Function ToByteArray(ByVal HexString As String) As Byte()
            Dim NumberChars As Integer = HexString.Length
            Dim bytes As Byte() = New Byte(NumberChars / 2 - 1) {}

            For i As Integer = 0 To NumberChars - 1 Step 2
                bytes(i / 2) = Convert.ToByte(HexString.Substring(i, 2), 16)
            Next

            Return bytes
        End Function

        Private Function ToOpcode(ByVal hex As String) As Byte()
            Return ToByteArray(hex.Replace(" ", ""))
        End Function

        Private Sub pc(ByVal processname As String)
            Dim procs As Process() = Process.GetProcessesByName(processname)

            If processname IsNot Nothing Then

                If procs.Length = 0 Then
                    hasBase = False
                    ProcessRunning = False
                    Handle = IntPtr.Zero
                Else

                    For Each proc As Process In procs
                        Dim num As Integer = 256
                        Dim builder As StringBuilder = New StringBuilder(num)
                        ProcessRunning = True
                        Handle = OpenProcess(&H1F0FFF, False, proc.Id)

                        If Not hasBase Then
                            hasBase = True
                            Base = proc.MainModule.BaseAddress.ToInt64()
                        End If
                    Next
                End If
            End If
        End Sub

        Private Sub WM(ByVal opcode As Long, ByVal bytes As Byte())
            If ProcessRunning AndAlso Handle <> IntPtr.Zero Then
                Dim written As Integer = 0
                WriteMemory64(CInt(Handle), (Base + opcode), bytes, bytes.Length, written)
            End If
        End Sub

        Private Function ReadInt64(ByVal process As IntPtr, ByVal baseAddress As IntPtr) As Long
            Dim buffer = New Byte(15) {}
            Dim bytesRead As IntPtr
            ReadMemory(process, baseAddress, buffer, 8, bytesRead)
            Return BitConverter.ToInt64(buffer, 0)
        End Function

        Private Function GetRealAddress(ByVal process As IntPtr, ByVal baseAddress As IntPtr, ByVal offsets As Integer()) As Long
            Dim address = baseAddress.ToInt64()

            For Each offset In offsets
                address = ReadInt64(process, CType(address, IntPtr)) + offset
            Next

            Return address
        End Function

        Private Function GetPointer(ByVal base_ As Int64, ByVal offsets As Integer()) As Long
            If Handle <> IntPtr.Zero AndAlso ProcessRunning Then
                Dim realAddress As Long = GetRealAddress(Handle, CType((Base + base_), IntPtr), offsets)
                Return realAddress
            Else
                Return &HFFFFFFFFFFF
            End If
        End Function

        Sub IslemEkle(ByVal processname As String)
            pc(processname)
        End Sub

        Sub OpcodeKullan(ByVal opcode As Long, ByVal bytes As String)
            WM(opcode, ToOpcode(bytes))
        End Sub

        Sub PointerKullan(ByVal address As Long, ByVal offset1 As Integer, ByVal offset2 As Integer, ByVal offset3 As Integer, ByVal value As Integer)
            Dim Pointer As Long = GetPointer(address, New Integer() {offset1, offset2, offset3})
            Dim val As Byte() = New Byte(3) {}
            val = BitConverter.GetBytes(value)
            Dim written As Integer = 0
            WriteMemory64(CInt(Handle), Pointer, val, val.Length, written)
        End Sub
    End Module
End Namespace


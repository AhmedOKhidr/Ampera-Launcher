Imports System.IO
Imports System.Net

Public Class Main

    Dim bPathString As String = ".\ampex.jar"
    Dim directory As String = Application.StartupPath & "/"
    Dim checked As Boolean = False
    Dim client As New WebClient
    Dim watcher As New FileSystemWatcher(directory, "*.jar")

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        WebBrowser1.ScriptErrorsSuppressed = True
        WebBrowser1.Navigate("http://amperacoin.blogspot.com/?m=1")

        For Each foundFile As String In My.Computer.FileSystem.GetFiles(directory)

            If foundFile.EndsWith(".jar") Then

                ListBox1.Items.Add(Path.GetFileName(foundFile))

            End If




        Next


        For Each foundFile As String In My.Computer.FileSystem.GetFiles(directory)
            Dim java As String = ListBox1.SelectedItem
            Dim file As System.IO.StreamWriter

            If foundFile.EndsWith("launch.bat") Then
                file = My.Computer.FileSystem.OpenTextFileWriter("launch.bat", True)
                file.WriteLine("java -jar " & java & " -md -pd -bd -full & pause")
                file.Close()
            Else
                file = My.Computer.FileSystem.OpenTextFileWriter("launch.bat", True)
                file.Close()
            End If
        Next

        writeoutput("Launcher Ready..." & vbNewLine & vbNewLine)
        Dim message As String = "NOTICE: Please Select a .jar File from the list located on the right! A check will be performed apon Launching to confirm there is a .jar File selected!"
        MessageBox.Show(message, "*NOTICE*", MessageBoxButtons.OK, MessageBoxIcon.Information)

        FullModeToolStripMenuItem.Checked = True
        watcher.NotifyFilter = NotifyFilters.LastWrite
        AddHandler watcher.Changed, AddressOf file_complete
        watcher.EnableRaisingEvents = True

    End Sub

    Private Sub launch_java(arguments As String)

        Dim batch = New Process
        batch.StartInfo.FileName = "run.bat"
        batch.StartInfo.WorkingDirectory = directory
        batch.StartInfo.UseShellExecute = False
        batch.StartInfo.RedirectStandardOutput = True
        batch.StartInfo.RedirectStandardError = True
        batch.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        batch.StartInfo.CreateNoWindow = True
        AddHandler batch.OutputDataReceived, AddressOf batch_output
        batch.StartInfo.Arguments = arguments
        batch.Start()
        batch.BeginOutputReadLine()

    End Sub

    Delegate Sub SetTextCallback([text] As String)
    Private Sub UpdateTextBox(ByVal text As String)

        If console.InvokeRequired Then
            Dim d As New SetTextCallback(AddressOf UpdateTextBox)
            Invoke(d, New Object() {text})
        Else
            If console.Text = Nothing Then console.Text = text Else console.AppendText(vbNewLine & text)
        End If

    End Sub

    Private Sub batch_output(ByVal sender As Object, ByVal e As DataReceivedEventArgs)

        UpdateTextBox(e.Data)

    End Sub

    Private Sub writeoutput(text As String)

        console.AppendText(text)

    End Sub

    Private Sub download_update()

        Dim request As HttpWebRequest = WebRequest.Create("https://textuploader.com/dbprd/raw")
        Dim response As HttpWebResponse = request.GetResponse()
        Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
        Dim newestversion As String = reader.ReadToEnd()

        Dim save_directory As String = directory & newestversion.Replace("https://bitbucket.org/backspace119/ampera/downloads/", "")
        Dim url As New Uri(newestversion)

        Try

            Dim message As String = "Updating Please Wait..."
            MessageBox.Show(message, "*UPDATING*", MessageBoxButtons.OK, MessageBoxIcon.Information)
            client.DownloadFileAsync(url, save_directory)

        Catch ex As WebException

            Dim message As String = ex.Message
            MessageBox.Show(message, "*EXECEPTION CAUGHT*", MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try

    End Sub

    Private Sub file_complete()

        Dim message As String = "Updating Complete..."
        MessageBox.Show(message, "*COMPLETE*", MessageBoxButtons.OK, MessageBoxIcon.Information)

        update_list()

    End Sub

    Private Sub update_list()

        On Error Resume Next

        ListBox1.Items.Clear()

        For Each foundFile As String In My.Computer.FileSystem.GetFiles(directory)

            If foundFile.EndsWith(".jar") Then

                ListBox1.Items.Add(Path.GetFileName(foundFile))

            End If

        Next

    End Sub

    Private Sub select_mode(mode As String, java As String)

        If mode = "full" Then

            Dim message As String = "NOTICE: Full Mode will be enabled this can take about 30 minutes to finish setting up if its your first time." & vbNewLine & vbNewLine &
        "Are you sure you want to run this Java File in Full Mode?"
            Select Case MessageBox.Show(message, "Full Mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

                Case DialogResult.Yes
                    launch_java("java -jar " & java & " -md -pd -bd -full & pause")
                    writeoutput("Successfully Launched Java File Using Full Mode..." & vbNewLine & vbNewLine)

                Case DialogResult.No
                    'do nothing'

            End Select

        ElseIf mode = "light" Then
            Dim message3 As String = "NOTICE: Light Mode will be enabled, This version is a bit unstable so use Full Mode If you dont know what you are doing." & vbNewLine & vbNewLine &
        "Are you sure you want to run this Java File in Light Mode?"
            Select Case MessageBox.Show(message3, "Light Mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

                Case DialogResult.Yes
                    launch_java("java -jar " & java & " -md -pd -bd -light & pause")
                    writeoutput("Successfully Launched Java File Using Light Mode..." & vbNewLine & vbNewLine)

                Case DialogResult.No
                    'do nothing'

            End Select

        ElseIf mode = "testnet" Then

            Dim message1 As String = "NOTICE: Test Net will be enabled this only works if you have the custom .jar provided by Backspace." & vbNewLine & vbNewLine &
          "Are you sure you want to run this Java File in Test Net Mode?"
            Select Case MessageBox.Show(message1, "Full Mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

                Case DialogResult.Yes
                    launch_java("java -jar " & java & " -md -pd -bd -full -testnet & pause")
                    writeoutput("Successfully Launched Java File Using Test Net Mode..." & vbNewLine & vbNewLine)

                Case DialogResult.No
                    'do nothing'

            End Select

        ElseIf mode = "server" Then

            Dim message2 As String = "NOTICE: Server Mode will be enabled this will enable server functionality to run a Pool!" & vbNewLine & vbNewLine &
          "Are you sure you want to run this Java File in Server Mode Mode?"
            Select Case MessageBox.Show(message2, "Full Mode", MessageBoxButtons.YesNo, MessageBoxIcon.Information)

                Case DialogResult.Yes
                    launch_java("java -jar " & java & " -pr & pause")
                    writeoutput("Successfully Launched Java File Using Server Mode..." & vbNewLine & vbNewLine)

                Case DialogResult.No
                    'do nothing'

            End Select

        End If

    End Sub

    Private Sub LaunchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LaunchToolStripMenuItem.Click

        Dim items As Integer = ListBox1.CheckedItems.Count
        Dim java As String = ListBox1.SelectedItem

        If File.Exists(directory & "run.bat") Then

            If checked = False Then

                Dim message As String = "ERROR: Ampex Crypto Launcher! Has detected there is no .jar file selected please select a Valid .jar file to continue!"
                MessageBox.Show(message, "*ERROR*", MessageBoxButtons.OK, MessageBoxIcon.Error)

            Else

                If items < 2 Then

                    If FullModeToolStripMenuItem.Checked = True Then

                        Dim writer As New StreamWriter(directory & "run.bat")
                        writer.Write("java -jar " & java & " -md -pd -bd -full & pause")
                        writer.Close()

                        select_mode("full", java)

                    ElseIf LightModeToolStripMenuItem.Checked = True Then

                        Dim writer As New StreamWriter(directory & "run.bat")
                        writer.Write("java -jar " & java & " -md -pd -bd -light & pause")
                        writer.Close()

                        select_mode("light", java)

                    ElseIf TestNetToolStripMenuItem.Checked = True Then

                        Dim writer As New StreamWriter(directory & "run.bat")
                        writer.Write("java -jar " & java & " -md -pd -bd -full -testnet & pause")
                        writer.Close()

                        select_mode("testnet", java)

                    ElseIf ServerModeToolStripMenuItem.Checked = True Then

                        Dim writer As New StreamWriter(directory & "run.bat")
                        writer.Write("java -jar " & java & " -pr & pause")
                        writer.Close()

                        select_mode("server", java)

                    End If

                Else

                    Dim message As String = "ERROR: Only one Java File can be Selected per Launch!"
                    MessageBox.Show(message, "*ERROR*", MessageBoxButtons.OK, MessageBoxIcon.Error)

                End If

            End If

        Else

            Dim message As String = "ERROR: It appears the file Java File does not exist!"
            MessageBox.Show(message, "*ERROR*", MessageBoxButtons.OK, MessageBoxIcon.Error)

        End If

    End Sub

    Private Sub ListBox1_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles ListBox1.ItemCheck

        If e.NewValue = CheckState.Checked Then
            checked = True
        Else
            checked = False
        End If

    End Sub

    Private Sub UpdateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpdateToolStripMenuItem.Click

        download_update()

    End Sub

    Private Sub RefreshListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshListToolStripMenuItem.Click

        update_list()

    End Sub

    Private Sub ClearConsoleToolStripMenuItem_Click(sender As Object, e As EventArgs)

        console.Clear()
        console.Text = "Launcher Ready..."

    End Sub

    Private Sub FullModeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FullModeToolStripMenuItem.Click

        FullModeToolStripMenuItem.Checked = True
        LightModeToolStripMenuItem.Checked = False
        TestNetToolStripMenuItem.Checked = False
        ServerModeToolStripMenuItem.Checked = False

    End Sub

    Private Sub LightModeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LightModeToolStripMenuItem.Click

        FullModeToolStripMenuItem.Checked = False
        LightModeToolStripMenuItem.Checked = True
        TestNetToolStripMenuItem.Checked = False
        ServerModeToolStripMenuItem.Checked = False

    End Sub

    Private Sub TestNetToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TestNetToolStripMenuItem.Click

        FullModeToolStripMenuItem.Checked = False
        LightModeToolStripMenuItem.Checked = False
        TestNetToolStripMenuItem.Checked = True
        ServerModeToolStripMenuItem.Checked = False

    End Sub

    Private Sub ServerModeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ServerModeToolStripMenuItem.Click

        FullModeToolStripMenuItem.Checked = False
        LightModeToolStripMenuItem.Checked = False
        TestNetToolStripMenuItem.Checked = False
        ServerModeToolStripMenuItem.Checked = True

    End Sub

    Private Sub DiscordToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DiscordToolStripMenuItem.Click

        Process.Start("https://discordapp.com/invite/4pgGznh")

    End Sub

    Private Sub BitBucketWikiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BitBucketWikiToolStripMenuItem.Click

        Process.Start("https://bitbucket.org/backspace119/ampera/wiki/Home")

    End Sub

    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        On Error Resume Next

        If Process.GetProcessesByName("java").Count > 0 Then
            Process.GetProcessesByName("java")(0).Kill()
        End If

        Dispose()

    End Sub

End Class


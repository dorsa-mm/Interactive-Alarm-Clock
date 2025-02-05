Imports System.Drawing.Imaging

Public Class AlarmPanel
    Inherits Panel

    ' AlarmPanel controls
    Public chkAlarmOn As CheckBox
    Public lstSounds As ListBox
    Public picAnimation As PictureBox
    Public lblHour As Label
    Public lblMinute As Label
    Public lblColon As Label
    Public rbtnAM As RadioButton
    Public rbtnPM As RadioButton
    Public btnSet As Button
    Public btnReset As Button
    Public KeypadPanel As Panel
    Private digitButtons(9) As Button ' Array for digit buttons
    Public btnOk As Button
    Public btnCancel As Button
    Private enteredTime As String = ""
    Private currentInputField As String = "" ' To track hour or minute input 
    Private alarmImage As Image ' To hold the static image
    Private alarmAnimation As Image ' To hold the animated image
    Private alarmTime As DateTime ' Stores the set alarm time
    Private isAlarmRinging As Boolean = False
    Public isAlarmHandled As Boolean = False
    Private originalAlarmTime As DateTime
    Private isSettingInProgress As Boolean = False
    Private isAlarmTimeSet As Boolean = False


    ' Initialize the AlarmPanel
    Public Sub New(alarmIndex As Integer)

        Me.Size = New Drawing.Size(350, 350)

        ' Initialization of time
        originalAlarmTime = DateTime.MinValue


        ' Initialize labels for hour and minute 
        lblHour = New Label With {
            .Text = "00",
            .Font = New Font("Arial", 20, FontStyle.Bold),
            .Location = New Point(45, 15),
            .AutoSize = True
        }
        lblMinute = New Label With {
            .Text = "00",
            .Font = New Font("Arial", 20, FontStyle.Bold),
            .Location = New Point(103, 15),
            .AutoSize = True
        }

        ' Initialize colon
        lblColon = New Label With {
            .Text = ":",
            .Font = New Font("Arial", 20, FontStyle.Bold),
            .Location = New Point(85, 15),
            .AutoSize = True
        }

        ' Initialize AM/PM radio buttons
        rbtnAM = New RadioButton With {
            .Text = "AM",
            .Location = New Point(170, 40),
            .AutoSize = True,
            .Checked = True ' Set AM as the default 
        }
        rbtnPM = New RadioButton With {
            .Text = "PM",
            .Location = New Point(170, 60),
            .AutoSize = True
        }

        ' Initialize Set and Reset buttons 
        btnSet = New Button With {
            .Text = "Set",
            .Location = New Point(38, 50),
            .Size = New Drawing.Size(60, 25)
        }
        btnReset = New Button With {
            .Text = "Reset",
            .Location = New Point(98, 50),
            .Size = New Drawing.Size(60, 25)
        }

        ' Initialize picture box for animation (on the left side)
        picAnimation = New PictureBox With {
            .Location = New Point(10, 90),
            .Size = New Drawing.Size(95, 88),
            .SizeMode = PictureBoxSizeMode.StretchImage,
            .Enabled = False
        }
        AddHandler picAnimation.Click, AddressOf StopAlarm ' Add click event to stop the alarm

        ' Initialize listbox for sound selection (to the right of animation)
        lstSounds = New ListBox With {
            .Location = New Point(120, 100),
            .Size = New Drawing.Size(100, 60)
        }
        lstSounds.Items.AddRange(New String() {"Crickets", "Bird", "Cow", "Drum Roll", "Boat Horn", "Train", "Fanfare"}) ' All sound options


        ' Initialize alarm on/off checkbox
        chkAlarmOn = New CheckBox With {
            .Text = "Alarm ON",
            .Location = New Point(120, 160),
            .AutoSize = True
        }

        ' Initialize keypad panel
        KeypadPanel = New Panel With {
            .Size = New Drawing.Size(200, 200),
            .Location = New Point(25, 200),
            .Visible = False ' Hide initially
        }

        ' Add digit buttons (0-9) in a 3x4 grid layout
        Dim buttonSize As New Size(50, 30)
        For i As Integer = 1 To 9
            digitButtons(i) = New Button With {
                .Text = i.ToString(),
                .Size = buttonSize,
                .Location = New Point(((i - 1) Mod 3) * 50, ((i - 1) \ 3) * 40)
            }
            AddHandler digitButtons(i).Click, AddressOf DigitButton_Click
            KeypadPanel.Controls.Add(digitButtons(i))
        Next

        ' Add 0, Cancel, and OK buttons
        digitButtons(0) = New Button With {
            .Text = "0",
            .Size = buttonSize,
            .Location = New Point(50, 120) ' Centered
        }
        AddHandler digitButtons(0).Click, AddressOf DigitButton_Click
        KeypadPanel.Controls.Add(digitButtons(0))

        btnCancel = New Button With {
            .Text = "Cancel",
            .Size = buttonSize,
            .Location = New Point(0, 120) ' Leftmost button
        }
        AddHandler btnCancel.Click, AddressOf CancelButton_Click
        KeypadPanel.Controls.Add(btnCancel)

        btnOk = New Button With {
            .Text = "OK",
            .Size = buttonSize,
            .Location = New Point(100, 120) ' Rightmost button
        }
        AddHandler btnOk.Click, AddressOf OkButton_Click
        KeypadPanel.Controls.Add(btnOk)


        ' Load an image for each alarm - Get the first frame of the GIF for each Alarm and use the full GIF as animation 
        Select Case alarmIndex
            Case 1
                alarmImage = GetFirstFrameFromGif(My.Resources.BoatTintin)
                alarmAnimation = My.Resources.BoatTintin
            Case 2
                alarmImage = GetFirstFrameFromGif(My.Resources.Speaker)
                alarmAnimation = My.Resources.Speaker
            Case 3
                alarmImage = GetFirstFrameFromGif(My.Resources.Twister)
                alarmAnimation = My.Resources.Twister
        End Select

        ' Set initial static image
        picAnimation.Image = alarmImage


        ' Add controls to the AlarmPanel
        Me.Controls.Add(lblHour)
        Me.Controls.Add(lblMinute)
        Me.Controls.Add(lblColon)
        Me.Controls.Add(rbtnAM)
        Me.Controls.Add(rbtnPM)
        Me.Controls.Add(btnSet)
        Me.Controls.Add(btnReset)
        Me.Controls.Add(KeypadPanel)
        Me.Controls.Add(picAnimation)
        Me.Controls.Add(lstSounds)
        Me.Controls.Add(chkAlarmOn)

        ' Functionality for Set button
        AddHandler btnSet.Click, AddressOf SetButton_Click

        ' Functionality for Reset button
        AddHandler btnReset.Click, AddressOf ResetButton_Click

        ' Start the clock timer
        Dim clockTimer As New Timer With {.Interval = 60000} ' 
        AddHandler clockTimer.Tick, AddressOf UpdateClock
        clockTimer.Start()
    End Sub


    ' Update the clock every minute
    Private Sub UpdateClock(sender As Object, e As EventArgs)
        If Not isSettingInProgress And chkAlarmOn.Checked Then
            If AlarmTimeReached() Then
                StartAlarm()
            End If
        End If
    End Sub


    ' Check if the current time matches the alarm time
    Private Function AlarmTimeReached() As Boolean
        Dim currentTime As DateTime = DateTime.Now
        Dim setTime As DateTime = DateTime.ParseExact(lblHour.Text & ":" & lblMinute.Text & If(rbtnAM.Checked, " AM", " PM"), "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture)
        Return currentTime.ToString("hh:mm tt") = setTime.ToString("hh:mm tt")
    End Function


    ' Start the alarm (animation and sound)
    Public Sub StartAlarm()
        If Not isAlarmRinging And Not isAlarmHandled Then
            isAlarmRinging = True
            picAnimation.Image = alarmAnimation
            picAnimation.Enabled = True ' Make it clickable

            ' Play the selected sound from the listbox - 7 options 
            Dim selectedSound As String = lstSounds.SelectedItem.ToString()
            Select Case selectedSound
                Case "Crickets"
                    My.Computer.Audio.Play(My.Resources.crickets, AudioPlayMode.BackgroundLoop)
                Case "Bird"
                    My.Computer.Audio.Play(My.Resources.bird, AudioPlayMode.BackgroundLoop)
                Case "Cow"
                    My.Computer.Audio.Play(My.Resources.cow, AudioPlayMode.BackgroundLoop)
                Case "Drum Roll"
                    My.Computer.Audio.Play(My.Resources.drum_roll, AudioPlayMode.BackgroundLoop)
                Case "Boat Horn"
                    My.Computer.Audio.Play(My.Resources.boat_horn, AudioPlayMode.BackgroundLoop)
                Case "Train"
                    My.Computer.Audio.Play(My.Resources.train, AudioPlayMode.BackgroundLoop)
                Case "Fanfare"
                    My.Computer.Audio.Play(My.Resources.fanfare, AudioPlayMode.BackgroundLoop)
            End Select
        End If
    End Sub


    ' Stop the alarm (stop sound and animation)
    Private Sub StopAlarm(sender As Object, e As EventArgs)
        If isAlarmRinging Then
            isAlarmRinging = False
            isAlarmHandled = True
            picAnimation.Image = alarmImage
            picAnimation.Enabled = False
            My.Computer.Audio.Stop()
        End If
    End Sub


    ' Set the alarm time 
    Private Sub SetButton_Click(sender As Object, e As EventArgs)
        isAlarmHandled = False ' Reset this so the alarm can ring again

        ' Save the current alarm time before changing
        If lblHour.Text <> "00" Or lblMinute.Text <> "00" Then
            originalAlarmTime = DateTime.ParseExact(lblHour.Text & ":" & lblMinute.Text & If(rbtnAM.Checked, " AM", " PM"), "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture)
        Else
            originalAlarmTime = DateTime.Now 
        End If

        isSettingInProgress = True
        lblHour.Text = "00"
        lblMinute.Text = "00"
        rbtnAM.Checked = True ' Default to AM

        KeypadPanel.Visible = True
        enteredTime = "" ' 
        currentInputField = "Hour"
        EnableValidKeysForHour(True)
    End Sub


    ' Reset the alarm time
    Private Sub ResetButton_Click(sender As Object, e As EventArgs)
        lblHour.Text = "00"
        lblMinute.Text = "00"
        rbtnAM.Checked = True
        alarmTime = Nothing

        ' Reset originalAlarmTime and flag, as the time is now reset
        originalAlarmTime = DateTime.ParseExact("00:00 AM", "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture)
        isAlarmTimeSet = False ' Mark that no alarm time is set
        isAlarmHandled = False ' Reset the flag so the alarm can ring again
    End Sub



    ' Handle digit button clicks
    Private Sub DigitButton_Click(sender As Object, e As EventArgs)
        Dim button As Button = DirectCast(sender, Button)
        Dim digit As String = button.Text

        If currentInputField = "Hour" Then
            If enteredTime.Length = 0 Then
                ' Set first hour digit
                enteredTime &= digit
                lblHour.Text = digit & "0"
                If digit = "1" Then
                    EnableValidKeysForHour(False) ' Enable 0-2 for second hour digit
                ElseIf digit = "0" Then
                    EnableValidKeysForHour(False) ' Enable 1-9 for second hour digit if first digit is 0
                End If
            Else
                ' Set second hour digit
                enteredTime &= digit
                lblHour.Text = enteredTime
                EnableValidKeysForMinute()
                currentInputField = "Minute"
                enteredTime = ""
            End If
        ElseIf currentInputField = "Minute" Then
            If enteredTime.Length = 0 Then
                ' Set first minute digit
                enteredTime &= digit
                lblMinute.Text = digit & "0"
                EnableValidKeysForMinuteSecondDigit() ' Enable valid keys for the second minute digit
            Else
                ' Set second minute digit
                enteredTime &= digit
                lblMinute.Text = enteredTime
                DisableAllDigitButtons() ' Disable all digit buttons after second minute digit is set
            End If
        End If
    End Sub


    ' Disable all digit buttons after four digits are entered
    Private Sub DisableAllDigitButtons()
        For i As Integer = 0 To 9
            digitButtons(i).Enabled = False
        Next

        btnOk.Enabled = True
        btnCancel.Enabled = True
    End Sub


    ' Enable valid keys for the first hour digit (1 or 0)
    Private Sub EnableValidKeysForHour(isFirstDigit As Boolean)
        ' Disable all keys
        For i As Integer = 0 To 9
            digitButtons(i).Enabled = False
        Next

        If isFirstDigit Then
            ' Enable only 1 and 0 for the first digit of hour
            digitButtons(1).Enabled = True
            digitButtons(0).Enabled = True
        Else
            ' If first digit is 1, enable 0-2 for second hour digit
            If lblHour.Text.StartsWith("1") Then
                digitButtons(0).Enabled = True
                digitButtons(1).Enabled = True
                digitButtons(2).Enabled = True
            ElseIf lblHour.Text.StartsWith("0") Then
                ' If first digit is 0, enable only 1-9 for second hour digit
                For i As Integer = 1 To 9
                    digitButtons(i).Enabled = True
                Next
                digitButtons(0).Enabled = False ' Disable 0 as second digit
            End If
        End If
    End Sub


    ' Enable valid keys for the first minute digit (0-5)
    Private Sub EnableValidKeysForMinute()
        For i As Integer = 0 To 9
            digitButtons(i).Enabled = False
        Next
        For i As Integer = 0 To 5
            digitButtons(i).Enabled = True
        Next
    End Sub

    ' Enable valid keys for the second minute digit (0-9)
    Private Sub EnableValidKeysForMinuteSecondDigit()
        For i As Integer = 0 To 9
            digitButtons(i).Enabled = True
        Next
    End Sub



    ' OK button
    Private Sub OkButton_Click(sender As Object, e As EventArgs)
        alarmTime = DateTime.ParseExact(lblHour.Text & ":" & lblMinute.Text & If(rbtnAM.Checked, " AM", " PM"), "hh:mm tt", System.Globalization.CultureInfo.InvariantCulture)
        originalAlarmTime = alarmTime
        isAlarmTimeSet = True
        KeypadPanel.Visible = False

        isAlarmHandled = False ' Reset the flag so the alarm can ring again
    End Sub


    ' Cancel button 
    Private Sub CancelButton_Click(sender As Object, e As EventArgs)
        If Not isAlarmTimeSet Then
            lblHour.Text = "00"
            lblMinute.Text = "00"
            rbtnAM.Checked = True ' Default to AM
        Else
            lblHour.Text = originalAlarmTime.ToString("hh")
            lblMinute.Text = originalAlarmTime.ToString("mm")
            If originalAlarmTime.Hour >= 12 Then
                rbtnPM.Checked = True
            Else
                rbtnAM.Checked = True
            End If
        End If

        KeypadPanel.Visible = False
        enteredTime = ""
        isSettingInProgress = False
    End Sub



    ' Function to extract the first frame from a GIF
    Private Function GetFirstFrameFromGif(gifImage As Image) As Image
        Dim dimension As New FrameDimension(gifImage.FrameDimensionsList(0))
        gifImage.SelectActiveFrame(dimension, 0) ' Select the first frame
        Return CType(gifImage.Clone(), Image) ' Clone the first frame as a static image
    End Function

End Class



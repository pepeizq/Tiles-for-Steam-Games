﻿Imports Microsoft.Toolkit.Uwp.UI.Controls
Imports Windows.Storage
Imports Windows.Storage.AccessCache
Imports Windows.Storage.Pickers
Imports Windows.UI

Module Steam

    Dim clave As String = "carpeta35"

    Public Async Sub Generar(boolBuscarCarpeta As Boolean)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim botonAñadir As Button = pagina.FindName("botonAñadirCarpetaSteam")
        botonAñadir.IsEnabled = False

        Dim botonBorrar As Button = pagina.FindName("botonBorrarCarpetasSteam")
        botonBorrar.IsEnabled = False

        Dim pr As ProgressRing = pagina.FindName("prTilesSteam")
        pr.Visibility = Visibility.Visible

        Dim gv As GridView = pagina.FindName("gridViewTilesSteam")

        Dim tbCarpetas As TextBlock = pagina.FindName("tbCarpetasDetectadasSteam")

        If Not tbCarpetas.Text = Nothing Then
            tbCarpetas.Text = ""
        End If

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()
        Dim numCarpetas As ApplicationDataContainer = ApplicationData.Current.LocalSettings

        If boolBuscarCarpeta = True Then
            Try
                Dim picker As FolderPicker = New FolderPicker()

                picker.FileTypeFilter.Add("*")
                picker.ViewMode = PickerViewMode.List

                Dim carpeta As StorageFolder = Await picker.PickSingleFolderAsync()
                Dim carpetaTemp As StorageFolder = Nothing

                Dim i As Integer = 0
                While i < (numCarpetas.Values("numCarpetas") + 1)

                    Try
                        carpetaTemp = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave + i.ToString)
                        tbCarpetas.Text = tbCarpetas.Text + carpetaTemp.Path + Environment.NewLine
                    Catch ex As Exception
                        StorageApplicationPermissions.FutureAccessList.AddOrReplace(clave + i.ToString, carpeta)
                        numCarpetas.Values("numCarpetas") = i + 1
                        tbCarpetas.Text = tbCarpetas.Text + carpeta.Path + Environment.NewLine
                        Exit While
                    End Try

                    i += 1
                End While

            Catch ex As Exception

            End Try
        Else
            Dim i As Integer = 0
            While i < (numCarpetas.Values("numCarpetas") + 1)
                Try
                    Dim carpetaTemp As StorageFolder = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave + i.ToString)
                    tbCarpetas.Text = tbCarpetas.Text + carpetaTemp.Path + Environment.NewLine
                Catch ex As Exception

                End Try
                i += 1
            End While
        End If

        If tbCarpetas.Text = Nothing Then
            tbCarpetas.Text = recursos.GetString("NoFoldersDetected")
        Else
            tbCarpetas.Text = tbCarpetas.Text.Trim
        End If

        '-------------------------------------------------------------

        Dim listaTemp As New List(Of String)
        Dim listaFinal As List(Of Tile) = New List(Of Tile)

        Dim h As Integer = 0
        While h < numCarpetas.Values("numCarpetas") + 1
            Dim listaCarpetas As New List(Of StorageFolder)
            Dim carpetaMaestra As StorageFolder = Nothing

            Try
                carpetaMaestra = Await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clave + h.ToString)
                listaCarpetas.Add(carpetaMaestra)
            Catch ex As Exception

            End Try

            Dim i As Integer = 0

            'If Not carpetaMaestra Is Nothing Then
            '    Dim ficheros As IReadOnlyList(Of StorageFile) = Await carpetaMaestra.GetFilesAsync()

            '    For Each fichero As StorageFile In ficheros
            '        If fichero.Name.Contains("libraryfolders") Then
            '            Try
            '                Dim lineas As String = Await FileIO.ReadTextAsync(fichero)

            '                If Not lineas = Nothing Then
            '                    i = 0
            '                    While i < 10
            '                        If lineas.Contains(ChrW(34) + i.ToString + ChrW(34)) Then
            '                            Dim temp, temp2, temp3 As String
            '                            Dim int, int2, int3 As Integer

            '                            int = lineas.IndexOf(ChrW(34) + i.ToString + ChrW(34))
            '                            temp = lineas.Remove(0, int + 3)

            '                            int2 = temp.IndexOf(ChrW(34))
            '                            temp2 = temp.Remove(0, int2 + 1)

            '                            int3 = temp2.IndexOf(ChrW(34))
            '                            temp3 = temp2.Remove(int3, temp2.Length - int3)

            '                            temp3 = temp3.Replace("\\", "\")

            '                            StorageApplicationPermissions.FutureAccessList.AddOrReplace("carpetaSecundaria" + i.ToString, Await StorageFolder.GetFolderFromPathAsync(temp3))
            '                            Dim carpetaSecundaria As StorageFolder = Await StorageFolder.GetFolderFromPathAsync(temp3 + "\steamapps")
            '                            MessageBox.EnseñarMensaje(carpetaSecundaria.Path)
            '                        End If
            '                        i += 1
            '                    End While
            '                End If
            '            Catch ex As Exception

            '            End Try
            '        End If
            '    Next
            'End If

            If listaCarpetas.Count > 0 Then
                For Each carpeta As StorageFolder In listaCarpetas
                    If Not carpeta Is Nothing Then
                        Dim ficheros As IReadOnlyList(Of StorageFile) = Await carpeta.GetFilesAsync()

                        i = 0
                        If gv.Items.Count > 0 Then
                            While i < gv.Items.Count
                                Dim boton As Button = gv.Items(i)
                                Dim tile As Tile = boton.Tag

                                listaTemp.Add(tile.Titulo + "/*/" + tile.ID)
                                i += 1
                            End While
                        End If

                        For Each fichero As StorageFile In ficheros
                            If fichero.FileType.Contains(".acf") Then
                                Try
                                    Dim lineas As String = Await FileIO.ReadTextAsync(fichero)

                                    Dim temp, temp2 As String
                                    Dim int, int2 As Integer

                                    int = lineas.IndexOf("name")
                                    temp = lineas.Remove(0, int + 5)

                                    int2 = temp.IndexOf("StateFlags")
                                    temp2 = temp.Remove(int2 - 1, temp.Length - int2 + 1)

                                    temp2 = temp2.Trim
                                    temp2 = temp2.Remove(0, 1)
                                    temp2 = temp2.Remove(temp2.Length - 1, 1)

                                    Dim titulo As String = temp2.Trim

                                    Dim temp3, temp4 As String
                                    Dim int3, int4 As Integer

                                    int3 = lineas.IndexOf("appid")
                                    temp3 = lineas.Remove(0, int3 + 6)

                                    int4 = temp3.IndexOf("Universe")
                                    temp4 = temp3.Remove(int4 - 1, temp3.Length - int4 + 1)

                                    temp4 = temp4.Trim
                                    temp4 = temp4.Remove(0, 1)
                                    temp4 = temp4.Remove(temp4.Length - 1, 1)

                                    Dim id As String = temp4.Trim

                                    listaTemp.Add(titulo + "/*/" + id)
                                Catch ex As Exception

                                End Try
                            End If
                        Next

                        i = 0
                        While i < listaTemp.Count
                            If Not listaTemp(i) = Nothing Then
                                If listaTemp(i).Contains("/*/") Then
                                    Dim int As Integer

                                    int = listaTemp(i).IndexOf("/*/")

                                    Dim titulo As String = listaTemp(i).Remove(int, listaTemp(i).Length - int)
                                    Dim id As String = listaTemp(i).Remove(0, int + 3)

                                    Dim tituloBool As Boolean = False
                                    Dim g As Integer = 0
                                    While g < listaFinal.Count
                                        If listaFinal(g).Titulo = titulo Then
                                            tituloBool = True
                                        End If
                                        g += 1
                                    End While

                                    If tituloBool = False Then
                                        Dim imagenWide As Uri = New Uri("http://cdn.edgecast.steamstatic.com/steam/apps/" + id + "/header.jpg", UriKind.RelativeOrAbsolute)
                                        Dim imagenLarge As Uri = New Uri("http://cdn.akamai.steamstatic.com/steam/apps/" + id + "/page_bg_generated_v6b.jpg", UriKind.RelativeOrAbsolute)

                                        Dim juego As New Tile(titulo, id, New Uri("steam://rungameid/" + id), imagenWide, imagenLarge, "Steam", Nothing)
                                        juego.Tile = juego

                                        listaFinal.Add(juego)
                                    End If
                                End If
                            End If
                            i += 1
                        End While
                    End If
                Next
            End If
            h += 1
        End While

        Dim panelAvisoNoJuegos As DropShadowPanel = pagina.FindName("panelAvisoNoJuegos")
        Dim gridSeleccionar As Grid = pagina.FindName("gridSeleccionarJuego")

        If listaFinal.Count > 0 Then
            panelAvisoNoJuegos.Visibility = Visibility.Collapsed
            gridSeleccionar.Visibility = Visibility.Visible

            listaFinal.Sort(Function(x, y) x.Titulo.CompareTo(y.Titulo))

            gv.Items.Clear()

            For Each juego In listaFinal
                Dim boton As New Button

                Dim imagen As New ImageEx

                Try
                    imagen.Source = New BitmapImage(juego.ImagenWide)
                Catch ex As Exception

                End Try

                imagen.IsCacheEnabled = True
                imagen.Stretch = Stretch.UniformToFill
                imagen.Padding = New Thickness(0, 0, 0, 0)

                boton.Tag = juego
                boton.Content = imagen
                boton.Padding = New Thickness(0, 0, 0, 0)
                boton.BorderThickness = New Thickness(1, 1, 1, 1)
                boton.BorderBrush = New SolidColorBrush(Colors.Black)
                boton.Background = New SolidColorBrush(Colors.Transparent)

                Dim tbToolTip As TextBlock = New TextBlock With {
                    .Text = juego.Titulo,
                    .FontSize = 16
                }

                ToolTipService.SetToolTip(boton, tbToolTip)
                ToolTipService.SetPlacement(boton, PlacementMode.Mouse)

                AddHandler boton.Click, AddressOf BotonTile_Click

                gv.Items.Add(boton)
            Next

            If boolBuscarCarpeta = True Then
                Toast(listaFinal.Count.ToString + " " + recursos.GetString("GamesDetected"), Nothing)
            End If
        Else
            panelAvisoNoJuegos.Visibility = Visibility.Visible
            gridSeleccionar.Visibility = Visibility.Collapsed

            If boolBuscarCarpeta = True Then
                Toast(recursos.GetString("ErrorSteam1"), Nothing)
            End If
        End If

        botonAñadir.IsEnabled = True
        botonBorrar.IsEnabled = True
        pr.Visibility = Visibility.Collapsed

    End Sub

    Private Sub BotonTile_Click(sender As Object, e As RoutedEventArgs)

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content

        Dim gv As GridView = pagina.FindName("gridViewTilesSteam")

        Dim botonJuego As Button = e.OriginalSource

        Dim borde As Thickness = New Thickness(6, 6, 6, 6)
        If botonJuego.BorderThickness = borde Then
            botonJuego.BorderThickness = New Thickness(1, 1, 1, 1)
            botonJuego.BorderBrush = New SolidColorBrush(Colors.Black)

            Dim grid As Grid = pagina.FindName("gridAñadirTiles")
            grid.Visibility = Visibility.Collapsed

            Dim gridSeleccionar As Grid = pagina.FindName("gridSeleccionarJuego")
            gridSeleccionar.Visibility = Visibility.Visible
        Else
            For Each item In gv.Items
                Dim itemBoton As Button = item
                itemBoton.BorderThickness = New Thickness(1, 1, 1, 1)
                itemBoton.BorderBrush = New SolidColorBrush(Colors.Black)
            Next

            botonJuego.BorderThickness = New Thickness(6, 6, 6, 6)
            botonJuego.BorderBrush = New SolidColorBrush(App.Current.Resources("ColorSecundario"))

            Dim botonAñadirTile As Button = pagina.FindName("botonAñadirTile")
            Dim juego As Tile = botonJuego.Tag
            botonAñadirTile.Tag = juego

            Dim imageJuegoSeleccionado As ImageEx = pagina.FindName("imageJuegoSeleccionado")
            Dim imagenCapsula As String = juego.ImagenWide.ToString
            imagenCapsula = imagenCapsula.Replace("header.jpg", "capsule_184x69.jpg")
            imageJuegoSeleccionado.Source = New BitmapImage(New Uri(imagenCapsula))

            Dim tbJuegoSeleccionado As TextBlock = pagina.FindName("tbJuegoSeleccionado")
            tbJuegoSeleccionado.Text = juego.Titulo

            Dim gridAñadir As Grid = pagina.FindName("gridAñadirTiles")
            gridAñadir.Visibility = Visibility.Visible

            Dim gridSeleccionar As Grid = pagina.FindName("gridSeleccionarJuego")
            gridSeleccionar.Visibility = Visibility.Collapsed
        End If

    End Sub

    Public Sub Borrar()

        StorageApplicationPermissions.FutureAccessList.Clear()

        Dim recursos As Resources.ResourceLoader = New Resources.ResourceLoader()
        Dim numCarpetas As ApplicationDataContainer = ApplicationData.Current.LocalSettings
        numCarpetas.Values("numCarpetas") = 0

        Dim frame As Frame = Window.Current.Content
        Dim pagina As Page = frame.Content
        Dim tbCarpetas As TextBlock = pagina.FindName("tbCarpetasDetectadasSteam")

        tbCarpetas.Text = recursos.GetString("NoFoldersDetected")

        Dim gv As GridView = pagina.FindName("gridViewTilesSteam")
        gv.Items.Clear()

        Generar(False)

    End Sub

    Public Async Function SacarIcono(id As String) As Task(Of Uri)

        Dim html As String = Await Decompiladores.HttpClient(New Uri("http://store.steampowered.com/app/" + id + "/"))
        Dim uriIcono As Uri = Nothing

        If Not html = Nothing Then
            If html.Contains("<div class=" + ChrW(34) + "apphub_AppIcon") Then
                Dim temp, temp2 As String
                Dim int, int2 As Integer

                int = html.IndexOf("<div class=" + ChrW(34) + "apphub_AppIcon")
                temp = html.Remove(0, int)

                int = temp.IndexOf("<img src=")
                temp = temp.Remove(0, int + 10)

                int2 = temp.IndexOf(ChrW(34))
                temp2 = temp.Remove(int2, temp.Length - int2)

                uriIcono = New Uri(temp2.Trim)
            End If
        End If

        If uriIcono = Nothing Then
            html = Await Decompiladores.HttpClient(New Uri("https://steamdb.info/app/" + id + "/"))

            If Not html = Nothing Then
                If html.Contains("<img class=" + ChrW(34) + "app-icon avatar") Then
                    Dim temp, temp2 As String
                    Dim int, int2 As Integer

                    int = html.IndexOf("<img class=" + ChrW(34) + "app-icon avatar")
                    temp = html.Remove(0, int)

                    int = temp.IndexOf("src=")
                    temp = temp.Remove(0, int + 5)

                    int2 = temp.IndexOf(ChrW(34))
                    temp2 = temp.Remove(int2, temp.Length - int2)

                    uriIcono = New Uri(temp2.Trim)
                End If
            End If
        End If

        Return uriIcono
    End Function

End Module

'
' DotNetNuke® - http://www.dnnsoftware.com
' Copyright (c) 2002-2013
' by DNNCorp
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports System
Imports System.Web.UI
Imports System.Web.UI.WebControls

Namespace DotNetNuke.Modules.Events

    Partial Class EventRpt
        Inherits EventBase

#Region "Event Handlers"
        Private _selectedEvents As ArrayList


        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            Try
                SetupViewControls(EventIcons, EventIcons2, SelectCategory, SelectLocation)

                If Page.IsPostBack = False Then
                    BindDataGrid()
                End If
            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub
#End Region

#Region "Helper Functions"
        Private _rptItemCount As Integer = 0
        Public Property PageNumber() As Integer
            Get
                If Not (ViewState("PageNumber")) Is Nothing Then
                    Return CInt(ViewState("PageNumber"))
                Else
                    Return 0
                End If
            End Get
            Set(ByVal value As Integer)
                ViewState("PageNumber") = value
            End Set
        End Property

        Private Sub BindDataGrid()
            'Default sort from settings
            Dim sortDirection As SortDirection
            If Settings.EventsListSortDirection = "ASC" Then
                sortDirection = sortDirection.Ascending
            Else
                sortDirection = sortDirection.Descending
            End If

            Dim sortExpression As EventInfo.SortFilter = CType(GetListSortExpression(Settings.EventsListSortColumn), EventInfo.SortFilter)

            ' Get Events/Sub-Calendar Events
            _selectedEvents = Get_ListView_Events(SelectCategory.SelectedCategory, SelectLocation.SelectedLocation)

            EventInfo.SortExpression = sortExpression
            EventInfo.SortDirection = sortDirection
            _selectedEvents.Sort()

            Dim tcc As New TokenReplaceControllerClass(ModuleId, LocalResourceFile)
            Dim eventTable As DataTable = New DataTable("Events")
            With eventTable.Columns
                .Add("EventText", Type.GetType("System.String"))
                .Add("Tooltip", Type.GetType("System.String"))
            End With

            If Settings.Eventtooltiplist Then
                toolTipManager.TargetControls.Clear()
            End If

            Dim dgRow As DataRow
            Dim clientIdCount As Integer = 1
            For Each objEvent As EventInfo In _selectedEvents
                dgRow = eventTable.NewRow()
                Dim blAddSubModuleName As Boolean = False
                If objEvent.ModuleID <> ModuleId And objEvent.ModuleTitle <> Nothing And Settings.Addsubmodulename Then
                    blAddSubModuleName = True
                End If
                Dim isEvtEditor As Boolean = IsEventEditor(objEvent, False)
                Dim tmpText As String = Settings.Templates.txtListRptBody
                Dim tmpTooltip As String = ""
                If Settings.Eventtooltiplist Then
                    tmpTooltip = ToolTipCreate(objEvent, Settings.Templates.txtTooltipTemplateTitle, Settings.Templates.txtTooltipTemplateBody, isEvtEditor)
                    dgRow("Tooltip") = tmpTooltip
                End If
                If Not Settings.ListViewTable Then
                    Dim tooltip As String = HttpUtility.HtmlEncode(tmpTooltip)
                    tmpText = AddTooltip(clientIdCount, tooltip, tmpText)
                    clientIdCount += 1
                End If

                dgRow("EventText") = tcc.TokenReplaceEvent(objEvent, tmpText, Nothing, blAddSubModuleName, isEvtEditor)

                eventTable.Rows.Add(dgRow)
            Next

            Dim pgEvents As New PagedDataSource
            Dim dvEvents As New DataView(eventTable)
            pgEvents.DataSource = dvEvents
            pgEvents.AllowPaging = True
            pgEvents.PageSize = Settings.RptColumns * Settings.RptRows
            pgEvents.CurrentPageIndex = PageNumber
            If pgEvents.PageCount > 1 Then
                rptTRPager.Visible = True
                Dim pages As New ArrayList
                For i As Integer = 0 To pgEvents.PageCount - 1
                    pages.Add(i + 1)
                Next
                rptPager.DataSource = pages
                rptPager.DataBind()
            Else
                rptTRPager.Visible = False
            End If

            If pgEvents.CurrentPageIndex + 1 < pgEvents.PageCount Then
                _rptItemCount = pgEvents.PageSize
            Else
                _rptItemCount = eventTable.Rows.Count - (pgEvents.CurrentPageIndex * pgEvents.PageSize)
            End If

            rptEvents.DataSource = pgEvents
            rptEvents.DataBind()
        End Sub

#End Region

#Region "Control Events"
        Private Sub SelectCategory_CategorySelected(ByVal sender As Object, ByVal e As CommandEventArgs) Handles SelectCategory.CategorySelectedChanged
            'Store the other selection(s) too.
            SelectLocation.StoreLocations()
            BindDataGrid()
        End Sub
        Private Sub SelectLocation_LocationSelected(ByVal sender As Object, ByVal e As CommandEventArgs) Handles SelectLocation.LocationSelectedChanged
            'Store the other selection(s) too.
            SelectCategory.StoreCategories()
            BindDataGrid()
        End Sub

        Private _rptCurrentItemCount As Integer = 0
        Private _rptAlternate As Boolean = True

        Private Sub rptEvents_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptEvents.ItemDataBound
            Dim rptColumns As Integer = Settings.RptColumns
            Dim columnWidth As String = """" & CInt(100 / Settings.RptColumns).ToString & "%"""
            Select Case e.Item.ItemType
                Case ListItemType.Header
                    Const rptHeaderTable As String = "<table class=""RptRepeater"">"
                    Dim rptHeaderStart As String = "<tr id=""rptTRHeader"" ><th id=""rptTDHeader"" class=""RptHeader"" colspan=""" & Settings.RptColumns.ToString & """>"
                    Const rptHeaderEnd As String = "</th></tr>"

                    Dim rptHeaderBody As String = Settings.Templates.txtListRptHeader.Replace("[event:repeaterheadertext]", Localization.GetString("TokenListRptHeader", LocalResourceFile))
                    rptHeaderBody = rptHeaderBody.Replace("[event:repeaterzeroeventstext]", Localization.GetString("TokenListRptHeaderZeroEvents", LocalResourceFile))
                    Dim tcc As New TokenReplaceControllerClass(ModuleId, LocalResourceFile)
                    If _rptItemCount = 0 Then
                        rptHeaderBody = tcc.TokenOneParameter(rptHeaderBody, "IFZEROEVENTS", True)
                    Else
                        rptHeaderBody = tcc.TokenOneParameter(rptHeaderBody, "IFZEROEVENTS", False)
                    End If

                    Dim rptHeader As Literal = CType(e.Item.FindControl("rptHeader"), Literal)
                    If Settings.ListViewTable Then
                        rptHeader.Text = rptHeaderTable
                        If rptHeaderBody <> "" Then
                            rptHeader.Text = rptHeader.Text & rptHeaderStart & rptHeaderBody & rptHeaderEnd
                        End If
                    Else
                        rptHeader.Text = rptHeaderBody
                    End If
                Case ListItemType.Footer
                    Dim rptFooterStart As String = "<tr id=""rptTRFooter""><td id=""rptTDFooter"" class=""RptFooter"" colspan=""" & Settings.RptColumns.ToString & """>"
                    Const rptFooterEnd As String = "</td></tr>"
                    Const rptFooterTable As String = "</table>"
                    Dim rptFooterBody As String = Settings.Templates.txtListRptFooter.Replace("[event:repeaterfootertext]", Localization.GetString("TokenListRptFooter", LocalResourceFile))
                    Dim rptFooter As Literal = CType(e.Item.FindControl("rptFooter"), Literal)
                    If Settings.ListViewTable Then
                        If rptFooterBody <> "" Then
                            rptFooter.Text = rptFooterStart & rptFooterBody & rptFooterEnd
                        End If
                        rptFooter.Text = rptFooter.Text & rptFooterTable
                    Else
                        rptFooter.Text = rptFooterBody
                    End If
                Case Else
                    Dim rptBody As Literal = CType(e.Item.FindControl("rptBody"), Literal)
                    Dim rptRowBody As String = CType(DataBinder.Eval(e.Item.DataItem, "EventText"), String)
                    _rptCurrentItemCount += 1
                    If Settings.ListViewTable Then
                        Dim rptBodyStart As String = "<td [event:repeatertooltip] width=" & columnWidth & ">"

                        Const rptBodyEnd As String = "</td>"
                        Dim rptRowStart As String
                        If (_rptCurrentItemCount - 1) Mod rptColumns = 0 Then
                            _rptAlternate = Not _rptAlternate
                            Dim rptCellClass As String = "RptNormal"
                            If _rptAlternate Then
                                rptCellClass = "RptAlternate"
                            End If
                            rptRowStart = "<tr class=""" & rptCellClass & """ >" & rptBodyStart
                        Else
                            rptRowStart = rptBodyStart
                        End If

                        Dim rptRowEnd As String = ""
                        If (_rptCurrentItemCount) Mod rptColumns = 0 Then
                            rptRowEnd = rptBodyEnd & "</tr>"
                        ElseIf _rptItemCount = _rptCurrentItemCount Then
                            ' ReSharper disable RedundantAssignment
                            For i As Integer = 1 To rptColumns - (_rptCurrentItemCount Mod rptColumns)
                                ' ReSharper restore RedundantAssignment
                                rptRowEnd += "<td width=" & columnWidth & " ></td>"
                            Next
                            rptRowEnd += "</tr>"
                        Else
                            rptRowEnd = rptBodyEnd
                        End If
                        Dim tooltip As String = ""
                        If Settings.Eventtooltiplist Then
                            tooltip = HttpUtility.HtmlEncode(CType(DataBinder.Eval(e.Item.DataItem, "Tooltip"), String))
                        End If
                        rptBody.Text = AddTooltip(_rptCurrentItemCount, tooltip, rptRowStart) & rptRowBody & rptRowEnd
                    Else
                        rptBody.Text = rptRowBody
                    End If
            End Select
        End Sub

        Private Function AddTooltip(ByVal itemCount As Integer, ByVal toolTip As String, ByVal body As String) As String
            Dim fullTooltip As String = ""
            If Settings.Eventtooltiplist Then
                Dim ttClientId As String = "ctlEvents_Mod_" & ModuleId.ToString & "_RptRowBody_" & itemCount.ToString
                fullTooltip = "ID=""" & ttClientId & """ title=""" & toolTip & """"
                toolTipManager.TargetControls.Add(clientId, True)
            End If
            Return Replace(Body, "[event:repeatertooltip]", fullTooltip)
        End Function

        Private Sub rptPages_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles rptPager.ItemDataBound
            Select Case e.Item.ItemType
                Case ListItemType.Header
                Case ListItemType.Footer
                Case Else
                    Dim lnkPage As LinkButton = CType(e.Item.FindControl("cmdPage"), LinkButton)
                    If CInt(lnkPage.CommandArgument) = PageNumber + 1 Then
                        lnkPage.Style.Add("cursor", "text")
                        lnkPage.Style.Add("text-decoration", "none")
                        lnkPage.Attributes.Add("onclick", "javascript:return false;")
                        lnkPage.CssClass = "RptPagerCurrentPage"
                    End If
            End Select

        End Sub

        Private Sub rptPages_ItemCommand(ByVal source As Object, ByVal e As RepeaterCommandEventArgs) Handles rptPager.ItemCommand
            PageNumber = CInt(e.CommandArgument) - 1
            BindDataGrid()
        End Sub
#End Region

#Region " Web Form Designer Generated Code "
        'This call is required by the Web Form Designer.
        <DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub
#End Region

    End Class
End Namespace
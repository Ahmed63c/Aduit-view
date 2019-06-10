<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div>
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="GridView1" runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateSelectButton="True"
                        DataSourceID="sqlComputers" Font-Names="Arial" Font-Size="Small" AutoGenerateColumns="False" DataKeyNames="id">
                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                        <AlternatingRowStyle BackColor="Silver" />
                        <Columns>
                            <asp:BoundField DataField="computerName" HeaderText="computerName" SortExpression="computerName" />
                            <asp:BoundField DataField="osVersion" HeaderText="osVersion" SortExpression="osVersion" />
                            <asp:BoundField DataField="lastAudit" HeaderText="lastAudit" SortExpression="lastAudit" />
                            <asp:BoundField DataField="model" HeaderText="model" SortExpression="model" />
                            <asp:TemplateField HeaderText="totalMemory" SortExpression="totalMemory">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# cleanSize(DataBinder.Eval(Container.DataItem, "totalMemory")) %>'></asp:TextBox>MB
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# cleanSize(DataBinder.Eval(Container.DataItem, "totalMemory")) %>'></asp:Label> MB
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="processor" HeaderText="processor" SortExpression="processor" />
                            <asp:BoundField DataField="userName" HeaderText="userName" SortExpression="userName" />
                        </Columns>
                        <SelectedRowStyle BackColor="#C0C0FF" />
                    </asp:GridView>
                    <asp:DetailsView ID="DetailsView1" runat="server" AllowPaging="True" AutoGenerateRows="False"
                        DataSourceID="sqlMemory" HeaderText="Memory Configuration" Height="50px" Width="500px" Font-Names="Arial" Font-Size="Small">
                        <PagerSettings Mode="NumericFirstLast" />
                        <Fields>
                            <asp:TemplateField HeaderText="capacity" SortExpression="capacity">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# cleanSize(DataBinder.Eval(Container.DataItem, "capacity")) %>'></asp:TextBox>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# cleanSize(DataBinder.Eval(Container.DataItem, "capacity")) %>'></asp:TextBox>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# cleanSize(DataBinder.Eval(Container.DataItem, "capacity")) %>'></asp:Label>
                                    MB
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="location" HeaderText="location" SortExpression="location" />
                        </Fields>
                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                    </asp:DetailsView>
                    <asp:DetailsView ID="DetailsView2" runat="server" AllowPaging="True" AutoGenerateRows="False"
                        DataSourceID="sqlDisks" HeaderText="Disk Drives" Height="50px" Width="500px" Font-Names="Arial" Font-Size="Small">
                        <PagerSettings Mode="NumericFirstLast" />
                        <Fields>
                            <asp:BoundField DataField="deviceID" HeaderText="deviceID" SortExpression="deviceID" />
                            <asp:BoundField DataField="description" HeaderText="description" SortExpression="description" />
                            <asp:TemplateField HeaderText="freeSpace" SortExpression="freeSpace">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("freeSpace") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("freeSpace") %>'></asp:TextBox>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# cleanSize(DataBinder.Eval(Container.DataItem, "freeSpace")) %>'></asp:Label> MB
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="size" SortExpression="size">
                                <EditItemTemplate>
                                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("size") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("size") %>'></asp:TextBox>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# cleanSize(DataBinder.Eval(Container.DataItem, "size")) %>'></asp:Label> MB
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                    </asp:DetailsView>
                    <asp:DataList ID="DataList1" runat="server" DataSourceID="sqlSoftware" Width="500px" Font-Names="Arial" Font-Size="Small">
                        <ItemTemplate>
                            <asp:Label ID="softwareNameLabel" runat="server" Text='<%# Eval("softwareName") %>'></asp:Label><br />
                            <br />
                        </ItemTemplate>
                        <AlternatingItemStyle BackColor="Silver" />
                        <HeaderTemplate>
                            Software
                        </HeaderTemplate>
                        <HeaderStyle BackColor="Gray" ForeColor="White" />
                    </asp:DataList><br />
                    <asp:SqlDataSource ID="sqlComputers" runat="server" ConnectionString="<%$ ConnectionStrings:audit_dataConnectionString %>"
                        SelectCommand="SELECT [id], [computerName], [osVersion], [lastAudit], [model], [totalMemory], [processor], [userName] FROM [computers] ORDER BY [computerName]">
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="sqlMemory" runat="server" ConnectionString="<%$ ConnectionStrings:audit_dataConnectionString %>"
                        SelectCommand="SELECT [id], [computerId], [capacity], [location] FROM [memory] WHERE ([computerId] = @computerId)">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="GridView1" Name="computerId" PropertyName="SelectedValue"
                                Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="sqlDisks" runat="server" ConnectionString="<%$ ConnectionStrings:audit_dataConnectionString %>"
                        SelectCommand="SELECT [id], [computerId], [deviceID], [description], [freeSpace], [size] FROM [diskDrives] WHERE ([computerId] = @computerId)">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="GridView1" Name="computerId" PropertyName="SelectedValue"
                                Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                    <asp:SqlDataSource ID="sqlSoftware" runat="server" ConnectionString="<%$ ConnectionStrings:audit_dataConnectionString %>"
                        SelectCommand="SELECT [id], [computerId], [softwareName] FROM [software] WHERE ([computerId] = @computerId) ORDER BY [softwareName]">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="GridView1" Name="computerId" PropertyName="SelectedValue"
                                Type="Int32" />
                        </SelectParameters>
                    </asp:SqlDataSource>
                </ContentTemplate>
            </asp:UpdatePanel>
            </div>
    </form>
</body>
</html>

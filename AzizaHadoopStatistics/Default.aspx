<%@ Page Title="Statistics" Async="true" Trace="false" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AzizaHadoopStatistics._Default" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 style="text-align: center;">Hadoop Map/Reduce
    </h2>
    <table>
        <tr>
            <td>
                <img src="bi.png" class="img-responsive" width="265" />
            </td>
            <td>
                <h3>Hashlysis shows the number of Retweets , tweets and Favorite. along with finds top users use hashtags and countries posted in the hashtag. in addition discover what the top source used to write that hashtag.</h3>
            </td>
        </tr>
    </table>

    <div class="row">
        <div class="col-md-12">
            <h2>Select Hashtag : 
                <asp:DropDownList runat="server" ID="ddlFile" AutoPostBack="True" OnSelectedIndexChanged="ddlFile_SelectedIndexChanged" OnDataBound="ddlFile_DataBound">
                </asp:DropDownList>
            </h2>
        </div>
    </div>
    <asp:Panel runat="server" ID="pnlAll" Visible="false" class="row">
        <div class="col-md-12">
            <table class="row table table-striped">
                <tr>
                    <td colspan="2">
                        <h3>Time line</h3>
                         <asp:Chart ID="chDates" runat="server" Height="500px" Width="800px">
                                <Series>
                                    <asp:Series Name="Series1" ChartType="Spline"></asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                    </td>
                   
                </tr>
                <tr>
                    <td colspan="2">
                      <table>
                          <tr>
                              <td style="width:39%">
                        <h3>Other Statistics</h3>
                            <asp:Chart ID="ChOthers" runat="server" Height="300px" Width="300px">
                                <Series>
                                    <asp:Series ChartType="Spline" Name="Series1">
                                    </asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1">
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                              </td>
                              <td style="width:39%">
                                    <h3>Top Users</h3>
                        <asp:BulletedList ID="BulletedList1" runat="server"></asp:BulletedList>

                              </td>
                              <td style="width:22%">
                                    <h3>Countries</h3>
                            <asp:Chart ID="chCountries" runat="server" Height="500px" Width="500px">
                                <Series>
                                    <asp:Series Name="Series1"></asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                              </td>
                          </tr>
                      </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align:center;">
                        <h3 style="text-align:center;">Sources</h3>
                        <p>
                            <asp:Chart ID="chSources" runat="server" Height="600px" Width="600px">
                                <Series>
                                    <asp:Series ChartType="Pie" Name="Series1">
                                    </asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1">
                                    </asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </p>
                       
                    </td>
                </tr>

            </table>


        </div>
    </asp:Panel>
   
</asp:Content>

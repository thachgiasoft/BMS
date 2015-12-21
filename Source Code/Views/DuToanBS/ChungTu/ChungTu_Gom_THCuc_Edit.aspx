﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site_KeToan_Default.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<%@ Import Namespace="VIETTEL.Models.DuToanBS" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    <%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%
        string MaND = User.Identity.Name;
        string ParentID = "DTBS_ChungTu";
        
        string maChungTuTLTHCuc = Convert.ToString(ViewData["iID_MaChungTu"]);

        string dsMaChungTuTLTH = Convert.ToString(CommonFunction.LayTruong("DTBS_ChungTu_TLTHCuc", "iID_MaChungTu_TLTHCuc", maChungTuTLTHCuc, "iID_MaChungTu_TLTH"));
        if (Html.ValidationMessage(ParentID + "_" + "err_ChungTu") !=null && !String.IsNullOrEmpty(Html.ValidationMessage(ParentID + "_" + "err_ChungTu").ToString()))
        {
            dsMaChungTuTLTH = "";
        }
        string[] arrChungTu = dsMaChungTuTLTH.Split(',');
        
        //Lấy thông tin chứng từ TLTHCuc
        NameValueCollection data = DuToanBSChungTuModels.LayThongTinChungTuTLTHCuc(maChungTuTLTHCuc);
        string dNgayChungTu = "";
        if (ViewData["dNgayChungTu"] != null)
        {
            dNgayChungTu = Convert.ToString(ViewData["dNgayChungTu"]);
        }
        else if (data != null)
        {
            dNgayChungTu = CommonFunction.LayXauNgay(Convert.ToDateTime(data["dNgayChungTu"]));
        }
        else
        {
            dNgayChungTu = CommonFunction.LayXauNgay(DateTime.Now);
        }
        //Lấy danh sách chứng từ TLTH
        DataTable dtChungTuDuyet = DuToanBSChungTuModels.LayDanhSachChungTuDeSuaTLTHCuc(MaND, maChungTuTLTHCuc);
        int columnCount = 1;
        string BackURL = Url.Action("Index", "DuToanBSChungTu", new { iLoai =2});
    %>
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left" style="width: 9%;">
                <div style="padding-left: 22px; padding-bottom: 5px; text-transform: uppercase; color: #ec3237;">
                    <b>
                        <%=NgonNgu.LayXau("Liên kết nhanh: ")%></b>
                </div>
            </td>
            <td align="left">
                <div style="padding-bottom: 5px; color: #ec3237;">
                    <%=MyHtmlHelper.ActionLink(Url.Action("Index", "Home"), "Trang chủ")%>
                </div>
            </td>
            <td align="right" style="padding-bottom: 5px; color: #ec3237; font-weight: bold;
                padding-right: 20px;">
                <% Html.RenderPartial("LogOnUserControl_KeToan"); %>
            </td>
        </tr>
    </table>
    <%
        using (Html.BeginForm("ThemSuaChungTuTLTHCuc", "DuToanBSChungTu", new { ParentID = ParentID, MaChungTu = maChungTuTLTHCuc }))
        {
    %>
    <%= Html.Hidden(ParentID + "_DuLieuMoi", 0)%>
    <div class="box_tong">
        <div id="Div1">
            <div id="Div2">
                
                <table cellpadding="0" cellspacing="0" width="100%" class="table_form2">
                    <tr>
                        <td style="width: 80%">
                            <table cellpadding="0" cellspacing="0" border="0" width="50%" class="table_form2">
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Chọn đợt</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                    <div style="overflow: scroll; width: 50%; height: 300px">
                                        <table class="mGrid" style="width: 100%">
                                            <tr>
                                                <th align="center" style="width: 40px;">
                                                    <input type="checkbox" id="abc" onclick="CheckAll(this.checked)" />
                                                </th>
                                                <% for (int c = 0; c < columnCount * 2 - 1; c++)
                                                   {%>
                                                <th>
                                                </th>
                                                <% } %>
                                            </tr>
                                            <%
                                                string strTen = "";
                                                string strMa = "";
                                                string strChecked = "";
                                                for (int i = 0; i < dtChungTuDuyet.Rows.Count; i = i + 1)
                                                {
                                            %>
                                            <tr>
                                                <% for (int c = 0; c < columnCount; c++)
                                                   {
                                                       if (i + c < dtChungTuDuyet.Rows.Count)
                                                       {
                                                           strChecked = "";
                                                           strTen = CommonFunction.LayXauNgay(
                                                                    Convert.ToDateTime(dtChungTuDuyet.Rows[i + c]["dNgayChungTu"])) + '-' +
                                                                Convert.ToString(dtChungTuDuyet.Rows[i + c]["sID_MaNguoiDungtao"]) + '-' + Convert.ToString(dtChungTuDuyet.Rows[i + c]["sTenPhongBan"]) + '-' + Convert.ToString(dtChungTuDuyet.Rows[i + c]["sNoiDung"]);
                                                           strMa = Convert.ToString(dtChungTuDuyet.Rows[i + c]["iID_MaChungTu_TLTH"]);
                                                           if (arrChungTu.Contains(strMa))
                                                           {
                                                               strChecked = "checked=\"checked\"";
                                                           }
                                                %>
                                                <td align="center" style="width: 10px;">
                                                    <input type="checkbox" value="<%= strMa %>" <%= strChecked %> check-group="ChungTu" id="iID_MaChungTu_TLTH" name="iID_MaChungTu_TLTH" />
                                                </td>
                                                <td align="left">
                                                    <%= strTen%>
                                                </td>
                                                <% } %>
                                                <% } %>
                                            </tr>
                                             <% } %>
                                        </table>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                            &nbsp;
                                    </td>
                                    <td class="td_form2_td5">
                                        <div><%= Html.ValidationMessage(ParentID + "_" + "err_ChungTu")%></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Ngày tháng</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div style="width: 200px; float: left;">
                                            <%= MyHtmlHelper.DatePicker(ParentID, dNgayChungTu, "dNgayChungTu", "",
                                                                    "class=\"input1_2\"  style=\"width: 200px;\"") %>
                                            
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        &nbsp;
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%= Html.ValidationMessage(ParentID + "_" + "err_dNgayChungTu") %>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1">
                                        <div>
                                            <b>Nội dung đợt</b></div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <%= MyHtmlHelper.TextArea(ParentID, data["sNoiDung"], "sNoiDung", "","class=\"input1_2\" style=\"height: 100px;\"") %></div>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="td_form2_td1" style="width: 15%;">
                                        <div>
                                        </div>
                                    </td>
                                    <td class="td_form2_td5">
                                        <div>
                                            <table cellpadding="0" cellspacing="0" border="0">
                                                <tr>
                                                    <td width="65%" class="td_form2_td5">&nbsp;</td>   
                                                    <td width="30%" align="right" class="td_form2_td5">
                                                        <input type="submit" class="button" id="Submit1" value="Lưu" />
                                                    </td>          
                                                    <td width="5px">&nbsp;</td>          
                                                    <td class="td_form2_td5">
                                                        <input class="button" type="button" value="Hủy" onclick="Huy()" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
               
            </div>
        </div>
    </div>
    <% } %>
    <script type="text/javascript">
        function CheckAll(value) {
            $("input:checkbox[check-group='ChungTu']").each(function (i) {
                this.checked = value;
            });
        }
        function Huy()
        {
            window.location.href = '<%=BackURL%>';
        }
    </script>
</asp:Content>

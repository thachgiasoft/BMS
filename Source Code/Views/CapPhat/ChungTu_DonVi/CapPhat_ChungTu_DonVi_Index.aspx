﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="DomainModel" %>
<%@ Import Namespace="DomainModel.Controls" %>
<%@ Import Namespace="VIETTEL.Models" %>
<script runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {

    }
</script>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	<%=ConfigurationManager.AppSettings["TitleView"]%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%
    int i;
    String MaND = User.Identity.Name;
    String ParentID = "CapPhat_ChungTu_DonVi";
    
    //Lấy thông tin tìm kiếm
    String iSoCapPhat = Request.QueryString["SoCapPhat"];
    String sTuNgay = Request.QueryString["TuNgay"];
    String sDenNgay = Request.QueryString["DenNgay"];
    String iID_MaTrangThaiDuyet = Request.QueryString["iID_MaTrangThaiDuyet"];
    String iDM_MaLoaiCapPhat = Request.QueryString["iDM_MaLoaiCapPhat"];
    String iID_MaTinhChatCapThu = Request.QueryString["iID_MaTinhChatCapThu"];
    String iID_MaDonVi = Request.QueryString["iID_MaDonVi"];
    String sLNS = Request.QueryString["sLNS"];
    
    String page = Request.QueryString["page"];
    
    
    
    String Loai = Request.QueryString["Loai"];
    if (string.IsNullOrEmpty(Loai))
        Loai = "1";
    if (HamChung.isDate(sTuNgay) == false) sTuNgay = "";
    if (HamChung.isDate(sDenNgay) == false) sDenNgay = "";
    int CurrentPage = 1;
    SqlCommand cmd;

    if (String.IsNullOrEmpty(iID_MaTrangThaiDuyet) || iID_MaTrangThaiDuyet == "-1")
    {
        iID_MaTrangThaiDuyet = "";
    }
    
    DataTable dtTrangThai_All = LuongCongViecModel.Get_dtDSTrangThaiDuyet(PhanHeModels.iID_MaPhanHeCapPhat);
    
    DataTable dtTrangThai = LuongCongViecModel.Get_dtDSTrangThaiDuyet_DuocXem(PhanHeModels.iID_MaPhanHeCapPhat, MaND);
    dtTrangThai.Rows.InsertAt(dtTrangThai.NewRow(), 0);
    dtTrangThai.Rows[0]["iID_MaTrangThaiDuyet"] = "-1";
    dtTrangThai.Rows[0]["sTen"] = "-- Chọn trạng thái --";
    SelectOptionList slTrangThai = new SelectOptionList(dtTrangThai, "iID_MaTrangThaiDuyet", "sTen");
    
    if(String.IsNullOrEmpty(page) == false){
        CurrentPage = Convert.ToInt32(page);
    }

    DataTable dt = CapPhat_ChungTuModels.LayDanhSachChungTuDonVi(iSoCapPhat, iDM_MaLoaiCapPhat, iID_MaTinhChatCapThu, iID_MaDonVi, iID_MaTrangThaiDuyet, sTuNgay, sDenNgay, MaND, false, CurrentPage, Globals.PageSize);

    double nums = CapPhat_ChungTuModels.LayDanhSachChungTuCapPhatDonViCount(iSoCapPhat, iDM_MaLoaiCapPhat, iID_MaTinhChatCapThu, iID_MaDonVi, iID_MaTrangThaiDuyet, sTuNgay, sDenNgay, MaND, false);
    int TotalPages = (int)Math.Ceiling(nums / Globals.PageSize);
    String strPhanTrang = MyHtmlHelper.PageLinks(String.Format("Trang {0}/{1}:", CurrentPage, TotalPages), CurrentPage, TotalPages, x => Url.Action("Index", new { iID_MaDonVi = iID_MaDonVi, MaND = MaND, SoCapPhat = iSoCapPhat, TuNgay = sTuNgay, DenNgay = sDenNgay, iID_MaTrangThaiDuyet = iID_MaTrangThaiDuyet, page = x }));
    String strThemMoi = Url.Action("SuaChungTu", "CapPhat_ChungTu_DonVi", new { Loai = Loai });
    
    //Lấy danh sách loại cấp phát
    DataTable dtLoaiCapPhat = DanhMucModels.DT_DanhMuc("LoaiCapPhat",true,"--Chọn loại cấp phát--");
    SelectOptionList slLoaiCapPhat = new SelectOptionList(dtLoaiCapPhat, "iID_MaDanhMuc", "sTen");
    dtLoaiCapPhat.Dispose();
    
    //Lấy danh sách loại ngân sách quốc phòng
    String iID_MaPhongBan = NganSach_HamChungModels.MaPhongBanCuaMaND(MaND);
    DataTable dtLNSQuocPhong = DanhMucModels.NS_LoaiNganSachQuocPhong(iID_MaPhongBan);
    SelectOptionList slLNSQuocPhong = new SelectOptionList(dtLNSQuocPhong, "sLNS", "TenHT");
    dtLNSQuocPhong.Rows.InsertAt(dtLNSQuocPhong.NewRow(), 0);
    dtLNSQuocPhong.Rows[0]["sLNS"] = "-1";
    dtLNSQuocPhong.Rows[0]["TenHT"] = "--Chọn loại ngân sách--";
    dtLNSQuocPhong.Dispose();
    
    //Lấy danh sách tính chất cấp thu
    DataTable dtTinhChatCapThu = TinhChatCapThuModels.Get_dtTinhChatCapThu();
    SelectOptionList slTinhChatCapThu = new SelectOptionList(dtTinhChatCapThu, "iID_MaTinhChatCapThu", "sTen");
    dtTinhChatCapThu.Rows.InsertAt(dtTinhChatCapThu.NewRow(), 0);
    dtTinhChatCapThu.Rows[0]["iID_MaTinhChatCapThu"] = "-1";
    dtTinhChatCapThu.Rows[0]["sTen"] = "--Chọn tính chất cấp thu--";
    dtTinhChatCapThu.Dispose();
    
    //Danh sách đơn vị
    DataTable dtDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(MaND);
    SelectOptionList slDonVi = new SelectOptionList(dtDonVi, "iID_MaDonVi", "sTen");
    dtDonVi.Rows.InsertAt(dtDonVi.NewRow(), 0);
    dtDonVi.Rows[0]["iID_MaDonVi"] = "-1";
    dtDonVi.Rows[0]["sTen"] = "--Chọn đơn vị--";
    dtDonVi.Dispose();    
    
    using (Html.BeginForm("TimKiemChungTu", "CapPhat_ChungTu_DonVi", new { ParentID = ParentID, Loai = Loai }))
    {
%>
<div class="box_tong">
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
        	<tr>
            	<td>
                	<span>Thông tin tìm kiếm</span>
                </td>
            </tr>
        </table>
    </div>
    <div id="nhapform">
        <div id="form2">
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td valign="top" align="left" style="width: 45%;">
                        <table cellpadding="5" cellspacing="5" width="100%">
                            <tr>
                                <td class="td_form2_td1"><div><b>Số cấp phát</b></div></td>
                                <td class="td_form2_td5">
                                    <div>
                                        <%=MyHtmlHelper.TextBox(ParentID, iSoCapPhat, "iSoCapPhat", "", "class=\"input1_2\"")%>
                                    </div>
                                </td>
                            </tr>

                           <tr>
                                <td class="td_form2_td1"><div><b>Loại cấp phát</b></div></td>
                                <td class="td_form2_td5">
                                    <div>
                                        <%=MyHtmlHelper.DropDownList(ParentID, slLoaiCapPhat, iDM_MaLoaiCapPhat, "iDM_MaLoaiCapPhat", "", "class=\"input1_2\"")%>
                                    </div>
                                </td>
                            </tr>

                            <tr>
                                <td class="td_form2_td1"><div><b>Tính chất cấp thu</b></div></td>
                                <td class="td_form2_td5">
                                    <div>
                                        <%=MyHtmlHelper.DropDownList(ParentID, slTinhChatCapThu, iID_MaTinhChatCapThu, "iID_MaTinhChatCapThu", "", "class=\"input1_2\"")%>
                                    </div>
                                </td>
                            </tr>

                             <tr>
                                <td class="td_form2_td1"><div><b>Đơn vị</b></div></td>
                                <td class="td_form2_td5">
                                    <div>
                                        <%=MyHtmlHelper.DropDownList(ParentID, slDonVi, iID_MaDonVi, "iID_MaDonVi", "", "class=\"input1_2\"")%>
                                    </div>
                                </td>
                            </tr>

                            <tr>
                                <td class="td_form2_td1"><div><b>Loại ngân sách</b></div></td>
                                <td class="td_form2_td5">
                                    <div>
                                         <%=MyHtmlHelper.DropDownList(ParentID, slLNSQuocPhong, sLNS, "sLNS", "", "class=\"input1_2\"")%>
                                    </div>
                                </td>
                            </tr>

                            <tr>
                                <td class="td_form2_td1"><div><b>Trạng thái</b></div></td>
                                <td class="td_form2_td5">
                                    <div>
                                        <%=MyHtmlHelper.DropDownList(ParentID, slTrangThai, iID_MaTrangThaiDuyet, "iID_MaTrangThaiDuyet", "", "class=\"input1_2\"")%>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" align="left" style="width: 45%;">
                        <table cellpadding="5" cellspacing="5" width="100%">
                        
                            <tr>
                                <td class="td_form2_td1"; width="10%"><div><b>Từ ngày</b></div></td>
                                <td class="td_form2_td5">
                                    <div  style="width: 35%">
                                        <%=MyHtmlHelper.DatePicker(ParentID, sTuNgay, "dTuNgay", "", "class=\"input1_2\" onblur=isDate(this);")%>        
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="td_form2_td1"><div><b>Đến ngày</b></div></td>
                                <td class="td_form2_td5">
                                    <div style="width: 35%">
                                        <%=MyHtmlHelper.DatePicker(ParentID, sDenNgay, "dDenNgay", "", "class=\"input1_2\" onblur=isDate(this);")%>
                                    </div>
                                </td>
                            </tr>
                          
                          <tr><td class="td_form2_td1" colspan = "2">&nbsp</td></tr>
                          <tr><td class="td_form2_td1" colspan = "2">&nbsp</td></tr>
                          <tr><td class="td_form2_td1" colspan = "2">&nbsp</td></tr>
                          <tr><td class="td_form2_td1" colspan = "2">&nbsp</td></tr>
                          <tr><td class="td_form2_td1" colspan = "2">&nbsp</td></tr>
                          <tr><td class="td_form2_td1" colspan = "2">&nbsp</td></tr>
                          <tr><td class="td_form2_td1" colspan = "2">&nbsp</td></tr>
                          <tr><td class="td_form2_td1" colspan = "2">&nbsp</td></tr>
                          <tr><td class="td_form2_td1" colspan = "2">&nbsp</td></tr>
                          <tr><td class="td_form2_td1" colspan = "2">&nbsp</td></tr>
                        </table>
                    </td>
                </tr>
                <tr><td colspan="2" align="center" class="td_form2_td1" style="height: 10px;"></td></tr>
                <tr>
                    <td colspan="2" align="center" style="background-color: #f0f9fe; padding: 0px 0px 10px 0px;">
                        <table border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td>
                                    <input type="submit" class="button" value="Tìm kiếm"/>
                                </td>
                                <td style="width: 10px;"></td>
                                <td>
                                    <%
                                        if (LuongCongViecModel.NguoiDung_DuocThemChungTu(PhanHeModels.iID_MaPhanHeCapPhat, MaND))                                        {
                                        %>
                                            <input id="TaoMoi" type="button" class="button" value="Tạo mới" onclick="javascript:location.href='<%=strThemMoi %>'" />
                                        <%
                                        } %>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>
<%  } %>
<br />
<div class="box_tong">
    <div class="title_tong">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
        	<tr>
            	<td>
                	<span>Danh sách cấp phát đơn vị</span>
                </td>
            </tr>
        </table>
    </div>
    <table class="mGrid" id="<%= ParentID %>_thList">
        <tr>
            <th style="width: 3%;" align="center">STT</th>
            <th style="width: 7%;" align="center">Ngày cấp phát</th>
            <th style="width: 5%;" align="center">Số cấp phát</th>
            <th style="width: 7.5%;" align="center">Loại cấp phát</th>
            <th style="width: 7.5%;" align="center">Tính chất cấp thu</th>
            <th style="width: 7%;" align="center">Đơn vị</th>
            <th style="width: 4%;" align="center">LNS</th>
            <th style="width: 7%;" align="center">Chi tiết đến</th>
            <th style="width: 17%;" align="center">Nội dung</th>
            <th style="width: 10%;" align="center">Trạng thái</th>
            <th style="width: 16%;" align="center">Lý do</th>
            <th style="width: 3%;" align="center">Sửa</th>
            <th style="width: 3%;" align="center">Xóa</th>
        </tr>
        <%
        for (i = 0; i < dt.Rows.Count; i++)
        {
            DataRow R = dt.Rows[i];
            String classtr = "";
            int STT = i + 1;
            String NgayChungTu = CommonFunction.LayXauNgay(Convert.ToDateTime(R["dNgayCapPhat"]));
            String sTrangThai = "",strColor="";
            int TrangThaiDuyet = Convert.ToInt16(R["iID_MaTrangThaiDuyet"]);
            int DaDuyet = LuongCongViecModel.Get_iID_MaTrangThaiDuyet_DaDuyet(PhanHeModels.iID_MaPhanHeCapPhat);
            String iID_MaCapPhat = Convert.ToString(R["iID_MaCapPhat"]);
            for (int j = 0; j < dtTrangThai_All.Rows.Count; j++)
            {
                if (Convert.ToString(R["iID_MaTrangThaiDuyet"]) == Convert.ToString(dtTrangThai_All.Rows[j]["iID_MaTrangThaiDuyet"]))
                {
                    sTrangThai = Convert.ToString(dtTrangThai_All.Rows[j]["sTen"]);
                    strColor = String.Format("style='background-color: {0}; background-repeat: repeat;'", dtTrangThai_All.Rows[j]["sMauSac"]);
                    break;
                }
            }

            //VungNV: Lấy LNS
            String sDSLNS = Convert.ToString(R["sDSLNS"]);
            
            //Lấy tên đơn vị
            String strTenDonVi = DonViModels.Get_TenDonVi(Convert.ToString(R["iID_MaDonVi"]), MaND);          

            //Lấy loại cấp phát
            String LoaiCapPhat = "";
            for (int j = 0; j < dtLoaiCapPhat.Rows.Count; j++)
            {
                if(Convert.ToString(R["iDM_MaLoaiCapPhat"])==Convert.ToString(dtLoaiCapPhat.Rows[j]["iID_MaDanhMuc"]))
                {
                    LoaiCapPhat=Convert.ToString(dtLoaiCapPhat.Rows[j]["sTen"]);
                    break;
                }
            }

            //Lấy tính chất cấp thu "iID_MaTinhChatCapThu", "sTen"
            String TinhChatCapThu = "";
            for (int j = 0; j < dtTinhChatCapThu.Rows.Count; j++)
            {
                if (Convert.ToString(R["iID_MaTinhChatCapThu"]) == Convert.ToString(dtTinhChatCapThu.Rows[j]["iID_MaTinhChatCapThu"]))
                {
                    TinhChatCapThu = Convert.ToString(dtTinhChatCapThu.Rows[j]["sTen"]);
                    break;
                }
            }
            
            //Thông tin chứng từ chi tiết đến
            String ChiTietDen = "";
            DataTable dtLoai = CapPhat_ChungTuModels.LayLoaiNganSachCon();
            for (int j = 0; j < dtLoai.Rows.Count; j++)
            {
                if (Convert.ToString(R["sLoai"]) == Convert.ToString(dtLoai.Rows[j]["iID_Loai"]))
                {
                    ChiTietDen = Convert.ToString(dtLoai.Rows[j]["TenHT"]);
                    break;
                }
            }
            
            String strEdit = "";
            String strDelete = "";

            if (LuongCongViecModel.NguoiDung_DuocThemChungTu(PhanHeModels.iID_MaPhanHeCapPhat, MaND) &&
                                LuongCongViecModel.KiemTra_TrangThaiKhoiTao(PhanHeModels.iID_MaPhanHeCapPhat, Convert.ToInt32(R["iID_MaTrangThaiDuyet"])))
            {
                strEdit = MyHtmlHelper.ActionLink(Url.Action("SuaChungTu", "CapPhat_ChungTu_DonVi", new { iID_MaCapPhat = R["iID_MaCapPhat"], Loai = Loai }).ToString(), "<img src='../Content/Themes/images/edit.gif' alt='' />", "Edit", "");
                strDelete = MyHtmlHelper.ActionLink(Url.Action("XoaChungTu", "CapPhat_ChungTu_DonVi", new { iID_MaCapPhat = R["iID_MaCapPhat"], Loai = Loai }).ToString(), "<img src='../Content/Themes/images/delete.gif' alt='' />", "Delete", "");
            }
            
            %>
            <tr <%=strColor %>>
                <td align="center"><%=R["rownum"]%></td>            
                <td align="center"><%=NgayChungTu %></td>
                <td align="center">
                    <b><%=MyHtmlHelper.ActionLink(Url.Action("ChungTuChiTiet", "CapPhat_ChungTu_DonVi", new { iID_MaCapPhat = R["iID_MaCapPhat"], HienThiOpt = 0 }).ToString(), Convert.ToString(R["sTienToChungTu"]) + Convert.ToString(R["iSoCapPhat"]), "Detail", "")%></b>
                </td>
                <td><%=LoaiCapPhat %></td>
                <td><%=TinhChatCapThu %></td>
                <td><%=strTenDonVi%></td>
                <td><%=sDSLNS%></td>
                <td><%=ChiTietDen%></td>
                <td align="left"><%=dt.Rows[i]["sNoiDung"]%></td>
                <td align="center"><%=sTrangThai %></td>
                <td align="left"><%=dt.Rows[i]["sLyDo"]%></td>

                <%-- VungNV: 2015/11/28: không làm thông tri              
                <td align="center">
                    <%if(TrangThaiDuyet==DaDuyet){ %>
                          <div style="margin-right: 5px;" onclick="OnInit_CT();">      
                            <%= Ajax.ActionLink("Thông tri", "Index", "NhapNhanh", new { id = "CapPhat_ThongTri", OnLoad = "OnLoad_CT", OnSuccess = "CallSuccess_CT", iID_MaCapPhat = iID_MaCapPhat}, new AjaxOptions { }, new { @class = "button_title" })%>                                
                          </div>
                     <%}%>
                </td>--%>

                <td align="center">
                    <%=strEdit%>                   
                </td>
                <td align="center">
                    <%=strDelete%>                                       
                </td>
            </tr>
        <%} %>
        <tr class="pgr">
            <td colspan="13" align="right">
                <%=strPhanTrang%>
            </td>
        </tr>
    </table>
</div>
    <%
    dt.Dispose();
    dtTrangThai.Dispose();
    dtLoaiCapPhat.Dispose();
    dtTrangThai_All.Dispose();
    %>
    <script type="text/javascript">
        function CallSuccess_CT() {
            location.reload();
            return false;
        }
        function OnInit_CT() {
            $("#idDialog").dialog("destroy");
            document.getElementById("idDialog").title = 'Thông tri';
            document.getElementById("idDialog").innerHTML = "";
            $("#idDialog").dialog({
                resizeable: false,
                width: 400,
                modal: true
            });
        }
        function OnLoad_CT(v) {
            document.getElementById("idDialog").innerHTML = v;
        }
    </script>

<div id="idDialog" style="display: none;">    
</div>
</asp:Content>


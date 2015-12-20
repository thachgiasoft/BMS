﻿using System;
using System.Web.Mvc;
using FlexCel.Core;
using FlexCel.Report;
using FlexCel.Render;
using FlexCel.XlsAdapter;
using System.Data;
using DomainModel;
using VIETTEL.Models;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using VIETTEL.Models.QuyetToan;

namespace VIETTEL.Report_Controllers.QuyetToan.QuyetToanQuy
{
    public class rptQuyetToanThongTriController : Controller
    {
      
        public string sViewPath = "~/Report_Views/";
        private string VIEWS_PATH_QUYETTOAN_THONGTRI = "~/Report_Views/QuyetToan/QuyetToanQuy/rptQuyetToan_ThongTri.aspx";
        private const string sFilePath_ChiTiet_Nganh = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_ThongTri_ChiTiet_Nganh.xls";
        private const string sFilePath_ChiTiet_Muc = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_ThongTri_ChiTiet_Muc.xls";
        private const string sFilePath_TongHop_Nganh = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_ThongTri_TongHop_Nganh.xls";
        private const string sFilePath_TongHop_Muc = "/Report_ExcelFrom/QuyetToan/QuyetToanQuy/rptQuyetToan_ThongTri_TongHop_Muc.xls";

            public ActionResult Index()
        {
            ViewData["path"] = VIEWS_PATH_QUYETTOAN_THONGTRI;
            return View(sViewPath + "ReportView.aspx");
        }

        /// <summary>
        /// Lấy các giá trị từ Form gán vào ViewData
        /// </summary>
        /// <returns></returns>
            public ActionResult FormSubmit(String ParentID)
        {
            String sLNS = Request.Form["sLNS"];
            String iID_MaDonVi = Request.Form["iID_MaDonVi"];
            String iThang_Quy = Request.Form[ParentID + "_iThang_Quy"];
            String iID_MaNamNganSach = Request.Form[ParentID + "_iID_MaNamNganSach"];
            String MaPhongBan = Request.Form[ParentID + "_iID_MaPhongBan"];
            String LoaiTongHop = Request.Form[ParentID + "_LoaiTongHop"];
            String DenMuc = Request.Form[ParentID + "_DenMuc"];
            String LoaiCapPhat = Request.Form[ParentID + "_LoaiCapPhat"];
            String LoaiThongTri = Request.Form[ParentID + "_LoaiThongTri"];
            
            ViewData["PageLoad"] = "1";
            ViewData["sLNS"] = sLNS;
            ViewData["iID_MaDonVi"] = iID_MaDonVi;
            ViewData["iThang_Quy"] = iThang_Quy;
            ViewData["iID_MaNamNganSach"] = iID_MaNamNganSach;
            ViewData["MaPhongBan"] = MaPhongBan;
            ViewData["LoaiTongHop"] = LoaiTongHop;
            ViewData["DenMuc"] = DenMuc;
            ViewData["LoaiCapPhat"] = LoaiCapPhat;
            ViewData["LoaiThongTri"] = LoaiThongTri;

            ViewData["path"] = VIEWS_PATH_QUYETTOAN_THONGTRI;
            return View(sViewPath + "ReportView.aspx");
        }

        /// <summary>
        /// Xuất file PDF quyết toán thông tri
        /// </summary>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="iThang_Quy">Quý</param>
        /// <param name="sLNS">Loại ngân sách</param>
        /// <param name="iID_MaDonVi">Mã đơn vị</param>
        /// <param name="iID_MaNamNganSach">Năm ngân sách</param>
        /// <param name="LoaiTongHop">Loại báo cáo tổng hợp hay chi tiết</param>
        /// <param name="DenMuc">Báo cáo chi tiết đến mục hay ngành</param>
        /// <param name="LoaiCapPhat">Loại quyết toán</param>
        /// <param name="LoaiThongTri">Loại thông tri</param>
        /// <returns></returns>
        public ActionResult ViewPDF(String MaND, String iThang_Quy, String sLNS, String iID_MaDonVi, String iID_MaNamNganSach,
                    String LoaiTongHop, String DenMuc, String LoaiCapPhat, String LoaiThongTri)
        {
            HamChung.Language();
            String sDuongDan = "";
            //Begin: VungNV: 2015/09/23
            if (LoaiTongHop == "ChiTiet")
            {
                if (DenMuc == "Nganh")
                {
                    sDuongDan = sFilePath_ChiTiet_Nganh;
                }
                if (DenMuc == "Muc")
                {
                    sDuongDan = sFilePath_ChiTiet_Muc;
                }
            }

            if (LoaiTongHop == "TongHop")
            {
                if (DenMuc == "Nganh")
                {
                    sDuongDan = sFilePath_TongHop_Nganh;
                }
                if (DenMuc == "Muc")
                {
                    sDuongDan = sFilePath_TongHop_Muc;
                }
            }

            ExcelFile xls = CreateReport(Server.MapPath(sDuongDan), MaND, sLNS, iThang_Quy, iID_MaDonVi, iID_MaNamNganSach, LoaiTongHop, LoaiCapPhat, LoaiThongTri);
            using (FlexCelPdfExport pdf = new FlexCelPdfExport())
            {
                pdf.Workbook = xls;
                using (MemoryStream ms = new MemoryStream())
                {
                    pdf.BeginExport(ms);
                    pdf.ExportAllVisibleSheets(false, "BaoCao");
                    pdf.EndExport();
                    ms.Position = 0;
                    return File(ms.ToArray(), "application/pdf");
                }
            }

        }

        /// <summary>
        /// Tạo file PDF xuất dữ liệu của quyết toán thông tri
        /// </summary>
        /// <param name="path"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="sLNS">Loại ngân sách</param>
        /// <param name="iThang_Quy">Quý</param>
        /// <param name="iID_MaDonVi">Mã đơn vị</param>
        /// <param name="iID_MaNamNganSach">Năm ngân sách</param>
        /// <param name="LoaiTongHop">Loại báo cáo tổng hợp hay chi tiết</param>
        /// <param name="LoaiCapPhat">Loại quyết toán</param>
        /// <param name="LoaiThongTri">Loại thông tri</param>
        /// <returns></returns>
        public ExcelFile CreateReport(String path, String MaND, String sLNS, String iThang_Quy, String iID_MaDonVi, String iID_MaNamNganSach, String LoaiTongHop, String LoaiCapPhat, String LoaiThongTri)
        {
            XlsFile Result = new XlsFile(true);
            Result.Open(path);
            FlexCelReport fr = new FlexCelReport();
            fr = ReportModels.LayThongTinChuKy(fr, "rptQuyetToan_ThongTri");
            
            LoadData(fr, MaND, sLNS, iThang_Quy, iID_MaDonVi, iID_MaNamNganSach, LoaiTongHop);
            String Nam = ReportModels.LayNamLamViec(MaND);

            //lay ten nam ngan sach
            String NamNganSach = "";
            if (iID_MaNamNganSach == "1")
                NamNganSach = "QUYẾT TOÁN NĂM TRƯỚC";
            else if (iID_MaNamNganSach == "2")
                NamNganSach = "QUYẾT TOÁN NĂM NAY";
                else
                {
                    NamNganSach = "TỔNG HỢP";
                }

            String sTenDonVi = DonViModels.Get_TenDonVi(iID_MaDonVi,MaND);

            if (String.IsNullOrWhiteSpace(LoaiCapPhat)) 
            {
                DataTable dtLoaiThongTri = QuyetToanModels.GetDanhSachLoaiNSQuyetToan_ThongTri();

                foreach(DataRow row in dtLoaiThongTri.Rows)
                {
                    string sMaLoai = row["MaLoai"].ToString();

                    if(sMaLoai == LoaiThongTri)
                    {
                        LoaiCapPhat = row["sTen"].ToString();
                        break;
                    }
                }
            }

            if (!String.IsNullOrEmpty(LoaiCapPhat))
            {
                LoaiCapPhat.Trim();
            }

            fr.SetValue("BoQuocPhong", ReportModels.CauHinhTenDonViSuDung(1, MaND));
            fr.SetValue("QuanKhu", ReportModels.CauHinhTenDonViSuDung(2, MaND));
            fr.SetValue("NgayThang", ReportModels.Ngay_Thang_Nam_HienTai());
            fr.SetValue("Nam", Nam);
            fr.SetValue("NgayLap", "quý " + iThang_Quy + " năm " + Nam);
            fr.SetValue("NamNganSach", NamNganSach);
            fr.SetValue("sTenDonVi", sTenDonVi);
            fr.SetValue("LoaiCapPhat", LoaiCapPhat);
           
            fr.Run(Result);
            return Result;
        }

        /// <summary>
        /// Lấy dữ liệu chi tiết của quyết toán thông tri
        /// </summary>
        /// <param name="fr"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="sLNS">Loại ngân sách</param>
        /// <param name="iThang_Quy">Quý</param>
        /// <param name="iID_MaDonVi">Mã đơn vị</param>
        /// <param name="iID_MaNamNganSach">Năm ngân sách</param>
        /// <param name="LoaiTongHop">Loại báo cáo tổng hợp hay chi tiết</param>
        private void LoadData(FlexCelReport fr, String MaND, String sLNS, String iThang_Quy, String iID_MaDonVi, String iID_MaNamNganSach, String LoaiTongHop)
        {
            int SoDong = 0;
            DataRow r;
            DataTable data = new DataTable();
            DataTable dtDonVi = new DataTable();

            if(LoaiTongHop == "ChiTiet")
            {
                data = QuyetToan_ReportModels.rptQuyetToanThongTri(MaND, sLNS, iThang_Quy, iID_MaNamNganSach, iID_MaDonVi, LoaiTongHop);
            }
            if (LoaiTongHop == "TongHop")
            {
                dtDonVi = QuyetToan_ReportModels.rptQuyetToanThongTri(MaND, sLNS, iThang_Quy, iID_MaNamNganSach, iID_MaDonVi, LoaiTongHop);
                data = HamChung.SelectDistinct("ChiTiet", dtDonVi, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa");
                fr.AddTable("dtDonVi", dtDonVi);
                dtDonVi.Dispose();
            }

            data.TableName = "ChiTiet";
            fr.AddTable("ChiTiet", data);
            DataTable dtsTM = HamChung.SelectDistinct("dtsTM", data, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sTM,sMoTa", "sLNS,sL,sK,sM,sTM,sTTM");
            DataTable dtsM = HamChung.SelectDistinct("dtsM", dtsTM, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sM,sMoTa", "sLNS,sL,sK,sM,sTM");
            DataTable dtsL = HamChung.SelectDistinct("dtsL", dtsM, "sLNS1,sLNS3,sLNS5,sLNS,sL,sK", "sLNS1,sLNS3,sLNS5,sLNS,sL,sK,sMoTa", "sLNS,sL,sK,sM");
            DataTable dtsLNS = HamChung.SelectDistinct("dtsLNS", dtsL, "sLNS1,sLNS3,sLNS5,sLNS", "sLNS1,sLNS3,sLNS5,sLNS,sMoTa", "sLNS,sL");


            DataTable dtsLNS5 = HamChung.SelectDistinct("dtsLNS5", dtsLNS, "sLNS1,sLNS3,sLNS5", "sLNS1,sLNS3,sLNS5,sMoTa");
            for (int i = 0; i < dtsLNS5.Rows.Count; i++)
            {
                r = dtsLNS5.Rows[i];
                r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS5"]));
            }
            DataTable dtsLNS3 = HamChung.SelectDistinct("dtsLNS3", dtsLNS5, "sLNS1,sLNS3", "sLNS1,sLNS3,sMoTa");

            for (int i = 0; i < dtsLNS3.Rows.Count; i++)
            {
                r = dtsLNS3.Rows[i];
                r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS3"]));
            }

            DataTable dtsLNS1 = HamChung.SelectDistinct("dtsLNS1", dtsLNS3, "sLNS1", "sLNS1,sMoTa");
            
            for (int i = 0; i < dtsLNS1.Rows.Count; i++)
            {
                r = dtsLNS1.Rows[i];
                r["sMoTa"] = ReportModels.LayMoTa(Convert.ToString(r["sLNS1"]));
            }

            long TongTien = 0;

            if (LoaiTongHop == "ChiTiet")
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    if (data.Rows[i]["rTuChi"].ToString() != "")
                    {
                        TongTien += long.Parse(data.Rows[i]["rTuChi"].ToString());
                    }
                }
            }
            else
            {
                for (int i = 0; i < dtDonVi.Rows.Count; i++)
                {
                    if (dtDonVi.Rows[i]["rTuChi"].ToString() != "")
                    {
                        TongTien += long.Parse(dtDonVi.Rows[i]["rTuChi"].ToString());
                    }
                }
            }

            String Tien = "";
            Tien = CommonFunction.TienRaChu(TongTien).ToString();

            //Ghi chú
            DataTable dt = new DataTable();
            dt.Columns.Add("sGhiChu", typeof(String));
            int soChu1Trang = 80;

            //Lấy ghi chú của đơn vị
            String sGhiChu = QuyetToan_ReportModels.LayGhiChuQuyetToan(MaND, iID_MaDonVi);

            ArrayList arrDongTong = new ArrayList();
            String[] arrDong = Regex.Split(sGhiChu, "&#10;");

            for (int i = 0; i < arrDong.Length; i++)
            {
                if (arrDong[i] != "")
                {
                    int tg = 0;
                    String s = "";
                    String[] arrDongCon = arrDong[i].Split(' ');
                    for (int j = 0; j < arrDongCon.Length; j++)
                    {
                        
                            int x = arrDongCon[j].Length;
                            tg = tg + x + 1;
                            if (tg > soChu1Trang)
                            {
                                arrDongTong.Add(s);
                                j--;
                                tg = 0;
                                s = "";
                                continue;
                            }
                            s += arrDongCon[j].Trim() + " ";
                        
                    }
                    if (tg <= soChu1Trang) arrDongTong.Add(s);

                }
            }
            for (int j = 0; j < arrDongTong.Count; j++)
            {
                r = dt.NewRow();
                r["sGhiChu"] = arrDongTong[j];
                dt.Rows.Add(r);
            }

            SoDong = data.Rows.Count;

            for (int i = 0; i < dtsTM.Rows.Count; i++)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(dtsTM.Rows[i]["sMoTa"])))
                    SoDong++;
            }
            for (int i = 0; i < dtsM.Rows.Count; i++)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(dtsM.Rows[i]["sMoTa"])))
                    SoDong++;
            }
            for (int i = 0; i < dtsL.Rows.Count; i++)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(dtsL.Rows[i]["sMoTa"])))
                    SoDong++;
            }
            for (int i = 0; i < dtsLNS.Rows.Count; i++)
            {
                if (!String.IsNullOrEmpty(Convert.ToString(dtsLNS.Rows[i]["sMoTa"])))
                    SoDong++;
            }
            fr.AddTable("dtDongTrang", dt);
            fr.SetValue("Tien", Tien);
            fr.AddTable("dtsTM", dtsTM);
            fr.AddTable("dtsM", dtsM);
            fr.AddTable("dtsL", dtsL);
            fr.AddTable("dtsLNS", dtsLNS);
            fr.AddTable("dtsLNS1", dtsLNS1);
            fr.AddTable("dtsLNS3", dtsLNS3);
            fr.AddTable("dtsLNS5", dtsLNS5);
            int KhoanhCachDong = 120;
            int SoDongTrang1 = 23;
            int SoDongTrang2 = 47;
            int SoDongGhiChu = dt.Rows.Count;
           
            //trang 1 voi cỡ chữ 10, số dòng trên trang 23 dòng
            if (SoDongGhiChu == 0)
            {
                SoDongTrang1 = 23;
                if(SoDong<=SoDongTrang1+3 && SoDong>SoDongTrang1)
                    KhoanhCachDong = 158 + (SoDongTrang1 - SoDong)*2;
            }
             //có ghi chú
            else
            {
                if(SoDongGhiChu<=10)
                {
                    if (SoDong + SoDongGhiChu > SoDongTrang1 - 3 && SoDong + SoDongGhiChu < SoDongTrang1 + 3)
                    {
                        KhoanhCachDong = 200;
                    }

                }
            }

            fr.SetExpression("test", "<#Row height(Autofit;"+KhoanhCachDong+")>");
            data.Dispose();
            dtsTM.Dispose();
            dtsM.Dispose();
            dtsL.Dispose();
            dtsLNS.Dispose();
            dtsLNS1.Dispose();
            dtsLNS3.Dispose();
            dtsLNS5.Dispose();

        }
        
        /// <summary>
        /// Lấy loại ngân sách dựa vào quý, mã phòng ban
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="Thang_Quy">Quý</param>
        /// <param name="iID_MaNamNganSach">Năm ngân sách</param>
        /// <param name="LoaiThongTri">Loại thông tri</param>
        /// <param name="sLNS">Loại ngân sách</param>
        /// <param name="iID_MaPhongBan">Mã phòng ban</param>
        /// <returns></returns>
        public JsonResult LayDanhSachLNS(String ParentID, String Thang_Quy, String iID_MaNamNganSach, String LoaiThongTri, String sLNS, String iID_MaPhongBan)
        {
            String MaND = User.Identity.Name;
            String sViewPath = "~/Views/DungChung/DonVi/LNS_DanhSach_ThongTri.ascx";

            DataTable dt = QuyetToan_ReportModels.dtLoaiThongTri_LNS(Thang_Quy, iID_MaNamNganSach, MaND, LoaiThongTri, iID_MaPhongBan);
            
            if (String.IsNullOrEmpty(sLNS))
            {
                sLNS = Guid.Empty.ToString();
            }
            
            DanhSachDonViModels Model = new DanhSachDonViModels(MaND, sLNS, dt, ParentID);
            String strDonVi = HamChung.RenderPartialViewToStringLoad(sViewPath, Model, this);

            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách đơn vị dựa vào quý, loại ngân sách, phòng ban
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="Thang_Quy">Quý</param>
        /// <param name="iID_MaNamNganSach">Năm ngân sách</param>
        /// <param name="LoaiThongTri">Loại thông tri</param>
        /// <param name="sLNS">Loại ngân sách</param>
        /// <param name="iID_MaDonVi">Mã đơn vị</param>
        /// <param name="iID_MaPhongBan">Mã phòng ban</param>
        /// <returns></returns>
        public JsonResult LayDanhSachDonVi(String ParentID, String Thang_Quy, String iID_MaNamNganSach, String LoaiThongTri, String sLNS, String iID_MaDonVi, String iID_MaPhongBan)
        {
            String MaND = User.Identity.Name;
            string sViewPath = "~/Views/DungChung/DonVi/DonVi_DanhSach_ThongTri.ascx";

            DataTable dt = QuyetToan_ReportModels.dtLNS_DonVi(Thang_Quy, iID_MaNamNganSach, MaND, sLNS, iID_MaPhongBan);

            if (String.IsNullOrEmpty(iID_MaDonVi))
            {
                iID_MaDonVi = Guid.Empty.ToString();
            }
            
            DanhSachDonViModels Model = new DanhSachDonViModels(MaND, iID_MaDonVi, dt, ParentID);
            String strDonVi = HamChung.RenderPartialViewToStringLoad(sViewPath, Model, this);
            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy ghi chú của đơn vị
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="iID_MaDonVi">Mã đơn vị</param>
        /// <returns></returns>
        public JsonResult LayGhiChuQuyetToan(String ParentID, String MaND, String iID_MaDonVi)
        {
            string sGhiChu = "";
            String strDonVi = "";

            sGhiChu = QuyetToan_ReportModels.LayGhiChuQuyetToan(MaND, iID_MaDonVi);
            
            if (iID_MaDonVi != "-1")
                strDonVi = MyHtmlHelper.TextArea(ParentID, sGhiChu, "sGhiChu", "", "style=\"width:100%; height: 220px\" onchange=\"changeTest(this.value)\"");
            
            return Json(strDonVi, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thêm mới hoặc cập nhật ghi chú của đơn vị trong quyết toán thông tri
        /// </summary>
        /// <param name="sGhiChu">Nội dung ghi chú</param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="iID_MaDonVi">Mã đơn vị</param>
        /// <returns></returns>
        public ActionResult CapNhatGhiChuQuyetToan(String sGhiChu, String MaND, String iID_MaDonVi)
        {
           QuyetToan_ReportModels.CapNhatGhiChuQuyetToan(sGhiChu, MaND, iID_MaDonVi);
           
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thêm mới hoặc cập nhật loại quyết toán
        /// </summary>
        /// <param name="sLoaiQuyetToan"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <returns></returns>
        public ActionResult CapNhatLoaiQuyetToan(String sLoaiQuyetToan, String MaND)
        {
            QuyetToan_ReportModels.CapNhatLoaiQuyetToan(sLoaiQuyetToan, MaND);
            
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy loại quyết toán
        /// </summary>
        /// <param name="ParentID"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <returns></returns>
        public ActionResult LayLoaiQuyetToan(String ParentID, String MaND) 
        {
            string sLoaiQuyetToan = "";

            sLoaiQuyetToan = QuyetToan_ReportModels.LayLoaiQuyetToan(ParentID, MaND);

            return Json(sLoaiQuyetToan, JsonRequestBehavior.AllowGet);
        }

    }
}

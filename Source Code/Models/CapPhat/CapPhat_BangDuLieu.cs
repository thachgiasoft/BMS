﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.Abstract;
using System.Collections.Specialized;
using DomainModel;
using System.Data;
using DomainModel.Controls;
using System.Data.SqlClient;

namespace VIETTEL.Models
{
    /// <summary>
    /// Lớp CapPhat_BangDuLieu cho phần bảng của Cấp phát
    /// </summary>
    public class CapPhat_BangDuLieu : BangDuLieu
    {
        // loai hien thi chung tu chi tiet: tat ca, cap phat, chua cap phat
        public static string MALOAI = "TATCA";
        public static void ThietLapHienThi(int option)
        {
            switch (option)
            {
                case 0: MALOAI = "TATCA";
                    break;
                case 1: MALOAI = "CAPPHAT";
                    break;
                case 2: MALOAI = "CHUACAPPHAT";
                    break;
                default: MALOAI = "TATCA";
                    break;
            }
        }
        public static string MaDonViChoCapPhat = "";
        private List<List<Double>> _arrGiaTriNhom = new List<List<Double>>();
        private List<int> _arrChiSoNhom = new List<int>();
        private List<String> _arrMaMucLucNganSach = new List<String>();
        private DataTable _dtDonVi = null;
        public String strDSChiSoNhom
        {
            get
            {
                String vR = "";
                for (int i = 0; i < _arrChiSoNhom.Count; i++)
                {
                    if (i > 0) vR += ",";
                    vR += _arrChiSoNhom[i];
                }
                return vR;
            }
        }
        private string _dsTruong = "sLNS,sL,sK,sM,sTM,sTTM,sNG,sMoTa";
        private string _dsTruongTieuDe = "LNS,L,K,M,TM,TTM,NG,Nội dung";
        private string _dsTruongDoRong = "60,30,30,40,40,30,30,250";

        public String strMaMucLucNganSach
        {
            get
            {
                String vR = "";
                for (int i = 0; i < _arrMaMucLucNganSach.Count; i++)
                {
                    if (i > 0) vR += ",";
                    vR += _arrMaMucLucNganSach[i];
                }
                return vR;
            }
        }

        /// <summary>
        /// Hàm khởi tạo
        /// </summary>
        /// <param name="iID_MaCapPhat"></param>
        /// <param name="MaND">Mã người dùng</param>
        /// <param name="IPSua">IP của máy yêu cầu</param>
        public CapPhat_BangDuLieu(String iID_MaCapPhat, Dictionary<String, String> arrGiaTriTimKiem,String maLoai, String MaND, String IPSua)
        {
            this._iID_Ma = iID_MaCapPhat;
            this._MaND = MaND;
            this._IPSua = IPSua;

            String SQL;
            SqlCommand cmd;
            SQL = "SELECT * FROM CP_CapPhat WHERE iID_MaCapPhat=@iID_MaCapPhat AND iTrangThai=1";
            cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@iID_MaCapPhat", _iID_Ma);
            cmd.CommandText = SQL;
            _dtBang = Connection.GetDataTable(cmd);
            cmd.Dispose();
            _dtDonVi = NganSach_HamChungModels.DSDonViCuaNguoiDung(_MaND);
            int iID_MaTrangThaiDuyet = Convert.ToInt32(_dtBang.Rows[0]["iID_MaTrangThaiDuyet"]);

            Boolean ND_DuocSuaChungTu = LuongCongViecModel.NguoiDung_DuocSuaChungTu(CapPhatModels.iID_MaPhanHe, MaND, iID_MaTrangThaiDuyet);
            if (LuongCongViecModel.KiemTra_TrangThaiDaDuyet(CapPhatModels.iID_MaPhanHe, iID_MaTrangThaiDuyet))
            {
                _ChiDoc = true;
            }

            if (ND_DuocSuaChungTu == false)
            {
                _ChiDoc = true;
            }

            if (LuongCongViecModel.KiemTra_TrangThaiTrinhDuyet(CapPhatModels.iID_MaPhanHe, iID_MaTrangThaiDuyet) &&
                ND_DuocSuaChungTu)
            {
                _CoCotDuyet = true;
                _DuocSuaDuyet = true;
            }

            if (LuongCongViecModel.KiemTra_TrangThaiTuChoi(CapPhatModels.iID_MaPhanHe, iID_MaTrangThaiDuyet))
            {
                _CoCotDuyet = true;
            }

            _DuocSuaChiTiet = LuongCongViecModel.NguoiDung_DuocThemChungTu(CapPhatModels.iID_MaPhanHe, MaND);

            _dtChiTiet = CapPhat_ChungTuChiTietModels.LayDtChungTuChiTietCuc(_iID_Ma, arrGiaTriTimKiem, "", MaND);

            _dtChiTiet_Cu = _dtChiTiet.Copy();

            int d = 0;
            String[] arrDSTruongTien = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            for (int i = 0; i < dtChiTiet.Rows.Count; i++)
            {
                if (i > 0)
                {
                    if (dtChiTiet.Rows[i - 1]["iID_MaMucLucNganSach"].ToString() != dtChiTiet.Rows[i]["iID_MaMucLucNganSach"].ToString())
                    {
                        d++;
                    }
                }

                _arrChiSoNhom.Add(d);
                if (Convert.ToBoolean(dtChiTiet.Rows[i]["bLaHangCha"]) == false)
                {
                    Boolean okKhongCoDuLieu = true;
                    for (int j = 0; j < arrDSTruongTien.Length; j++)
                    {
                        if (Convert.ToDouble(dtChiTiet.Rows[i][arrDSTruongTien[j]]) != 0)
                        {
                            okKhongCoDuLieu = false;
                            break;
                        }
                    }
                    if (okKhongCoDuLieu)
                    {
                        dtChiTiet.Rows[i]["iID_MaDonVi"] = MaDonViChoCapPhat;
                    }
                }
            }

            DienDuLieu(maLoai);
        }
        /// <summary>
        /// Hàm hủy bỏ sẽ hủy dữ liệu của bảng _dtDonVi
        /// </summary>
        ~CapPhat_BangDuLieu()
        {
            if (_dtDonVi != null) _dtDonVi.Dispose();
        }

        /// <summary>
        /// Thuộc tính lấy danh sách mã đơn vị và tên đơn vị cho Javascript
        /// </summary>
        public String strDSDonVi
        {
            get
            {
                String _strDSDonVi = "";
                for (int csDonVi = 0; csDonVi < _dtDonVi.Rows.Count; csDonVi++)
                {
                    if (csDonVi > 0) _strDSDonVi += "##";
                    _strDSDonVi += String.Format("{0}##{1}", _dtDonVi.Rows[csDonVi]["iID_MaDonVi"], _dtDonVi.Rows[csDonVi]["sTen"]);
                }
                return _strDSDonVi;
            }
        }
        /// <summary>
        /// Hàm điền dữ liệu
        /// Mục đích: Điền tất cả thông tin vào các tham số của đối tượng Bảng dữ liệu
        /// </summary>
        protected void DienDuLieu(String sMaLoai)
        {
            // lấy danh sách trường tiền chứng từ
            String[] arrDSTruongTien = MucLucNganSachModels.strDSTruongTien.Split(',');
            if (_arrDuLieu == null)
            {
                if (sMaLoai.ToUpper() == "CAPPHAT")
                {
                    CapNhapHangTongCong();

                    if (sMaLoai == "CapPhat")
                    {
                        int count = dtChiTiet.Rows.Count - 1;
                        bool ok = true;
                        for (int i = count; i >= 0; i--)
                        {
                            ok = false;
                            for (int j = 0; j < arrDSTruongTien.Length; j++)
                            {
                                if (!String.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i][arrDSTruongTien[j]])) && Convert.ToDecimal(_dtChiTiet.Rows[i][arrDSTruongTien[j]]) > 0)
                                {
                                    ok = true;
                                }
                                //neu ma phong ban nguoi dung khac ma phong ban tao
                            }

                            //Loai bo het dong khong co du lieu
                            if (ok == false)
                            {
                                _dtChiTiet.Rows.RemoveAt(i);
                            }

                        }

                    }
                }
                else if (sMaLoai.ToUpper() == "CHUACAPPHAT")
                {
                    int count = dtChiTiet.Rows.Count - 1;
                    bool ok = true;

                    for (int i = count; i >= 0; i--)
                    {
                        ok = false;
                        for (int j = 0; j < arrDSTruongTien.Length; j++)
                        {
                            if ((!String.IsNullOrEmpty(Convert.ToString(_dtChiTiet.Rows[i][arrDSTruongTien[j]])) && Convert.ToDecimal(_dtChiTiet.Rows[i][arrDSTruongTien[j]]) > 0))
                            {
                                ok = true;
                            }
                        }

                        //Loai bo het dong  co du lieu
                        if (ok)
                        {
                            _dtChiTiet.Rows.RemoveAt(i);
                        }

                    }
                    CapNhapHangTongCong();
                }
                else
                    CapNhapHangTongCong();
                
                CapNhapDanhSachMaHang();
                CapNhapDanhSachMaCot_Fixed();
                CapNhapDanhSachMaCot_Slide();
                CapNhapDanhSachMaCot_Them();
                CapNhap_arrLaHangCha();
                CapNhap_arrEdit();
                CapNhap_arrDuLieu();
                CapNhap_arrThayDoi();
            }
        }

        /// <summary>
        /// Hàm cập nhập vào tham số _arrDSMaHang
        /// </summary>
        protected void CapNhapDanhSachMaHang()
        {
            _arrDSMaHang = new List<string>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                DataRow R = _dtChiTiet.Rows[i];
                String MaHang = "";
                if (Convert.ToBoolean(R["bLaHangCha"]) == false)
                {
                    MaHang = String.Format("{0}_{1}", R["iID_MaCapPhatChiTiet"], R["iID_MaMucLucNganSach"]);
                }

                _arrDSMaHang.Add(MaHang);

                _arrMaMucLucNganSach.Add(Convert.ToString(R["iID_MaMucLucNganSach"]));
            }
        }

        /// <summary>
        /// Hàm thêm danh sách cột Fixed vào bảng
        ///     - Cột Fixed của bảng gồm:
        ///         + Các trường của mục lục ngân sách
        ///         + Trường sMaCongTrinh, sTenCongTrinh của cột tiền
        ///     - Cập nhập số lượng cột Fixed
        /// </summary>
        protected void CapNhapDanhSachMaCot_Fixed()
        {
            //Khởi tạo các mảng
            _arrHienThiCot = new List<Boolean>();
            _arrTieuDe = new List<string>();
            _arrDSMaCot = new List<string>();
            _arrWidth = new List<int>();

            String[] arrDSTruong = _dsTruong.Split(',');
            String[] arrDSTruongTieuDe = _dsTruongTieuDe.Split(',');
            String[] arrDSTruongTien = MucLucNganSachModels.strDSTruongTien.Split(',');
            String[] arrDSTruongTienTieuDe = MucLucNganSachModels.strDSTruongTienTieuDe.Split(',');
            String[] arrDSTruongTienDoRong = MucLucNganSachModels.strDSTruongTienDoRong.Split(',');
            String[] arrDSTruongDoRong = _dsTruongDoRong.Split(',');

            //Xác định các cột tiền sẽ hiển thị
            _arrCotTienDuocHienThi = new Dictionary<String, Boolean>();
            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {
                _arrCotTienDuocHienThi.Add(arrDSTruongTien[j], false);
                for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
                {
                    DataRow R = _dtChiTiet.Rows[i];
                    //HungPX: NghiepNC yêu cầu chỉ để trường rTuChi
                    if (Convert.ToBoolean(R["b" + arrDSTruongTien[j]]) && arrDSTruongTien[j] == "rTuChi")
                    {
                        _arrCotTienDuocHienThi[arrDSTruongTien[j]] = true;
                        break;
                    }
                }
            }
            DataTable dtCapPhat = CapPhat_ChungTuModels.LayToanBoThongTinChungTu(_iID_Ma);
            DataRow dr = dtCapPhat.Rows[0];
            int index = -1;
            if (dtCapPhat != null && dtCapPhat.Rows.Count > 0)
            {
                index = getIndex(Convert.ToString(dr["sLoai"]));
            }
            //Tiêu đề fix: Thêm trường sMaCongTrinh, sTenCongTrinh
            for (int j = 0; j < arrDSTruongTieuDe.Length; j++)
            {
                _arrDSMaCot.Add(arrDSTruong[j]);
                _arrTieuDe.Add(arrDSTruongTieuDe[j]);
                _arrWidth.Add(Convert.ToInt32(arrDSTruongDoRong[j]));
                if (arrDSTruong[j] == "rTongSo")
                {
                    _arrHienThiCot.Add(false);
                }
                else
                {
                    if(arrDSTruong[j]!="sMoTa" && (getIndex(arrDSTruong[j]) > index || index == -1))
                        _arrHienThiCot.Add(false);
                    else
                        _arrHienThiCot.Add(true);
                }

                _arrSoCotCungNhom.Add(1);
                _arrTieuDeNhomCot.Add("");
            }

            _nCotFixed = _arrDSMaCot.Count;
        }

        /// <summary>
        /// Hàm thêm danh sách cột Slide vào bảng
        ///     - Cột Slide của bảng gồm:
        ///         + Trường iID_MaDonVi
        ///         + Trường của cột tiền trừ sMaCongTrinh, sTenCongTrinh
        ///             - Cột phân bổ: Cột tổng phân bổ cho đơn vị
        ///             - Cột đã cấp: Cột đã cấp cho đơn vị trong năm ngân sách
        ///             - Cột còn lại: Cột còn lại chưa cấp cho đơn vị
        ///         + Trường sTongSo
        ///         + Trường bDongY, sLyDo
        ///     - Cập nhập số lượng cột Slide
        /// </summary>
        protected void CapNhapDanhSachMaCot_Slide()
        {
            String[] arrDSTruongTien = "rNgay,rSoNguoi,rChiTaiKhoBac,rTonKho,rTuChi,rChiTapTrung,rHangNhap,rHangMua,rHienVat,rDuPhong,rPhanCap,rTongSo".Split(',');
            String[] arrDSTruongTienTieuDe = "Ngày,Người,Chi tại kho bạc,Tồn kho,Tự chi,Chi tập trung,Hàng nhập,Hàng mua,Hiện vật,Dự phòng,Phân cấp,Tổng số".Split(',');
            String[] arrDSTruongTienDoRong = "100,100,100,100,200,100,100,100,100,100,100,100,100".Split(',');

            //HungPX: Add cột mã đơn vị
            _arrDSMaCot.Add("iID_MaDonVi");
            _arrTieuDe.Add("Đơn vị");
            _arrWidth.Add(40);
            _arrHienThiCot.Add(true);
            _arrSoCotCungNhom.Add(1);
            _arrTieuDeNhomCot.Add("");

            //HungPX: Add cột Tên đơn vị
            _arrDSMaCot.Add("sTenDonVi");
            _arrTieuDe.Add("Tên Đơn vị");
            _arrWidth.Add(150);
            _arrHienThiCot.Add(true);
            _arrSoCotCungNhom.Add(1);
            _arrTieuDeNhomCot.Add("");

            //Tiêu đề tiền
            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {
                if (arrDSTruongTien[j] == "sTenCongTrinh" &&
                    _arrCotTienDuocHienThi[arrDSTruongTien[j]])
                {
                    _arrDSMaCot.Add(arrDSTruongTien[j]);
                    _arrTieuDe.Add(arrDSTruongTienTieuDe[j]);
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(true);
                    _arrSoCotCungNhom.Add(1);
                    _arrTieuDeNhomCot.Add("");
                }
            }
            //Tuannm Yêu cầu  bỏ toàn bộ phần tổng số ngày 24/8/2012
            Boolean bHienThiCot = true;
            for (int j = 0; j < arrDSTruongTien.Length; j++)
            {

                //HungPX : Yêu cầu ẩn cột Tổng số. Yêu cầu ẩn cột Phân Bổ và Còn lại
                if (arrDSTruongTien[j] == "rTongSo")
                {
                    _arrDSMaCot.Add(arrDSTruongTien[j] + "_Phanbo");
                    _arrTieuDe.Add("Phân Bổ");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(false);
                    _arrSoCotCungNhom.Add(4);
                    _arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);


                    _arrDSMaCot.Add(arrDSTruongTien[j]);
                    _arrTieuDe.Add("Cấp phát");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(false);
                    _arrSoCotCungNhom.Add(4);
                    _arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);

                    _arrDSMaCot.Add(arrDSTruongTien[j] + "_DaCap");
                    _arrTieuDe.Add("Đã cấp");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(false);
                    _arrSoCotCungNhom.Add(4);
                    _arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);

                    _arrDSMaCot.Add(arrDSTruongTien[j] + "_ConLai");
                    _arrTieuDe.Add("Còn lại");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(false);
                    _arrSoCotCungNhom.Add(4);
                    _arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);
                }

                if (arrDSTruongTien[j] != "sTenCongTrinh" && arrDSTruongTien[j] != "rTongSo"
                    && _arrCotTienDuocHienThi[arrDSTruongTien[j]])
                {
                    _arrDSMaCot.Add(arrDSTruongTien[j] + "_Phanbo");
                    _arrTieuDe.Add("Phân Bổ");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(false);
                    _arrSoCotCungNhom.Add(4);
                    _arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);

                    _arrDSMaCot.Add(arrDSTruongTien[j] + "_DaCap");
                    _arrTieuDe.Add("Đã cấp");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(bHienThiCot);
                    _arrSoCotCungNhom.Add(4);
                    _arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);

                    _arrDSMaCot.Add(arrDSTruongTien[j]);
                    _arrTieuDe.Add("Cấp phát");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(bHienThiCot);
                    _arrSoCotCungNhom.Add(4);
                    _arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);

                    _arrDSMaCot.Add(arrDSTruongTien[j] + "_ConLai");
                    _arrTieuDe.Add("Còn lại");
                    _arrWidth.Add(Convert.ToInt32(arrDSTruongTienDoRong[j]));
                    _arrHienThiCot.Add(false);
                    _arrSoCotCungNhom.Add(4);
                    _arrTieuDeNhomCot.Add(arrDSTruongTienTieuDe[j]);

                }
            }

            //Them cot duyet
            if (CoCotDuyet)
            {
                //Cột đồng ý
                _arrDSMaCot.Add("bDongY");
                if (_ChiDoc)
                {
                    _arrTieuDe.Add("<div class='check'></div>");
                }
                else
                {
                    _arrTieuDe.Add("<div class='check' onclick='BangDuLieu_CheckAll();'></div>");
                }
                _arrWidth.Add(20);
                _arrHienThiCot.Add(false);
                _arrSoCotCungNhom.Add(1);
                _arrTieuDeNhomCot.Add("");
                //Cột Lý do
                _arrDSMaCot.Add("sLyDo");
                _arrTieuDe.Add("Nhận xét");
                _arrWidth.Add(200);
                _arrHienThiCot.Add(true);
                _arrSoCotCungNhom.Add(1);
                _arrTieuDeNhomCot.Add("");
            }

            _nCotSlide = _arrDSMaCot.Count - _nCotFixed;
        }

        /// <summary>
        /// Hàm thêm các cột thêm của bảng
        /// </summary>
        protected void CapNhapDanhSachMaCot_Them()
        {
            String strDSTruong = "sMaCongTrinh";
            String[] arrDSTruong = strDSTruong.Split(',');
            for (int j = 0; j < arrDSTruong.Length; j++)
            {
                _arrDSMaCot.Add(arrDSTruong[j]);
                _arrTieuDe.Add(arrDSTruong[j]);
                _arrWidth.Add(0);
                _arrHienThiCot.Add(false);
                _arrSoCotCungNhom.Add(1);
                _arrTieuDeNhomCot.Add("");
            }
        }

        /// <summary>
        /// Hàm xác định hàng cha, con
        /// </summary>
        protected void CapNhap_arrLaHangCha()
        {
            //Xác định hàng là hàng cha, con
            _arrCSCha = new List<int>();
            _arrLaHangCha = new List<bool>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                DataRow R = _dtChiTiet.Rows[i];
                //Xac dinh hang nay co phai la hang cha khong?
                _arrLaHangCha.Add(Convert.ToBoolean(R["bLaHangCha"]));
                int CSCha = -1;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (Convert.ToString(R["iID_MaMucLucNganSach_Cha"]) == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach"]))
                    {
                        CSCha = j;
                        break;
                    }
                }
                _arrCSCha.Add(CSCha);
            }
        }

        /// <summary>
        /// Hàm xác định các ô có được Edit hay không
        /// </summary>
        protected void CapNhap_arrEdit()
        {
            _arrEdit = new List<List<string>>();
            // HungPX: lấy giá trị chi tiết đến của chứng từ
            DataTable dtCapPhat;
            SqlCommand cmdCapPhat = new SqlCommand();
            String sqlCapPhat = "Select * from Cp_CapPhat where iID_MaCapPhat = @iID_MaCapPhat";
            cmdCapPhat.Parameters.AddWithValue("@iID_MaCapPhat", _iID_Ma);
            cmdCapPhat.CommandText = sqlCapPhat;
            dtCapPhat = Connection.GetDataTable(cmdCapPhat);
            String sLoai = Convert.ToString(dtCapPhat.Rows[0]["sLoai"]);
            dtCapPhat.Dispose();
            cmdCapPhat.Dispose();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                Boolean okHangChiDoc = false;
                _arrEdit.Add(new List<string>());
                DataRow R = _dtChiTiet.Rows[i];
                //HungPX : cho phép nhập vào hàng cha "sloai"
                if (Convert.ToBoolean(R["bLaHangCha"]))
                {
                    okHangChiDoc = true;
                    if (!String.IsNullOrEmpty(R[sLoai].ToString()) && Convert.ToInt32(R[sLoai].ToString()) > 0)
                        okHangChiDoc = false;
                }

                for (int j = 0; j < _arrDSMaCot.Count; j++)
                {
                    Boolean okOChiDoc = true;
                    //Xac dinh o chi doc
                    if (_arrDSMaCot[j] == "iID_MaDonVi")
                    {
                        //Cot don vi
                        if (_DuocSuaChiTiet && _ChiDoc == false && okHangChiDoc == false)
                        {
                            okOChiDoc = false;
                        }
                    }
                    else if (_arrDSMaCot[j] == "bDongY" || _arrDSMaCot[j] == "sLyDo")
                    {
                        //Cot duyet
                        if (_DuocSuaDuyet && _ChiDoc == false && okHangChiDoc == false)
                        {
                            okOChiDoc = false;
                        }
                    }
                    else
                    {
                        //Cot tien
                        if (_DuocSuaChiTiet &&
                                _ChiDoc == false &&
                                okHangChiDoc == false &&
                                _arrDSMaCot[j] != "rTongSo" &&
                                _arrDSMaCot[j].EndsWith("_PhanBo") == false &&
                                _arrDSMaCot[j].EndsWith("_DaCap") == false &&
                                _arrDSMaCot[j].EndsWith("_ConLai") == false &&
                                _dtChiTiet.Columns.IndexOf('b' + _arrDSMaCot[j]) >= 0 &&
                                Convert.ToBoolean(R['b' + _arrDSMaCot[j]]))
                        {
                            okOChiDoc = false;
                        }
                    }
                    if (okOChiDoc)
                    {
                        _arrEdit[i].Add("");
                    }
                    else
                    {
                        _arrEdit[i].Add("1");
                    }
                }
            }
        }

        /// <summary>
        /// Hàm cập nhập mảng dữ liệu
        /// </summary>
        protected void CapNhap_arrDuLieu()
        {
            _arrDuLieu = new List<List<string>>();
            for (int i = 0; i < _dtChiTiet.Rows.Count; i++)
            {
                _arrDuLieu.Add(new List<string>());
                DataRow R = _dtChiTiet.Rows[i];
                for (int j = 0; j < _arrDSMaCot.Count; j++)
                {
                    //Xac dinh gia tri
                    if (_arrDSMaCot[j].EndsWith("_ConLai"))
                    {
                        Double GT1 = Convert.ToDouble(_arrDuLieu[i][j - 1]);
                        Double GT2 = Convert.ToDouble(_arrDuLieu[i][j - 2]);
                        Double GT3 = Convert.ToDouble(_arrDuLieu[i][j - 3]);
                        _arrDuLieu[i].Add(Convert.ToString(GT3 - GT2 - GT1));
                    }
                    else
                    {
                        _arrDuLieu[i].Add(Convert.ToString(R[_arrDSMaCot[j]]));
                    }
                }
            }
        }

        /// <summary>
        /// Hàm tính lại các ô tổng số và tổng cộng các hàng cha
        /// </summary>
        protected void CapNhapHangTongCong()
        {
            String strDSTruongTien = MucLucNganSachModels.strDSTruongTien_So + ",rTongSo";
            String strDSTruongTien_PhanBo = MucLucNganSachModels.strDSTruongTien_So.Replace(",", "_PhanBo,") + "_PhanBo,rTongSo_PhanBo";
            String strDSTruongTien_DaCap = MucLucNganSachModels.strDSTruongTien_So.Replace(",", "_DaCap,") + "_DaCap,rTongSo_DaCap";
            String strDSTruongTien_ConLai = MucLucNganSachModels.strDSTruongTien_So.Replace(",", "_ConLai,") + "_DaCap,rTongSo_ConLai";
            String[] arrDSTruongTien = strDSTruongTien.Split(',');
            String[] arrDSTruongTien_PhanBo = strDSTruongTien_PhanBo.Split(',');
            String[] arrDSTruongTien_DaCap = strDSTruongTien_DaCap.Split(',');
            String[] arrDSTruongTien_ConLai = strDSTruongTien_ConLai.Split(',');

            int len = arrDSTruongTien.Length;
            //Tinh lai cot tong so
            for (int i = _dtChiTiet.Rows.Count - 1; i >= 0; i--)
            {
                if (Convert.ToBoolean(_dtChiTiet.Rows[i]["bLaHangCha"]) == false)
                {
                    double S;
                    //rTongSo
                    S = 0;
                    for (int k = 0; k < len - 1; k++)
                    {
                        if (arrDSTruongTien[k].StartsWith("rChiTapTrung") == false)
                        {
                            S += Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien[k]]);
                        }
                    }
                    if (Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien[len - 1]]) != S)
                    {
                        _dtChiTiet.Rows[i][arrDSTruongTien[len - 1]] = S;
                    }
                    //rTongSo_PhanBo
                    S = 0;
                    for (int k = 0; k < len - 1; k++)
                    {
                        if (arrDSTruongTien_PhanBo[k].StartsWith("rChiTapTrung") == false)
                        {
                            S += Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien_PhanBo[k]]);
                        }
                    }
                    if (Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien_PhanBo[len - 1]]) != S)
                    {
                        _dtChiTiet.Rows[i][arrDSTruongTien_PhanBo[len - 1]] = S;
                    }
                    //rTongSo_DaCap
                    S = 0;
                    for (int k = 0; k < len - 1; k++)
                    {
                        if (arrDSTruongTien[k].StartsWith("rChiTapTrung") == false)
                        {
                            S += Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien_DaCap[k]]);
                        }
                    }
                    if (Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien_DaCap[len - 1]]) != S)
                    {
                        _dtChiTiet.Rows[i][arrDSTruongTien_DaCap[len - 1]] = S;
                    }
                    ////rTongSo_ConLai
                    //S = 0;
                    //for (int k = 0; k < len - 1; k++)
                    //{
                    //    if (arrDSTruongTien[k].StartsWith("rChiTapTrung") == false)
                    //    {
                    //        S += Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien_ConLai[k]]);
                    //    }
                    //}
                    //if (Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien_ConLai[len - 1]]) != S)
                    //{
                    //    _dtChiTiet.Rows[i][arrDSTruongTien_ConLai[len - 1]] = S;
                    //}
                }
            }
            //Tinh lai cac hang cha
            for (int i = _dtChiTiet.Rows.Count - 1; i >= 0; i--)
            {
                if (Convert.ToBoolean(_dtChiTiet.Rows[i]["bLaHangCha"]))
                {
                    String iID_MaMucLucNganSach = Convert.ToString(_dtChiTiet.Rows[i]["iID_MaMucLucNganSach"]);
                    for (int k = 0; k < len; k++)
                    {
                        if (i == 2 && k > len - 2)
                        {
                            i = 2;
                        }
                        double S, S_PhanBo, S_DaCap, S_ConLai;
                        //rTongSo
                        S = 0;
                        S_PhanBo = 0;
                        S_DaCap = 0;
                        S_ConLai = 0;
                        for (int j = i + 1; j < _dtChiTiet.Rows.Count; j++)
                        {
                            if (iID_MaMucLucNganSach == Convert.ToString(_dtChiTiet.Rows[j]["iID_MaMucLucNganSach_Cha"]))
                            {
                                S += Convert.ToDouble(_dtChiTiet.Rows[j][arrDSTruongTien[k]]);
                                S_PhanBo += Convert.ToDouble(_dtChiTiet.Rows[j][arrDSTruongTien_PhanBo[k]]);
                                S_DaCap += Convert.ToDouble(_dtChiTiet.Rows[j][arrDSTruongTien_DaCap[k]]);
                            }
                        }
                        if (Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien[k]]) != S ||
                            Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien_DaCap[k]]) != S_DaCap ||
                            Convert.ToDouble(_dtChiTiet.Rows[i][arrDSTruongTien_PhanBo[k]]) != S_PhanBo)
                        {
                            _dtChiTiet.Rows[i][arrDSTruongTien[k]] = S;
                            _dtChiTiet.Rows[i][arrDSTruongTien_PhanBo[k]] = S_PhanBo;
                            _dtChiTiet.Rows[i][arrDSTruongTien_DaCap[k]] = S_DaCap;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Hàm lấy thông tin của chỉ tiêu đã cấp cho đơn vị theo Mục lục ngân sách
        /// </summary>
        /// <param name="iID_MaMucLucNganSach"></param>
        /// <param name="iID_MaDonVi"></param>
        /// <param name="iNamLamViec"></param>
        /// <param name="iID_MaNguonNganSach"></param>
        /// <param name="iID_MaNamNganSach"></param>
        /// <returns></returns>
        public static String LayGiaTri_ChiTieu_DaCap(String iDM_MaLoaiCapPhat,
                                                     String iID_MaTinhChatCapThu,
                                                     String sDSLNS, 
                                                     String iID_MaMucLucNganSach,
                                                     String iID_MaDonVi,
                                                     int iNamLamViec,
                                                     String dNgayCapPhat,
                                                     int iID_MaNguonNganSach,
                                                     int iID_MaNamNganSach)
        {
            String vR = "";
            DataTable dtTongPhanBo = PhanBo_PhanBoChiTietModels.Get_dtTongPhanBoChoDonVi(iID_MaMucLucNganSach, iID_MaDonVi, iNamLamViec, dNgayCapPhat, iID_MaNguonNganSach, iID_MaNamNganSach);
            DataTable dtTongDaCapPhat = CapPhat_ChungTuChiTietModels.LayDtTongCapPhatChoDonVi(iDM_MaLoaiCapPhat, iID_MaTinhChatCapThu,sDSLNS,iID_MaMucLucNganSach, iID_MaDonVi, iNamLamViec, dNgayCapPhat, iID_MaNguonNganSach, iID_MaNamNganSach);

            String[] arrDSTruongTien_So = MucLucNganSachModels.strDSTruongTien_So.Split(',');
            String strTruong = MucLucNganSachModels.strDSTruongTien_So;
            String strTongPhanBo = "";
            String strTongDaCapPhat = "";
            for (int i = 0; i < arrDSTruongTien_So.Length; i++)
            {
                if (i > 0)
                {
                    strTongPhanBo += ",";
                    strTongDaCapPhat += ",";
                }
                String Truong = "Sum" + arrDSTruongTien_So[i];
                Object Value = 0;
                if (dtTongPhanBo.Rows.Count > 0 && dtTongPhanBo.Rows[0][Truong] != DBNull.Value && dtTongPhanBo != null)
                {
                    Value = dtTongPhanBo.Rows[0][Truong];
                }

                strTongPhanBo += Value;

                Value = 0;
                if (dtTongDaCapPhat.Rows.Count > 0 && dtTongDaCapPhat.Rows[0][Truong] != DBNull.Value && dtTongDaCapPhat != null)
                {
                    Value = dtTongDaCapPhat.Rows[0][Truong];
                }
                strTongDaCapPhat += Value;
            }

            dtTongPhanBo.Dispose();
            dtTongDaCapPhat.Dispose();

            vR = String.Format("{0}#{1}#{2}", strTruong, strTongPhanBo, strTongDaCapPhat);
            return vR;
        }
        /*
         * <summary>
         * lấy index của chuỗi loại ngân sách: LNS, L, K, M, TM, NG 
         * index cha lớn hơn index con
         * </summary>
         * */
        public static int getIndex(string val)
        {
            String[] arrDSTruong = MucLucNganSachModels.arrDSTruong;
            int index = -1;
            for (int i = 0; i < arrDSTruong.Length; i++)
                if (val == arrDSTruong[i])
                {
                    index = i;
                    break;
                }
            return index;
        }
        
    }
}
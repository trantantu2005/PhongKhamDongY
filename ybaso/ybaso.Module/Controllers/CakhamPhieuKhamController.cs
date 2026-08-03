using ClosedXML.Excel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Templates;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using System;
using System.IO;
using System.Linq;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.Controllers
{
    public class CakhamPhieuKhamController : ObjectViewController<DetailView, Cakham>
    {
        private readonly SimpleAction xuatPhieuKhamAction;

        public CakhamPhieuKhamController()
        {
            xuatPhieuKhamAction = new SimpleAction(this, "XuatPhieuKham", PredefinedCategory.View)
            {
                Caption = "Phiếu khám",
                ImageName = "BO_Report",
                PaintStyle = ActionItemPaintStyle.CaptionAndImage
            };

            xuatPhieuKhamAction.Execute += XuatPhieuKhamAction_Execute;
        }

        private void XuatPhieuKhamAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            Cakham caHienTai = View.CurrentObject as Cakham;

            if (caHienTai == null)
            {
                Application.ShowViewStrategy.ShowMessage(
                    "Không tìm thấy ca khám hiện tại.",
                    InformationType.Warning,
                    4000,
                    InformationPosition.Top
                );
                return;
            }

            if (caHienTai.BenhNhan == null)
            {
                Application.ShowViewStrategy.ShowMessage(
                    "Vui lòng chọn bệnh nhân trước khi xuất phiếu khám.",
                    InformationType.Warning,
                    4000,
                    InformationPosition.Top
                );
                return;
            }

            if (ObjectSpace.IsNewObject(caHienTai) || ObjectSpace.ModifiedObjects.Count > 0)
            {
                ObjectSpace.CommitChanges();
                caHienTai = View.CurrentObject as Cakham;
            }

            IObjectSpace phieuObjectSpace = Application.CreateObjectSpace(typeof(PhieuKhamXuatFile));
            Cakham ca = phieuObjectSpace.GetObject(caHienTai);

            string tenBenhNhan = ca.BenhNhan == null ? "BenhNhan" : ca.BenhNhan.ToString();
            string fileName = TaoTenFileAnToan($"PhieuKham_{tenBenhNhan}_{ca.NgayCT:yyyyMMdd_HHmm}.xlsx");

            PhieuKhamXuatFile phieu = phieuObjectSpace.CreateObject<PhieuKhamXuatFile>();
            phieu.CaKham = ca;
            phieu.TenPhieu = $"Phiếu khám - {tenBenhNhan}";
            phieu.NgayXuat = DateTime.Now;

            using MemoryStream stream = new MemoryStream();
            TaoFileExcelPhieuKham(ca, stream);
            stream.Position = 0;

            FileData fileData = phieuObjectSpace.CreateObject<FileData>();
            fileData.LoadFromStream(fileName, stream);

            phieu.TepTin = fileData;

            TaoDuLieuXemTruoc(phieuObjectSpace, phieu, ca);

            phieuObjectSpace.CommitChanges();

            DetailView detailView = Application.CreateDetailView(phieuObjectSpace, phieu);
            detailView.ViewEditMode = ViewEditMode.View;

            e.ShowViewParameters.CreatedView = detailView;
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
        }

        private static void TaoFileExcelPhieuKham(Cakham ca, Stream outputStream)
        {
            using XLWorkbook workbook = new XLWorkbook();

            TaoSheetPhieuKham(workbook, ca);
            TaoSheetChanDoan(workbook, ca);
            TaoSheetNguTa(workbook, ca);
            TaoSheetBenhChung(workbook, ca);
            TaoSheetPhapTri(workbook, ca);

            workbook.SaveAs(outputStream);
        }

        private static void TaoSheetPhieuKham(XLWorkbook workbook, Cakham ca)
        {
            IXLWorksheet ws = workbook.Worksheets.Add("Phiếu khám");

            ws.Cell(1, 1).Value = "PHIẾU KHÁM";
            ws.Range(1, 1, 1, 8).Merge();
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(3, 1).Value = "Thông tin bệnh nhân";
            ws.Range(3, 1, 3, 8).Merge();
            ws.Cell(3, 1).Style.Font.Bold = true;
            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.LightGray;

            string gioiTinh = "";
            if (ca.BenhNhan != null)
            {
                gioiTinh = ca.BenhNhan.Gioitinh == 1 ? "Nam" :
                           ca.BenhNhan.Gioitinh == 2 ? "Nữ" : "";
            }

            GhiThongTin(ws, 4, 1, "Họ và tên:", ca.BenhNhan?.ToString());
            GhiThongTin(ws, 4, 5, "Tuổi:", ca.Tuoi.ToString());

            GhiThongTin(ws, 5, 1, "Địa chỉ:", ca.BenhNhan?.Diachi);
            GhiThongTin(ws, 5, 5, "Điện thoại:", ca.BenhNhan?.Dienthoai);

            GhiThongTin(ws, 6, 1, "Giới tính:", gioiTinh);
            GhiThongTin(ws, 6, 5, "Ngày khám:", ca.NgayCT.ToString("dd/MM/yyyy HH:mm"));

            GhiThongTin(ws, 7, 1, "Huyết áp:", ca.HuyetAp);
            GhiThongTin(ws, 7, 5, "Mạch:", ca.Mach);

            GhiThongTin(ws, 8, 1, "Chiều cao:", ca.Chieucao.ToString("0.##"));
            GhiThongTin(ws, 8, 3, "Cân nặng:", ca.Cannang.ToString("0.##"));
            GhiThongTin(ws, 8, 5, "BMI:", ca.BMI.ToString("0.##"));
            GhiThongTin(ws, 8, 7, "NĐMT:", ca.NhietDoMoiTruong.ToString("0.##"));

            GhiThongTin(ws, 9, 1, "Lý do khám:", ca.LydoKham);
            ws.Range(9, 2, 9, 8).Merge();

            int row = 11;

            ws.Cell(row, 1).Value = "Số đo";
            ws.Range(row, 1, row, 8).Merge();
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;

            GhiBangSoDo(ws, row, "Chi trên", ca, NhomChiDo.ChiTren);
            row += ca.SoDoChiTiets.Count(x => x.NhomChi == NhomChiDo.ChiTren) + 4;

            GhiBangSoDo(ws, row, "Chi dưới", ca, NhomChiDo.ChiDuoi);

            ws.Columns().AdjustToContents();
            ws.Rows().AdjustToContents();

            ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        private static void GhiBangSoDo(IXLWorksheet ws, int startRow, string title, Cakham ca, NhomChiDo nhomChi)
        {
            int row = startRow;

            ws.Cell(row, 1).Value = title;
            ws.Range(row, 1, row, 8).Merge();
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 1).Style.Font.Bold = true;
            ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;

            ws.Cell(row, 1).Value = "Tinh huyệt";
            ws.Cell(row, 2).Value = "Trái";
            ws.Cell(row, 3).Value = "Trái +/-";
            ws.Cell(row, 4).Value = "Lệch TB";
            ws.Cell(row, 5).Value = "Phải";
            ws.Cell(row, 6).Value = "Phải +/-";
            ws.Cell(row, 7).Value = "Chênh trái/phải";
            ws.Cell(row, 8).Value = "Ngưỡng";
            ws.Range(row, 1, row, 8).Style.Font.Bold = true;
            ws.Range(row, 1, row, 8).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;

            var list = ca.SoDoChiTiets
                .Where(x => x.NhomChi == nhomChi)
                .OrderBy(x => x.TT)
                .ToList();

            foreach (var item in list)
            {
                ws.Cell(row, 1).Value = item.TinhHuyet;
                ws.Cell(row, 2).Value = item.TraiDoC;
                ws.Cell(row, 3).Value = item.TraiDanhGia;
                ws.Cell(row, 4).Value = item.LechTrungBinh;
                ws.Cell(row, 5).Value = item.PhaiDoC;
                ws.Cell(row, 6).Value = item.PhaiDanhGia;
                ws.Cell(row, 7).Value = item.ChenhTraiPhai;
                ws.Cell(row, 8).Value = $"{item.NguongDuoi:0.##} - {item.NguongTren:0.##}";
                row++;
            }

            ws.Range(startRow, 1, row - 1, 8).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(startRow, 1, row - 1, 8).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
        }

        private static void TaoSheetChanDoan(XLWorkbook workbook, Cakham ca)
        {
            IXLWorksheet ws = workbook.Worksheets.Add("Chẩn đoán");

            GhiTieuDeSheet(ws, "CHẨN ĐOÁN");

            int row = 3;
            ws.Cell(row, 1).Value = "Nhóm";
            ws.Cell(row, 2).Value = "Kết quả";
            ws.Range(row, 1, row, 2).Style.Font.Bold = true;
            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;

            foreach (var item in ca.ChanDoanTuDongs.OrderBy(x => x.Nhom))
            {
                ws.Cell(row, 1).Value = item.Nhom;
                ws.Cell(row, 2).Value = item.KetQua;
                row++;
            }

            DinhDangBang(ws, 3, 1, row - 1, 2);
        }

        private static void TaoSheetNguTa(XLWorkbook workbook, Cakham ca)
        {
            IXLWorksheet ws = workbook.Worksheets.Add("Ngũ tà");

            GhiTieuDeSheet(ws, "NGŨ TÀ");

            int row = 3;
            ws.Cell(row, 1).Value = "Nhóm";
            ws.Cell(row, 2).Value = "Kết quả";
            ws.Range(row, 1, row, 2).Style.Font.Bold = true;
            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;

            foreach (var item in ca.NguTaTuDongs.OrderBy(x => x.Nhom))
            {
                ws.Cell(row, 1).Value = item.Nhom;
                ws.Cell(row, 2).Value = item.KetQua;
                row++;
            }

            DinhDangBang(ws, 3, 1, row - 1, 2);
        }

        private static void TaoSheetBenhChung(XLWorkbook workbook, Cakham ca)
        {
            IXLWorksheet ws = workbook.Worksheets.Add("Bệnh chứng");

            GhiTieuDeSheet(ws, "BỆNH CHỨNG");

            int row = 3;
            ws.Cell(row, 1).Value = "Bệnh chứng";
            ws.Cell(row, 2).Value = "Triệu chứng";
            ws.Range(row, 1, row, 2).Style.Font.Bold = true;
            ws.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;

            foreach (var item in ca.BenhChungTuDongs.OrderBy(x => x.TenBenhChung))
            {
                ws.Cell(row, 1).Value = item.TenBenhChung;
                ws.Cell(row, 2).Value = item.TrieuChung;
                row++;
            }

            DinhDangBang(ws, 3, 1, row - 1, 2);
        }

        private static void TaoSheetPhapTri(XLWorkbook workbook, Cakham ca)
        {
            IXLWorksheet ws = workbook.Worksheets.Add("Pháp trị");

            GhiTieuDeSheet(ws, "PHÁP TRỊ");

            int row = 3;
            ws.Cell(row, 1).Value = "Bệnh chứng";
            ws.Cell(row, 2).Value = "Pháp điều trị";
            ws.Cell(row, 3).Value = "Phương dược";
            ws.Cell(row, 4).Value = "Huyệt";
            ws.Range(row, 1, row, 4).Style.Font.Bold = true;
            ws.Range(row, 1, row, 4).Style.Fill.BackgroundColor = XLColor.LightGray;
            row++;

            foreach (var item in ca.PhapTriTuDongs.OrderBy(x => x.BenhChung))
            {
                ws.Cell(row, 1).Value = item.BenhChung;
                ws.Cell(row, 2).Value = item.PhapDieuTri;
                ws.Cell(row, 3).Value = item.PhuongDuoc;
                ws.Cell(row, 4).Value = item.Huyet;
                row++;
            }

            DinhDangBang(ws, 3, 1, row - 1, 4);
        }

        private static void GhiTieuDeSheet(IXLWorksheet ws, string title)
        {
            ws.Cell(1, 1).Value = title;
            ws.Range(1, 1, 1, 6).Merge();
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 16;
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private static void GhiThongTin(IXLWorksheet ws, int row, int col, string label, string value)
        {
            ws.Cell(row, col).Value = label;
            ws.Cell(row, col).Style.Font.Bold = true;
            ws.Cell(row, col + 1).Value = value ?? "";
        }

        private static void DinhDangBang(IXLWorksheet ws, int row1, int col1, int row2, int col2)
        {
            if (row2 < row1)
                row2 = row1;

            ws.Range(row1, col1, row2, col2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(row1, col1, row2, col2).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            ws.Range(row1, col1, row2, col2).Style.Alignment.WrapText = true;

            ws.Columns().AdjustToContents();
            ws.Rows().AdjustToContents();
        }
        private static void TaoDuLieuXemTruoc(IObjectSpace objectSpace, PhieuKhamXuatFile phieu, Cakham ca)
        {
            foreach (var item in ca.SoDoChiTiets.OrderBy(x => x.TT))
            {
                PhieuKhamSoDoPreview preview = objectSpace.CreateObject<PhieuKhamSoDoPreview>();

                preview.PhieuKham = phieu;
                preview.TT = item.TT;
                preview.NhomChi = item.NhomChi == NhomChiDo.ChiTren ? "Chi trên" : "Chi dưới";
                preview.TinhHuyet = item.TinhHuyet;
                preview.Trai = item.TraiDoC;
                preview.TraiDanhGia = item.TraiDanhGia;
                preview.Phai = item.PhaiDoC;
                preview.PhaiDanhGia = item.PhaiDanhGia;
                preview.LechTrungBinh = item.LechTrungBinh;
                preview.ChenhTraiPhai = item.ChenhTraiPhai;
                preview.Nguong = $"{item.NguongDuoi:0.##} - {item.NguongTren:0.##}";
            }

            foreach (var item in ca.ChanDoanTuDongs.OrderBy(x => x.Nhom))
            {
                PhieuKhamChanDoanPreview preview = objectSpace.CreateObject<PhieuKhamChanDoanPreview>();

                preview.PhieuKham = phieu;
                preview.Nhom = item.Nhom;
                preview.KetQua = item.KetQua;
            }

            foreach (var item in ca.NguTaTuDongs.OrderBy(x => x.Nhom))
            {
                PhieuKhamNguTaPreview preview = objectSpace.CreateObject<PhieuKhamNguTaPreview>();

                preview.PhieuKham = phieu;
                preview.Nhom = item.Nhom;
                preview.KetQua = item.KetQua;
            }

            foreach (var item in ca.BenhChungTuDongs.OrderBy(x => x.TenBenhChung))
            {
                PhieuKhamBenhChungPreview preview = objectSpace.CreateObject<PhieuKhamBenhChungPreview>();

                preview.PhieuKham = phieu;
                preview.TenBenhChung = item.TenBenhChung;
                preview.TrieuChung = item.TrieuChung;
            }

            foreach (var item in ca.PhapTriTuDongs.OrderBy(x => x.BenhChung))
            {
                PhieuKhamPhapTriPreview preview = objectSpace.CreateObject<PhieuKhamPhapTriPreview>();

                preview.PhieuKham = phieu;
                preview.BenhChung = item.BenhChung;
                preview.PhapDieuTri = item.PhapDieuTri;
                preview.PhuongDuoc = item.PhuongDuoc;
                preview.Huyet = item.Huyet;
            }
        }

        private static string TaoTenFileAnToan(string fileName)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '_');
            }

            return fileName;
        }
    }
}
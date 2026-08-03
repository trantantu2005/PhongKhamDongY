using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using DevExpress.ExpressApp;
using ybaso.Module.BusinessObjects;

namespace ybaso.Module.DatabaseUpdate
{
    public class DongYExcelImporter
    {
        private readonly IObjectSpace objectSpace;

        public DongYExcelImporter(IObjectSpace objectSpace)
        {
            this.objectSpace = objectSpace;
        }

        public void Import(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            using XLWorkbook workbook = new XLWorkbook(filePath);

            ImportDanhMucHuyet();
            objectSpace.CommitChanges();

            if (workbook.Worksheets.TryGetWorksheet("Bệnh danh", out IXLWorksheet benhDanhSheet))
            {
                ImportBenhDanhVaPhacDo(benhDanhSheet);
                objectSpace.CommitChanges();
            }

            if (workbook.Worksheets.TryGetWorksheet("Công thức", out IXLWorksheet congThucSheet))
            {
                ImportCongThuc(congThucSheet);
                objectSpace.CommitChanges();
            }
        }

        private void ImportDanhMucHuyet()
        {
            var data = new[]
            {
                new { ThuTu = 1, Ma = "TIEU", Ten = "Tiểu", Nhom = NhomChiDo.ChiTren },
                new { ThuTu = 2, Ma = "TAM", Ten = "Tâm", Nhom = NhomChiDo.ChiTren },
                new { ThuTu = 3, Ma = "TAM_BAO", Ten = "Tam", Nhom = NhomChiDo.ChiTren },
                new { ThuTu = 4, Ma = "BAO", Ten = "Bào", Nhom = NhomChiDo.ChiTren },
                new { ThuTu = 5, Ma = "DAI", Ten = "Đại", Nhom = NhomChiDo.ChiTren },
                new { ThuTu = 6, Ma = "PHE", Ten = "Phế", Nhom = NhomChiDo.ChiTren },

                new { ThuTu = 7, Ma = "BANG", Ten = "Bàng", Nhom = NhomChiDo.ChiDuoi },
                new { ThuTu = 8, Ma = "THAN", Ten = "Thận", Nhom = NhomChiDo.ChiDuoi },
                new { ThuTu = 9, Ma = "DAM", Ten = "Đảm", Nhom = NhomChiDo.ChiDuoi },
                new { ThuTu = 10, Ma = "VI", Ten = "Vị", Nhom = NhomChiDo.ChiDuoi },
                new { ThuTu = 11, Ma = "CAN", Ten = "Can", Nhom = NhomChiDo.ChiDuoi },
                new { ThuTu = 12, Ma = "TY", Ten = "Tỳ", Nhom = NhomChiDo.ChiDuoi }
            };

            foreach (var item in data)
            {
                DanhMucHuyet huyet = TimHuyetTheoMa(item.Ma);

                if (huyet == null)
                    huyet = objectSpace.CreateObject<DanhMucHuyet>();

                huyet.ThuTu = item.ThuTu;
                huyet.MaHuyet = item.Ma;
                huyet.TenHuyet = item.Ten;
                huyet.NhomChi = item.Nhom;
                huyet.Nhom = item.Nhom == NhomChiDo.ChiTren ? "Chi trên" : "Chi dưới";
                huyet.SuDung = true;
            }
        }

        private void ImportBenhDanhVaPhacDo(IXLWorksheet sheet)
        {
            int lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;

            for (int row = 3; row <= lastRow; row++)
            {
                int stt = GetInt(sheet.Cell(row, 1));
                string tenBenh = GetText(sheet.Cell(row, 2));

                if (string.IsNullOrWhiteSpace(tenBenh))
                    continue;

                string ma = TaoMaBenh(stt, tenBenh);

                Benhdanh benh = TimBenhDanhTheoMa(ma);

                if (benh == null)
                    benh = objectSpace.CreateObject<Benhdanh>();

                benh.Stt = stt;
                benh.Ma = ma;
                benh.Tenbenh = tenBenh;
                benh.Trieuchung = GetText(sheet.Cell(row, 3));
                benh.Benhcanh = GetText(sheet.Cell(row, 4));
                benh.Phapdieutri = GetText(sheet.Cell(row, 5));
                benh.Phuongduoc = GetText(sheet.Cell(row, 6));
                benh.Phuonghuyet = GetText(sheet.Cell(row, 8));
                benh.SuDung = true;

                PhacDoDieuTri phacDo = TimPhacDoTheoBenh(benh);

                if (phacDo == null)
                    phacDo = objectSpace.CreateObject<PhacDoDieuTri>();

                phacDo.Benh = benh;
                phacDo.PhapDieuTri = benh.Phapdieutri;
                phacDo.PhuongDuoc = benh.Phuongduoc;
                phacDo.PhuongHuyet = benh.Phuonghuyet;
                phacDo.SuDung = true;
            }
        }

        private void ImportCongThuc(IXLWorksheet sheet)
        {
            int lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;

            for (int row = 3; row <= lastRow; row++)
            {
                int stt = GetInt(sheet.Cell(row, 1));
                string tenBenh = GetText(sheet.Cell(row, 2));

                if (string.IsNullOrWhiteSpace(tenBenh))
                    continue;

                string ma = TaoMaBenh(stt, tenBenh);

                Benhdanh benh = TimBenhDanhTheoMa(ma);

                if (benh == null)
                    benh = objectSpace.CreateObject<Benhdanh>();

                benh.Stt = stt;
                benh.Ma = ma;
                benh.Tenbenh = tenBenh;
                benh.LyChung = GetText(sheet.Cell(row, 3));
                benh.BieuChung = GetText(sheet.Cell(row, 4));
                benh.SuDung = true;

                foreach (Dieukien dkCu in benh.DieuKiens.ToList())
                {
                    objectSpace.Delete(dkCu);
                }

                int thuTuDieuKien = 1;

                for (int col = 5; col <= 16; col++)
                {
                    string tenHuyet = GetText(sheet.Cell(2, col));
                    string congThuc = GetText(sheet.Cell(row, col));

                    if (string.IsNullOrWhiteSpace(tenHuyet) || string.IsNullOrWhiteSpace(congThuc))
                        continue;

                    DanhMucHuyet huyet = TimHuyetTheoTenHoacMa(tenHuyet);

                    if (huyet == null)
                        continue;

                    List<string> danhSachDieuKien = TachDieuKien(congThuc);

                    foreach (string dkText in danhSachDieuKien)
                    {
                        Dieukien dk = objectSpace.CreateObject<Dieukien>();

                        dk.Ben = benh;
                        dk.Huyet = huyet;
                        dk.ThuTu = thuTuDieuKien++;
                        dk.NhomDieuKien = 1;
                        dk.BatBuoc = true;
                        dk.Kieugiatri = KieuGiaTriDieuKien.LechTrungBinh;
                        dk.CongThucGoc = congThuc;
                        dk.DienGiai = dkText;

                        GanPhepSoSanh(dk, dkText);
                    }
                }
            }
        }

        private Benhdanh TimBenhDanhTheoMa(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma))
                return null;

            ma = ma.Trim();

            Benhdanh pending = objectSpace.ModifiedObjects
                .OfType<Benhdanh>()
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Ma) && x.Ma.Trim() == ma);

            if (pending != null)
                return pending;

            return objectSpace.GetObjectsQuery<Benhdanh>()
                .FirstOrDefault(x => x.Ma == ma);
        }

        private DanhMucHuyet TimHuyetTheoMa(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma))
                return null;

            ma = ma.Trim();

            DanhMucHuyet pending = objectSpace.ModifiedObjects
                .OfType<DanhMucHuyet>()
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.MaHuyet) && x.MaHuyet.Trim() == ma);

            if (pending != null)
                return pending;

            return objectSpace.GetObjectsQuery<DanhMucHuyet>()
                .FirstOrDefault(x => x.MaHuyet == ma);
        }

        private DanhMucHuyet TimHuyetTheoTenHoacMa(string tenHoacMa)
        {
            if (string.IsNullOrWhiteSpace(tenHoacMa))
                return null;

            string value = tenHoacMa.Trim();

            DanhMucHuyet pending = objectSpace.ModifiedObjects
                .OfType<DanhMucHuyet>()
                .FirstOrDefault(x =>
                    (!string.IsNullOrWhiteSpace(x.TenHuyet) && x.TenHuyet.Trim() == value) ||
                    (!string.IsNullOrWhiteSpace(x.MaHuyet) && x.MaHuyet.Trim() == value)
                );

            if (pending != null)
                return pending;

            return objectSpace.GetObjectsQuery<DanhMucHuyet>()
                .FirstOrDefault(x => x.TenHuyet == value || x.MaHuyet == value);
        }

        private PhacDoDieuTri TimPhacDoTheoBenh(Benhdanh benh)
        {
            if (benh == null)
                return null;

            PhacDoDieuTri pending = objectSpace.ModifiedObjects
                .OfType<PhacDoDieuTri>()
                .FirstOrDefault(x => x.Benh == benh && x.Ca == null);

            if (pending != null)
                return pending;

            return benh.PhacDoDieuTris.FirstOrDefault(x => x.Ca == null);
        }

        private static string TaoMaBenh(int stt, string tenBenh)
        {
            if (stt > 0)
                return stt.ToString(CultureInfo.InvariantCulture);

            return (tenBenh ?? string.Empty).Trim();
        }

        private static List<string> TachDieuKien(string congThuc)
        {
            return congThuc
                .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }

        private static void GanPhepSoSanh(Dieukien dk, string text)
        {
            string raw = text.Trim();
            string compact = raw.Replace(" ", "");

            bool laAbs = compact.Contains("abs", StringComparison.OrdinalIgnoreCase);

            string op = TimToanTu(compact);

            if (string.IsNullOrWhiteSpace(op))
            {
                dk.Sosanh = "=";
                dk.Giatri = compact;
                dk.GiaTriSo = ParseDoubleSafe(compact);
                return;
            }

            int index = compact.IndexOf(op, StringComparison.Ordinal);
            string value = index >= 0 ? compact.Substring(index + op.Length) : "0";

            dk.Sosanh = laAbs ? "abs" + op : op;
            dk.Giatri = value;
            dk.GiaTriSo = ParseDoubleSafe(value);
        }

        private static string TimToanTu(string text)
        {
            string[] operators = { ">=", "<=", "!=", "<>", ">", "<", "=" };

            foreach (string op in operators)
            {
                if (text.Contains(op))
                    return op;
            }

            return string.Empty;
        }

        private static int GetInt(IXLCell cell)
        {
            if (cell.TryGetValue<int>(out int value))
                return value;

            int.TryParse(GetText(cell), out value);
            return value;
        }

        private static string GetText(IXLCell cell)
        {
            if (cell == null)
                return string.Empty;

            return cell.GetValue<string>()?.Trim() ?? string.Empty;
        }

        private static double ParseDoubleSafe(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            string clean = text
                .Replace("(", "")
                .Replace(")", "")
                .Replace("'", "")
                .Trim();

            if (double.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out double valueInvariant))
                return valueInvariant;

            if (double.TryParse(clean, NumberStyles.Any, CultureInfo.CurrentCulture, out double valueCurrent))
                return valueCurrent;

            return 0;
        }
    }
}
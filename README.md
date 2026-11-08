# PHÒNG KHÁM ĐÔNG Y – HỆ THỐNG QUẢN LÝ KHÁM CHỮA BỆNH

**Đồ án phát triển ứng dụng web – Hệ thống thông tin quản lý (MIS)**

**Công nghệ:** C# | .NET 8 | ASP.NET Core Blazor Server | DevExpress XAF/XPO | SQL Server

---

## 1. Giới thiệu dự án

**Phòng Khám Đông Y (Y Bạ Số)** là hệ thống quản lý khám chữa bệnh được phát triển trên nền tảng web, nhằm hỗ trợ số hóa các hoạt động quản lý tại phòng khám, đặc biệt là những nghiệp vụ liên quan đến y học cổ truyền.

Hệ thống được xây dựng với mục tiêu tổ chức và quản lý tập trung thông tin bệnh nhân, quá trình khám chữa bệnh, đơn thuốc, kho dược và các danh mục chuyên môn trên một nền tảng thống nhất.

Thông qua việc ứng dụng công nghệ thông tin, dự án hướng đến việc hỗ trợ nhân viên y tế trong công tác tra cứu, cập nhật và theo dõi thông tin, đồng thời góp phần nâng cao hiệu quả quản lý và vận hành phòng khám.

Dự án được thực hiện theo hình thức đồ án nhóm, phục vụ mục đích học tập, nghiên cứu và vận dụng kiến thức về lập trình, cơ sở dữ liệu và phát triển hệ thống thông tin quản lý.

## 2. Mục tiêu dự án

- Xây dựng hệ thống quản lý thông tin bệnh nhân và hồ sơ khám chữa bệnh.
- Hỗ trợ bác sĩ quản lý ca khám, chẩn đoán, điều trị và kê đơn thuốc.
- Số hóa hoạt động quản lý thuốc, dược liệu và kho của phòng khám.
- Tổ chức dữ liệu chuyên môn liên quan đến y học cổ truyền.
- Hỗ trợ tra cứu, thống kê và quản lý thông tin nghiệp vụ.
- Phân quyền người dùng phù hợp với từng vai trò trong hệ thống.
- Vận dụng kiến thức lập trình web, thiết kế dữ liệu và xử lý nghiệp vụ vào bài toán quản lý thực tế.

## 3. Các chức năng chính

### 3.1. Quản lý bệnh nhân

- Quản lý hồ sơ và thông tin cá nhân của bệnh nhân.
- Hỗ trợ tra cứu, cập nhật thông tin bệnh nhân.
- Theo dõi lịch sử khám chữa bệnh.
- Hiển thị thông tin chi tiết các lần khám và điều trị.
- Hỗ trợ tổ chức dữ liệu bệnh nhân phục vụ quá trình khám chữa bệnh.

### 3.2. Quản lý khám chữa bệnh

- Quản lý danh sách và thông tin các ca khám.
- Ghi nhận thông tin khám bệnh, chẩn đoán và điều trị.
- Theo dõi quá trình khám chữa bệnh của từng bệnh nhân.
- Hỗ trợ bác sĩ thao tác với dữ liệu khám chữa bệnh.
- Liên kết thông tin ca khám với bệnh nhân và dữ liệu điều trị liên quan.

### 3.3. Quản lý đơn thuốc

- Tạo và quản lý thông tin đơn thuốc.
- Ghi nhận các loại thuốc được kê trong quá trình điều trị.
- Tra cứu danh sách và thông tin chi tiết đơn thuốc.
- Liên kết thông tin đơn thuốc với hồ sơ khám chữa bệnh.
- Hỗ trợ theo dõi dữ liệu liên quan đến quá trình sử dụng thuốc.

### 3.4. Quản lý thuốc, dược liệu và kho

- Quản lý danh mục thuốc và dược liệu.
- Theo dõi thông tin nhập và xuất kho.
- Quản lý các phiếu nhập kho, xuất kho.
- Theo dõi số lượng tồn kho.
- Quản lý thông tin đơn vị tính và dữ liệu liên quan.
- Hỗ trợ quản lý hoạt động sử dụng thuốc và dược liệu tại phòng khám.

### 3.5. Quản lý dữ liệu y học cổ truyền

- Quản lý danh mục huyệt.
- Quản lý thông tin bệnh danh.
- Tổ chức dữ liệu phác đồ điều trị.
- Quản lý dữ liệu ngũ tạng và các danh mục chuyên môn liên quan.
- Hỗ trợ tra cứu dữ liệu phục vụ nghiệp vụ khám chữa bệnh bằng y học cổ truyền.

### 3.6. Báo cáo và thống kê

- Tổ chức dữ liệu phục vụ báo cáo hoạt động phòng khám.
- Hỗ trợ tổng hợp thông tin doanh thu.
- Quản lý dữ liệu thống kê theo loại khám.
- Theo dõi thông tin tồn kho thuốc và dược liệu.
- Hỗ trợ người quản lý tra cứu các dữ liệu nghiệp vụ cần thiết.

### 3.7. Quản lý tài khoản và phân quyền

Hệ thống tổ chức giao diện và chức năng phù hợp với các nhóm người dùng:

- **Quản trị viên:** Quản lý và cấu hình hệ thống theo quyền được cấp.
- **Bác sĩ:** Truy cập các chức năng hỗ trợ khám chữa bệnh và quản lý thông tin điều trị.
- **Nhân viên:** Thực hiện các nghiệp vụ quản lý và hỗ trợ vận hành phòng khám.
- **Bệnh nhân:** Truy cập những thông tin và chức năng dành cho người bệnh.

Việc kiểm soát quyền truy cập được xây dựng dựa trên các thành phần bảo mật của DevExpress XAF.

*Các chức năng được mô tả dựa trên mã nguồn dự án. Chưa xác nhận toàn bộ luồng nghiệp vụ đã hoạt động hoàn chỉnh trên môi trường triển khai mới.*

## 4. Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Ngôn ngữ lập trình | C# |
| Framework | .NET 8 |
| Nền tảng ứng dụng web | ASP.NET Core Blazor Server |
| Framework nghiệp vụ | DevExpress XAF 26.1 |
| Truy cập và quản lý dữ liệu | DevExpress XPO |
| Cơ sở dữ liệu | Microsoft SQL Server / LocalDB |
| Phân quyền và bảo mật | DevExpress XAF Security |
| Xử lý dữ liệu Excel | ClosedXML |
| Quản lý mã nguồn | Git, GitHub |

## 5. Kiến trúc và cấu trúc dự án

Hệ thống được tổ chức thành hai project chính, giúp phân tách phần ứng dụng web và phần xử lý dữ liệu, nghiệp vụ.

### 5.1. ybaso.Blazor.Server

Đảm nhiệm phần ứng dụng web và tương tác với người dùng, bao gồm:

- Giao diện được phát triển bằng ASP.NET Core Blazor.
- Các trang chức năng dành cho từng nhóm người dùng.
- Thành phần xử lý yêu cầu từ giao diện.
- Cấu hình ứng dụng và kết nối với các thành phần nghiệp vụ.

### 5.2. ybaso.Module

Đảm nhiệm các thành phần nghiệp vụ và dữ liệu, bao gồm:

- **Business Objects:** Các đối tượng dữ liệu nghiệp vụ.
- **Controllers:** Thành phần điều khiển và xử lý nghiệp vụ.
- **DatabaseUpdate:** Các thành phần phục vụ cập nhật dữ liệu.
- Các chức năng hỗ trợ tổ chức và quản lý dữ liệu của hệ thống.

### 5.3. Cấu trúc mã nguồn

```text
ybaso/
├── ybaso.slnx
└── ybaso/
    ├── ybaso.Blazor.Server/
    │   ├── Components/
    │   ├── Services/
    │   ├── wwwroot/
    │   ├── Program.cs
    │   ├── appsettings.json
    │   └── ybaso.Blazor.Server.csproj
    │
    └── ybaso.Module/
        ├── BusinessObjects/
        ├── Controllers/
        ├── DatabaseUpdate/
        └── ybaso.Module.csproj
```

## 6. Vai trò và đóng góp cá nhân

**Hình thức thực hiện:** Đồ án nhóm – Phát triển hệ thống quản lý phòng khám.

**Vai trò:** Thành viên phát triển hệ thống, tham gia xây dựng giao diện, xử lý nghiệp vụ, làm việc với dữ liệu và hoàn thiện các chức năng của ứng dụng.

### 6.1. Phát triển giao diện ứng dụng

- Tham gia xây dựng và hoàn thiện giao diện hệ thống bằng ASP.NET Core Blazor.
- Phát triển các thành phần giao diện phục vụ hoạt động của bác sĩ, nhân viên và bệnh nhân.
- Tổ chức bố cục, điều hướng và các thao tác người dùng phù hợp với từng nhóm chức năng.
- Điều chỉnh giao diện nhằm nâng cao tính trực quan và thuận tiện trong quá trình sử dụng.

### 6.2. Phát triển chức năng quản lý bệnh nhân và khám chữa bệnh

- Tham gia phát triển các chức năng quản lý hồ sơ và thông tin bệnh nhân.
- Xử lý các nghiệp vụ liên quan đến ca khám, lịch sử khám chữa bệnh và thông tin điều trị.
- Kết nối dữ liệu bệnh nhân với các thành phần nghiệp vụ liên quan.
- Hoàn thiện các thao tác tra cứu, hiển thị và cập nhật thông tin trong hệ thống.

### 6.3. Phát triển chức năng quản lý đơn thuốc và kho dược

- Tham gia xây dựng các chức năng quản lý đơn thuốc và danh mục thuốc, dược liệu.
- Xử lý và tổ chức dữ liệu liên quan đến nhập kho, xuất kho và tồn kho.
- Hỗ trợ kết nối thông tin thuốc với các nghiệp vụ khám chữa bệnh.
- Kiểm tra tính nhất quán của dữ liệu trong các thao tác nghiệp vụ liên quan.

### 6.4. Xử lý nghiệp vụ và làm việc với cơ sở dữ liệu

- Làm việc với các Business Objects và Controllers trong DevExpress XAF.
- Sử dụng DevExpress XPO để tổ chức và truy cập dữ liệu nghiệp vụ.
- Tìm hiểu và xử lý mối liên hệ giữa các đối tượng dữ liệu trong hệ thống.
- Phối hợp kết nối giao diện ứng dụng với các thành phần xử lý nghiệp vụ và cơ sở dữ liệu.

### 6.5. Tích hợp, kiểm thử và hoàn thiện hệ thống

- Tham gia tích hợp các chức năng vào hệ thống chung.
- Kiểm tra hoạt động của giao diện và các luồng nghiệp vụ trong quá trình phát triển.
- Phát hiện và phối hợp xử lý những vấn đề phát sinh.
- Điều chỉnh và hoàn thiện chức năng theo yêu cầu của đồ án.
- Phối hợp với các thành viên trong nhóm để hoàn thiện sản phẩm.

### 6.6. Công nghệ và kỹ năng tích lũy

- **Lập trình:** C#, .NET 8, ASP.NET Core Blazor.
- **Phát triển ứng dụng:** DevExpress XAF, XPO.
- **Cơ sở dữ liệu:** SQL Server, quản lý dữ liệu quan hệ.
- **Phân tích hệ thống:** Tìm hiểu và xử lý quy trình nghiệp vụ.
- **Kiểm thử:** Kiểm tra chức năng và xử lý lỗi.
- **Kỹ năng làm việc:** Giải quyết vấn đề, phối hợp nhóm và tích hợp phần mềm.

## 7. Hướng dẫn cài đặt và chạy dự án

### 7.1. Yêu cầu môi trường

- .NET 8 SDK.
- Microsoft SQL Server hoặc SQL Server Express LocalDB.
- DevExpress XAF 26.1 và quyền truy cập các gói NuGet hợp lệ.
- PowerShell hoặc môi trường phát triển .NET tương thích.

### 7.2. Khôi phục thư viện và biên dịch

Mở Terminal tại thư mục chứa file `ybaso.slnx`.

**Bước 1 – Kiểm tra .NET SDK**

```powershell
dotnet --version
```

**Bước 2 – Khôi phục các thư viện**

```powershell
dotnet restore .\ybaso.slnx
```

**Bước 3 – Biên dịch dự án**

```powershell
dotnet build .\ybaso.slnx -c Debug
```

**Bước 4 – Khởi chạy ứng dụng**

```powershell
dotnet run --project .\ybaso\ybaso.Blazor.Server\ybaso.Blazor.Server.csproj --launch-profile "ybaso.Blazor.Server"
```

Cần cấu hình kết nối SQL Server và các thông số ứng dụng phù hợp trước khi khởi chạy.

Theo cấu hình phát triển, ứng dụng dự kiến sử dụng địa chỉ:

- `http://localhost:5000`
- `https://localhost:5001`

**Lưu ý:** Dự án sử dụng DevExpress XAF có yêu cầu giấy phép riêng. Việc khôi phục thư viện và biên dịch phụ thuộc vào môi trường và quyền sử dụng DevExpress hợp lệ.

## 8. Kết quả và kiến thức tích lũy

Thông qua quá trình tham gia phát triển dự án, các nội dung kiến thức và kỹ năng được tiếp cận bao gồm:

- Hiểu rõ hơn về quá trình phát triển một ứng dụng web quản lý nghiệp vụ.
- Vận dụng kiến thức lập trình C# và nền tảng .NET.
- Tìm hiểu cách tổ chức ứng dụng với ASP.NET Core Blazor và DevExpress XAF.
- Làm việc với các đối tượng dữ liệu và nghiệp vụ trong hệ thống.
- Hiểu mối quan hệ giữa giao diện, xử lý nghiệp vụ và cơ sở dữ liệu.
- Rèn luyện kỹ năng kiểm tra, phát hiện và xử lý vấn đề trong quá trình phát triển.
- Tích lũy kinh nghiệm phối hợp và hoàn thiện sản phẩm phần mềm theo nhóm.

Dự án là cơ hội vận dụng kiến thức ngành Hệ thống thông tin quản lý vào một bài toán thực tế trong lĩnh vực y tế.

## 9. Giới hạn và lưu ý

- Dự án được xây dựng phục vụ mục đích học tập và nghiên cứu, chưa phải phần mềm y tế được kiểm định để triển khai chính thức.
- Một số chức năng cần môi trường .NET, SQL Server và DevExpress phù hợp để hoạt động.
- Chưa xác nhận toàn bộ mã nguồn biên dịch và chạy thành công trong môi trường mới.
- Không công khai thông tin bệnh nhân thật, tài khoản, mật khẩu, khóa bảo mật hoặc dữ liệu nhạy cảm.
- Các thư viện DevExpress phải được sử dụng theo đúng điều kiện giấy phép.
- Repository được Fork từ dự án của thành viên nhóm. Nội dung trình bày nhằm giới thiệu dự án học tập và những đóng góp trong quá trình phát triển.

---

**Phòng Khám Đông Y – Y Bạ Số**

*Đồ án phát triển ứng dụng web ứng dụng công nghệ thông tin trong quản lý khám chữa bệnh.*

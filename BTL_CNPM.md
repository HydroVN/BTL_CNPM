# MÔ TẢ TỔNG THỂ BÀI TOÁN

## Mục tiêu hệ thống

Hệ thống Quản lý các Dự án và Công việc (Platform) là một ứng dụng Web được xây dựng theo kiến trúc MVC, nhằm cung cấp một giải pháp quản trị toàn diện cho các doanh nghiệp và đội nhóm. Hệ thống cho phép tối ưu hóa hiệu suất làm việc thông qua việc số hóa quy trình quản lý dự án, phân công nhiệm vụ, theo dõi tiến độ trực quan và tăng cường khả năng tương tác nội bộ.

## Danh sách chức năng

 **Nhóm chức năng tài khoản & Không gian làm việc (Workspace):** Đăng ký, Đăng nhập, Tạo/Tham gia Không gian làm việc, Quản lý thành viên trong Workspace.

 **Nhóm chức năng quản lý dự án (Spaces/Lists):** Tạo không gian dự án, Thiết lập quy trình làm việc (Workflow/Status), Phân quyền dự án.

 **Nhóm chức năng quản lý công việc (Tasks):** Tạo công việc, Phân công người thực hiện (Assignee), Đặt thời hạn (Due date), Thiết lập mức độ ưu tiên (Priority), Đính kèm tệp tin.

 **Nhóm chức năng hiển thị trực quan (Views):** Xem dưới dạng Danh sách (List View), dạng bảng Kanban (Board View), dạng Lịch (Calendar View).

 **Nhóm chức năng tương tác (Collaboration):** Bình luận trong công việc (Task Comments), Gửi thông báo hệ thống (Notifications).

 **Nhóm chức năng báo cáo & Dashboard:** Thống kê tiến độ công việc, Theo dõi hiệu suất thành viên (Burndown chart).

 **Nhóm chức năng công khai (Dành cho Khách vãng lai):** Xem giới thiệu tính năng, Xem bảng giá các gói dịch vụ (Pricing), Đăng ký tài khoản dùng thử (Free Trial).

## Danh sách các Stakeholder & các loại người dùng

| **STT** | **Lớp stakeholder** | **Mô tả** |
| --- | --- | --- |
| 1 | Quản trị viên (Admin/Owner) | Chủ sở hữu Không gian làm việc, có quyền quản lý tối cao về cấu hình hệ thống, quản lý thành viên và thanh toán gói dịch vụ. |
| 2 | Quản lý dự án (Project Manager) | Khởi tạo dự án, thiết lập Workflow, phân công và kiểm soát thời hạn của các đầu việc. |
| 3 | Thành viên (Member/Employee) | Thực hiện các công việc được giao, cập nhật trạng thái nhiệm vụ, bình luận và báo cáo tiến độ. |
| 4 | Khách vãng lai (Guest/Visitor) | Người dùng bên ngoài truy cập trang chủ công khai để tìm hiểu công cụ, xem bảng giá và đăng ký trải nghiệm. |
| 5 | Hệ thống phần mềm | Thành phần xử lý logic, lưu trữ trạng thái công việc, tính toán tiến độ và hiển thị các góc nhìn dữ liệu khác nhau. |

# ĐẶC TẢ YÊU CẦU HỆ THỐNG

## Các tác nhân

 **Quản trị viên (Admin/Workspace Owner):** Quản lý cài đặt Workspace, mời hoặc kích người dùng ra khỏi không gian làm việc, thiết lập các tùy chỉnh nâng cao.

 **Quản lý dự án (Project Manager):** Tạo dự án mới, định nghĩa các trạng thái công việc (To Do, In Progress, Review, Done), phân công nhiệm vụ.

 **Thành viên (Member):** Tiếp nhận công việc, kéo thả trạng thái công việc trên bảng Kanban, cập nhật tiến độ, thảo luận trực tiếp trên Task.

 **Khách vãng lai (Guest):** Duyệt xem giao diện công khai để tham khảo tính năng phần mềm, xem biểu phí dịch vụ và điền form đăng ký sử dụng.

 **Hệ thống phần mềm:** Xử lý các thao tác kéo thả thời gian thực, lưu trữ dữ liệu công việc, gửi thông báo khi có thay đổi trên Task.

## Các chức năng của hệ thống

### Chức năng Đăng nhập

a. Tên chức năng: Đăng nhập hệ thống

b. Đường dẫn: Trang chủ >> Điều hướng > Đăng nhập

c. Mô tả chức năng (Brief description): Cho phép người dùng truy cập hệ thống bằng tài khoản đã được cấp.

d. Dòng sự kiện chính (Basic Flow)

1. Người dùng truy cập form đăng nhập bao gồm các trường: Tên đăng nhập (Email), Mật khẩu và nút "Đăng nhập".
2. Người dùng nhập đầy đủ thông tin tài khoản và nhấn nút "Đăng nhập".
3. Hệ thống tiến hành mã hóa mật khẩu, đối chiếu và kiểm tra tính hợp lệ của tài khoản trong cơ sở dữ liệu.
4. Nếu thông tin chính xác: Hệ thống xác thực quyền truy cập, chuyển hướng người dùng đến trang Dashboard tương ứng với vai trò của họ.

e. Dòng sự kiện phụ (Alternative Flow) (màn hình phụ, trường hợp ngoại lệ)

- Trường hợp dữ liệu trống: Người dùng không nhập tên đăng nhập hoặc mật khẩu và nhấn nút, hệ thống hiển thị thông báo lỗi: "Tên đăng nhập và mật khẩu không để trống"

- Trường hợp sai thông tin: Người dùng nhập sai tên đăng nhập hoặc mật khẩu, hệ thống hiển thị thông báo: "Tên đăng nhập hoặc mật khẩu không đúng"".

### Chức năng Đăng ký

a. Tên chức năng: Đăng ký hệ thống

b. Đường dẫn: Trang chủ >> Điều hướng >> Đăng ký

c. Mô tả chức năng (Brief description): Cho phép nhân sự mới điền thông tin để đăng ký tài khoản thành viên trên hệ thống.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng chọn chức năng Đăng ký trên màn hình.
2. Hệ thống hiển thị form đăng ký gồm các trường thông tin: Họ tên (Full name), Số điện thoại (Mobile number), Mật khẩu (Password).
3. Người dùng nhập đầy đủ thông tin và click chọn nút "Đăng ký".
4. Hệ thống kiểm tra tính hợp lệ và duy nhất của thông tin.
5. Hệ thống thực hiện băm mật khẩu (Hash) để đảm bảo an toàn, sau đó lưu thông tin tài khoản mới vào cơ sở dữ liệu.
6. Hệ thống kiểm tra dữ liệu, lưu thông tin tài khoản mới vào cơ sở dữ liệu và hiển thị thông báo đăng ký thành công.

e. Dòng sự kiện phụ (Alternative Flow):

- Trường hợp bỏ trống: Người dùng bỏ trống họ tên, số điện thoại hoặc mật khẩu, hệ thống thông báo: "Họ tên, số điện thoại, mật khẩu không được để trống".

### Chức năng Đăng xuất

a. Tên chức năng: Đăng xuất hệ thống.

b. Đường dẫn: Góc trên bên phải màn hình >> Menu tài khoản >> Đăng xuất

c. Mô tả chức năng (Brief description): Hủy phiên làm việc hiện tại của người dùng và đưa họ trở về trạng thái chưa đăng nhập nhằm đảm bảo an toàn thông tin.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng click vào biểu tượng tài khoản cá nhân và chọn mục "Đăng xuất".
2. Hệ thống thực hiện xóa session/cookie làm việc hiện tại của tài khoản.
3. Hệ thống điều hướng người dùng quay trở lại trang Đăng nhập hệ thống.

e. Dòng sự kiện phụ (Alternative Flow): Không có

### Chức năng Đổi mật khẩu

a. Tên chức năng: Đổi mật khẩu

b. Đường dẫn: Trang chủ >> Quản lý thông tin cá nhân >> Đổi mật khẩu

c. Mô tả chức năng (Brief description): Cho phép người dùng tự thay đổi mật khẩu đăng nhập để nâng cao tính bảo mật cho tài khoản cá nhân

d. Dòng sự kiện chính (Basic Flow):

1. Hệ thống hiển thị form đổi mật khẩu bao gồm các trường: Mật khẩu hiện tại, Mật khẩu mới, Xác nhận mật khẩu mới và nút "Cập nhật"

2. Người dùng điền đầy đủ thông tin vào các trường và ấn "Cập nhật".

3. Hệ thống kiểm tra tính chính xác của mật khẩu cũ và sự trùng khớp của mật khẩu mới.

4. Hệ thống cập nhật mật khẩu mới đã được mã hóa vào CSDL và thông báo thành công.

e. Dòng sự kiện phụ (Alternative Flow):

- Mật khẩu cũ không chính xác: Hệ thống thông báo "Mật khẩu hiện tại không đúng".

- Mật khẩu mới không trùng khớp: Hệ thống thông báo "Mật khẩu mới và mật khẩu xác nhận không khớp".

### Chức năng Quản lý thông tin cá nhân

a. Tên chức năng: Quản lý thông tin cá nhân.

b. Đường dẫn: Trang chủ >> Điều hướng >> Hồ sơ cá nhân.

c. Mô tả chức năng (Brief description): Cho phép người dùng tự xem và cập nhật một số thông tin cơ bản của chính mình (Họ tên, Số điện thoại, Email).

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng click vào biểu tượng tài khoản cá nhân và chọn mục "Đăng xuất".
2. Người dùng chỉnh sửa các trường thông tin cho phép (Ví dụ: Số điện thoại, địa chỉ) và nhấn nút "Lưu thay đổi".
3. Hệ thống kiểm tra định dạng dữ liệu (Số điện thoại hợp lệ, Email đúng cấu trúc), tiến hành ghi đè dữ liệu mới vào bảng thông tin và thông báo cập nhật thành công.

e. Dòng sự kiện phụ (Alternative Flow):

- Thông tin không hợp lệ: Hệ thống thông báo lỗi và yêu cầu sửa lại dữ liệu (ví dụ: Số điện thoại chứa ký tự chữ).

### Chức năng Thêm dự án

a. Tên chức năng: Thêm dự án mới.

b. Đường dẫn: Dashboard >> Quản lý dự án >> Thêm mới.

c. Mô tả chức năng (Brief description): Cho phép Quản trị viên hoặc Quản lý dự án khởi tạo một dự án mới trong hệ thống.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng click vào biểu tượng tài khoản cá nhân và chọn mục "Đăng xuất".
2. Người dùng nhập đầy đủ các thông tin bắt buộc gồm: Tên dự án/Không gian, Mô tả, Quy trình trạng thái (Workflow), Ngày bắt đầu, Ngày kết thúc dự kiến và chọn nút "Lưu dự án".
3. Hệ thống thực hiện kiểm tra tính hợp lệ (Ngày kết thúc phải sau ngày bắt đầu).
4. Hệ thống thêm dự án vào cơ sở dữ liệu với trạng thái mặc định ban đầu là "Chưa thực hiện" (màu xanh) và thông báo thành công.

e. Dòng sự kiện phụ (Alternative Flow):

- Thiếu thông tin bắt buộc: Hệ thống hiển thị thông báo lỗi: "Vui lòng điền đầy đủ các thông tin có dấu ()"\*.

- Sai logic thời gian: Hệ thống thông báo: "Ngày kết thúc dự án không thể trước ngày bắt đầu".

### Chức năng Sửa dự án

a. Tên chức năng: Chỉnh sửa thông tin dự án.

b. Đường dẫn: Dashboard >> Quản lý dự án >> Danh sách dự án >> Nút [Sửa] của dự án tương ứng.

c. Mô tả chức năng (Brief description): Cho phép sửa đổi thông tin cấu hình hoặc cập nhật trạng thái hoạt động của một dự án hiện hành.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng chọn một dự án trong danh sách và bấm nút "Sửa".
2. Form "Cập nhật dự án" xuất hiện hiển thị đầy đủ dữ liệu cũ của dự án (trừ mã dự án không được sửa).
3. Người dùng thực hiện chỉnh sửa thông tin hoặc cập nhật lại Trạng thái (Chuyển sang "Đang thực hiện" - hiển thị màu vàng, hoặc "Đã hoàn thành" - hiển thị màu đỏ).
4. Người dùng nhấn nút "Lưu" để đồng bộ dữ liệu vào hệ thống.

e. Dòng sự kiện phụ (Alternative Flow):

- Dự án không tồn tại: Nếu dự án vừa chọn bị tác nhân khác xóa trước đó, hệ thống báo lỗi: "Dự án không tồn tại hoặc đã bị xóa".

### Chức năng Xóa dự án

a. Tên chức năng: Xóa dự án.

b. Đường dẫn: Cho phép gỡ bỏ một dự án ra khỏi danh sách hiển thị trên hệ thống.

c. Mô tả chức năng (Brief description): Cho phép sửa đổi thông tin hoặc cập nhật trạng thái hoạt động của một dự án hiện hành.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng bấm vào nút "Xóa" tại dòng chứa thông tin dự án cần loại bỏ.
2. Hệ thống hiển thị hộp thoại cảnh báo: *"Bạn có chắc chắn muốn xóa dự án này? Hành động này sẽ xóa toàn bộ công việc liên quan.*"
3. Người dùng bấm nút "Xác nhận xóa".
4. Hệ thống thực hiện xóa dự án (hoặc chuyển trạng thái lưu trữ ẩn), cập nhật lại giao diện bảng danh sách dự án và thông báo xóa thành công.

e. Dòng sự kiện phụ (Alternative Flow):

- Hủy bỏ thao tác: Người dùng nhấn nút "Hủy", hệ thống đóng hộp thoại và giữ nguyên hiện trạng dự án.

### Chức năng Tìm kiếm dự án

a. Tên chức năng: Tìm kiếm dự án.

b. Đường dẫn: Dashboard >> Quản lý dự án >> Thanh tìm kiếm.

c. Mô tả chức năng (Brief description): Hỗ trợ người dùng tìm nhanh các dự án theo từ khóa (Tên dự án, mã dự án) hoặc bộ lọc (Theo trạng thái màu sắc, theo Quản lý phụ trách).

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng nhập từ khóa tìm kiếm hoặc lựa chọn tiêu chí lọc trên thanh công cụ.
2. Người dùng nhấn phím Enter hoặc click vào biểu tượng kính lúp "Tìm kiếm"*.*
3. Hệ thống quét qua bảng dữ liệu dự án và lọc ra các bản ghi thỏa mãn điều kiện.
4. Kết quả được trả về hiển thị dưới dạng bảng danh sách dự án thu hẹp.

e. Dòng sự kiện phụ (Alternative Flow):

- Không có kết quả: Hệ thống hiển thị thông báo trên màn hình: "Không tìm thấy dự án nào phù hợp với tiêu chí tìm kiếm của bạn".

### Chức năng Xem chi tiết dự án

a. Tên chức năng: Xem chi tiết dự án.

b. Đường dẫn: Dashboard >> Quản lý dự án >> Nút [Xem chi tiết].

c. Mô tả chức năng (Brief description): Hiển thị toàn bộ thông tin chuyên sâu của một dự án cụ thể, bao gồm tiến độ phần trăm tổng thể, danh sách nhân viên tham gia và danh sách các công việc chi tiết đi kèm.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng click vào nút "Xem chi tiết" của một dự án trên giao diện bảng.
2. Hệ thống chuyển hướng tới trang thông tin chuyên sâu của dự án đó*.*
3. Màn hình hiển thị: Thông tin tổng quan (Mô tả, Ngân sách), Biểu đồ tiến độ, Danh sách nhân sự phụ trách dự án, Danh sách công việc chia theo tiến độ.

e. Dòng sự kiện phụ (Alternative Flow): Không có.

### Chức năng Thêm công việc

a. Tên chức năng: Thêm công việc.

b. Đường dẫn: Dashboard >> Xem chi tiết dự án >> Tab Công việc >> [Thêm công việc].

c. Mô tả chức năng (Brief description): Tạo một đầu việc (Task) cụ thể trực thuộc một dự án đang quản lý.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng nhấn nút "Thêm công việc".
2. Hệ thống hiển thị một biểu mẫu yêu cầu nhập: Tên công việc, Mô tả nhiệm vụ, Ngày bắt đầu, Hạn hoàn thành (Deadline), Mức độ ưu tiên (Thấp/Trung bình/Cao) và tệp đính kèm*.*
3. Người dùng nhập đầy đủ thông tin và chọn "Lưu".
4. Hệ thống kiểm tra tính hợp lệ và thêm đầu việc mới vào dự án dưới trạng thái mặc định "To Do".

e. Dòng sự kiện phụ (Alternative Flow):

- Sai thời hạn: Hệ thống báo lỗi nếu hạn hoàn thành công việc vượt quá thời gian cho phép của dự án cha.

### Chức năng Phân công công việc

a. Tên chức năng: Phân công công việc.

b. Đường dẫn: Dashboard >> Quản lý công việc >> Chọn công việc >> [Phân công].

c. Mô tả chức năng (Brief description): Giao đầu việc đã tạo cho một hoặc nhiều thành viên chịu trách nhiệm thực hiện.

d. Dòng sự kiện chính (Basic Flow):

1. Tại màn hình quản lý công việc, người dùng click vào chức năng "Phân công" (Assignee) tại đầu việc tương ứng.
2. Hệ thống hiển thị danh sách thả xuống (Dropdown) chứa toàn bộ thành viên thuộc dự án này*.*
3. Người dùng tích chọn nhân viên chịu trách nhiệm và ấn "Xác nhận".
4. Hệ thống cập nhật trường người phụ trách vào bảng công việc và gửi thông báo hệ thống đến tài khoản của thành viên đó.

e. Dòng sự kiện phụ (Alternative Flow): Không có.

### Chức năng Xóa công việc

a. Tên chức năng: Xóa công việc.

b. Đường dẫn: Dashboard >> Xem chi tiết dự án >> Danh sách công việc >> Nút [Xóa] tại dòng công việc.

c. Mô tả chức năng (Brief description): Loại bỏ một đầu việc không còn cần thiết ra khỏi hệ thống.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng nhấn nút "Xóa" tại đầu việc cần loại bỏ.
2. Hệ thống hiển thị popup yêu cầu xác nhận hành động xóa*.*
3. Người dùng nhấn nút "Đồng ý xóa".
4. Hệ thống thực hiện xóa bản ghi công việc khỏi CSDL và làm mới màn hình.

e. Dòng sự kiện phụ (Alternative Flow):

- Nếu người dùng chọn "Hủy", hệ thống đóng popup và giữ nguyên dữ liệu công việc.

### Chức năng Mời và quản lý thành viên

a. Tên chức năng: Quản lý thành viên.

b. Đường dẫn: Phân hệ Admin >> Quản lý Workspace >> Thành viên.

c. Mô tả chức năng (Brief description): Admin gửi lời mời qua Email để nhân sự tham gia vào hệ thống hoặc chỉnh sửa vai trò, loại bỏ thành viên ra khỏi Workspace.

d. Dòng sự kiện chính (Basic Flow):

1. Admin nhấn nút "Mời thành viên mới", nhập Email của nhân sự và chọn vai trò (Manager/Member).
2. Hệ thống gửi mã liên kết/email xác nhận đến nhân sự. Sau khi họ chấp nhận, hệ thống tự động ghi nhận hồ sơ thành viên hoạt động.

e. Dòng sự kiện phụ (Alternative Flow):

- Email đã tồn tại: Hệ thống báo lỗi nếu email đã nằm trong không gian làm việc này.

### Chức năng Phân công nhân viên tham gia dự án

a. Tên chức năng: Thêm thành viên vào dự án.

b. Đường dẫn: Dashboard >> Chi tiết dự án >> Tab Thành viên >> [Thêm thành viên].

c. Mô tả chức năng (Brief description): Phân quyền cho nhân sự nội bộ được phép truy cập, theo dõi và nhận việc trong một dự án cụ thể.

d. Dòng sự kiện chính (Basic Flow):

1. Quản lý dự án chọn mục "Thêm thành viên vào dự án"
2. Hệ thống hiển thị danh sách nhân sự khả dụng trong Workspace.
3. Người dùng tích chọn nhân viên, phân vai trò trong dự án (Developer, Tester,...) và nhấn "Xác nhận".

e. Dòng sự kiện phụ (Alternative Flow): Không có

### Chức năng Báo cáo tiến độ dự án

a. Tên chức năng: Báo cáo tiến độ dự án.

b. Đường dẫn: Dashboard >> Phân hệ Báo cáo >> Báo cáo dự án.

c. Mô tả chức năng (Brief description): Tổng hợp số liệu phần trăm hoàn thành, số lượng Task chưa làm, đang làm, và đã xong của toàn bộ dự án.

d. Dòng sự kiện chính (Basic Flow):

1. Quản lý dự án chọn mục "Thêm thành viên vào dự án"
2. Hệ thống tự động quét dữ liệu tổng thể và hiển thị báo cáo dạng biểu đồ trực quan, hỗ trợ bộ lọc theo thời gian.

e. Dòng sự kiện phụ (Alternative Flow): Không có

### Chức năng Báo cáo hiệu suất thành viên

a. Tên chức năng: Báo cáo hiệu suất thành viên.

b. Đường dẫn: Dashboard >> Phân hệ Báo cáo >> Hiệu suất (Burndown chart).

c. Mô tả chức năng (Brief description): Thống kê khối lượng công việc, số lượng đầu việc bị trễ hạn và tốc độ xử lý Task của từng thành viên phục vụ đánh giá KPI.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng chọn mục Báo cáo nhân viên trên menu chức năng.
2. Hệ thống tổng hợp dữ liệu từ bảng công việc gắn với từng mã thành viên và xuất ra biểu đồ.

e. Dòng sự kiện phụ (Alternative Flow): Không có

### Chức năng Xuất dữ liệu Excel/PDF

a. Tên chức năng: Xuất Excel/PDF.

b. Đường dẫn: Xuất hiện tại góc trên tất cả các màn hình hiển thị Báo cáo thống kê.

c. Mô tả chức năng (Brief description): Cho phép kết xuất toàn bộ nội dung báo cáo đang hiển thị ra file tài liệu độc lập để lưu trữ hoặc in ấn.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng click chọn nút "Xuất dữ liệu Excel" hoặc "Xuất dữ liệu PDF".
2. Hệ thống tiếp nhận yêu cầu, xử lý chuyển đổi cấu trúc giao diện thành file tài liệu và tiến hành tải xuống thiết bị cục bộ.

e. Dòng sự kiện phụ (Alternative Flow):

- Lỗi xuất file: Hệ thống hiển thị thông báo lỗi: "Xuất file thất bại, vui lòng thử lại sau"

### Chức năng Xem giới thiệu tính năng phần mềm (Dành cho Khách vãng lai)

a. Tên chức năng: Xem tính năng phần mềm.

b. Đường dẫn: Trang chủ công khai (Khi chưa đăng nhập).

c. Mô tả chức năng (Brief description): Cho phép khách vãng lai tìm hiểu các giải pháp quản lý công việc mà nền tảng cung cấp.

d. Dòng sự kiện chính (Basic Flow):

1. Khách vãng lai truy cập vào URL hệ thống.
2. Hệ thống hiển thị trang chủ công khai (Landing Page) bao gồm các khối nội dung: Hình ảnh giao diện trực quan, khối giới thiệu giải pháp quản trị, video mô phỏng cách vận hành.

e. Dòng sự kiện phụ (Alternative Flow): Không có.

### Chức năng Xem bảng giá gói dịch vụ (Pricing)

a. Tên chức năng: Xem bảng giá dịch vụ.

b. Đường dẫn: Trang chủ >> Menu >> Bảng giá (Pricing).

c. Mô tả chức năng (Brief description): Hiển thị các gói dịch vụ (Gói Miễn phí, Gói Doanh nghiệp) kèm biểu phí và tính năng giới hạn tương ứng.

d. Dòng sự kiện chính (Basic Flow):

1. Khách vãng lai click vào mục "Bảng giá" trên thanh điều hướng.
2. Hệ thống hiển thị bảng so sánh chi phí trên mỗi người dùng/tháng và các giới hạn về dung lượng dữ liệu, số lượng dự án tạo lập.
3. Người dùng có thể click chọn nút "Bắt đầu dùng thử" (Free Trial) để chuyển hướng đến form đăng ký.

e. Dòng sự kiện phụ (Alternative Flow): Không có.

### Chức năng Quản lý công việc theo giao diện Bảng

a. Tên chức năng: Tương tác qua giao diện Board.

b. Đường dẫn: Workspace >> Chọn Dự án >> Chọn "Board View".

c. Mô tả chức năng (Brief description): Hiển thị toàn bộ công việc dưới dạng các thẻ (Cards) phân bố theo các cột trạng thái. Cho phép cập nhật trạng thái bằng thao tác kéo thả (Drag and drop) thời gian thực.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng (PM hoặc Thành viên) truy cập vào dự án và chọn góc nhìn "Board".
2. Hệ thống hiển thị các cột tương ứng với các trạng thái (To Do, In Progress, Done). Mỗi nhiệm vụ hiển thị dưới dạng một thẻ thông tin.
3. Người dùng dùng chuột nhấn giữ một thẻ công việc, thực hiện kéo từ cột trạng thái cũ (ví dụ: To Do) sang cột trạng thái mới (ví dụ: In Progress).
4. Hệ thống ghi nhận hành động kéo thả, tự động cập nhật trường trạng thái của Task trong CSDL và làm mới màn hình ngay lập tức.

e. Dòng sự kiện phụ (Alternative Flow): Không có.

### Chức năng Thảo luận và Tương tác trên Công việc (Task Comments)

a. Tên chức năng: Bình luận trên công việc.

b. Đường dẫn: Click chọn một Task cụ thể >> Phân hệ Bình luận (Comments).

c. Mô tả chức năng (Brief description): Cho phép các thành viên viết bình luận, trao đổi ý kiến, đính kèm file trực tiếp bên trong cửa sổ chi tiết của công việc đó.

d. Dòng sự kiện chính (Basic Flow):

1. Người dùng click vào một thẻ công việc để mở cửa sổ chi tiết (Task Modal).
2. Tại ô nhập liệu "Viết bình luận...", người dùng nhập nội dung trao đổi và bấm nút "Gửi".
3. Hệ thống tiến hành lưu trữ nội dung bình luận, thông tin người gửi, dòng thời gian vào cơ sở dữ liệu.
4. Hệ thống kết xuất hiển thị bình luận mới ngay lập tức tại luồng thảo luận của Task và đẩy thông báo cho những thành viên liên quan.

e. Dòng sự kiện phụ (Alternative Flow):

- Dữ liệu rỗng: Người dùng nhấn gửi khi chưa nhập ký tự nào, hệ thống sẽ vô hiệu hóa không cho phép bấm nút "Gửi"

## Yêu cầu phi chức năng của phần mềm

### Yêu cầu bảo mật

#### Xác thực người dùng

Hệ thống bắt buộc xây dựng cơ chế đăng nhập an toàn, yêu cầu tên người dùng và mật khẩu hợp lệ. Hỗ trợ công nghệ xác thực đa yếu tố (MFA) để nâng cao tính an toàn.

#### Phân quyền truy cập

Thiết lập quyền hạn chặt chẽ khác nhau cho từng loại người dùng (Admin, Quản lý, Thành viên). Đảm bảo chỉ những người được cấp quyền mới có thể tiếp cận hoặc sửa đổi các dữ liệu nhạy cảm của dự án

#### Mã hóa dữ liệu

Các dữ liệu nhạy cảm liên quan đến thông tin cá nhân hoặc thông tin thanh toán của doanh nghiệp cần được mã hóa an toàn cả khi lưu trữ trong CSDL và khi truyền tải qua môi trường mạng.

#### Bảo vệ chống lại các mối đe dọa

Vận hành hệ thống sau các lớp bảo mật như tường lửa (Firewall), triển khai các giải pháp chống mã độc, cập nhật phần mềm định kỳ để vá lỗ hổng bảo mật trực tuyến.

### Yêu cầu sao lưu

Dữ liệu lưu trong hệ thống bắt buộc phải được sao lưu dự phòng tự động liên tục 24/24 thông qua kiến trúc máy chủ song hành (Replication) nhằm triệt tiêu nguy cơ mất mát dữ liệu khi có sự cố vật lý xảy ra.

Hỗ trợ tính năng kết xuất dữ liệu an toàn ra các thiết bị lưu trữ ngoài và đảm bảo khả năng phục hồi (Restore) dữ liệu toàn vẹn khi cần thiết

### Các yêu cầu về tính sử dụng (Usability)

- Hệ thống bắt buộc phải hỗ trợ truy cập, xử lý và cập nhật dữ liệu theo thời gian thực (Real-time update).

- Các tác vụ thông thường phải phản hồi tức thời; trong trường hợp tải nặng, thời gian trễ tối đa chấp nhận được phải dưới 30 giây.

- Đảm bảo hạ tầng phục vụ tốt tối thiểu 50 người dùng tương tác online cùng một thời điểm mà không gây nghẽn mạng.

- Thiết kế giao diện tinh gọn, thân thiện, tương thích tối ưu trên các trình duyệt web phổ biến hiện nay.

### Các yêu cầu về tính ổn định (Reliability)

Hệ thống đáp ứng các yêu cầu:

* Khi có sự cố nghiêm trọng làm ngừng vận hành hệ thống, cam kết phải khắc phục và phục hồi 90% chức năng trong vòng 1 giờ, đạt trạng thái ổn định 100% trong vòng 24 giờ tiếp theo.
* Tỷ lệ lỗi hệ thống phải giảm thiểu theo thời gian: Chấp nhận trung bình tối đa 1 lỗi/tháng trong 3 tháng đầu vận hành ; giảm xuống còn tối đa 1 lỗi/năm trong 3 năm tiếp theo và hướng tới sự ổn định tuyệt đối (0 lỗi/năm) ở các năm sau đó. Quy định lỗi chấp nhận là lỗi trung bình, không gây hỏng hóc hay tổn hại trầm trọng đến tính toàn vẹn của hệ thống.

# Đặc tả yêu cầu chức năng

## Use Case

### Sơ đồ Use Case tổng quát (cần vẽ)

![](data:image/png;base64...)

### Sơ đồ usecase phân rã chức năng

#### Sơ đồ Phân hệ Tài khoản & Xác thực

![](data:image/png;base64...)

#### Sơ đồ Phân hệ Quản lý Workspace

![](data:image/png;base64...)

#### Sơ đồ Phân hệ Quản lý Thành viên Dự án

![](data:image/png;base64...)

#### Sơ đồ Phân hệ Quản lý Dự án

![](data:image/png;base64...)

#### Sơ đồ Phân hệ Quản lý Công việc

![](data:image/png;base64...)

#### Sơ đồ Phân hệ Public (Landing Page)

![](data:image/png;base64...)

#### Sơ đồ Phân hệ Báo cáo & Dashboard

![](data:image/png;base64...)

#### Sơ đồ Tiến trình chạy ngầm (Background Jobs)

![](data:image/png;base64...)

### Danh sách các Use Case

|  |  |  |
| --- | --- | --- |
| **Mã** | **Tên use case** | **Ý nghĩa/ Ghi chú** |
| *1* | Đăng nhập | Xác thực tài khoản người dùng để truy cập vào phân hệ làm việc. |
| *2* | Đăng ký | Cho phép người dùng mới đăng ký tài khoản thành viên trong hệ thống. |
| *3* | Đăng xuất | Hủy phiên làm việc hiện tại, bảo mật thông tin tài khoản. |
| *4* | Đổi mật khẩu | Cho phép người dùng chủ động thay đổi mật khẩu cá nhân. |
| *5* | Quản lý thông tin cá nhân | Xem và cập nhật các thông tin cơ bản của hồ sơ cá nhân. |
| *6* | Tạo Không gian dự án (Space) | Khởi tạo một dự án lớn hoặc một không gian làm việc mới. |
| *7* | Chỉnh sửa thông tin dự án | Thay đổi mô tả, thời hạn hoặc cấu hình quy trình của dự án. |
| *8* | Xóa dự án | Loại bỏ dự án hoặc đưa dự án vào trạng thái lưu trữ ẩn. |
| *9* | Tìm kiếm dự án | Tra cứu nhanh dự án theo từ khóa hoặc bộ lọc trạng thái |
| *10* | Xem chi tiết dự án | Hiển thị toàn bộ thành viên, tiến độ và các task thuộc dự án. |
| *11* | Tạo công việc (Task) | Khởi tạo một đầu việc mới trực thuộc một dự án cụ thể. |
| *12* | Phân công công việc (Assign Task) | Giao nhiệm vụ cho một hoặc nhiều thành viên phụ trách. |
| *13* | Xóa công việc | Gỡ bỏ đầu việc không còn cần thiết ra khỏi danh sách |
| *14* | Mời và quản lý thành viên Workspace | Admin gửi email mời nhân sự mới hoặc phân vai trò hệ thống |
| *15* | Thêm thành viên vào dự án | Cấp quyền cho thành viên nội bộ tham gia vào một Space cụ thể. |
| *16* | Xem báo cáo tiến độ dự án | Thống kê số lượng Task theo trạng thái của dự án. |
| *17* | Xem báo cáo hiệu suất thành viên | Theo dõi biểu đồ Burndown chart và tốc độ xử lý Task của nhân sự. |
| *18* | Xuất dữ liệu Excel/PDF | Trích xuất các dữ liệu báo cáo ra file tài liệu độc lập. |
| *19* | Xem giới thiệu tính năng | Khách vãng lai xem thông tin giải pháp phần mềm tại trang chủ. |
| *20* | Xem bảng giá gói dịch vụ (Pricing) | Khách vãng lai tham khảo biểu phí và các giới hạn của từng gói. |
| *21* | Tương tác qua giao diện Board (Kanban) | Kéo thả để cập nhật trạng thái công việc trực quan (To Do -> Done). |
| *22* | Bình luận trên công việc (Task Comments) | Thảo luận, đính kèm tệp tin trực tiếp bên trong cửa sổ của Task. |

### Danh sách các tác nhân

| **Mã** | **Tác nhân** | **Mã Use case** |
| --- | --- | --- |
| 1 | Quản trị viên (Admin/Owner) | 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 21, 22 |
| 2 | Quản lý dự án (Project Manager) | 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15, 16, 17, 18, 21, 22 |
| 3 | Thành viên (Member/Employee) | 2, 3, 4, 5, 9, 10, 11, 21, 22 |
| 4 | Khách vãng lai (Guest/Visitor) | 1, 19, 20 |

## Đặc tả Use Case

### Use case 1, Đăng nhập

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#01** | | **Đăng nhập** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Xác thực tài khoản người dùng để truy cập vào hệ thống làm việc | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án, Thành viên. | |
| **Tiền điều kiện** | | Người dùng đã có tài khoản được kích hoạt trên hệ thống. | |
| **Hậu điều kiện** | **Thành công** | Đăng nhập thành công vào hệ thống, chuyển đến giao diện làm việc | |
| **Lỗi** | Đăng nhập thất bại, hệ thống báo lỗi | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Chức năng bắt đầu khi người dùng muốn đăng nhập vào hệ thống. 2. Hệ thống yêu cầu tác nhân nhập tài khoản và mật khẩu đăng nhập. 3. Tác nhân nhập tài khoản và mật khẩu đăng nhập của mình. 4. Hệ thống xác nhận tài khoản và mật khẩu đăng nhập có hợp lệ không. Nếu không hợp lệ thì thực hiện luồng A. 5. Hệ thống ghi lại quá trình đăng nhập và chuyển hướng sang Dashboard. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Nhập sai tài khoản/ mật khẩu đăng nhập** | | | |
| 1. Hệ thống hiển thị thông báo lỗi: "Tên đăng nhập hoặc mật khẩu không đúng". 2. Người sử dụng có thể chọn đăng nhập lại hoặc hủy bỏ đăng nhập, khi đó use case này sẽ kết thúc. | | | |
| **Luồng A1: Nhập thông tin không hợp lệ** | | | |
| 1. Hệ thống hiển thị thông báo "Tên đăng nhập và mật khẩu không để trống". 2. Người sử dụng nhập lại thông tin hoặc hủy bỏ, khi đó use case kết thúc. | | | |
| **Giao diện minh họa** | | | |
| *Giao diện đăng nhập*  ![](data:image/png;base64...) | | | |

### Use case 2, Đăng ký

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#02** | | **Đăng ký** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Cho phép người dùng mới điền thông tin để đăng ký tài khoản thành viên trong hệ thống | |
| **Tác nhân** | | Khách vãng lai | |
| **Tiền điều kiện** | | Người dùng chưa có tài khoản trong hệ thống và muốn tạo tài khoản mới. | |
| **Hậu điều kiện** | **Thành công** | Tài khoản mới được tạo và lưu thông tin mật khẩu dạng băm (Hash) vào cơ sở dữ liệu. | |
| **Lỗi** | Đăng ký không thành công. | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Tác nhân chọn chức năng Đăng ký trên màn hình. 2. Hệ thống hiển thị form yêu cầu nhập các thông tin cần thiết: Họ tên, Số điện thoại, Email, Mật khẩu. 3. Người dùng nhập đầy đủ thông tin và gửi yêu cầu đăng ký. 4. Hệ thống kiểm tra tính hợp lệ và duy nhất của thông tin đăng ký. Nếu không hợp lệ thực hiện luồng A. 5. Hệ thống thực hiện băm mật khẩu (Hash), tạo tài khoản mới vào cơ sở dữ liệu và gửi thông báo thành công. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Tài khoản đã tồn tại** | | | |
| 1. Hệ thống hiển thị thông báo lỗi: "Tài khoản đã tồn tại, vui lòng chọn tên khác." 2. Người dùng chọn nhập lại tên tài khoản khác hoặc hủy bỏ và quay lại trang chủ. | | | |
| **Luồng A1: Nhập thông tin không hợp lệ** | | | |
| 1. Hệ thống hiển thị thông báo lỗi: "Họ tên, số điện thoại, mật khẩu không được để trống". 2. Người dùng sửa lại thông tin hoặc hủy bỏ đăng ký | | | |
| **Giao diện minh họa** | | | |
| **![](data:image/png;base64...)** | | | |

### Use case 3, Đăng xuất

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#03** | | **Đăng xuất** | **Độ phức tạp: thấp** |
| **Mô tả** | | Hủy phiên làm việc hiện tại của người dùng để bảo mật thông tin tài khoản. | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án, Thành viên | |
| **Tiền điều kiện** | | Người dùng đang đăng nhập trong hệ thống. | |
| **Hậu điều kiện** | **Thành công** | Hủy session/cookie thành công và chuyển hướng về trang đăng nhập | |
| **Lỗi** | Phiên đăng nhập giữ nguyên (lỗi kết nối cục bộ) | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Tác nhân click vào menu tài khoản cá nhân và chọn mục "Đăng xuất". 2. Hệ thống thực hiện xóa session/cookie làm việc hiện tại của tài khoản. 3. Hệ thống điều hướng người dùng quay trở lại trang đăng nhập hệ thống. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh: N/A** | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use case 4, Đổi mật khẩu

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#01** | | **Đổi mật khẩu** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Cho phép người dùng chủ động thay đổi mật khẩu đăng nhập cá nhân | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án, Thành viên | |
| **Tiền điều kiện** | | Người dùng đang đăng nhập trong hệ thống | |
| **Hậu điều kiện** | **Thành công** | Mật khẩu mới được mã hóa và cập nhật thành công vào CSDL | |
| **Lỗi** | Mật khẩu cũ giữ nguyên, hệ thống báo lỗi xác thực dữ liệu | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Hệ thống hiển thị form đổi mật khẩu bao gồm các trường: Mật khẩu hiện tại, Mật khẩu mới, Xác nhận mật khẩu mới và nút "Cập nhật". 2. Người dùng điền đầy đủ thông tin vào các trường và ấn "Cập nhật". 3. Hệ thống kiểm tra tính chính xác của mật khẩu cũ và sự trùng khớp của mật khẩu mới. Nếu sai thực hiện luồng A hoặc A1. 4. Hệ thống cập nhật mật khẩu mới đã được mã hóa vào CSDL và thông báo thành công. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Mật khẩu cũ không chính xác** | | | |
| 1. Hệ thống hiển thị thông báo "Mật khẩu hiện tại không đúng". 2. Người dùng chọn nhập lại hoặc hủy bỏ thao tác. | | | |
| **Luồng A1: Mật khẩu mới không trùng khớp** | | | |
| Hệ thống hiển thị thông báo "Mật khẩu mới và mật khẩu xác nhận không khớp" | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use case 5, Quản lý thông tin cá nhân

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#5** | | **Quản lý thông tin cá nhân** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Xem và cập nhật các thông tin cơ bản của hồ sơ cá nhân. | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án, Thành viên | |
| **Tiền điều kiện** | | Người dùng đang truy cập hệ thống bằng tài khoản của mình | |
| **Hậu điều kiện** | **Thành công** | Thông tin mới được ghi đè vào bảng lưu trữ và hiển thị ra giao diện. | |
| **Lỗi** | Thay đổi bị từ chối, giữ nguyên dữ liệu hồ sơ cũ. | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng truy cập phân hệ hồ sơ cá nhân, hệ thống kết xuất thông tin hiện hành (Họ tên, SĐT, Email). 2. Người dùng chỉnh sửa các trường thông tin cho phép (Ví dụ: Số điện thoại, địa chỉ) và nhấn nút "Lưu thay đổi". 3. Hệ thống kiểm tra định dạng dữ liệu (Số điện thoại hợp lệ, Email đúng cấu trúc), tiến hành ghi đè dữ liệu mới vào bảng thông tin và thông báo cập nhật thành công. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Thông tin không hợp lệ** | | | |
| 1. Hệ thống hiển thị thông báo lỗi và yêu cầu sửa lại dữ liệu (ví dụ: Số điện thoại chứa ký tự chữ). 2. Người sử dụng có thể chọn sửa lại dữ liệu hoặc hủy bỏ thay đổi để kết thúc use case. | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 6: Tạo Không gian dự án (Space)

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#06** | | **Tạo Không gian dự án (Space)** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Khởi tạo một dự án lớn hoặc một không gian làm việc mới | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án | |
| **Tiền điều kiện** | | Người dùng có thẩm quyền khởi tạo thuộc nhóm quyền Admin/PM | |
| **Hậu điều kiện** | **Thành công** | Bản ghi dự án được khởi tạo thành công trong CSDL với trạng thái mặc định "Chưa thực hiện" | |
| **Lỗi** | Không tạo được không gian dự án, hệ thống báo lỗi logic | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Tác nhân click chọn chức năng "Thêm mới" dự án từ Dashboard. 2. Người dùng nhập đầy đủ các thông tin bắt buộc gồm: Tên dự án/Không gian, Mô tả, Quy trình trạng thái (Workflow), Ngày bắt đầu, Ngày kết thúc dự kiến và chọn nút "Lưu dự án". 3. Hệ thống thực hiện kiểm tra tính hợp lệ (Ngày kết thúc phải sau ngày bắt đầu). Nếu sai thực hiện luồng A hoặc A1. 4. Hệ thống thêm dự án vào cơ sở dữ liệu với trạng thái mặc định ban đầu là "Chưa thực hiện" (màu xanh) và thông báo thành công. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Thiếu thông tin bắt buộc** | | | |
| 1. Hệ thống hiển thị thông báo lỗi: "Vui lòng điền đầy đủ các thông tin có dấu (\*)" | | | |
| **Luồng A1: *Sai logic thời gian*** | | | |
| 1. Hệ thống thông báo: "Ngày kết thúc dự án không thể trước ngày bắt đầu" | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 7: Chỉnh sửa thông tin dự án

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#07** | | **Chỉnh sửa thông tin dự án** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Thay đổi mô tả, thời hạn hoặc cấu hình quy trình của dự án | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án | |
| **Tiền điều kiện** | | Không gian dự án cần chỉnh sửa đang hiện hữu trong hệ thống. | |
| **Hậu điều kiện** | **Thành công** | Các thông số hoặc trạng thái màu sắc mới của dự án được cập nhật đồng bộ | |
| **Lỗi** | Giữ nguyên dữ liệu cũ của dự án, xuất hộp thoại báo lỗi | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng chọn một dự án trong danh sách và bấm nút "Sửa". 2. Form "Cập nhật dự án" xuất hiện hiển thị đầy đủ dữ liệu cũ của dự án (mã dự án được khóa không cho sửa). 3. Người dùng thực hiện chỉnh sửa thông tin hoặc cập nhật lại Trạng thái (Chuyển sang "Đang thực hiện" - hiển thị màu vàng, hoặc "Đã hoàn thành" - hiển thị màu đỏ). 4. Người dùng nhấn nút "Lưu" để đồng bộ dữ liệu vào hệ thống. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Dự án không tồn tại** | | | |
| 1. Nếu dự án vừa chọn bị tác nhân khác xóa trước đó, hệ thống báo lỗi: "Dự án không tồn tại hoặc đã bị xóa". | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 8: Xóa dự án

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#08** | | **Xóa dự án** | **Độ phức tạp: thấp** |
| **Mô tả** | | Loại bỏ dự án hoặc đưa dự án vào trạng thái lưu trữ ẩn | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án | |
| **Tiền điều kiện** | | Dự án được chọn xóa tồn tại trên cơ sở dữ liệu | |
| **Hậu điều kiện** | **Thành công** | Bản ghi dự án và các task liên quan bị gỡ bỏ, làm mới danh sách hiển thị | |
| **Lỗi** | Dự án được giữ nguyên, hệ thống đóng hộp thoại cảnh báo | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng bấm vào nút "Xóa" tại dòng chứa thông tin dự án cần loại bỏ. 2. Hệ thống hiển thị hộp thoại cảnh báo: "Bạn có chắc chắn muốn xóa dự án này? Hành động này sẽ xóa toàn bộ công việc liên quan." 3. Người dùng bấm nút "Xác nhận xóa". 4. Hệ thống thực hiện xóa dự án, cập nhật lại giao diện bảng danh sách dự án và thông báo xóa thành công. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Hủy bỏ thao tác** | | | |
| 1. Người dùng nhấn nút "Hủy", hệ thống đóng hộp thoại và giữ nguyên hiện trạng dự án | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 9: Tìm kiếm dự án

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#09** | | **Tìm kiếm dự án** | **Độ phức tạp: thấp** |
| **Mô tả** | | Tra cứu nhanh dự án theo từ khóa hoặc bộ lọc trạng thái | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án, Thành viên | |
| **Tiền điều kiện** | | Người dùng đang đứng ở phân hệ Quản lý dự án | |
| **Hậu điều kiện** | **Thành công** | Giao diện hiển thị danh sách các bản ghi thỏa mãn từ khóa tìm kiếm | |
| **Lỗi** | Hệ thống xuất dòng cảnh báo không tìm thấy kết quả phù hợp | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng nhập từ khóa tìm kiếm hoặc lựa chọn tiêu chí lọc trên thanh công cụ. 2. Người dùng nhấn phím Enter hoặc click vào biểu tượng kính lúp "Tìm kiếm". 3. Hệ thống quét qua bảng dữ liệu dự án và lọc ra các bản ghi thỏa mãn điều kiện. 4. Kết quả được trả về hiển thị dưới dạng bảng danh sách dự án thu hẹp. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Không có kết quả phù hợp** | | | |
| 1. Hệ thống hiển thị thông báo trên màn hình: "Không tìm thấy dự án nào phù hợp với tiêu chí tìm kiếm của bạn" | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 10: Xem chi tiết dự án

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#10** | | **Xem chi tiết dự án** | **Độ phức tạp: thấp** |
| **Mô tả** | | Hiển thị toàn bộ thành viên, tiến độ và các task thuộc dự án. | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án, Thành viên | |
| **Tiền điều kiện** | | Dự án hợp lệ có sẵn trên bảng điều khiển để lựa chọn | |
| **Hậu điều kiện** | **Thành công** | Trang thông tin chuyên sâu của dự án tải thành công đầy đủ số liệu | |
| **Lỗi** | Hệ thống báo lỗi kết nối dữ liệu trang, giữ nguyên màn hình cũ | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng click vào nút "Xem chi tiết" của một dự án trên giao diện bảng. 2. Hệ thống chuyển hướng tới trang thông tin chuyên sâu của dự án đó. 3. Màn hình hiển thị: Thông tin tổng quan (Mô tả, Quy trình), Biểu đồ tiến độ, Danh sách nhân sự phụ trách dự án, Danh sách công việc chia theo tiến độ. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh: N/A** | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 11: Tạo công việc (Task)

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#11** | | **Tạo công việc (Task)** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Khởi tạo một đầu việc mới trực thuộc một dự án cụ thể | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án, Thành viên | |
| **Tiền điều kiện** | | Người dùng đã truy cập vào giao diện chi tiết của một dự án cụ thể | |
| **Hậu điều kiện** | **Thành công** | Task mới được lưu thành công vào CSDL với trạng thái mặc định "To Do". | |
| **Lỗi** | Nhiệm vụ không được lưu, hệ thống báo lỗi sai thời hạn | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng nhấn nút "Thêm công việc". 2. Hệ thống hiển thị một biểu mẫu yêu cầu nhập: Tên công việc, Mô tả nhiệm vụ, Ngày bắt đầu, Hạn hoàn thành (Deadline), Mức độ ưu tiên (Thấp/Trung bình/Cao) và tệp đính kèm. 3. Người dùng nhập đầy đủ thông tin và chọn "Lưu". 4. Hệ thống kiểm tra tính hợp lệ thời gian. Nếu quá hạn dự án thực hiện luồng A. 5. Hệ thống thêm đầu việc mới vào dự án dưới trạng thái mặc định "To Do". | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Sai thời hạn công việc** | | | |
| 1. Sai thời hạn: Hệ thống báo lỗi nếu hạn hoàn thành công việc vượt quá thời gian cho phép của dự án cha | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 12: Phân công công việc (Assign Task)

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#12** | | **Phân công công việc (Assign Task)** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Giao nhiệm vụ cho một hoặc nhiều thành viên phụ trách | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án | |
| **Tiền điều kiện** | | Thẻ công việc cần giao đang ở trạng thái trống người đảm nhận. | |
| **Hậu điều kiện** | **Thành công** | Mã thành viên được liên kết vào Task, hệ thống tự động đẩy thông báo | |
| **Lỗi** | Không cập nhật được người phụ trách, giữ nguyên trạng thái cũ | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Tại màn hình quản lý công việc, người dùng click vào chức năng "Phân công" (Assignee) tại đầu việc tương ứng. 2. Hệ thống hiển thị danh sách thả xuống (Dropdown) chứa toàn bộ thành viên thuộc dự án này. 3. Người dùng tích chọn nhân viên chịu trách nhiệm và ấn "Xác nhận". 4. Hệ thống cập nhật trường người phụ trách vào bảng công việc và gửi thông báo hệ thống đến tài khoản của thành viên đó | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh: N/A** | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 13: Xóa công việc

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#13** | | **Xóa công việc** | **Độ phức tạp: thấp** |
| **Mô tả** | | Gỡ bỏ đầu việc không còn cần thiết ra khỏi danh sách | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án | |
| **Tiền điều kiện** | | Thẻ nhiệm vụ cần xóa đang tồn tại trong phân hệ dự án | |
| **Hậu điều kiện** | **Thành công** | Bản ghi công việc bị xóa khỏi CSDL, làm mới màn hình | |
| **Lỗi** | Dữ liệu công việc giữ nguyên, popup xác nhận tự động đóng | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng nhấn nút "Xóa" tại đầu việc cần loại bỏ. 2. Hệ thống hiển thị popup yêu cầu xác nhận hành động xóa. 3. Người dùng nhấn nút "Đồng ý xóa". 4. Hệ thống thực hiện xóa bản ghi công việc khỏi CSDL và làm mới màn hình. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Hủy bỏ thao tác** | | | |
| 1. Nếu người dùng chọn "Hủy", hệ thống đóng popup và giữ nguyên dữ liệu công việc. | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 14: Mời và quản lý thành viên Workspace

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#14** | | **Mời và quản lý thành viên Workspace** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Admin khởi tạo đường dẫn liên kết (Link mời) để nhân sự mới kích hoạt tham gia vào hệ thống hoặc điều chỉnh vai trò hệ thống | |
| **Tác nhân** | | Quản trị viên (Admin/Owner) | |
| **Tiền điều kiện** | | Admin đang đăng nhập và thực hiện cấu hình tại phân hệ Quản lý Workspace | |
| **Hậu điều kiện** | **Thành công** | Đường dẫn liên kết (Link mời) được khởi tạo thành công, hệ thống sinh mã token hợp lệ. | |
| **Lỗi** | Không tạo được đường dẫn, hệ thống báo lỗi kết nối hoặc phân quyền | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Admin nhấn chọn chức năng "Quản lý thành viên" từ menu Workspace. 2. Admin nhấn chọn nút lệnh [Khởi tạo Link mời] (Generate Invite Link). 3. Hệ thống yêu cầu Admin chọn vai trò phân quyền mặc định cho link này (Manager hoặc Member). 4. Admin chọn vai trò tương ứng và nhấn nút [Xác nhận]. 5. Hệ thống tự động sinh ra một đường dẫn liên kết độc nhất (URL kèm Token bảo mật), hiển thị lên màn hình và tự động lưu vào bộ nhớ tạm (Clipboard) để Admin có thể sao chép và gửi cho nhân sự. 6. Khi nhân sự click vào đường dẫn này, hệ thống sẽ điều hướng họ đến trang Đăng ký tài khoản nội bộ và tự động gắn họ vào Workspace với vai trò đã cấu hình. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Đường dẫn liên kết hết hạn hoặc không hợp lệ** | | | |
| 1. Nhân sự click vào đường dẫn mời đã quá thời gian hiệu lực cấu hình (ví dụ: sau 7 ngày). 2. Hệ thống hiển thị thông báo lỗi: *"Đường dẫn mời đã hết hạn hoặc không còn tồn tại trên hệ thống. Vui lòng liên hệ Admin để nhận link mới"*. 3. Tiến trình đăng ký thành viên bị hủy bỏ. | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 15: Thêm thành viên vào dự án

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#01** | | **Thêm thành viên vào dự án** | **Độ phức tạp: thấp** |
| **Mô tả** | | Cập nhật quyền hạn cho thành viên tham gia vào một Space cụ thể | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án | |
| **Tiền điều kiện** | | Tài khoản nhân sự đã nằm trong danh sách Workspace tổng | |
| **Hậu điều kiện** | **Thành công** | Thành viên được thêm vào bảng liên kết dự án, cấp quyền truy cập Space | |
| **Lỗi** | Giữ nguyên danh sách thành viên dự án hiện tại | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Quản lý dự án chọn mục "Thêm thành viên vào dự án" tại giao diện chi tiết dự án. 2. Hệ thống hiển thị danh sách nhân sự khả dụng trong Workspace tổng. 3. Người dùng tích chọn nhân viên, phân vai trò trong dự án (Developer, Tester,...) và nhấn "Xác nhận". | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh: N/A** | | | |
| **Giao diện minh họa** | | | |
| ![](data:image/png;base64...) | | | |

### Use Case 16: Xem báo cáo tiến độ dự án

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#16** | | **Xem báo cáo tiến độ dự án** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Thống kê số lượng Task theo các trạng thái của dự án | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án | |
| **Tiền điều kiện** | | Hệ thống đã ghi nhận các đầu việc thuộc không gian dự án | |
| **Hậu điều kiện** | **Thành công** | Màn hình hiển thị đầy đủ số liệu thống kê dưới dạng biểu đồ | |
| **Lỗi** | Biểu đồ không tải được dữ liệu do sự cố đồng bộ | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng chọn mục "Báo cáo dự án" từ phân hệ Báo cáo thống kê. 2. Hệ thống tự động quét dữ liệu tổng thể và tính toán trạng thái các Task vụn vạt. 3. Hệ thống hiển thị báo cáo dạng biểu đồ trực quan, hỗ trợ bộ lọc theo thời gian | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh: N/A** | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 17: Xem báo cáo hiệu suất thành viên

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#17** | | **Xem báo cáo hiệu suất thành viên** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Theo dõi biểu đồ Burndown chart và tốc độ xử lý Task của nhân sự | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án | |
| **Tiền điều kiện** | | Thành viên đã có tương tác xử lý các Task được phân công | |
| **Hậu điều kiện** | **Thành công** | Kết xuất và hiển thị chính xác biểu đồ năng suất làm việc | |
| **Lỗi** | Hệ thống báo lỗi không tải được dữ liệu hiệu suất của nhân sự | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng chọn mục Báo cáo nhân viên trên menu chức năng. 2. Hệ thống tổng hợp dữ liệu từ bảng công việc gắn với từng mã thành viên cụ thể. 3. Hệ thống xử lý dữ liệu và xuất ra biểu đồ hiệu suất (Burndown chart) trực quan. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh: N/A** | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 18: Xuất dữ liệu Excel/PDF

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#18** | | **Xuất dữ liệu Excel/PDF** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Trích xuất các dữ liệu báo cáo ra file tài liệu độc lập | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án | |
| **Tiền điều kiện** | | Người dùng đang đứng ở màn hình hiển thị bảng số liệu báo cáo | |
| **Hậu điều kiện** | **Thành công** | Tệp tài liệu định dạng Excel/PDF được tải xuống máy thành công | |
| **Lỗi** | Tiến trình xuất file thất bại, dữ liệu tải xuống bị hủy | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng click chọn nút "Xuất dữ liệu Excel" hoặc "Xuất dữ liệu PDF" tại góc màn hình. 2. Hệ thống tiếp nhận yêu cầu, xử lý chuyển đổi cấu trúc giao diện thành file tài liệu độc lập. 3. Nếu gặp sự cố biên dịch tệp dữ liệu, thực hiện luồng A. 4. Hệ thống tiến hành tải xuống thiết bị cục bộ của người dùng. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Lỗi tiến trình kết xuất file** | | | |
| 1. Lỗi xuất file: Hệ thống hiển thị thông báo lỗi: "Xuất file thất bại, vui lòng thử lại sau". | | | |
| **Giao diện minh họa** | | | |
| *![](data:image/png;base64...)* | | | |

### Use Case 19: Xem giới thiệu tính năng

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#19** | | **Xem giới thiệu tính năng** | **Độ phức tạp: thấp** |
| **Mô tả** | | Khách vãng lai xem thông tin giải pháp phần mềm tại trang chủ | |
| **Tác nhân** | | Khách vãng lai | |
| **Tiền điều kiện** | | Người dùng truy cập hệ thống ở chế độ chưa đăng nhập tài khoản | |
| **Hậu điều kiện** | **Thành công** | Giao diện Landing Page tải thành công đầy đủ nội dung quảng bá | |
| **Lỗi** | Trình duyệt báo lỗi kết nối mạng (HTTP 404/500) | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Khách vãng lai truy cập vào URL công khai của hệ thống phần mềm. 2. Hệ thống tiếp nhận yêu cầu và điều hướng hiển thị trang chủ công khai (Landing Page). 3. Màn hình kết xuất hiển thị các khối nội dung giới thiệu: Hình ảnh trực quan, giải pháp quản trị, video mô phỏng quy trình phối hợp nội bộ. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh: N/A** | | | |
| **Giao diện minh họa** | | | |
| ![](data:image/png;base64...) | | | |

### Use Case 20: Xem bảng giá gói dịch vụ (Pricing)

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#20** | | **Xem bảng giá gói dịch vụ (Pricing)** | **Độ phức tạp: thấp** |
| **Mô tả** | | Khách vãng lai tham khảo biểu phí và các giới hạn của từng gói tài khoản. | |
| **Tác nhân** | | Khách vãng lai | |
| **Tiền điều kiện** | | Người dùng đang đứng ở giao diện trang chủ công khai của phần mềm. | |
| **Hậu điều kiện** | **Thành công** | Bảng thông tin so sánh giá cả và hạn mức các gói hiển thị rõ ràng | |
| **Lỗi** | Hệ thống không phản hồi danh mục bảng giá | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Khách vãng lai click vào mục "Bảng giá" (Pricing) trên thanh điều hướng công khai. 2. Hệ thống truy xuất dữ liệu gói và hiển thị bảng so sánh chi phí trên mỗi thành viên/tháng. 3. Hiển thị chi tiết hạn mức về dung lượng lưu trữ, số lượng dự án tạo lập của từng gói. 4. Người dùng có thể click chọn nút "Bắt đầu dùng thử" (Free Trial) để chuyển hướng đến form đăng ký tài khoản. | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh: N/A** | | | |
| **Giao diện minh họa** | | | |
| ![](data:image/png;base64...) | | | |

### Use Case 21:Tương tác qua giao diện Board (Kanban View)

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#21** | | **Tương tác qua giao diện Board (Kanban)** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Kéo thả để cập nhật trạng thái công việc trực quan (To Do -> Done) | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án, Thành viên | |
| **Tiền điều kiện** | | Người dùng đã mở giao diện phân hệ dự án và chọn chế độ xem "Board View". | |
| **Hậu điều kiện** | **Thành công** | Thẻ công việc nằm cố định tại cột mới, trường status thay đổi trong CSDL | |
| **Lỗi** | Thẻ công việc bị đẩy ngược lại cột cũ (lỗi kết nối CSDL mạng) | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng truy cập vào dự án và chọn góc nhìn bảng "Board View". 2. Hệ thống hiển thị các cột tương ứng với trạng thái quy trình (To Do, In Progress, Done). 3. Người dùng dùng chuột nhấn giữ một thẻ công việc, thực hiện kéo từ cột trạng thái cũ sang cột trạng thái mới. 4. Hệ thống ghi nhận hành động kéo thả, tự động gọi Controller cập nhật trường trạng thái của Task trong CSDL và làm mới màn hình ngay lập tức | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh: N/A** | | | |
| **Giao diện minh họa** | | | |
| ![](data:image/png;base64...) | | | |

### Use Case 22: Bình luận trên công việc (Task Comments)

|  |  |  |  |
| --- | --- | --- | --- |
| **UC#22** | | **Bình luận trên công việc (Task Comments)** | **Độ phức tạp: trung bình** |
| **Mô tả** | | Thảo luận, đính kèm tệp tin trực tiếp bên trong cửa sổ của Task | |
| **Tác nhân** | | Quản trị viên, Quản lý dự án, Thành viên. | |
| **Tiền điều kiện** | | Người dùng đã click mở cửa sổ chi tiết của một Task (Task Modal) | |
| **Hậu điều kiện** | **Thành công** | Nội dung bình luận được lưu vào bảng dữ liệu và kết xuất hiển thị lập tức | |
| **Lỗi** | Ý kiến gửi đi bị chặn, hệ thống báo lỗi trường thông tin rỗng | |
| **ĐẶC TẢ CHỨC NĂNG** | | | |
| **Luồng sự kiện chính/Kịch bản chính** | | | |
| 1. Người dùng click mở cửa sổ xem chi tiết của một nhiệm vụ công việc. 2. Tại ô nhập liệu "Viết bình luận...", người dùng nhập nội dung trao đổi và bấm nút "Gửi". 3. Hệ thống kiểm tra dữ liệu, nếu trống ký tự thực hiện luồng A. 4. Hệ thống tiến hành lưu trữ nội dung bình luận, thông tin người gửi, dòng thời gian vào cơ sở dữ liệu. 5. Hệ thống kết xuất hiển thị bình luận mới ngay lập tức tại luồng thảo luận của Task và đẩy thông báo cho những thành viên liên quan | | | |
| **Luồng sự kiện phát sinh/Kịch bản phát sinh** | | | |
| **Luồng A: Ô nhập liệu rỗng không có văn bản** | | | |
| 1. Dữ liệu rỗng: Người dùng nhấn gửi khi chưa nhập ký tự nào, hệ thống sẽ vô hiệu hóa không cho phép bấm nút "Gửi" | | | |
| **Giao diện minh họa** | | | |
| ![](data:image/png;base64...) | | | |

# THIẾT KẾ PHẦN MỀM

## Biểu đồ kiến trúc

Hệ thống được phát triển trên nền tảng .NET 10 bằng ngôn ngữ C#, tuân thủ nghiêm ngặt mô hình kiến trúc MVC (Model - View - Controller) kết hợp với các công nghệ kết nối thời gian thực nhằm đáp ứng trải nghiệm kéo thả như ClickUp.

* View (Giao diện hiển thị - Razor Views .cshtml):
  + Sử dụng cú pháp Razor engine (C#) kết hợp HTML5, CSS3 và JavaScript để dựng giao diện.
  + Tích hợp thư viện Frontend (như SortableJS hoặc Dragula) để xử lý sự kiện kéo thả các thẻ Task trên bảng Kanban (Board View).
  + Sử dụng SignalR Core (công nghệ Real-time mặc định của .NET) để khi một thành viên kéo thả Task hoặc gửi bình luận, giao diện của các thành viên khác lập tức cập nhật dữ liệu tự động mà không cần tải lại trang.
* Controller (Thành phần điều khiển - C# Classes):
  + Kế thừa lớp Microsoft.AspNetCore.Mvc.Controller.
  + Tiếp nhận các HTTP Request (GET, POST, PUT, DELETE) từ View hoặc các lệnh gọi API từ JavaScript gửi lên.
  + Thực hiện kiểm tra logic nghiệp vụ (Validation), kiểm tra quyền hạn (Authorize) của tài khoản và gọi xuống lớp Model để tương tác với Cơ sở dữ liệu.
* Model (Thành phần dữ liệu & Thực thể C#):
  + Bao gồm các lớp đối tượng đại diện cho cấu trúc bảng CSDL (Entities) và các lớp chứa logic nghiệp vụ xử lý dữ liệu (Repositories/Services).
  + Sử dụng công nghệ Entity Framework Core 10 (EF Core) theo hướng *Code First* để tự động ánh xạ (Mapping) các lớp C# thành các bảng tương ứng trong Hệ quản trị cơ sở dữ liệu.

## Thiết kế đối tượng

### Biểu đồ lớp đối tượng và quan hệ

![](data:image/png;base64...)

### Đặc tả các lớp đối tượng

#### Đối tượng TAIKHOAN

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_tai\_khoan | Mã định danh của tài khoản |
| **2** | ma\_goi | Mã gói đang sử dụng |
| **3** | ho\_ten | Họ và tên người dùng |
| **4** | email | Địa chỉ email đăng nhập |
| **5** | vai\_tro | Vai trò của người dùng trong hệ thống |
| **6** | so\_dien\_thoai | Số điện thoại liên hệ |
| **7** | mat\_khau | Mật khẩu đăng nhập (đã hash) |
| **8** | trang\_thai | Trạng thái hoạt động của tài khoản |
| **9** | ngay\_tao | Ngày tạo tài khoản |

#### Đối tượng GOIDICHVU

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_goi | Mã gói dịch vụ |
| **2** | ten\_goi | Tên gói dịch vụ |
| **3** | gia | Giá của gói dịch vụ |
| **4** | so\_du\_an\_toi\_da | Số lượng dự án tối đa được tạo |
| **5** | so\_thanh\_vien\_toi\_da | Số lượng thành viên tối đa |
| **6** | dung\_luong\_luu\_tru | Dung lượng lưu trữ tối đa |
| **7** | mo\_ta | Mô tả gói dịch vụ |

#### Đối tượng WORKSPACE

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_workspace | Mã Workspace |
| **2** | ma\_tai\_khoan | Mã tài khoản sở hữu |
| **3** | ten\_workspace | Tên Workspace |
| **4** | mo\_ta | Mô tả Workspace |
| **5** | ngay\_tao | Ngày tạo Workspace |

#### Đối tượng DUAN

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_du\_an | Mã dự án |
| **2** | ma\_workspace | Mã Workspace chứa dự án |
| **3** | ten\_du\_an | Tên dự án |
| **4** | mo\_ta | Mô tả dự án |
| **5** | ngay\_bat\_dau | Ngày bắt đầu dự án |
| **6** | ngay\_ket\_thuc | Ngày kết thúc dự án (dự kiến) |
| **7** | trang\_thai | Trạng thái của dự án |

#### Đối tượng THANHVIEN

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_thanh\_vien | Mã thành viên |
| **2** | ma\_du\_an | Mã dự án tham gia |
| **3** | ma\_tai\_khoan | Mã tài khoản của thành viên |
| **4** | vai\_tro\_du\_an | Vai trò của thành viên trong dự án |
| **5** | ngay\_tham\_gia | Ngày tham gia dự án |

#### Đối tượng CONGVIEC

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_cong\_viec | Mã công việc |
| **2** | ma\_du\_an | Mã dự án chứa công việc |
| **3** | ma\_thanh\_vien | Mã thành viên được giao thực hiện |
| **4** | ten\_cong\_viec | Tên công việc |
| **5** | deadline | Hạn hoàn thành |
| **6** | mo\_ta | Mô tả công việc |
| **7** | muc\_do\_uu\_tien | Mức độ ưu tiên |
| **8** | trang\_thai | Trạng thái |

#### Đối tượng BINHLUAN

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_binh\_luan | Mã bình luận |
| **2** | ma\_cong\_viec | Mã công việc được bình luận |
| **3** | ma\_tai\_khoan | Mã tài khoản tạo bình luận |
| **4** | noi\_dung | Nội dung bình luận |
| **5** | thoi\_gian | Thời gian tạo bình luận |
|  |  |  |

#### Đối tượng TEPDINHKEM

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_tep | Mã tệp đính kèm |
| **2** | ma\_cong\_viec | Mã công việc được bình luận |
| **3** | ten\_tep | Tên tệp |
| **4** | duong\_dan | Đường dẫn lưu trữ tệp |
| **5** | dung\_luong | Dung lượng tệp |

#### Đối tượng THONGBAO

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_thong\_bao | Mã thông báo |
| **2** | ma\_tai\_khoan | Mã tài khoản nhận thông báo |
| **3** | noi\_dung | Nội dung thông báo |
| **4** | thoi\_gian | Thời gian gửi thông báo |
| **5** | trang\_thai | Trạng thái đã đọc hoặc chưa đọc |

#### Đối tượng BAOCAO

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_bao\_cao | Mã báo cáo |
| **2** | loai\_bao\_cao | |  | | --- | |  |  |  | | --- | | Loại báo cáo | |
| **3** | ngay\_tao | Ngày tạo báo cáo |

#### Đối tượng INVITELINK

| **STT** | **Thuộc tính/ Phương thức** | **Mô tả** |
| --- | --- | --- |
| **1** | ma\_link | Mã liên kết mời |
| **2** | ma\_workspace | |  | | --- | |  |   Mã Workspace được tạo liên kết |
| **3** | token | Mã xác thực của liên kết |
| **4** | ngay\_het\_han | Thời gian hết hạn của liên kết |

## Biểu đồ luồng dữ liệu

![](data:image/png;base64...)

## Biểu đồ trình tự

### Biểu đồ trình tự Đăng nhập

![](data:image/png;base64...)

### Biểu đồ trình tự Đăng ký

![](data:image/png;base64...)

### Biểu đồ trình tự Đăng xuất

![](data:image/png;base64...)

### Biểu đồ trình tự Đổi Mật Khẩu

![](data:image/png;base64...)

### Biểu đồ trình tự Cập nhật thông tin cá nhân

![](data:image/png;base64...)

### Biểu đồ trình tự Tạo Workspace

![](data:image/png;base64...)

### Biểu đồ trình tự Chỉnh sửa dự án

![](data:image/png;base64...)

### Biểu đồ trình tự Xóa dự án

![](data:image/png;base64...)

### Biểu đồ trình tự Tìm kiếm dự án

![](data:image/png;base64...)

### Biểu đồ trình tự Xem chi tiết dự án

![](data:image/png;base64...)

### Biểu đồ trình tự Tạo công việc

![](data:image/png;base64...)

### Biểu đồ trình tự Phân công công việc

![](data:image/png;base64...)

### Biểu đồ trình tự Xóa công việc

![](data:image/png;base64...)

### Biểu đồ trình tự Mời và quản lý thành viên dự án

![](data:image/png;base64...)

### Biểu đồ trình tự Thêm thành viên vào dự án

![](data:image/png;base64...)

### Biểu đồ trình tự Xem báo cáo tiến độ dự án

![](data:image/png;base64...)

### Biểu đồ trình tự Xem báo cáo hiệu suất thành viên

![](data:image/png;base64...)

### Biểu đồ trình tự Xuất dữ liệu Excel/PDF

![](data:image/png;base64...)

### Biểu đồ trình tự Xem giới thiệu tính năng

![](data:image/png;base64...)

### Biểu đồ trình tự Xem bảng giá gói dịch vụ

![](data:image/png;base64...)

### Biểu đồ trình tự Tương tác qua giao diện Board (Kanban View)

![](data:image/png;base64...)

### Biểu đồ trình tự Bình luận trên công việc (Task Comments)

![](data:image/png;base64...)

## Sơ đồ hoạt động

### Sơ đồ hoạt động Đăng nhập

![](data:image/png;base64...)

### Sơ đồ hoạt động Đăng ký

![](data:image/png;base64...)

### Sơ đồ hoạt động Đăng xuất

![](data:image/png;base64...)

### Sơ đồ hoạt động Đổi mật khẩu

![](data:image/png;base64...)

### Sơ đồ hoạt động Quản lý thông tin cá nhân

![](data:image/png;base64...)

### Sơ đồ hoạt động Tạo Không gian dự án (Space)

![](data:image/png;base64...)

### Sơ đồ hoạt động Chỉnh sửa thông tin dự án

![](data:image/png;base64...)

### Sơ đồ hoạt động Xóa dự án

![](data:image/png;base64...)

### Sơ đồ hoạt động Tìm kiếm dự án

![](data:image/png;base64...)

### Sơ đồ hoạt động Xem chi tiết dự án

![](data:image/png;base64...)

### Sơ đồ hoạt động Tạo công việc

![](data:image/png;base64...)

### Sơ đồ hoạt động Phân công việc

![](data:image/png;base64...)

### Sơ đồ hoạt động Xóa công việc

![](data:image/png;base64...)

### Sơ đồ hoạt động Mời và quản lý thành viên Workspace

![](data:image/png;base64...)

### Sơ đồ hoạt động Thêm thành viên vào dự án

![](data:image/png;base64...)

### Sơ đồ hoạt động Xem báo cáo tiến độ dự án

![](data:image/png;base64...)

### Sơ đồ hoạt động Xem báo cáo hiệu suất thành viên

![](data:image/png;base64...)

### Sơ đồ hoạt động Xuất dữ liệu Excel/PDF

![](data:image/png;base64...)

### Sơ đồ hoạt động Xem giới thiệu tính năng

![](data:image/png;base64...)

### Sơ đồ hoạt động Xem bảng giá gói dịch vụ

![](data:image/png;base64...)

### Sơ đồ hoạt động Tương tác qua giao diện Board (Kanban View)

![](data:image/png;base64...)

### Sơ đồ hoạt động Bình luận trên công việc (Task Comments)

![](data:image/png;base64...)

# THIẾT KẾ DỮ LIỆU

## Biểu đồ dữ liệu

![](data:image/png;base64...)

## Đặc tả các kiểu dữ liệu

### Bảng Gói dịch vụ (GOIDICHVU)

|  |  |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **STT** | |  | | --- | | **Tên Trường** |  |  | | --- | |  | | |  | | --- | | **Ràng buộc** |  |  | | --- | |  | | |  | | --- | | **Độ dài dữ liệu** |  |  | | --- | |  | | | **Mô tả** | | --- |  |  | | --- | |  | |
| **1** | ma\_goi | PRIMARY KEY | VARCHAR(50) | Mã định danh duy nhất của gói dịch vụ |
| **2** | ten\_goi | NOT NULL | NVARCHAR(100) | Tên của gói dịch vụ (Free, Pro, Enterprise...) |
| **3** | gia | NOT NULL, DEFAULT 0 | DECIMAL(10,2) | Giá tiền của gói dịch vụ/tháng |
| **4** | so\_du\_an\_toi\_da | NOT NULL | INT | Giới hạn số lượng dự án được phép tạo lập |
| **5** | so\_tv\_toi\_da | NOT NULL | INT | Giới hạn số lượng thành viên tham gia Workspace |
| **6** | dung\_luong\_max | NOT NULL | BIGINT | Dung lượng lưu trữ tối đa cho phép (tính bằng MB) |
| **7** | mo\_ta | NULL | NVARCHAR(500) | Mô tả chi tiết quyền lợi của gói dịch vụ |

### Bảng Tài khoản (TAIKHOAN)

|  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **STT** | |  | | --- | | **Tên Trường** |  |  | | --- | |  | | **Ràng buộc** | |  | | --- | | **Độ dài dữ liệu** |  |  | | --- | |  | | | **Mô tả** | | --- |  |  | | --- | |  | |
| **1** | ma\_tai\_khoan | PRIMARY KEY | VARCHAR(50) | Mã định danh duy nhất của tài khoản người dùng |
| **2** | ma\_goi | FOREIGN KEY | VARCHAR(50) | Liên kết tới bảng GOIDICHVU |
| **3** | ho\_ten | NOT NULL | NVARCHAR(100) | Họ và tên đầy đủ của người dùng |
| **4** | email | UNIQUE, NOT NULL | VARCHAR(100) | Địa chỉ Email dùng làm tên đăng nhập hệ thống |
| **5** | mat\_khau | NOT NULL | VARCHAR(255) | Mật khẩu truy cập hệ thống (đã băm mã hóa) |
| **6** | so\_dien\_thoai | NULL | VARCHAR(15) | Số điện thoại liên hệ |
| **7** | vai\_tro | NOT NULL | VARCHAR(50) | Vai trò hệ thống chung (Admin, Manager, Member, Guest) |
| **8** | trang\_thai | NOT NULL | VARCHAR(50) | Trạng thái tài khoản (KichHoat, Khoa, ChoXacNhan) |
| **9** | ngay\_tao | DEFAULT CURRENT\_TIMESTAMP | DATETIME | Ngày khởi tạo tài khoản người dùng |

### Bảng Không gian làm việc (WORKSPACE)

|  |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **STT** | |  | | --- | | **Tên Trường** |  |  | | --- | |  | | **Ràng buộc**   |  | | --- | |  | | |  | | --- | | **Độ dài dữ liệu** |  |  | | --- | |  | | | **Mô tả** | | --- |  |  | | --- | |  | |
| **1** | ma\_workspace | PRIMARY KEY | VARCHAR(50) | Mã định danh không gian làm việc |
| **2** | ma\_tai\_khoan | FOREIGN KEY | VARCHAR(50) | Mã tài khoản của chủ sở hữu Workspace (Owner) |
| **3** | ten\_workspace | NOT NULL | NVARCHAR(100) | Tên của Không gian làm việc (Ví dụ: Công ty Tech) |
| **4** | mo\_ta | NULL | NVARCHAR(500) | Mô tả thông tin chung về không gian làm việc |
| **5** | ngay\_tao | DEFAULT CURRENT\_TIMESTAMP | DATETIME | Ngày tạo lập Workspace |

###

### Bảng Liên kết mời (INVITELINK)

|  |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **STT** | |  | | --- | | **Tên Trường** |  |  | | --- | |  | | **Ràng buộc**   |  | | --- | |  | | |  | | --- | | **Độ dài dữ liệu** |  |  | | --- | |  | | | **Mô tả** | | --- |  |  | | --- | |  | |
| **1** | ma\_link | PRIMARY KEY | VARCHAR(50) | Mã định danh liên kết mời |
| **2** | ma\_workspace | FOREIGN KEY | VARCHAR(50) | Thuộc về Workspace được mời tham gia |
| **3** | token | UNIQUE, NOT NULL | VARCHAR(255) | Chuỗi mã hóa bảo mật ngẫu nhiên đi kèm URL mời |
| **4** | vai\_tro\_mac\_dinh | NOT NULL | VARCHAR(50) | Vai trò được gán sẵn khi click link (Manager/Member) |
| **5** | ngay\_het\_han | NOT NULL | DATETIME | Thời hạn hiệu lực của đường dẫn liên kết mời |

### Bảng Dự án (DUAN)

|  |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **STT** | |  | | --- | | **Tên Trường** |  |  | | --- | |  | | **Ràng buộc**   |  | | --- | |  | | |  | | --- | | **Độ dài dữ liệu** |  |  | | --- | |  | | | **Mô tả** | | --- |  |  | | --- | |  | |
| **1** | ma\_du\_an | PRIMARY KEY | VARCHAR(50) | Mã định danh duy nhất của dự án |
| **2** | ma\_workspace | FOREIGN KEY | VARCHAR(50) | Thuộc không gian làm việc Workspace nào |
| **3** | ten\_du\_an | NOT NULL | NVARCHAR(150) | Tên gọi của dự án/chiến dịch |
| **4** | mo\_ta | NULL | NVARCHAR(MAX) | Bài viết đặc tả hoặc mô tả mục tiêu dự án |
| **5** | ngay\_bat\_dau | NULL | DATETIME | Ngày dự án chính thức bắt đầu |
| **6** | ngay\_ket\_thuc | NULL | DATETIME | Ngày dự kiến hoặc ngày hạn cuối phải hoàn thành |
| **7** | trang\_thai | NOT NULL | NVARCHAR(50) | Trạng thái dự án (ChuaThucHien, DangThucHien, DaHoanThanh) |

###

### Bảng Thành viên dự án (THANHVIEN)

|  |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **STT** | |  | | --- | | **Tên Trường** |  |  | | --- | |  | | **Ràng buộc**   |  | | --- | |  | | |  | | --- | | **Độ dài dữ liệu** |  |  | | --- | |  | | | **Mô tả** | | --- |  |  | | --- | |  | |
| **1** | ma\_thanh\_vien | PRIMARY KEY | VARCHAR(50) | Mã định danh thành viên thuộc dự án |
| **2** | ma\_du\_an | FOREIGN KEY | VARCHAR(50) | Liên kết đến bảng DUAN |
| **3** | ma\_tai\_khoan | FOREIGN KEY | VARCHAR(50) | Liên kết đến bảng TAIKHOAN |
| **4** | vai\_tro\_du\_an | NOT NULL | NVARCHAR(50) | Vai trò cụ thể trong dự án này (PM, Dev, Tester...) |
| **5** | ngay\_tham\_gia   |  | | --- | |  | | DEFAULT CURRENT\_TIMESTAMP | DATETIME | Ngày thành viên được gán tham gia vào dự án |

### Bảng Công việc (CONGVIEC)

|  |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **STT** | |  | | --- | | **Tên Trường** |  |  | | --- | |  | | **Ràng buộc**   |  | | --- | |  | | |  | | --- | | **Độ dài dữ liệu** |  |  | | --- | |  | | | **Mô tả** | | --- |  |  | | --- | |  | |
| **1** | ma\_cong\_viec | PRIMARY KEY | VARCHAR(50) | Mã định danh duy nhất của công việc |
| **2** | ma\_du\_an | FOREIGN KEY | VARCHAR(50) | Thuộc dự án (dự án cha) nào |
| **3** | ma\_thanh\_vien | FOREIGN KEY, NULL | VARCHAR(50) | Thành viên được phân công chịu trách nhiệm chính (Assignee) |
| **4** | ten\_cong\_viec | NOT NULL | NVARCHAR(255) | Tiêu đề hoặc tên của đầu việc cần xử lý |
| **5** | mo\_ta | NULL | NVARCHAR(MAX) | Nội dung chi tiết các yêu cầu thực hiện nhiệm vụ |
| **6** | deadline | NULL | DATETIME | Hạn chót bắt buộc phải hoàn thành công việc |
| **7** | muc\_do\_uu\_tien | NOT NULL | NVARCHAR(50) | Mức độ khẩn cấp (Thap, TrungBinh, Cao, KhanCap) |
| **8** | trang\_thai | NOT NULL | NVARCHAR(50) | Trạng thái công việc (To Do, In Progress, Review, Done) |
| **9** | ngay\_tao | DEFAULT CURRENT\_TIMESTAMP | DATETIME | Ngày khởi tạo đầu việc nhiệm vụ này |

### Bảng Bình luận (BINHLUAN)

|  |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **STT** | |  | | --- | | **Tên Trường** |  |  | | --- | |  | | **Ràng buộc**   |  | | --- | |  | | |  | | --- | | **Độ dài dữ liệu** |  |  | | --- | |  | | | **Mô tả** | | --- |  |  | | --- | |  | |
| **1** | ma\_binh\_luan | PRIMARY KEY | VARCHAR(50) | Mã định danh của dòng bình luận |
| **2** | ma\_cong\_viec | FOREIGN KEY | VARCHAR(50) | Bình luận được viết tại thẻ công việc nào |
| **3** | ma\_tai\_khoan | FOREIGN KEY | VARCHAR(50) | Tài khoản của nhân sự viết bình luận |
| **4** | noi\_dung | NOT NULL | NVARCHAR(MAX) | Nội dung văn bản thảo luận |
| **5** | thoi\_gian | DEFAULT CURRENT\_TIMESTAMP | DATETIME | Mốc thời gian chính xác khi gửi bình luận |

###

### Bảng Tệp đính kèm (TEPDINHKEM)

|  |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **STT** | |  | | --- | | **Tên Trường** |  |  | | --- | |  | | **Ràng buộc**   |  | | --- | |  | | |  | | --- | | **Độ dài dữ liệu** |  |  | | --- | |  | | | **Mô tả** | | --- |  |  | | --- | |  | |
| **1** | ma\_tep | PRIMARY KEY | VARCHAR(50) | Mã định danh của tệp đính kèm |
| **2** | ma\_cong\_viec | FOREIGN KEY | VARCHAR(50) | Tệp tin này được đính kèm vào công việc nào |
| **3** | ten\_tep | NOT NULL | NVARCHAR(255) | Tên gốc của tệp tin khi tải lên (Ví dụ: tailieu.pdf) |
| **4** | duong\_dan | NOT NULL | VARCHAR(500) | Đường dẫn (URL) lưu trữ tệp tin trên hệ thống |
| **5** | dung\_luong | NOT NULL | INT | Dung lượng của tệp tin (tính bằng đơn vị KB) |

###

### Bảng Thông báo (THONGBAO)

|  |  |  |  |  |  |  |  |  |  |  |  |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **STT** | |  | | --- | | **Tên Trường** |  |  | | --- | |  | | **Ràng buộc**   |  | | --- | |  | | |  | | --- | | **Độ dài dữ liệu** |  |  | | --- | |  | | | **Mô tả** | | --- |  |  | | --- | |  | |
| **1** | ma\_thong\_bao | PRIMARY KEY | VARCHAR(50) | Mã định danh thông báo hệ thống |
| **2** | ma\_tai\_khoan | FOREIGN KEY | VARCHAR(50) | Mã tài khoản người nhận thông báo trực tiếp |
| **3** | noi\_dung | NOT NULL | NVARCHAR(500) | Nội dung chi tiết của thông báo gửi đi |
| **4** | trang\_thai | NOT NULL | VARCHAR(50) | Trạng thái thông báo (ChuaDoc, DaDoc) |
| **5** | thoi\_gian | DEFAULT CURRENT\_TIMESTAMP | DATETIME | Thời điểm hệ thống gửi thông báo đi |

###

**Tài liệu tham khảo**

***Tài liệu sách, giáo trình***

1. Nguyễn Đức Anh, Lê Chí Luận, Phạm Thị Tố Nga, “*Công nghệ phần mềm*”, 2024, NXB Đại học Quốc gia Hà Nội.

**Link website:**
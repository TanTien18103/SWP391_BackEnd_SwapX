using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Constants
{
    public class ApplicationConstants
    {
        public const string KEYID_EXISTED = "KeyId {0} đã tồn tại.";
        public const string KeyId = "KeyId";
        public const string DUPLICATE = "Symtem_id is duplicated";
    }

    //Service (1-0-0)
    public class ResponseCodeConstants
    {
        public const string NOT_FOUND = "Not found!";
        public const string BAD_REQUEST = "Bad request!";
        public const string SUCCESS = "Success!";
        public const string FAILED = "Failed!";
        public const string EXISTED = "Existed!";
        public const string DUPLICATE = "Duplicate!";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public const string INVALID_INPUT = "Invalid input!";
        public const string REQUIRED_INPUT = "Input required!";
        public const string UNAUTHORIZED = "Unauthorized!";
        public const string FORBIDDEN = "Forbidden!";
        public const string EXPIRED = "Expired!";
        public const string VALIDATION_FAIL = "Validation fail!";
        public const string CONFLICT = "Conflict!";
    }

    //Controllers
    public class ResponseMessageConstantsCommon
    {
        public const string NOT_FOUND = "Không tìm thấy dữ liệu";
        public const string EXISTED = "Already existed!";
        public const string SUCCESS = "Thao tác thành công";
        public const string NO_DATA = "Không có dữ liệu trả về";
        public const string SERVER_ERROR = "Lỗi từ phía server vui lòng liên hệ đội ngũ phát triển";
        public const string DATE_WRONG_FORMAT = "Dữ liệu ngày không đúng định dạng yyyy-mm-dd";
        public const string DATA_NOT_ENOUGH = "Dữ liệu đưa vào không đầy đủ";
    }

    //Auth-Account
    public class ResponseMessageIdentity
    {
        public const string INVALID_USER = "Người dùng không tồn tại.";
        public const string UNAUTHENTICATED = "Không xác thực.";
        public const string UNAUTHENTICATED_OR_UNAUTHORIZED = "Người dùng chưa xác thực hoặc không có quyền truy cập.";
        public const string PASSWORD_NOT_MATCH = "Mật khẩu không giống nhau.";
        public const string NEW_PASSWORD_CANNOT_MATCH = "Mật khẩu mới không được trùng với mật khẩu cũ.";
        public const string PASSWORD_WRONG = "Mật khẩu không đúng.";
        public const string OLD_PASSWORD_WRONG = "Mật khẩu cũ không đúng.";
        public const string PASSWORD_INVALID = "Mật khẩu không hợp lệ.";
        public const string EXISTED_USER_NAME = "Người dùng đã tồn tại.";
        public const string EXISTED_ACCOUNT_NO = "Tài khoản ngân hàng đã tồn tại.";
        public const string EXISTED_EMAIL = "Email đã tồn tại.";
        public const string EXISTED_PHONE = "Số điện thoại đã tồn tại.";
        public const string TOKEN_INVALID_OR_EXPIRED = "Token không xác thực hoặc đã hết hạn.";
        public const string TOKEN_INVALID = "Token xác thực không hợp lệ.";
        public const string TOKEN_NOT_SEND = "Token xác thực không được cung cấp";
        public const string GOOGLE_TOKEN_INVALID = "Token Google không hợp lệ.";
        public const string EMAIL_VALIDATED = "Email đã được xác thực.";
        public const string PHONE_VALIDATED = "Số điện thoại đã được xác thực.";
        public const string ROLE_INVALID = "Role không xác thực.";
        public const string CLAIM_NOTFOUND = "Không tìm thấy claim.";
        public const string EXISTED_ROLE = "Role đã tồn tại.";
        public const string INCORRECT_EMAIL = "Email Không tìm thấy";
        public const string ACCOUNT_NOT_FOUND = "Tài khoản Không tìm thấy";

        public const string USERNAME_REQUIRED = "Tên người dùng không được để trống.";
        public const string NAME_REQUIRED = "Tên không được để trống.";
        public const string GENDER_REQUIRED = "Giới tính không được để trống.";
        public const string USERCODE_REQUIRED = "Mã người dùng không được để trống.";
        public const string PASSWORD_REQUIRED = "Mật khẩu không được để trống.";
        public const string PASSSWORD_LENGTH = "Mật khẩu phải có ít nhất 8 ký tự.";
        public const string CONFIRM_PASSWORD_REQUIRED = "Xác nhận mật khẩu không được để trống.";
        public const string EMAIL_REQUIRED = "Email không được để trống.";
        public const string PHONENUMBER_REQUIRED = "Số điện thoại không được để trống.";
        public const string PHONENUMBER_INVALID = "Số điện thoại không hợp lệ.";
        public const string PHONENUMBER_LENGTH = "Số điện thoại phải có chính xác 10 số.";
        public const string ROLES_REQUIRED = "Role không được để trống.";
        public const string USER_NOT_ALLOWED = "Bạn không có quyền truy cập vào mục này.";
        public const string SESSION_NOT_FOUND = "Không tìm thấy session.";
        public const string SESSION_INVALID = "Session không hợp lệ, hãy đăng nhập lại.";
        public const string EMAIL_VALIDATION_REQUIRED = "Vui lòng nhập mã OTP được gửi đến email của bạn để kích hoạt tài khoản.";
        public const string MASTER_NOT_FOUND = "Không tìm thấy Master";
        public const string UPDATE_MASTER_SUCCESS = "Cập nhật thông tin Master thành công";
        public const string INVALID_DOB_YEAR = "Ngày sinh không hợp lệ. Vui lòng kiểm tra lại năm sinh của bạn.";
        public const string FORGOT_PASSWORD_SUCCESS = "Xác nhận Otp đặt lại mật khẩu thành công! Vui lòng kiểm tra email để nhận mật khẩu mới.";
        public const string OTP_EXPIRED = "OTP đã hết hạn. Vui lòng yêu cầu mã OTP mới.";
        public const string OTP_INVALID = "Mã OTP không hợp lệ. Vui lòng kiểm tra lại mã OTP bạn đã nhập.";
        public const string EXISTED_USERNAME = "Username đã tồn tại.";
        public const string LOGOUT_SUCCESS = "Đăng xuất thành công.";
        public const string EMAIL_IN_USE = "Email đã được sử dụng bởi người dùng khác.";
        public const string DUPLICATED_EMAIL = "Email này đang được sử dụng bởi nhiều tài khoản Active.";

        public const string ACCOUNT_INACTIVE ="Tài khoản của bạn hiện không hoạt động.";
    }

    //Auth-Account Controllers
    public class ResponseMessageIdentitySuccess
    {
        public const string REGIST_USER_SUCCESS = "Đăng kí tài khoản thành công! Vui lòng kiểm tra email để xác thực tài khoản.";
        public const string VERIFY_PHONE_SUCCESS = "Xác thực số điện thoại thành công!";
        public const string VERIFY_EMAIL_SUCCESS = "Xác thực email thành công!";
        public const string FORGOT_PASSWORD_SUCCESS = "Yêu cầu đặt lại mật khẩu thành công! Vui lòng kiểm tra email để đặt lại mật khẩu.";
        public const string RESET_PASSWORD_SUCCESS = "Cấp lại mật khẩu thành công!";
        public const string CHANGE_PASSWORD_SUCCESS = "Đổi mật khẩu thành công!";
        public const string RESEND_EMAIL_SUCCESS = "Gửi lại email xác thực thành công!";
        public const string UPDATE_USER_SUCCESS = "Cập nhật thông tin người dùng thành công!";
        public const string DELETE_USER_SUCCESS = "Xóa người dùng thành công!";
        public const string ADD_ROLE_SUCCESS = "Thêm role thành công!";
        public const string UPDATE_ROLE_SUCCESS = "Cập nhật role thành công!";
        public const string DELETE_ROLE_SUCCESS = "Xóa role thành công!";
    }

    //For User (0-1-0)  
    public class ResponseMessageConstantsUser
    {
        public const string GET_USER_INFO_SUCCESS = "Lấy thông tin người dùng thành công";
        public const string USER_NOT_FOUND = "Không tìm thấy người dùng";
        public const string NOT_UPDATED_ELEMENT = "Người dùng chưa cập nhật thông tin mệnh";
        public const string USER_INACTIVE = "Tài khoản của bạn đã bị ngừng hoạt động";
        public const string USER_EXISTED = "Người dùng đã tồn tại";
        public const string ADD_USER_SUCCESS = "Thêm người dùng thành công";
        public const string UPDATE_USER_SUCCESS = "Cập nhật người dùng thành công";
        public const string DELETE_USER_SUCCESS = "Xóa người dùng thành công";
        public const string ADMIN_NOT_FOUND = "Không tìm thấy quản trị viên";
        public const string CUSTOMER_NOT_FOUND = "Không tìm thấy khách hàng";
        public const string CUSTOMER_INFO_NOT_FOUND = "Không tìm thấy thông tin khách hàng";
        public const string CUSTOMER_BANK_INFO_NOT_FOUND = "Không tìm thấy thông tin tài khoản ngân hàng của khách hàng";
        public const string USER_NOT_STAFF = "Người dùng không phải là nhân viên";
        public const string CREATE_STAFF_SUCCESS = "Tạo nhân viên thành công";
        public const string USER_NOT_CUSTOMER = "Người dùng không phải là khách hàng";
        public const string CREATE_ADMIN_SUCCESS = "Tạo quản trị viên thành công";
        public const string USERNAME_DUPLICATED = "Trùng tên đăng nhập";
        public const string INVALID_STATUS = "Trạng thái không hợp lệ";
        public const string UPDATE_STATUS_SUCCESS = "Cập nhật trạng thái người dùng thành công";
        public const string STATUS_NOT_CHANGED = "Trạng thái người dùng không thay đổi";
        public const string CANNOT_CHANGE_ADMIN_STATUS = "Không thể thay đổi trạng thái của quản trị viên";
        public const string EVDRIVER_NOT_FOUND = "Không tìm thấy tài xế";
    }
    public class ResponseMessageImage
    {
        public const string INVALID_IMAGE = "Hình ảnh không hợp lệ. ";
        public const string INVALID_SIZE = "Kích thước hình ảnh không hợp lệ. ";
        public const string INVALID_FORMAT = "Định dạng hình ảnh không hợp lệ. ";
        public const string INVALID_URL = "Đường dẫn hình ảnh không hợp lệ. ";
    }

    //For Station (0-0-1)
    public class ResponseMessageConstantsStation
    {
        public const string STATION_NOT_FOUND = "Không tìm thấy trạm";
        public const string STATION_EXISTED = "Trạm đã tồn tại";
        public const string ADD_STATION_SUCCESS = "Thêm trạm thành công";
        public const string UPDATE_STATION_SUCCESS = "Cập nhật trạm thành công";
        public const string DELETE_STATION_SUCCESS = "Xóa trạm thành công";
        public const string STATION_INACTIVE = "Trạm đã bị ngừng hoạt động";
        public const string GET_STATION_LIST_SUCCESS = "Lấy danh sách trạm thành công";
        public const string GET_STATION_DETAIL_SUCCESS = "Lấy chi tiết trạm thành công";
        public const string ADD_STATION_FAIL = "Thêm trạm thất bại";
        public const string GET_ALL_STATION_FAIL = "Lấy danh sách trạm thất bại";
        public const string STATION_LIST_EMPTY = "Lấy danh sách trạm thất bại";
        public const string GET_ALL_STATION_SUCCESS = "Lấy danh sách trạm thành công";
        public const string UPDATE_STATION_FAILED = "Cập nhật trạm thất bại";
        public const string GET_STATION_FAIL = "Lấy trạm thất bại";
        public const string GET_STATION_SUCCESS = "Lấy trạm thành công";
        public const string DELETE_STATION_FAILED = "Xóa trạm thất bại";
        public const string STAFF_ALREADY_ASSIGNED_TO_ANOTHER_STATION = "Nhân viên đã được phân công cho một trạm khác.";
        public const string ADD_STAFF_TO_STATION_SUCCESS = "Thêm nhân viên vào trạm thành công";
        public const string ADD_STAFF_TO_STATION_FAILED = "Thêm nhân viên vào trạm thất bại";
        public const string STAFF_LIST_EMPTY = "Trạm chưa có nhân viên nào";
        public const string GET_STAFFS_BY_STATION_SUCCESS = "Lấy danh sách nhân viên của trạm thành công";
        public const string GET_STAFFS_BY_STATION_FAILED = "Lấy danh sách nhân viên của trạm thất bại";
        public const string STAFF_NOT_FOUND_IN_STATION = "Nhân viên không thuộc trạm này";
        public const string REMOVE_STAFF_FROM_STATION_FAILED = "Xóa nhân viên khỏi trạm thất bại";
        public const string REMOVE_STAFF_FROM_STATION_SUCCESS = "Xóa nhân viên khỏi trạm thành công";

        public const string GET_STATION_BY_STAFF_SUCCESS = "Lấy trạm theo nhân viên thành công";
        public const string STAFF_NOT_ASSIGNED_TO_ANY_STATION = "Nhân viên chưa được phân công cho trạm nào.";
        public const string GET_STATION_BY_STAFF_FAILED = "Lấy trạm theo nhân viên thất bại";
        public const string STAFF_ALREADY_ASSIGNED_TO_THIS_STATION = "Nhân viên đã được phân công cho trạm này.";

        public static string CANNOT_CHANGE_STATUS_DUE_TO_TODAY_SCHEDULE { get; set; }
        public static string STATION_ALREADY_IN_THIS_STATUS { get; set; }
        public static string INVALID_STATION_STATUS { get; set; }
        public static string UPDATE_STATION_STATUS_SUCCESS { get; set; }
        public static string UPDATE_STATION_STATUS_FAILED { get; set; }
    }
    // For battery (0-0-1)
    public class ResponseMessageConstantsBattery
    {
        public const string BATTERY_NOT_FOUND = "Không tìm thấy pin";
        public const string BATTERY_EXISTED = "Pin đã tồn tại";
        public const string ADD_BATTERY_SUCCESS = "Thêm pin thành công";
        public const string UPDATE_BATTERY_SUCCESS = "Cập nhật pin thành công";
        public const string DELETE_BATTERY_SUCCESS = "Xóa pin thành công";
        public const string BATTERY_INACTIVE = "Pin đã bị ngừng hoạt động";
        public const string GET_BATTERY_LIST_SUCCESS = "Lấy danh sách pin thành công";
        public const string GET_BATTERY_DETAIL_SUCCESS = "Lấy chi tiết pin thành công";
        public const string DefaultBatterySuffix = "'s Battery";
        public const string BATTERY_STATUS_ALREADY_EXISTS = "Trạng thái pin không thay đổi";
        public const string UPDATE_BATTERY_STATUS_IN_STATION_SUCCESS = "Cập nhật trạng thái pin trong trạm thành công";
        public const string UPDATE_BATTERY_STATUS_IN_STATION_FAILED = "Cập nhật trạng thái pin trong trạm thất bại";
        public const string BATTERY_DECOMMISSIONED_CANNOT_UPDATE_STATUS = "Pin đã ngừng hoạt động, không thể cập nhật trạng thái";
        public static string ADD_BATTERY_FAIL = "Thêm pin thất bại";
        public static string GET_ALL_BATTERY_FAIL = "Lấy danh sách pin thất bại";
        public static string BATTERY_LIST_EMPTY = "Lấy danh sách pin thất bại";
        public static string GET_ALL_BATTERY_SUCCESS = "Lấy danh sách pin thành công";
        public static string UPDATE_BATTERY_FAILED = "Cập nhật pin thất bại";
        public static string GET_BATTERY_FAIL = "Lấy pin thất bại";
        public static string GET_BATTERY_SUCCESS = "Lấy pin thành công";
        public static string DELETE_BATTERY_FAILED = "Xóa pin thất bại";
        public static string ADD_BATTERY_IN_STATION_FAILED = "Thêm pin vào trạm thất bại";
        public static string ADD_BATTERY_IN_STATION_SUCCESS = "Thêm pin vào trạm thành công";
        public const string GET_BATTERY_HISTORY_SUCCESS = "Lấy lịch sử pin thành công";
        public const string GET_BATTERY_HISTORY_FAIL = "Lấy lịch sử pin thất bại";
        public const string BATTERY_HISTORY_EMPTY = "Không có lịch sử cho pin này";
        public const string UPDATE_BATTERY_STATUS_SUCCESS = "Cập nhật trạng thái pin thành công";
        public const string UPDATE_BATTERY_STATUS_FAIL = "Cập nhật trạng thái pin thất bại";

    }
    public static class EmailConstants
    {
        // OTP
        public const string OtpSubject = "Otp xác nhận đổi mật khẩu mới của bạn";
        public const string OtpBodyTemplate = "Otp xác nhận đổi mật khẩu mới của bạn là: <b>{0}</b>. Hãy nhập otp để nhận mật khẩu mới ngay.";

        // New Password
        public const string NewPasswordSubject = "Mật khẩu mới của bạn";
        public const string NewPasswordBodyTemplate = "Mật khẩu mới của bạn là: <b>{0}</b>. Hãy đăng nhập và đổi mật khẩu ngay.";
    }
    public static class HistoryActionConstants { 
    
    
    
    
    }
    public static class ResponseMessageConstantsForm
    {
        public const string FORM_NOT_FOUND = "Không tìm thấy biểu mẫu";
        public const string FORM_EXISTED = "Biểu mẫu đã tồn tại";
        public const string ADD_FORM_SUCCESS = "Thêm biểu mẫu thành công";
        public const string UPDATE_FORM_SUCCESS = "Cập nhật biểu mẫu thành công";
        public const string DELETE_FORM_SUCCESS = "Xóa biểu mẫu thành công";
        public const string FORM_INACTIVE = "Biểu mẫu đã bị ngừng hoạt động";
        public const string GET_FORM_LIST_SUCCESS = "Lấy danh sách biểu mẫu thành công";
        public const string GET_FORM_DETAIL_SUCCESS = "Lấy chi tiết biểu mẫu thành công";
        public const string ADD_FORM_FAIL = "Thêm biểu mẫu thất bại";
        public const string GET_ALL_FORM_FAIL = "Lấy danh sách biểu mẫu thất bại";
        public const string FORM_LIST_EMPTY = "Lấy danh sách biểu mẫu thất bại";
        public const string GET_ALL_FORM_SUCCESS = "Lấy danh sách biểu mẫu thành công";
        public const string UPDATE_FORM_FAILED = "Cập nhật biểu mẫu thất bại";
        public const string GET_FORM_FAIL = "Lấy biểu mẫu thất bại";
        public const string GET_FORM_SUCCESS = "Lấy biểu mẫu thành công";
        public const string DELETE_FORM_FAILED = "Xóa biểu mẫu thất bại";
        public const string INVALID_FORM_DATE = "Ngày biểu mẫu không hợp lệ";
        public const string INVALID_FORM_TIME = "Thời gian không nằm trong giờ hành chính (7h30–12h, 13h30–17h).";
        public const string INVALID_FORM_STATUS_VALUE = "Trạng thái biểu mẫu không hợp lệ";
        public const string INVALID_FORM_STATUS_UPDATE = "Không thể cập nhật trạng thái biểu mẫu";
        public const string UPDATE_FORM_STATUS_SUCCESS = " Cập nhật trạng thái biểu mẫu thành công";
        public const string UPDATE_FORM_STATUS_FAILED = " Cập nhật trạng thái biểu mẫu thất bại";
    }
    // For Package (0-0-1)
    public static class ResponseMessageConstantsPackage
    {
        public const string PACKAGE_NOT_FOUND = "Không tìm thấy gói";
        public const string PACKAGE_EXISTED = "Gói đã tồn tại";
        public const string ADD_PACKAGE_SUCCESS = "Thêm gói thành công";
        public const string UPDATE_PACKAGE_SUCCESS = "Cập nhật gói thành công";
        public const string DELETE_PACKAGE_SUCCESS = "Xóa gói thành công";
        public const string PACKAGE_INACTIVE = "Gói đã bị ngừng hoạt động";
        public const string GET_PACKAGE_LIST_SUCCESS = "Lấy danh sách gói thành công";
        public const string GET_PACKAGE_DETAIL_SUCCESS = "Lấy chi tiết gói thành công";
        public const string ADD_PACKAGE_FAIL = "Thêm gói thất bại";
        public const string GET_ALL_PACKAGE_FAIL = "Lấy danh sách gói thất bại";
        public const string PACKAGE_LIST_EMPTY = "Lấy danh sách gói thất bại";
        public const string GET_ALL_PACKAGE_SUCCESS = "Lấy danh sách gói thành công";
        public const string UPDATE_PACKAGE_FAILED = "Cập nhật gói thất bại";
        public const string GET_PACKAGE_FAIL = "Lấy gói thất bại";
        public const string GET_PACKAGE_SUCCESS = "Lấy gói thành công";
        public const string DELETE_PACKAGE_FAILED = "Xóa gói thất bại";

        public const string GET_PACKAGE_BY_BATTERY_TYPE_FAIL =" Lấy gói theo loại pin thất bại";
        public const string GET_PACKAGE_BY_BATTERY_TYPE_SUCCESS = "Lấy gói theo loại pin thành công";

        public const string PACKAGE_STATUS_SAME = "Trạng thái gói không thay đổi";
        public const string UPDATE_PACKAGE_STATUS_SUCCESS = "Cập nhật trạng thái gói thành công";
        public const string PACKAGE_IN_USE_CANNOT_INACTIVE = "Gói đang được sử dụng, không thể ngừng hoạt động";
        public const string UPDATE_PACKAGE_STATUS_FAILED = "Cập nhật trạng thái gói thất bại";
    }
    //for Vehicle (0-0-1)
    public static class ResponseMessageConstantsVehicle
    {
        public const string VEHICLE_NOT_FOUND = "Không tìm thấy xe";
        public const string VEHICLE_EXISTED = "Xe đã tồn tại";
        public const string ADD_VEHICLE_SUCCESS = "Thêm xe thành công";
        public const string UPDATE_VEHICLE_SUCCESS = "Cập nhật xe thành công";
        public const string DELETE_VEHICLE_SUCCESS = "Xóa xe thành công";
        public const string VEHICLE_INACTIVE = "Xe đã bị ngừng hoạt động";
        public const string GET_VEHICLE_LIST_SUCCESS = "Lấy danh sách xe thành công";
        public const string GET_VEHICLE_DETAIL_SUCCESS = "Lấy chi tiết xe thành công";
        public const string ADD_VEHICLE_FAIL = "Thêm xe thất bại";
        public const string GET_ALL_VEHICLE_FAIL = "Lấy danh sách xe thất bại";
        public const string VEHICLE_LIST_EMPTY = "Lấy danh sách xe thất bại";
        public const string GET_ALL_VEHICLE_SUCCESS = "Lấy danh sách xe thành công";
        public const string UPDATE_VEHICLE_FAILED = "Cập nhật xe thất bại";
        public const string GET_VEHICLE_FAIL = "Lấy xe thất bại";
        public const string GET_VEHICLE_SUCCESS = "Lấy xe thành công";
        public const string DELETE_VEHICLE_FAILED = "Xóa xe thất bại";
        public const string GET_VEHICLES_BY_PACKAGE_ID_FAILED = "Lấy xe theo gói thất bại";
        public const string GET_VEHICLES_BY_PACKAGE_ID_SUCCESS = "Lấy xe theo gói thành công";

        public const string LINK_VEHICLE_FAILED = "Liên kết xe thất bại";
        public const string LINK_VEHICLE_SUCCESS = "Liên kết xe thành công";

        public const string ADD_VEHICLE_IN_PACKAGE_SUCCESS = "Thêm xe vào gói thành công";
        public const string ADD_VEHICLE_IN_PACKAGE_FAILED = "Thêm xe vào gói thất bại";

        public const string DELETE_VEHICLE_IN_PACKAGE_FAILED = "Hủy xe khỏi gói thất bại";
        public const string DELETE_VEHICLE_IN_PACKAGE_SUCCESS = "Hủy xe khỏi gói thành công";

        public const string VEHICLE_ALREADY_EXISTS = "Xe đã tồn tại trong hệ thống";

        public const string UNLINK_VEHICLE_SUCCESS = "Hủy liên kết xe thành công";
        public const string UNLINK_VEHICLE_FAILED = "Hủy liên kết xe thất bại";

        public const string VEHICLE_ALREADY_IN_PACKAGE = "Xe đã có gói";

        public const string GET_PACKAGE_BY_VEHICLE_ID_SUCCESS =" Lấy gói theo xe thành công";
        public const string GET_PACKAGE_BY_VEHICLE_ID_FAILED = "Lấy gói theo xe thất bại";

        public const string VEHICLE_NOT_OWNED = "Xe không thuộc sở hữu của khách hàng";

        public const string NO_VEHICLE_FOR_USER = "Người dùng không có xe được liên kết";
    }
    //for StationSchedule (0-0-1)
    public static class ResponseMessageConstantsStationSchedule
    {
        public const string STATION_SCHEDULE_NOT_FOUND = "Không tìm thấy lịch trạm";
        public const string STATION_SCHEDULE_EXISTED = "Lịch trạm đã tồn tại";
        public const string ADD_STATION_SCHEDULE_SUCCESS = "Thêm lịch trạm thành công";
        public const string UPDATE_STATION_SCHEDULE_SUCCESS = "Cập nhật lịch trạm thành công";
        public const string DELETE_STATION_SCHEDULE_SUCCESS = "Xóa lịch trạm thành công";
        public const string STATION_SCHEDULE_INACTIVE = "Lịch trạm đã bị ngừng hoạt động";
        public const string GET_STATION_SCHEDULE_LIST_SUCCESS = "Lấy danh sách lịch trạm thành công";
        public const string GET_STATION_SCHEDULE_DETAIL_SUCCESS = "Lấy chi tiết lịch trạm thành công";
        public const string ADD_STATION_SCHEDULE_FAIL = "Thêm lịch trạm thất bại";
        public const string GET_ALL_STATION_SCHEDULE_FAIL = "Lấy danh sách lịch trạm thất bại";
        public const string STATION_SCHEDULE_LIST_EMPTY = "Lấy danh sách lịch trạm thất bại";
        public const string GET_ALL_STATION_SCHEDULE_SUCCESS = "Lấy danh sách lịch trạm thành công";
        public const string UPDATE_STATION_SCHEDULE_FAILED = "Cập nhật lịch trạm thất bại";
        public const string GET_STATION_SCHEDULE_FAIL = "Lấy lịch trạm thất bại";
        public const string GET_STATION_SCHEDULE_SUCCESS = "Lấy lịch trạm thành công";
        public const string DELETE_STATION_SCHEDULE_FAILED = "Xóa lịch trạm thất bại";
        public const string INVALID_TIME_RANGE = "Khoảng thời gian không hợp lệ";

        public static string GET_STATION_SCHEDULE_BY_STATION_ID_SUCCESS = "Lấy lịch trạm theo ID trạm thành công";
        public static string GET_STATION_SCHEDULE_BY_STATION_ID_FAILED = "Lấy lịch trạm theo ID trạm thất bại";
    }
    //for Rating (0-0-1)
    public static class ResponseMessageConstantsRating
    {
        public const string RATING_NOT_FOUND = "Không tìm thấy đánh giá";
        public const string RATING_EXISTED = "Đánh giá đã tồn tại";
        public const string ADD_RATING_SUCCESS = "Thêm đánh giá thành công";
        public const string UPDATE_RATING_SUCCESS = "Cập nhật đánh giá thành công";
        public const string DELETE_RATING_SUCCESS = "Xóa đánh giá thành công";
        public const string RATING_INACTIVE = "Đánh giá đã bị ngừng hoạt động";
        public const string GET_RATING_LIST_SUCCESS = "Lấy danh sách đánh giá thành công";
        public const string GET_RATING_DETAIL_SUCCESS = "Lấy chi tiết đánh giá thành công";
        public const string ADD_RATING_FAIL = "Thêm đánh giá thất bại";
        public const string GET_ALL_RATING_FAIL = "Lấy danh sách đánh giá thất bại";
        public const string RATING_LIST_EMPTY = "Lấy danh sách đánh giá thất bại";
        public const string GET_ALL_RATING_SUCCESS = "Lấy danh sách đánh giá thành công";
        public const string UPDATE_RATING_FAILED = "Cập nhật đánh giá thất bại";
        public const string GET_RATING_FAIL = "Lấy đánh giá thất bại";
        public const string GET_RATING_SUCCESS = "Lấy đánh giá thành công";
        public const string DELETE_RATING_FAILED = "Xóa đánh giá thất bại";
    }
    //for Report (0-0-1)
    public static class ResponseMessageConstantsReport
    {
        public const string REPORT_NOT_FOUND = "Không tìm thấy báo cáo";
        public const string REPORT_EXISTED = "Báo cáo đã tồn tại";
        public const string ADD_REPORT_SUCCESS = "Thêm báo cáo thành công";
        public const string UPDATE_REPORT_SUCCESS = "Cập nhật báo cáo thành công";
        public const string DELETE_REPORT_SUCCESS = "Xóa báo cáo thành công";
        public const string REPORT_INACTIVE = "Báo cáo đã bị ngừng hoạt động";
        public const string GET_REPORT_LIST_SUCCESS = "Lấy danh sách báo cáo thành công";
        public const string GET_REPORT_DETAIL_SUCCESS = "Lấy chi tiết báo cáo thành công";
        public const string ADD_REPORT_FAIL = "Thêm báo cáo thất bại";
        public const string GET_ALL_REPORT_FAIL = "Lấy danh sách báo cáo thất bại";
        public const string REPORT_LIST_EMPTY = "Lấy danh sách báo cáo thất bại";
        public const string GET_ALL_REPORT_SUCCESS = "Lấy danh sách báo cáo thành công";
        public const string UPDATE_REPORT_FAILED = "Cập nhật báo cáo thất bại";
        public const string GET_REPORT_FAIL = "Lấy báo cáo thất bại";
        public const string GET_REPORT_SUCCESS = "Lấy báo cáo thành công";
        public const string DELETE_REPORT_FAILED = "Xóa báo cáo thất bại";
    }
    //for BatteryReport (0-0-1)
    public static class ResponseMessageConstantsBatteryReport
    {
        public const string BATTERY_REPORT_NOT_FOUND = "Không tìm thấy báo cáo pin";
        public const string BATTERY_REPORT_EXISTED = "Báo cáo pin đã tồn tại";
        public const string ADD_BATTERY_REPORT_SUCCESS = "Thêm báo cáo pin thành công";
        public const string UPDATE_BATTERY_REPORT_SUCCESS = "Cập nhật báo cáo pin thành công";
        public const string DELETE_BATTERY_REPORT_SUCCESS = "Xóa báo cáo pin thành công";
        public const string BATTERY_REPORT_INACTIVE = "Báo cáo pin đã bị ngừng hoạt động";
        public const string GET_BATTERY_REPORT_LIST_SUCCESS = "Lấy danh sách báo cáo pin thành công";
        public const string GET_BATTERY_REPORT_DETAIL_SUCCESS = "Lấy chi tiết báo cáo pin thành công";
        public const string ADD_BATTERY_REPORT_FAIL = "Thêm báo cáo pin thất bại";
        public const string GET_ALL_BATTERY_REPORT_FAIL = "Lấy danh sách báo cáo pin thất bại";
        public const string BATTERY_REPORT_LIST_EMPTY = "Lấy danh sách báo cáo pin thất bại";
        public const string GET_ALL_BATTERY_REPORT_SUCCESS = "Lấy danh sách báo cáo pin thành công";
        public const string UPDATE_BATTERY_REPORT_FAILED = "Cập nhật báo cáo pin thất bại";
        public const string GET_BATTERY_REPORT_FAIL = "Lấy báo cáo pin thất bại";
        public const string GET_BATTERY_REPORT_SUCCESS = "Lấy báo cáo pin thành công";
        public const string DELETE_BATTERY_REPORT_FAILED = "Xóa báo cáo pin thất bại";
    }
    
    public static class ExchangeMessages
    {
        public const string CreateSuccess = "Tạo giao dịch đổi pin thành công.";
        public const string CreateFailed = "Không thể tạo giao dịch đổi pin.";

        public const string NotFound = "Không tìm thấy thông tin giao dịch đổi pin.";
        public const string ListEmpty = "Không có bản ghi giao dịch đổi pin nào.";

        public const string InvalidStation = "Mã trạm không hợp lệ.";
        public const string InvalidOrder = "Mã đơn hàng không hợp lệ.";
        public const string InvalidBattery = "Thông tin pin không hợp lệ.";

        public const string UpdateSuccess = "Cập nhật giao dịch đổi pin thành công.";
        public const string UpdateFailed = "Cập nhật giao dịch đổi pin thất bại.";

        public const string DeleteSuccess = "Xóa giao dịch đổi pin thành công.";
        public const string DeleteFailed = "Xóa giao dịch đổi pin thất bại.";

        public const string PermissionDenied = "Bạn không có quyền thực hiện hành động này.";
        public const string UnexpectedError = "Đã xảy ra lỗi không mong muốn trong quá trình xử lý giao dịch đổi pin.";
    }
}

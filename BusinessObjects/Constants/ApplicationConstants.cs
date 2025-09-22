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
    }
}

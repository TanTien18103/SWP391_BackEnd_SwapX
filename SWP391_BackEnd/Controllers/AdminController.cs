using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SWP391_BackEnd.Controllers
{
    
    [Route("api/[controller]")]    
    public class AdminController : ControllerBase   
    {
        private readonly SwapXContext _context;
        public AdminController(SwapXContext context)
        {
            _context = context;
        }

        // GET: api/Admin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _context.Accounts.ToListAsync();
        }

        // GET: api/Admin/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(string id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return NotFound(new { message = "Không tìm thấy tài khoản" });
            return account;
        }

        // POST: api/Admin
        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount([FromBody] Account account)
        {
            if (await _context.Accounts.AnyAsync(a => a.Username == account.Username))
            {
                return BadRequest(new { message = "Username đã tồn tại" });
            }

            account.StartDate = DateTime.Now;
            account.UpdateDate = DateTime.Now;
            // TODO: mã hoá mật khẩu trước khi lưu (bcrypt hoặc hashing)

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAccount), new { id = account.AccountId }, account);
        }

        // PUT: api/Admin/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(string id, [FromBody] Account account)
        {
            if (id != account.AccountId) return BadRequest(new { message = "ID không khớp" });

            var acc = await _context.Accounts.FindAsync(id);
            if (acc == null) return NotFound(new { message = "Không tìm thấy tài khoản" });

            // Cập nhật thông tin
            acc.Role = account.Role;
            acc.Username = account.Username;
            acc.Password = account.Password; // nên hash trước khi lưu
            acc.Name = account.Name;
            acc.Phone = account.Phone;
            acc.Address = account.Address;
            acc.Email = account.Email;
            acc.Status = account.Status;
            acc.UpdateDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công" });
        }

        // DELETE: api/Admin/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var acc = await _context.Accounts.FindAsync(id);
            if (acc == null) return NotFound(new { message = "Không tìm thấy tài khoản" });

            _context.Accounts.Remove(acc);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa thành công" });
        }
    }
}


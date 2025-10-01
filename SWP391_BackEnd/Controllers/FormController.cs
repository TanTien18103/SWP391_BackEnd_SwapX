using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.ApiModels.Form;
using Services.Services.FormService;

namespace SWP391_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        private readonly IFormService _formService;
        public FormController(IFormService formService)
        {
            _formService = formService;
        }

        [HttpPost("create-form")]
        [Authorize(Roles = "EvDriver")]
        public async Task<IActionResult> CreateForm([FromForm] AddFormRequest addFormRequest)
        {
            var res = await _formService.AddForm(addFormRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-form-by-id/{formId}")]
        [Authorize]
        public async Task<IActionResult> GetFormById(string formId)
        {
            var res = await _formService.GetFormById(formId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-all-forms")]
        [Authorize]
        public async Task<IActionResult> GetAllForms()
        {
            var res = await _formService.GetAllForms();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-forms-by-account-id/{accountId}")]
        [Authorize]
        public async Task<IActionResult> GetFormsByAccountId(string accountId)
        {
            var res = await _formService.GetFormsByAccountId(accountId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-forms-by-station-id/{stationId}")]
        [Authorize]

        public async Task<IActionResult> GetFormsByStationId(string stationId)
        {
            var res = await _formService.GetFormsByStationId(stationId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("update-form")]
        [Authorize]
        public async Task<IActionResult> UpdateForm([FromForm] UpdateFormRequest updateFormRequest)
        {
            var res = await _formService.UpdateForm(updateFormRequest);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("delete-form/{formId}")]
        [Authorize]
        public async Task<IActionResult> DeleteForm(string formId)
        {
            var res = await _formService.DeleteForm(formId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-form-by-id-driver/{formId}")]
        [Authorize(Roles = "EvDriver")]
        public async Task<IActionResult> GetFormByIdDriver(string formId)
        {
            var res = await _formService.GetFormByIdDriver(formId);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("get-all-forms-driver")]
        [Authorize(Roles = "EvDriver")]
        public async Task<IActionResult> GetAllFormsDriver()
        {
            var res = await _formService.GetAllFormsDriver();
            return StatusCode(res.StatusCode, res);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using SSD_Lab1.Models;
using System.Collections.Generic;

namespace SSD_Lab1.Controllers
{
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Success")
        {
            return Ok(new ApiResponse<T> { Success = true, Message = message, Data = data });
        }

        protected IActionResult Error(string message, List<string> errors = null)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            });
        }
    }
}
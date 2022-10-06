using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Demo.Entities;
using Demo.IService;
using Demo.ViewModel;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;

        public UserController(IUserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // GET: api/User
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                IEnumerable<Users> users = await _service.GetAll();
                if (users == null)
                {
                    return NoContent();
                }
                return Ok(_mapper.Map<IEnumerable<SigninVM>>(users));
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return StatusCode(StatusCodes.Status403Forbidden, exp);
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                Users User = await _service.GetById(id);
                if (User == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<SigninVM>(User));
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return StatusCode(StatusCodes.Status403Forbidden, exp.Message);
            }
        }

        // POST: api/User
   

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Users user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { ModelState });
                }

                if (id != user.Id)
                {
                    return BadRequest();
                }

                if (_service.GetById(id) == null)
                {
                    return NotFound();
                }

                user.EditedDate = DateTime.Now;
                bool status = await _service.Update(user);

                if (status == false)
                {
                    return Forbid();
                }
                return StatusCode(StatusCodes.Status201Created, new { message = "User putet" });
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return StatusCode(StatusCodes.Status403Forbidden, exp.Message);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                Users user = await _service.GetById(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.DeletedDate = DateTime.Now;
                user.IsDeleted = true;
                bool status = await _service.Delete(user);

                if (status == false)
                {
                    return Forbid();
                }
                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
                return StatusCode(StatusCodes.Status403Forbidden, exp.Message);
            }
        }
    }
}

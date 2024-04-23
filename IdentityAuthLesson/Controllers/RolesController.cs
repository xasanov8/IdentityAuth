using IdentityAuthLesson.DTOs;
using IdentityAuthLesson.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace IdentityAuthLesson.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDTO>> CreateRole(RoleDTO role)
        {
            
            var result = await _roleManager.FindByNameAsync(role.RoleName);
                
            if (result == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role.RoleName));

                return Ok(new ResponseDTO
                {
                    Message = "Role Created",
                    IsSuccess = true,
                    StatusCode = 201
                });
            }

            return Ok(new ResponseDTO
            {
                Message = "Role cann not created",
                StatusCode = 403
            });
        }


        [HttpGet]
        public async Task<ActionResult<List<IdentityRole>>> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return Ok(roles);
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role is null)
            {
                return Unauthorized("Role not Found with this id");
            }

            return Ok(role);
        }

        [HttpPut]
        public async Task<ActionResult<string>> Update(string id, string updateRoleName)
        {
            var result = await _roleManager.FindByNameAsync(updateRoleName);

            if (result == null)
            {
                var role = await _roleManager.FindByIdAsync(id);

                if (role is null)
                {
                    return Unauthorized("Role not Found with this id");
                }

                role.Name = updateRoleName;

                var updateRole = await _roleManager.UpdateAsync(role);

                if (!updateRole.Succeeded)
                {
                    return BadRequest(updateRole.Errors);
                }

                return Ok(updateRole);
            }

            return Ok(new ResponseDTO
            {
                Message = "Role cann not created",
                StatusCode = 403
            });
        }


        [HttpDelete]
        public async Task<ActionResult<string>> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role is null)
            {
                return Unauthorized("Role not Found with this id");
            }

            var deleteRole = await _roleManager.DeleteAsync(role);

            if (!deleteRole.Succeeded)
            {
                return BadRequest(deleteRole.Errors);
            }

            return Ok(deleteRole);
        }
    }
}

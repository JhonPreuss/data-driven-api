using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("users")]
    public class UserController:Controller
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get(
            [FromServices] DataContext context
        )
        {
            var users = await context
                .Users
                .AsNoTracking()
                .ToListAsync();
            return users;
        }

        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]--sem usuarios n base n dá pra criar
        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody] User model
        )
        {
            if (!ModelState.IsValid)//Testando se a model
                return BadRequest(ModelState);
            
            try{   
                model.Role = "employee";
                context.Users.Add(model);
                await context.SaveChangesAsync();

                //oculta a senha para o retorno
                model.Password = "";
                return model;
            }
            catch(System.Exception){
                return BadRequest(new {message = " Não foi possível criar o usuário"});
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Put(
            [FromServices] DataContext context,
            int id,
            [FromBody] User model
        )
        {
            if (!ModelState.IsValid)//Testando se a model do produto tá ok
                BadRequest(ModelState);

            if (id != model.Id)
                return NotFound(new {message = " Não foi possível criar o usuário"});
                
            try
            {
                context.Users.Add(model);
                await context.SaveChangesAsync();
                return model;
                
            }
            catch (System.Exception)
            {
                return BadRequest(new {message = " Não foi possível criar o usuário"});
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate(
            [FromServices] DataContext context,
            [FromBody] User model
        )
        {
            var user = await context.Users
            .AsNoTracking()
            .Where(x => model.UserName == x.UserName && model.Password == x.Password)
            .FirstOrDefaultAsync();

            if ( user == null)
                return NotFound (new { message = "Usuário ou senha inválidos"});

            
            user.Password ="";
            var token = TokenService.GenerateToken(user);
            return new
            {
                user = user, 
                token = token
            };
        }

    }
}
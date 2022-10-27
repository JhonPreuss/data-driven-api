using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;


namespace Shop.Controllers
{
    [Route("Products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Product>>> Get(
            [FromServices] DataContext context
        )
        {
            var products = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .ToListAsync();
                //podemos ter mais de um include nesse tipo de contexto, adicionando mais de uma tabela para consultas
                //Ele dá um join em todas as categorias /seleção em duas tabelas
            return products;
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> Get(
            int id,
            [FromServices] DataContext context
        )
        {
            var product = await context
                .Products
                .Include(x=>x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
            return Ok(product);
        }

        [HttpGet]//products/cagories/{id}
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(
            int id,
            [FromServices] DataContext context
        )
        {
            var products = await context
                .Products
                .Include(x=>x.Category)
                .AsNoTracking()
                .Where(x=> x.CategoryId == id)
                .ToListAsync();
            return products;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        //o comando dotnet watch  run, a cada alteração ele reestarda automatico
        public async Task<ActionResult<Product>> Post(
            [FromServices] DataContext context,
            [FromBody] Product model
        )
        {
            if (ModelState.IsValid)//Testando se a model do produto tá ok
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
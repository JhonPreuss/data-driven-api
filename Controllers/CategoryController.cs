using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]//Decorar o método com o verbo da requisição
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        //[ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.(outras opções de local), Duration = 30)]
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] -> Opçaõ de cache direto no builder, e apenas alguns métodos n guardar cache
        public async Task<ActionResult<List<Category>>> Get(
            [FromServices]DataContext context
        )
        {
            //sempre que der .ToList é no final, pq ele executa a query
            //AsNoTraking-> só busca no banco e entrega, caso contrario o EF cria 
            //mecanismos de monitoramento da entidade
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }
        
        [HttpGet]
        [Route("{id:int}")]//restrição na rota
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(
            int id,
            [FromServices]DataContext context
        )
        {
            var categories = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(categories);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Post(
            [FromBody]Category model,
            [FromServices]DataContext context
        )
        //public string Post(Category model)
        { 
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                //persistir 
                await context.SaveChangesAsync();//gera o id e retora na model
                return Ok(model);
            }
            catch
            {
                return BadRequest( new { message = "Não foi possível criar a categoria"});
            }
        }
        
        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Category>>> Put(
            int id, 
            [FromBody]Category model,
            [FromServices]DataContext context
        )
        {
            if (model.Id != id)
            {
                return NotFound( new {message = "Categoria não encontrada"});
            }
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                //verifica as modificações do modelo, sem precisar cada verificação
                //o EF verifica e persist só o que mudou em relação ao modelo
                context.Entry<Category>(model).State = EntityState.Modified;
                //persistir 
                await context.SaveChangesAsync();//gera o id e retora na model
                return Ok(model);
            }
            catch(DbUpdateConcurrencyException)
            {
                return BadRequest( new { message = "Erro ao alterar registo, outra alteração em andamento"});
            }
            catch(Exception)
            {
                return BadRequest( new { message = "Não foi possível alterar a categoria"});
            };
        }
        
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<List<Category>>> HttpDelete( 
            int id,
            [FromServices]DataContext context
        )
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category ==null)
            {
                return NotFound(new { message = "Categoria não encontrada"});
            }
            try
            {
               context.Categories.Remove(category);
               await context.SaveChangesAsync();
               return Ok( new { message = "Categoria removida"});
            }
            catch (Exception)
            {
                return BadRequest( new { message = "Não foi remover a categoria"});
            }

        }
        
    }
}
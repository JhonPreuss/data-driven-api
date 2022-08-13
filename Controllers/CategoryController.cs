using Microsoft.AspNetCore.Mvc;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("categories")]
    public class CategoryController :ControllerBase
    {
        [HttpGet]//Decorar o método com o verbo da requisição
        [Route("")]
        public string Get()
        {
            return "Opa! Get";
        }
        
        [HttpGet]
        [Route("{id:int}")]//restrição na rota
        public string GetById(int id)
        {
            return "Opa! Get - " + id.ToString();
        }

        [HttpPost]
        [Route("")]
        public Category Post([FromBody]Category model)
        //public string Post(Category model)
        {
            return model;
        }
        
        [HttpPut]
        [Route("{id:int}")]
        public Category Put(int id, [FromBody]Category model)
        {
            if (model.Id == id)
            {
                return model;
            }
            return null;
        }
        
        [HttpDelete]
        [Route("{id:int}")]
        public string HttpDelete( int id)
        {
            return "Opa! Delete";
        }
        
    }
}
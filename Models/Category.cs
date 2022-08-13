using System.ComponentModel.DataAnnotations;//Criar anotações nas entidades
//Aqui posso manipular os schemas, tipo de dados e colunas com data annotations
namespace Shop.Models
{
    public class Category
    {
        [Key] //Exemplo de anotação
        public int Id {get; set;}
        
        [Required(ErrorMessage ="Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage ="Este campo deve conter entre 3 e 60 caractéres")]
        [MinLength(3, ErrorMessage ="Este campo deve conter entre 3 e 60 caractéres")]
        public string? Title {get; set;}
    }

}
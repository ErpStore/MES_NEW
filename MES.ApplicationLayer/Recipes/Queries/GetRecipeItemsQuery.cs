using MediatR;
using MES.ApplicationLayer.Recipes.Dtos;

namespace MES.ApplicationLayer.Recipes.Queries
{
    public class GetRecipeItemsQuery : IRequest<List<RecipeItemDto>>
    {
        public int RecipeId { get; set; }
    }
}

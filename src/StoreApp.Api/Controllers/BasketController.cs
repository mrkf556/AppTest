using Microsoft.AspNetCore.Mvc;
using StoreApp.Api.Results;
using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.Results;
using StoreApp.Application.Basket.AddItemToBasket;
using StoreApp.Application.Basket.ClearBasket;
using StoreApp.Application.Basket.DTOs;
using StoreApp.Application.Basket.GetOrCreateBasket;
using StoreApp.Application.Basket.RemoveBasketItem;
using StoreApp.Application.Basket.UpdateBasketItemQuantity;

namespace StoreApp.Api.Controllers
{
    [Route("api/v1/basket")]
    public sealed class BasketController : BaseController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public BasketController(ICommandDispatcher commandDispatcher,IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet("{userId:long}")]
        public async Task<ActionResult<ApiResult<BasketDTO>>> GetOrCreateBasket(
            long userId,
            CancellationToken cancellationToken)
        {
            var query = new GetOrCreateBasketQuery(userId);

            var result = await _queryDispatcher.Send(query,cancellationToken);

            return this.ToApiResult(result);
        }

        [HttpPost("{userId:long}/items")]
        public async Task<ActionResult<ApiResult>> AddItem(long userId,[FromBody] AddBasketItemDTO item,CancellationToken cancellationToken)
        {
            var command = new AddItemToBasketCommand(userId,item);

            var result = await _commandDispatcher.Send(command,cancellationToken);

            return this.ToApiResult(result);
        }

        [HttpPut("{userId:long}/items/{productId:long}")]
        public async Task<ActionResult<ApiResult>> UpdateQuantity(long userId,long productId,[FromBody] int newQuantity,CancellationToken cancellationToken)
        {
            var command = new UpdateBasketItemQuantityCommand(userId,productId,newQuantity);

            var result = await _commandDispatcher.Send(command,cancellationToken);

            return this.ToApiResult(result);
        }

        [HttpDelete("{userId:long}/items/{productId:long}")]
        public async Task<ActionResult<ApiResult>> RemoveItem(long userId,long productId,CancellationToken cancellationToken)
        {
            var command = new RemoveBasketItemCommand(userId,productId);
    //        var reFsult = await _commandDispatcher.Send<ServiceResult>(
    //command,
    //cancellationToken);
            var result = await _commandDispatcher.Send(command,cancellationToken);

            return this.ToApiResult(result);
        }

        [HttpDelete("{userId:long}")]
        public async Task<ActionResult<ApiResult>> ClearBasket(long userId,CancellationToken cancellationToken)
        {
            var command = new ClearBasketCommand(userId);

            var result = await _commandDispatcher.Send(command,cancellationToken);

            return this.ToApiResult(result);
        }
    }
}
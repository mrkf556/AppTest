using Moq;
using StoreApp.Application.Abstractions.Redis;
using StoreApp.Application.Basket.AddItemToBasket;
 using StoreApp.Infrastructure.Persistence;
using StoreApp.Domain.Enitities;
using Xunit;
using StoreApp.Application.Abstractions.Contracts;
using BasketEntity = StoreApp.Domain.Enitities.Basket;
using StoreApp.Application.Abstractions.DTOs;
namespace StoreApp.UnitTests.Basket
{
    public class AddItemToBasketCommandHandlerTests
    {
        private readonly Mock<IBasketRepository> _basketRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IBasketCacheService> _basketCacheServiceMock;

        private readonly AddItemToBasketCommandHandler _handler;

        public AddItemToBasketCommandHandlerTests()
        {
            _basketRepositoryMock = new Mock<IBasketRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _basketCacheServiceMock = new Mock<IBasketCacheService>();

            _handler = new AddItemToBasketCommandHandler(_basketRepositoryMock.Object,_unitOfWorkMock.Object,_basketCacheServiceMock.Object);
        }
        //// تست موفقیت افزودن کالا و حذف کش

        [Fact]
        public async Task Handle_ShouldAddItem_AndRemoveCache()
        {
            // Arrange

            var userId = 1L;

            var basket = new BasketEntity(userId);

            var command = new AddItemToBasketCommand(userId,new AddBasketItemDTO
                {
                    ProductId = 10,
                    Quantity = 2
                });

            _basketRepositoryMock.Setup(x => x.GetActiveBasketByUserIdAsync(userId,It.IsAny<CancellationToken>())).ReturnsAsync(basket);

            _basketCacheServiceMock.Setup(x => x.RemoveAsync(userId,It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act

            var result = await _handler.Handle(command,CancellationToken.None);

            // Assertt

            Assert.NotNull(result);
            // سبد دقیقاً یک آیتم داشته باشد
            Assert.Single(basket.Items);

            var item = basket.Items.First();

            Assert.Equal(10, item.ProductId);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(1_000_000m, item.UnitPrice);

            _basketCacheServiceMock.Verify(
                x => x.RemoveAsync(
                    userId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
        //// تست شکست در صورت عبور تعداد کالا از حداکثر مجاز
        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenQuantityExceedsMaximum()
        {
            // Arrange
            var userId = 1L;

            var basket = new BasketEntity(userId);

            var command = new AddItemToBasketCommand(
                userId,
                new AddBasketItemDTO
                {
                    ProductId = 10,
                    Quantity = 11
                });

            _basketRepositoryMock
                .Setup(x => x.GetActiveBasketByUserIdAsync(
                    userId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(basket);

            // Act
            var result = await _handler.Handle(command,CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.False(result.IsSuccess);

            _basketCacheServiceMock.Verify(
                x => x.RemoveAsync(
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
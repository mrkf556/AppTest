using StoreApp.Application.Basket.Events;
using StoreApp.Domain.Enums;
using StoreApp.Domain.Events.Basket;
using StoreApp.Domain.Events.DomainEvent;
namespace StoreApp.Domain.Enitities
{
    // این کلاس Aggregate Root مربوط به سبد خزید است.
    // آیتم‌های سبد و اجرای قوانین کسب‌وکار  است
    //وظیفه کنسلل کردن شبد را هندل میکند

    public class Basket
    {
        private const int MaxItemQuantity = 10;
        private const decimal MaxBasketPrice = 50_000_000m;
        /// <summary>
        /// Relationship
        /// </summary>
        /// ////این خیلی مهم استتت چون در DDD می‌خواهیم تغیرات Basket از طریق قواانین خود Basket انجامز شود.
        /// 


        ///به دلیل اینکه خود مدل کسب کار عملیات addd به basketitem را انجام میدهد از بیرون این امکان گرفته می شود و به خود مدل کسب کار داده می شود و فقط میتوان از بیرون ان را خواند 
        private readonly List<BasketItem> _items = new();
        /// <summary>
        /// راهی است که بیرون از Aggregate بتواند آیتم‌های Basket را بخواند بدون اینکه بتواند مستقیم List را تغییر دهد.
        /// </summary>
        /// //////اگر می‌خواهی آیتم‌های Basket را ببینی، می‌توانی بخوانی؛ ولی اجازه تغییر مستقیم نداری
        public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();
        public long Id { get; private set; }

        public long UserId { get; private set; }
        //
        public BasketStatus Status { get; private set; }
   

        public DateTime CreatedAt { get; private set; }

        public DateTime? LastUpdatedAt { get; private set; }
        /// <summary>
        /// دامین event ها را به transaction بشناسونیم
        /// </summary>
        private readonly List<IDomainEvent> _domainEvents = new();
        /// <summary>
        /// Eventهای داخل Basket چی هستند؟
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
       

        private Basket()
        {
        }

        public Basket(long userId)
        {
            UserId = userId;
            Status = BasketStatus.Active;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddItem(long productId,int quantity,decimal unitPrice)
        {
            ///قبل از اضافه کردن محصول به basket

            ////check active or deactive basket
            EnsureBasketIsActive();
            ///عددی صحیح برای تعداد
            ValidateQuantity(quantity);
            ////صحیح بودن مقدار قیمتز محصول
            ValidatePrice(unitPrice);

            var existingItem = _items
                .FirstOrDefault(x => x.ProductId == productId);

            if (existingItem is not null)
            {


                var newQuantity = existingItem.Quantity + quantity;

                ValidateQuantity(newQuantity);

                var newTotalPrice =
    GetTotalPrice()
    - (existingItem.Quantity * existingItem.UnitPrice)
    + (newQuantity * unitPrice);

                ValidateBasketTotal(newTotalPrice);


                existingItem.UpdateQuantity(   newQuantity,    unitPrice);
            }
            else
            {



                var newTotalPrice = GetTotalPrice() + (quantity * unitPrice);

                ValidateBasketTotal(newTotalPrice);

                _items.Add( new BasketItem(productId,quantity,unitPrice));
         
            
            }

            LastUpdatedAt = DateTime.UtcNow;
            _domainEvents.Add(new BasketItemAddedEvent(Id,UserId,productId,quantity));
        }

        public void UpdateQuantity( long productId, int newQuantity, decimal unitPrice)
        {
            EnsureBasketIsActive();

            ValidateQuantity(newQuantity);
            ValidatePrice(unitPrice);

            var item = _items .FirstOrDefault(x => x.ProductId == productId);


            if (item is null)
                throw new InvalidOperationException( $"Product with id {productId}  not exist in basket");



            var newTotalPrice =GetTotalPrice()- (item.Quantity * item.UnitPrice)+ (newQuantity * unitPrice);

            ValidateBasketTotal(newTotalPrice);



            item.UpdateQuantity( newQuantity, unitPrice);



            LastUpdatedAt = DateTime.UtcNow;
        
        }
        //به طور کلی متدهایی به شکل زیر که انجام شده است رویه لیست دیتا تغییرات انحام میشه و بعد توسط unitOfWork ثبت میشه
        public void RemoveItem(long productId)
        {
            EnsureBasketIsActive();

            var item = _items
                .FirstOrDefault(x => x.ProductId == productId);

            if (item is null)
                return;

            _items.Remove(item);

            LastUpdatedAt = DateTime.UtcNow;
        }

        public void Clear()
        {

            EnsureBasketIsActive();

            _items.Clear();

            LastUpdatedAt = DateTime.UtcNow;
        }

        ///یرای event
        public void Expire()
        {

            if (Status == BasketStatus.Expired)
            {
                return;
            }
            Status = BasketStatus.Expired;


            LastUpdatedAt = DateTime.UtcNow;
            //رویداد برای  منقضی شدن بسکت
            _domainEvents.Add(  new BasketExpiredEvent(Id,UserId));
        }

        public decimal GetTotalPrice()
        {

            return _items.Sum(  x => x.Quantity * x.UnitPrice);
     
        
        }

        private void EnsureBasketIsActive()
        {

            if (Status != BasketStatus.Active)
            {
                throw new InvalidOperationException( "Basket is expired");
            
            }
        }

        private static void ValidateQuantity(int quantity)
        {


            if (quantity <= 0)
            {

                throw new ArgumentException( "Quantity must be greater than zero");
            }
            if (quantity > MaxItemQuantity)
            {
                throw new InvalidOperationException( $"Maximum quantity of each product is {MaxItemQuantity}");
            }
        }

        private static void ValidatePrice(decimal unitPrice)
        {
            if (unitPrice < 0)
            {

                throw new ArgumentException("Unit price cannot be negative");
            }
        }
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        private static void ValidateBasketTotal(decimal totalPrice)
        {

            if (totalPrice > MaxBasketPrice)
            {
                throw new InvalidOperationException( $"Basket total price cannot exceed {MaxBasketPrice:N0} IRR" );
            
            
            }
        }


   
    }
}

using Library.Models;
using Newtonsoft.Json;

namespace Library.Helpers
{
    public static class CartHelper
    {
        private const string CartSessionKey = "Cart";

        public static List<CartItem> GetCart(ISession session)
        {
            var cart = session.GetString(CartSessionKey);
            return cart == null ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cart);
        }

        public static void SaveCart(ISession session, List<CartItem> cart)
        {
            session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }

        public static void AddToCart(ISession session, CartItem item)
        {
            var cart = GetCart(session);
            cart.Add(item);
            SaveCart(session, cart);
        }

        public static void RemoveFromCart(ISession session, int bookId)
        {
            var cart = GetCart(session);
            var itemToRemove = cart.Find(x => x.BookId == bookId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
            }
            SaveCart(session, cart);
        }

        public static void ClearCart(ISession session)
        {
            session.Remove(CartSessionKey);
        }
    }
}

namespace vk.Models.VkApi {
   public class AccessToken {
      public string Token { get; set; }
      public int UserID { get; set; }

      public void Set(AccessToken other) {
         Token = other.Token;
         UserID = other.UserID;
      }
   }
}
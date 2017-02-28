namespace vk.Models.VkApi.Entities {
   public interface IPost {
      string Message { get; set; }
      int OwnerId { get; set; }
      int Date { get; set; }
   }
}
using JetBrains.Annotations;

namespace vk.Models.VkApi {
   [UsedImplicitly]
   public class VkApiProvider {
      public AccessToken Token { get; private set; }
      public GroupsGet GroupsGet { get; private set; }
      public GroupsGetById GroupsGetById { get; private set; }
      public PhotosGetWallUploadSever PhotosGetWallUploadSever { get; private set; }
      public PhotosSaveWallPhoto PhotosSaveWallPhoto { get; private set; }
      public StatsTrackVisitor StatsTrackVisitor { get; private set; }
      public UsersGet UsersGet { get; private set; }
      public WallGet WallGet { get; private set; }
      public WallPost WallPost { get; private set; }

      public VkApiProvider(AccessToken token, VkApiBase api) {
         Token = token;
         GroupsGet = new GroupsGet(api);
         GroupsGetById = new GroupsGetById(api);
         PhotosGetWallUploadSever = new PhotosGetWallUploadSever(api);
         PhotosSaveWallPhoto = new PhotosSaveWallPhoto(api);
         StatsTrackVisitor = new StatsTrackVisitor(api);
         UsersGet = new UsersGet(api);
         WallGet = new WallGet(api);
         WallPost = new WallPost(api);
      }
   }
}
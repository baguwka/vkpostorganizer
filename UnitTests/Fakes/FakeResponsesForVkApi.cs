using System.Collections.Generic;
using vk.Models.VkApi;
using vk.Models.VkApi.Entities;

namespace UnitTests.Fakes {
   public static class FakeResponsesForVkApi {
      public static UploadServerInfo ForPhotosGetWallUploadServer() {
         return new UploadServerInfo {
            AlbumId = 1,
            UploadUrl = "http://upload.uri/",
            UserId = 1
         };
      }

      public static PhotosSaveWallPhotoResponse ForPhotosSaveWallPhoto() {
         return new PhotosSaveWallPhotoResponse {
            Content = new List<Photo> {
               new Photo {
                  Id = 1,
                  OwnerId = 1,
                  Date = 140000000,
                  Photo1280 = "1280px_photo.jpg",
                  Text = "text"
               }
            }
         };
      }

      public static UsersGetResponse ForUsersGet() {
         return new UsersGetResponse {
            Users = new List<User> {
               new User {
                  ID = 1,
                  FirstName = "Mark",
                  LastName = "Anderson",
                  Photo200 = "200px_photo.jpg",
                  Photo50 = "50px_photo.jpg"
               }
            }
         };
      }

      public static GroupsGetResponse ForGroupsGet() {
         return new GroupsGetResponse { 
            Content = new GroupGetCollection {
               Count = 2,
               Groups = new List<Group> {
                  new Group {
                     ID = 1,
                     Name = "Glorious fake group #1",
                     Description = "We're top fake group, join us please",
                     Photo200 = "200px_photo.jpg",
                     Photo50 = "50px_photo.jpg"
                  },

                  new Group {
                     ID = 2,
                     Name = "Glorious fake group #2",
                     Description = "We're top 2 fake group, join us please, we want be first :C",
                     Photo200 = "200px_photo.jpg",
                     Photo50 = "50px_photo.jpg"
                  }
               }
            }
         };
      }
   }
}
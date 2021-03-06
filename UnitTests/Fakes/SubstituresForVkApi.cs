﻿using System.Threading;
using NSubstitute;
using vk.Models.VkApi;

namespace UnitTests.Fakes {
   public static class SubstituresForVkApi {
      public static IUsersGet UsersGet() {
         var usersGet = Substitute.For<IUsersGet>();
         var fakeUserResponse = FakeResponsesForVkApi.ForUsersGet();
         usersGet.GetAsync(QueryParameters.No()).ReturnsForAnyArgs(fakeUserResponse);
         usersGet.GetAsync(QueryParameters.No(), CancellationToken.None).ReturnsForAnyArgs(fakeUserResponse);
         return usersGet;
      }

      public static IGroupsGet GroupsGet() {
         var groupsGet = Substitute.For<IGroupsGet>();
         var fakeGroupResponse = FakeResponsesForVkApi.ForGroupsGet();
         groupsGet.GetAsync(QueryParameters.No()).ReturnsForAnyArgs(fakeGroupResponse);
         groupsGet.GetAsync(QueryParameters.No(), CancellationToken.None).ReturnsForAnyArgs(fakeGroupResponse);
         return groupsGet;
      }

      public static IPhotosGetWallUploadSever PhotosGetWallUploadSever() {
         var photosGetWallUploadServer = Substitute.For<IPhotosGetWallUploadSever>();
         var fakeResponse = FakeResponsesForVkApi.ForPhotosGetWallUploadServer();
         photosGetWallUploadServer.GetAsync(0).ReturnsForAnyArgs(fakeResponse);
         photosGetWallUploadServer.GetAsync(0, CancellationToken.None).ReturnsForAnyArgs(fakeResponse);
         return photosGetWallUploadServer;
      }

      public static IPhotosSaveWallPhoto PhotosSaveWallPhoto() {
         var photosSaveWallPhoto = Substitute.For<IPhotosSaveWallPhoto>();
         var fakeResponse = FakeResponsesForVkApi.ForPhotosSaveWallPhoto();
         photosSaveWallPhoto.SaveAsync(0, "uploadResponse").ReturnsForAnyArgs(fakeResponse);
         photosSaveWallPhoto.SaveAsync(0, "uploadResponse", CancellationToken.None).ReturnsForAnyArgs(fakeResponse);
         return photosSaveWallPhoto;
      }
   }
}
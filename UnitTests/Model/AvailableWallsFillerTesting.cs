using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using UnitTests.Fakes;
using vk.Models;
using vk.Models.VkApi;

namespace UnitTests.Model {
   [TestFixture]
   public class AvailableWallsFillerTesting {
      [Test]
      public async Task Fill_empty_wall_list_with_fake_data() {
         var usersGet = SubstituresForVkApi.UsersGet();
         var groupsGet = SubstituresForVkApi.GroupsGet();
         var wallList = new WallList();

         var filler = new AvailableWallsFiller(usersGet, groupsGet);
         var info = await filler.FillAsync(wallList);
         Assert.That(info.Succeed, Is.True);
         Assert.That(wallList.Items.Count, Is.EqualTo(3));
      }

      [Test]
      public async Task Fill_empty_wall_list_with_corrupted_groups_data() {
         var usersGet = SubstituresForVkApi.UsersGet();
         var groupsGet = Substitute.For<IGroupsGet>();
         groupsGet.GetAsync(VkParameters.No()).ReturnsForAnyArgs(new GroupsGetResponse {
            Collection = null
         });

         var wallList = new WallList();

         var filler = new AvailableWallsFiller(usersGet, groupsGet);
         var info = await filler.FillAsync(wallList);

         Assert.That(info.Succeed, Is.False);
         Assert.That(info.ErrorMessage, Is.Not.Null);
      }

      [Test]
      public async Task Fill_empty_wall_list_with_corrupted_users_data() {
         var usersGet = Substitute.For<IUsersGet>();
         usersGet.GetAsync().Returns(new UsersGetResponse {
            Users = null
         });

         var groupsGet = SubstituresForVkApi.GroupsGet();
         var wallList = new WallList();

         var filler = new AvailableWallsFiller(usersGet, groupsGet);
         var info = await filler.FillAsync(wallList);

         Assert.That(info.Succeed, Is.False);
         Assert.That(info.ErrorMessage, Is.Not.Null);
      }
   }
}
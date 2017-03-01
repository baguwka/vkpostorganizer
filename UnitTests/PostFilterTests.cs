using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using vk.Models;
using vk.Models.Filter;

namespace UnitTests {
   internal class FakePost : IPostType {
      public FakePost(PostType postType) {
         PostType = postType;
      }

      public FakePost() {
      }

      public PostType PostType { get; set; }
   }

   [TestFixture]
   public class PostFilterTests {
      [Test]
      public void Composite_post_filter_test() {
         var compositePostFilter = new CompositePostFilter(PostType.Repost | PostType.Missing);

         var postList = new List<IPostType> {
            new FakePost(PostType.Missing),
            new FakePost(PostType.Missing),
            new FakePost(PostType.Post),
            new FakePost(PostType.Post),
            new FakePost(PostType.Post),
            new FakePost(PostType.Missing),
            new FakePost(PostType.Repost),
            new FakePost(PostType.Missing),
            new FakePost(PostType.Post)
         };

         var filteredList = postList.Where(compositePostFilter.Suitable).ToList();
         Assert.That(filteredList.Count(), Is.EqualTo(5));
         Assert.That(filteredList.Any(p => p.PostType == PostType.Post), Is.False);
         Assert.That(filteredList.Count(p => p.PostType == PostType.Missing), Is.EqualTo(4));
         Assert.That(filteredList.Count(p => p.PostType == PostType.Repost), Is.EqualTo(1));
         //compositePostFilter.FilterPosts();
      }
   }
}
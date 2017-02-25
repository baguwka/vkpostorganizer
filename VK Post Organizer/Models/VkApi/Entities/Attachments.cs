using System.Collections.Generic;
using System.Linq;

namespace vk.Models.VkApi.Entities {
   public class Attachments {
      private readonly IList<Attachment> _attachments;

      public IEnumerable<Attachment> Exposed => _attachments;

      public static Attachments No() {
         return new Attachments();
      }

      public Attachments() {
         _attachments = new List<Attachment>();
      }

      public bool Any() {
         return _attachments.Any();
      }

      public Attachments Add(Attachment attachment) {
         _attachments.Add(attachment);
         return this;
      }

      public VkParameters GetAsParameters() {
         var parameters = VkParameters.New();
         if (Any()) {
            parameters.AddParameter("attachments", ToString());
         }
         return parameters;
      }

      public override string ToString() {
         var attachmentsString = "";

         if (Any()) {
            attachmentsString = string.Join(",", _attachments);
         }

         return attachmentsString;
      }
   }
}
using Prism.Events;

namespace vk.Events {
   public class AuthBarEvents {
      /// <summary>
      /// Raised upon LogOut click
      /// </summary>
      public class LogOutRequest : PubSubEvent { }

      /// <summary>
      /// Raised upon Authorize click
      /// </summary>
      public class AuthorizeRequest : PubSubEvent { }


      /// <summary>
      /// Raised upon LogOut is completed
      /// </summary>
      public class LogOutCompleted : PubSubEvent { }

      /// <summary>
      /// Raised upon Authorization completed
      /// </summary>
      public class AuthorizationCompleted : PubSubEvent<bool> { }


      /// <summary>
      /// Send a request to AuthBar to try to authorize, if user already loggined in before
      /// </summary>
      public class AuthorizeIfAlreadyLoggined : PubSubEvent { }
   }
}
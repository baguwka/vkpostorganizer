using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using vk.Models;

namespace vk.ViewModels {
   public abstract class PostViewModelBase : BindableBase, IPostType {
      private bool _expanded;
      private PostType _postType;
      private ObservableCollection<ImageItem> _previewImages;
      private bool _canExpand;

      public bool Expanded {
         get { return _expanded; }
         set { SetProperty(ref _expanded, value); }
      }

      public bool CanExpand {
         get { return _canExpand; }
         set { SetProperty(ref _canExpand, value); }
      }

      public PostType PostType {
         get { return _postType; }
         set { SetProperty(ref _postType, value); }
      }

      public ObservableCollection<ImageItem> PreviewImages {
         get { return _previewImages; }
         set { SetProperty(ref _previewImages, value); }
      }

      public ICommand ExpandToggleCommand { get; set; }

      public PostViewModelBase() {
         PreviewImages = new ObservableCollection<ImageItem>();

         ExpandToggleCommand = new DelegateCommand(expandToggle,
               () => PostType != PostType.Missing)
            .ObservesProperty(() => PostType);
      }

      protected virtual void loadPreviews() {
      }

      protected virtual void expandToggle() {
         if (!CanExpand) {
            Expanded = false;
            return;
         }

         Expanded = !Expanded;
      }

      public virtual void Expand() {
         if (!CanExpand) {
            Expanded = false;
            return;
         }

         Expanded = true;
      }

      public virtual void Collapse() {
         if (!CanExpand) {
            Expanded = false;
            return;
         }

         Expanded = false;
      }

      public virtual void ClearPreview() {
         PreviewImages.Clear();
      }
   }
}
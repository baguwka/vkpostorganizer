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

      public bool Expanded {
         get { return _expanded; }
         set { SetProperty(ref _expanded, value); }
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
         Expanded = !Expanded;
      }

      public virtual void Expand() {
         Expanded = true;
      }

      public virtual void Collapse() {
         Expanded = false;
      }

      public virtual void ClearPreview() {
         PreviewImages.Clear();
      }
   }
}
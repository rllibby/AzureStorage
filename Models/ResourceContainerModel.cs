/*
 *  Copyright © 2016, Russell Libby 
 */

using System;
using Template10.Mvvm;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace AzureStorage.Models
{
    /// <summary>
    /// Container class for Azure table, queue, and blob container.
    /// </summary>
    public class ResourceContainerModel : BindableBase
    {
        #region Private fields

        private readonly ResourceContainersModel _parent;
        private DelegateCommand _add;
        private ContainerType _type;
        private bool _selectionMode;
        private bool _selected;
        private string _name;
        private string _uri;

        #endregion

        #region Private methods

        /// <summary>
        /// Event handler to notify when added.
        /// </summary>
        public event EventHandler OnAdd;

        /// <summary>
        /// Determines if the resource  can be added.
        /// </summary>
        /// <returns></returns>
        private bool CanAddResource()
        {
            return !string.IsNullOrEmpty(_name);
        }

        /// <summary>
        /// Action called when adding a new resource
        /// </summary>
        private void AddResource()
        {
            OnAdd?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Construtor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourceContainerModel()
        {
            _add = new DelegateCommand(new Action(AddResource), CanAddResource);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent">The owner of the item</param>
        public ResourceContainerModel(ResourceContainersModel parent) : this()
        {
            if (parent == null) throw new ArgumentNullException("parent");

            _parent = parent;
            _selectionMode = parent.GetSelectionMode();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent">The owner of the item</param> 
        /// <param name="type">The container type.</param>
        public ResourceContainerModel(ResourceContainersModel parent, ContainerType type) : this(parent)
        {
            _type = type;
        }

        #endregion

        #region Public properties

        /// <summary>
        /// The comamnd action for adding a resource.
        /// </summary>
        public DelegateCommand Add
        {
            get { return _add; }
        }

        /// <summary>
        /// The name of the resource.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                _add.RaiseCanExecuteChanged();
                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The resource container type.
        /// </summary>
        public ContainerType ResourceType
        {
            get { return _type; }
            set
            {
                _type = value;

                RaisePropertyChanged("NewDescription");
                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Returns the description for adding new resource.
        /// </summary>
        public string NewDescription
        {
            get
            {
                switch (_type)
                {
                    case ContainerType.BlobContainer:
                        return "Blob Container Name";

                    case ContainerType.Queue:
                        return "Queue Name";

                    default:
                        return "Table Name";
                }
            }
        }

        /// <summary>
        /// The primary uri for the resource.
        /// </summary>
        public string Uri
        {
            get { return _uri; }
            set
            {
                _uri = value;

                base.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The selected state of the item.
        /// </summary>
        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    if (_parent != null) _parent.Selected += (_selected ? 1 : -1);

                    base.RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Determines if the user interface will allow the item to be selected.
        /// </summary>
        public bool SelectionMode
        {
            get { return _selectionMode; }
            set
            {
                _selectionMode = value;

                base.RaisePropertyChanged();
            }
        }
        
        /// <summary>
        /// Image source for container.
        /// </summary>
        public ImageSource ResourceImage
        {
            get
            {
                return new BitmapImage(new Uri(string.Format("ms-appx:///Assets/{0}.png", _type.ToString().ToLower())));
            }
        }

        #endregion
    }
}
